// HomeForm.cs - 完整修复版本
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Translator
{
    public partial class HomeForm : Form
    {
        private Form1 translatorForm;
        private CacheManagementForm cacheForm;
        private Control currentControl;
        private Panel contentPanel; // 专门用于显示内容的Panel

        // 在HomeForm的构造函数中
        public HomeForm()
        {
            // 先执行控件初始化
            InitializeComponent();

            // 初始化完成后再设置属性
            if (splitContainer != null)
            {
                splitContainer.Panel1MinSize = 200; // 左侧导航最小宽度
                splitContainer.Panel2MinSize = 600; // 右侧内容最小宽度
                splitContainer.IsSplitterFixed = false;
            }
            else
            {
                // 调试用：如果splitContainer仍为null，检查设计器文件
                MessageBox.Show("splitContainer控件未正确初始化", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            // 其他初始化代码...
            InitializeTreeView();
            InitializeContentPanel();
            AddHeaderPanel();
            ShowWelcomePage();
        }

        private void InitializeTreeView()
        {
            // 设置TreeView样式
            treeView.Font = new Font("Microsoft YaHei UI", 10F);
            treeView.BackColor = Color.FromArgb(45, 45, 48);
            treeView.ForeColor = Color.White;
            treeView.BorderStyle = BorderStyle.None;
            treeView.FullRowSelect = true;
            treeView.ShowLines = false;
            treeView.ShowRootLines = false;
            treeView.ShowPlusMinus = false;
            treeView.ItemHeight = 40;
            treeView.Indent = 25;

            // 创建根节点
            TreeNode rootNode = new TreeNode("翻译工具");
            rootNode.Tag = "root";

            // 创建翻译节点
            TreeNode translateNode = new TreeNode("批量翻译");
            translateNode.Tag = "batch_translate";

            // 创建字典翻译节点
            //TreeNode dictionaryNode = new TreeNode("词典翻译");
            //dictionaryNode.Tag = "dictionary";

            // 创建缓存管理节点
            TreeNode cacheNode = new TreeNode("缓存管理");
            cacheNode.Tag = "cache";

            // 创建设置节点
            TreeNode settingsNode = new TreeNode("系统设置");
            settingsNode.Tag = "settings";

            // 添加子节点到根节点
            rootNode.Nodes.Add(translateNode);
            //rootNode.Nodes.Add(dictionaryNode);
            rootNode.Nodes.Add(cacheNode);
            rootNode.Nodes.Add(settingsNode);

            // 添加到TreeView
            treeView.Nodes.Add(rootNode);
            rootNode.ExpandAll();

            // 注册事件
            treeView.AfterSelect += TreeView_AfterSelect;

            // 确保TreeView可见
            treeView.Visible = true;
        }

        private void InitializeContentPanel()
        {
            // 创建专门的内容面板
            contentPanel = new Panel();
            contentPanel.Dock = DockStyle.Fill;
            contentPanel.BackColor = Color.White;
            contentPanel.Padding = new Padding(0, 50, 0, 0); // 顶部留出50像素给标题栏

            // 将内容面板添加到主面板的Panel2
            splitContainer.Panel2.Controls.Add(contentPanel);
            contentPanel.BringToFront();
        }

        private void AddHeaderPanel()
        {
            Panel headerPanel = new Panel();
            headerPanel.Dock = DockStyle.Top;
            headerPanel.Height = 50;
            headerPanel.BackColor = Color.FromArgb(37, 37, 38);

            // 添加标题
            Label titleLabel = new Label();
            titleLabel.Text = "多语言翻译工具";
            titleLabel.Font = new Font("Microsoft YaHei UI", 14F, FontStyle.Bold);
            titleLabel.ForeColor = Color.White;
            titleLabel.AutoSize = false;
            titleLabel.Dock = DockStyle.Left;
            titleLabel.TextAlign = ContentAlignment.MiddleLeft;
            titleLabel.Padding = new Padding(20, 0, 0, 0);
            titleLabel.Height = 50;

            // 添加最小化、最大化、关闭按钮
            FlowLayoutPanel buttonPanel = new FlowLayoutPanel();
            buttonPanel.Dock = DockStyle.Right;
            buttonPanel.FlowDirection = FlowDirection.RightToLeft;
            buttonPanel.Padding = new Padding(0, 10, 10, 10);
            buttonPanel.AutoSize = true;

            Button btnMinimize = CreateHeaderButton("─", "最小化");
            Button btnMaximize = CreateHeaderButton("□", "最大化");
            Button btnClose = CreateHeaderButton("×", "关闭");

            btnMinimize.Click += (s, e) => this.WindowState = FormWindowState.Minimized;
            btnMaximize.Click += (s, e) =>
                this.WindowState = this.WindowState == FormWindowState.Maximized ?
                FormWindowState.Normal : FormWindowState.Maximized;
            btnClose.Click += (s, e) => Application.Exit();

            buttonPanel.Controls.Add(btnClose);
            buttonPanel.Controls.Add(btnMaximize);
            buttonPanel.Controls.Add(btnMinimize);

            headerPanel.Controls.Add(titleLabel);
            headerPanel.Controls.Add(buttonPanel);

            // 将标题栏添加到Panel2的最上面
            splitContainer.Panel2.Controls.Add(headerPanel);
            splitContainer.Panel2.Controls.SetChildIndex(headerPanel, 0);

            // 确保标题栏可见
            headerPanel.BringToFront();
        }

        private Button CreateHeaderButton(string text, string tooltip)
        {
            Button button = new Button();
            button.Text = text;
            button.Font = new Font("Microsoft YaHei UI", 12F);
            button.ForeColor = Color.White;
            button.BackColor = Color.Transparent;
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.FlatAppearance.MouseOverBackColor = Color.FromArgb(62, 62, 64);
            button.FlatAppearance.MouseDownBackColor = Color.FromArgb(27, 27, 28);
            button.Size = new Size(40, 30);
            button.Cursor = Cursors.Hand;
            button.Margin = new Padding(2);
            button.TabStop = false;

            ToolTip tip = new ToolTip();
            tip.SetToolTip(button, tooltip);

            return button;
        }

        private void TreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Tag == null) return;

            string tag = e.Node.Tag.ToString();

            switch (tag)
            {
                case "batch_translate":
                    ShowTranslatorForm();
                    break;
                //case "dictionary":
                //    ShowDictionaryForm();
                //    break;
                case "cache":
                    ShowCacheForm();
                    break;
                case "settings":
                    ShowSettingsForm();
                    break;
                default:
                    ShowWelcomePage();
                    break;
            }
        }

        private void ShowTranslatorForm()
        {
            if (translatorForm == null || translatorForm.IsDisposed)
            {
                translatorForm = new Form1();
            }
            LayoutHelper.AdaptFormToContainer(translatorForm, contentPanel);
        }

        private void ShowCacheForm()
        {
            if (cacheForm == null || cacheForm.IsDisposed)
            {
                cacheForm = new CacheManagementForm();
            }
            LayoutHelper.AdaptFormToContainer(cacheForm, contentPanel);
        }

        // 在 HomeForm.cs 中修改 ShowSettingsForm 方法
        private void ShowSettingsForm()
        {
            // 使用新的 SettingsForm
            SettingsForm settingsForm = new SettingsForm();
            LayoutHelper.AdaptFormToContainer(settingsForm, contentPanel);
        }
        private void ShowDictionaryForm()
        {
            DictionaryForm dictionaryForm = new DictionaryForm();
            LayoutHelper.AdaptFormToContainer(dictionaryForm, contentPanel);
        }
        private void ShowWelcomePage()
        {
            Panel welcomePanel = new Panel();
            welcomePanel.Dock = DockStyle.Fill;
            welcomePanel.BackColor = Color.White;
            welcomePanel.Padding = new Padding(40, 80, 40, 40);

            // 添加欢迎标题
            Label welcomeLabel = new Label();
            welcomeLabel.Text = "欢迎使用多语言翻译工具";
            welcomeLabel.Font = new Font("Microsoft YaHei UI", 24F, FontStyle.Bold);
            welcomeLabel.AutoSize = true;
            welcomeLabel.ForeColor = Color.FromArgb(51, 153, 255);
            welcomeLabel.Location = new Point(40, 40);

            // 添加描述文本
            Label descLabel = new Label();
            descLabel.Text = "请从左侧导航栏选择功能：\n\n" +
                            "• 📝 批量翻译 - 同时翻译到多种语言\n" +
                            "• 📚 词典翻译 - 单句翻译，支持历史记录和收藏\n" +
                            "• 💾 缓存管理 - 管理翻译缓存数据\n" +
                            "• ⚙️  系统设置 - 配置应用程序参数";
            descLabel.Font = new Font("Microsoft YaHei UI", 11F);
            descLabel.AutoSize = false;
            descLabel.Size = new Size(500, 150);
            descLabel.Location = new Point(40, 120);
            descLabel.TextAlign = ContentAlignment.MiddleLeft;
            descLabel.ForeColor = Color.Gray;

            // 添加统计信息区域
            try
            {
                var stats = DatabaseHelper.GetStatistics();

                FlowLayoutPanel statsPanel = new FlowLayoutPanel();
                statsPanel.Location = new Point(40, 280);
                statsPanel.Size = new Size(800, 120);
                statsPanel.FlowDirection = FlowDirection.LeftToRight;
                statsPanel.WrapContents = false;
                statsPanel.AutoScroll = true;

                // 缓存统计卡片
                if (stats.ContainsKey("CacheCount"))
                {
                    Panel cacheCard = LayoutHelper.CreateCardPanel(
                        "翻译缓存",
                        $"总数: {stats["CacheCount"]}\n今日使用: {stats["TodayUsage"]}",
                        Color.FromArgb(52, 152, 219));
                    statsPanel.Controls.Add(cacheCard);
                }

                // 收藏统计卡片
                if (stats.ContainsKey("FavoriteCount"))
                {
                    Panel favoriteCard = LayoutHelper.CreateCardPanel(
                        "收藏夹",
                        $"收藏数量: {stats["FavoriteCount"]}",
                        Color.FromArgb(155, 89, 182));
                    statsPanel.Controls.Add(favoriteCard);
                }

                // 历史记录卡片
                if (stats.ContainsKey("HistoryCount"))
                {
                    Panel historyCard = LayoutHelper.CreateCardPanel(
                        "历史记录",
                        $"记录数量: {stats["HistoryCount"]}",
                        Color.FromArgb(46, 204, 113));
                    statsPanel.Controls.Add(historyCard);
                }

                welcomePanel.Controls.Add(statsPanel);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"加载统计信息失败: {ex.Message}");
            }

            welcomePanel.Controls.Add(welcomeLabel);
            welcomePanel.Controls.Add(descLabel);

            ShowControlInPanel(welcomePanel);
        }


        private void ShowControlInPanel(Control control)
        {
            // 清除当前内容
            contentPanel.Controls.Clear();

            // 将控件添加到内容面板
            contentPanel.Controls.Add(control);
            control.Dock = DockStyle.Fill;
            currentControl = control;

            // 确保控件正确显示
            contentPanel.Invalidate();
            contentPanel.Update();
        }

        private void HomeForm_Load(object sender, EventArgs e)
        {
            // 确保所有控件正确布局
            splitContainer.Refresh();
            treeView.Refresh();
            contentPanel.Refresh();

            // 默认选择第一个节点
            if (treeView.Nodes.Count > 0 && treeView.Nodes[0].Nodes.Count > 0)
            {
                treeView.SelectedNode = treeView.Nodes[0].Nodes[0]; // 选择翻译功能
                ShowTranslatorForm();
            }
        }



    }
}