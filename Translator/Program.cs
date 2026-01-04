using OfficeOpenXml; // 确保已引入此命名空间
using System;
using System.Windows.Forms;

namespace Translator
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new HomeForm());
        }
    }
}