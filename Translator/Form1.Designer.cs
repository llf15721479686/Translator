namespace Translator
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        private void InitializeComponent()
        {
            this.txtSource = new System.Windows.Forms.TextBox();
            this.btnTranslate = new System.Windows.Forms.Button();
            this.dgvTranslations = new System.Windows.Forms.DataGridView();
            this.btnCopyAll = new System.Windows.Forms.Button();
            this.btnGenerateMessages = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTranslations)).BeginInit();
            this.SuspendLayout();

            // txtSource
            this.txtSource.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSource.Font = new System.Drawing.Font("微软雅黑", 10F);
            this.txtSource.Location = new System.Drawing.Point(12, 30);
            this.txtSource.Multiline = true;
            this.txtSource.Name = "txtSource";
            this.txtSource.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtSource.Size = new System.Drawing.Size(1376, 120);
            this.txtSource.TabIndex = 0;

            // btnTranslate
            this.btnTranslate.Font = new System.Drawing.Font("微软雅黑", 10F);
            this.btnTranslate.Location = new System.Drawing.Point(12, 160);
            this.btnTranslate.Name = "btnTranslate";
            this.btnTranslate.Size = new System.Drawing.Size(140, 35);
            this.btnTranslate.TabIndex = 1;
            this.btnTranslate.Text = "一键翻译";
            this.btnTranslate.UseVisualStyleBackColor = true;
            this.btnTranslate.Click += new System.EventHandler(this.btnTranslate_Click);

            // btnCopyAll
            this.btnCopyAll.Enabled = false;
            this.btnCopyAll.Font = new System.Drawing.Font("微软雅黑", 10F);
            this.btnCopyAll.Location = new System.Drawing.Point(162, 160);
            this.btnCopyAll.Name = "btnCopyAll";
            this.btnCopyAll.Size = new System.Drawing.Size(140, 35);
            this.btnCopyAll.TabIndex = 2;
            this.btnCopyAll.Text = "一键复制";
            this.btnCopyAll.UseVisualStyleBackColor = true;
            this.btnCopyAll.Click += new System.EventHandler(this.btnCopyAll_Click);

            // btnGenerateMessages
            this.btnGenerateMessages.Font = new System.Drawing.Font("微软雅黑", 10F);
            this.btnGenerateMessages.Location = new System.Drawing.Point(312, 160);
            this.btnGenerateMessages.Name = "btnGenerateMessages";
            this.btnGenerateMessages.Size = new System.Drawing.Size(140, 35);
            this.btnGenerateMessages.TabIndex = 3;
            this.btnGenerateMessages.Text = "生成消息翻译";
            this.btnGenerateMessages.UseVisualStyleBackColor = true;
            this.btnGenerateMessages.Click += new System.EventHandler(this.btnGenerateMessages_Click);

            // dgvTranslations
            this.dgvTranslations.AllowUserToAddRows = false;
            this.dgvTranslations.AllowUserToDeleteRows = false;
            this.dgvTranslations.AllowUserToResizeRows = false;
            this.dgvTranslations.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvTranslations.BackgroundColor = System.Drawing.SystemColors.Control;
            this.dgvTranslations.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvTranslations.GridColor = System.Drawing.SystemColors.ControlDarkDark;
            this.dgvTranslations.Location = new System.Drawing.Point(12, 210);
            this.dgvTranslations.Name = "dgvTranslations";
            this.dgvTranslations.ReadOnly = true;
            this.dgvTranslations.RowHeadersWidth = 100;
            this.dgvTranslations.RowTemplate.Height = 60;
            this.dgvTranslations.Size = new System.Drawing.Size(1376, 479);
            this.dgvTranslations.TabIndex = 4;

            // Form1
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1400, 700);
            this.Controls.Add(this.btnGenerateMessages);
            this.Controls.Add(this.btnCopyAll);
            this.Controls.Add(this.dgvTranslations);
            this.Controls.Add(this.btnTranslate);
            this.Controls.Add(this.txtSource);
            this.Font = new System.Drawing.Font("宋体", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Form1";
            this.Text = "中文多语言翻译工具";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvTranslations)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.TextBox txtSource;
        private System.Windows.Forms.Button btnTranslate;
        private System.Windows.Forms.DataGridView dgvTranslations;
        private System.Windows.Forms.Button btnCopyAll;
        private System.Windows.Forms.Button btnGenerateMessages;
    }
}