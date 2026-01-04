using System;
using System.Net;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text.RegularExpressions;

public class BaiduTranslatorHelper
{
    // 百度翻译API的APP ID和密钥
    private const string APP_ID = "20250711002403394";
    private const string SECRET_KEY = "_qnpHJqG3nsng76K8jlg";
    private const string API_URL = "https://fanyi-api.baidu.com/api/trans/vip/translate";

    private static readonly Dictionary<string, string> LanguageMap = new Dictionary<string, string>
    {
        {"中文", "zh"}, {"英语", "en"}, {"法语", "fra"}, {"德语", "de"},{"阿拉伯语", "ara"}, {"俄语", "ru"}, {"葡萄牙语", "pt"}, {"泰语", "th"},{"西班牙语", "spa"}, {"意大利语", "it"},{"印度尼西亚语", "id"}, {"越南语", "vie"},{"马来西亚","ms"}
    };

    private static string GetIPAddressInfo()
    {
        try
        {
            string localIp = GetLocalIPAddress();
            string publicIp = GetPublicIPAddressWithFallback();

            return $"请将以下IP添加到百度翻译API白名单:\n" +
                   $"本地IP: {localIp}\n" +
                   $"公网IP: {publicIp}\n" +
                   $"返回编码:58001,译文语言方向不支持，个人标准版和高级版支持28个常见语种，企业尊享版支持全部语种";
        }
        catch
        {
            return "无法获取IP信息，请手动检查网络连接";
        }
    }

    private static string GetLocalIPAddress()
    {
        try
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            return "无法获取本地IP";
        }
        catch
        {
            return "本地IP获取失败";
        }
    }

    private static string GetPublicIPAddressWithFallback()
    {
        // 尝试多个IP查询服务
        string[] ipServices = new[]
        {
            "https://ipinfo.io/ip",
            "https://ifconfig.me/ip",
            "https://checkip.amazonaws.com"
        };

        foreach (var service in ipServices)
        {
            try
            {
                using (var client = new WebClient())
                {
                    client.Headers.Add("User-Agent", "Mozilla/4.0");

                    return client.DownloadString(service).Trim();
                }
            }
            catch { }
        }

        return "无法获取公网IP";
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

            return $"无法解析翻译结果: {json}\n\n{GetIPAddressInfo()}";
        }
        catch (Exception ex)
        {
            return $"解析结果时出错: {ex.Message}\n\n{GetIPAddressInfo()}";
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
        if (string.IsNullOrWhiteSpace(text))
            return string.Empty;

        try
        {
            if (!LanguageMap.TryGetValue(from, out string fromCode))
                throw new ArgumentException("不支持的源语言");

            if (!LanguageMap.TryGetValue(to, out string toCode))
                throw new ArgumentException("不支持的目标语言");

            // 直接调用API，不检查缓存
            string salt = new Random().Next(100000).ToString();
            string sign = ComputeMd5(APP_ID + text + salt + SECRET_KEY);
            string postData = $"q={Uri.EscapeDataString(text)}&from={fromCode}&to={toCode}&appid={APP_ID}&salt={salt}&sign={sign}";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(API_URL);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";

            byte[] data = Encoding.UTF8.GetBytes(postData);
            request.ContentLength = data.Length;

            using (Stream stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
            {
                string result = reader.ReadToEnd();
                return ParseResult(result);
            }
        }
        catch (WebException ex)
        {
            string errorResponse = ex.Response != null ?
                new StreamReader(ex.Response.GetResponseStream()).ReadToEnd() :
                "无响应内容";

            string ipInfo = GetIPAddressInfo();
            return $"翻译API错误: {errorResponse}\n\n{ipInfo}";
        }
        catch (Exception ex)
        {
            string ipInfo = GetIPAddressInfo();
            return $"翻译出错: {ex.Message}\n\n{ipInfo}";
        }
    }


    /// <summary>
    /// 批量翻译方法（百度API支持批量）
    /// </summary>
    public static Dictionary<string, string> BatchTranslateWithoutCache(
        List<string> texts, string from, string to)
    {
        if (texts == null || texts.Count == 0)
            return new Dictionary<string, string>();

        var results = new Dictionary<string, string>();

        try
        {
            if (!LanguageMap.TryGetValue(from, out string fromCode))
                throw new ArgumentException("不支持的源语言");

            if (!LanguageMap.TryGetValue(to, out string toCode))
                throw new ArgumentException("不支持的目标语言");

            // 百度API单次最多支持2000字符，限制每次请求的文本数量
            int maxBatchSize = 10; // 每次最多翻译10个文本
            int currentIndex = 0;

            while (currentIndex < texts.Count)
            {
                int batchSize = Math.Min(maxBatchSize, texts.Count - currentIndex);
                var batchTexts = texts.GetRange(currentIndex, batchSize);

                // 将多个文本用换行符连接（百度API支持换行符分隔的批量翻译）
                string combinedText = string.Join("\n", batchTexts);

                string salt = new Random().Next(100000).ToString();
                string sign = ComputeMd5(APP_ID + combinedText + salt + SECRET_KEY);
                string postData = $"q={Uri.EscapeDataString(combinedText)}&from={fromCode}&to={toCode}&appid={APP_ID}&salt={salt}&sign={sign}";

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(API_URL);
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";

                byte[] data = Encoding.UTF8.GetBytes(postData);
                request.ContentLength = data.Length;

                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                {
                    string resultJson = reader.ReadToEnd();
                    var batchResults = ParseBatchResult(resultJson, batchTexts);

                    foreach (var kvp in batchResults)
                    {
                        results[kvp.Key] = kvp.Value;
                    }
                }

                currentIndex += batchSize;

                // 批量之间添加较小延迟（百度API要求QPS<=1，但批量翻译可以减少调用次数）
                if (currentIndex < texts.Count)
                {
                    System.Threading.Thread.Sleep(1200); // 1.2秒延迟
                }
            }
        }
        catch (Exception ex)
        {
            // 如果批量失败，回退到单个翻译
            Console.WriteLine($"批量翻译失败，回退到单个翻译: {ex.Message}");
            return FallbackToSingleTranslation(texts, from, to);
        }

        return results;
    }

    /// <summary>
    /// 解析批量翻译结果
    /// </summary>
    private static Dictionary<string, string> ParseBatchResult(string json, List<string> originalTexts)
    {
        var results = new Dictionary<string, string>();

        try
        {
            // 百度批量翻译返回格式示例：
            // {"from":"zh","to":"en","trans_result":[{"src":"你好","dst":"Hello"},{"src":"世界","dst":"World"}]}
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
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"解析批量翻译结果失败: {ex.Message}");
        }

        return results;
    }

    /// <summary>
    /// 回退到单个翻译（批量失败时使用）
    /// </summary>
    private static Dictionary<string, string> FallbackToSingleTranslation(
        List<string> texts, string from, string to)
    {
        var results = new Dictionary<string, string>();

        foreach (var text in texts)
        {
            try
            {
                string result = TranslateWithoutCache(text, from, to);
                results[text] = result;

                // 单个翻译时保持1秒延迟
                System.Threading.Thread.Sleep(1000);
            }
            catch (Exception ex)
            {
                results[text] = $"翻译失败: {ex.Message}";
            }
        }

        return results;
    }
}