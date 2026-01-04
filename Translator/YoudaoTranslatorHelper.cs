using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

public class YoudaoTranslatorHelper
{
    // 有道智云API配置
    private const string APP_KEY = "00557bf448f5eef1";
    private const string APP_SECRET = "YtduGCxHdSLbbmh2JbxJk90uqSfbIMlV";
    private const string API_URL = "https://openapi.youdao.com/api";

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

    // 生成签名
    private static string GenerateSign(string input, string salt, string curtime)
    {
        string inputStr = "";
        if (input.Length > 20)
        {
            inputStr = input.Substring(0, 10) + input.Length + input.Substring(input.Length - 10);
        }
        else
        {
            inputStr = input;
        }

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
            var jsonObj = JObject.Parse(json);

            // 检查错误码
            if (jsonObj["errorCode"] != null && jsonObj["errorCode"].ToString() != "0")
            {
                string errorCode = jsonObj["errorCode"].ToString();
                return $"有道翻译错误({errorCode}): {GetYoudaoErrorDescription(errorCode)}";
            }

            // 获取翻译结果
            if (jsonObj["translation"] != null)
            {
                var translationArray = jsonObj["translation"] as JArray;
                if (translationArray != null && translationArray.Count > 0)
                {
                    string translatedText = translationArray[0].ToString();
                    return DecodeUnicodeEscapes(translatedText);
                }
            }

            return $"翻译结果为空: {json}";
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
            case "1001": return "无效的OCR类型";
            case "1002": return "不支持的OCR image类型";
            case "1003": return "不支持的OCR Language类型";
            case "1004": return "识别图片过大";
            case "1201": return "图片base64解密失败";
            case "1301": return "OCR段落识别失败";
            case "1411": return "访问频率受限";
            case "1412": return "超过最大识别字节数";
            default: return $"未知错误({errorCode})";
        }
    }

    // 有道翻译方法
    public static string Translate(string text, string from, string to)
    {
        if (string.IsNullOrWhiteSpace(text))
            return string.Empty;

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
            string postData = $"q={Uri.EscapeDataString(text)}" +
                            $"&from={fromCode}" +
                            $"&to={toCode}" +
                            $"&appKey={APP_KEY}" +
                            $"&salt={salt}" +
                            $"&sign={sign}" +
                            $"&signType=v3" +
                            $"&curtime={curtime}";

            // 发送请求
            using (var client = new WebClient())
            {
                client.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                client.Encoding = Encoding.UTF8;

                byte[] responseBytes = client.UploadData(API_URL, "POST", Encoding.UTF8.GetBytes(postData));
                string responseJson = Encoding.UTF8.GetString(responseBytes);

                return ParseYoudaoResult(responseJson);
            }
        }
        catch (WebException ex)
        {
            if (ex.Response != null)
            {
                using (var stream = ex.Response.GetResponseStream())
                using (var reader = new System.IO.StreamReader(stream, Encoding.UTF8))
                {
                    string errorResponse = reader.ReadToEnd();
                    return $"有道API网络错误: {ex.Status} - {errorResponse}";
                }
            }
            return $"有道API网络错误: {ex.Message}";
        }
        catch (Exception ex)
        {
            return $"有道翻译出错: {ex.Message}";
        }
    }

    // 异步翻译方法
    public static async Task<string> TranslateAsync(string text, string from, string to)
    {
        return await Task.Run(() => Translate(text, from, to));
    }
}