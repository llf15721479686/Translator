using System;
using System.Configuration;
using System.IO;
using System.Xml;

namespace Translator
{
    public static class AppSettingsManager
    {
        private static string configFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Translator.config");

        public static void SaveSetting(string key, string value)
        {
            try
            {
                // 方法1: 使用 ConfigurationManager（需要读写权限）
                try
                {
                    var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                    if (config.AppSettings.Settings[key] == null)
                    {
                        config.AppSettings.Settings.Add(key, value);
                    }
                    else
                    {
                        config.AppSettings.Settings[key].Value = value;
                    }
                    config.Save(ConfigurationSaveMode.Modified);
                    ConfigurationManager.RefreshSection("appSettings");
                    return;
                }
                catch
                {
                    // 如果失败，使用方法2
                }

                // 方法2: 使用自定义XML配置文件
                SaveToCustomConfig(key, value);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"保存设置失败: {ex.Message}");
                throw;
            }
        }

        private static void SaveToCustomConfig(string key, string value)
        {
            XmlDocument xmlDoc = new XmlDocument();

            // 如果配置文件不存在，创建新的
            if (!File.Exists(configFilePath))
            {
                XmlDeclaration xmlDeclaration = xmlDoc.CreateXmlDeclaration("1.0", "UTF-8", null);
                xmlDoc.AppendChild(xmlDeclaration);

                XmlElement root = xmlDoc.CreateElement("configuration");
                xmlDoc.AppendChild(root);

                XmlElement appSettings = xmlDoc.CreateElement("appSettings");
                root.AppendChild(appSettings);
            }
            else
            {
                xmlDoc.Load(configFilePath);
            }

            // 查找或创建appSettings节点
            XmlNode appSettingsNode = xmlDoc.SelectSingleNode("//appSettings");
            if (appSettingsNode == null)
            {
                appSettingsNode = xmlDoc.CreateElement("appSettings");
                xmlDoc.DocumentElement.AppendChild(appSettingsNode);
            }

            // 查找或创建指定key的设置
            XmlNode settingNode = appSettingsNode.SelectSingleNode($"//add[@key='{key}']");
            if (settingNode == null)
            {
                settingNode = xmlDoc.CreateElement("add");
                XmlAttribute keyAttr = xmlDoc.CreateAttribute("key");
                keyAttr.Value = key;
                settingNode.Attributes.Append(keyAttr);

                XmlAttribute valueAttr = xmlDoc.CreateAttribute("value");
                valueAttr.Value = value;
                settingNode.Attributes.Append(valueAttr);

                appSettingsNode.AppendChild(settingNode);
            }
            else
            {
                settingNode.Attributes["value"].Value = value;
            }

            xmlDoc.Save(configFilePath);
        }

        public static string GetSetting(string key, string defaultValue = "")
        {
            try
            {
                // 方法1: 先尝试从ConfigurationManager读取
                try
                {
                    var value = ConfigurationManager.AppSettings[key];
                    if (!string.IsNullOrEmpty(value))
                        return value;
                }
                catch
                {
                    // 如果失败，使用方法2
                }

                // 方法2: 从自定义配置文件读取
                return GetFromCustomConfig(key, defaultValue);
            }
            catch
            {
                return defaultValue;
            }
        }

        private static string GetFromCustomConfig(string key, string defaultValue)
        {
            try
            {
                if (!File.Exists(configFilePath))
                    return defaultValue;

                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(configFilePath);

                XmlNode settingNode = xmlDoc.SelectSingleNode($"//configuration/appSettings/add[@key='{key}']");
                if (settingNode != null && settingNode.Attributes["value"] != null)
                {
                    return settingNode.Attributes["value"].Value;
                }
            }
            catch
            {
                // 忽略错误，返回默认值
            }

            return defaultValue;
        }

        public static bool GetBoolSetting(string key, bool defaultValue = false)
        {
            string value = GetSetting(key, defaultValue.ToString());
            return bool.TryParse(value, out bool result) ? result : defaultValue;
        }

        public static void SaveBoolSetting(string key, bool value)
        {
            SaveSetting(key, value.ToString());
        }
    }
}