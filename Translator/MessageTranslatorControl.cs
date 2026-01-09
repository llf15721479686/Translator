
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Translator
{
    public partial class MessageTranslatorControl : UserControl
    {
        private List<LanguageInfo> languages;
        private string[] sourceTexts;
        private List<TextBox> prefixTextBoxes;
        private Dictionary<string, Dictionary<int, string>> translations;
        private FlowLayoutPanel mainPanel;
        private Button btnTranslate;
        private FlowLayoutPanel languageButtonsPanel;
        private Dictionary<string, Button> languageCopyButtons;
        private Panel scrollPanel;

        public MessageTranslatorControl()
        {
            InitializeComponent();
            InitializeUI();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // MessageTranslatorControl
            // 
            this.BackColor = System.Drawing.Color.White;
            this.Name = "MessageTranslatorControl";
            this.Size = new System.Drawing.Size(1839, 842);
            this.ResumeLayout(false);

        }

        private void InitializeUI()
        {
            // 主容器
            Panel mainContainer = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(15)
            };

            // 顶部按钮面板
            Panel topPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 50,
                Padding = new Padding(0, 5, 0, 10)
            };

            FlowLayoutPanel topFlow = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                AutoSize = true
            };

            // 翻译按钮
            btnTranslate = new Button
            {
                Text = "🚀 开始翻译所有语言",
                Font = new Font("微软雅黑", 11, FontStyle.Bold),
                Size = new Size(180, 30),
                BackColor = Color.FromArgb(39, 174, 96),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Margin = new Padding(0, 0, 15, 0)
            };
            btnTranslate.FlatAppearance.BorderSize = 0;
            btnTranslate.FlatAppearance.MouseOverBackColor = Color.FromArgb(46, 204, 113);
            btnTranslate.Click += BtnTranslate_Click;

            topFlow.Controls.Add(btnTranslate);
            topPanel.Controls.Add(topFlow);
            mainContainer.Controls.Add(topPanel);

            // 内容区域
            scrollPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(0, 5, 0, 10),
                AutoScroll = true,
                BackColor = Color.FromArgb(245, 245, 245)
            };

            mainPanel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                BackColor = Color.White,
                Padding = new Padding(10),
                MinimumSize = new Size(600, 0)
            };

            scrollPanel.Controls.Add(mainPanel);
            mainContainer.Controls.Add(scrollPanel);

            // 语言复制按钮区域
            Panel languagePanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 90,
                Padding = new Padding(10, 5, 10, 5),
                Visible = false,
                BackColor = Color.FromArgb(250, 250, 250),
                BorderStyle = BorderStyle.FixedSingle
            };

            languageButtonsPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
                AutoScroll = false,
                Padding = new Padding(5, 5, 0, 5),
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink
            };

            languagePanel.Controls.Add(languageButtonsPanel);
            mainContainer.Controls.Add(languagePanel);

            this.Controls.Add(mainContainer);
        }

        private List<LanguageInfo> GetSupportedLanguages()
        {
            return new List<LanguageInfo>
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
                new LanguageInfo { Name = "西班牙语", Code = "3082" }
            };
        }

        public void SetMessages(string[] messages)
        {
            this.sourceTexts = messages;
            this.languages = GetSupportedLanguages();
            this.prefixTextBoxes = new List<TextBox>();
            this.translations = new Dictionary<string, Dictionary<int, string>>();
            this.languageCopyButtons = new Dictionary<string, Button>();

            CreateMessageRows();

            // 隐藏语言按钮区域
            foreach (Control ctrl in this.Controls)
            {
                if (ctrl is Panel mainContainer)
                {
                    foreach (Control innerCtrl in mainContainer.Controls)
                    {
                        if (innerCtrl is Panel languagePanel && innerCtrl.Dock == DockStyle.Bottom)
                        {
                            innerCtrl.Visible = false;
                        }
                    }
                }
            }
        }

        private void CreateMessageRows()
        {
            mainPanel.Controls.Clear();
            prefixTextBoxes.Clear();

            if (sourceTexts == null || sourceTexts.Length == 0)
                return;

            // 表头
            Panel headerRow = CreateRow("原文", "消息标识前缀", true);
            headerRow.Height = 40;
            headerRow.BackColor = Color.FromArgb(52, 73, 94);
            foreach (Control ctrl in headerRow.Controls)
            {
                if (ctrl is Panel panel)
                {
                    foreach (Control innerCtrl in panel.Controls)
                    {
                        if (innerCtrl is Label label)
                        {
                            label.ForeColor = Color.White;
                        }
                    }
                }
            }
            mainPanel.Controls.Add(headerRow);

            // 为每条消息创建一行
            for (int i = 0; i < sourceTexts.Length; i++)
            {
                Panel messageRow = CreateMessageRow(i, sourceTexts[i]);
                mainPanel.Controls.Add(messageRow);
            }

            // 调整mainPanel的宽度以匹配容器
            if (scrollPanel != null)
            {
                mainPanel.Width = Math.Max(scrollPanel.ClientSize.Width - 30, mainPanel.MinimumSize.Width);
            }
        }

        private Panel CreateRow(string leftText, string rightText, bool isHeader = false)
        {
            int rowWidth = mainPanel.Width - mainPanel.Padding.Horizontal - 20;
            int leftWidth = (int)(rowWidth * 0.65);
            int rightWidth = rowWidth - leftWidth;

            Panel row = new Panel
            {
                Width = rowWidth,
                Height = isHeader ? 40 : 55,
                BorderStyle = isHeader ? BorderStyle.None : BorderStyle.FixedSingle,
                Margin = new Padding(0, 0, 0, 8),
                BackColor = isHeader ? Color.Transparent : Color.FromArgb(255, 255, 255)
            };

            // 左侧：原文
            Panel leftPanel = new Panel
            {
                Dock = DockStyle.Left,
                Width = leftWidth,
                Padding = new Padding(15, 12, 10, 12)
            };

            Label leftLabel = new Label
            {
                Text = leftText,
                Font = new Font("微软雅黑", isHeader ? 10 : 9.5f, isHeader ? FontStyle.Bold : FontStyle.Regular),
                Dock = DockStyle.Fill,
                TextAlign = isHeader ? ContentAlignment.MiddleLeft : ContentAlignment.TopLeft,
                ForeColor = isHeader ? Color.White : Color.FromArgb(60, 60, 60),
                AutoEllipsis = true
            };

            leftPanel.Controls.Add(leftLabel);

            // 右侧：输入框
            Panel rightPanel = new Panel
            {
                Dock = DockStyle.Right,
                Width = rightWidth,
                Padding = new Padding(10, 8, 15, 8)
            };

            if (isHeader)
            {
                Label rightLabel = new Label
                {
                    Text = rightText,
                    Font = new Font("微软雅黑", 10, FontStyle.Bold),
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleLeft,
                    ForeColor = Color.White
                };
                rightPanel.Controls.Add(rightLabel);
            }
            else
            {
                TextBox txtPrefix = new TextBox
                {
                    Font = new Font("微软雅黑", 10),
                    Dock = DockStyle.Fill,
                    BackColor = Color.FromArgb(250, 250, 250),
                    BorderStyle = BorderStyle.FixedSingle,
                    Padding = new Padding(5)
                };
                rightPanel.Controls.Add(txtPrefix);
                prefixTextBoxes.Add(txtPrefix);
            }

            row.Controls.Add(leftPanel);
            row.Controls.Add(rightPanel);

            return row;
        }

        private Panel CreateMessageRow(int index, string message)
        {
            return CreateRow($"{index + 1}. {message}", "", false);
        }

        private async void BtnTranslate_Click(object sender, EventArgs e)
        {
            if (sourceTexts == null || sourceTexts.Length == 0)
            {
                MessageBox.Show("请先在翻译表格页面输入要翻译的消息文本", "提示",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 验证所有前缀是否已输入
            for (int i = 0; i < prefixTextBoxes.Count; i++)
            {
                if (string.IsNullOrWhiteSpace(prefixTextBoxes[i].Text))
                {
                    MessageBox.Show($"请为第 {i + 1} 条消息输入消息标识前缀", "提示",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    prefixTextBoxes[i].Focus();
                    return;
                }
            }

            btnTranslate.Enabled = false;
            btnTranslate.Text = "翻译中...";

            try
            {
                // 清空之前的翻译结果
                translations.Clear();

                // 创建语言复制按钮
                CreateLanguageCopyButtons();

                // 显示语言按钮区域
                foreach (Control ctrl in this.Controls)
                {
                    if (ctrl is Panel mainContainer)
                    {
                        foreach (Control innerCtrl in mainContainer.Controls)
                        {
                            if (innerCtrl is Panel languagePanel && innerCtrl.Dock == DockStyle.Bottom)
                            {
                                innerCtrl.Visible = true;
                                break;
                            }
                        }
                    }
                }

                // 并行翻译所有语言
                var translationTasks = new List<Task>();

                foreach (var language in languages)
                {
                    var task = TranslateLanguage(language);
                    translationTasks.Add(task);
                    await Task.Delay(200); // 控制并发频率
                }

                await Task.WhenAll(translationTasks);

                MessageBox.Show("所有语言翻译完成！", "完成",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"翻译失败: {ex.Message}", "错误",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnTranslate.Enabled = true;
                btnTranslate.Text = "🚀 开始翻译所有语言";
            }
        }

        private async Task TranslateLanguage(LanguageInfo language)
        {
            try
            {
                var langTranslations = new Dictionary<int, string>();

                for (int i = 0; i < sourceTexts.Length; i++)
                {
                    string text = sourceTexts[i];

                    // 检查缓存
                    string cached = DatabaseHelper.GetCachedTranslation(text, "中文", language.Name);
                    string result;

                    if (!string.IsNullOrEmpty(cached))
                    {
                        result = cached;
                    }
                    else
                    {
                        // 翻译
                        if (language.Name == "印度尼西亚语" || language.Name == "马来西亚" || text.Contains("/"))
                        {
                            result = await YoudaoTranslatorHelper.TranslateAsync(text, "中文", language.Name);
                        }
                        else
                        {
                            result = await Task.Run(() => BaiduTranslatorHelper.TranslateWithoutCache(text, "中文", language.Name));
                        }

                        // 保存到缓存
                        if (!result.Contains("翻译失败") && !result.Contains("API错误"))
                        {
                            DatabaseHelper.SaveTranslation(text, "中文", language.Name, result);
                        }
                    }

                    langTranslations[i] = result;

                    // 控制频率
                    await Task.Delay(1000);
                }

                lock (translations)
                {
                    translations[language.Name] = langTranslations;
                }

                // 启用该语言的复制按钮
                this.Invoke((MethodInvoker)delegate
                {
                    if (languageCopyButtons.TryGetValue(language.Name, out Button btn))
                    {
                        btn.Enabled = true;
                        btn.BackColor = Color.FromArgb(52, 152, 219);
                        btn.Text = $"{language.Name}({language.Code})";
                    }
                });
            }
            catch (Exception ex)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    // 显示错误
                    if (languageCopyButtons.TryGetValue(language.Name, out Button btn))
                    {
                        btn.Text = $"{language.Name}({language.Code}) 翻译失败";
                        btn.BackColor = Color.FromArgb(231, 76, 60);
                    }
                });
            }
        }

        private void CreateLanguageCopyButtons()
        {
            languageButtonsPanel.Controls.Clear();
            languageCopyButtons.Clear();

            int buttonWidth = 150;
            int buttonMargin = 6;

            foreach (var language in languages)
            {
                Button btn = new Button
                {
                    Text = $"{language.Name}({language.Code})",
                    Font = new Font("微软雅黑", 8.5f),
                    Size = new Size(150, 28),
                    Margin = new Padding(3, 3, 3, 3),
                    BackColor = Color.FromArgb(200, 200, 200),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Cursor = Cursors.Hand,
                    Tag = language,
                    Enabled = false,
                    TextAlign = ContentAlignment.MiddleCenter
                };
                btn.FlatAppearance.BorderSize = 0;
                btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(52, 152, 219);

                ToolTip toolTip = new ToolTip();
                toolTip.SetToolTip(btn, $"复制 {language.Name}({language.Code}) 的翻译结果");

                btn.Click += BtnCopyLanguage_Click;

                languageButtonsPanel.Controls.Add(btn);
                languageCopyButtons[language.Name] = btn;
            }
        }

        private void BtnCopyLanguage_Click(object sender, EventArgs e)
        {
            var button = sender as Button;
            if (button == null) return;

            var languageInfo = button.Tag as LanguageInfo;
            if (languageInfo == null) return;

            try
            {
                StringBuilder sb = new StringBuilder();

                if (translations.TryGetValue(languageInfo.Name, out var langTranslations))
                {
                    for (int i = 0; i < sourceTexts.Length; i++)
                    {
                        if (langTranslations.TryGetValue(i, out string translation))
                        {
                            string prefix = prefixTextBoxes[i].Text.Trim();
                            if (string.IsNullOrEmpty(prefix))
                            {
                                prefix = $"Message{i + 1}";
                            }

                            sb.AppendLine($"  \"{prefix}\": \"{translation.Replace("\"", "\\\"")}\",");
                        }
                    }

                    if (sb.Length > 0)
                    {
                        sb.Length = sb.Length - Environment.NewLine.Length - 1;
                    }
                }

                Clipboard.SetText(sb.ToString());

                // 显示成功提示
                button.Text = "✅ 已复制!";
                button.BackColor = Color.FromArgb(46, 204, 113);

                Timer timer = new Timer { Interval = 1500 };
                timer.Tick += (s, args) =>
                {
                    button.Text = $"{languageInfo.Name}({languageInfo.Code})";
                    button.BackColor = Color.FromArgb(52, 152, 219);
                    timer.Stop();
                    timer.Dispose();
                };
                timer.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"复制失败: {ex.Message}", "错误",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
