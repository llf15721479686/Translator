namespace Translator
{
    partial class SettingsForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.TabControl tabControlSettings;
        private System.Windows.Forms.TabPage tabPageDatabase;
        private System.Windows.Forms.TabPage tabPageGeneral;
        private System.Windows.Forms.GroupBox groupBoxBackup;
        private System.Windows.Forms.Button btnBackup;
        private System.Windows.Forms.TextBox txtBackupPath;
        private System.Windows.Forms.Label lblBackupPath;
        private System.Windows.Forms.GroupBox groupBoxRestore;
        private System.Windows.Forms.Button btnRestore;
        private System.Windows.Forms.TextBox txtRestorePath;
        private System.Windows.Forms.Label lblRestorePath;
        private System.Windows.Forms.GroupBox groupBoxSqlite;
        private System.Windows.Forms.Button btnSyncToSqlite;
        private System.Windows.Forms.Label lblSqliteStatus;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Button btnSelectBackup;
        private System.Windows.Forms.Button btnSelectRestore;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.GroupBox groupBoxSystem;
        private System.Windows.Forms.CheckBox cbAutoBackup;
        private System.Windows.Forms.CheckBox cbAutoUpdate;
        private System.Windows.Forms.Button btnSaveSettings;
        private System.Windows.Forms.PictureBox pictureBoxLogo;
        private System.Windows.Forms.Label lblVersion;
        private System.Windows.Forms.Button btnTestConnection;

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsForm));
            this.tabControlSettings = new System.Windows.Forms.TabControl();
            this.tabPageDatabase = new System.Windows.Forms.TabPage();
            this.btnTestConnection = new System.Windows.Forms.Button();
            this.groupBoxSqlite = new System.Windows.Forms.GroupBox();
            this.lblSqliteStatus = new System.Windows.Forms.Label();
            this.btnSyncToSqlite = new System.Windows.Forms.Button();
            this.groupBoxRestore = new System.Windows.Forms.GroupBox();
            this.btnSelectRestore = new System.Windows.Forms.Button();
            this.txtRestorePath = new System.Windows.Forms.TextBox();
            this.lblRestorePath = new System.Windows.Forms.Label();
            this.btnRestore = new System.Windows.Forms.Button();
            this.groupBoxBackup = new System.Windows.Forms.GroupBox();
            this.btnSelectBackup = new System.Windows.Forms.Button();
            this.txtBackupPath = new System.Windows.Forms.TextBox();
            this.lblBackupPath = new System.Windows.Forms.Label();
            this.btnBackup = new System.Windows.Forms.Button();
            this.tabPageGeneral = new System.Windows.Forms.TabPage();
            this.groupBoxSystem = new System.Windows.Forms.GroupBox();
            this.btnSaveSettings = new System.Windows.Forms.Button();
            this.cbAutoUpdate = new System.Windows.Forms.CheckBox();
            this.cbAutoBackup = new System.Windows.Forms.CheckBox();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.lblStatus = new System.Windows.Forms.Label();
            this.pictureBoxLogo = new System.Windows.Forms.PictureBox();
            this.lblVersion = new System.Windows.Forms.Label();
            this.tabControlSettings.SuspendLayout();
            this.tabPageDatabase.SuspendLayout();
            this.groupBoxSqlite.SuspendLayout();
            this.groupBoxRestore.SuspendLayout();
            this.groupBoxBackup.SuspendLayout();
            this.tabPageGeneral.SuspendLayout();
            this.groupBoxSystem.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLogo)).BeginInit();
            this.SuspendLayout();

            // 
            // tabControlSettings
            // 
            this.tabControlSettings.Controls.Add(this.tabPageDatabase);
            this.tabControlSettings.Controls.Add(this.tabPageGeneral);
            this.tabControlSettings.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlSettings.Font = new System.Drawing.Font("Microsoft YaHei UI", 10F);
            this.tabControlSettings.ItemSize = new System.Drawing.Size(120, 40);
            this.tabControlSettings.Location = new System.Drawing.Point(0, 0);
            this.tabControlSettings.Name = "tabControlSettings";
            this.tabControlSettings.SelectedIndex = 0;
            this.tabControlSettings.Size = new System.Drawing.Size(1000, 700);
            this.tabControlSettings.TabIndex = 0;
            // 
            // tabPageDatabase
            // 
            this.tabPageDatabase.BackColor = System.Drawing.Color.White;
            this.tabPageDatabase.Controls.Add(this.btnTestConnection);
            this.tabPageDatabase.Controls.Add(this.groupBoxSqlite);
            this.tabPageDatabase.Controls.Add(this.groupBoxRestore);
            this.tabPageDatabase.Controls.Add(this.groupBoxBackup);
            this.tabPageDatabase.Controls.Add(this.lblVersion);
            this.tabPageDatabase.Controls.Add(this.pictureBoxLogo);
            this.tabPageDatabase.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            this.tabPageDatabase.Location = new System.Drawing.Point(4, 44);
            this.tabPageDatabase.Name = "tabPageDatabase";
            this.tabPageDatabase.Padding = new System.Windows.Forms.Padding(30);
            this.tabPageDatabase.Size = new System.Drawing.Size(992, 652);
            this.tabPageDatabase.TabIndex = 0;
            this.tabPageDatabase.Text = "数据库管理";
            // 
            // btnTestConnection
            // 
            this.btnTestConnection.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(152)))), ((int)(((byte)(219)))));
            this.btnTestConnection.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTestConnection.Font = new System.Drawing.Font("Microsoft YaHei UI", 10F);
            this.btnTestConnection.ForeColor = System.Drawing.Color.White;
            this.btnTestConnection.Location = new System.Drawing.Point(790, 30);
            this.btnTestConnection.Name = "btnTestConnection";
            this.btnTestConnection.Size = new System.Drawing.Size(172, 35);
            this.btnTestConnection.TabIndex = 4;
            this.btnTestConnection.Text = "测试数据库连接";
            this.btnTestConnection.UseVisualStyleBackColor = false;
            this.btnTestConnection.Click += new System.EventHandler(this.btnTestConnection_Click);
            // 
            // groupBoxSqlite
            // 
            this.groupBoxSqlite.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxSqlite.Controls.Add(this.lblSqliteStatus);
            this.groupBoxSqlite.Controls.Add(this.btnSyncToSqlite);
            this.groupBoxSqlite.Font = new System.Drawing.Font("Microsoft YaHei UI", 11F, System.Drawing.FontStyle.Bold);
            this.groupBoxSqlite.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(204)))), ((int)(((byte)(113)))));
            this.groupBoxSqlite.Location = new System.Drawing.Point(30, 400);
            this.groupBoxSqlite.Name = "groupBoxSqlite";
            this.groupBoxSqlite.Size = new System.Drawing.Size(932, 160);
            this.groupBoxSqlite.TabIndex = 3;
            this.groupBoxSqlite.TabStop = false;
            this.groupBoxSqlite.Text = "SQLite 数据库同步";
            // 
            // lblSqliteStatus
            // 
            this.lblSqliteStatus.AutoSize = true;
            this.lblSqliteStatus.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            this.lblSqliteStatus.ForeColor = System.Drawing.Color.Gray;
            this.lblSqliteStatus.Location = new System.Drawing.Point(30, 70);
            this.lblSqliteStatus.Name = "lblSqliteStatus";
            this.lblSqliteStatus.Size = new System.Drawing.Size(200, 17);
            this.lblSqliteStatus.TabIndex = 1;
            this.lblSqliteStatus.Text = "SQLite状态：检查本地数据库...";
            // 
            // btnSyncToSqlite
            // 
            this.btnSyncToSqlite.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(204)))), ((int)(((byte)(113)))));
            this.btnSyncToSqlite.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSyncToSqlite.Font = new System.Drawing.Font("Microsoft YaHei UI", 11F);
            this.btnSyncToSqlite.ForeColor = System.Drawing.Color.White;
            this.btnSyncToSqlite.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSyncToSqlite.Location = new System.Drawing.Point(30, 100);
            this.btnSyncToSqlite.Name = "btnSyncToSqlite";
            this.btnSyncToSqlite.Size = new System.Drawing.Size(250, 45);
            this.btnSyncToSqlite.TabIndex = 0;
            this.btnSyncToSqlite.Text = "同步到 SQLite 数据库";
            this.btnSyncToSqlite.UseVisualStyleBackColor = false;
            this.btnSyncToSqlite.Click += new System.EventHandler(this.btnSyncToSqlite_Click);
            // 
            // groupBoxRestore
            // 
            this.groupBoxRestore.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxRestore.Controls.Add(this.btnSelectRestore);
            this.groupBoxRestore.Controls.Add(this.txtRestorePath);
            this.groupBoxRestore.Controls.Add(this.lblRestorePath);
            this.groupBoxRestore.Controls.Add(this.btnRestore);
            this.groupBoxRestore.Font = new System.Drawing.Font("Microsoft YaHei UI", 11F, System.Drawing.FontStyle.Bold);
            this.groupBoxRestore.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(76)))), ((int)(((byte)(60)))));
            this.groupBoxRestore.Location = new System.Drawing.Point(30, 220);
            this.groupBoxRestore.Name = "groupBoxRestore";
            this.groupBoxRestore.Size = new System.Drawing.Size(932, 160);
            this.groupBoxRestore.TabIndex = 2;
            this.groupBoxRestore.TabStop = false;
            this.groupBoxRestore.Text = "数据库还原";
            // 
            // btnSelectRestore
            // 
            this.btnSelectRestore.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(196)))), ((int)(((byte)(15)))));
            this.btnSelectRestore.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSelectRestore.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            this.btnSelectRestore.ForeColor = System.Drawing.Color.White;
            this.btnSelectRestore.Location = new System.Drawing.Point(780, 60);
            this.btnSelectRestore.Name = "btnSelectRestore";
            this.btnSelectRestore.Size = new System.Drawing.Size(120, 30);
            this.btnSelectRestore.TabIndex = 3;
            this.btnSelectRestore.Text = "选择文件";
            this.btnSelectRestore.UseVisualStyleBackColor = false;
            this.btnSelectRestore.Click += new System.EventHandler(this.btnSelectRestore_Click);
            // 
            // txtRestorePath
            // 
            this.txtRestorePath.Font = new System.Drawing.Font("Microsoft YaHei UI", 10F);
            this.txtRestorePath.Location = new System.Drawing.Point(30, 60);
            this.txtRestorePath.Name = "txtRestorePath";
            this.txtRestorePath.ReadOnly = true;
            this.txtRestorePath.Size = new System.Drawing.Size(730, 25);
            this.txtRestorePath.TabIndex = 2;
            this.txtRestorePath.Text = "请选择备份文件 (.bak)";
            // 
            // lblRestorePath
            // 
            this.lblRestorePath.AutoSize = true;
            this.lblRestorePath.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            this.lblRestorePath.ForeColor = System.Drawing.Color.Gray;
            this.lblRestorePath.Location = new System.Drawing.Point(30, 40);
            this.lblRestorePath.Name = "lblRestorePath";
            this.lblRestorePath.Size = new System.Drawing.Size(176, 17);
            this.lblRestorePath.TabIndex = 1;
            this.lblRestorePath.Text = "备份文件路径 (.bak 格式):";
            // 
            // btnRestore
            // 
            this.btnRestore.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(76)))), ((int)(((byte)(60)))));
            this.btnRestore.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRestore.Font = new System.Drawing.Font("Microsoft YaHei UI", 11F);
            this.btnRestore.ForeColor = System.Drawing.Color.White;
            this.btnRestore.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnRestore.Location = new System.Drawing.Point(30, 100);
            this.btnRestore.Name = "btnRestore";
            this.btnRestore.Size = new System.Drawing.Size(250, 45);
            this.btnRestore.TabIndex = 0;
            this.btnRestore.Text = "还原数据库";
            this.btnRestore.UseVisualStyleBackColor = false;
            this.btnRestore.Click += new System.EventHandler(this.btnRestore_Click);
            // 
            // groupBoxBackup
            // 
            this.groupBoxBackup.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxBackup.Controls.Add(this.btnSelectBackup);
            this.groupBoxBackup.Controls.Add(this.txtBackupPath);
            this.groupBoxBackup.Controls.Add(this.lblBackupPath);
            this.groupBoxBackup.Controls.Add(this.btnBackup);
            this.groupBoxBackup.Font = new System.Drawing.Font("Microsoft YaHei UI", 11F, System.Drawing.FontStyle.Bold);
            this.groupBoxBackup.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(152)))), ((int)(((byte)(219)))));
            this.groupBoxBackup.Location = new System.Drawing.Point(30, 40);
            this.groupBoxBackup.Name = "groupBoxBackup";
            this.groupBoxBackup.Size = new System.Drawing.Size(932, 160);
            this.groupBoxBackup.TabIndex = 1;
            this.groupBoxBackup.TabStop = false;
            this.groupBoxBackup.Text = "数据库备份";
            // 
            // btnSelectBackup
            // 
            this.btnSelectBackup.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(155)))), ((int)(((byte)(89)))), ((int)(((byte)(182)))));
            this.btnSelectBackup.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSelectBackup.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            this.btnSelectBackup.ForeColor = System.Drawing.Color.White;
            this.btnSelectBackup.Location = new System.Drawing.Point(780, 60);
            this.btnSelectBackup.Name = "btnSelectBackup";
            this.btnSelectBackup.Size = new System.Drawing.Size(120, 30);
            this.btnSelectBackup.TabIndex = 3;
            this.btnSelectBackup.Text = "选择路径";
            this.btnSelectBackup.UseVisualStyleBackColor = false;
            this.btnSelectBackup.Click += new System.EventHandler(this.btnSelectBackup_Click);
            // 
            // txtBackupPath
            // 
            this.txtBackupPath.Font = new System.Drawing.Font("Microsoft YaHei UI", 10F);
            this.txtBackupPath.Location = new System.Drawing.Point(30, 60);
            this.txtBackupPath.Name = "txtBackupPath";
            this.txtBackupPath.ReadOnly = true;
            this.txtBackupPath.Size = new System.Drawing.Size(730, 25);
            this.txtBackupPath.TabIndex = 2;
            this.txtBackupPath.Text = "请选择备份保存路径";
            // 
            // lblBackupPath
            // 
            this.lblBackupPath.AutoSize = true;
            this.lblBackupPath.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            this.lblBackupPath.ForeColor = System.Drawing.Color.Gray;
            this.lblBackupPath.Location = new System.Drawing.Point(30, 40);
            this.lblBackupPath.Name = "lblBackupPath";
            this.lblBackupPath.Size = new System.Drawing.Size(92, 17);
            this.lblBackupPath.TabIndex = 1;
            this.lblBackupPath.Text = "备份保存路径:";
            // 
            // btnBackup
            // 
            this.btnBackup.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(152)))), ((int)(((byte)(219)))));
            this.btnBackup.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBackup.Font = new System.Drawing.Font("Microsoft YaHei UI", 11F);
            this.btnBackup.ForeColor = System.Drawing.Color.White;
            this.btnBackup.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnBackup.Location = new System.Drawing.Point(30, 100);
            this.btnBackup.Name = "btnBackup";
            this.btnBackup.Size = new System.Drawing.Size(250, 45);
            this.btnBackup.TabIndex = 0;
            this.btnBackup.Text = "备份数据库";
            this.btnBackup.UseVisualStyleBackColor = false;
            this.btnBackup.Click += new System.EventHandler(this.btnBackup_Click);
            // 
            // pictureBoxLogo
            // 
            this.pictureBoxLogo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBoxLogo.BackColor = System.Drawing.Color.Transparent;
            this.pictureBoxLogo.Location = new System.Drawing.Point(820, 590);
            this.pictureBoxLogo.Name = "pictureBoxLogo";
            this.pictureBoxLogo.Size = new System.Drawing.Size(40, 40);
            this.pictureBoxLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxLogo.TabIndex = 4;
            this.pictureBoxLogo.TabStop = false;
            // 
            // lblVersion
            // 
            this.lblVersion.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblVersion.AutoSize = true;
            this.lblVersion.BackColor = System.Drawing.Color.Transparent;
            this.lblVersion.Font = new System.Drawing.Font("Microsoft YaHei UI", 8F);
            this.lblVersion.ForeColor = System.Drawing.Color.Gray;
            this.lblVersion.Location = new System.Drawing.Point(870, 605);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Size = new System.Drawing.Size(60, 16);
            this.lblVersion.TabIndex = 5;
            this.lblVersion.Text = "版本 1.0.0";
            // 
            // tabPageGeneral
            // 
            this.tabPageGeneral.BackColor = System.Drawing.Color.White;
            this.tabPageGeneral.Controls.Add(this.groupBoxSystem);
            this.tabPageGeneral.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            this.tabPageGeneral.Location = new System.Drawing.Point(4, 44);
            this.tabPageGeneral.Name = "tabPageGeneral";
            this.tabPageGeneral.Padding = new System.Windows.Forms.Padding(30);
            this.tabPageGeneral.Size = new System.Drawing.Size(992, 652);
            this.tabPageGeneral.TabIndex = 1;
            this.tabPageGeneral.Text = "常规设置";
            // 
            // groupBoxSystem
            // 
            this.groupBoxSystem.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxSystem.Controls.Add(this.btnSaveSettings);
            this.groupBoxSystem.Controls.Add(this.cbAutoUpdate);
            this.groupBoxSystem.Controls.Add(this.cbAutoBackup);
            this.groupBoxSystem.Font = new System.Drawing.Font("Microsoft YaHei UI", 11F, System.Drawing.FontStyle.Bold);
            this.groupBoxSystem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(155)))), ((int)(((byte)(89)))), ((int)(((byte)(182)))));
            this.groupBoxSystem.Location = new System.Drawing.Point(30, 40);
            this.groupBoxSystem.Name = "groupBoxSystem";
            this.groupBoxSystem.Size = new System.Drawing.Size(932, 200);
            this.groupBoxSystem.TabIndex = 0;
            this.groupBoxSystem.TabStop = false;
            this.groupBoxSystem.Text = "系统设置";
            // 
            // btnSaveSettings
            // 
            this.btnSaveSettings.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(155)))), ((int)(((byte)(89)))), ((int)(((byte)(182)))));
            this.btnSaveSettings.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSaveSettings.Font = new System.Drawing.Font("Microsoft YaHei UI", 11F);
            this.btnSaveSettings.ForeColor = System.Drawing.Color.White;
            this.btnSaveSettings.Location = new System.Drawing.Point(30, 130);
            this.btnSaveSettings.Name = "btnSaveSettings";
            this.btnSaveSettings.Size = new System.Drawing.Size(200, 45);
            this.btnSaveSettings.TabIndex = 2;
            this.btnSaveSettings.Text = "保存设置";
            this.btnSaveSettings.UseVisualStyleBackColor = false;
            this.btnSaveSettings.Click += new System.EventHandler(this.btnSaveSettings_Click);
            // 
            // cbAutoUpdate
            // 
            this.cbAutoUpdate.AutoSize = true;
            this.cbAutoUpdate.Font = new System.Drawing.Font("Microsoft YaHei UI", 10F);
            this.cbAutoUpdate.ForeColor = System.Drawing.Color.Gray;
            this.cbAutoUpdate.Location = new System.Drawing.Point(30, 90);
            this.cbAutoUpdate.Name = "cbAutoUpdate";
            this.cbAutoUpdate.Size = new System.Drawing.Size(151, 23);
            this.cbAutoUpdate.TabIndex = 1;
            this.cbAutoUpdate.Text = "自动检查更新 (启用)";
            this.cbAutoUpdate.UseVisualStyleBackColor = true;
            // 
            // cbAutoBackup
            // 
            this.cbAutoBackup.AutoSize = true;
            this.cbAutoBackup.Font = new System.Drawing.Font("Microsoft YaHei UI", 10F);
            this.cbAutoBackup.ForeColor = System.Drawing.Color.Gray;
            this.cbAutoBackup.Location = new System.Drawing.Point(30, 50);
            this.cbAutoBackup.Name = "cbAutoBackup";
            this.cbAutoBackup.Size = new System.Drawing.Size(231, 23);
            this.cbAutoBackup.TabIndex = 0;
            this.cbAutoBackup.Text = "每周自动备份数据库 (启用)";
            this.cbAutoBackup.UseVisualStyleBackColor = true;
            // 
            // progressBar
            // 
            this.progressBar.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.progressBar.Location = new System.Drawing.Point(0, 700);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(1000, 10);
            this.progressBar.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar.TabIndex = 2;
            this.progressBar.Visible = false;
            // 
            // lblStatus
            // 
            this.lblStatus.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.lblStatus.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lblStatus.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            this.lblStatus.ForeColor = System.Drawing.Color.Gray;
            this.lblStatus.Location = new System.Drawing.Point(0, 710);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(1000, 30);
            this.lblStatus.TabIndex = 3;
            this.lblStatus.Text = "就绪";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1000, 740);
            this.Controls.Add(this.tabControlSettings);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.progressBar);
            this.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            this.MinimumSize = new System.Drawing.Size(1016, 779);
            this.Name = "SettingsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "系统设置";
            this.Load += new System.EventHandler(this.SettingsForm_Load);
            this.tabControlSettings.ResumeLayout(false);
            this.tabPageDatabase.ResumeLayout(false);
            this.tabPageDatabase.PerformLayout();
            this.groupBoxSqlite.ResumeLayout(false);
            this.groupBoxSqlite.PerformLayout();
            this.groupBoxRestore.ResumeLayout(false);
            this.groupBoxRestore.PerformLayout();
            this.groupBoxBackup.ResumeLayout(false);
            this.groupBoxBackup.PerformLayout();
            this.tabPageGeneral.ResumeLayout(false);
            this.groupBoxSystem.ResumeLayout(false);
            this.groupBoxSystem.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLogo)).EndInit();
            this.ResumeLayout(false);

        }
    }
}