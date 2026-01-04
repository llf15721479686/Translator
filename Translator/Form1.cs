using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
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
        private Button btnTranslateSingapore;
        private Button btnImportExcel;
        private Button btnImportXmlExcel;
        // 语言列表，按照原选项卡顺序
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
            new LanguageInfo { Name = "汉语", Code = "2052" }, // 在西班牙语前添加汉语列
            new LanguageInfo { Name = "西班牙语", Code = "3082" }
        };
        // 分隔符
        private const string Separator = "\r\n";

        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            InitializeLayout();
        }
        private void InitializeLayout()
        {
            // 设置文本框
            txtSource.Height = 120;
            txtSource.Top = 30;
            txtSource.Left = 10;
            txtSource.Width = this.ClientSize.Width - 20;

            // 所有按钮统一大小
            int buttonWidth = 160;
            int buttonHeight = 35;
            int buttonSpacing = 10;
            int buttonTop = txtSource.Bottom + 15;
            int buttonLeft = 10;

            // 设置已有按钮的大小和位置（在Designer.cs中设置的会被这里覆盖）
            btnTranslate.Location = new Point(buttonLeft, buttonTop);
            btnTranslate.Size = new Size(buttonWidth, buttonHeight);
            buttonLeft += buttonWidth + buttonSpacing;

            button1.Location = new Point(buttonLeft, buttonTop);
            button1.Size = new Size(buttonWidth, buttonHeight);
            buttonLeft += buttonWidth + buttonSpacing;

            btnCopyAll.Location = new Point(buttonLeft, buttonTop);
            btnCopyAll.Size = new Size(buttonWidth, buttonHeight);
            buttonLeft += buttonWidth + buttonSpacing;

            // 新加坡环境翻译按钮
            btnTranslateSingapore = new Button
            {
                Text = "新加坡环境翻译",
                Font = new Font("微软雅黑", 10F),
                Location = new Point(buttonLeft, buttonTop),
                Size = new Size(buttonWidth, buttonHeight),
                UseVisualStyleBackColor = true
            };
            btnTranslateSingapore.Click += btnTranslateSingapore_Click;
            this.Controls.Add(btnTranslateSingapore);
            buttonLeft += buttonWidth + buttonSpacing;
            // 在原有按钮布局后添加新按钮
            // btnGenerateMessages
            btnGenerateMessages = new Button
            {
                Text = "生成消息翻译",
                Font = new Font("微软雅黑", 10F),
                Location = new Point(buttonLeft, buttonTop),
                Size = new Size(buttonWidth, buttonHeight),
                UseVisualStyleBackColor = true
            };
            btnGenerateMessages.Click += btnGenerateMessages_Click;
            this.Controls.Add(btnGenerateMessages);
            buttonLeft += buttonWidth + buttonSpacing;


            // 数据表格位置
            dgvTranslations.Top = buttonTop + buttonHeight + 15;
            dgvTranslations.Left = 10;
            dgvTranslations.Width = this.ClientSize.Width - 20;
            dgvTranslations.Height = this.ClientSize.Height - dgvTranslations.Top - 10;

            // 如果按钮超出窗体宽度，调整按钮大小
            if (buttonLeft + buttonWidth > this.ClientSize.Width - 20)
            {
                // 计算需要调整的宽度
                int totalWidthNeeded = buttonLeft + buttonWidth + 10; // 10是右边距
                int currentWidth = this.ClientSize.Width - 20;

                if (totalWidthNeeded > currentWidth)
                {
                    // 计算每个按钮需要减少的宽度
                    int buttonCount = 6; // 总共6个按钮
                    int totalSpacing = buttonSpacing * (buttonCount - 1);
                    int availableWidth = currentWidth - 10; // 留出左边距
                    int newButtonWidth = (availableWidth - totalSpacing) / buttonCount;

                    if (newButtonWidth > 120) // 确保按钮不会太小
                    {
                        // 重新排列按钮
                        buttonLeft = 10;
                        buttonWidth = newButtonWidth;

                        btnTranslate.Location = new Point(buttonLeft, buttonTop);
                        btnTranslate.Size = new Size(buttonWidth, buttonHeight);
                        buttonLeft += buttonWidth + buttonSpacing;

                        button1.Location = new Point(buttonLeft, buttonTop);
                        button1.Size = new Size(buttonWidth, buttonHeight);
                        buttonLeft += buttonWidth + buttonSpacing;

                        btnCopyAll.Location = new Point(buttonLeft, buttonTop);
                        btnCopyAll.Size = new Size(buttonWidth, buttonHeight);
                        buttonLeft += buttonWidth + buttonSpacing;

                        btnTranslateSingapore.Location = new Point(buttonLeft, buttonTop);
                        btnTranslateSingapore.Size = new Size(buttonWidth, buttonHeight);
                        buttonLeft += buttonWidth + buttonSpacing;

                        btnImportExcel.Location = new Point(buttonLeft, buttonTop);
                        btnImportExcel.Size = new Size(buttonWidth, buttonHeight);
                        buttonLeft += buttonWidth + buttonSpacing;

                        btnImportXmlExcel.Location = new Point(buttonLeft, buttonTop);
                        btnImportXmlExcel.Size = new Size(buttonWidth, buttonHeight);
                    }
                }
            }
        }

        #region 新加坡环境翻译
        // 新加坡环境翻译按钮点击事件
        private async void btnTranslateSingapore_Click(object sender, EventArgs e)
        {
            await TranslateSingaporeLanguages();
        }

        // 新加坡环境翻译方法
        private async Task TranslateSingaporeLanguages()
        {
            string inputText = txtSource.Text.Trim();
            if (string.IsNullOrWhiteSpace(inputText))
            {
                MessageBox.Show("请输入要翻译的中文文本", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // 分割词汇
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

            // 只显示指定的语言列
            string[] singaporeLanguageCodes = { "1033", "1049", "1054", "1057", "1066", "1086", "2052" };

            // 筛选新加坡环境的语言
            var singaporeLanguages = languages.Where(l => singaporeLanguageCodes.Contains(l.Code)).ToList();

            // 初始化DataGridView，只显示指定的列
            InitializeSingaporeDataGridView(singaporeLanguages);

            // 为每个词汇添加一行
            for (int i = 0; i < words.Length; i++)
            {
                dgvTranslations.Rows.Add();
            }

            // 填充初始数据
            for (int row = 0; row < words.Length; row++)
            {
                for (int col = 0; col < dgvTranslations.Columns.Count; col++)
                {
                    string columnName = dgvTranslations.Columns[col].HeaderText;
                    string languageName = columnName.Substring(0, columnName.IndexOf('['));

                    // 汉语列显示中文原文
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

            btnTranslateSingapore.Enabled = false;
            btnCopyAll.Enabled = false;
            btnTranslateSingapore.Text = "翻译中...";

            try
            {
                // 按顺序执行翻译（只翻译新加坡需要的语言）
                for (int col = 0; col < dgvTranslations.Columns.Count; col++)
                {
                    string columnName = dgvTranslations.Columns[col].HeaderText;
                    string languageName = columnName.Substring(0, columnName.IndexOf('['));

                    // 跳过不需要翻译的列（汉语列已经在初始化时填充）
                    if (languageName == "汉语")
                        continue;

                    // 更新状态为正在翻译
                    for (int row = 0; row < words.Length; row++)
                    {
                        dgvTranslations.Rows[row].Cells[col].Value = "正在翻译...";
                    }
                    dgvTranslations.Refresh();

                    // 翻译当前语言的所有词汇
                    await TranslateWordsForLanguage(words, languageName, col);

                    // 严格遵守1秒1次的限制
                    await Task.Delay(1000);
                }
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
                btnTranslateSingapore.Enabled = true;
                btnTranslateSingapore.Text = "新加坡环境翻译";
                btnCopyAll.Enabled = true;
            }
        }

        // 初始化新加坡环境DataGridView（只显示指定列）
        private void InitializeSingaporeDataGridView(List<LanguageInfo> singaporeLanguages)
        {
            // 清空现有行和列
            dgvTranslations.Rows.Clear();
            dgvTranslations.Columns.Clear();

            // 核心：移除左侧行标题列（行号列）
            dgvTranslations.RowHeadersVisible = false;

            // 设置DataGridView自适应
            dgvTranslations.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvTranslations.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            // 补充：移除横向滚动条，避免列被遮挡
            dgvTranslations.ScrollBars = ScrollBars.Vertical;

            // 添加列：只添加新加坡需要的语言
            foreach (var language in singaporeLanguages)
            {
                // 创建列
                DataGridViewTextBoxColumn column = new DataGridViewTextBoxColumn();
                column.HeaderText = $"{language.Name}[{language.Code}]";
                column.Name = $"col{language.Code}";
                column.ReadOnly = true;
                column.FillWeight = 100; // 确保列有足够的权重
                dgvTranslations.Columns.Add(column);
            }

            // 设置行高自适应内容
            dgvTranslations.AutoResizeRows(DataGridViewAutoSizeRowsMode.AllCellsExceptHeaders);
        }


        private void InitializeDataGridView(bool showRedColumnsOnly)
        {
            // 清空现有行和列
            dgvTranslations.Rows.Clear();
            dgvTranslations.Columns.Clear();

            // 核心：移除左侧行标题列（行号列）
            dgvTranslations.RowHeadersVisible = false;

            // 设置DataGridView自适应
            dgvTranslations.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvTranslations.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            // 补充：移除横向滚动条，避免列被遮挡
            dgvTranslations.ScrollBars = ScrollBars.Vertical;

            // 添加列：按指定顺序添加，确保汉语在西班牙语之前
            foreach (var language in languages)
            {
                // 创建列
                DataGridViewTextBoxColumn column = new DataGridViewTextBoxColumn();
                column.HeaderText = $"{language.Name}[{language.Code}]";
                column.Name = $"col{language.Code}";
                column.ReadOnly = true;
                column.FillWeight = 100; // 确保列有足够的权重
                dgvTranslations.Columns.Add(column);
            }

            // 设置行高自适应内容
            dgvTranslations.AutoResizeRows(DataGridViewAutoSizeRowsMode.AllCellsExceptHeaders);
        }

        #endregion


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

                // 不复制表头，直接复制数据行
                foreach (DataGridViewRow row in dgvTranslations.Rows)
                {
                    for (int i = 0; i < dgvTranslations.Columns.Count; i++)
                    {
                        var cellValue = row.Cells[i].Value?.ToString() ?? "";

                        // 处理Excel格式：用制表符分隔，文本用引号括起来避免格式问题
                        if (!string.IsNullOrEmpty(cellValue))
                        {
                            // 移除可能存在的换行符，避免Excel格式混乱
                            cellValue = cellValue.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ");

                            // 如果包含制表符或引号，需要用引号括起来并转义引号
                            if (cellValue.Contains("\t") || cellValue.Contains("\"") || cellValue.Contains(","))
                            {
                                sb.Append($"\"{cellValue.Replace("\"", "\"\"")}\"");
                            }
                            else
                            {
                                sb.Append(cellValue);
                            }
                        }

                        // 添加制表符分隔（除了最后一列）
                        if (i < dgvTranslations.Columns.Count - 1)
                        {
                            sb.Append("\t");
                        }
                    }
                    sb.AppendLine();
                }

                // 复制到剪贴板
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

        // 添加复制按钮的点击事件
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

            // 分割文本
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
            await TranslateAllLanguages(false);
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            await TranslateAllLanguages(true);
        }

        private async Task TranslateAllLanguages(bool showRedColumnsOnly)
        {
            string inputText = txtSource.Text.Trim();
            if (string.IsNullOrWhiteSpace(inputText))
            {
                MessageBox.Show("请输入要翻译的中文文本", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // 分割词汇
            string[] words = inputText.Split(new[] { Separator }, StringSplitOptions.RemoveEmptyEntries)
                                     .Select(w => w.Trim())
                                     .Where(w => !string.IsNullOrWhiteSpace(w))
                                     .ToArray();

            if (words.Length == 0)
            {
                MessageBox.Show("没有找到有效的词汇，请使用\"&\"分隔词汇", "提示",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // 根据模式重新初始化DataGridView
            InitializeDataGridView(showRedColumnsOnly);

            // 为每个词汇添加一行
            for (int i = 0; i < words.Length; i++)
            {
                dgvTranslations.Rows.Add();
            }

            // 填充初始数据
            for (int row = 0; row < words.Length; row++)
            {
                for (int col = 0; col < dgvTranslations.Columns.Count; col++)
                {
                    string columnName = dgvTranslations.Columns[col].HeaderText;
                    string languageName = columnName.Substring(0, columnName.IndexOf('['));

                    // 汉语列显示中文原文
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

            Button currentButton = showRedColumnsOnly ? button1 : btnTranslate;
            currentButton.Enabled = false;
            btnCopyAll.Enabled = false;
            currentButton.Text = "翻译中...";

            try
            {
                // 按顺序执行翻译
                for (int col = 0; col < dgvTranslations.Columns.Count; col++)
                {
                    string columnName = dgvTranslations.Columns[col].HeaderText;
                    string languageName = columnName.Substring(0, columnName.IndexOf('['));

                    // 跳过不需要翻译的列
                    if (languageName == "汉语")
                        continue;

                    // 更新状态为正在翻译
                    for (int row = 0; row < words.Length; row++)
                    {
                        dgvTranslations.Rows[row].Cells[col].Value = "正在翻译...";
                    }
                    dgvTranslations.Refresh();

                    // 翻译当前语言的所有词汇
                    await TranslateWordsForLanguage(words, languageName, col);

                    // 严格遵守1秒1次的限制
                    await Task.Delay(1000);
                }
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
                currentButton.Enabled = true;
                currentButton.Text = showRedColumnsOnly ? "一键翻译所有语言(UAT)" : "一键翻译所有语言(DEV)";
                btnCopyAll.Enabled = true;
            }
        }

        // 在 Form1.cs 中修改 TranslateWordsForLanguage 方法
        private async Task TranslateWordsForLanguage(string[] words, string targetLanguage, int columnIndex)
        {
            // 跳过汉语列的翻译
            if (targetLanguage == "汉语")
                return;

            // 1. 先批量查询数据库缓存
            var cachedTranslations = DatabaseHelper.GetBatchCachedTranslations(words.ToList(), "中文", targetLanguage);

            // 准备需要API翻译的词汇列表
            var wordsToTranslate = new List<string>();
            var wordIndexMap = new Dictionary<string, List<int>>();

            for (int row = 0; row < words.Length; row++)
            {
                string word = words[row];

                if (cachedTranslations.TryGetValue(word, out string cachedResult))
                {
                    // 从缓存中获取到结果，直接显示
                    this.Invoke((MethodInvoker)delegate
                    {
                        dgvTranslations.Rows[row].Cells[columnIndex].Value = cachedResult;
                        dgvTranslations.Refresh();
                    });
                }
                else
                {
                    // 需要调用API翻译
                    if (!wordIndexMap.ContainsKey(word))
                    {
                        wordIndexMap[word] = new List<int>();
                        wordsToTranslate.Add(word);
                    }
                    wordIndexMap[word].Add(row);
                }
            }

            // 2. 如果没有需要翻译的词汇，直接返回
            if (wordsToTranslate.Count == 0)
                return;

            // 3. 批量翻译（优化版本）
            // 修改判断条件：检查是否有词汇包含斜杠 /
            bool hasSlash = wordsToTranslate.Any(word => word.Contains("/") || word.Contains("\\"));
            bool useYoudao = targetLanguage == "印度尼西亚语" ||
                             targetLanguage == "马来西亚" ||
                             hasSlash;  // 新增条件：包含斜杠的词汇使用有道翻译

            if (useYoudao)
            {
                // 有道翻译：使用并行处理（有道没有批量API）
                await TranslateWithYoudao(wordsToTranslate, wordIndexMap, columnIndex, targetLanguage);
            }
            else
            {
                // 百度翻译：使用批量翻译
                await TranslateWithBaiduBatch(wordsToTranslate, wordIndexMap, columnIndex, targetLanguage);
            }

            // 4. 不再需要单独的保存逻辑，已在翻译过程中保存
        }

        /// <summary>
        /// 使用百度批量翻译
        /// </summary>
        private async Task TranslateWithBaiduBatch(
            List<string> wordsToTranslate,
            Dictionary<string, List<int>> wordIndexMap,
            int columnIndex,
            string targetLanguage)
        {
            try
            {
                // 使用批量翻译
                var batchResults = await Task.Run(() =>
                    BaiduTranslatorHelper.BatchTranslateWithoutCache(wordsToTranslate, "中文", targetLanguage));

                // 更新UI并保存到数据库
                var successfulTranslations = new Dictionary<string, string>();

                foreach (var word in wordsToTranslate)
                {
                    if (batchResults.TryGetValue(word, out string result))
                    {
                        // 更新UI
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

                        // 收集成功的翻译用于保存
                        if (!result.Contains("翻译失败") && !result.Contains("翻译API错误"))
                        {
                            successfulTranslations[word] = result;
                        }
                    }
                }

                // 批量保存到数据库
                if (successfulTranslations.Count > 0)
                {
                    await Task.Run(() =>
                        DatabaseHelper.SaveBatchTranslations(successfulTranslations, "中文", targetLanguage));
                }
            }
            catch (Exception ex)
            {
                // 批量失败，回退到单个翻译
                await TranslateWithBaiduSingle(wordsToTranslate, wordIndexMap, columnIndex, targetLanguage);
            }
        }

        /// <summary>
        /// 使用百度单个翻译（回退方法）
        /// </summary>
        private async Task TranslateWithBaiduSingle(
            List<string> wordsToTranslate,
            Dictionary<string, List<int>> wordIndexMap,
            int columnIndex,
            string targetLanguage)
        {
            var batchResults = new Dictionary<string, string>();
            var successfulTranslations = new Dictionary<string, string>();

            // 分组处理，每组最多5个，每组之间有延迟
            int groupSize = 5;
            for (int i = 0; i < wordsToTranslate.Count; i += groupSize)
            {
                var group = wordsToTranslate.Skip(i).Take(groupSize).ToList();

                // 并行处理组内的翻译（并行度2，避免超过API限制）
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

                                // 更新UI
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

                                // 收集成功的翻译
                                if (!result.Contains("翻译失败") && !result.Contains("翻译API错误"))
                                {
                                    lock (successfulTranslations)
                                    {
                                        successfulTranslations[word] = result;
                                    }
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
                            }
                        }
                    });

                    tasks.Add(task);

                    // 每组内部并行，但组之间有延迟
                    if (tasks.Count >= 2) // 最多并行2个任务
                    {
                        await Task.WhenAll(tasks);
                        tasks.Clear();

                        // 组之间添加延迟
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

            // 批量保存成功的翻译
            if (successfulTranslations.Count > 0)
            {
                await Task.Run(() =>
                    DatabaseHelper.SaveBatchTranslations(successfulTranslations, "中文", targetLanguage));
            }
        }
        /// <summary>
        /// 使用有道翻译（支持并行）
        /// </summary>
        private async Task TranslateWithYoudao(
            List<string> wordsToTranslate,
            Dictionary<string, List<int>> wordIndexMap,
            int columnIndex,
            string targetLanguage)
        {
            var successfulTranslations = new Dictionary<string, string>();

            // 有道翻译：使用并行处理，但控制并发数
            int maxConcurrent = 3; // 最多同时3个有道翻译请求
            var semaphore = new SemaphoreSlim(maxConcurrent);
            var tasks = new List<Task>();

            foreach (var word in wordsToTranslate)
            {
                await semaphore.WaitAsync();

                var task = Task.Run(async () =>
                {
                    try
                    {
                        string result = await YoudaoTranslatorHelper.TranslateAsync(word, "中文", targetLanguage);

                        // 更新UI
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

                        // 收集成功的翻译
                        if (!result.Contains("翻译失败") &&
                            !result.Contains("有道翻译错误") &&
                            !result.Contains("有道API"))
                        {
                            lock (successfulTranslations)
                            {
                                successfulTranslations[word] = result;
                            }
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
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                });

                tasks.Add(task);

                // 控制请求频率
                await Task.Delay(1000); 
            }

            await Task.WhenAll(tasks);

            // 批量保存到数据库
            if (successfulTranslations.Count > 0)
            {
                await Task.Run(() =>
                    DatabaseHelper.SaveBatchTranslations(successfulTranslations, "中文", targetLanguage));
            }
        }
    }

    public class LanguageInfo
    {
        public string Name { get; set; }
        public string Code { get; set; }
    }
}
