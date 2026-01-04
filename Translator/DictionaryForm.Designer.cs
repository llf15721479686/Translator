using System.Drawing;

namespace Translator
{
    partial class DictionaryForm
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

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // DictionaryForm
            // 
            this.BackColor = Color.FromArgb(245, 247, 250);
            this.ClientSize = new Size(1200, 700);
            this.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            this.Name = "DictionaryForm";
            this.Text = "智能翻译词典";
            this.ResumeLayout(false);
        }

        #endregion
    }
}