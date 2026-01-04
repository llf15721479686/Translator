// CacheManagementForm.cs
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Translator
{
    public partial class CacheManagementForm : Form
    {
        private DataTable cacheDataTable;
        private string connectionString = "Server=.;Database=Translator;Integrated Security=True;";
        // 分页相关变量
        private int currentPage = 1;
        private int pageSize = 1000; // 每页显示20条记录
        private int totalPages = 1;
        public CacheManagementForm()
        {
            InitializeComponent();
            LoadCacheData();
        }

        private void CacheManagementForm_Load(object sender, EventArgs e)
        {
            SetupAdaptiveLayout();
        }

        private void SetupAdaptiveLayout()
        {
            // 搜索面板自适应
            panelTop.Dock = DockStyle.Top;
            panelTop.Height = 70;
            panelTop.Margin = new Padding(0);
            panelTop.Padding = new Padding(12, 10, 12, 10);

            // 数据表格核心自适应配置（关键修复）
            dgvCache.Dock = DockStyle.Fill; // 强制填充面板剩余空间
            dgvCache.Margin = new Padding(12, 0, 12, 12);
            dgvCache.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill; // 列自动填充宽度
            dgvCache.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.False;
            dgvCache.RowHeadersVisible = false; // 隐藏行头，避免占用空间
            dgvCache.AllowUserToResizeColumns = true; // 允许用户微调列宽
            dgvCache.AllowUserToResizeRows = false;

            // 底部按钮面板
            panelBottom.Dock = DockStyle.Bottom;
            panelBottom.Height = 50;
            panelBottom.Margin = new Padding(0);

            // 列宽比例分配（仅在列加载后执行）
            if (dgvCache.Columns.Count > 0)
            {
                SetColumnFillWeights();
            }
        }
        // 单独封装列宽比例设置，避免重复代码
        private void SetColumnFillWeights()
        {
            if (dgvCache == null || dgvCache.Columns == null) return;

            // 辅助方法：安全设置列填充权重
            void SetWeight(string colName, float weight)
            {
                if (dgvCache.Columns.Contains(colName))
                {
                    dgvCache.Columns[colName].FillWeight = weight;
                }
            }

            // 核心调整：大幅降低SourceText权重，微调其他列，保证最后一列显示
            SetWeight("Id", 5);
            SetWeight("SourceText", 15); // 从25降到18，减少宽度
            SetWeight("SourceLanguage", 8);
            SetWeight("TargetLanguage", 8);
            SetWeight("TranslatedText", 22); // 适度降低，腾出空间
            SetWeight("CreatedTime", 12);
            SetWeight("LastUsedTime", 12);
            SetWeight("UseCount", 8); // 从1升到5，保证最后一列能显示
        }

        // 完全重构 Resize 事件，移除所有手动调整尺寸的代码
        private void CacheManagementForm_Resize(object sender, EventArgs e)
        {
            // 仅确保面板宽度跟随窗体，表格由Dock=Fill自动处理
            if (panelTop != null)
            {
                panelTop.Width = this.ClientSize.Width;
            }
            if (panelBottom != null)
            {
                panelBottom.Width = this.ClientSize.Width;
            }

            // 移除AutoResizeColumns调用（Fill模式下无需手动调整，Dock+Fill会自动适配）
            // 如需调整列宽，仅保留填充模式配置，不调用AutoResizeColumns
            if (dgvCache.Columns.Count > 0 && dgvCache.AutoSizeColumnsMode != DataGridViewAutoSizeColumnsMode.Fill)
            {
                dgvCache.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
        }


        // 补充：在 LoadCacheData 后调用列宽设置，确保数据加载后列宽正确
        private void LoadCacheData()
        {
            // 初始化空表格，避免null导致判断异常
            cacheDataTable = new DataTable();
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // 先获取总记录数用于计算分页
                    string countQuery = "SELECT COUNT(*) FROM TranslationCache";
                    using (var countCommand = new SqlCommand(countQuery, connection))
                    {
                        int totalRecords = (int)countCommand.ExecuteScalar();
                        totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
                        currentPage = Math.Min(currentPage, totalPages); // 确保当前页不超过总页数
                    }

                    // 分页查询
                    string query = @"
            SELECT * FROM (
                SELECT 
                    ROW_NUMBER() OVER (ORDER BY LastUsedTime DESC) AS RowNum,
                    Id,
                    SourceText,
                    SourceLanguage,
                    TargetLanguage,
                    TranslatedText,
                    CONVERT(varchar, CreatedTime, 120) as CreatedTime,
                    CONVERT(varchar, LastUsedTime, 120) as LastUsedTime,
                    UseCount
                FROM TranslationCache 
            ) AS T
            WHERE RowNum BETWEEN @StartRow AND @EndRow";

                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                    adapter.SelectCommand.Parameters.AddWithValue("@StartRow", (currentPage - 1) * pageSize + 1);
                    adapter.SelectCommand.Parameters.AddWithValue("@EndRow", currentPage * pageSize);

                    adapter.Fill(cacheDataTable);

                    // 移除RowNum列，不显示给用户
                    if (cacheDataTable.Columns.Contains("RowNum"))
                        cacheDataTable.Columns.Remove("RowNum");

                    dgvCache.DataSource = cacheDataTable;

                    dgvCache.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                    if (dgvCache.Columns.Count > 0)
                    {
                        SetColumnFillWeights();
                    }

                    // 更新分页信息
                    UpdatePaginationInfo();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"加载数据失败: {ex.Message}", "错误",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            UpdateStatistics();
        }
        private void UpdatePaginationInfo()
        {
            lblPageInfo.Text = $"第 {currentPage} 页 / 共 {totalPages} 页 (每页 {pageSize} 条)";
            btnPrev.Enabled = currentPage > 1;
            btnNext.Enabled = currentPage < totalPages;
        }

        private void btnFirst_Click(object sender, EventArgs e)
        {
            if (currentPage != 1)
            {
                currentPage = 1;
                LoadCacheData();
            }
        }

        private void btnPrev_Click(object sender, EventArgs e)
        {
            if (currentPage > 1)
            {
                currentPage--;
                LoadCacheData();
            }
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (currentPage < totalPages)
            {
                currentPage++;
                LoadCacheData();
            }
        }

        private void btnLast_Click(object sender, EventArgs e)
        {
            if (currentPage != totalPages)
            {
                currentPage = totalPages;
                LoadCacheData();
            }
        }

        private void cboPageSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (int.TryParse(cboPageSize.Text, out int newSize) && newSize > 0)
            {
                pageSize = newSize;
                currentPage = 1; // 重置到第一页
                LoadCacheData();
            }
        }

        private void UpdateStatistics()
        {
            // 先清空原有文本，避免覆盖
            lblStatistics.Text = "";
            lblTotal.Text = "";

            // 覆盖所有空数据场景：表格为null、行数为0
            if (cacheDataTable == null || cacheDataTable.Rows.Count == 0)
            {
                lblStatistics.Text = "没有缓存数据";
                lblTotal.Text = "总计: 0 条记录";
                return;
            }

            // 有数据时的统计逻辑（保留原有）
            int totalRecords = cacheDataTable.Rows.Count;
            int uniqueSourceCount = cacheDataTable.AsEnumerable()
                .Select(row => row["SourceText"].ToString())
                .Distinct()
                .Count();

            DateTime today = DateTime.Today;
            int usedToday = 0;
            // 容错：避免LastUsedTime为空导致解析异常
            try
            {
                usedToday = cacheDataTable.AsEnumerable()
                    .Count(row =>
                        !string.IsNullOrEmpty(row["LastUsedTime"].ToString())
                        && DateTime.Parse(row["LastUsedTime"].ToString()).Date == today);
            }
            catch
            {
                usedToday = 0;
            }

            lblStatistics.Text = $"源文本: {uniqueSourceCount} 种";
            lblTotal.Text = $"总计: {totalRecords} 条记录 | 今日使用: {usedToday} 次";
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            SearchCacheData();
        }

        private void SearchCacheData()
        {
            if (cacheDataTable == null) return;

            string searchText = txtSearch.Text.Trim();
            if (string.IsNullOrEmpty(searchText))
            {
                dgvCache.DataSource = cacheDataTable;
                UpdateStatistics();
                return;
            }

            try
            {
                // 模糊查询，搜索所有文本字段
                var filteredRows = cacheDataTable.AsEnumerable()
                    .Where(row =>
                        row["SourceText"].ToString().IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0 ||
                        row["TranslatedText"].ToString().IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0 ||
                        row["SourceLanguage"].ToString().IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0 ||
                        row["TargetLanguage"].ToString().IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0);

                if (filteredRows.Any())
                {
                    DataTable filteredTable = filteredRows.CopyToDataTable();
                    dgvCache.DataSource = filteredTable;
                    lblStatistics.Text = $"找到 {filteredTable.Rows.Count} 条匹配记录";
                }
                else
                {
                    dgvCache.DataSource = null;
                    lblStatistics.Text = "未找到匹配的记录";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"搜索失败: {ex.Message}", "错误",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadCacheData();
            txtSearch.Clear();
        }

        private void btnClearOld_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("确定要清理30天以上未使用的缓存吗？", "确认清理",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                CleanupOldData(30);
            }
        }

        private void CleanupOldData(int daysToKeep)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = @"
                        DELETE FROM TranslationCache 
                        WHERE LastUsedTime < DATEADD(day, -@DaysToKeep, GETDATE())
                        AND UseCount <= 1";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@DaysToKeep", daysToKeep);
                        int deletedRows = command.ExecuteNonQuery();

                        MessageBox.Show($"清理完成！删除了 {deletedRows} 条旧记录。",
                            "清理成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                LoadCacheData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"清理失败: {ex.Message}", "错误",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            ExportToExcel();
        }

        private void ExportToExcel()
        {
            if (dgvCache.Rows.Count == 0)
            {
                MessageBox.Show("没有数据可导出", "提示",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Filter = "Excel文件 (*.xlsx)|*.xlsx|CSV文件 (*.csv)|*.csv";
            saveDialog.FileName = $"翻译缓存_{DateTime.Now:yyyyMMdd_HHmmss}";

            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    // 这里可以添加导出到Excel的逻辑
                    // 可以使用 NPOI 或 EPPlus 库来导出Excel
                    // 暂时使用简单的CSV格式
                    ExportToCsv(saveDialog.FileName);
                    MessageBox.Show($"数据已导出到: {saveDialog.FileName}", "导出成功",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"导出失败: {ex.Message}", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ExportToCsv(string filePath)
        {
            using (var writer = new System.IO.StreamWriter(filePath, false, System.Text.Encoding.UTF8))
            {
                // 写入列标题
                var headers = new List<string>();
                foreach (DataGridViewColumn column in dgvCache.Columns)
                {
                    headers.Add(column.HeaderText);
                }
                writer.WriteLine(string.Join(",", headers));

                // 写入数据
                foreach (DataGridViewRow row in dgvCache.Rows)
                {
                    if (!row.IsNewRow)
                    {
                        var cells = new List<string>();
                        foreach (DataGridViewCell cell in row.Cells)
                        {
                            string value = cell.Value?.ToString() ?? "";
                            // 处理CSV格式（转义引号和逗号）
                            if (value.Contains(",") || value.Contains("\"") || value.Contains("\n"))
                            {
                                value = "\"" + value.Replace("\"", "\"\"") + "\"";
                            }
                            cells.Add(value);
                        }
                        writer.WriteLine(string.Join(",", cells));
                    }
                }
            }
        }

        private void txtSearch_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                SearchCacheData();
                e.Handled = true;
            }
        }

        private void btnDeleteAll_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("确定要清空所有缓存数据吗？此操作不可恢复！", "警告",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                return;

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "DELETE FROM TranslationCache";

                    using (var command = new SqlCommand(query, connection))
                    {
                        int deletedRows = command.ExecuteNonQuery();
                        LoadCacheData();
                        MessageBox.Show($"已清空所有缓存，共删除 {deletedRows} 条记录",
                            "清空完成", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"清空失败: {ex.Message}", "错误",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}