using Discord;
using Discord.WebSocket;
using DiscordBot.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DiscordBot.Plugin.Weather
{
    //ICommand の実装
    public class DiscordBot_Weather : ICommand, ICacheClearProvider
    {
        private readonly ILogger _logger;
        //HttpClient は静的かつシングルトンとして扱うのが推奨されるため、static readonly に変更。
        //これにより、ソケット枯渇の問題を回避し、リソースを効率的に再利用できます。
        private static readonly HttpClient _httpClient = new HttpClient();

        //レジストリから読み込んだ API Key を保持するフィールド
        private readonly string _apiKey;

        public string CommandName => "wn";
        public string PluginName => "天気予報(DiscordBot_Weather)";
        public string PluginVersion => "1.1.0.0";
        public string PluginDLL => "DiscordBot.Plugin.Weather.dll";

        // キャッシュ格納用：キー=正規化された都市名, 値=キャッシュデータ
        private readonly Dictionary<string, WeatherCacheEntry> _weatherCache = new Dictionary<string, WeatherCacheEntry>();
        // レジストリから読み込んだキャッシュ持続時間 (分)
        private readonly int _cacheDurationMinutes;

        public DiscordBot_Weather(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger), $"[{PluginName}(DLLログ)] Loggerインスタンスはnullにできません!!");
            _logger.Log("----------------------------------------------------", (int)LogType.Success);
            _logger.Log($"[{PluginName}(DLLログ)] ILoggerの初期化に成功!!", (int)LogType.Success);

            //レジストリからキャッシュ時間を読み込み
            try
            {
                //RegistryHelperを使って設定値を読み込む
                //RegistryHelper.csに LoadWeatherCacheDurationSetting が追加されている前提
                _cacheDurationMinutes = RegistryHelper.LoadWeatherCacheDurationSetting();
                _logger.Log($"[{PluginName}(DLLログ)] 天気予報データのキャッシュ時間：[{_cacheDurationMinutes} 分]", (int)LogType.Success);
                _apiKey = RegistryHelper.LoadWeatherApiKeySetting();
                _logger.Log($"[{PluginName}(DLLログ)] APIキーの取得に成功!!", (int)LogType.Success);
                if (string.IsNullOrWhiteSpace(_apiKey))
                {
                    _logger.Log($"[{PluginName}(DLLログ)] APIキーが設定されていません!!天気予報コマンドは機能しません!!", (int)LogType.Error);
                    _logger.Log($"[{PluginName}(DLLログ)] [プラグイン]⇒[天気予報]から設定を確認してください!!", (int)LogType.Error);
                    //検証用
                    //_apiKey = "dd5aa823e5737e1d0120245bb26a168e"; 
                }
            }
            catch (Exception ex)
            {
                //読み込みに失敗した場合のフォールバック (RegistryHelperで設定されたデフォルト値に合わせる)
                _cacheDurationMinutes = 30;
                _logger.Log($"[{PluginName}(DLLログ)] 天気予報データのキャッシュ読み込みに失敗しました!!デフォルト値：[{_cacheDurationMinutes} 分] を使用します!!\nエラー：{ex.Message}", (int)LogType.Error);
            }
        }
        //全ての天気情報キャッシュをクリア(WeatherInfoSettingフォームから呼び出すために公開)
        public int ClearCache()
        {
            try
            {
                int count = _weatherCache.Count;
                _weatherCache.Clear();
                _logger.Log($"[{PluginName}(DLLログ)] 全ての天気情報キャッシュをクリアしました!!", (int)LogType.Debug);
                _logger.Log($"[{PluginName}(DLLログ)] クリアされたキャッシュエントリ数：[{count} 個]", (int)LogType.Debug);

                //削除件数を返します
                return count;
            }
            catch (Exception ex)
            {
                _logger.Log($"[{PluginName}(DLLログ)] キャッシュデータの削除中にエラーが発生しました!!\n{ex.Message}", (int)LogType.DebugError);
                return 0;
            }
        }

        public async Task<MessageResponse> ExecuteInitialAsync(SocketMessage message, string fullCommandText)
        {
            //自動削除の有効/無効と遅延時間を読み込む
            bool shouldDelete = await Task.Run(() => RegistryHelper.LoadWeatherShouldDelete());
            int deleteDelayMs = await Task.Run(() => RegistryHelper.LoadWeatherDeleteDelayMs());
            try
            {
                //コマンド引数から都市名を取得（例："!wn 東京" -> "東京"）
                //Trim() を使用して、前後の空白を除去
                var parts = fullCommandText.Trim().Split(new char[] { ' ' }, 2);
                string city = (parts.Length > 1) ? parts[1].Trim() : string.Empty;

                if (string.IsNullOrWhiteSpace(city))
                {
                    _logger.Log($"[{PluginName}(DLLログ)] [引数エラー] 都市名を指定してください!!", (int)LogType.DebugError);
                    var failureEmbed = new EmbedBuilder()
                        .WithTitle("❌ [引数エラー]")
                        .WithDescription($"都市名を指定してください!! ※例：[`!{CommandName} tokyo`]")
                        .WithColor(Color.Red)
                        .Build();

                    return new MessageResponse
                    {
                        Embed = failureEmbed,
                        //自動削除有効/無効フラグ
                        ShouldDelete = shouldDelete,
                        //プロンプトMSG自動削除時間
                        DeleteDelayMs = deleteDelayMs
                    };
                }

                //都市名を正規化（キャッシュキーとして使用するため、大文字・小文字、全角・半角などを統一）
                string normalizedCity = NormalizeCityName(city);

                //キャッシュのチェック
                if (_weatherCache.TryGetValue(normalizedCity, out var cacheEntry) && IsCacheValid(cacheEntry))
                {
                    //キャッシュ残り時間の計算

                    //1．キャッシュの有効期限を計算
                    DateTime expiryTime = cacheEntry.Timestamp.AddMinutes(_cacheDurationMinutes);

                    //2．残りの時間を計算
                    TimeSpan remainingTime = expiryTime - DateTime.UtcNow;

                    //3．表示形式を整形
                    string remainingTimeFormatted;

                    //残り時間が5秒以下の場合、"期限切れ間近"と表示
                    if (remainingTime.TotalSeconds <= 5)
                    {
                        _logger.Log($"[{PluginName}(DLLログ)] 天気予報キャッシュデータ解放まで5秒未満!!", (int)LogType.Debug);
                        remainingTimeFormatted = "期限切れ間近!!";
                    }
                    else
                    {
                        int totalMinutes = (int)remainingTime.TotalMinutes;
                        int seconds = remainingTime.Seconds;

                        if (totalMinutes > 0)
                        {
                            //1分以上ある場合、分と秒で表示
                            remainingTimeFormatted = $"[{totalMinutes}分{seconds}秒]";
                        }
                        else
                        {
                            //1分未満の場合、秒のみで表示
                            remainingTimeFormatted = $"[{remainingTime.TotalSeconds:F0}秒]";
                        }
                    }

                    //ログに残り時間を追加して出力
                    _logger.Log($"[{PluginName}(DLLログ)] {city} のキャッシュが有効です!!", (int)LogType.Debug);
                    _logger.Log($"[{PluginName}(DLLログ)] 天気予報キャッシュデータ解放まで残り：{remainingTimeFormatted}", (int)LogType.Debug);
                    //location と updateTime (キャッシュのタイムスタンプ) を追加
                    return CreateWeatherEmbed(cacheEntry.Response, city, true, cacheEntry.Timestamp);
                }

                //キャッシュが無効または存在しない場合、APIを叩く
                //URLエンコードを考慮して、city変数自体をURLに含める
                string apiUrl = $"http://api.openweathermap.org/data/2.5/weather?q={Uri.EscapeDataString(city)}&appid={_apiKey}&units=metric&lang=ja";
                _logger.Log($"[{PluginName}(DLLログ)] {city} の情報を取得中... API URL：[{apiUrl}]", (int)LogType.Debug);

                using (var response = await _httpClient.GetAsync(apiUrl))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        string jsonContent = await response.Content.ReadAsStringAsync();
                        var weatherResponse = JsonConvert.DeserializeObject<OpenWeatherMapResponse>(jsonContent);

                        //キャッシュを更新
                        _weatherCache[normalizedCity] = new WeatherCacheEntry
                        {
                            Response = weatherResponse,
                            Timestamp = DateTime.UtcNow
                        };

                        _logger.Log($"[{PluginName}(DLLログ)] {city} の情報をキャッシュしました!!", (int)LogType.Debug);
                        //location と updateTime (現在時刻) を追加
                        return CreateWeatherEmbed(weatherResponse, city, false, DateTime.Now);
                    }
                    else
                    {
                        string errorContent = await response.Content.ReadAsStringAsync();
                        _logger.Log($"[{PluginName}(DLLログ)] OpenWeatherMap APIからのエラー：[{response.StatusCode} - {errorContent}]", (int)LogType.Error);
                        //404 Not Found の場合は、都市名が見つからなかったことを伝える
                        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                        {
                            var argumentEmbed = new EmbedBuilder()
                                .WithTitle("🚫 引数エラー")
                                .WithDescription($"都市 `{city}` は見つかりませんでした!! 都市名を確認してください!!")
                                .WithColor(Color.DarkRed)
                                .Build();
                            return new MessageResponse
                            {
                                Embed = argumentEmbed,
                                //自動削除有効/無効フラグ
                                ShouldDelete = shouldDelete,
                                //プロンプトMSG自動削除時間
                                DeleteDelayMs = deleteDelayMs
                            };
                        }
                        var acquisitionEmbed = new EmbedBuilder()
                                .WithTitle("🚫 情報取得エラー")
                                .WithDescription($"都市 `{city}` の天気情報を取得できませんでした!!\n[{response.StatusCode}]")
                                .WithColor(Color.DarkRed)
                                .Build();
                        return new MessageResponse
                        {
                            Embed = acquisitionEmbed,
                            //自動削除有効/無効フラグ
                            ShouldDelete = shouldDelete,
                            //プロンプトMSG自動削除時間
                            DeleteDelayMs = deleteDelayMs
                        };
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                //ネットワーク関連のエラーをキャッチ
                _logger.Log($"[{PluginName}(DLLログ)] [ネットワークエラー] ネットワークエラーが発生しました!!\n{ex.Message}", (int)LogType.DebugError);
                var networkEmbed = new EmbedBuilder()
                    .WithTitle("🚫 ネットワークエラー")
                    .WithDescription($"天気情報の取得中にネットワーク接続エラーが発生しました!!\n[{ex.Message}]")
                    .WithColor(Color.DarkRed)
                    .Build();
                return new MessageResponse
                {
                    Embed = networkEmbed,
                    //自動削除有効/無効フラグ
                    ShouldDelete = shouldDelete,
                    //プロンプトMSG自動削除時間
                    DeleteDelayMs = deleteDelayMs
                };
            }
            catch (JsonException ex)
            {
                //JSONのデシリアライズエラーをキャッチ
                _logger.Log($"[{PluginName}(DLLログ)] JSONデシリアライズエラーが発生しました!!\n{ex.Message}", (int)LogType.DebugError);
                var jsonEmbed = new EmbedBuilder()
                    .WithTitle("🚫 JSONデシリアライズエラー")
                    .WithDescription($"APIからのデータ解析中にエラーが発生しました!!\n[{ex.Message}]")
                    .WithColor(Color.DarkRed)
                    .Build();
                return new MessageResponse
                {
                    Embed = jsonEmbed,
                    //自動削除有効/無効フラグ
                    ShouldDelete = shouldDelete,
                    //プロンプトMSG自動削除時間
                    DeleteDelayMs = deleteDelayMs
                };
            }
            catch (Exception ex)
            {
                _logger.Log($"[{PluginName}(DLLログ)] 予期せぬエラーが発生しました!!\n{ex.Message}", (int)LogType.Error);
                var systemerrorEmbed = new EmbedBuilder()
                    .WithTitle("🚫 予期せぬエラー")
                    .WithDescription($"天気情報の取得中にシステムエラーが発生しました!!\n[{ex.Message}]")
                    .WithColor(Color.DarkRed)
                    .Build();
                return new MessageResponse
                {
                    Embed = systemerrorEmbed,
                    //自動削除有効/無効フラグ
                    ShouldDelete = shouldDelete,
                    //プロンプトMSG自動削除時間
                    DeleteDelayMs = deleteDelayMs
                };
            }
        }

        //キャッシュが有効かどうかを判定
        private bool IsCacheValid(WeatherCacheEntry entry)
        {
            return (DateTime.UtcNow - entry.Timestamp).TotalMinutes < _cacheDurationMinutes;
        }

        //都市名を正規化するためのヘルパー
        private string NormalizeCityName(string city)
        {
            // 全角スペースを半角に、すべてを小文字に変換
            return city.Replace("　", " ").ToLowerInvariant().Trim();
        }

        //Discordに送信する埋め込みメッセージを作成
        private MessageResponse CreateWeatherEmbed(OpenWeatherMapResponse weather, string location, bool isCache, DateTime updateTime)
        {
            //都市名と国コードを結合 (例：東京都 (JP))
            string cityAndCountry = $"{location} ({weather.sys.country})";

            //天気の説明(日本語で最初の要素を使用)
            string weatherDescription = weather.weather[0].description;
            //天気のメインタイプを取得
            string mainWeather = weather.weather[0].main;

            //天気アイコンURLの構築
            //OpenWeatherMap APIのアイコンURL形式を使用
            string iconCode = weather.weather[0].icon;
            string iconUrl = $"http://openweathermap.org/img/w/{iconCode}.png";

            //キャッシュ使用状況に応じたタイトルを作成
            string cacheStatus = isCache ? " [キャッシュデータを使用]" : " [最新APIデータ]";
            //説明文をタイトル直下のコンテンツとして使用
            string description = $"**現在の天気：**`{cityAndCountry}`\n**{weatherDescription}**";

            //日の出/日の入りのUNIXタイムスタンプをDateTimeに変換し、JSTに変換
            DateTime sunriseJst = DateTimeOffset.FromUnixTimeSeconds(weather.sys.sunrise).ToLocalTime().DateTime;
            DateTime sunsetJst = DateTimeOffset.FromUnixTimeSeconds(weather.sys.sunset).ToLocalTime().DateTime;

            //埋め込みメッセージを構築
            var embed = new EmbedBuilder
            {
                Title = $"🏙️ **{weather.name} ({weather.sys.country})** の現在の天気情報です!!{cacheStatus}",

                //天気に応じた色を設定
                Color = GetColorFromWeather(mainWeather),

                Description = description
            };

            //埋め込みメッセージのサムネイルとしてアイコンURLを設定
            embed.WithThumbnailUrl(iconUrl);

            //フィールドデータの成形

            //1．気温/体感温度 (インライン：true)
            string tempValue = $"**{weather.main.temp:F2}°C** (体感: {weather.main.feels_like:F2}°C)";
            embed.AddField(
                name: "🌡️ 気温",
                value: tempValue,
                inline: true);

            //2．湿度/風速 (インライン：true)
            string windAndHumidityValue =
                $"**{weather.main.humidity}%**\n{weather.wind.speed:F2} m/s";
            embed.AddField(
                name: "💧 湿度 / 🍃 風速",
                value: windAndHumidityValue,
                inline: true);

            //3．隙間用の空フィールド (インライン：true)
            embed.AddField("\u200B", "\u200B", true);

            //4．最高/最低気温 (インライン：false - 区切り線代わり)
            string tempMinMaxValue =
                $"`最高: {weather.main.temp_max:F2}°C`\n`最低: {weather.main.temp_min:F2}°C`";
            embed.AddField(
                name: "📈 気温の範囲",
                value: tempMinMaxValue,
                inline: false);

            //5．隙間用の空フィールド (インライン：false - 行の区切りをさらに強調)
            embed.AddField("\u200B", "\u200B", false);

            //6．気圧 (インライン：true)
            embed.AddField(
                name: "🧭 気圧",
                value: $"**{weather.main.pressure} hPa**",
                inline: true);

            //7．日の出/日の入り (インライン：true)
            string sunTimeValue =
                $"**日の出:** `{sunriseJst:HH:mm}`\n**日の入り:** `{sunsetJst:HH:mm}`";
            embed.AddField(
                name: "☀️ 日照時間 (JST)",
                value: sunTimeValue,
                inline: true);

            //8．隙間用の空フィールド (インライン：true)
            embed.AddField("\u200B", "\u200B", true);

            //9．フッターに最終更新日時を記載
            embed.WithFooter(
                text: $"最終更新：{updateTime:yyyy/MM/dd HH:mm:ss JST}",
                iconUrl: null);

            return new MessageResponse
            {
                Embed = embed.Build(),
            };
        }

        //天気情報キャッシュのエントリを格納するための内部クラス
        private class WeatherCacheEntry
        {
            public OpenWeatherMapResponse Response { get; set; }
            public DateTime Timestamp { get; set; }
        }

        //天気コードに基づいて埋め込みメッセージの色を取得
        private Color GetColorFromWeather(string main)
        {
            switch (main.ToLower())
            {
                case "clear":
                    //晴れ：明るい黄色
                    return new Color(0xFEE18B);
                case "clouds":
                    //曇り：薄い灰色
                    return new Color(0xbdc3c7);
                case "rain":
                case "drizzle":
                    //雨：青
                    return new Color(0x3498DB);
                case "thunderstorm":
                    //雷：紫
                    return new Color(0x9b59b6);
                case "snow":
                    //雪：白に近い
                    return new Color(0xecf0f1);
                case "mist":
                case "smoke":
                case "haze":
                case "dust":
                case "fog":
                case "sand":
                case "ash":
                case "squall":
                case "tornado":
                    //その他：暗めの灰色
                    return new Color(0x7f8c8d);
                default:
                    //デフォルト：Discordの青
                    return new Color(0x3498DB);
            }
        }

        //OpenWeatherMap APIのJSONレスポンスを格納するためのクラス
        private class OpenWeatherMapResponse
        {
            public class Weather
            {
                //id は使用しないが、JSON構造に合わせるために残す
                public int id { get; set; }
                public string main { get; set; }
                public string description { get; set; }
                public string icon { get; set; }
            }

            public class Main
            {
                public double temp { get; set; }
                public double feels_like { get; set; }
                public double temp_min { get; set; }
                public double temp_max { get; set; }
                public int pressure { get; set; }
                public int humidity { get; set; }
            }

            public class Wind
            {
                public double speed { get; set; }
                //deg (風向) も追加可能ですが、今回は速度のみで元のコードを維持
            }

            //Sys クラスは、日の出/日の入り情報用として残します
            public class Sys
            {
                public int type { get; set; }
                public long id { get; set; }
                public string country { get; set; }
                public long sunrise { get; set; }
                public long sunset { get; set; }
            }

            public Weather[] weather { get; set; }
            public Main main { get; set; }
            public Wind wind { get; set; }
            public Sys sys { get; set; }
            public string name { get; set; }
        }
    }
}
