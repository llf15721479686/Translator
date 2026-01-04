create database Translator
use Translator

CREATE TABLE TranslationCache
(
    Id INT PRIMARY KEY IDENTITY(1,1),
    SourceText NVARCHAR(500) NOT NULL,
    SourceLanguage NVARCHAR(50) NOT NULL,
    TargetLanguage NVARCHAR(50) NOT NULL,
    TranslatedText NVARCHAR(MAX) NOT NULL,
    CreatedTime DATETIME NOT NULL DEFAULT GETDATE(),
    LastUsedTime DATETIME NOT NULL DEFAULT GETDATE(),
    UseCount INT NOT NULL DEFAULT 1,
    
    CONSTRAINT UQ_SourceTarget UNIQUE (SourceText, SourceLanguage, TargetLanguage)
);

-- 创建索引以提高查询性能
CREATE INDEX IX_SourceText ON TranslationCache(SourceText);
CREATE INDEX IX_LanguagePair ON TranslationCache(SourceLanguage, TargetLanguage);

-- 周一&周二&周三&周四&周五&周六&周日&一月&二月&三月&四月&五月&六月&七月&八月&九月&十月&十一月&十二月&中国&美国&日本

select * from TranslationCache



-- 创建用户定义表类型
CREATE TYPE dbo.TranslationBatchType AS TABLE
(
    SourceText NVARCHAR(500),
    TranslatedText NVARCHAR(MAX)
);

-- 创建存储过程用于批量保存
CREATE PROCEDURE sp_SaveBatchTranslations
    @Translations TranslationBatchType READONLY,
    @SourceLanguage NVARCHAR(50),
    @TargetLanguage NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    
    -- 更新已存在的记录
    UPDATE tc
    SET tc.TranslatedText = t.TranslatedText,
        tc.LastUsedTime = GETDATE(),
        tc.UseCount = tc.UseCount + 1
    FROM TranslationCache tc
    INNER JOIN @Translations t ON tc.SourceText = t.SourceText
    WHERE tc.SourceLanguage = @SourceLanguage 
      AND tc.TargetLanguage = @TargetLanguage;
    
    -- 插入新记录
    INSERT INTO TranslationCache 
        (SourceText, SourceLanguage, TargetLanguage, TranslatedText, CreatedTime, LastUsedTime, UseCount)
    SELECT 
        t.SourceText,
        @SourceLanguage,
        @TargetLanguage,
        t.TranslatedText,
        GETDATE(),
        GETDATE(),
        1
    FROM @Translations t
    WHERE NOT EXISTS (
        SELECT 1 FROM TranslationCache tc
        WHERE tc.SourceText = t.SourceText
          AND tc.SourceLanguage = @SourceLanguage
          AND tc.TargetLanguage = @TargetLanguage
    );
END


