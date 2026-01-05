using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Translator
{
    public partial class Form1 : Form
    {
        private Button btnConfigureLanguages;
        private ProgressBar progressBar;
        private Label lblProgress;

        private LanguageConfigManager languageConfigManager = new LanguageConfigManager();

        private readonly List<LanguageInfo> languages = new List<LanguageInfo>
        {
            new LanguageInfo { Name = "英语", Code = "1033" },
            new LanguageInfo { Name = "阿拉伯语", Code = "1025" },
            new LanguageInfo { Name = "德语", Code = "1031" },
            new LanguageInfo { Name = "法语", Code = "1036" },
            new LanguageInfo { Name = "意大利语", Code = "1040" },
            new LanguageInfo { Name = "葡萄牙语", Code = "1046" },
            new LanguageInfo { Name = "俄语", Code = "1049" },
            new LanguageInfo { Name = "泰语", Code = "1054" },
            new LanguageInfo { Name = "印度尼西亚语", Code = "1057" },
            new LanguageInfo { Name = "越南语", Code = "1066" },
            new LanguageInfo { Name = "马来西亚", Code = "1086" },
            new LanguageInfo { Name = "汉语", Code = "2052" },
            new LanguageInfo { Name = "西班牙语", Code = "3082" }
        };

        private const string Separator = "\r\n";

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            InitializeLayout();
            languageConfigManager.LoadConfig();
        }

        private void InitializeLayout()
        {
            txtSource.Height = 120;
            txtSource.Top = 30;
            txtSource.Left = 10;
            txtSource.Width = this.ClientSize.Width - 20;

            int buttonWidth = 140;
            int buttonHeight = 35;
            int buttonSpacing = 10;
            int buttonTop = txtSource.Bottom + 15;
            int buttonLeft = 10;

            btnTranslate.Location = new Point(buttonLeft, buttonTop);
            btnTranslate.Size = new Size(buttonWidth, buttonHeight);
            buttonLeft += buttonWidth + buttonSpacing;

            btnCopyAll.Location = new Point(buttonLeft, buttonTop);
            btnCopyAll.Size = new Size(buttonWidth, buttonHeight);
            buttonLeft += buttonWidth + buttonSpacing;

            btnGenerateMessages.Location = new Point(buttonLeft, buttonTop);
            btnGenerateMessages.Size = new Size(buttonWidth, buttonHeight);
            buttonLeft += buttonWidth + buttonSpacing;

            btnConfigureLanguages = new Button
            {
                Text = "翻译列配置",
                Font = new Font("微软雅黑", 10F),
                Location = new Point(buttonLeft, buttonTop),
                Size = new Size(buttonWidth, buttonHeight),
                UseVisualStyleBackColor = true
            };
            btnConfigureLanguages.Click += btnConfigureLanguages_Click;
            this.Controls.Add(btnConfigureLanguages);

            int progressBarTop = buttonTop + buttonHeight + 10;
            int progressBarWidth = this.ClientSize.Width - 40;

            progressBar = new ProgressBar
            {
                Location = new Point(20, progressBarTop),
                Size = new Size(progressBarWidth, 20),
                Visible = false,
                Style = ProgressBarStyle.Continuous
            };
            this.Controls.Add(progressBar);

            lblProgress = new Label
            {
                Location = new Point(20, progressBarTop + 25),
                Size = new Size(progressBarWidth, 20),
                Text = "",
                Font = new Font("微软雅黑", 9F),
                TextAlign = ContentAlignment.MiddleLeft,
                Visible = false
            };
            this.Controls.Add(lblProgress);

            dgvTranslations.Top = progressBarTop + 50;
            dgvTranslations.Left = 10;
            dgvTranslations.Width = this.ClientSize.Width - 20;
            dgvTranslations.Height = this.ClientSize.Height - dgvTranslations.Top - 10;

            AdjustButtonLayoutIfNeeded();
        }

        private void AdjustButtonLayoutIfNeeded()
        {
            int buttonWidth = 140;
            int buttonHeight = 35;
            int buttonSpacing = 10;
            int buttonTop = txtSource.Bottom + 15;
            int buttonLeft = 10;

            int totalButtonsWidth = buttonWidth * 4 + buttonSpacing * 3 + 20;

            if (totalButtonsWidth > this.ClientSize.Width - 20)
            {
                btnTranslate.Location = new Point(buttonLeft, buttonTop);
                buttonLeft += buttonWidth + buttonSpacing;

                btnCopyAll.Location = new Point(buttonLeft, buttonTop);
                buttonLeft += buttonWidth + buttonSpacing;

                btnGenerateMessages.Location = new Point(buttonLeft, buttonTop);
                buttonLeft += buttonWidth + buttonSpacing;

                btnConfigureLanguages.Location = new Point(buttonLeft, buttonTop);

                progressBar.Top = buttonTop + buttonHeight + 10;
                lblProgress.Top = progressBar.Top + 25;
                dgvTranslations.Top = progressBar.Top + 50;
            }
        }

        private void ShowProgressBar(string initialText = "准备翻译...", int totalSteps = 100)
        {
            this.Invoke((MethodInvoker)delegate
            {
                progressBar.Visible = true;
                progressBar.Value = 0;
                progressBar.Maximum = totalSteps;
                lblProgress.Visible = true;
                lblProgress.Text = initialText;

                dgvTranslations.Top = progressBar.Top + 50;
                dgvTranslations.Height = this.ClientSize.Height - dgvTranslations.Top - 10;

                progressBar.Refresh();
                lblProgress.Refresh();
                Application.DoEvents();
            });
        }

        private void UpdateProgress(int value, string status)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<int, string>(UpdateProgress), value, status);
                return;
            }

            if (value < 0) value = 0;
            if (value > progressBar.Maximum) value = progressBar.Maximum;

            progressBar.Value = value;
            lblProgress.Text = $"{status} ({value}/{progressBar.Maximum})";

            progressBar.Refresh();
            lblProgress.Refresh();
            Application.DoEvents();
        }

        private void HideProgressBar()
        {
            this.Invoke((MethodInvoker)delegate
            {
                progressBar.Visible = false;
                lblProgress.Visible = false;

                int buttonRowBottom = btnConfigureLanguages.Bottom;
                dgvTranslations.Top = buttonRowBottom + 15;
                dgvTranslations.Height = this.ClientSize.Height - dgvTranslations.Top - 10;
            });
        }

        private class ProgressState
        {
            public int CurrentStep { get; set; }
            public int TotalSteps { get; set; }
        }

        private void btnConfigureLanguages_Click(object sender, EventArgs e)
        {
            ShowLanguageConfiguration();
        }

        private void ShowLanguageConfiguration()
        {
            using (var configForm = new LanguageConfigForm(languageConfigManager, languages))
            {
                if (configForm.ShowDialog() == DialogResult.OK)
                {
                    languageConfigManager.SaveConfig();
                    MessageBox.Show("翻译列配置已保存", "提示",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private List<LanguageInfo> GetEnabledLanguages()
        {
            var enabledLanguages = new List<LanguageInfo>();
            var config = languageConfigManager.GetLanguageConfig();

            foreach (var language in languages)
            {
                if (config.ContainsKey(language.Name) && config[language.Name])
                {
                    enabledLanguages.Add(language);
                }
            }

            if (enabledLanguages.Count == 0)
            {
                enabledLanguages.Add(languages.First(l => l.Name == "英语"));
                enabledLanguages.Add(languages.First(l => l.Name == "汉语"));
            }

            return enabledLanguages;
        }

        private async Task TranslateSelectedLanguages()
        {
            string inputText = txtSource.Text.Trim();
            if (string.IsNullOrWhiteSpace(inputText))
            {
                MessageBox.Show("请输入要翻译的中文文本", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string[] words = inputText.Split(new[] { Separator }, StringSplitOptions.RemoveEmptyEntries)
                                     .Select(w => w.Trim())
                                     .Where(w => !string.IsNullOrWhiteSpace(w))
                                     .ToArray();

            if (words.Length == 0)
            {
                MessageBox.Show("没有找到有效的词汇", "提示",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var enabledLanguages = GetEnabledLanguages();
            InitializeDataGridView(enabledLanguages);

            for (int i = 0; i < words.Length; i++)
            {
                dgvTranslations.Rows.Add();
            }

            for (int row = 0; row < words.Length; row++)
            {
                for (int col = 0; col < dgvTranslations.Columns.Count; col++)
                {
                    string columnName = dgvTranslations.Columns[col].HeaderText;
                    string languageName = columnName.Substring(0, columnName.IndexOf('['));

                    if (languageName == "汉语")
                    {
                        dgvTranslations.Rows[row].Cells[col].Value = words[row];
                    }
                    else
                    {
                        dgvTranslations.Rows[row].Cells[col].Value = "等待翻译...";
                    }
                }
            }

            btnTranslate.Enabled = false;
            btnCopyAll.Enabled = false;
            btnConfigureLanguages.Enabled = false;
            btnGenerateMessages.Enabled = false;
            btnTranslate.Text = "翻译中...";

            int totalLanguagesToTranslate = enabledLanguages.Count(l => l.Name != "汉语");
            int totalSteps = words.Length * totalLanguagesToTranslate;

            var progressState = new ProgressState { TotalSteps = totalSteps, CurrentStep = 0 };

            ShowProgressBar("开始翻译...", totalSteps);

            try
            {
                for (int col = 0; col < dgvTranslations.Columns.Count; col++)
                {
                    string columnName = dgvTranslations.Columns[col].HeaderText;
                    string languageName = columnName.Substring(0, columnName.IndexOf('['));

                    if (languageName == "汉语")
                        continue;

                    UpdateProgress(progressState.CurrentStep, $"正在准备翻译{languageName}...");

                    for (int row = 0; row < words.Length; row++)
                    {
                        dgvTranslations.Rows[row].Cells[col].Value = "正在翻译...";
                    }
                    dgvTranslations.Refresh();
                    Application.DoEvents();

                    await TranslateWordsForLanguageWithProgress(words, languageName, col, progressState);
                    await Task.Delay(1000);
                }

                UpdateProgress(totalSteps, "翻译完成！");
                await Task.Delay(500);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"翻译出错: {ex.Message}\n\n请检查网络连接或API配额",
                              "错误",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Error);
            }
            finally
            {
                btnTranslate.Enabled = true;
                btnTranslate.Text = "一键翻译";
                btnCopyAll.Enabled = true;
                btnConfigureLanguages.Enabled = true;
                btnGenerateMessages.Enabled = true;
                HideProgressBar();
            }
        }

        private async Task TranslateWordsForLanguageWithProgress(string[] words, string targetLanguage,
            int columnIndex, ProgressState progressState)
        {
            if (targetLanguage == "汉语")
                return;

            var cachedTranslations = DatabaseHelper.GetBatchCachedTranslations(words.ToList(), "中文", targetLanguage);

            var wordsToTranslate = new List<string>();
            var wordIndexMap = new Dictionary<string, List<int>>();

            for (int row = 0; row < words.Length; row++)
            {
                string word = words[row];

                if (cachedTranslations.TryGetValue(word, out string cachedResult))
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        dgvTranslations.Rows[row].Cells[columnIndex].Value = cachedResult;
                        dgvTranslations.Refresh();
                    });

                    progressState.CurrentStep++;
                    UpdateProgress(progressState.CurrentStep, $"{targetLanguage}: 第{row + 1}/{words.Length}个词汇（缓存）");
                }
                else
                {
                    if (!wordIndexMap.ContainsKey(word))
                    {
                        wordIndexMap[word] = new List<int>();
                        wordsToTranslate.Add(word);
                    }
                    wordIndexMap[word].Add(row);
                }
            }

            if (wordsToTranslate.Count == 0)
                return;

            bool hasSlash = wordsToTranslate.Any(word => word.Contains("/") || word.Contains("\\"));
            bool useYoudao = targetLanguage == "印度尼西亚语" ||
                             targetLanguage == "马来西亚" ||
                             hasSlash;

            if (useYoudao)
            {
                await TranslateWithYoudaoWithProgress(wordsToTranslate, wordIndexMap, columnIndex,
                    targetLanguage, progressState);
            }
            else
            {
                await TranslateWithBaiduBatchWithProgress(wordsToTranslate, wordIndexMap, columnIndex,
                    targetLanguage, progressState);
            }
        }

        private async Task TranslateWithBaiduBatchWithProgress(
            List<string> wordsToTranslate,
            Dictionary<string, List<int>> wordIndexMap,
            int columnIndex,
            string targetLanguage,
            ProgressState progressState)
        {
            try
            {
                var batchResults = await Task.Run(() =>
                    BaiduTranslatorHelper.BatchTranslateWithoutCache(wordsToTranslate, "中文", targetLanguage));

                var successfulTranslations = new Dictionary<string, string>();
                int translatedCount = 0;

                foreach (var word in wordsToTranslate)
                {
                    if (batchResults.TryGetValue(word, out string result))
                    {
                        if (wordIndexMap.TryGetValue(word, out List<int> rows))
                        {
                            this.Invoke((MethodInvoker)delegate
                            {
                                foreach (int row in rows)
                                {
                                    dgvTranslations.Rows[row].Cells[columnIndex].Value = result;
                                }
                                dgvTranslations.Refresh();
                            });
                        }

                        if (!result.Contains("翻译失败") && !result.Contains("翻译API错误"))
                        {
                            successfulTranslations[word] = result;
                        }

                        translatedCount++;
                        progressState.CurrentStep++;
                        UpdateProgress(progressState.CurrentStep, $"{targetLanguage}: {translatedCount}/{wordsToTranslate.Count}个词汇");
                    }
                }

                if (successfulTranslations.Count > 0)
                {
                    await Task.Run(() =>
                        DatabaseHelper.SaveBatchTranslations(successfulTranslations, "中文", targetLanguage));
                }
            }
            catch (Exception ex)
            {
                await TranslateWithBaiduSingleWithProgress(wordsToTranslate, wordIndexMap, columnIndex,
                    targetLanguage, progressState);
            }
        }

        private async Task TranslateWithBaiduSingleWithProgress(
            List<string> wordsToTranslate,
            Dictionary<string, List<int>> wordIndexMap,
            int columnIndex,
            string targetLanguage,
            ProgressState progressState)
        {
            var successfulTranslations = new Dictionary<string, string>();

            int groupSize = 5;
            for (int i = 0; i < wordsToTranslate.Count; i += groupSize)
            {
                var group = wordsToTranslate.Skip(i).Take(groupSize).ToList();

                var tasks = new List<Task>();
                var groupResults = new ConcurrentDictionary<string, string>();

                for (int j = 0; j < group.Count; j += 2)
                {
                    int start = j;
                    int end = Math.Min(j + 2, group.Count);

                    var task = Task.Run(async () =>
                    {
                        for (int k = start; k < end; k++)
                        {
                            string word = group[k];
                            try
                            {
                                string result = BaiduTranslatorHelper.TranslateWithoutCache(word, "中文", targetLanguage);
                                groupResults[word] = result;

                                if (wordIndexMap.TryGetValue(word, out List<int> rows))
                                {
                                    this.Invoke((MethodInvoker)delegate
                                    {
                                        foreach (int row in rows)
                                        {
                                            dgvTranslations.Rows[row].Cells[columnIndex].Value = result;
                                        }
                                        dgvTranslations.Refresh();
                                    });
                                }

                                if (!result.Contains("翻译失败") && !result.Contains("翻译API错误"))
                                {
                                    lock (successfulTranslations)
                                    {
                                        successfulTranslations[word] = result;
                                    }
                                }

                                lock (progressState)
                                {
                                    progressState.CurrentStep++;
                                    UpdateProgress(progressState.CurrentStep, $"{targetLanguage}: {i + k + 1}/{wordsToTranslate.Count}个词汇");
                                }
                            }
                            catch (Exception ex)
                            {
                                string errorMessage = $"{targetLanguage}翻译失败: {ex.Message}";
                                groupResults[word] = errorMessage;

                                if (wordIndexMap.TryGetValue(word, out List<int> rows))
                                {
                                    this.Invoke((MethodInvoker)delegate
                                    {
                                        foreach (int row in rows)
                                        {
                                            dgvTranslations.Rows[row].Cells[columnIndex].Value = errorMessage;
                                        }
                                        dgvTranslations.Refresh();
                                    });
                                }

                                lock (progressState)
                                {
                                    progressState.CurrentStep++;
                                    UpdateProgress(progressState.CurrentStep, $"{targetLanguage}: {i + k + 1}/{wordsToTranslate.Count}个词汇（失败）");
                                }
                            }
                        }
                    });

                    tasks.Add(task);

                    if (tasks.Count >= 2)
                    {
                        await Task.WhenAll(tasks);
                        tasks.Clear();

                        if (i + groupSize < wordsToTranslate.Count)
                        {
                            await Task.Delay(1200);
                        }
                    }
                }

                if (tasks.Count > 0)
                {
                    await Task.WhenAll(tasks);
                }
            }

            if (successfulTranslations.Count > 0)
            {
                await Task.Run(() =>
                    DatabaseHelper.SaveBatchTranslations(successfulTranslations, "中文", targetLanguage));
            }
        }

        private async Task TranslateWithYoudaoWithProgress(
            List<string> wordsToTranslate,
            Dictionary<string, List<int>> wordIndexMap,
            int columnIndex,
            string targetLanguage,
            ProgressState progressState)
        {
            var successfulTranslations = new Dictionary<string, string>();

            int maxConcurrent = 3;
            var semaphore = new SemaphoreSlim(maxConcurrent);
            var tasks = new List<Task>();

            for (int i = 0; i < wordsToTranslate.Count; i++)
            {
                var word = wordsToTranslate[i];
                await semaphore.WaitAsync();

                var task = Task.Run(async () =>
                {
                    try
                    {
                        string result = await YoudaoTranslatorHelper.TranslateAsync(word, "中文", targetLanguage);

                        if (wordIndexMap.TryGetValue(word, out List<int> rows))
                        {
                            this.Invoke((MethodInvoker)delegate
                            {
                                foreach (int row in rows)
                                {
                                    dgvTranslations.Rows[row].Cells[columnIndex].Value = result;
                                }
                                dgvTranslations.Refresh();
                            });
                        }

                        if (!result.Contains("翻译失败") &&
                            !result.Contains("有道翻译错误") &&
                            !result.Contains("有道API"))
                        {
                            lock (successfulTranslations)
                            {
                                successfulTranslations[word] = result;
                            }
                        }

                        lock (progressState)
                        {
                            progressState.CurrentStep++;
                            int currentIndex = Array.IndexOf(wordsToTranslate.ToArray(), word) + 1;
                            UpdateProgress(progressState.CurrentStep, $"{targetLanguage}: {currentIndex}/{wordsToTranslate.Count}个词汇");
                        }
                    }
                    catch (Exception ex)
                    {
                        string errorMessage = $"{targetLanguage}翻译失败: {ex.Message}";

                        if (wordIndexMap.TryGetValue(word, out List<int> rows))
                        {
                            this.Invoke((MethodInvoker)delegate
                            {
                                foreach (int row in rows)
                                {
                                    dgvTranslations.Rows[row].Cells[columnIndex].Value = errorMessage;
                                }
                                dgvTranslations.Refresh();
                            });
                        }

                        lock (progressState)
                        {
                            progressState.CurrentStep++;
                            UpdateProgress(progressState.CurrentStep, $"{targetLanguage}: 翻译失败");
                        }
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                });

                tasks.Add(task);
                await Task.Delay(1000);
            }

            await Task.WhenAll(tasks);

            if (successfulTranslations.Count > 0)
            {
                await Task.Run(() =>
                    DatabaseHelper.SaveBatchTranslations(successfulTranslations, "中文", targetLanguage));
            }
        }

        private void InitializeDataGridView(List<LanguageInfo> enabledLanguages)
        {
            dgvTranslations.Rows.Clear();
            dgvTranslations.Columns.Clear();

            dgvTranslations.RowHeadersVisible = false;
            dgvTranslations.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvTranslations.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            dgvTranslations.ScrollBars = ScrollBars.Vertical;

            foreach (var language in enabledLanguages)
            {
                DataGridViewTextBoxColumn column = new DataGridViewTextBoxColumn();
                column.HeaderText = $"{language.Name}[{language.Code}]";
                column.Name = $"col{language.Code}";
                column.ReadOnly = true;
                column.FillWeight = 100;
                dgvTranslations.Columns.Add(column);
            }

            dgvTranslations.AutoResizeRows(DataGridViewAutoSizeRowsMode.AllCellsExceptHeaders);
        }

        private void CopyAllToClipboard()
        {
            try
            {
                if (dgvTranslations.Rows.Count == 0)
                {
                    MessageBox.Show("没有可复制的数据", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                StringBuilder sb = new StringBuilder();

                foreach (DataGridViewRow row in dgvTranslations.Rows)
                {
                    for (int i = 0; i < dgvTranslations.Columns.Count; i++)
                    {
                        var cellValue = row.Cells[i].Value?.ToString() ?? "";

                        if (!string.IsNullOrEmpty(cellValue))
                        {
                            cellValue = cellValue.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ");

                            if (cellValue.Contains("\t") || cellValue.Contains("\"") || cellValue.Contains(","))
                            {
                                sb.Append($"\"{cellValue.Replace("\"", "\"\"")}\"");
                            }
                            else
                            {
                                sb.Append(cellValue);
                            }
                        }

                        if (i < dgvTranslations.Columns.Count - 1)
                        {
                            sb.Append("\t");
                        }
                    }
                    sb.AppendLine();
                }

                Clipboard.SetText(sb.ToString());

                MessageBox.Show("所有翻译结果已复制到剪贴板！", "成功",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
            catch (Exception ex)
            {
                MessageBox.Show($"复制失败: {ex.Message}", "错误",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCopyAll_Click(object sender, EventArgs e)
        {
            CopyAllToClipboard();
        }

        private void btnGenerateMessages_Click(object sender, EventArgs e)
        {
            string inputText = txtSource.Text.Trim();
            if (string.IsNullOrWhiteSpace(inputText))
            {
                MessageBox.Show("请先在上方输入要翻译的消息文本", "提示",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string[] messages = inputText.Split(new[] { "\r\n", "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries)
                                        .Select(m => m.Trim())
                                        .Where(m => !string.IsNullOrWhiteSpace(m))
                                        .ToArray();

            if (messages.Length == 0)
            {
                MessageBox.Show("没有找到有效的消息文本", "提示",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            MessageTranslatorForm form = new MessageTranslatorForm(messages);
            form.ShowDialog();
        }

        private async void btnTranslate_Click(object sender, EventArgs e)
        {
            await TranslateSelectedLanguages();
        }

        public class LanguageConfigManager
        {
            private Dictionary<string, bool> languageConfig = new Dictionary<string, bool>();
            private readonly string configFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "language_config.json");

            private Dictionary<string, bool> defaultConfig = new Dictionary<string, bool>
            {
                { "英语", true },
                { "汉语", true },
                { "阿拉伯语", false },
                { "德语", false },
                { "法语", false },
                { "意大利语", false },
                { "葡萄牙语", false },
                { "俄语", false },
                { "泰语", false },
                { "印度尼西亚语", false },
                { "越南语", false },
                { "马来西亚", false },
                { "西班牙语", false }
            };

            public LanguageConfigManager()
            {
                languageConfig = new Dictionary<string, bool>(defaultConfig);
            }

            public void LoadConfig()
            {
                try
                {
                    if (File.Exists(configFilePath))
                    {
                        string json = File.ReadAllText(configFilePath);
                        var loadedConfig = JsonConvert.DeserializeObject<Dictionary<string, bool>>(json);

                        foreach (var key in defaultConfig.Keys)
                        {
                            if (loadedConfig.ContainsKey(key))
                            {
                                languageConfig[key] = loadedConfig[key];
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"加载配置文件失败: {ex.Message}");
                    languageConfig = new Dictionary<string, bool>(defaultConfig);
                }
            }

            public void SaveConfig()
            {
                try
                {
                    string json = JsonConvert.SerializeObject(languageConfig, Formatting.Indented);
                    File.WriteAllText(configFilePath, json, Encoding.UTF8);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"保存配置失败: {ex.Message}", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            public Dictionary<string, bool> GetLanguageConfig()
            {
                return new Dictionary<string, bool>(languageConfig);
            }

            public void UpdateAllConfig(Dictionary<string, bool> newConfig)
            {
                foreach (var kvp in newConfig)
                {
                    if (languageConfig.ContainsKey(kvp.Key))
                    {
                        languageConfig[kvp.Key] = kvp.Value;
                    }
                }
            }
        }
    }

    public class LanguageInfo
    {
        public string Name { get; set; }
        public string Code { get; set; }
    }
}