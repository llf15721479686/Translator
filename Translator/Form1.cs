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
        // 修改Form1_Load方法，确保初始配置包含汉语
        private void Form1_Load(object sender, EventArgs e)
        {
            InitializeLayout();
            languageConfigManager.LoadConfig();

            // 确保汉语在初始配置中被启用
            var config = languageConfigManager.GetLanguageConfig();
            if (!config.ContainsKey("汉语") || !config["汉语"])
            {
                config["汉语"] = true;
                languageConfigManager.UpdateAllConfig(config);
                languageConfigManager.SaveConfig();
            }
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

            // 修改：进度条宽度自适应，和表格保持一致
            int progressBarLeft = 10; // 与表格左对齐
            int progressBarWidth = this.ClientSize.Width - 20; // 与表格宽度一致

            progressBar = new ProgressBar
            {
                Location = new Point(progressBarLeft, progressBarTop),
                Size = new Size(progressBarWidth, 20),
                Visible = false,
                Style = ProgressBarStyle.Continuous,
                Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top // 添加锚点，使其自适应
            };
            this.Controls.Add(progressBar);

            lblProgress = new Label
            {
                Location = new Point(progressBarLeft, progressBarTop + 25),
                Size = new Size(progressBarWidth, 20),
                Text = "",
                Font = new Font("微软雅黑", 9F),
                TextAlign = ContentAlignment.MiddleLeft,
                Visible = false,
                Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top // 添加锚点，使其自适应
            };
            this.Controls.Add(lblProgress);

            dgvTranslations.Top = progressBarTop + 50;
            dgvTranslations.Left = 10;
            dgvTranslations.Width = this.ClientSize.Width - 20;
            dgvTranslations.Height = this.ClientSize.Height - dgvTranslations.Top - 10;
            dgvTranslations.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

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

            // 确保按钮始终显示在一行
            if (totalButtonsWidth > this.ClientSize.Width - 20)
            {
                // 缩小按钮宽度以适应窗口
                buttonWidth = (this.ClientSize.Width - 20 - buttonSpacing * 3) / 4;

                btnTranslate.Location = new Point(buttonLeft, buttonTop);
                btnTranslate.Size = new Size(buttonWidth, buttonHeight);
                buttonLeft += buttonWidth + buttonSpacing;

                btnCopyAll.Location = new Point(buttonLeft, buttonTop);
                btnCopyAll.Size = new Size(buttonWidth, buttonHeight);
                buttonLeft += buttonWidth + buttonSpacing;

                btnGenerateMessages.Location = new Point(buttonLeft, buttonTop);
                btnGenerateMessages.Size = new Size(buttonWidth, buttonHeight);
                buttonLeft += buttonWidth + buttonSpacing;

                btnConfigureLanguages.Location = new Point(buttonLeft, buttonTop);
                btnConfigureLanguages.Size = new Size(buttonWidth, buttonHeight);
            }
            else
            {
                // 保持原始布局
                btnTranslate.Location = new Point(buttonLeft, buttonTop);
                btnTranslate.Size = new Size(buttonWidth, buttonHeight);
                buttonLeft += buttonWidth + buttonSpacing;

                btnCopyAll.Location = new Point(buttonLeft, buttonTop);
                btnCopyAll.Size = new Size(buttonWidth, buttonHeight);
                buttonLeft += buttonWidth + buttonSpacing;

                btnGenerateMessages.Location = new Point(buttonLeft, buttonTop);
                btnGenerateMessages.Size = new Size(buttonWidth, buttonHeight);
                buttonLeft += buttonWidth + buttonSpacing;

                btnConfigureLanguages.Location = new Point(buttonLeft, buttonTop);
                btnConfigureLanguages.Size = new Size(buttonWidth, buttonHeight);
            }

            // 确保按钮固定在合适的位置
            progressBar.Top = buttonTop + buttonHeight + 10;
            lblProgress.Top = progressBar.Top + 25;
            dgvTranslations.Top = progressBar.Top + 50;

            // 确保宽度自适应
            progressBar.Width = this.ClientSize.Width - 20;
            lblProgress.Width = this.ClientSize.Width - 20;
            dgvTranslations.Width = this.ClientSize.Width - 20;
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
                // 确保汉语在配置窗口中默认被选中
                var config = languageConfigManager.GetLanguageConfig();
                if (!config.ContainsKey("汉语"))
                {
                    config["汉语"] = true;
                    languageConfigManager.UpdateAllConfig(config);
                    languageConfigManager.SaveConfig();
                }

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

            // 确保至少有一种语言被选中
            bool anyEnabled = config.Any(kvp => kvp.Value);

            foreach (var language in languages)
            {
                if (config.ContainsKey(language.Name) && config[language.Name])
                {
                    enabledLanguages.Add(language);
                }
            }

            // 如果没有语言被选中，默认启用英语和汉语
            if (enabledLanguages.Count == 0)
            {
                // 确保汉语被包含
                enabledLanguages.Add(languages.First(l => l.Name == "英语"));
                enabledLanguages.Add(languages.First(l => l.Name == "汉语"));

                // 更新配置
                config["英语"] = true;
                config["汉语"] = true;
                languageConfigManager.UpdateAllConfig(config);
            }
            // 确保汉语始终被包含（即使配置中未勾选）
            else if (!enabledLanguages.Any(l => l.Name == "汉语"))
            {
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

            // 第一步：处理缓存
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
                    // 简化进度条更新，减少UI阻塞
                    if (row % 10 == 0) // 每10个更新一次进度
                    {
                        UpdateProgress(progressState.CurrentStep, $"{targetLanguage}: 第{row + 1}/{words.Length}个词汇");
                    }
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

            // 第二步：翻译剩余词汇
            if (useYoudao)
            {
                await TranslateWithYoudaoOptimized(wordsToTranslate, wordIndexMap, columnIndex,
                    targetLanguage, progressState);
            }
            else
            {
                await TranslateWithBaiduOptimized(wordsToTranslate, wordIndexMap, columnIndex,
                    targetLanguage, progressState);
            }
        }

        private async Task TranslateWithBaiduOptimized(
            List<string> wordsToTranslate,
            Dictionary<string, List<int>> wordIndexMap,
            int columnIndex,
            string targetLanguage,
            ProgressState progressState)
        {
            try
            {
                // 使用优化的批量翻译
                var batchResults = await BaiduTranslatorHelper.BatchTranslateWithoutCacheAsync(wordsToTranslate, "中文", targetLanguage);

                var successfulTranslations = new Dictionary<string, string>();

                for (int i = 0; i < wordsToTranslate.Count; i++)
                {
                    string word = wordsToTranslate[i];
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
                            });
                        }

                        if (!result.Contains("翻译失败") && !result.Contains("翻译API错误"))
                        {
                            successfulTranslations[word] = result;
                        }

                        progressState.CurrentStep += rows.Count;
                        // 批量更新进度，减少UI刷新
                        if (i % 5 == 0 || i == wordsToTranslate.Count - 1)
                        {
                            UpdateProgress(progressState.CurrentStep, $"{targetLanguage}: {i + 1}/{wordsToTranslate.Count}个词汇");
                        }
                    }
                }

                // 异步保存缓存，不阻塞主线程
                if (successfulTranslations.Count > 0)
                {
                    _ = Task.Run(() =>
                        DatabaseHelper.SaveBatchTranslations(successfulTranslations, "中文", targetLanguage));
                }
            }
            catch (Exception ex)
            {
                // 批量失败，回退到单个翻译
                await TranslateWithBaiduSingleOptimized(wordsToTranslate, wordIndexMap, columnIndex,
                    targetLanguage, progressState);
            }
        }
        // 修改Form1.cs中的TranslateWithYoudaoOptimized方法
        private async Task TranslateWithYoudaoOptimized(
            List<string> wordsToTranslate,
            Dictionary<string, List<int>> wordIndexMap,
            int columnIndex,
            string targetLanguage,
            ProgressState progressState)
        {
            var successfulTranslations = new Dictionary<string, string>();
            // 根据API限制调整并发数，有道建议不超过1-2
            var semaphore = new SemaphoreSlim(1);
            var tasks = new List<Task>();
            int totalWords = wordsToTranslate.Count;

            // 按批次处理，每批处理后增加延迟
            int batchSize = 5;
            for (int i = 0; i < totalWords; i += batchSize)
            {
                var batch = wordsToTranslate.Skip(i).Take(batchSize).ToList();

                foreach (var word in batch)
                {
                    await semaphore.WaitAsync();
                    tasks.Add(Task.Run(async () =>
                    {
                        try
                        {
                            // 对于长文本，增加超时时间
                            int timeout = word.Length > 50 ? 15000 : 8000;
                            // 对于失败过的词，增加重试次数
                            int retryCount = 3;

                            string result = await YoudaoTranslatorHelper.TranslateAsync(
                                word, "中文", targetLanguage, retryCount).ConfigureAwait(false);

                            if (wordIndexMap.TryGetValue(word, out List<int> rows))
                            {
                                this.Invoke((MethodInvoker)delegate
                                {
                                    foreach (int row in rows)
                                    {
                                        dgvTranslations.Rows[row].Cells[columnIndex].Value = result;
                                    }
                                });
                            }

                            // 检查是否有效的翻译结果
                            if (!string.IsNullOrEmpty(result) &&
                                !result.Contains("翻译失败") &&
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
                                progressState.CurrentStep += rows?.Count ?? 0;
                            }

                            // 更新进度
                            int currentProgress = (int)(((double)progressState.CurrentStep / progressState.TotalSteps) * 100);
                            UpdateProgress(progressState.CurrentStep,
                                $"{targetLanguage}: {progressState.CurrentStep}/{progressState.TotalSteps} ({currentProgress}%)");
                        }
                        catch (Exception ex)
                        {
                            string errorMessage = $"翻译失败: {ex.Message}";

                            if (wordIndexMap.TryGetValue(word, out List<int> rows))
                            {
                                this.Invoke((MethodInvoker)delegate
                                {
                                    foreach (int row in rows)
                                    {
                                        dgvTranslations.Rows[row].Cells[columnIndex].Value = errorMessage;
                                    }
                                });
                            }

                            lock (progressState)
                            {
                                progressState.CurrentStep += rows?.Count ?? 0;
                            }
                        }
                        finally
                        {
                            semaphore.Release();
                        }
                    }));
                }

                // 等待当前批次完成
                await Task.WhenAll(tasks);
                tasks.Clear();

                // 批次之间增加延迟，避免触发频率限制
                if (i + batchSize < totalWords)
                {
                    await Task.Delay(2000); // 每批处理后等待2秒
                }
            }

            // 异步保存缓存
            if (successfulTranslations.Count > 0)
            {
                _ = Task.Run(() =>
                    DatabaseHelper.SaveBatchTranslations(successfulTranslations, "中文", targetLanguage));
            }
        }
        private async Task TranslateWithBaiduSingleOptimized(
            List<string> wordsToTranslate,
            Dictionary<string, List<int>> wordIndexMap,
            int columnIndex,
            string targetLanguage,
            ProgressState progressState)
        {
            var successfulTranslations = new Dictionary<string, string>();
            var semaphore = new SemaphoreSlim(3); // 限制并发数

            var tasks = new List<Task>();

            for (int i = 0; i < wordsToTranslate.Count; i++)
            {
                string word = wordsToTranslate[i];
                await semaphore.WaitAsync();

                tasks.Add(Task.Run(async () =>
                {
                    try
                    {
                        string result = await BaiduTranslatorHelper.TranslateWithoutCacheAsync(word, "中文", targetLanguage);

                        if (wordIndexMap.TryGetValue(word, out List<int> rows))
                        {
                            this.Invoke((MethodInvoker)delegate
                            {
                                foreach (int row in rows)
                                {
                                    dgvTranslations.Rows[row].Cells[columnIndex].Value = result;
                                }
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
                            progressState.CurrentStep += rows.Count;
                        }

                        // 减少UI刷新频率
                        lock (progressState)
                        {
                            if (progressState.CurrentStep % 10 == 0)
                            {
                                UpdateProgress(progressState.CurrentStep, $"{targetLanguage}: 翻译中...");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        string errorMessage = $"{targetLanguage}翻译失败";

                        if (wordIndexMap.TryGetValue(word, out List<int> rows))
                        {
                            this.Invoke((MethodInvoker)delegate
                            {
                                foreach (int row in rows)
                                {
                                    dgvTranslations.Rows[row].Cells[columnIndex].Value = errorMessage;
                                }
                            });
                        }

                        lock (progressState)
                        {
                            progressState.CurrentStep += rows.Count;
                        }
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                }));

                // 控制任务创建速度
                if (tasks.Count >= 3)
                {
                    await Task.WhenAll(tasks);
                    tasks.Clear();
                    await Task.Delay(500); // 小延迟避免频率过高
                }
            }

            if (tasks.Count > 0)
            {
                await Task.WhenAll(tasks);
            }

            // 异步保存缓存
            if (successfulTranslations.Count > 0)
            {
                _ = Task.Run(() =>
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