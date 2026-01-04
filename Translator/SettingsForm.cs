using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Translator
{
    public partial class SettingsForm : Form
    {
        private string sqlServerConnectionString = "Server=.;Database=Translator;Integrated Security=True;";
        private string sqliteConnectionString = "Data Source=Translator.db;Version=3;";
        private string appDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Translator");

        public SettingsForm()
        {
            InitializeComponent();
            InitializeSettings();
            SetupFormStyle();
        }
        private void SetupFormStyle()
        {
            // 设置窗体边框样式
            this.FormBorderStyle = FormBorderStyle.None;

            // 设置窗体背景色为白色
            this.BackColor = Color.White;

            // 确保所有控件正确显示
            this.Invalidate();
            this.Update();
        }
        // 在 SettingsForm.cs 中修改 InitializeSettings 方法
        private void InitializeSettings()
        {
            // 创建应用数据目录
            if (!Directory.Exists(appDataPath))
            {
                Directory.CreateDirectory(appDataPath);
            }

            // 设置默认备份路径
            string defaultBackupPath = Path.Combine(appDataPath, "Backups");
            if (!Directory.Exists(defaultBackupPath))
            {
                Directory.CreateDirectory(defaultBackupPath);
            }
            txtBackupPath.Text = defaultBackupPath;

            // 检查SQLite数据库状态
            CheckSqliteDatabase();

            // 显示版本信息
            lblVersion.Text = $"版本 {Application.ProductVersion}";

            // 加载保存的设置
            LoadSettings();
        }


        // 添加 LoadSettings 方法
        private void LoadSettings()
        {
            try
            {
                // 从 ApplicationSettings 中读取设置
                cbAutoBackup.Checked = ApplicationSettings.GetBool("AutoBackup", true);
                cbAutoUpdate.Checked = ApplicationSettings.GetBool("AutoUpdate", true);

                // 加载备份路径
                string backupPath = ApplicationSettings.Get("BackupPath", "");
                if (!string.IsNullOrEmpty(backupPath))
                {
                    // 如果路径不存在，使用默认值
                    if (!Directory.Exists(backupPath))
                    {
                        backupPath = Path.Combine(appDataPath, "Backups");
                        ApplicationSettings.Set("BackupPath", backupPath);
                        Directory.CreateDirectory(backupPath);
                    }
                    txtBackupPath.Text = backupPath;
                }
                else
                {
                    // 使用默认路径
                    string defaultPath = Path.Combine(appDataPath, "Backups");
                    if (!Directory.Exists(defaultPath))
                    {
                        Directory.CreateDirectory(defaultPath);
                    }
                    txtBackupPath.Text = defaultPath;
                    ApplicationSettings.Set("BackupPath", defaultPath);
                }

                // 加载备份设置
                string backupFrequency = ApplicationSettings.Get("BackupFrequency", "Weekly");
                string backupDay = ApplicationSettings.Get("BackupDay", "Sunday");
                int backupHour = ApplicationSettings.GetInt("BackupHour", 2); // 默认凌晨2点
            }
            catch (Exception ex)
            {
                Console.WriteLine($"加载设置失败: {ex.Message}");
            }
        }

        private void btnSaveSettings_Click(object sender, EventArgs e)
        {
            try
            {
                // 保存设置
                ApplicationSettings.SetBool("AutoBackup", cbAutoBackup.Checked);
                ApplicationSettings.SetBool("AutoUpdate", cbAutoUpdate.Checked);

                // 保存备份路径
                if (Directory.Exists(txtBackupPath.Text))
                {
                    ApplicationSettings.Set("BackupPath", txtBackupPath.Text);
                }

                // 如果启用了自动备份，设置定时任务
                if (cbAutoBackup.Checked)
                {
                    //SetupAutoBackup();
                }
                else
                {
                    //RemoveAutoBackup();
                }

                MessageBox.Show("设置保存成功！", "保存成功",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                UpdateStatus("设置已保存");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"保存设置失败: {ex.Message}", "错误",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void SettingsForm_Load(object sender, EventArgs e)
        {
            UpdateStatus("就绪");

            // 设置窗体的启动位置为居中显示
            if (this.Owner != null)
            {
                this.Location = new Point(
                    this.Owner.Location.X + (this.Owner.Width - this.Width) / 2,
                    this.Owner.Location.Y + (this.Owner.Height - this.Height) / 2
                );
            }
        }

        private void btnTestConnection_Click(object sender, EventArgs e)
        {
            try
            {
                ShowProgress(true, "正在测试数据库连接...");

                using (SqlConnection connection = new SqlConnection(sqlServerConnectionString))
                {
                    connection.Open();
                    MessageBox.Show("SQL Server 数据库连接成功！", "连接测试",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    connection.Close();
                }

                UpdateStatus("数据库连接成功");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"SQL Server 数据库连接失败: {ex.Message}\n\n" +
                    "将使用 SQLite 数据库作为备用。", "连接失败",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                UpdateStatus($"连接失败: {ex.Message}");
            }
            finally
            {
                ShowProgress(false, "");
            }
        }

        // 修改 btnSelectBackup_Click 方法，保存选择的路径
        private void btnSelectBackup_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog dialog = new FolderBrowserDialog())
            {
                dialog.Description = "选择备份保存路径";
                dialog.SelectedPath = txtBackupPath.Text;
                dialog.ShowNewFolderButton = true;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    txtBackupPath.Text = dialog.SelectedPath;

                    // 立即保存路径设置
                    AppSettingsManager.SaveSetting("BackupPath", dialog.SelectedPath);
                    UpdateStatus($"备份路径已保存: {dialog.SelectedPath}");
                }
            }
        }

        private void btnBackup_Click(object sender, EventArgs e)
        {
            try
            {
                if (!Directory.Exists(txtBackupPath.Text))
                {
                    Directory.CreateDirectory(txtBackupPath.Text);
                }

                string backupFileName = $"Translator_Backup_{DateTime.Now:yyyyMMdd_HHmmss}.bak";
                string backupPath = Path.Combine(txtBackupPath.Text, backupFileName);

                ShowProgress(true, "正在备份数据库...");

                using (SqlConnection connection = new SqlConnection(sqlServerConnectionString))
                {
                    connection.Open();

                    string backupQuery = $"BACKUP DATABASE [Translator] TO DISK = '{backupPath}'";

                    using (SqlCommand command = new SqlCommand(backupQuery, connection))
                    {
                        command.CommandTimeout = 300; // 5分钟超时
                        command.ExecuteNonQuery();
                    }

                    connection.Close();
                }

                MessageBox.Show($"数据库备份成功！\n备份文件: {backupPath}", "备份成功",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                UpdateStatus($"备份完成: {backupFileName}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"备份失败: {ex.Message}", "错误",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                UpdateStatus($"备份失败: {ex.Message}");
            }
            finally
            {
                ShowProgress(false, "");
            }
        }

        private void btnSelectRestore_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Filter = "SQL Server 备份文件 (*.bak)|*.bak|所有文件 (*.*)|*.*";
                dialog.Title = "选择数据库备份文件";
                dialog.InitialDirectory = appDataPath;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    txtRestorePath.Text = dialog.FileName;
                }
            }
        }

        private void btnRestore_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtRestorePath.Text) || !File.Exists(txtRestorePath.Text))
            {
                MessageBox.Show("请选择有效的备份文件", "提示",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show("此操作将覆盖现有数据库，确定要还原吗？", "警告",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
            {
                return;
            }

            try
            {
                ShowProgress(true, "正在还原数据库...");

                using (SqlConnection connection = new SqlConnection(sqlServerConnectionString))
                {
                    connection.Open();

                    // 断开其他连接
                    string killConnections = @"
                        DECLARE @kill varchar(8000) = '';  
                        SELECT @kill = @kill + 'kill ' + CONVERT(varchar(5), spid) + ';'  
                        FROM master..sysprocesses  
                        WHERE dbid = db_id('Translator') AND spid > 50;
                        EXEC(@kill);";

                    using (SqlCommand killCommand = new SqlCommand(killConnections, connection))
                    {
                        killCommand.ExecuteNonQuery();
                    }

                    // 还原数据库
                    string restoreQuery = @"
                        RESTORE DATABASE [Translator] 
                        FROM DISK = @backupPath
                        WITH REPLACE, RECOVERY;
                    ";

                    using (SqlCommand command = new SqlCommand(restoreQuery, connection))
                    {
                        command.Parameters.AddWithValue("@backupPath", txtRestorePath.Text);
                        command.CommandTimeout = 600; // 10分钟超时
                        command.ExecuteNonQuery();
                    }

                    connection.Close();
                }

                MessageBox.Show("数据库还原成功！", "还原成功",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                UpdateStatus($"还原完成: {Path.GetFileName(txtRestorePath.Text)}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"还原失败: {ex.Message}\n\n" +
                    "请确保没有其他程序正在使用数据库。", "错误",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                UpdateStatus($"还原失败: {ex.Message}");
            }
            finally
            {
                ShowProgress(false, "");
            }
        }

        private void btnSyncToSqlite_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("此操作将把 SQL Server 数据同步到 SQLite 数据库。确定继续吗？", "确认同步",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            {
                return;
            }

            try
            {
                ShowProgress(true, "正在同步到 SQLite 数据库...");

                // 创建或更新 SQLite 数据库
                CreateOrUpdateSqliteDatabase();

                // 同步数据
                SyncDataToSqlite();

                MessageBox.Show("数据同步到 SQLite 完成！\n\n" +
                    "现在您可以在没有 SQL Server 的环境中使用本软件。", "同步成功",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                UpdateStatus("SQLite 同步完成");
                CheckSqliteDatabase();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"同步失败: {ex.Message}", "错误",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                UpdateStatus($"同步失败: {ex.Message}");
            }
            finally
            {
                ShowProgress(false, "");
            }
        }

        private void CreateOrUpdateSqliteDatabase()
        {
            try
            {
                string sqlitePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Translator.db");

                // 检查 SQLite 连接是否可用
                if (!TestSqliteConnection())
                {
                    throw new Exception("SQLite 连接不可用，请确保 SQLite 运行时库已正确安装。");
                }

                // 如果文件存在但大小为0，删除它
                if (File.Exists(sqlitePath))
                {
                    FileInfo fileInfo = new FileInfo(sqlitePath);
                    if (fileInfo.Length == 0)
                    {
                        File.Delete(sqlitePath);
                    }
                }

                // 创建数据库文件（如果不存在）
                if (!File.Exists(sqlitePath))
                {
                    try
                    {
                        SQLiteConnection.CreateFile(sqlitePath);
                        UpdateStatus("SQLite 数据库文件已创建");
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"创建数据库文件失败: {ex.Message}", ex);
                    }
                }

                using (SQLiteConnection connection = new SQLiteConnection($"Data Source={sqlitePath};Version=3;"))
                {
                    connection.Open();

                    // 检查数据库是否有效
                    try
                    {
                        using (var testCommand = new SQLiteCommand("SELECT 1", connection))
                        {
                            testCommand.ExecuteScalar();
                        }
                    }
                    catch
                    {
                        throw new Exception("数据库文件已存在但不是有效的 SQLite 数据库");
                    }

                    // 创建翻译缓存表
                    string createTableQuery = @"
                CREATE TABLE IF NOT EXISTS TranslationCache (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    SourceText TEXT NOT NULL,
                    SourceLanguage TEXT NOT NULL,
                    TargetLanguage TEXT NOT NULL,
                    TranslatedText TEXT NOT NULL,
                    CreatedTime DATETIME DEFAULT CURRENT_TIMESTAMP,
                    LastUsedTime DATETIME DEFAULT CURRENT_TIMESTAMP,
                    UseCount INTEGER DEFAULT 1
                );
                
                CREATE INDEX IF NOT EXISTS idx_Search 
                ON TranslationCache(SourceText, SourceLanguage, TargetLanguage);
            ";

                    using (SQLiteCommand command = new SQLiteCommand(createTableQuery, connection))
                    {
                        command.ExecuteNonQuery();
                    }

                    UpdateStatus("SQLite 表结构已创建");
                }
            }
            catch (SQLiteException ex)
            {
                // 处理特定的 SQLite 错误
                if (ex.Message.Contains("could not open database"))
                {
                    throw new Exception("无法打开数据库文件，可能被其他进程占用或权限不足", ex);
                }
                else if (ex.Message.Contains("not a database"))
                {
                    throw new Exception("文件不是有效的 SQLite 数据库", ex);
                }
                else
                {
                    throw;
                }
            }
            catch (DllNotFoundException ex)
            {
                throw new Exception("SQLite 运行时库未找到。请安装 System.Data.SQLite 包或手动添加 sqlite3.dll。", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"创建或更新 SQLite 数据库失败: {ex.Message}", ex);
            }
        }

        private bool TestSqliteConnection()
        {
            try
            {
                // 尝试使用内存数据库测试连接
                using (var connection = new SQLiteConnection("Data Source=:memory:;Version=3;"))
                {
                    connection.Open();
                    using (var command = new SQLiteCommand("SELECT 1", connection))
                    {
                        var result = command.ExecuteScalar();
                        return result != null;
                    }
                }
            }
            catch (DllNotFoundException ex)
            {
                UpdateStatus($"缺少 SQLite 运行时库: {ex.Message}");
                return false;
            }
            catch (SQLiteException ex)
            {
                UpdateStatus($"SQLite 错误: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                UpdateStatus($"测试连接失败: {ex.Message}");
                return false;
            }
        }

        private void SyncDataToSqlite()
        {
            string sqlitePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Translator.db");

            using (SQLiteConnection sqliteConn = new SQLiteConnection($"Data Source={sqlitePath};Version=3;"))
            using (SqlConnection sqlConn = new SqlConnection(sqlServerConnectionString))
            {
                sqliteConn.Open();
                sqlConn.Open();

                // 清空 SQLite 表
                using (SQLiteCommand clearCommand = new SQLiteCommand("DELETE FROM TranslationCache", sqliteConn))
                {
                    clearCommand.ExecuteNonQuery();
                }

                // 从 SQL Server 读取数据
                string selectQuery = "SELECT * FROM TranslationCache";
                using (SqlCommand selectCommand = new SqlCommand(selectQuery, sqlConn))
                using (SqlDataReader reader = selectCommand.ExecuteReader())
                {
                    int batchSize = 1000;
                    int count = 0;
                    bool useTransaction = false;
                    SQLiteTransaction transaction = null;

                    try
                    {
                        // 开始事务
                        transaction = sqliteConn.BeginTransaction();
                        useTransaction = true;

                        while (reader.Read())
                        {
                            string insertQuery = @"
                        INSERT INTO TranslationCache 
                        (SourceText, SourceLanguage, TargetLanguage, TranslatedText, 
                         CreatedTime, LastUsedTime, UseCount)
                        VALUES (@SourceText, @SourceLanguage, @TargetLanguage, @TranslatedText,
                                @CreatedTime, @LastUsedTime, @UseCount)
                    ";

                            using (SQLiteCommand insertCommand = new SQLiteCommand(insertQuery, sqliteConn))
                            {
                                insertCommand.Parameters.AddWithValue("@SourceText", reader["SourceText"]);
                                insertCommand.Parameters.AddWithValue("@SourceLanguage", reader["SourceLanguage"]);
                                insertCommand.Parameters.AddWithValue("@TargetLanguage", reader["TargetLanguage"]);
                                insertCommand.Parameters.AddWithValue("@TranslatedText", reader["TranslatedText"]);
                                insertCommand.Parameters.AddWithValue("@CreatedTime", reader["CreatedTime"]);
                                insertCommand.Parameters.AddWithValue("@LastUsedTime", reader["LastUsedTime"]);
                                insertCommand.Parameters.AddWithValue("@UseCount", reader["UseCount"]);

                                insertCommand.ExecuteNonQuery();
                            }

                            count++;

                            // 分批提交，提高性能
                            if (count % batchSize == 0 && useTransaction)
                            {
                                transaction.Commit();
                                transaction.Dispose();
                                transaction = sqliteConn.BeginTransaction(); // 开始新的事务

                                // 更新进度显示
                                UpdateStatus($"已同步 {count} 条记录...");
                                Application.DoEvents();
                            }
                        }

                        // 提交最后一批
                        if (useTransaction && transaction != null)
                        {
                            transaction.Commit();
                        }

                        UpdateStatus($"总共同步了 {count} 条记录");
                    }
                    catch (Exception ex)
                    {
                        if (useTransaction && transaction != null)
                        {
                            transaction.Rollback();
                        }
                        throw;
                    }
                    finally
                    {
                        if (transaction != null)
                        {
                            transaction.Dispose();
                        }
                    }
                }
            }
        }

        private void CheckSqliteDatabase()
        {
            try
            {
                string sqlitePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Translator.db");

                if (!File.Exists(sqlitePath))
                {
                    lblSqliteStatus.Text = "SQLite状态：数据库不存在，需要创建";
                    lblSqliteStatus.ForeColor = Color.Red;
                }
                else
                {
                    // 检查文件大小
                    FileInfo fileInfo = new FileInfo(sqlitePath);
                    if (fileInfo.Length == 0)
                    {
                        lblSqliteStatus.Text = "SQLite状态：数据库文件为空";
                        lblSqliteStatus.ForeColor = Color.Orange;
                        return;
                    }

                    // 尝试连接并检查
                    try
                    {
                        using (var connection = new SQLiteConnection($"Data Source={sqlitePath};Version=3;"))
                        {
                            connection.Open();

                            // 检查表是否存在
                            string checkTableQuery = "SELECT name FROM sqlite_master WHERE type='table' AND name='TranslationCache'";
                            using (var command = new SQLiteCommand(checkTableQuery, connection))
                            {
                                object result = command.ExecuteScalar();

                                if (result == null)
                                {
                                    lblSqliteStatus.Text = "SQLite状态：表结构不完整，需要同步";
                                    lblSqliteStatus.ForeColor = Color.Orange;
                                }
                                else
                                {
                                    // 获取记录数量
                                    string countQuery = "SELECT COUNT(*) FROM TranslationCache";
                                    using (var countCommand = new SQLiteCommand(countQuery, connection))
                                    {
                                        int recordCount = Convert.ToInt32(countCommand.ExecuteScalar());
                                        lblSqliteStatus.Text = $"SQLite状态：正常 ({recordCount} 条记录)";
                                        lblSqliteStatus.ForeColor = Color.Green;
                                    }
                                }
                            }
                        }
                    }
                    catch (SQLiteException ex)
                    {
                        if (ex.Message.Contains("not a database"))
                        {
                            lblSqliteStatus.Text = "SQLite状态：数据库文件损坏或格式不正确";
                            lblSqliteStatus.ForeColor = Color.Red;
                        }
                        else
                        {
                            lblSqliteStatus.Text = $"SQLite状态：连接错误 - {ex.Message}";
                            lblSqliteStatus.ForeColor = Color.Red;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lblSqliteStatus.Text = $"SQLite状态：检查失败 - {ex.Message}";
                lblSqliteStatus.ForeColor = Color.Red;
            }
        }

        // 修改 btnSaveSettings_Click 方法
        //private void btnSaveSettings_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        // 保存设置
        //        AppSettingsManager.SaveBoolSetting("AutoBackup", cbAutoBackup.Checked);
        //        AppSettingsManager.SaveBoolSetting("AutoUpdate", cbAutoUpdate.Checked);

        //        // 保存备份路径
        //        if (Directory.Exists(txtBackupPath.Text))
        //        {
        //            AppSettingsManager.SaveSetting("BackupPath", txtBackupPath.Text);
        //        }

        //        MessageBox.Show("设置保存成功！", "保存成功",
        //            MessageBoxButtons.OK, MessageBoxIcon.Information);

        //        UpdateStatus("设置已保存");
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show($"保存设置失败: {ex.Message}", "错误",
        //            MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    }
        //}

        private void ShowProgress(bool show, string message)
        {
            progressBar.Visible = show;
            progressBar.Style = show ? ProgressBarStyle.Marquee : ProgressBarStyle.Blocks;

            if (!string.IsNullOrEmpty(message))
            {
                UpdateStatus(message);
            }
        }

        private void UpdateStatus(string message)
        {
            lblStatus.Text = message;
            Application.DoEvents();
        }
    }
}