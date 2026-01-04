using System;
using System.Data;
using System.Data.SQLite;
using System.IO;

namespace Translator
{
    public static class SqliteHelper
    {
        private static string connectionString = "Data Source=Translator.db;Version=3;";

        public static bool IsSqliteAvailable()
        {
            try
            {
                string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Translator.db");
                return File.Exists(dbPath);
            }
            catch
            {
                return false;
            }
        }

        public static string GetCachedTranslationFromSqlite(string sourceText, string sourceLanguage, string targetLanguage)
        {
            try
            {
                if (!IsSqliteAvailable())
                {
                    CreateSqliteDatabase();
                }

                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    string query = @"
                        SELECT TranslatedText 
                        FROM TranslationCache 
                        WHERE SourceText = @SourceText 
                        AND SourceLanguage = @SourceLanguage 
                        AND TargetLanguage = @TargetLanguage";

                    using (SQLiteCommand command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@SourceText", sourceText);
                        command.Parameters.AddWithValue("@SourceLanguage", sourceLanguage);
                        command.Parameters.AddWithValue("@TargetLanguage", targetLanguage);

                        var result = command.ExecuteScalar();
                        if (result != null)
                        {
                            UpdateUsageStatisticsInSqlite(sourceText, sourceLanguage, targetLanguage);
                            return result.ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SQLite 查询失败: {ex.Message}");
            }

            return null;
        }

        public static void SaveTranslationToSqlite(string sourceText, string sourceLanguage, string targetLanguage, string translatedText)
        {
            try
            {
                if (!IsSqliteAvailable())
                {
                    CreateSqliteDatabase();
                }

                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    string query = @"
                        INSERT OR REPLACE INTO TranslationCache 
                        (SourceText, SourceLanguage, TargetLanguage, TranslatedText, LastUsedTime, UseCount)
                        VALUES (@SourceText, @SourceLanguage, @TargetLanguage, @TranslatedText, 
                                datetime('now'), 
                                COALESCE((SELECT UseCount + 1 FROM TranslationCache 
                                         WHERE SourceText = @SourceText 
                                         AND SourceLanguage = @SourceLanguage 
                                         AND TargetLanguage = @TargetLanguage), 1))";

                    using (SQLiteCommand command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@SourceText", sourceText);
                        command.Parameters.AddWithValue("@SourceLanguage", sourceLanguage);
                        command.Parameters.AddWithValue("@TargetLanguage", targetLanguage);
                        command.Parameters.AddWithValue("@TranslatedText", translatedText);

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"保存到 SQLite 失败: {ex.Message}");
            }
        }

        private static void UpdateUsageStatisticsInSqlite(string sourceText, string sourceLanguage, string targetLanguage)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    string query = @"
                        UPDATE TranslationCache 
                        SET LastUsedTime = datetime('now'),
                            UseCount = UseCount + 1
                        WHERE SourceText = @SourceText 
                        AND SourceLanguage = @SourceLanguage 
                        AND TargetLanguage = @TargetLanguage";

                    using (SQLiteCommand command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@SourceText", sourceText);
                        command.Parameters.AddWithValue("@SourceLanguage", sourceLanguage);
                        command.Parameters.AddWithValue("@TargetLanguage", targetLanguage);

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"更新 SQLite 使用统计失败: {ex.Message}");
            }
        }

        private static void CreateSqliteDatabase()
        {
            try
            {
                string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Translator.db");

                // 创建数据库文件
                SQLiteConnection.CreateFile(dbPath);

                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    string createTableQuery = @"
                        CREATE TABLE TranslationCache (
                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
                            SourceText TEXT NOT NULL,
                            SourceLanguage TEXT NOT NULL,
                            TargetLanguage TEXT NOT NULL,
                            TranslatedText TEXT NOT NULL,
                            CreatedTime DATETIME DEFAULT CURRENT_TIMESTAMP,
                            LastUsedTime DATETIME DEFAULT CURRENT_TIMESTAMP,
                            UseCount INTEGER DEFAULT 1
                        );
                        
                        CREATE INDEX idx_Search ON TranslationCache(SourceText, SourceLanguage, TargetLanguage);
                    ";

                    using (SQLiteCommand command = new SQLiteCommand(createTableQuery, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"创建 SQLite 数据库失败: {ex.Message}");
                throw;
            }
        }

        public static DataTable GetAllCacheData()
        {
            DataTable dataTable = new DataTable();

            try
            {
                if (!IsSqliteAvailable())
                {
                    return dataTable;
                }

                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    string query = @"
                        SELECT 
                            Id,
                            SourceText,
                            SourceLanguage,
                            TargetLanguage,
                            TranslatedText,
                            CreatedTime,
                            LastUsedTime,
                            UseCount
                        FROM TranslationCache 
                        ORDER BY LastUsedTime DESC";

                    using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(query, connection))
                    {
                        adapter.Fill(dataTable);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"从 SQLite 加载数据失败: {ex.Message}");
            }

            return dataTable;
        }
    }
}