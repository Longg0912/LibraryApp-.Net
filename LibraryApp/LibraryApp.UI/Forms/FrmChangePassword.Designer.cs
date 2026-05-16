namespace LibraryApp.UI.Forms;

partial class FrmChangePassword
{
    private System.ComponentModel.IContainer? components = null;

    protected override void Dispose(bool disposing)
    {
        if (disposing && components is not null)
            components.Dispose();
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    private void InitializeComponent()
    {
        this.components = new System.ComponentModel.Container();

        this.pnlHeader = new Panel();
        this.lblTitle = new Label();
        this.lblSubtitle = new Label();

        this.lblOldPassword = new Label();
        this.txtOldPassword = new TextBox();
        this.lblNewPassword = new Label();
        this.txtNewPassword = new TextBox();
        this.lblConfirm = new Label();
        this.txtConfirm = new TextBox();
        this.chkShowPassword = new CheckBox();
        this.lblHint = new Label();

        this.btnSave = new Button();
        this.btnCancel = new Button();

        this.errorProvider = new ErrorProvider(this.components);

        this.pnlHeader.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
        this.SuspendLayout();

        // ============================================================
        // pnlHeader
        // ============================================================
        this.pnlHeader.BackColor = System.Drawing.Color.FromArgb(33, 64, 154);
        this.pnlHeader.Controls.Add(this.lblSubtitle);
        this.pnlHeader.Controls.Add(this.lblTitle);
        this.pnlHeader.Dock = DockStyle.Top;
        this.pnlHeader.Height = 80;
        this.pnlHeader.Padding = new Padding(24, 14, 24, 14);

        this.lblTitle.AutoSize = false;
        this.lblTitle.Dock = DockStyle.Top;
        this.lblTitle.Font = new System.Drawing.Font("Segoe UI Semibold", 14F, System.Drawing.FontStyle.Bold);
        this.lblTitle.ForeColor = System.Drawing.Color.White;
        this.lblTitle.Height = 28;
        this.lblTitle.Text = "🔒 Đổi mật khẩu";

        this.lblSubtitle.AutoSize = false;
        this.lblSubtitle.Dock = DockStyle.Top;
        this.lblSubtitle.Font = new System.Drawing.Font("Segoe UI", 9.5F);
        this.lblSubtitle.ForeColor = System.Drawing.Color.FromArgb(200, 215, 255);
        this.lblSubtitle.Height = 22;
        this.lblSubtitle.Text = "Mật khẩu mới phải đáp ứng yêu cầu bảo mật.";

        // ============================================================
        // lblOldPassword / txtOldPassword
        // ============================================================
        this.lblOldPassword.AutoSize = true;
        this.lblOldPassword.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
        this.lblOldPassword.ForeColor = System.Drawing.Color.FromArgb(60, 60, 60);
        this.lblOldPassword.Location = new System.Drawing.Point(28, 105);
        this.lblOldPassword.Text = "MẬT KHẨU HIỆN TẠI";

        this.txtOldPassword.BorderStyle = BorderStyle.FixedSingle;
        this.txtOldPassword.Font = new System.Drawing.Font("Segoe UI", 10.5F);
        this.txtOldPassword.Location = new System.Drawing.Point(28, 128);
        this.txtOldPassword.MaxLength = 100;
        this.txtOldPassword.PasswordChar = '●';
        this.txtOldPassword.PlaceholderText = "Nhập mật khẩu hiện tại";
        this.txtOldPassword.Size = new System.Drawing.Size(384, 30);
        this.txtOldPassword.TabIndex = 0;

        // ============================================================
        // lblNewPassword / txtNewPassword
        // ============================================================
        this.lblNewPassword.AutoSize = true;
        this.lblNewPassword.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
        this.lblNewPassword.ForeColor = System.Drawing.Color.FromArgb(60, 60, 60);
        this.lblNewPassword.Location = new System.Drawing.Point(28, 172);
        this.lblNewPassword.Text = "MẬT KHẨU MỚI";

        this.txtNewPassword.BorderStyle = BorderStyle.FixedSingle;
        this.txtNewPassword.Font = new System.Drawing.Font("Segoe UI", 10.5F);
        this.txtNewPassword.Location = new System.Drawing.Point(28, 195);
        this.txtNewPassword.MaxLength = 100;
        this.txtNewPassword.PasswordChar = '●';
        this.txtNewPassword.PlaceholderText = "Tối thiểu 6 ký tự, có chữ và số";
        this.txtNewPassword.Size = new System.Drawing.Size(384, 30);
        this.txtNewPassword.TabIndex = 1;

        // ============================================================
        // lblConfirm / txtConfirm
        // ============================================================
        this.lblConfirm.AutoSize = true;
        this.lblConfirm.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
        this.lblConfirm.ForeColor = System.Drawing.Color.FromArgb(60, 60, 60);
        this.lblConfirm.Location = new System.Drawing.Point(28, 239);
        this.lblConfirm.Text = "NHẬP LẠI MẬT KHẨU MỚI";

        this.txtConfirm.BorderStyle = BorderStyle.FixedSingle;
        this.txtConfirm.Font = new System.Drawing.Font("Segoe UI", 10.5F);
        this.txtConfirm.Location = new System.Drawing.Point(28, 262);
        this.txtConfirm.MaxLength = 100;
        this.txtConfirm.PasswordChar = '●';
        this.txtConfirm.PlaceholderText = "Nhập lại để xác nhận";
        this.txtConfirm.Size = new System.Drawing.Size(384, 30);
        this.txtConfirm.TabIndex = 2;

        // ============================================================
        // chkShowPassword
        // ============================================================
        this.chkShowPassword.AutoSize = true;
        this.chkShowPassword.Font = new System.Drawing.Font("Segoe UI", 9F);
        this.chkShowPassword.ForeColor = System.Drawing.Color.FromArgb(80, 80, 80);
        this.chkShowPassword.Location = new System.Drawing.Point(28, 302);
        this.chkShowPassword.Text = "Hiển thị mật khẩu";
        this.chkShowPassword.TabIndex = 3;
        this.chkShowPassword.UseVisualStyleBackColor = true;
        this.chkShowPassword.CheckedChanged += new EventHandler(this.chkShowPassword_CheckedChanged);

        // ============================================================
        // lblHint - gợi ý yêu cầu mật khẩu
        // ============================================================
        this.lblHint.AutoSize = false;
        this.lblHint.BackColor = System.Drawing.Color.FromArgb(254, 252, 232);
        this.lblHint.Font = new System.Drawing.Font("Segoe UI", 8.5F);
        this.lblHint.ForeColor = System.Drawing.Color.FromArgb(146, 64, 14);
        this.lblHint.Location = new System.Drawing.Point(28, 332);
        this.lblHint.Size = new System.Drawing.Size(384, 60);
        this.lblHint.Padding = new Padding(12, 8, 12, 8);
        this.lblHint.Text = "💡 Yêu cầu mật khẩu:\n" +
                            "  • Tối thiểu 6 ký tự\n" +
                            "  • Có ít nhất 1 chữ cái và 1 chữ số\n" +
                            "  • Khác mật khẩu hiện tại";

        // ============================================================
        // btnCancel / btnSave
        // ============================================================
        this.btnCancel.BackColor = System.Drawing.Color.White;
        this.btnCancel.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(200, 200, 200);
        this.btnCancel.FlatStyle = FlatStyle.Flat;
        this.btnCancel.Font = new System.Drawing.Font("Segoe UI Semibold", 10F);
        this.btnCancel.ForeColor = System.Drawing.Color.FromArgb(60, 60, 60);
        this.btnCancel.Location = new System.Drawing.Point(160, 410);
        this.btnCancel.Size = new System.Drawing.Size(120, 38);
        this.btnCancel.Text = "✖ Huỷ";
        this.btnCancel.Cursor = Cursors.Hand;
        this.btnCancel.UseVisualStyleBackColor = false;
        this.btnCancel.DialogResult = DialogResult.Cancel;
        this.btnCancel.TabIndex = 5;

        this.btnSave.BackColor = System.Drawing.Color.FromArgb(33, 64, 154);
        this.btnSave.FlatAppearance.BorderSize = 0;
        this.btnSave.FlatStyle = FlatStyle.Flat;
        this.btnSave.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold);
        this.btnSave.ForeColor = System.Drawing.Color.White;
        this.btnSave.Location = new System.Drawing.Point(290, 410);
        this.btnSave.Size = new System.Drawing.Size(120, 38);
        this.btnSave.Text = "💾 Lưu";
        this.btnSave.Cursor = Cursors.Hand;
        this.btnSave.UseVisualStyleBackColor = false;
        this.btnSave.Click += new EventHandler(this.btnSave_Click);
        this.btnSave.TabIndex = 4;

        // errorProvider
        this.errorProvider.BlinkStyle = ErrorBlinkStyle.NeverBlink;
        this.errorProvider.ContainerControl = this;

        // ============================================================
        // FrmChangePassword
        // ============================================================
        this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
        this.AutoScaleMode = AutoScaleMode.Font;
        this.AcceptButton = this.btnSave;
        this.CancelButton = this.btnCancel;
        this.BackColor = System.Drawing.Color.White;
        this.ClientSize = new System.Drawing.Size(440, 470);
        this.Controls.Add(this.btnSave);
        this.Controls.Add(this.btnCancel);
        this.Controls.Add(this.lblHint);
        this.Controls.Add(this.chkShowPassword);
        this.Controls.Add(this.txtConfirm);
        this.Controls.Add(this.lblConfirm);
        this.Controls.Add(this.txtNewPassword);
        this.Controls.Add(this.lblNewPassword);
        this.Controls.Add(this.txtOldPassword);
        this.Controls.Add(this.lblOldPassword);
        this.Controls.Add(this.pnlHeader);
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;
        this.Name = "FrmChangePassword";
        this.StartPosition = FormStartPosition.CenterParent;
        this.Text = "Đổi mật khẩu";

        this.pnlHeader.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
        this.ResumeLayout(false);
        this.PerformLayout();
    }

    #endregion

    private Panel pnlHeader = null!;
    private Label lblTitle = null!;
    private Label lblSubtitle = null!;
    private Label lblOldPassword = null!;
    private TextBox txtOldPassword = null!;
    private Label lblNewPassword = null!;
    private TextBox txtNewPassword = null!;
    private Label lblConfirm = null!;
    private TextBox txtConfirm = null!;
    private CheckBox chkShowPassword = null!;
    private Label lblHint = null!;
    private Button btnSave = null!;
    private Button btnCancel = null!;
    private ErrorProvider errorProvider = null!;
}
