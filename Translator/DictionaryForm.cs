using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Translator
{
    public partial class DictionaryForm : Form
    {
        // 语言信息类
        public class LanguageInfo
        {
            public string Name { get; set; }
            public string Code { get; set; }
            public string EnglishName { get; set; }
        }

        // 历史记录类
        public class TranslationRecord
        {
            public string SourceText { get; set; }
            public string ResultText { get; set; }
            public string SourceLang { get; set; }
            public string TargetLang { get; set; }
            public string Method { get; set; }
            public DateTime TranslateTime { get; set; }
        }

        // 字段声明
        private Dictionary<string, LanguageInfo> languages;
        private LanguageInfo sourceLanguage;
        private LanguageInfo targetLanguage;
        private int translationEngine = 1; // 1:百度 2:有道

        // 控件声明
        private TableLayoutPanel mainLayout;
        private ComboBox cmbSource;
        private ComboBox cmbTarget;
        private ComboBox cmbEngine;
        private Button btnSwap;
        private TextBox txtSource;
        private TextBox txtResult;
        private ListBox lstHistory;
        private TextBox txtDetail;
        private Button btnTranslate;
        private Button btnClear;
        private Button btnCopy;
        private Button btnFavorite;
        private Button btnHistory;
        private Button btnSettings;

        public DictionaryForm()
        {
            InitializeComponent();
            InitializeLanguages();
            InitializeForm();
            this.Resize += DictionaryForm_Resize;
        }

        // 窗体大小改变时重新布局
        private void DictionaryForm_Resize(object sender, EventArgs e)
        {
            if (this.Width < 800)
            {
                // 小屏幕布局调整
                AdjustLayoutForSmallScreen();
            }
            else
            {
                // 正常布局
                AdjustLayoutForNormalScreen();
            }
        }

        // 修改 InitializeLanguages 方法，使用已有的语种
        private void InitializeLanguages()
        {
            languages = new Dictionary<string, LanguageInfo>
            {
                { "1033", new LanguageInfo { Name = "英语", Code = "1033", EnglishName = "English" } },
                { "2052", new LanguageInfo { Name = "中文", Code = "2052", EnglishName = "Chinese" } },
                { "1025", new LanguageInfo { Name = "阿拉伯语", Code = "1025", EnglishName = "Arabic" } },
                { "1031", new LanguageInfo { Name = "德语", Code = "1031", EnglishName = "German" } },
                { "1036", new LanguageInfo { Name = "法语", Code = "1036", EnglishName = "French" } },
                { "1040", new LanguageInfo { Name = "意大利语", Code = "1040", EnglishName = "Italian" } },
                { "1046", new LanguageInfo { Name = "葡萄牙语", Code = "1046", EnglishName = "Portuguese" } },
                { "1049", new LanguageInfo { Name = "俄语", Code = "1049", EnglishName = "Russian" } },
                { "1054", new LanguageInfo { Name = "泰语", Code = "1054", EnglishName = "Thai" } },
                { "1057", new LanguageInfo { Name = "印度尼西亚语", Code = "1057", EnglishName = "Indonesian" } },
                { "1066", new LanguageInfo { Name = "越南语", Code = "1066", EnglishName = "Vietnamese" } },
                { "1086", new LanguageInfo { Name = "马来西亚语", Code = "1086", EnglishName = "Malay" } },
                { "3082", new LanguageInfo { Name = "西班牙语", Code = "3082", EnglishName = "Spanish" } }
            };

            // 默认设置：中文->英语
            sourceLanguage = languages["2052"];
            targetLanguage = languages["1033"];
        }

        // 在DictionaryForm.cs中修改InitializeForm方法和布局相关代码
        private void InitializeForm()
        {
            this.Text = "词典翻译";
            this.BackColor = Color.FromArgb(240, 242, 245);
            this.Dock = DockStyle.Fill; // 设置为填充模式

            // 关键：设置锚点和停靠属性，确保控件随容器大小变化
            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.SizeChanged += DictionaryForm_SizeChanged;

            // 创建主布局
            CreateMainLayout();
            // 添加双击事件
            lstHistory.DoubleClick += LstHistory_DoubleClick;
        }
        // 修改替换原Resize事件处理方法
        private void DictionaryForm_SizeChanged(object sender, EventArgs e)
        {
            AdjustLayout();
        }

        // 统一的布局调整方法
        private void AdjustLayout()
        {
            if (mainLayout == null) return;

            // 根据当前宽度调整布局结构
            if (this.Width < 800)
            {
                // 小屏幕布局 - 垂直排列
                SetSmallScreenLayout();
            }
            else
            {
                // 大屏幕布局 - 水平排列
                SetLargeScreenLayout();
            }

            // 调整控件大小和位置
            AdjustControlSizes();
        }

        // 设置小屏幕布局
        private void SetSmallScreenLayout()
        {
            mainLayout.ColumnCount = 1;
            mainLayout.RowCount = 4;

            mainLayout.ColumnStyles.Clear();
            mainLayout.RowStyles.Clear();

            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 120));  // 控制栏
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 30F));   // 输入区
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 30F));   // 输出区
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 40F));   // 历史和详情区

            RepositionControlsForSmallScreen();
        }

        // 设置大屏幕布局
        private void SetLargeScreenLayout()
        {
            mainLayout.ColumnCount = 3;
            mainLayout.RowCount = 3;

            mainLayout.ColumnStyles.Clear();
            mainLayout.RowStyles.Clear();

            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 180));
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));

            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 100));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 55F));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 45F));

            RepositionControlsForLargeScreen();
        }

        // 为小屏幕重新定位控件
        private void RepositionControlsForSmallScreen()
        {
            if (mainLayout.Controls.Count == 0) return;

            // 控制栏
            var controlPanel = mainLayout.Controls.OfType<Panel>().FirstOrDefault(p => p.Name == "controlPanel");
            if (controlPanel != null)
            {
                mainLayout.SetRow(controlPanel, 0);
                mainLayout.SetColumn(controlPanel, 0);
                mainLayout.SetColumnSpan(controlPanel, 1);
            }

            // 输入区、中间按钮区、输出区
            var inputGroup = mainLayout.Controls.OfType<GroupBox>().FirstOrDefault(g => g.Text.Contains("输入原文"));
            var middlePanel = mainLayout.Controls.OfType<Panel>().FirstOrDefault(p => p.Controls.OfType<Button>().Any(b => b.Text.Contains("开始翻译")));
            var outputGroup = mainLayout.Controls.OfType<GroupBox>().FirstOrDefault(g => g.Text.Contains("翻译结果"));

            if (inputGroup != null)
            {
                mainLayout.SetRow(inputGroup, 1);
                mainLayout.SetColumn(inputGroup, 0);
                mainLayout.SetColumnSpan(inputGroup, 1);
            }

            if (middlePanel != null)
            {
                middlePanel.Dock = DockStyle.Top;
                middlePanel.Height = 100;
                if (inputGroup != null)
                {
                    inputGroup.Controls.Add(middlePanel);
                }
            }

            if (outputGroup != null)
            {
                mainLayout.SetRow(outputGroup, 2);
                mainLayout.SetColumn(outputGroup, 0);
                mainLayout.SetColumnSpan(outputGroup, 1);
            }

            // 历史记录和详情区
            var historyGroup = mainLayout.Controls.OfType<GroupBox>().FirstOrDefault(g => g.Text.Contains("翻译历史"));
            var detailGroup = mainLayout.Controls.OfType<GroupBox>().FirstOrDefault(g => g.Text.Contains("翻译详情"));

            if (historyGroup != null)
            {
                mainLayout.SetRow(historyGroup, 3);
                mainLayout.SetColumn(historyGroup, 0);
                mainLayout.SetColumnSpan(historyGroup, 1);
                mainLayout.SetRowSpan(historyGroup, 1);
            }

            if (detailGroup != null && historyGroup != null)
            {
                historyGroup.Controls.Add(detailGroup);
                detailGroup.Dock = DockStyle.Right;
                detailGroup.Width = (int)(historyGroup.Width * 0.5);
            }
        }

        // 为大屏幕重新定位控件
        private void RepositionControlsForLargeScreen()
        {
            // 恢复控件到大屏幕布局位置
            var controlPanel = mainLayout.Controls.OfType<Panel>().FirstOrDefault(p => p.Name == "controlPanel");
            if (controlPanel != null)
            {
                mainLayout.SetRow(controlPanel, 0);
                mainLayout.SetColumn(controlPanel, 0);
                mainLayout.SetColumnSpan(controlPanel, 3);
            }

            var inputGroup = mainLayout.Controls.OfType<GroupBox>().FirstOrDefault(g => g.Text.Contains("输入原文"));
            var middlePanel = mainLayout.Controls.OfType<Panel>().FirstOrDefault(p => p.Controls.OfType<Button>().Any(b => b.Text.Contains("开始翻译")));
            var outputGroup = mainLayout.Controls.OfType<GroupBox>().FirstOrDefault(g => g.Text.Contains("翻译结果"));

            if (inputGroup != null)
            {
                mainLayout.SetRow(inputGroup, 1);
                mainLayout.SetColumn(inputGroup, 0);
                mainLayout.SetColumnSpan(inputGroup, 1);
                if (middlePanel != null && inputGroup.Controls.Contains(middlePanel))
                {
                    inputGroup.Controls.Remove(middlePanel);
                    mainLayout.Controls.Add(middlePanel);
                }
            }

            if (middlePanel != null)
            {
                middlePanel.Dock = DockStyle.Fill;
                mainLayout.SetRow(middlePanel, 1);
                mainLayout.SetColumn(middlePanel, 1);
            }

            if (outputGroup != null)
            {
                mainLayout.SetRow(outputGroup, 1);
                mainLayout.SetColumn(outputGroup, 2);
            }

            var historyGroup = mainLayout.Controls.OfType<GroupBox>().FirstOrDefault(g => g.Text.Contains("翻译历史"));
            var detailGroup = historyGroup?.Controls.OfType<GroupBox>().FirstOrDefault(g => g.Text.Contains("翻译详情"));

            if (historyGroup != null)
            {
                mainLayout.SetRow(historyGroup, 2);
                mainLayout.SetColumn(historyGroup, 0);
                mainLayout.SetColumnSpan(historyGroup, 2);
                mainLayout.SetRowSpan(historyGroup, 1);

                if (detailGroup != null)
                {
                    historyGroup.Controls.Remove(detailGroup);
                    mainLayout.Controls.Add(detailGroup);
                }
            }

            if (detailGroup != null)
            {
                mainLayout.SetRow(detailGroup, 2);
                mainLayout.SetColumn(detailGroup, 2);
            }
        }
        // 调整控件大小
        private void AdjustControlSizes()
        {
            // 调整按钮大小
            if (btnTranslate != null)
            {
                btnTranslate.Width = (int)(this.Width * 0.15);
                btnTranslate.Height = (int)(this.Height * 0.05);
            }

            // 调整字体大小以适应屏幕
            float fontSize = Math.Max(8, this.Width / 100f);
            if (txtSource != null)
            {
                txtSource.Font = new Font("微软雅黑", fontSize);
            }
            if (txtResult != null)
            {
                txtResult.Font = new Font("微软雅黑", fontSize);
            }
        }
        // 创建主布局
        private void CreateMainLayout()
        {
            // 主表格布局
            mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10),
                ColumnCount = 3,
                RowCount = 3, // 减少一行，移除标题栏
                BackColor = Color.Transparent
            };

            // 设置列宽 - 自适应比例
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 180)); // 中间按钮区域固定宽度
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));

            // 设置行高 - 自适应
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 100)); // 控制栏高度
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 55F));  // 输入输出区域
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 45F));  // 历史记录和详情

            // 1. 语言选择和控制栏（现在在第0行）
            CreateControlBar(mainLayout);

            // 2. 输入输出区域（现在在第1行）
            CreateInputOutputArea(mainLayout);

            // 3. 历史记录和详情区域（现在在第2行）
            CreateHistoryDetailArea(mainLayout);

            this.Controls.Add(mainLayout);
        }

        // 小屏幕布局调整
        private void AdjustLayoutForSmallScreen()
        {
            if (mainLayout == null) return;

            mainLayout.ColumnCount = 2;
            mainLayout.RowCount = 4;

            // 重新设置行列比例
            mainLayout.ColumnStyles.Clear();
            mainLayout.RowStyles.Clear();

            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));

            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 120));  // 控制栏
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 35F));   // 输入
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 35F));   // 输出
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 30F));   // 历史和详情

            // 重新排列控件
            ReorderControlsForSmallScreen();
        }

        // 正常屏幕布局
        private void AdjustLayoutForNormalScreen()
        {
            if (mainLayout == null) return;

            mainLayout.ColumnCount = 3;
            mainLayout.RowCount = 3;

            // 重新设置行列比例
            mainLayout.ColumnStyles.Clear();
            mainLayout.RowStyles.Clear();

            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 180));
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));

            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 100));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 55F));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 45F));

            // 恢复控件布局
            RestoreControlsForNormalScreen();
        }

        private void ReorderControlsForSmallScreen()
        {
            // 这里需要重新排列控件位置
            // 简化版：调整控件大小
            if (btnTranslate != null)
            {
                btnTranslate.Size = new Size(140, 35);
                btnTranslate.Font = new Font("微软雅黑", 10F, FontStyle.Bold);
            }
        }

        private void RestoreControlsForNormalScreen()
        {
            if (btnTranslate != null)
            {
                btnTranslate.Size = new Size(160, 40);
                btnTranslate.Font = new Font("微软雅黑", 11F, FontStyle.Bold);
            }
        }

        // 创建控制栏（现在位于第0行）
        private void CreateControlBar(TableLayoutPanel mainLayout)
        {
            Panel controlPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(10),
                BorderStyle = BorderStyle.FixedSingle
            };

            // 第一行：语言选择和引擎选择
            TableLayoutPanel topRowLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 40,
                ColumnCount = 5,
                BackColor = Color.Transparent
            };

            // 设置列宽
            topRowLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 60));  // "源语言"标签
            topRowLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));  // 源语言下拉框
            topRowLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 70));  // "目标语言"标签
            topRowLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));  // 目标语言下拉框
            topRowLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100)); // 交换和引擎选择

            // 源语言选择
            Label lblSource = new Label
            {
                Text = "源语言:",
                Font = new Font("微软雅黑", 9F, FontStyle.Bold),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                ForeColor = Color.FromArgb(64, 64, 64)
            };

            cmbSource = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("微软雅黑", 9F),
                Dock = DockStyle.Fill,
                FlatStyle = FlatStyle.Flat
            };

            // 目标语言选择
            Label lblTarget = new Label
            {
                Text = "目标语言:",
                Font = new Font("微软雅黑", 9F, FontStyle.Bold),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                ForeColor = Color.FromArgb(64, 64, 64)
            };

            cmbTarget = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("微软雅黑", 9F),
                Dock = DockStyle.Fill,
                FlatStyle = FlatStyle.Flat
            };

            // 交换按钮和引擎选择面板
            Panel rightPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent
            };

            btnSwap = new Button
            {
                Text = "↔ 交换",
                Font = new Font("微软雅黑", 9F, FontStyle.Bold),
                Size = new Size(70, 25),
                Location = new Point(0, 5),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnSwap.FlatAppearance.BorderSize = 0;
            btnSwap.Click += BtnSwap_Click;

            // 填充语言列表
            foreach (var lang in languages.Values.OrderBy(l => l.Name))
            {
                cmbSource.Items.Add($"{lang.Name} [{lang.Code}]");
                cmbTarget.Items.Add($"{lang.Name} [{lang.Code}]");
            }

            // 设置默认选择
            cmbSource.SelectedItem = $"{sourceLanguage.Name} [{sourceLanguage.Code}]";
            cmbTarget.SelectedItem = $"{targetLanguage.Name} [{targetLanguage.Code}]";
            cmbSource.SelectedIndexChanged += Language_SelectedIndexChanged;
            cmbTarget.SelectedIndexChanged += Language_SelectedIndexChanged;

            // 第二行：引擎选择和其他按钮
            TableLayoutPanel bottomRowLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Bottom,
                Height = 40,
                Top = 45,
                ColumnCount = 4,
                BackColor = Color.Transparent
            };

            // 设置列宽
            bottomRowLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 60));  // "引擎"标签
            bottomRowLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));  // 引擎下拉框
            bottomRowLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100)); // 历史记录按钮
            bottomRowLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80));  // 设置按钮

            // 翻译引擎选择
            Label lblEngine = new Label
            {
                Text = "引擎:",
                Font = new Font("微软雅黑", 9F, FontStyle.Bold),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                ForeColor = Color.FromArgb(64, 64, 64)
            };

            cmbEngine = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("微软雅黑", 9F),
                Dock = DockStyle.Fill,
                FlatStyle = FlatStyle.Flat
            };
            cmbEngine.Items.AddRange(new object[] { "百度翻译", "有道翻译" });
            cmbEngine.SelectedIndex = 0;
            cmbEngine.SelectedIndexChanged += CmbEngine_SelectedIndexChanged;

            // 历史记录按钮
            btnHistory = new Button
            {
                Text = "📋 历史记录",
                Font = new Font("微软雅黑", 9F),
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(155, 89, 182),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnHistory.FlatAppearance.BorderSize = 0;
            btnHistory.Click += BtnHistory_Click;

            // 设置按钮
            btnSettings = new Button
            {
                Text = "⚙️ 设置",
                Font = new Font("微软雅黑", 9F),
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(149, 165, 166),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnSettings.FlatAppearance.BorderSize = 0;
            btnSettings.Click += BtnSettings_Click;

            // 添加到顶部行布局
            topRowLayout.Controls.Add(lblSource, 0, 0);
            topRowLayout.Controls.Add(cmbSource, 1, 0);
            topRowLayout.Controls.Add(lblTarget, 2, 0);
            topRowLayout.Controls.Add(cmbTarget, 3, 0);
            rightPanel.Controls.Add(btnSwap);
            topRowLayout.Controls.Add(rightPanel, 4, 0);

            // 添加到底部行布局
            bottomRowLayout.Controls.Add(lblEngine, 0, 0);
            bottomRowLayout.Controls.Add(cmbEngine, 1, 0);
            bottomRowLayout.Controls.Add(btnHistory, 2, 0);
            bottomRowLayout.Controls.Add(btnSettings, 3, 0);

            // 添加到控制面板
            controlPanel.Controls.Add(topRowLayout);
            controlPanel.Controls.Add(bottomRowLayout);

            mainLayout.SetColumnSpan(controlPanel, 3);
            mainLayout.Controls.Add(controlPanel, 0, 0);
        }

        // 创建输入输出区域（现在在第1行）
        private void CreateInputOutputArea(TableLayoutPanel mainLayout)
        {
            // 输入区域
            GroupBox inputGroup = new GroupBox
            {
                Text = "📝 输入原文",
                Font = new Font("微软雅黑", 10F, FontStyle.Bold),
                Dock = DockStyle.Fill,
                ForeColor = Color.FromArgb(44, 62, 80),
                Padding = new Padding(5),
                Margin = new Padding(5)
            };

            txtSource = new TextBox
            {
                Multiline = true,
                Dock = DockStyle.Fill,
                Font = new Font("微软雅黑", 10F),
                ScrollBars = ScrollBars.Vertical,
                BorderStyle = BorderStyle.None,
                Padding = new Padding(8),
                AcceptsTab = true,
                AcceptsReturn = true
            };
            inputGroup.Controls.Add(txtSource);

            // 输出区域
            GroupBox outputGroup = new GroupBox
            {
                Text = "💡 翻译结果",
                Font = new Font("微软雅黑", 10F, FontStyle.Bold),
                Dock = DockStyle.Fill,
                ForeColor = Color.FromArgb(44, 62, 80),
                Padding = new Padding(5),
                Margin = new Padding(5)
            };

            txtResult = new TextBox
            {
                Multiline = true,
                Dock = DockStyle.Fill,
                Font = new Font("微软雅黑", 10F),
                ScrollBars = ScrollBars.Vertical,
                BorderStyle = BorderStyle.None,
                ReadOnly = true,
                Padding = new Padding(8),
                BackColor = Color.FromArgb(248, 249, 250)
            };
            outputGroup.Controls.Add(txtResult);

            // 中间操作面板
            Panel middlePanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent,
                Padding = new Padding(10),
                Margin = new Padding(5)
            };

            btnTranslate = new Button
            {
                Text = "🚀 开始翻译",
                Font = new Font("微软雅黑", 11F, FontStyle.Bold),
                Size = new Size(160, 40),
                Location = new Point(10, 30),
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnTranslate.FlatAppearance.BorderSize = 0;
            btnTranslate.Click += BtnTranslate_Click;

            btnClear = new Button
            {
                Text = "🗑️ 清空",
                Font = new Font("微软雅黑", 9F),
                Size = new Size(140, 35),
                Location = new Point(20, 85),
                BackColor = Color.FromArgb(231, 76, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnClear.FlatAppearance.BorderSize = 0;
            btnClear.Click += BtnClear_Click;

            btnCopy = new Button
            {
                Text = "📋 复制结果",
                Font = new Font("微软雅黑", 9F),
                Size = new Size(140, 35),
                Location = new Point(20, 130),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnCopy.FlatAppearance.BorderSize = 0;
            btnCopy.Click += BtnCopy_Click;

            middlePanel.Controls.AddRange(new Control[] { btnTranslate, btnClear, btnCopy });

            // 添加到主布局
            mainLayout.Controls.Add(inputGroup, 0, 1);
            mainLayout.Controls.Add(middlePanel, 1, 1);
            mainLayout.Controls.Add(outputGroup, 2, 1);
        }

        // 创建历史记录和详情区域（现在在第2行）
        private void CreateHistoryDetailArea(TableLayoutPanel mainLayout)
        {
            // 历史记录区域
            GroupBox historyGroup = new GroupBox
            {
                Text = "📜 翻译历史",
                Font = new Font("微软雅黑", 10F, FontStyle.Bold),
                Dock = DockStyle.Fill,
                ForeColor = Color.FromArgb(44, 62, 80),
                Padding = new Padding(5),
                Margin = new Padding(5)
            };

            lstHistory = new ListBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("微软雅黑", 9F),
                BorderStyle = BorderStyle.None,
                ItemHeight = 22,
                ScrollAlwaysVisible = true,
                BackColor = Color.FromArgb(248, 249, 250)
            };
            lstHistory.SelectedIndexChanged += LstHistory_SelectedIndexChanged;

            // 历史记录操作按钮
            Panel historyButtonPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 40,
                Padding = new Padding(5),
                BackColor = Color.FromArgb(248, 249, 250)
            };

            btnFavorite = new Button
            {
                Text = "⭐ 收藏",
                Font = new Font("微软雅黑", 9F),
                Size = new Size(70, 30),
                Location = new Point(10, 5),
                BackColor = Color.FromArgb(241, 196, 15),
                ForeColor = Color.Black,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnFavorite.FlatAppearance.BorderSize = 0;
            btnFavorite.Click += BtnFavorite_Click;

            Button btnClearHistory = new Button
            {
                Text = "清除",
                Font = new Font("微软雅黑", 9F),
                Size = new Size(60, 30),
                Location = new Point(90, 5),
                BackColor = Color.FromArgb(149, 165, 166),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnClearHistory.FlatAppearance.BorderSize = 0;
            btnClearHistory.Click += BtnClearHistory_Click;

            historyButtonPanel.Controls.AddRange(new Control[] { btnFavorite, btnClearHistory });

            Panel historyContainer = new Panel { Dock = DockStyle.Fill };
            historyContainer.Controls.Add(lstHistory);
            historyContainer.Controls.Add(historyButtonPanel);
            historyGroup.Controls.Add(historyContainer);

            // 详情区域
            GroupBox detailGroup = new GroupBox
            {
                Text = "🔍 翻译详情",
                Font = new Font("微软雅黑", 10F, FontStyle.Bold),
                Dock = DockStyle.Fill,
                ForeColor = Color.FromArgb(44, 62, 80),
                Padding = new Padding(5),
                Margin = new Padding(5)
            };

            txtDetail = new TextBox
            {
                Multiline = true,
                Dock = DockStyle.Fill,
                Font = new Font("微软雅黑", 9F),
                ScrollBars = ScrollBars.Vertical,
                BorderStyle = BorderStyle.None,
                ReadOnly = true,
                Padding = new Padding(8),
                BackColor = Color.FromArgb(248, 249, 250)
            };
            detailGroup.Controls.Add(txtDetail);

            // 添加到主布局
            mainLayout.Controls.Add(historyGroup, 0, 2);
            mainLayout.SetColumnSpan(historyGroup, 2);
            mainLayout.Controls.Add(detailGroup, 2, 2);
        }

        // ==================== 事件处理方法 ====================

        private void DictionaryForm_Load(object sender, EventArgs e)
        {
            LoadTranslationHistory();
        }

        private void Language_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateSelectedLanguages();
        }

        private void UpdateSelectedLanguages()
        {
            try
            {
                if (cmbSource.SelectedItem != null)
                {
                    string sourceCode = cmbSource.SelectedItem.ToString().Split('[')[1].Trim(']');
                    if (languages.ContainsKey(sourceCode))
                        sourceLanguage = languages[sourceCode];
                }

                if (cmbTarget.SelectedItem != null)
                {
                    string targetCode = cmbTarget.SelectedItem.ToString().Split('[')[1].Trim(']');
                    if (languages.ContainsKey(targetCode))
                        targetLanguage = languages[targetCode];
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"更新语言选择失败: {ex.Message}");
            }
        }

        private void BtnSwap_Click(object sender, EventArgs e)
        {
            int sourceIndex = cmbSource.SelectedIndex;
            int targetIndex = cmbTarget.SelectedIndex;

            if (sourceIndex >= 0 && targetIndex >= 0)
            {
                cmbSource.SelectedIndex = targetIndex;
                cmbTarget.SelectedIndex = sourceIndex;

                // 交换文本框内容
                string tempText = txtSource.Text;
                txtSource.Text = txtResult.Text;
                txtResult.Text = tempText;
            }
        }

        private void CmbEngine_SelectedIndexChanged(object sender, EventArgs e)
        {
            translationEngine = cmbEngine.SelectedIndex + 1; // 1:百度, 2:有道
        }

        private async void BtnTranslate_Click(object sender, EventArgs e)
        {
            await TranslateText();
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            txtSource.Clear();
            txtResult.Clear();
            txtDetail.Clear();
        }

        private void BtnCopy_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtResult.Text))
            {
                try
                {
                    Clipboard.SetText(txtResult.Text);
                    MessageBox.Show("翻译结果已复制到剪贴板", "提示",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"复制失败: {ex.Message}", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void BtnFavorite_Click(object sender, EventArgs e)
        {
            SaveToFavorites();
        }

        private void BtnHistory_Click(object sender, EventArgs e)
        {
            LoadTranslationHistory();
            MessageBox.Show("已刷新历史记录", "提示",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnSettings_Click(object sender, EventArgs e)
        {
            ShowSettingsDialog();
        }

        private void BtnClearHistory_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("确定要清除所有历史记录吗？", "确认",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                ClearTranslationHistory();
            }
        }

        private void LstHistory_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstHistory.SelectedIndex >= 0)
            {
                string selectedItem = lstHistory.SelectedItem.ToString();
                ShowHistoryDetail(selectedItem);
            }
        }

        // ==================== 核心翻译逻辑 ====================

        private async Task TranslateText()
        {
            string text = txtSource.Text.Trim();

            if (string.IsNullOrWhiteSpace(text))
            {
                MessageBox.Show("请输入要翻译的文本", "提示",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (sourceLanguage.Code == targetLanguage.Code)
            {
                MessageBox.Show("源语言和目标语言不能相同", "提示",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 禁用按钮，显示加载状态
            btnTranslate.Enabled = false;
            btnTranslate.Text = "翻译中...";
            txtResult.Text = "正在翻译...";
            txtDetail.Text = $"正在从 {sourceLanguage.Name} 翻译到 {targetLanguage.Name}...";

            try
            {
                // 1. 先尝试从数据库获取翻译
                string cachedResult = await GetCachedTranslationAsync(text);

                if (!string.IsNullOrEmpty(cachedResult) &&
                    !cachedResult.Contains("未找到") &&
                    !cachedResult.Contains("错误"))
                {
                    // 使用缓存的翻译结果
                    DisplayTranslationResult(text, cachedResult, "数据库缓存");
                    return;
                }

                // 2. 缓存未命中，调用API翻译
                string apiResult = await TranslateWithAPIAsync(text);

                if (!string.IsNullOrEmpty(apiResult) &&
                    !apiResult.Contains("翻译失败") &&
                    !apiResult.Contains("错误"))
                {
                    DisplayTranslationResult(text, apiResult,
                        translationEngine == 1 ? "百度翻译" : "有道翻译");

                    // 保存到数据库缓存
                    SaveTranslationToCache(text, apiResult);
                }
                else
                {
                    ShowTranslationError(apiResult);
                }
            }
            catch (Exception ex)
            {
                ShowTranslationError($"翻译出错: {ex.Message}");
            }
            finally
            {
                btnTranslate.Enabled = true;
                btnTranslate.Text = "🚀 开始翻译";
            }
        }

        private async Task<string> GetCachedTranslationAsync(string text)
        {
            try
            {
                return await Task.Run(() =>
                    DatabaseHelper.GetCachedTranslation(
                        text,
                        sourceLanguage.Name,
                        targetLanguage.Name));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"获取缓存翻译失败: {ex.Message}");
                return "";
            }
        }

        private async Task<string> TranslateWithAPIAsync(string text)
        {
            try
            {
                if (translationEngine == 1) // 百度翻译
                {
                    return await Task.Run(() =>
                        BaiduTranslatorHelper.TranslateWithoutCache(
                            text, sourceLanguage.Name, targetLanguage.Name));
                }
                else // 有道翻译
                {
                    return await YoudaoTranslatorHelper.TranslateAsync(
                            text, sourceLanguage.Name, targetLanguage.Name);
                }
            }
            catch (Exception ex)
            {
                return $"API翻译失败: {ex.Message}";
            }
        }

        private void DisplayTranslationResult(string sourceText, string resultText, string method)
        {
            txtResult.Text = resultText;

            txtDetail.Text = $"✅ 翻译成功！\n\n" +
                           $"来源: {method}\n" +
                           $"原文: {sourceText}\n" +
                           $"译文: {resultText}\n" +
                           $"源语言: {sourceLanguage.Name} [{sourceLanguage.Code}]\n" +
                           $"目标语言: {targetLanguage.Name} [{targetLanguage.Code}]\n" +
                           $"翻译时间: {DateTime.Now:yyyy-MM-dd HH:mm:ss}\n" +
                           $"引擎: {(translationEngine == 1 ? "百度翻译" : "有道翻译")}";

            AddToHistory(sourceText, resultText, method);
        }

        private void ShowTranslationError(string errorMessage)
        {
            txtResult.Text = "翻译失败";
            txtDetail.Text = $"❌ 翻译失败\n\n" +
                           $"错误信息: {errorMessage}\n" +
                           $"建议:\n" +
                           $"1. 检查网络连接\n" +
                           $"2. 确认API密钥配置正确\n" +
                           $"3. 尝试切换翻译引擎\n" +
                           $"4. 稍后重试";
        }

        private void SaveTranslationToCache(string sourceText, string resultText)
        {
            try
            {
                DatabaseHelper.SaveTranslation(
                    sourceText, resultText,
                    sourceLanguage.Name, targetLanguage.Name);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"保存到缓存失败: {ex.Message}");
            }
        }

        // ==================== 历史记录管理 ====================

        private void AddToHistory(string source, string result, string method)
        {
            string historyItem = $"[{DateTime.Now:HH:mm}] " +
                               $"{sourceLanguage.Name}→{targetLanguage.Name}: " +
                               $"{TruncateText(source, 25)} → {TruncateText(result, 25)}";

            lstHistory.Items.Insert(0, historyItem);

            // 限制历史记录数量
            if (lstHistory.Items.Count > 50)
            {
                lstHistory.Items.RemoveAt(lstHistory.Items.Count - 1);
            }

            // 保存到数据库
            SaveToHistoryDatabase(source, result, method);
        }

        private void SaveToHistoryDatabase(string source, string result, string method)
        {
            try
            {
                DatabaseHelper.SaveTranslationHistory(
                    source, result,
                    sourceLanguage.Code, targetLanguage.Code,
                    method, DateTime.Now);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"保存历史记录失败: {ex.Message}");
            }
        }

        private void LoadTranslationHistory()
        {
            try
            {
                var history = DatabaseHelper.GetTranslationHistory(30);
                lstHistory.Items.Clear();

                foreach (var item in history)
                {
                    string sourceLang = languages.ContainsKey(item.SourceLangCode) ?
                        languages[item.SourceLangCode].Name : item.SourceLangCode;
                    string targetLang = languages.ContainsKey(item.TargetLangCode) ?
                        languages[item.TargetLangCode].Name : item.TargetLangCode;

                    string historyItem = $"[{item.TranslateTime:HH:mm}] {sourceLang}→{targetLang}: " +
                                       $"{TruncateText(item.SourceText, 20)} → {TruncateText(item.ResultText, 20)}";

                    lstHistory.Items.Add(historyItem);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"加载历史记录失败: {ex.Message}");
            }
        }

        private void ClearTranslationHistory()
        {
            try
            {
                DatabaseHelper.ClearAllHistory();
                lstHistory.Items.Clear();
                MessageBox.Show("历史记录已清除", "提示",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"清除历史记录失败: {ex.Message}", "错误",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ShowHistoryDetail(string historyItem)
        {
            txtDetail.Text = $"📜 历史记录详情\n\n" +
                           $"选择项: {historyItem}\n\n" +
                           $"双击历史记录项可以快速加载到输入框";
        }

        // ==================== 收藏功能 ====================

        private void SaveToFavorites()
        {
            string source = txtSource.Text.Trim();
            string result = txtResult.Text.Trim();

            if (string.IsNullOrWhiteSpace(source) || string.IsNullOrWhiteSpace(result))
            {
                MessageBox.Show("没有可收藏的翻译内容", "提示",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                DatabaseHelper.SaveToFavorites(
                    source, result,
                    sourceLanguage.Code, targetLanguage.Code,
                    translationEngine == 1 ? "百度" : "有道");

                MessageBox.Show("✅ 已成功添加到收藏夹", "成功",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ 添加到收藏失败: {ex.Message}", "错误",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ==================== 设置功能 ====================

        private void ShowSettingsDialog()
        {
            SettingsForm settingsForm = new SettingsForm();
            settingsForm.StartPosition = FormStartPosition.CenterParent;
            settingsForm.ShowDialog();
        }

        // 双击历史记录项加载到输入框
        private void LstHistory_DoubleClick(object sender, EventArgs e)
        {
            if (lstHistory.SelectedIndex >= 0)
            {
                try
                {
                    // 从数据库中获取完整的历史记录
                    var history = DatabaseHelper.GetTranslationHistory(50);
                    if (history.Count > 0 && lstHistory.SelectedIndex < history.Count)
                    {
                        var selectedHistory = history[lstHistory.SelectedIndex];
                        txtSource.Text = selectedHistory.SourceText;
                        // 自动触发翻译
                        BtnTranslate_Click(sender, e);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"加载历史记录失败: {ex.Message}", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // ==================== 辅助方法 ====================

        private string TruncateText(string text, int maxLength)
        {
            if (string.IsNullOrEmpty(text) || text.Length <= maxLength)
                return text;

            return text.Substring(0, maxLength) + "...";
        }
    }
}