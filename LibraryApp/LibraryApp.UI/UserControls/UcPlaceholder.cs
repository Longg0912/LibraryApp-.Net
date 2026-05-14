namespace LibraryApp.UI.UserControls;

/// <summary>
/// UserControl placeholder hiển thị thông báo "đang xây dựng".
/// Dùng tạm cho các menu chưa có form thực — nhờ vậy MenuStrip
/// hoạt động đầy đủ ngay cả khi mới làm xong khung MainForm.
/// </summary>
public sealed class UcPlaceholder : UserControl
{
    private readonly Label _lblIcon;
    private readonly Label _lblTitle;
    private readonly Label _lblMessage;

    /// <summary>Khởi tạo UserControl với layout sẵn.</summary>
    public UcPlaceholder()
    {
        BackColor = System.Drawing.Color.FromArgb(245, 247, 251);

        // Container ở giữa
        var panel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 3,
            BackColor = System.Drawing.Color.Transparent
        };
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        panel.RowStyles.Add(new RowStyle(SizeType.Percent, 40));
        panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        panel.RowStyles.Add(new RowStyle(SizeType.Percent, 60));

        _lblIcon = new Label
        {
            AutoSize = false,
            Dock = DockStyle.Fill,
            Font = new System.Drawing.Font("Segoe UI Emoji", 48F),
            ForeColor = System.Drawing.Color.FromArgb(180, 190, 210),
            Text = "🚧",
            TextAlign = System.Drawing.ContentAlignment.BottomCenter
        };

        _lblTitle = new Label
        {
            AutoSize = false,
            Dock = DockStyle.Top,
            Font = new System.Drawing.Font("Segoe UI Semibold", 16F, System.Drawing.FontStyle.Bold),
            ForeColor = System.Drawing.Color.FromArgb(60, 60, 60),
            Height = 40,
            Text = "Đang xây dựng",
            TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        };

        _lblMessage = new Label
        {
            AutoSize = false,
            Dock = DockStyle.Top,
            Font = new System.Drawing.Font("Segoe UI", 10.5F),
            ForeColor = System.Drawing.Color.FromArgb(120, 120, 120),
            Height = 30,
            Text = "Tính năng này sẽ được hoàn thiện trong các bước tiếp theo.",
            TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        };

        var middlePanel = new Panel { Dock = DockStyle.Fill };
        middlePanel.Controls.Add(_lblMessage);
        middlePanel.Controls.Add(_lblTitle);

        panel.Controls.Add(_lblIcon, 0, 0);
        panel.Controls.Add(middlePanel, 0, 1);

        Controls.Add(panel);
    }
}
