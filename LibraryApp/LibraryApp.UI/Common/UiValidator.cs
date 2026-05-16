using System.Text.RegularExpressions;

namespace LibraryApp.UI.Common;

/// <summary>
/// Helper validate các <see cref="Control"/> WinForms với <see cref="ErrorProvider"/>.
/// Có API kiểu "fluent" để xâu chuỗi nhiều rule validate trong vài dòng.
/// </summary>
/// <remarks>
/// Lý do có lớp này dù BLL đã có <c>Validator</c>:
/// <list type="bullet">
/// <item><c>BLL.Validator</c> ném <c>BusinessException</c> khi gặp lỗi — phù hợp validate
/// entity sau khi gom dữ liệu, nhưng không tự gắn lỗi vào <see cref="ErrorProvider"/>.</item>
/// <item><see cref="UiValidator"/> này gom <b>tất cả</b> lỗi vào <c>ErrorProvider</c> ngay
/// trên form, focus về control đầu tiên bị sai, và trả về <c>bool</c> để UI biết
/// có nên submit hay không. UX tốt hơn vì user thấy hết các ô đang sai một lần.</item>
/// </list>
///
/// Ví dụ dùng:
/// <code>
/// var v = new UiValidator(errorProvider);
/// v.RequireText(txtCode, "Mã sách")
///  .RequireText(txtTitle, "Tên sách")
///  .ValidateEmail(txtEmail, allowEmpty: true)
///  .RequirePositive(numQty, "Số lượng")
///  .RequireDateBefore(dtpFrom, dtpTo, "Ngày bắt đầu", "Ngày kết thúc");
///
/// if (!v.IsValid) return;   // ErrorProvider đã được set + focus về ô lỗi đầu tiên
/// </code>
/// </remarks>
public sealed class UiValidator
{
    private readonly ErrorProvider _errorProvider;
    private readonly List<(Control Control, string Message)> _errors = [];

    // Regex đồng bộ với BLL.Validation.Validator
    private static readonly Regex EmailPattern = new(
        @"^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$",
        RegexOptions.Compiled);

    private static readonly Regex PhonePattern = new(
        @"^[0-9+]{8,15}$",
        RegexOptions.Compiled);

    /// <summary>
    /// Khởi tạo validator với <see cref="ErrorProvider"/> trên form.
    /// Bộ validator này tự gọi <c>errorProvider.Clear()</c> để xóa lỗi cũ.
    /// </summary>
    public UiValidator(ErrorProvider errorProvider)
    {
        _errorProvider = errorProvider ?? throw new ArgumentNullException(nameof(errorProvider));
        _errorProvider.Clear();
    }

    /// <summary>Kết quả validate: <c>true</c> nếu không có lỗi nào.</summary>
    public bool IsValid => _errors.Count == 0;

    /// <summary>Số lỗi đã gom.</summary>
    public int ErrorCount => _errors.Count;

    /// <summary>
    /// Lỗi tổng hợp dạng văn bản (1 lỗi/dòng). Tiện hiển thị MessageBox nếu cần.
    /// </summary>
    public string GetSummary() => string.Join(Environment.NewLine,
        _errors.Select(e => $"• {e.Message}"));

    /// <summary>Focus về control bị lỗi đầu tiên (nếu có lỗi).</summary>
    public void FocusFirstError()
    {
        if (_errors.Count > 0)
            _errors[0].Control.Focus();
    }

    // ================================================================
    // Rule methods — fluent, trả về `this` để xâu chuỗi
    // ================================================================

    /// <summary>
    /// TextBox không được rỗng / chỉ chứa khoảng trắng.
    /// </summary>
    public UiValidator RequireText(TextBox txt, string fieldName, int? minLength = null, int? maxLength = null)
    {
        if (string.IsNullOrWhiteSpace(txt.Text))
            return AddError(txt, $"{fieldName} không được để trống.");

        var trimmed = txt.Text.Trim();
        if (minLength.HasValue && trimmed.Length < minLength.Value)
            return AddError(txt, $"{fieldName} phải có ít nhất {minLength.Value} ký tự.");
        if (maxLength.HasValue && trimmed.Length > maxLength.Value)
            return AddError(txt, $"{fieldName} không được vượt quá {maxLength.Value} ký tự.");

        return this;
    }

    /// <summary>
    /// ComboBox phải có item được chọn (không phải null/-1).
    /// </summary>
    public UiValidator RequireSelection(ComboBox cbo, string fieldName)
    {
        if (cbo.SelectedItem is null || cbo.SelectedIndex < 0)
            return AddError(cbo, $"Vui lòng chọn {fieldName.ToLowerInvariant()}.");

        // Nếu ComboBox bind kiểu key=0 cho "Tất cả", coi như chưa chọn
        if (cbo.ValueMember.Length > 0 && cbo.SelectedValue is int v && v == 0)
            return AddError(cbo, $"Vui lòng chọn {fieldName.ToLowerInvariant()}.");

        return this;
    }

    /// <summary>
    /// Validate định dạng email. Nếu <paramref name="allowEmpty"/> = true thì
    /// chuỗi rỗng được chấp nhận (email là optional).
    /// </summary>
    public UiValidator ValidateEmail(TextBox txt, bool allowEmpty = true, string fieldName = "Email")
    {
        if (string.IsNullOrWhiteSpace(txt.Text))
        {
            if (allowEmpty) return this;
            return AddError(txt, $"{fieldName} không được để trống.");
        }

        if (!EmailPattern.IsMatch(txt.Text.Trim()))
            return AddError(txt, $"{fieldName} không đúng định dạng (vd: name@domain.com).");

        return this;
    }

    /// <summary>
    /// Validate định dạng số điện thoại (8-15 ký tự, chỉ chữ số + dấu +).
    /// </summary>
    public UiValidator ValidatePhone(TextBox txt, bool allowEmpty = true, string fieldName = "Số điện thoại")
    {
        if (string.IsNullOrWhiteSpace(txt.Text))
        {
            if (allowEmpty) return this;
            return AddError(txt, $"{fieldName} không được để trống.");
        }

        if (!PhonePattern.IsMatch(txt.Text.Trim()))
            return AddError(txt,
                $"{fieldName} chỉ được chứa chữ số và dấu +, dài 8-15 ký tự.");

        return this;
    }

    /// <summary>NumericUpDown phải có giá trị &gt; 0.</summary>
    public UiValidator RequirePositive(NumericUpDown num, string fieldName)
    {
        if (num.Value <= 0)
            return AddError(num, $"{fieldName} phải lớn hơn 0.");
        return this;
    }

    /// <summary>NumericUpDown phải có giá trị &gt;= 0.</summary>
    public UiValidator RequireNonNegative(NumericUpDown num, string fieldName)
    {
        if (num.Value < 0)
            return AddError(num, $"{fieldName} không được âm.");
        return this;
    }

    /// <summary>NumericUpDown phải nằm trong khoảng [min, max].</summary>
    public UiValidator RequireInRange(NumericUpDown num, string fieldName, decimal min, decimal max)
    {
        if (num.Value < min || num.Value > max)
            return AddError(num,
                $"{fieldName} phải nằm trong khoảng từ {min:N0} đến {max:N0}.");
        return this;
    }

    /// <summary>DateTimePicker không được sau hôm nay.</summary>
    public UiValidator RequireDateNotFuture(DateTimePicker dtp, string fieldName)
    {
        if (dtp.Value.Date > DateTime.Today)
            return AddError(dtp, $"{fieldName} không được sau hôm nay.");
        return this;
    }

    /// <summary>DateTimePicker không được trước hôm nay.</summary>
    public UiValidator RequireDateNotPast(DateTimePicker dtp, string fieldName)
    {
        if (dtp.Value.Date < DateTime.Today)
            return AddError(dtp, $"{fieldName} phải từ hôm nay trở đi.");
        return this;
    }

    /// <summary>
    /// Ngày <paramref name="from"/> phải nhỏ hơn <paramref name="to"/>.
    /// Lỗi gắn vào control <paramref name="to"/> để focus đúng chỗ user cần sửa.
    /// </summary>
    public UiValidator RequireDateBefore(DateTimePicker from, DateTimePicker to,
        string fromName, string toName)
    {
        if (from.Value.Date >= to.Value.Date)
            return AddError(to, $"{toName} phải sau {fromName.ToLowerInvariant()}.");
        return this;
    }

    /// <summary>
    /// Khoảng cách giữa 2 ngày không vượt quá <paramref name="maxDays"/>.
    /// </summary>
    public UiValidator RequireDateSpan(DateTimePicker from, DateTimePicker to,
        int maxDays, string spanName)
    {
        if ((to.Value.Date - from.Value.Date).TotalDays > maxDays)
            return AddError(to, $"{spanName} không được vượt quá {maxDays} ngày.");
        return this;
    }

    /// <summary>
    /// Custom rule: <paramref name="check"/> trả về <c>true</c> nếu lỗi.
    /// </summary>
    public UiValidator Custom(Control control, Func<bool> check, string message)
    {
        if (check()) return AddError(control, message);
        return this;
    }

    // ================================================================
    // Internal
    // ================================================================

    private UiValidator AddError(Control control, string message)
    {
        _errors.Add((control, message));
        _errorProvider.SetError(control, message);
        return this;
    }
}
