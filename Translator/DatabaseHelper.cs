// DatabaseHelper.cs
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using Translator;

public static class DatabaseHelper
{
    public static string connectionString = ConfigurationManager.ConnectionStrings["Translator"]?.ConnectionString
        ?? "Server=.;Database=Translator;Integrated Security=True;";

    //public static string GetCachedTranslation(string sourceText, string sourceLanguage, string targetLanguage)
    //{
    //    try
    //    {
    //        using (var connection = new SqlConnection(connectionString))
    //        {
    //            connection.Open();
    //            string query = @"
    //                SELECT TranslatedText 
    //                FROM TranslationCache 
    //                WHERE SourceText = @SourceText 
    //                AND SourceLanguage = @SourceLanguage 
    //                AND TargetLanguage = @TargetLanguage";

    //            using (var command = new SqlCommand(query, connection))
    //            {
    //                command.Parameters.AddWithValue("@SourceText", sourceText);
    //                command.Parameters.AddWithValue("@SourceLanguage", sourceLanguage);
    //                command.Parameters.AddWithValue("@TargetLanguage", targetLanguage);

    //                var result = command.ExecuteScalar();
    //                if (result != null)
    //                {
    //                    // 更新使用计数和最后使用时间
    //                    UpdateUsageStatistics(sourceText, sourceLanguage, targetLanguage);
    //                    return result.ToString();
    //                }
    //            }
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        // 数据库错误时，直接调用API
    //        Console.WriteLine($"数据库查询失败: {ex.Message}");
    //    }

    //    return null;
    //}

    public static void SaveTranslation(string sourceText, string sourceLanguage, string targetLanguage, string translatedText)
    {
        try
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = @"
                    IF EXISTS (SELECT 1 FROM TranslationCache 
                              WHERE SourceText = @SourceText 
                              AND SourceLanguage = @SourceLanguage 
                              AND TargetLanguage = @TargetLanguage)
                    BEGIN
                        -- 更新现有记录
                        UPDATE TranslationCache 
                        SET TranslatedText = @TranslatedText,
                            LastUsedTime = GETDATE(),
                            UseCount = UseCount + 1
                        WHERE SourceText = @SourceText 
                        AND SourceLanguage = @SourceLanguage 
                        AND TargetLanguage = @TargetLanguage
                    END
                    ELSE
                    BEGIN
                        -- 插入新记录
                        INSERT INTO TranslationCache 
                            (SourceText, SourceLanguage, TargetLanguage, TranslatedText, CreatedTime, LastUsedTime, UseCount)
                        VALUES 
                            (@SourceText, @SourceLanguage, @TargetLanguage, @TranslatedText, GETDATE(), GETDATE(), 1)
                    END";

                using (var command = new SqlCommand(query, connection))
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
            Console.WriteLine($"保存到数据库失败: {ex.Message}");
        }
    }

    private static void UpdateUsageStatistics(string sourceText, string sourceLanguage, string targetLanguage)
    {
        try
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = @"
                    UPDATE TranslationCache 
                    SET LastUsedTime = GETDATE(),
                        UseCount = UseCount + 1
                    WHERE SourceText = @SourceText 
                    AND SourceLanguage = @SourceLanguage 
                    AND TargetLanguage = @TargetLanguage";

                using (var command = new SqlCommand(query, connection))
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
            Console.WriteLine($"更新使用统计失败: {ex.Message}");
        }
    }


    /// <summary>
    /// 批量获取缓存的翻译结果（修复版 - 添加使用计数更新）
    /// </summary>
    public static Dictionary<string, string> GetBatchCachedTranslations(
           List<string> words, string sourceLanguage, string targetLanguage)
    {
        var cachedTranslations = new Dictionary<string, string>();

        try
        {
            if (words.Count == 0)
                return cachedTranslations;

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // 构建IN子句参数
                var parameters = new List<SqlParameter>();
                var paramNames = new List<string>();

                for (int i = 0; i < words.Count; i++)
                {
                    string paramName = $"@word{i}";
                    paramNames.Add(paramName);
                    parameters.Add(new SqlParameter(paramName, words[i]));
                }

                // 修复SQL查询：添加源语言和目标语言筛选条件
                string query = $@"
                SELECT SourceText, TranslatedText 
                FROM TranslationCache 
                WHERE SourceText IN ({string.Join(",", paramNames)})
                AND SourceLanguage = @SourceLanguage 
                AND TargetLanguage = @TargetLanguage";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddRange(parameters.ToArray());
                    command.Parameters.Add(new SqlParameter("@SourceLanguage", sourceLanguage));
                    command.Parameters.Add(new SqlParameter("@TargetLanguage", targetLanguage));

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string sourceText = reader["SourceText"].ToString();
                            string translatedText = reader["TranslatedText"].ToString();
                            cachedTranslations[sourceText] = translatedText;
                        }
                    }
                }

                // 新增：批量更新使用计数和最后使用时间
                if (cachedTranslations.Count > 0)
                {
                    UpdateBatchUsageStatistics(cachedTranslations.Keys.ToList(), sourceLanguage, targetLanguage);
                }
            }
        }
        catch (Exception ex)
        {
            // 记录错误但不抛出，避免影响主流程
            Console.WriteLine($"获取缓存翻译失败: {ex.Message}");
        }

        return cachedTranslations;
    }

    /// <summary>
    /// 批量更新使用统计
    /// </summary>
    private static void UpdateBatchUsageStatistics(List<string> words, string sourceLanguage, string targetLanguage)
    {
        try
        {
            if (words.Count == 0)
                return;

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                var parameters = new List<SqlParameter>();
                var conditions = new List<string>();

                for (int i = 0; i < words.Count; i++)
                {
                    string paramName = $"@SourceText{i}";
                    conditions.Add($"(SourceText = {paramName} AND SourceLanguage = @SourceLanguage AND TargetLanguage = @TargetLanguage)");
                    parameters.Add(new SqlParameter(paramName, words[i]));
                }

                string query = $@"
                UPDATE TranslationCache 
                SET LastUsedTime = GETDATE(),
                    UseCount = UseCount + 1
                WHERE ({string.Join(" OR ", conditions)})
                AND SourceLanguage = @SourceLanguage 
                AND TargetLanguage = @TargetLanguage";

                parameters.Add(new SqlParameter("@SourceLanguage", sourceLanguage));
                parameters.Add(new SqlParameter("@TargetLanguage", targetLanguage));

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddRange(parameters.ToArray());
                    int updatedRows = command.ExecuteNonQuery();
                    Console.WriteLine($"更新了 {updatedRows} 条记录的使用统计");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"批量更新使用统计失败: {ex.Message}");
        }
    }

    /// <summary>
    /// 批量保存翻译结果（最终修复版）
    /// </summary>
    public static void SaveBatchTranslations(
        Dictionary<string, string> translations,
        string sourceLanguage,
        string targetLanguage)
    {
        if (translations == null || translations.Count == 0)
            return;

        try
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // 使用事务确保数据一致性
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        foreach (var kvp in translations)
                        {
                            // 检查是否已存在相同的记录
                            string checkQuery = @"
                            SELECT COUNT(*) FROM TranslationCache 
                            WHERE SourceText = @SourceText 
                            AND SourceLanguage = @SourceLanguage 
                            AND TargetLanguage = @TargetLanguage";

                            using (var checkCommand = new SqlCommand(checkQuery, connection, transaction))
                            {
                                checkCommand.Parameters.AddWithValue("@SourceText", kvp.Key);
                                checkCommand.Parameters.AddWithValue("@SourceLanguage", sourceLanguage);
                                checkCommand.Parameters.AddWithValue("@TargetLanguage", targetLanguage);

                                int exists = Convert.ToInt32(checkCommand.ExecuteScalar());

                                if (exists > 0)
                                {
                                    // 更新现有记录
                                    string updateQuery = @"
                                    UPDATE TranslationCache 
                                    SET TranslatedText = @TranslatedText,
                                        LastUsedTime = GETDATE(),
                                        UseCount = UseCount + 1
                                    WHERE SourceText = @SourceText 
                                    AND SourceLanguage = @SourceLanguage 
                                    AND TargetLanguage = @TargetLanguage";

                                    using (var updateCommand = new SqlCommand(updateQuery, connection, transaction))
                                    {
                                        updateCommand.Parameters.AddWithValue("@SourceText", kvp.Key);
                                        updateCommand.Parameters.AddWithValue("@SourceLanguage", sourceLanguage);
                                        updateCommand.Parameters.AddWithValue("@TargetLanguage", targetLanguage);
                                        updateCommand.Parameters.AddWithValue("@TranslatedText", kvp.Value);
                                        updateCommand.ExecuteNonQuery();
                                    }
                                }
                                else
                                {
                                    // 插入新记录
                                    string insertQuery = @"
                                    INSERT INTO TranslationCache 
                                        (SourceText, SourceLanguage, TargetLanguage, TranslatedText, CreatedTime, LastUsedTime, UseCount)
                                    VALUES 
                                        (@SourceText, @SourceLanguage, @TargetLanguage, @TranslatedText, GETDATE(), GETDATE(), 1)";

                                    using (var insertCommand = new SqlCommand(insertQuery, connection, transaction))
                                    {
                                        insertCommand.Parameters.AddWithValue("@SourceText", kvp.Key);
                                        insertCommand.Parameters.AddWithValue("@SourceLanguage", sourceLanguage);
                                        insertCommand.Parameters.AddWithValue("@TargetLanguage", targetLanguage);
                                        insertCommand.Parameters.AddWithValue("@TranslatedText", kvp.Value);
                                        insertCommand.ExecuteNonQuery();
                                    }
                                }
                            }
                        }

                        // 提交事务
                        transaction.Commit();

                        // 记录成功日志
                        Console.WriteLine($"成功保存 {translations.Count} 条翻译记录到数据库");
                        System.Diagnostics.Debug.WriteLine($"数据库保存成功: {sourceLanguage} -> {targetLanguage}, 数量: {translations.Count}");
                    }
                    catch (Exception ex)
                    {
                        // 回滚事务
                        transaction.Rollback();
                        throw new Exception($"批量保存事务失败: {ex.Message}", ex);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"批量保存翻译失败: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"数据库保存失败: {ex.Message}");

            // 如果批量保存失败，回退到逐个保存（不抛异常）
            try
            {
                foreach (var kvp in translations)
                {
                    try
                    {
                        SaveTranslation(kvp.Key, sourceLanguage, targetLanguage, kvp.Value);
                    }
                    catch (Exception innerEx)
                    {
                        Console.WriteLine($"单个保存失败 [{kvp.Key}]: {innerEx.Message}");
                    }
                }
            }
            catch (Exception fallbackEx)
            {
                Console.WriteLine($"回退保存也失败: {fallbackEx.Message}");
            }
        }
    }

    #region 词典翻译
    // 获取单个翻译缓存
    public static string GetCachedTranslation(string sourceText, string sourceLang, string targetLang)
    {
        try
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = @"
            SELECT TOP 1 TranslatedText 
            FROM TranslationCache 
            WHERE SourceText = @SourceText 
              AND SourceLanguage = @SourceLang 
              AND TargetLanguage = @TargetLang";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@SourceText", sourceText);
                    command.Parameters.AddWithValue("@SourceLang", sourceLang);
                    command.Parameters.AddWithValue("@TargetLang", targetLang);

                    object result = command.ExecuteScalar();

                    if (result != null)
                    {
                        string translatedText = result.ToString();

                        // 新增：更新单条记录的使用统计
                        UpdateSingleUsageStatistics(sourceText, sourceLang, targetLang);

                        return translatedText;
                    }

                    return "";
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"获取缓存翻译失败: {ex.Message}");
            return "";
        }
    }

    // 新增：更新单条记录的使用统计（与UpdateBatchUsageStatistics方法逻辑一致）
    private static void UpdateSingleUsageStatistics(string sourceText, string sourceLang, string targetLang)
    {
        try
        {
            if (string.IsNullOrEmpty(sourceText))
                return;

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string updateQuery = @"
            UPDATE TranslationCache 
            SET UseCount = ISNULL(UseCount, 0) + 1,
                LastUsedTime = GETDATE()
            WHERE SourceText = @SourceText 
              AND SourceLanguage = @SourceLanguage 
              AND TargetLanguage = @TargetLanguage";

                using (var command = new SqlCommand(updateQuery, connection))
                {
                    command.Parameters.Add(new SqlParameter("@SourceText", sourceText));
                    command.Parameters.Add(new SqlParameter("@SourceLanguage", sourceLang));
                    command.Parameters.Add(new SqlParameter("@TargetLanguage", targetLang));

                    int updatedRows = command.ExecuteNonQuery();
                    Console.WriteLine($"更新了 {updatedRows} 条记录的使用统计");
                }
            }
        }
        catch (Exception ex)
        {
            // 记录错误但不抛出，避免影响主流程
            Console.WriteLine($"更新单条使用统计失败: {ex.Message}");
        }
    }

    // 保存翻译记录到历史
    public static void SaveTranslationHistory(string sourceText, string resultText,
        string sourceLangCode, string targetLangCode, string method, DateTime translateTime)
    {
        try
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = @"
                INSERT INTO TranslationHistory 
                (SourceText, ResultText, SourceLangCode, TargetLangCode, 
                 Method, TranslateTime, CreatedTime)
                VALUES 
                (@SourceText, @ResultText, @SourceLangCode, @TargetLangCode, 
                 @Method, @TranslateTime, GETDATE())";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@SourceText", sourceText);
                    command.Parameters.AddWithValue("@ResultText", resultText);
                    command.Parameters.AddWithValue("@SourceLangCode", sourceLangCode);
                    command.Parameters.AddWithValue("@TargetLangCode", targetLangCode);
                    command.Parameters.AddWithValue("@Method", method);
                    command.Parameters.AddWithValue("@TranslateTime", translateTime);

                    command.ExecuteNonQuery();
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"保存翻译历史失败: {ex.Message}");
        }
    }

    // 获取翻译历史
    public static List<TranslationHistory> GetTranslationHistory(int maxCount = 20)
    {
        var history = new List<TranslationHistory>();

        try
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = @"
                SELECT TOP (@MaxCount) 
                       SourceText, ResultText, SourceLangCode, TargetLangCode, 
                       Method, TranslateTime
                FROM TranslationHistory 
                ORDER BY TranslateTime DESC";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@MaxCount", maxCount);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            history.Add(new TranslationHistory
                            {
                                SourceText = reader["SourceText"].ToString(),
                                ResultText = reader["ResultText"].ToString(),
                                SourceLangCode = reader["SourceLangCode"].ToString(),
                                TargetLangCode = reader["TargetLangCode"].ToString(),
                                Method = reader["Method"].ToString(),
                                TranslateTime = Convert.ToDateTime(reader["TranslateTime"])
                            });
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"获取翻译历史失败: {ex.Message}");
        }

        return history;
    }

    // 保存到收藏夹
    public static void SaveToFavorites(string sourceText, string resultText,
        string sourceLangCode, string targetLangCode, string engine)
    {
        try
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // 检查是否已存在
                string checkQuery = @"
                SELECT COUNT(*) FROM TranslationFavorites 
                WHERE SourceText = @SourceText 
                  AND SourceLangCode = @SourceLangCode 
                  AND TargetLangCode = @TargetLangCode";

                using (SqlCommand checkCommand = new SqlCommand(checkQuery, connection))
                {
                    checkCommand.Parameters.AddWithValue("@SourceText", sourceText);
                    checkCommand.Parameters.AddWithValue("@SourceLangCode", sourceLangCode);
                    checkCommand.Parameters.AddWithValue("@TargetLangCode", targetLangCode);

                    int count = Convert.ToInt32(checkCommand.ExecuteScalar());
                    if (count > 0)
                    {
                        throw new Exception("该翻译已存在于收藏夹中");
                    }
                }

                // 插入新收藏
                string insertQuery = @"
                INSERT INTO TranslationFavorites 
                (SourceText, ResultText, SourceLangCode, TargetLangCode, 
                 Engine, CreatedTime)
                VALUES 
                (@SourceText, @ResultText, @SourceLangCode, @TargetLangCode, 
                 @Engine, GETDATE())";

                using (SqlCommand insertCommand = new SqlCommand(insertQuery, connection))
                {
                    insertCommand.Parameters.AddWithValue("@SourceText", sourceText);
                    insertCommand.Parameters.AddWithValue("@ResultText", resultText);
                    insertCommand.Parameters.AddWithValue("@SourceLangCode", sourceLangCode);
                    insertCommand.Parameters.AddWithValue("@TargetLangCode", targetLangCode);
                    insertCommand.Parameters.AddWithValue("@Engine", engine);

                    insertCommand.ExecuteNonQuery();
                }
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"保存到收藏夹失败: {ex.Message}");
        }
    }

    // 历史记录类
    public class TranslationHistory
    {
        public string SourceText { get; set; }
        public string ResultText { get; set; }
        public string SourceLangCode { get; set; }
        public string TargetLangCode { get; set; }
        public string Method { get; set; }
        public DateTime TranslateTime { get; set; }
    }
    #endregion


    #region 字典功能相关方法

    /// <summary>
    /// 获取收藏夹列表
    /// </summary>
    public static List<FavoriteItem> GetFavorites(int maxCount = 50)
    {
        var favorites = new List<FavoriteItem>();

        try
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = @"
                SELECT TOP (@MaxCount) 
                       SourceText, ResultText, SourceLangCode, TargetLangCode, 
                       Engine, CreatedTime
                FROM TranslationFavorites 
                WHERE IsActive = 1
                ORDER BY CreatedTime DESC";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@MaxCount", maxCount);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            favorites.Add(new FavoriteItem
                            {
                                SourceText = reader["SourceText"].ToString(),
                                ResultText = reader["ResultText"].ToString(),
                                SourceLangCode = reader["SourceLangCode"].ToString(),
                                TargetLangCode = reader["TargetLangCode"].ToString(),
                                Engine = reader["Engine"].ToString(),
                                CreatedTime = Convert.ToDateTime(reader["CreatedTime"])
                            });
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"获取收藏夹失败: {ex.Message}");
        }

        return favorites;
    }

    /// <summary>
    /// 删除收藏项
    /// </summary>
    public static bool DeleteFavorite(string sourceText, string sourceLangCode, string targetLangCode)
    {
        try
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = @"
                DELETE FROM TranslationFavorites 
                WHERE SourceText = @SourceText 
                  AND SourceLangCode = @SourceLangCode 
                  AND TargetLangCode = @TargetLangCode";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@SourceText", sourceText);
                    command.Parameters.AddWithValue("@SourceLangCode", sourceLangCode);
                    command.Parameters.AddWithValue("@TargetLangCode", targetLangCode);

                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"删除收藏失败: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// 清除所有历史记录
    /// </summary>
    public static void ClearAllHistory()
    {
        try
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "DELETE FROM TranslationHistory";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"清除历史记录失败: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// 获取统计数据
    /// </summary>
    public static Dictionary<string, int> GetStatistics()
    {
        var stats = new Dictionary<string, int>();

        try
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = @"
                -- 缓存总数
                SELECT 'CacheCount' AS StatName, COUNT(*) AS StatValue FROM TranslationCache
                UNION ALL
                -- 今日使用次数
                SELECT 'TodayUsage', COUNT(*) FROM TranslationCache WHERE LastUsedTime >= CAST(GETDATE() AS DATE)
                UNION ALL
                -- 收藏总数
                SELECT 'FavoriteCount', COUNT(*) FROM TranslationFavorites WHERE IsActive = 1
                UNION ALL
                -- 历史记录总数
                SELECT 'HistoryCount', COUNT(*) FROM TranslationHistory
                UNION ALL
                -- 热门翻译（使用次数最多的）
                SELECT 'MostUsedCache', ISNULL(MAX(UseCount), 0) FROM TranslationCache";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            stats[reader["StatName"].ToString()] = Convert.ToInt32(reader["StatValue"]);
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"获取统计失败: {ex.Message}");
        }

        return stats;
    }

    #endregion

    #region 数据模型类

    public class FavoriteItem
    {
        public string SourceText { get; set; }
        public string ResultText { get; set; }
        public string SourceLangCode { get; set; }
        public string TargetLangCode { get; set; }
        public string Engine { get; set; }
        public DateTime CreatedTime { get; set; }
    }

    public class CacheItem
    {
        public string SourceText { get; set; }
        public string TranslatedText { get; set; }
        public string SourceLanguage { get; set; }
        public string TargetLanguage { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime LastUsedTime { get; set; }
        public int UseCount { get; set; }
    }

    #endregion
}