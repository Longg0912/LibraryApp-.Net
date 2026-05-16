using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace LibraryApp.UI.UserControls;

/// <summary>
/// Loại biểu đồ.
/// </summary>
public enum ChartType
{
    /// <summary>Cột dọc.</summary>
    Bar,
    /// <summary>Đường kẻ với điểm.</summary>
    Line,
    /// <summary>Tròn (pie).</summary>
    Pie
}

/// <summary>
/// Một điểm dữ liệu cho chart.
/// </summary>
/// <param name="Label">Nhãn hiển thị trên trục X (Bar/Line) hoặc trên lát (Pie).</param>
/// <param name="Value">Giá trị (trục Y).</param>
public sealed record ChartPoint(string Label, double Value);

/// <summary>
/// UserControl tự vẽ biểu đồ bằng GDI+. Hỗ trợ Bar / Line / Pie.
/// </summary>
/// <remarks>
/// Tự render bằng GDI+ thay vì dùng <c>System.Windows.Forms.DataVisualization.Charting</c>
/// vì package đó không có sẵn trên .NET 10 và các port community chưa stable.
/// Đủ dùng cho mức báo cáo nội bộ, không cần các tính năng nâng cao (zoom, animation, export).
/// </remarks>
public partial class SimpleChart : UserControl
{
    /// <summary>Loại biểu đồ. Đổi xong sẽ tự repaint.</summary>
    [Browsable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    public ChartType Type
    {
        get => _type;
        set { _type = value; Invalidate(); }
    }
    private ChartType _type = ChartType.Bar;

    /// <summary>Tiêu đề hiển thị trên đầu chart.</summary>
    [Browsable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    public string ChartTitle
    {
        get => _title;
        set { _title = value ?? ""; Invalidate(); }
    }
    private string _title = "";

    /// <summary>Định dạng giá trị (vd: "N0", "C0").</summary>
    [Browsable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    public string ValueFormat { get; set; } = "N0";

    private List<ChartPoint> _points = [];

    /// <summary>
    /// Bảng màu cho các series (xoay vòng nếu nhiều điểm hơn số màu).
    /// </summary>
    private static readonly Color[] Palette =
    [
        Color.FromArgb( 59, 130, 246),  // blue
        Color.FromArgb( 16, 185, 129),  // green
        Color.FromArgb(245, 158,  11),  // orange
        Color.FromArgb(139,  92, 246),  // purple
        Color.FromArgb(236,  72, 153),  // pink
        Color.FromArgb(  6, 182, 212),  // cyan
        Color.FromArgb(239,  68,  68),  // red
        Color.FromArgb( 99, 102, 241),  // indigo
        Color.FromArgb(234, 179,   8),  // yellow
        Color.FromArgb( 20, 184, 166),  // teal
    ];

    /// <summary>Khởi tạo control.</summary>
    public SimpleChart()
    {
        DoubleBuffered = true;
        BackColor = Color.White;
        SetStyle(ControlStyles.OptimizedDoubleBuffer
               | ControlStyles.AllPaintingInWmPaint
               | ControlStyles.UserPaint
               | ControlStyles.ResizeRedraw, true);
    }

    /// <summary>Cập nhật dữ liệu cho chart và repaint.</summary>
    public void SetData(IEnumerable<ChartPoint> points)
    {
        _points = points?.ToList() ?? [];
        Invalidate();
    }

    /// <inheritdoc/>
    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

        // Vẽ tiêu đề
        var titleHeight = 0;
        if (!string.IsNullOrEmpty(_title))
        {
            using var titleFont = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
            using var brush = new SolidBrush(Color.FromArgb(60, 60, 60));
            g.DrawString(_title, titleFont, brush, new PointF(12, 8));
            titleHeight = 32;
        }

        // Khu vực vẽ chart (trừ padding + tiêu đề)
        var chartArea = new RectangleF(
            16, titleHeight + 8,
            Width - 32, Height - titleHeight - 24);

        if (_points.Count == 0)
        {
            using var emptyFont = new Font("Segoe UI", 10F);
            using var emptyBrush = new SolidBrush(Color.FromArgb(150, 150, 150));
            var sf = new StringFormat
            { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            g.DrawString("(Không có dữ liệu)", emptyFont, emptyBrush, chartArea, sf);
            return;
        }

        switch (_type)
        {
            case ChartType.Bar: DrawBars(g, chartArea); break;
            case ChartType.Line: DrawLine(g, chartArea); break;
            case ChartType.Pie: DrawPie(g, chartArea); break;
        }
    }

    // ================================================================
    // Bar chart
    // ================================================================

    private void DrawBars(Graphics g, RectangleF area)
    {
        const float axisLeft = 60f;   // chỗ chừa cho nhãn trục Y
        const float axisBottom = 40f;   // chỗ chừa cho nhãn trục X
        var plot = new RectangleF(
            area.X + axisLeft, area.Y,
            area.Width - axisLeft, area.Height - axisBottom);

        // Tính max value để scale
        double maxVal = _points.Max(p => p.Value);
        if (maxVal <= 0) maxVal = 1;
        double niceMax = NiceCeiling(maxVal);

        // Vẽ grid + nhãn trục Y (5 mức)
        using var gridPen = new Pen(Color.FromArgb(232, 234, 240));
        using var axisFont = new Font("Segoe UI", 8.25F);
        using var axisBrush = new SolidBrush(Color.FromArgb(110, 110, 110));

        for (int i = 0; i <= 5; i++)
        {
            float y = plot.Bottom - (plot.Height * i / 5f);
            g.DrawLine(gridPen, plot.Left, y, plot.Right, y);

            double tickValue = niceMax * i / 5;
            string label = tickValue.ToString(ValueFormat);
            var sz = g.MeasureString(label, axisFont);
            g.DrawString(label, axisFont, axisBrush,
                plot.Left - sz.Width - 6, y - sz.Height / 2);
        }

        // Vẽ các cột
        int n = _points.Count;
        float groupWidth = plot.Width / n;
        float barWidth = groupWidth * 0.65f;

        for (int i = 0; i < n; i++)
        {
            var point = _points[i];
            float barHeight = (float)(plot.Height * point.Value / niceMax);
            float x = plot.Left + groupWidth * i + (groupWidth - barWidth) / 2;
            float y = plot.Bottom - barHeight;

            var color = Palette[i % Palette.Length];

            // Cột với gradient nhẹ
            using (var brush = new LinearGradientBrush(
                new RectangleF(x, y, barWidth, barHeight),
                color, ControlPaint.Light(color, 0.3f),
                LinearGradientMode.Vertical))
            {
                g.FillRectangle(brush, x, y, barWidth, Math.Max(1, barHeight));
            }

            // Nhãn giá trị trên đỉnh cột (chỉ khi cột đủ cao)
            if (barHeight > 24)
            {
                using var valFont = new Font("Segoe UI Semibold", 8.25F, FontStyle.Bold);
                using var valBrush = new SolidBrush(Color.White);
                string valStr = point.Value.ToString(ValueFormat);
                var sz = g.MeasureString(valStr, valFont);
                g.DrawString(valStr, valFont, valBrush,
                    x + (barWidth - sz.Width) / 2, y + 4);
            }

            // Nhãn trục X
            using var labelFont = new Font("Segoe UI", 8F);
            using var labelBrush = new SolidBrush(Color.FromArgb(80, 80, 80));
            var labelSize = g.MeasureString(point.Label, labelFont);
            float labelX = x + (barWidth - labelSize.Width) / 2;
            // Cắt label nếu quá dài
            string label = TruncateLabel(point.Label, g, labelFont, groupWidth - 4);
            g.DrawString(label, labelFont, labelBrush,
                x + (barWidth - g.MeasureString(label, labelFont).Width) / 2,
                plot.Bottom + 6);
        }

        // Vẽ trục
        using var blackPen = new Pen(Color.FromArgb(180, 180, 180));
        g.DrawLine(blackPen, plot.Left, plot.Top, plot.Left, plot.Bottom);
        g.DrawLine(blackPen, plot.Left, plot.Bottom, plot.Right, plot.Bottom);
    }

    // ================================================================
    // Line chart
    // ================================================================

    private void DrawLine(Graphics g, RectangleF area)
    {
        const float axisLeft = 60f;
        const float axisBottom = 40f;
        var plot = new RectangleF(
            area.X + axisLeft, area.Y,
            area.Width - axisLeft, area.Height - axisBottom);

        double maxVal = _points.Max(p => p.Value);
        if (maxVal <= 0) maxVal = 1;
        double niceMax = NiceCeiling(maxVal);

        // Grid + nhãn Y
        using var gridPen = new Pen(Color.FromArgb(232, 234, 240));
        using var axisFont = new Font("Segoe UI", 8.25F);
        using var axisBrush = new SolidBrush(Color.FromArgb(110, 110, 110));

        for (int i = 0; i <= 5; i++)
        {
            float y = plot.Bottom - (plot.Height * i / 5f);
            g.DrawLine(gridPen, plot.Left, y, plot.Right, y);

            double tickValue = niceMax * i / 5;
            string label = tickValue.ToString(ValueFormat);
            var sz = g.MeasureString(label, axisFont);
            g.DrawString(label, axisFont, axisBrush,
                plot.Left - sz.Width - 6, y - sz.Height / 2);
        }

        // Tính toạ độ các điểm
        int n = _points.Count;
        if (n < 1) return;

        var pts = new PointF[n];
        float xStep = n > 1 ? plot.Width / (n - 1) : 0;

        for (int i = 0; i < n; i++)
        {
            float x = plot.Left + (n > 1 ? xStep * i : plot.Width / 2);
            float y = plot.Bottom - (float)(plot.Height * _points[i].Value / niceMax);
            pts[i] = new PointF(x, y);
        }

        // Vẽ vùng đổ màu phía dưới đường
        if (n >= 2)
        {
            var areaPts = new List<PointF>(pts) {
                new(pts[n - 1].X, plot.Bottom),
                new(pts[0].X,     plot.Bottom)
            };
            using var areaBrush = new LinearGradientBrush(
                new PointF(0, plot.Top), new PointF(0, plot.Bottom),
                Color.FromArgb(50, 59, 130, 246),
                Color.FromArgb(0, 59, 130, 246));
            g.FillPolygon(areaBrush, areaPts.ToArray());

            // Vẽ đường line
            using var linePen = new Pen(Color.FromArgb(59, 130, 246), 2.5f)
            { LineJoin = LineJoin.Round };
            g.DrawLines(linePen, pts);
        }

        // Vẽ marker tròn ở mỗi điểm + nhãn X
        using var pointBrush = new SolidBrush(Color.White);
        using var pointPen = new Pen(Color.FromArgb(59, 130, 246), 2f);
        using var labelFont = new Font("Segoe UI", 8F);
        using var labelBrush = new SolidBrush(Color.FromArgb(80, 80, 80));

        for (int i = 0; i < n; i++)
        {
            // Bỏ qua nhãn nếu quá nhiều điểm (chỉ hiện 6 điểm đầu/cuối)
            bool showLabel = n <= 12 || i % Math.Max(1, n / 10) == 0;

            g.FillEllipse(pointBrush, pts[i].X - 4, pts[i].Y - 4, 8, 8);
            g.DrawEllipse(pointPen, pts[i].X - 4, pts[i].Y - 4, 8, 8);

            if (showLabel)
            {
                string label = TruncateLabel(_points[i].Label, g, labelFont, xStep);
                var sz = g.MeasureString(label, labelFont);
                g.DrawString(label, labelFont, labelBrush,
                    pts[i].X - sz.Width / 2, plot.Bottom + 6);
            }
        }

        using var blackPen = new Pen(Color.FromArgb(180, 180, 180));
        g.DrawLine(blackPen, plot.Left, plot.Top, plot.Left, plot.Bottom);
        g.DrawLine(blackPen, plot.Left, plot.Bottom, plot.Right, plot.Bottom);
    }

    // ================================================================
    // Pie chart
    // ================================================================

    private void DrawPie(Graphics g, RectangleF area)
    {
        double total = _points.Sum(p => p.Value);
        if (total <= 0) return;

        // Chia đôi: pie bên trái, legend bên phải
        float pieSize = Math.Min(area.Width * 0.55f, area.Height) - 20;
        var pieRect = new RectangleF(
            area.X + 20,
            area.Y + (area.Height - pieSize) / 2,
            pieSize, pieSize);

        float startAngle = -90f;
        for (int i = 0; i < _points.Count; i++)
        {
            float sweepAngle = (float)(360.0 * _points[i].Value / total);
            var color = Palette[i % Palette.Length];

            using var brush = new SolidBrush(color);
            g.FillPie(brush, pieRect, startAngle, sweepAngle);

            using var pen = new Pen(Color.White, 2);
            g.DrawPie(pen, pieRect, startAngle, sweepAngle);

            startAngle += sweepAngle;
        }

        // Legend bên phải
        float legendX = pieRect.Right + 24;
        float legendY = area.Y + 12;
        using var legendFont = new Font("Segoe UI", 9F);
        using var labelBrush = new SolidBrush(Color.FromArgb(60, 60, 60));

        for (int i = 0; i < _points.Count; i++)
        {
            var color = Palette[i % Palette.Length];
            using var brush = new SolidBrush(color);

            // Hộp màu
            g.FillRectangle(brush, legendX, legendY + 2, 14, 14);

            // Label + value + %
            double pct = _points[i].Value / total * 100;
            string text = $"{_points[i].Label} — {_points[i].Value.ToString(ValueFormat)} ({pct:F1}%)";
            g.DrawString(text, legendFont, labelBrush, legendX + 22, legendY);

            legendY += 22;
            if (legendY > area.Bottom - 20) break;  // tránh tràn
        }
    }

    // ================================================================
    // Helpers
    // ================================================================

    /// <summary>
    /// Làm tròn số lên giá trị "đẹp" để tick trục Y dễ đọc.
    /// </summary>
    private static double NiceCeiling(double value)
    {
        if (value <= 0) return 1;
        double mag = Math.Pow(10, Math.Floor(Math.Log10(value)));
        double norm = value / mag;
        double nice = norm switch
        {
            <= 1 => 1,
            <= 2 => 2,
            <= 5 => 5,
            _ => 10
        };
        return nice * mag;
    }

    /// <summary>Cắt nhãn nếu quá rộng so với <paramref name="maxWidth"/>.</summary>
    private static string TruncateLabel(string label, Graphics g, Font font, float maxWidth)
    {
        if (g.MeasureString(label, font).Width <= maxWidth) return label;
        for (int len = label.Length - 1; len > 0; len--)
        {
            var candidate = label[..len] + "…";
            if (g.MeasureString(candidate, font).Width <= maxWidth)
                return candidate;
        }
        return "…";
    }
}
