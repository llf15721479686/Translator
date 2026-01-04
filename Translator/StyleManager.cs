// StyleManager.cs
using System.Drawing;
using System.Windows.Forms;

namespace Translator
{
    public static class StyleManager
    {
        public static class Colors
        {
            public static Color Primary = Color.FromArgb(51, 153, 255);
            public static Color Secondary = Color.FromArgb(108, 117, 125);
            public static Color Success = Color.FromArgb(40, 167, 69);
            public static Color Danger = Color.FromArgb(220, 53, 69);
            public static Color Warning = Color.FromArgb(255, 193, 7);
            public static Color Info = Color.FromArgb(23, 162, 184);
            public static Color Light = Color.FromArgb(248, 249, 250);
            public static Color Dark = Color.FromArgb(52, 58, 64);
            public static Color Background = Color.FromArgb(255, 255, 255);
            public static Color Border = Color.FromArgb(222, 226, 230);
        }

        public static class Fonts
        {
            public static Font Title = new Font("Microsoft YaHei UI", 12F, FontStyle.Bold);
            public static Font Subtitle = new Font("Microsoft YaHei UI", 10F, FontStyle.Bold);
            public static Font Normal = new Font("Microsoft YaHei UI", 9F);
            public static Font Small = new Font("Microsoft YaHei UI", 8F);
        }

        public static void ApplyButtonStyle(Button button, ButtonStyle style = ButtonStyle.Primary)
        {
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.Cursor = Cursors.Hand;

            switch (style)
            {
                case ButtonStyle.Primary:
                    button.BackColor = Colors.Primary;
                    button.ForeColor = Color.White;
                    break;
                case ButtonStyle.Secondary:
                    button.BackColor = Colors.Secondary;
                    button.ForeColor = Color.White;
                    break;
                case ButtonStyle.Danger:
                    button.BackColor = Colors.Danger;
                    button.ForeColor = Color.White;
                    break;
                case ButtonStyle.Success:
                    button.BackColor = Colors.Success;
                    button.ForeColor = Color.White;
                    break;
                case ButtonStyle.Light:
                    button.BackColor = Colors.Light;
                    button.ForeColor = Colors.Dark;
                    break;
            }

            button.FlatAppearance.MouseOverBackColor = Color.FromArgb(
                button.BackColor.R - 20,
                button.BackColor.G - 20,
                button.BackColor.B - 20);
            button.FlatAppearance.MouseDownBackColor = Color.FromArgb(
                button.BackColor.R - 40,
                button.BackColor.G - 40,
                button.BackColor.B - 40);
        }

        public static void ApplyTextBoxStyle(TextBox textBox)
        {
            textBox.BorderStyle = BorderStyle.FixedSingle;
            textBox.Font = Fonts.Normal;
            textBox.BackColor = Colors.Background;
        }

        public static void ApplyLabelStyle(Label label, FontStyle fontStyle = FontStyle.Regular)
        {
            label.Font = new Font(label.Font.FontFamily, label.Font.Size, fontStyle);
            label.ForeColor = Colors.Dark;
        }
    }

    public enum ButtonStyle
    {
        Primary,
        Secondary,
        Success,
        Danger,
        Warning,
        Info,
        Light,
        Dark
    }
}