using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Translator
{
    public class ApplicationSettings
    {
        private static Dictionary<string, string> settings = new Dictionary<string, string>();
        private static string settingsFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.ini");

        static ApplicationSettings()
        {
            LoadSettings();
        }

        private static void LoadSettings()
        {
            try
            {
                if (File.Exists(settingsFile))
                {
                    string[] lines = File.ReadAllLines(settingsFile);
                    foreach (string line in lines)
                    {
                        if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
                            continue;

                        int equalsIndex = line.IndexOf('=');
                        if (equalsIndex > 0)
                        {
                            string key = line.Substring(0, equalsIndex).Trim();
                            string value = line.Substring(equalsIndex + 1).Trim();
                            settings[key] = value;
                        }
                    }
                }
            }
            catch
            {
                // 如果加载失败，使用默认值
            }

            // 设置默认值
            EnsureDefaultSettings();
        }

        private static void EnsureDefaultSettings()
        {
            if (!settings.ContainsKey("AutoBackup"))
                settings["AutoBackup"] = "true";

            if (!settings.ContainsKey("AutoUpdate"))
                settings["AutoUpdate"] = "true";

            if (!settings.ContainsKey("BackupPath"))
                settings["BackupPath"] = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Translator", "Backups");

            if (!settings.ContainsKey("UseSqliteFallback"))
                settings["UseSqliteFallback"] = "true";

            if (!settings.ContainsKey("Language"))
                settings["Language"] = "zh-CN";

            if (!settings.ContainsKey("Theme"))
                settings["Theme"] = "Light";
        }

        public static string Get(string key, string defaultValue = "")
        {
            if (settings.ContainsKey(key))
                return settings[key];
            return defaultValue;
        }

        public static bool GetBool(string key, bool defaultValue = false)
        {
            if (settings.ContainsKey(key))
            {
                if (bool.TryParse(settings[key], out bool result))
                    return result;
            }
            return defaultValue;
        }

        public static int GetInt(string key, int defaultValue = 0)
        {
            if (settings.ContainsKey(key))
            {
                if (int.TryParse(settings[key], out int result))
                    return result;
            }
            return defaultValue;
        }

        public static void Set(string key, string value)
        {
            settings[key] = value;
            SaveSettings();
        }

        public static void SetBool(string key, bool value)
        {
            Set(key, value.ToString());
        }

        public static void SetInt(string key, int value)
        {
            Set(key, value.ToString());
        }

        private static void SaveSettings()
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("# 多语言翻译工具配置文件");
                sb.AppendLine("# 自动生成，请勿手动修改（除非你知道在做什么）");
                sb.AppendLine();

                foreach (var kvp in settings.OrderBy(k => k.Key))
                {
                    sb.AppendLine($"{kvp.Key}={kvp.Value}");
                }

                File.WriteAllText(settingsFile, sb.ToString(), Encoding.UTF8);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"保存设置失败: {ex.Message}");
            }
        }

        public static void Reload()
        {
            settings.Clear();
            LoadSettings();
        }
    }
}