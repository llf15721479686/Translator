using System;
using System.Net.Http;
using System.Text;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Net;

public class BaiduTranslatorHelper
{
    // 百度翻译API的APP ID和密钥
    private const string APP_ID = "20250711002403394";
    private const string SECRET_KEY = "_qnpHJqG3nsng76K8jlg";
    private const string API_URL = "https://fanyi-api.baidu.com/api/trans/vip/translate";

    private static readonly HttpClient _httpClient = new HttpClient();
    private static readonly Random _random = new Random();

    private static readonly Dictionary<string, string> LanguageMap = new Dictionary<string, string>
    {
        {"中文", "zh"}, {"英语", "en"}, {"法语", "fra"}, {"德语", "de"},
        {"阿拉伯语", "ara"}, {"俄语", "ru"}, {"葡萄牙语", "pt"}, {"泰语", "th"},
        {"西班牙语", "spa"}, {"意大利语", "it"}, {"印度尼西亚语", "id"},
        {"越南语", "vie"}, {"马来西亚","ms"}
    };

    static BaiduTranslatorHelper()
    {
        // 配置HttpClient
        _httpClient.Timeout = TimeSpan.FromSeconds(30);
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");
    }

    private static string ComputeMd5(string input)
    {
        using (MD5 md5 = MD5.Create())
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(input);
            byte[] hashBytes = md5.ComputeHash(inputBytes);

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
            {
                sb.Append(hashBytes[i].ToString("x2"));
            }
            return sb.ToString();
        }
    }

    private static string ParseResult(string json)
    {
        try
        {
            // 使用正则表达式提取dst字段值
            var match = Regex.Match(json, "\"dst\":\"([^\"]+)\"");
            if (match.Success)
            {
                string translatedText = match.Groups[1].Value;
                // 解码Unicode转义序列
                translatedText = DecodeUnicodeEscapes(translatedText);
                // 处理其他转义字符
                translatedText = translatedText.Replace("\\\"", "\"");
                return translatedText;
            }

            // 检查错误码
            var errorMatch = Regex.Match(json, "\"error_code\":\"?([0-9]+)\"?");
            if (errorMatch.Success)
            {
                string errorCode = errorMatch.Groups[1].Value;
                return $"百度翻译错误({errorCode}): {GetBaiduErrorDescription(errorCode)}";
            }

            return $"翻译失败，无法解析结果: {json.Substring(0, Math.Min(100, json.Length))}...";
        }
        catch (Exception ex)
        {
            return $"解析结果时出错: {ex.Message}";
        }
    }

    private static string GetBaiduErrorDescription(string errorCode)
    {
        switch (errorCode)
        {
            case "52001": return "请求超时";
            case "52002": return "系统错误";
            case "52003": return "未授权用户";
            case "54000": return "必填参数为空";
            case "54001": return "签名错误";
            case "54003": return "访问频率受限";
            case "54004": return "账户余额不足";
            case "54005": return "长查询请求频繁";
            case "58000": return "客户端IP非法";
            case "58001": return "译文语言方向不支持";
            case "58002": return "服务当前已关闭";
            case "90107": return "认证未通过或未生效";
            default: return $"未知错误({errorCode})";
        }
    }

    private static string DecodeUnicodeEscapes(string input)
    {
        return Regex.Replace(input, @"\\u([0-9a-fA-F]{4})", match =>
        {
            string hex = match.Groups[1].Value;
            return ((char)Convert.ToInt32(hex, 16)).ToString();
        });
    }

    public static string TranslateWithoutCache(string text, string from, string to)
    {
        return TranslateWithoutCacheAsync(text, from, to).GetAwaiter().GetResult();
    }

    public static async Task<string> TranslateWithoutCacheAsync(string text, string from, string to)
    {
        if (string.IsNullOrWhiteSpace(text))
            return string.Empty;

        int retryCount = 0;
        while (retryCount < 3) // 最多重试3次
        {
            try
            {
                if (!LanguageMap.TryGetValue(from, out string fromCode))
                    throw new ArgumentException($"不支持的源语言: {from}");

                if (!LanguageMap.TryGetValue(to, out string toCode))
                    throw new ArgumentException($"不支持的目标语言: {to}");

                string salt = _random.Next(100000).ToString();
                string sign = ComputeMd5(APP_ID + text + salt + SECRET_KEY);

                var postData = new Dictionary<string, string>
                {
                    {"q", text},
                    {"from", fromCode},
                    {"to", toCode},
                    {"appid", APP_ID},
                    {"salt", salt},
                    {"sign", sign}
                };

                var content = new FormUrlEncodedContent(postData);
                var response = await _httpClient.PostAsync(API_URL, content);
                response.EnsureSuccessStatusCode();

                string result = await response.Content.ReadAsStringAsync();
                return ParseResult(result);
            }
            catch (HttpRequestException ex)
            {
                retryCount++;
                if (retryCount >= 3)
                {
                    return $"网络请求失败: {ex.Message}";
                }
                await Task.Delay(1000 * retryCount); // 延迟重试
            }
            catch (Exception ex)
            {
                return $"翻译出错: {ex.Message}";
            }
        }

        return "翻译失败，请重试";
    }

    /// <summary>
    /// 批量翻译方法（百度API支持批量）
    /// </summary>
    public static async Task<Dictionary<string, string>> BatchTranslateWithoutCacheAsync(
        List<string> texts, string from, string to)
    {
        if (texts == null || texts.Count == 0)
            return new Dictionary<string, string>();

        var results = new Dictionary<string, string>();
        int currentIndex = 0;

        while (currentIndex < texts.Count)
        {
            // 百度API单次最多支持10个文本
            int batchSize = Math.Min(10, texts.Count - currentIndex);
            var batchTexts = texts.GetRange(currentIndex, batchSize);

            try
            {
                var batchResults = await TranslateBatchInternal(batchTexts, from, to);
                foreach (var kvp in batchResults)
                {
                    results[kvp.Key] = kvp.Value;
                }
            }
            catch (Exception ex)
            {
                // 如果批量失败，回退到单个翻译
                Console.WriteLine($"批量翻译失败，回退到单个翻译: {ex.Message}");
                var fallbackResults = await FallbackToSingleTranslationAsync(batchTexts, from, to);
                foreach (var kvp in fallbackResults)
                {
                    results[kvp.Key] = kvp.Value;
                }
            }

            currentIndex += batchSize;

            // 批量之间添加延迟（避免QPS限制）
            if (currentIndex < texts.Count)
            {
                await Task.Delay(1100); // 1.1秒延迟
            }
        }

        return results;
    }

    private static async Task<Dictionary<string, string>> TranslateBatchInternal(
        List<string> batchTexts, string from, string to)
    {
        if (!LanguageMap.TryGetValue(from, out string fromCode))
            throw new ArgumentException($"不支持的源语言: {from}");

        if (!LanguageMap.TryGetValue(to, out string toCode))
            throw new ArgumentException($"不支持的目标语言: {to}");

        // 将多个文本用换行符连接
        string combinedText = string.Join("\n", batchTexts);
        string salt = _random.Next(100000).ToString();
        string sign = ComputeMd5(APP_ID + combinedText + salt + SECRET_KEY);

        var postData = new Dictionary<string, string>
        {
            {"q", combinedText},
            {"from", fromCode},
            {"to", toCode},
            {"appid", APP_ID},
            {"salt", salt},
            {"sign", sign}
        };

        var content = new FormUrlEncodedContent(postData);
        var response = await _httpClient.PostAsync(API_URL, content);
        response.EnsureSuccessStatusCode();

        string resultJson = await response.Content.ReadAsStringAsync();
        return ParseBatchResult(resultJson, batchTexts);
    }

    private static Dictionary<string, string> ParseBatchResult(string json, List<string> originalTexts)
    {
        var results = new Dictionary<string, string>();

        try
        {
            var regex = new Regex("\"src\":\"([^\"]+)\",\"dst\":\"([^\"]+)\"");
            var matches = regex.Matches(json);

            if (matches.Count == originalTexts.Count)
            {
                for (int i = 0; i < matches.Count; i++)
                {
                    string src = DecodeUnicodeEscapes(matches[i].Groups[1].Value);
                    string dst = DecodeUnicodeEscapes(matches[i].Groups[2].Value);
                    results[src] = dst;
                }
            }
            else
            {
                // 解析失败，尝试通用解析
                string translatedText = ParseResult(json);
                if (originalTexts.Count == 1)
                {
                    results[originalTexts[0]] = translatedText;
                }
                else
                {
                    // 多个文本但解析失败，标记为失败
                    foreach (var text in originalTexts)
                    {
                        results[text] = "批量解析失败";
                    }
                }
            }
        }
        catch (Exception)
        {
            // 解析失败，标记所有为失败
            foreach (var text in originalTexts)
            {
                results[text] = "解析结果失败";
            }
        }

        return results;
    }

    /// <summary>
    /// 回退到单个翻译（批量失败时使用）
    /// </summary>
    private static async Task<Dictionary<string, string>> FallbackToSingleTranslationAsync(
        List<string> texts, string from, string to)
    {
        var results = new Dictionary<string, string>();
        var tasks = new List<Task>();

        // 并行处理，但限制并发数
        var semaphore = new System.Threading.SemaphoreSlim(3);

        foreach (var text in texts)
        {
            await semaphore.WaitAsync();
            var task = Task.Run(async () =>
            {
                try
                {
                    string result = await TranslateWithoutCacheAsync(text, from, to);
                    lock (results)
                    {
                        results[text] = result;
                    }
                }
                catch (Exception ex)
                {
                    lock (results)
                    {
                        results[text] = $"翻译失败: {ex.Message}";
                    }
                }
                finally
                {
                    semaphore.Release();
                }
            });
            tasks.Add(task);
        }

        await Task.WhenAll(tasks);
        return results;
    }
}