using System.Drawing;
using System.Windows.Forms;

public static class LayoutHelper
{
    /// <summary>
    /// 全面调整子窗体以适应容器
    /// </summary>
    public static void AdaptFormToContainer(Form childForm, Control container)
    {
        childForm.TopLevel = false;
        childForm.FormBorderStyle = FormBorderStyle.None;
        childForm.Dock = DockStyle.Fill;
        childForm.AutoScroll = true;

        // 调整子窗体控件
        AdjustChildControls(childForm);

        // 添加到容器
        container.Controls.Clear();
        container.Controls.Add(childForm);
        childForm.Show();
    }

    /// <summary>
    /// 调整子窗体中的所有控件
    /// </summary>
    private static void AdjustChildControls(Form form)
    {
        foreach (Control control in form.Controls)
        {
            // 设置基础锚定
            if (control is TextBox textBox && textBox.Multiline)
            {
                control.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            }
            else if (control is DataGridView dgv)
            {
                control.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
                dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dgv.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            }
            else if (control is Button button)
            {
                control.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            }

            // 处理嵌套控件
            if (control.HasChildren)
            {
                AdjustNestedControls(control);
            }
        }
    }

    /// <summary>
    /// 调整嵌套控件
    /// </summary>
    private static void AdjustNestedControls(Control parent)
    {
        foreach (Control child in parent.Controls)
        {
            if (child is Panel panel)
            {
                panel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            }
            // 递归处理更深层次的控件
            if (child.HasChildren)
            {
                AdjustNestedControls(child);
            }
        }
    }
    /// <summary>
    /// 创建卡片式布局的面板
    /// </summary>
    public static Panel CreateCardPanel(string title, string description, Color backColor)
    {
        Panel card = new Panel
        {
            Size = new System.Drawing.Size(250, 150),
            BackColor = backColor,
            Padding = new Padding(15),
            BorderStyle = BorderStyle.FixedSingle,
            Margin = new Padding(10)
        };

        Label titleLabel = new Label
        {
            Text = title,
            Font = new System.Drawing.Font("Microsoft YaHei UI", 12F, System.Drawing.FontStyle.Bold),
            ForeColor = System.Drawing.Color.White,
            AutoSize = false,
            Size = new System.Drawing.Size(220, 30),
            TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        };

        Label descLabel = new Label
        {
            Text = description,
            Font = new System.Drawing.Font("Microsoft YaHei UI", 9F),
            ForeColor = System.Drawing.Color.White,
            AutoSize = false,
            Size = new System.Drawing.Size(220, 80),
            TextAlign = System.Drawing.ContentAlignment.TopLeft,
            Location = new System.Drawing.Point(0, 40)
        };

        card.Controls.Add(titleLabel);
        card.Controls.Add(descLabel);

        return card;
    }
}