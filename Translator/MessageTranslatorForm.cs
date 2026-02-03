using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Translator
{
    public partial class MessageTranslatorForm : Form
    {
        private List<LanguageInfo> languages;
        private string[] sourceTexts;
        private List<TextBox> prefixTextBoxes;
        private Dictionary<string, Dictionary<int, string>> translations;
        private Dictionary<string, Button> languageCopyButtons;
        private TextBox previewTextBox; // 新增：预览文本框引用

        // 布局控件（固定位置+尺寸，避免遮挡，兼容4.7.2）
        private Panel mainPanel;
        private Panel leftContentPanel;
        private Panel rightPreviewPanel;
        private Panel bottomButtonPanel;
        private FlowLayoutPanel languageButtonFlow;

        public MessageTranslatorForm(string[] texts)
        {
            this.sourceTexts = texts;
            this.languages = GetSupportedLanguages();
            this.prefixTextBoxes = new List<TextBox>();
            this.translations = new Dictionary<string, Dictionary<int, string>>();
            this.languageCopyButtons = new Dictionary<string, Button>();

            InitializeComponent();
            InitializeFixedLayoutUI();
            // 窗口大小变化时自动重绘布局（4.7.2 支持该事件）
            this.Resize += new EventHandler(AdjustLayoutOnResize);
        }

        private List<LanguageInfo> GetSupportedLanguages()
        {
            return new List<LanguageInfo>
            {
                new LanguageInfo { Name = "阿拉伯语", Code = "1025" },
                new LanguageInfo { Name = "德语", Code = "1031" },
                new LanguageInfo { Name = "英语", Code = "1033" },
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
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.BackColor = Color.FromArgb(248, 249, 250);
            this.ClientSize = new Size(1200, 768);
            this.Font = new Font("微软雅黑", 9.5F);
            this.MinimumSize = new Size(1000, 680);
            this.Name = "MessageTranslatorForm";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "消息翻译器 - 多语言批量翻译工具";
            this.ResumeLayout(false);
        }

        // 兼容4.7.2的固定布局，彻底解决遮挡
        private void InitializeFixedLayoutUI()
        {
            // 主容器（承载所有面板）
            mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent
            };
            this.Controls.Add(mainPanel);

            // ========== 左侧：原文配置区（固定宽度，兼容4.7.2） ==========
            leftContentPanel = new Panel
            {
                Location = new Point(20, 20),
                Size = new Size(650, 550),  // 修改：从580减少到550（为底部留更多空间）
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
            };
            mainPanel.Controls.Add(leftContentPanel);

            // 左侧标题
            Label leftTitle = new Label
            {
                Text = "📝 原文与标识配置",
                Font = new Font("微软雅黑", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(33, 37, 41),
                Location = new Point(20, 15),
                Size = new Size(300, 30),
                TextAlign = ContentAlignment.MiddleLeft
            };
            leftContentPanel.Controls.Add(leftTitle);

            // 翻译按钮
            Button btnTranslate = new Button
            {
                Text = "🚀 开始翻译所有语言",
                Font = new Font("微软雅黑", 11, FontStyle.Bold),
                Location = new Point(20, 55),
                Size = new Size(200, 40),
                BackColor = Color.FromArgb(25, 135, 84),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnTranslate.FlatAppearance.BorderSize = 0;
            btnTranslate.FlatAppearance.MouseOverBackColor = Color.FromArgb(28, 150, 93);
            btnTranslate.Click += new EventHandler(BtnTranslate_Click);
            leftContentPanel.Controls.Add(btnTranslate);

            // 原文表格（带滚动条，独立区域）
            Panel tableScrollPanel = new Panel
            {
                Location = new Point(20, 105),
                Size = new Size(610, 420),  // 修改：从450减少到420（因为整体高度减少了）
                AutoScroll = true,
                BackColor = Color.FromArgb(248, 249, 250),

            };
            leftContentPanel.Controls.Add(tableScrollPanel);

            // 用TableLayoutPanel确保行列不重叠（4.7.2 完全支持）
            TableLayoutPanel messageTable = new TableLayoutPanel
            {
                ColumnCount = 2,
                RowCount = sourceTexts.Length + 1,
                Size = new Size(580, 45 + (sourceTexts.Length * 40)),  // 修改：表头45 + 内容行×40
                BackColor = Color.White,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.Single
            };
            messageTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 65F));
            messageTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35F));
            tableScrollPanel.Controls.Add(messageTable);

            // 表头
            messageTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 45F));
            // 4.7.2 不支持内联初始化控件后直接添加，拆分步骤保证兼容
            Label originalHeader = new Label
            {
                Text = "原文内容",
                Font = new Font("微软雅黑", 11, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(33, 37, 41),
                Dock = DockStyle.Fill,  // 改回Fill，让TableLayoutPanel管理布局
                TextAlign = ContentAlignment.MiddleCenter,
                AutoEllipsis = true,
                Margin = new Padding(0)  // 确保没有外边距
            };
            messageTable.Controls.Add(originalHeader, 0, 0);

            Label prefixHeader = new Label
            {
                Text = "消息标识前缀",
                Font = new Font("微软雅黑", 11, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(33, 37, 41),
                Dock = DockStyle.Fill,  // 改回Fill，让TableLayoutPanel管理布局
                TextAlign = ContentAlignment.MiddleCenter,
                AutoEllipsis = true,
                Margin = new Padding(0)  // 确保没有外边距
            };
            messageTable.Controls.Add(prefixHeader, 1, 0);

            // 内容行（每个控件独立占行，兼容4.7.2）
            for (int i = 0; i < sourceTexts.Length; i++)
            {
                messageTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));  // 修改：从60F减少到40F

                Label textLabel = new Label
                {
                    Text = string.Format("{0}. {1}", i + 1, sourceTexts[i]), // 4.7.2 推荐用 string.Format 替代 $ 插值（虽然后者也支持，更稳妥）
                    Font = new Font("微软雅黑", 10),
                    ForeColor = Color.FromArgb(33, 37, 41),
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleCenter,
                    AutoEllipsis = true,
                    Padding = new Padding(10, 0, 10, 0)
                };
                messageTable.Controls.Add(textLabel, 0, i + 1);

                TextBox prefixBox = new TextBox
                {
                    Font = new Font("微软雅黑", 10),
                    Dock = DockStyle.Fill,
                    BackColor = Color.FromArgb(248, 249, 250),
                    BorderStyle = BorderStyle.FixedSingle,
                    TextAlign = HorizontalAlignment.Center,
                    Padding = new Padding(5)  // 修改：从8减少到5，适应更小的高度
                };
                // 4.7.2 支持匿名方法，保留焦点切换样式
                prefixBox.Enter += (s, e) => { prefixBox.BackColor = Color.FromArgb(230, 245, 255); };
                prefixBox.Leave += (s, e) => { prefixBox.BackColor = Color.FromArgb(248, 249, 250); };
                messageTable.Controls.Add(prefixBox, 1, i + 1);
                prefixTextBoxes.Add(prefixBox);
            }

            // ========== 右侧：预览区（固定位置，兼容4.7.2） ==========
            rightPreviewPanel = new Panel
            {
                Location = new Point(690, 20),
                Size = new Size(470, 550),  // 修改：从580减少到550（与左侧保持一致）
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            mainPanel.Controls.Add(rightPreviewPanel);

            Label rightTitle = new Label
            {
                Text = "🔍 翻译结果预览",
                Font = new Font("微软雅黑", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(33, 37, 41),
                Location = new Point(20, 15),
                Size = new Size(300, 30),
                TextAlign = ContentAlignment.MiddleLeft
            };
            rightPreviewPanel.Controls.Add(rightTitle);

            // 多行文本框显示翻译结果（替换原来的提示标签）
            previewTextBox = new TextBox
            {
                Location = new Point(20, 60),
                Size = new Size(430, 470),  // 修改：从500减少到470（因为整体高度减少了）
                Multiline = true,
                ScrollBars = ScrollBars.Both,
                Font = new Font("微软雅黑", 9.5F),
                BackColor = Color.FromArgb(248, 249, 250),
                BorderStyle = BorderStyle.FixedSingle,
                ReadOnly = true,
                WordWrap = true,
                Text = "翻译完成后，此处将显示选中语言的翻译结果\r\n\r\n格式说明：\r\n\"消息标识\": \"翻译文本\","
            };
            rightPreviewPanel.Controls.Add(previewTextBox);

            // ========== 底部：语言按钮区（固定高度，兼容4.7.2） ==========
            bottomButtonPanel = new Panel
            {
                Location = new Point(20, 580),  // 修改：从620改为600（向上移动20像素）
                Size = new Size(1140, 250),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            mainPanel.Controls.Add(bottomButtonPanel);

            Label bottomTitle = new Label
            {
                Text = "🌐 语言复制按钮",
                Font = new Font("微软雅黑", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(33, 37, 41),
                Location = new Point(20, 10),
                Size = new Size(300, 30),
                TextAlign = ContentAlignment.MiddleLeft
            };
            bottomButtonPanel.Controls.Add(bottomTitle);

            // 语言按钮流布局（独立滚动）
            languageButtonFlow = new FlowLayoutPanel
            {
                Location = new Point(20, 45),
                Size = new Size(1100, 230), // 修改：高度从60增加到90
                FlowDirection = FlowDirection.LeftToRight,
                AutoScroll = true,
                BackColor = Color.FromArgb(248, 249, 250)
            };
            bottomButtonPanel.Controls.Add(languageButtonFlow);

            CreateLanguageCopyButtons();
        }

        // 窗口缩放时自动调整面板位置（4.7.2 要求显式声明 EventHandler）
        private void AdjustLayoutOnResize(object sender, EventArgs e)
        {
            // 左侧面板保持左对齐，预留边框空间
            leftContentPanel.Size = new Size(650, this.ClientSize.Height - 210); // 增加更多空间
            leftContentPanel.Location = new Point(20, 20);

            // 右侧面板跟在左侧面板右边，预留20像素间距
            rightPreviewPanel.Location = new Point(leftContentPanel.Right + 20, 20);
            rightPreviewPanel.Size = new Size(this.ClientSize.Width - rightPreviewPanel.Left - 20, leftContentPanel.Height);

            // 底部面板占满宽度，预留边框空间
            bottomButtonPanel.Size = new Size(this.ClientSize.Width - 40, 160); // 高度稍增加
            bottomButtonPanel.Location = new Point(20, leftContentPanel.Bottom + 40); // 增加更多间距

            // 调整预览文本框大小，确保在边框内
            if (previewTextBox != null)
            {
                // 减去边框和标题区域的空间
                previewTextBox.Size = new Size(rightPreviewPanel.Width - 45, rightPreviewPanel.Height - 85);
            }

            // 按钮流布局宽度自适应，确保在边框内
            languageButtonFlow.Size = new Size(bottomButtonPanel.Width - 45, 100);
        }

        private void CreateLanguageCopyButtons()
        {
            languageButtonFlow.Controls.Clear();
            languageCopyButtons.Clear();

            foreach (var language in languages)
            {
                Button btn = new Button
                {
                    Text = string.Format("{0} ({1})", language.Name, language.Code),
                    Font = new Font("微软雅黑", 9),
                    Size = new Size(160, 30),
                    Margin = new Padding(8),
                    BackColor = Color.FromArgb(108, 117, 125),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Cursor = Cursors.Hand,
                    Tag = language,
                    Enabled = false
                };
                btn.FlatAppearance.BorderSize = 0;
                btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(70, 130, 180);
                btn.Click += new EventHandler(BtnCopyLanguage_Click);

                // 4.7.2 支持 ToolTip，正常使用
                ToolTip toolTip = new ToolTip();
                toolTip.SetToolTip(btn, string.Format("复制{0}的翻译结果", language.Name));
                languageButtonFlow.Controls.Add(btn);
                languageCopyButtons[language.Name] = btn;
            }
        }

        // 异步翻译按钮点击事件（4.7.2 完全支持 async/await）
        private async void BtnTranslate_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < prefixTextBoxes.Count; i++)
            {
                if (string.IsNullOrWhiteSpace(prefixTextBoxes[i].Text))
                {
                    MessageBox.Show(string.Format("请为第 {0} 条消息输入消息标识前缀", i + 1), "输入验证", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    prefixTextBoxes[i].Focus();
                    return;
                }
            }

            Button btn = sender as Button;
            if (btn == null) return;

            btn.Enabled = false;
            btn.Text = "🔄 翻译中...";

            try
            {
                translations.Clear();
                // 清空预览文本框
                if (previewTextBox != null)
                {
                    previewTextBox.Text = "翻译进行中，请稍候...";
                }

                foreach (var langBtn in languageCopyButtons.Values)
                {
                    langBtn.Enabled = false;
                    langBtn.BackColor = Color.FromArgb(108, 117, 125);
                    LanguageInfo langInfo = langBtn.Tag as LanguageInfo;
                    if (langInfo != null)
                    {
                        langBtn.Text = string.Format("{0} ({1})", langInfo.Name, langInfo.Code);
                    }
                }

                List<Task> translationTasks = new List<Task>();
                foreach (var language in languages)
                {
                    translationTasks.Add(TranslateLanguage(language));
                    await Task.Delay(200);
                }

                await Task.WhenAll(translationTasks);
                MessageBox.Show("✅ 所有语言翻译完成！", "翻译完成", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // 翻译完成后更新预览文本框提示
                if (previewTextBox != null)
                {
                    previewTextBox.Text = "翻译完成！请点击下方语言按钮查看翻译结果。";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("❌ 翻译失败: {0}", ex.Message), "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (previewTextBox != null)
                {
                    previewTextBox.Text = string.Format("翻译失败: {0}", ex.Message);
                }
            }
            finally
            {
                btn.Enabled = true;
                btn.Text = "🚀 开始翻译所有语言";
            }
        }

        private async Task TranslateLanguage(LanguageInfo language)
        {
            try
            {
                Dictionary<int, string> langTranslations = new Dictionary<int, string>();
                for (int i = 0; i < sourceTexts.Length; i++)
                {
                    string text = sourceTexts[i];
                    string result = string.Empty;

                    if (language.Name == "汉语")
                    {
                        result = text;
                    }
                    else
                    {
                        // 4.7.2 异步逻辑拆分，避免嵌套推断问题
                        if (language.Name == "印度尼西亚语" || language.Name == "马来西亚" || text.Contains("/"))
                        {
                            result = await YoudaoTranslatorHelper.TranslateAsync(text, "中文", language.Name);
                        }
                        else
                        {
                            // 同步方法包装为 Task，兼容 async/await
                            result = await Task.Run(() => BaiduTranslatorHelper.TranslateWithoutCache(text, "中文", language.Name));
                        }
                    }

                    // 缓存判断（兼容4.7.2）
                    if (!result.Contains("翻译失败") && !result.Contains("API错误") && language.Name != "汉语")
                    {
                        DatabaseHelper.SaveTranslation(text, "中文", language.Name, result);
                    }

                    langTranslations.Add(i, result);
                    await Task.Delay(1000);
                }

                // 线程安全更新字典（4.7.2 支持 lock）
                lock (translations)
                {
                    translations[language.Name] = langTranslations;
                }

                // 跨线程更新UI（4.7.2 推荐 Invoke + MethodInvoker）
                this.Invoke(new MethodInvoker(delegate
                {
                    if (languageCopyButtons.TryGetValue(language.Name, out Button langBtn))
                    {
                        langBtn.Enabled = true;
                        langBtn.BackColor = Color.FromArgb(25, 135, 84);
                    }
                }));
            }
            catch
            {
                this.Invoke(new MethodInvoker(delegate
                {
                    if (languageCopyButtons.TryGetValue(language.Name, out Button langBtn))
                    {
                        langBtn.Text = string.Format("{0} 翻译失败", language.Name);
                        langBtn.BackColor = Color.FromArgb(220, 53, 69);
                    }
                }));
            }
        }

        private void BtnCopyLanguage_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            if (btn == null) return;

            LanguageInfo langInfo = btn.Tag as LanguageInfo;
            if (langInfo == null) return;

            try
            {
                StringBuilder sb = new StringBuilder();
                if (translations.TryGetValue(langInfo.Name, out Dictionary<int, string> langTranslations))
                {
                    for (int i = 0; i < sourceTexts.Length; i++)
                    {
                        if (langTranslations.TryGetValue(i, out string translationText))
                        {
                            string prefix = prefixTextBoxes[i].Text.Trim();
                            if (string.IsNullOrWhiteSpace(prefix))
                            {
                                prefix = string.Format("Message{0}", i + 1);
                            }

                            // 转义双引号，兼容格式要求
                            string escapedText = translationText.Replace("\"", "\\\"");
                            sb.AppendLine(string.Format("  \"{0}\": \"{1}\",", prefix, escapedText));
                        }
                    }

                    // 移除最后一行的多余逗号（兼容4.7.2）
                    if (sb.Length > 0)
                    {
                        sb.Length = sb.Length - Environment.NewLine.Length - 1;
                    }
                }

                // 在复制之前，先显示到预览框
                if (previewTextBox != null)
                {
                    previewTextBox.Text = sb.ToString();
                }

                // 复制到剪贴板（4.7.2 支持）
                Clipboard.SetText(sb.ToString());

                // 复制成功反馈
                string originalText = btn.Text;
                btn.Text = "✅ 已复制！";
                btn.BackColor = Color.FromArgb(19, 161, 13);

                // 定时器恢复按钮文本（4.7.2 支持 Timer）
                Timer timer = new Timer();
                timer.Interval = 1500;
                timer.Tick += new EventHandler(delegate (object s, EventArgs args)
                {
                    btn.Text = originalText;
                    btn.BackColor = Color.FromArgb(25, 135, 84);
                    timer.Stop();
                    timer.Dispose();
                });
                timer.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("复制失败: {0}", ex.Message), "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}