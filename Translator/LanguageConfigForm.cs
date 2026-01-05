using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using static Translator.Form1;

namespace Translator
{
    public partial class LanguageConfigForm : Form
    {
        private FlowLayoutPanel flowPanel;
        private Button btnSave;
        private Button btnCancel;
        private Button btnSelectAll;
        private Button btnClearAll;
        
        private LanguageConfigManager configManager;
        private List<LanguageInfo> allLanguages;
        private Dictionary<string, CheckBox> checkBoxes = new Dictionary<string, CheckBox>();

        public LanguageConfigForm(LanguageConfigManager configManager, List<LanguageInfo> allLanguages)
        {
            this.configManager = configManager;
            this.allLanguages = allLanguages;
            InitializeComponent();
            LoadConfigToUI();
        }
        private void InitializeComponent()
        {
            this.Text = "请选择要翻译的语言列";
            this.Size = new Size(400, 500);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // 主容器
            flowPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoScroll = true,
                Padding = new Padding(10)
            };

            // 按钮面板
            var buttonPanel = new Panel
            {
                Height = 50,
                Dock = DockStyle.Bottom
            };

            // 全选按钮
            btnSelectAll = new Button
            {
                Text = "全选",
                Size = new Size(80, 30),
                Location = new Point(20, 10)
            };
            btnSelectAll.Click += (s, e) => SetAllCheckBoxes(true);

            // 全不选按钮
            btnClearAll = new Button
            {
                Text = "全不选",
                Size = new Size(80, 30),
                Location = new Point(110, 10)
            };
            btnClearAll.Click += (s, e) => SetAllCheckBoxes(false);

            // 保存按钮
            btnSave = new Button
            {
                Text = "保存",
                Size = new Size(80, 30),
                Location = new Point(200, 10)
            };
            btnSave.Click += btnSave_Click;

            // 取消按钮
            btnCancel = new Button
            {
                Text = "取消",
                Size = new Size(80, 30),
                Location = new Point(290, 10)
            };
            btnCancel.Click += (s, e) => this.DialogResult = DialogResult.Cancel;

            buttonPanel.Controls.AddRange(new Control[] { btnSelectAll, btnClearAll, btnSave, btnCancel });


            this.Controls.AddRange(new Control[] {  flowPanel, buttonPanel });
        }


        private void LoadConfigToUI()
        {
            var config = configManager.GetLanguageConfig();

            foreach (var language in allLanguages)
            {
                var checkbox = new CheckBox
                {
                    Text = $"{language.Name} [{language.Code}]",
                    Checked = config.ContainsKey(language.Name) ? config[language.Name] : false,
                    AutoSize = true,
                    Margin = new Padding(3, 8, 3, 8),
                    Font = new Font("微软雅黑", 10)
                };

                flowPanel.Controls.Add(checkbox);
                checkBoxes[language.Name] = checkbox;
            }
        }

        private void SetAllCheckBoxes(bool isChecked)
        {
            foreach (var checkbox in checkBoxes.Values)
            {
                checkbox.Checked = isChecked;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            var newConfig = new Dictionary<string, bool>();
            
            foreach (var kvp in checkBoxes)
            {
                newConfig[kvp.Key] = kvp.Value.Checked;
            }

            // 更新配置
            configManager.UpdateAllConfig(newConfig);
            
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}