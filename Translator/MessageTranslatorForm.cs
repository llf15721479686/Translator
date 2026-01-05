using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Translator
{
    // Form1.cs - 添加新的翻译窗体

    public partial class MessageTranslatorForm : Form
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

        public MessageTranslatorForm(string[] texts)
        {
            this.sourceTexts = texts;
            this.languages = GetSupportedLanguages();
            this.prefixTextBoxes = new List<TextBox>();
            this.translations = new Dictionary<string, Dictionary<int, string>>();
            this.languageCopyButtons = new Dictionary<string, Button>();

            InitializeComponent();
            InitializeUI();
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

        private void InitializeComponent()
        {
            this.Size = new Size(1100, 750);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "消息翻译器";
            this.Font = new Font("微软雅黑", 10F);
            this.BackColor = Color.White;
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.MinimumSize = new Size(900, 600);
        }

        private void InitializeUI()
        {
            // 主容器
            Panel mainContainer = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(15)
            };

            // 顶部按钮面板 - 调整为更紧凑
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

         
            topFlow.Controls.AddRange(new Control[] { btnTranslate });
            topPanel.Controls.Add(topFlow);
            mainContainer.Controls.Add(topPanel);

            // 内容区域 - 使用滚动面板
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

            // 创建原文和前缀输入框的行
            CreateMessageRows();

            scrollPanel.Controls.Add(mainPanel);
            mainContainer.Controls.Add(scrollPanel);

            // 语言复制按钮区域 - 修改这部分
            Panel languagePanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 90, // 增加高度到90px，给两行按钮足够的空间
                Padding = new Padding(10, 5, 10, 5),
                Visible = false,
                BackColor = Color.FromArgb(250, 250, 250),
                BorderStyle = BorderStyle.FixedSingle,
                AutoScroll = false // 父容器不滚动
            };

            languageButtonsPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true, // 启用换行
                AutoScroll = false, // 禁用滚动条
                Padding = new Padding(5, 5, 0, 5), // 增加上下内边距
                AutoSize = true, // 允许自动调整大小
                AutoSizeMode = AutoSizeMode.GrowAndShrink // 根据内容调整
            };

            //languagePanel.Controls.Add(languageLabel);
            languagePanel.Controls.Add(languageButtonsPanel);
            mainContainer.Controls.Add(languagePanel);

            this.Controls.Add(mainContainer);
        }

        private void CreateMessageRows()
        {
            mainPanel.Controls.Clear();
            prefixTextBoxes.Clear();

            // 表头 - 调整高度和样式
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
                if (languageButtonsPanel.Parent != null)
                {
                    languageButtonsPanel.Parent.Visible = true;
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

                        // 保存到缓存 - 修改参数顺序
                        if (!result.Contains("翻译失败") && !result.Contains("API错误"))
                        {
                            // 修改这里：参数顺序改为 text, "中文", language.Name, result
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

                // 启用该语言的复制按钮 - 这里也需要更新按钮文本显示编码
                this.Invoke((MethodInvoker)delegate
                {
                    if (languageCopyButtons.TryGetValue(language.Name, out Button btn))
                    {
                        btn.Enabled = true;
                        btn.BackColor = Color.FromArgb(52, 152, 219);
                        // 确保按钮文本显示编码
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

            // 增大按钮宽度以适应编码显示
            int buttonWidth = 150; // 从95增加到120
            int buttonMargin = 6;
            int panelWidth = languageButtonsPanel.Width - languageButtonsPanel.Padding.Horizontal;
            int buttonsPerRow = Math.Max(1, panelWidth / (buttonWidth + buttonMargin));

            foreach (var language in languages)
            {
                Button btn = new Button
                {
                    // 确保这里显示编码
                    Text = $"{language.Name}({language.Code})",
                    Font = new Font("微软雅黑", 8.5f), // 稍微减小字体以适应更多内容
                    Size = new Size(150, 28), // 增大宽度
                    Margin = new Padding(3, 3, 3, 3),
                    BackColor = Color.FromArgb(200, 200, 200),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Cursor = Cursors.Hand,
                    Tag = language, // 改为存储整个LanguageInfo对象，方便后续使用
                    Enabled = false,
                    TextAlign = ContentAlignment.MiddleCenter
                };
                btn.FlatAppearance.BorderSize = 0;
                btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(52, 152, 219);

                // 更新工具提示
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

            // 现在Tag存储的是LanguageInfo对象
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
                    // 使用存储的LanguageInfo对象来重置文本
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