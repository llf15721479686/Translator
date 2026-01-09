using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

public class YoudaoTranslatorHelper
{
    // 有道智云API配置
    private const string APP_KEY = "00557bf448f5eef1";
    private const string APP_SECRET = "YtduGCxHdSLbbmh2JbxJk90uqSfbIMlV";
    private const string API_URL = "https://openapi.youdao.com/api";

    private static readonly HttpClient _httpClient = new HttpClient();

    // 有道语言代码映射
    private static readonly Dictionary<string, string> LanguageMap = new Dictionary<string, string>
    {
        {"中文", "zh-CHS"},
        {"英语", "en"},
        {"法语", "fr"},
        {"德语", "de"},
        {"阿拉伯语", "ar"},
        {"俄语", "ru"},
        {"葡萄牙语", "pt"},
        {"泰语", "th"},
        {"西班牙语", "es"},
        {"意大利语", "it"},
        {"印度尼西亚语", "id"},
        {"越南语", "vi"},
        {"马来西亚", "ms"}
    };

    static YoudaoTranslatorHelper()
    {
        _httpClient.Timeout = TimeSpan.FromSeconds(30);
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");
    }

    // 生成签名
    private static string GenerateSign(string input, string salt, string curtime)
    {
        string inputStr = input.Length > 20 ?
            input.Substring(0, 10) + input.Length + input.Substring(input.Length - 10) :
            input;

        string signStr = APP_KEY + inputStr + salt + curtime + APP_SECRET;
        return ComputeSha256(signStr);
    }

    // 计算SHA256
    private static string ComputeSha256(string input)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                builder.Append(bytes[i].ToString("x2"));
            }
            return builder.ToString();
        }
    }

    // 解析有道API返回的JSON结果
    private static string ParseYoudaoResult(string json)
    {
        try
        {
            // 使用简单解析，避免依赖复杂JSON库
            var errorMatch = Regex.Match(json, "\"errorCode\":\"?([0-9]+)\"?");
            if (errorMatch.Success && errorMatch.Groups[1].Value != "0")
            {
                string errorCode = errorMatch.Groups[1].Value;
                return $"有道翻译错误({errorCode}): {GetYoudaoErrorDescription(errorCode)}";
            }

            // 提取翻译结果
            var translationMatch = Regex.Match(json, "\"translation\":\\s*\\[\\s*\"([^\"]+)\"");
            if (translationMatch.Success)
            {
                string translatedText = translationMatch.Groups[1].Value;
                return DecodeUnicodeEscapes(translatedText);
            }

            return "翻译结果为空";
        }
        catch (Exception ex)
        {
            return $"解析有道翻译结果时出错: {ex.Message}";
        }
    }

    // 解码Unicode转义序列
    private static string DecodeUnicodeEscapes(string input)
    {
        return Regex.Replace(input, @"\\u([0-9a-fA-F]{4})", match =>
        {
            string hex = match.Groups[1].Value;
            return ((char)Convert.ToInt32(hex, 16)).ToString();
        });
    }

    // 获取有道错误码描述
    private static string GetYoudaoErrorDescription(string errorCode)
    {
        switch (errorCode)
        {
            case "0": return "成功";
            case "101": return "缺少必填参数";
            case "102": return "不支持的语言类型";
            case "103": return "翻译文本过长";
            case "104": return "不支持的API类型";
            case "105": return "不支持的签名类型";
            case "108": return "应用ID无效";
            case "112": return "API已废弃";
            case "113": return "查询频率超限";
            case "201": return "解密失败";
            case "202": return "签名检验失败";
            case "203": return "访问IP地址不在可访问IP列表";
            case "205": return "请求的接口与应用的平台类型不一致";
            case "206": return "因为时间戳无效导致签名校验失败";
            case "207": return "重放请求";
            case "301": return "辞典查询失败";
            case "302": return "翻译查询失败";
            case "303": return "服务端的其它异常";
            case "401": return "账户已经欠费停";
            case "411": return "访问频率受限，请稍后访问";
            case "412": return "长请求过于频繁，请稍后访问";
            default: return $"未知错误({errorCode})";
        }
    }


    // 异步翻译方法
    // 修改YoudaoTranslatorHelper.cs中的TranslateAsync方法
    public static async Task<string> TranslateAsync(string text, string from, string to, int maxRetries = 2)
    {
        if (string.IsNullOrWhiteSpace(text))
            return string.Empty;

        int retryCount = 0;
        int baseDelay = 1000; // 基础延迟时间(ms)

        while (retryCount <= maxRetries) // 最多重试maxRetries次
        {
            try
            {
                // 获取有道语言代码
                if (!LanguageMap.TryGetValue(from, out string fromCode))
                    throw new ArgumentException($"有道不支持源语言: {from}");

                if (!LanguageMap.TryGetValue(to, out string toCode))
                    throw new ArgumentException($"有道不支持目标语言: {to}");

                // 准备请求参数
                string salt = DateTime.Now.Ticks.ToString();
                string curtime = ((int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds).ToString();
                string sign = GenerateSign(text, salt, curtime);

                // 构建POST数据
                var postData = new Dictionary<string, string>
            {
                {"q", text},
                {"from", fromCode},
                {"to", toCode},
                {"appKey", APP_KEY},
                {"salt", salt},
                {"sign", sign},
                {"signType", "v3"},
                {"curtime", curtime}
            };

                using (var content = new FormUrlEncodedContent(postData))
                using (var response = await _httpClient.PostAsync(API_URL, content).ConfigureAwait(false))
                {
                    response.EnsureSuccessStatusCode();
                    string responseJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    return ParseYoudaoResult(responseJson);
                }
            }
            catch (HttpRequestException ex)
            {
                retryCount++;
                if (retryCount > maxRetries)
                {
                    return $"网络请求失败: {ex.Message}";
                }

                // 指数退避策略：1s, 2s, 4s...
                int delay = baseDelay * (int)Math.Pow(2, retryCount - 1);
                await Task.Delay(delay).ConfigureAwait(false);
            }
            catch (TaskCanceledException)
            {
                // 超时重试
                retryCount++;
                if (retryCount > maxRetries)
                {
                    return "请求超时";
                }

                int delay = baseDelay * retryCount;
                await Task.Delay(delay).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                return $"有道翻译出错: {ex.Message}";
            }
        }

        return "有道翻译失败，请重试";
    }
}