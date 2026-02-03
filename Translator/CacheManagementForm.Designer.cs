namespace Translator
{
    partial class CacheManagementForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.DataGridView dgvCache;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Button btnClearOld;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.Label lblSearch;
        private System.Windows.Forms.Label lblStatistics;
        private System.Windows.Forms.Label lblTotal;
        private System.Windows.Forms.Button btnDeleteAll;

        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Panel panelBottom;
        private System.Windows.Forms.DataGridView dgvCellContextMenu;
        private System.Windows.Forms.Panel panelContent;
        // 新增FlowLayoutPanel用于布局搜索相关控件
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanelSearch;
        // 新增按钮
        private System.Windows.Forms.Button btnDeleteSelected;
        private System.Windows.Forms.Label lblPageInfo;
        private System.Windows.Forms.Button btnFirst;
        private System.Windows.Forms.Button btnPrev;
        private System.Windows.Forms.Button btnNext;
        private System.Windows.Forms.Button btnLast;
        private System.Windows.Forms.ComboBox cboPageSize;
        private System.Windows.Forms.Label label1;
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();  // 添加这行
            this.dgvCache = new System.Windows.Forms.DataGridView();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.btnSearch = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.btnClearOld = new System.Windows.Forms.Button();
            this.btnExport = new System.Windows.Forms.Button();
            this.lblSearch = new System.Windows.Forms.Label();
            this.lblStatistics = new System.Windows.Forms.Label();
            this.lblTotal = new System.Windows.Forms.Label();
            this.btnDeleteAll = new System.Windows.Forms.Button();
            this.btnDeleteSelected = new System.Windows.Forms.Button();  // 添加这行初始化
            this.panelTop = new System.Windows.Forms.Panel();
            this.panelBottom = new System.Windows.Forms.Panel();

            this.flowLayoutPanelSearch = new System.Windows.Forms.FlowLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCache)).BeginInit();
            this.panelTop.SuspendLayout();
            this.panelBottom.SuspendLayout();
            this.flowLayoutPanelSearch.SuspendLayout();
            this.SuspendLayout();

            // 
            // dgvCache
            // 
            this.dgvCache.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvCache.Location = new System.Drawing.Point(12, 70);
            this.dgvCache.Name = "dgvCache";
            this.dgvCache.Size = new System.Drawing.Size(960, 450);
            this.dgvCache.TabIndex = 0;
            // 
            // flowLayoutPanelSearch
            // 
            this.flowLayoutPanelSearch.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top |
            System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanelSearch.Controls.Add(this.lblSearch);
            this.flowLayoutPanelSearch.Controls.Add(this.txtSearch);
            this.flowLayoutPanelSearch.Controls.Add(this.btnSearch);
            this.flowLayoutPanelSearch.Controls.Add(this.btnRefresh);
            this.flowLayoutPanelSearch.Controls.Add(this.btnClearOld);
            this.flowLayoutPanelSearch.Controls.Add(this.btnExport);
            this.flowLayoutPanelSearch.Controls.Add(this.btnDeleteSelected);  // 先添加删除选中按钮
            this.flowLayoutPanelSearch.Controls.Add(this.btnDeleteAll);       // 再添加清空所有按钮
            this.flowLayoutPanelSearch.Controls.Add(this.btnDeleteAll);
            this.flowLayoutPanelSearch.Location = new System.Drawing.Point(12, 10);
            this.flowLayoutPanelSearch.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutPanelSearch.Name = "flowLayoutPanelSearch";
            this.flowLayoutPanelSearch.Size = new System.Drawing.Size(960, 35);
            this.flowLayoutPanelSearch.TabIndex = 13;
            this.flowLayoutPanelSearch.WrapContents = false;
            // 
            // txtSearch
            // 
            this.txtSearch.Font = new System.Drawing.Font("Microsoft YaHei UI", 10F);
            this.txtSearch.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(200, 24);
            this.txtSearch.TabIndex = 1;
            this.txtSearch.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtSearch_KeyPress);
            // 
            // btnSearch
            // 
            this.btnSearch.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            this.btnSearch.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.btnSearch.Location = new System.Drawing.Point(270, 5);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(80, 25);
            this.btnSearch.TabIndex = 2;
            this.btnSearch.Text = "搜索";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // btnRefresh
            // 
            this.btnRefresh.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            this.btnRefresh.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.btnRefresh.Location = new System.Drawing.Point(360, 5);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(80, 25);
            this.btnRefresh.TabIndex = 3;
            this.btnRefresh.Text = "刷新";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // btnClearOld
            // 
            this.btnClearOld.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            this.btnClearOld.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.btnClearOld.Location = new System.Drawing.Point(450, 5);
            this.btnClearOld.Name = "btnClearOld";
            this.btnClearOld.Size = new System.Drawing.Size(100, 25);
            this.btnClearOld.TabIndex = 4;
            this.btnClearOld.Text = "清理旧数据";
            this.btnClearOld.UseVisualStyleBackColor = true;
            this.btnClearOld.Click += new System.EventHandler(this.btnClearOld_Click);
            // 
            // btnExport
            // 
            this.btnExport.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            this.btnExport.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.btnExport.Location = new System.Drawing.Point(560, 5);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(80, 25);
            this.btnExport.TabIndex = 5;
            this.btnExport.Text = "导出";
            this.btnExport.UseVisualStyleBackColor = true;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // lblSearch
            // 
            this.lblSearch.AutoSize = true;
            this.lblSearch.Font = new System.Drawing.Font("Microsoft YaHei UI", 10F);
            this.lblSearch.Location = new System.Drawing.Point(5, 8);
            this.lblSearch.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.lblSearch.Name = "lblSearch";
            this.lblSearch.Size = new System.Drawing.Size(65, 20);
            this.lblSearch.TabIndex = 6;
            this.lblSearch.Text = "搜索：";
            // lblStatistics
            this.lblStatistics.AutoSize = true;
            this.lblStatistics.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblStatistics.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(153)))), ((int)(((byte)(255)))));
            this.lblStatistics.Location = new System.Drawing.Point(17, 44); // Y从48降到40，向上挪8px
            this.lblStatistics.Name = "lblStatistics";
            this.lblStatistics.Size = new System.Drawing.Size(44, 17);
            this.lblStatistics.TabIndex = 7;
            this.lblStatistics.Text = "统计：";

            // lblTotal
            this.lblTotal.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblTotal.AutoSize = true;
            this.lblTotal.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            this.lblTotal.ForeColor = System.Drawing.Color.Gray;
            this.lblTotal.Location = new System.Drawing.Point(780, 44); // Y从48降到40，同步向上挪
            this.lblTotal.Name = "lblTotal";
            this.lblTotal.Size = new System.Drawing.Size(56, 17);
            this.lblTotal.TabIndex = 8;
            this.lblTotal.Text = "总计：0";
            // 
            // btnDeleteAll
            // 
            this.btnDeleteAll.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            this.btnDeleteAll.ForeColor = System.Drawing.Color.Red;
            this.btnDeleteAll.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.btnDeleteAll.Location = new System.Drawing.Point(760, 5);  // 位置从650改为760
            this.btnDeleteAll.Name = "btnDeleteAll";
            this.btnDeleteAll.Size = new System.Drawing.Size(100, 25);
            this.btnDeleteAll.TabIndex = 11;
            this.btnDeleteAll.Text = "清空所有";
            this.btnDeleteAll.UseVisualStyleBackColor = true;
            this.btnDeleteAll.Click += new System.EventHandler(this.btnDeleteAll_Click);

            // 
            // btnDeleteSelected
            // 
            this.btnDeleteSelected.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            this.btnDeleteSelected.ForeColor = System.Drawing.Color.OrangeRed;
            this.btnDeleteSelected.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.btnDeleteSelected.Location = new System.Drawing.Point(650, 5);
            this.btnDeleteSelected.Name = "btnDeleteSelected";
            this.btnDeleteSelected.Size = new System.Drawing.Size(100, 25);
            this.btnDeleteSelected.TabIndex = 10;
            this.btnDeleteSelected.Text = "删除选中";
            this.btnDeleteSelected.UseVisualStyleBackColor = true;
            this.btnDeleteSelected.Click += new System.EventHandler(this.btnDeleteSelected_Click);

            // 
            // panelTop
            // 
            this.panelTop.Controls.Add(this.flowLayoutPanelSearch);
            this.panelTop.Controls.Add(this.lblStatistics);
            this.panelTop.Controls.Add(this.lblTotal);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(0, 0);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(984, 70);
            this.panelTop.TabIndex = 11;
            // 
            // panelBottom
            // 

            this.panelBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelBottom.Location = new System.Drawing.Point(0, 530);
            this.panelBottom.Name = "panelBottom";
            this.panelBottom.Size = new System.Drawing.Size(984, 50);
            this.panelBottom.TabIndex = 12;
            // 
            // CacheManagementForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;

            this.ClientSize = new System.Drawing.Size(984, 580);
            this.Controls.Add(this.dgvCache);
            this.Controls.Add(this.panelTop);
            this.Controls.Add(this.panelBottom);
            this.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            this.MinimumSize = new System.Drawing.Size(1000, 600);
            this.Name = "CacheManagementForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "翻译缓存管理";
            this.Load += new System.EventHandler(this.CacheManagementForm_Load);
            this.Resize += new System.EventHandler(this.CacheManagementForm_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.dgvCache)).EndInit();
            this.flowLayoutPanelSearch.ResumeLayout(false);
            this.flowLayoutPanelSearch.PerformLayout();
            this.panelTop.ResumeLayout(false);
            this.panelTop.PerformLayout();
            this.panelBottom.ResumeLayout(false);
            this.ResumeLayout(false);

            // 调整控件锚定属性
            // CacheManagementForm.Designer.cs中已有的正确设置
            // 在 InitializeComponent 方法中找到 dgvCache 的配置，替换为以下内容
            // dgvCache
            this.dgvCache.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvCache.Location = new System.Drawing.Point(12, 70);
            this.dgvCache.Name = "dgvCache";
            // 移除固定 Size，改用 Dock 或纯锚定（关键：不要同时设置 Size 和 Dock）
            this.dgvCache.Size = new System.Drawing.Size(0, 0); // 清空固定尺寸
            this.dgvCache.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvCache.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.None;
            this.dgvCache.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvCache.Dock = System.Windows.Forms.DockStyle.Fill; // 关键：填充父容器
            this.dgvCache.Margin = new System.Windows.Forms.Padding(12, 0, 12, 12);
            this.dgvCache.TabIndex = 0;
            // 核心设置：关闭最后一行的空行（新行添加行）
            this.dgvCache.AllowUserToAddRows = false;

            this.panelTop.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));

            this.panelBottom.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      

            // 在 InitializeComponent 中添加：
            this.panelContent = new System.Windows.Forms.Panel();
            this.panelContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelContent.Margin = new System.Windows.Forms.Padding(0);
            this.panelContent.Controls.Add(this.dgvCache); // 表格填充中间面板
            this.Controls.Add(this.panelContent);
            this.Controls.Add(this.panelTop);
            this.Controls.Add(this.panelBottom);


            // 分页控件
            this.lblPageInfo = new System.Windows.Forms.Label();
            this.btnFirst = new System.Windows.Forms.Button();
            this.btnPrev = new System.Windows.Forms.Button();
            this.btnNext = new System.Windows.Forms.Button();
            this.btnLast = new System.Windows.Forms.Button();
            this.cboPageSize = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();

            // 在panelBottom中移除btnClose并添加分页控件


            // lblPageInfo
            this.lblPageInfo.AutoSize = true;
            this.lblPageInfo.Location = new System.Drawing.Point(350, 15);
            this.lblPageInfo.Name = "lblPageInfo";
            this.lblPageInfo.Size = new System.Drawing.Size(100, 17);
            this.lblPageInfo.TabIndex = 13;
            this.lblPageInfo.Text = "第 1 页 / 共 0 页";

            // btnFirst
            this.btnFirst.Location = new System.Drawing.Point(12, 10);
            this.btnFirst.Name = "btnFirst";
            this.btnFirst.Size = new System.Drawing.Size(60, 30);
            this.btnFirst.TabIndex = 14;
            this.btnFirst.Text = "首页";
            this.btnFirst.UseVisualStyleBackColor = true;
            this.btnFirst.Click += new System.EventHandler(this.btnFirst_Click);

            // btnPrev
            this.btnPrev.Location = new System.Drawing.Point(78, 10);
            this.btnPrev.Name = "btnPrev";
            this.btnPrev.Size = new System.Drawing.Size(60, 30);
            this.btnPrev.TabIndex = 15;
            this.btnPrev.Text = "上一页";
            this.btnPrev.UseVisualStyleBackColor = true;
            this.btnPrev.Click += new System.EventHandler(this.btnPrev_Click);

            // btnNext
            this.btnNext.Location = new System.Drawing.Point(144, 10);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(60, 30);
            this.btnNext.TabIndex = 16;
            this.btnNext.Text = "下一页";
            this.btnNext.UseVisualStyleBackColor = true;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);

            // btnLast
            this.btnLast.Location = new System.Drawing.Point(210, 10);
            this.btnLast.Name = "btnLast";
            this.btnLast.Size = new System.Drawing.Size(60, 30);
            this.btnLast.TabIndex = 17;
            this.btnLast.Text = "末页";
            this.btnLast.UseVisualStyleBackColor = true;
            this.btnLast.Click += new System.EventHandler(this.btnLast_Click);

            // label1
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(280, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 17);
            this.label1.TabIndex = 18;
            this.label1.Text = "每页行数:";

            // cboPageSize
            this.cboPageSize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboPageSize.Items.AddRange(new object[] { "1000", "3000", "5000", "10000", "50000" });
            this.cboPageSize.Location = new System.Drawing.Point(342, 12);
            this.cboPageSize.Name = "cboPageSize";
            this.cboPageSize.Size = new System.Drawing.Size(60, 25);
            this.cboPageSize.TabIndex = 19;
            this.cboPageSize.SelectedIndex = 0; // 默认1000行
            this.cboPageSize.SelectedIndexChanged += new System.EventHandler(this.cboPageSize_SelectedIndexChanged);

            // 将分页控件添加到panelBottom
            this.panelBottom.Controls.Add(this.btnFirst);
            this.panelBottom.Controls.Add(this.btnPrev);
            this.panelBottom.Controls.Add(this.btnNext);
            this.panelBottom.Controls.Add(this.btnLast);
            this.panelBottom.Controls.Add(this.label1);
            this.panelBottom.Controls.Add(this.cboPageSize);
            this.panelBottom.Controls.Add(this.lblPageInfo);

        }
    }
}