namespace LibraryApp.UI.Forms;

partial class FrmReturn
{
    private System.ComponentModel.IContainer? components = null;

    protected override void Dispose(bool disposing)
    {
        if (disposing && components is not null)
            components.Dispose();
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support — do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        this.components = new System.ComponentModel.Container();

        // ============================================================
        // Controls
        // ============================================================
        this.pnlHeader = new Panel();
        this.lblHeader = new Label();
        this.lblReceiptInfo = new Label();
        this.lblReaderInfo = new Label();

        this.lblWarning = new Label();

        this.grid = new DataGridView();

        this.pnlFooter = new Panel();
        this.lblReturnDate = new Label();
        this.dtpReturnDate = new DateTimePicker();
        this.lblNote = new Label();
        this.txtNote = new TextBox();
        this.lblTotalFine = new Label();
        this.btnCancel = new Button();
        this.btnConfirm = new Button();

        ((System.ComponentModel.ISupportInitialize)(this.grid)).BeginInit();
        this.pnlHeader.SuspendLayout();
        this.pnlFooter.SuspendLayout();
        this.SuspendLayout();

        // ============================================================
        // pnlHeader - banner xanh navy phía trên
        // ============================================================
        this.pnlHeader.BackColor = System.Drawing.Color.FromArgb(33, 64, 154);
        this.pnlHeader.Controls.Add(this.lblReaderInfo);
        this.pnlHeader.Controls.Add(this.lblReceiptInfo);
        this.pnlHeader.Controls.Add(this.lblHeader);
        this.pnlHeader.Dock = DockStyle.Top;
        this.pnlHeader.Height = 90;
        this.pnlHeader.Padding = new Padding(24, 16, 24, 16);

        this.lblHeader.AutoSize = false;
        this.lblHeader.Dock = DockStyle.Top;
        this.lblHeader.Font = new System.Drawing.Font("Segoe UI Semibold", 13F, System.Drawing.FontStyle.Bold);
        this.lblHeader.ForeColor = System.Drawing.Color.White;
        this.lblHeader.Height = 28;
        this.lblHeader.Text = "✓ Ghi nhận trả sách";

        this.lblReceiptInfo.AutoSize = false;
        this.lblReceiptInfo.Dock = DockStyle.Top;
        this.lblReceiptInfo.Font = new System.Drawing.Font("Segoe UI", 9.5F);
        this.lblReceiptInfo.ForeColor = System.Drawing.Color.FromArgb(200, 215, 255);
        this.lblReceiptInfo.Height = 22;
        this.lblReceiptInfo.Text = "Đang tải thông tin phiếu...";

        this.lblReaderInfo.AutoSize = false;
        this.lblReaderInfo.Dock = DockStyle.Top;
        this.lblReaderInfo.Font = new System.Drawing.Font("Segoe UI", 9.5F);
        this.lblReaderInfo.ForeColor = System.Drawing.Color.FromArgb(200, 215, 255);
        this.lblReaderInfo.Height = 22;
        this.lblReaderInfo.Text = "";

        // ============================================================
        // lblWarning - thanh cảnh báo quá hạn
        // ============================================================
        this.lblWarning.AutoSize = false;
        this.lblWarning.BackColor = System.Drawing.Color.FromArgb(254, 243, 199);
        this.lblWarning.Dock = DockStyle.Top;
        this.lblWarning.Font = new System.Drawing.Font("Segoe UI", 9F);
        this.lblWarning.ForeColor = System.Drawing.Color.FromArgb(146, 64, 14);
        this.lblWarning.Height = 32;
        this.lblWarning.Padding = new Padding(24, 0, 24, 0);
        this.lblWarning.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
        this.lblWarning.Visible = false;

        // ============================================================
        // grid - DataGridView các dòng còn nợ
        // ============================================================
        this.grid.AllowUserToAddRows = false;
        this.grid.AllowUserToDeleteRows = false;
        this.grid.AllowUserToResizeRows = false;
        this.grid.AutoGenerateColumns = false;
        this.grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        this.grid.BackgroundColor = System.Drawing.Color.White;
        this.grid.BorderStyle = BorderStyle.None;
        this.grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
        this.grid.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
        {
            BackColor = System.Drawing.Color.FromArgb(245, 247, 251),
            Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold),
            Padding = new Padding(8, 0, 0, 0)
        };
        this.grid.ColumnHeadersHeight = 38;
        this.grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
        this.grid.DefaultCellStyle = new DataGridViewCellStyle
        {
            Font = new System.Drawing.Font("Segoe UI", 9.5F),
            SelectionBackColor = System.Drawing.Color.FromArgb(218, 232, 252),
            Padding = new Padding(8, 0, 0, 0)
        };
        this.grid.Dock = DockStyle.Fill;
        this.grid.EnableHeadersVisualStyles = false;
        this.grid.GridColor = System.Drawing.Color.FromArgb(232, 234, 240);
        this.grid.RowHeadersVisible = false;
        this.grid.RowTemplate.Height = 36;

        // Định nghĩa cột
        var colDetailId = new DataGridViewTextBoxColumn
        {
            Name = "BorrowDetailId",
            DataPropertyName = "BorrowDetailId",
            Visible = false
        };
        var colTitle = new DataGridViewTextBoxColumn
        {
            Name = "Title",
            DataPropertyName = "Title",
            HeaderText = "Sách",
            FillWeight = 30,
            ReadOnly = true
        };
        var colBorrowed = new DataGridViewTextBoxColumn
        {
            Name = "BorrowedQty",
            DataPropertyName = "BorrowedQty",
            HeaderText = "Đã mượn",
            FillWeight = 10,
            ReadOnly = true,
            DefaultCellStyle = new DataGridViewCellStyle
            { Alignment = DataGridViewContentAlignment.MiddleCenter }
        };
        var colReturned = new DataGridViewTextBoxColumn
        {
            Name = "ReturnedQty",
            DataPropertyName = "ReturnedQty",
            HeaderText = "Đã trả",
            FillWeight = 10,
            ReadOnly = true,
            DefaultCellStyle = new DataGridViewCellStyle
            { Alignment = DataGridViewContentAlignment.MiddleCenter }
        };
        var colRemaining = new DataGridViewTextBoxColumn
        {
            Name = "RemainingQty",
            DataPropertyName = "RemainingQty",
            HeaderText = "Còn nợ",
            FillWeight = 10,
            ReadOnly = true,
            DefaultCellStyle = new DataGridViewCellStyle
            {
                Alignment = DataGridViewContentAlignment.MiddleCenter,
                ForeColor = System.Drawing.Color.FromArgb(245, 158, 11),
                Font = new System.Drawing.Font("Segoe UI Semibold", 9.5F, System.Drawing.FontStyle.Bold)
            }
        };
        var colReturnNow = new DataGridViewTextBoxColumn
        {
            Name = "ReturnNow",
            DataPropertyName = "ReturnNow",
            HeaderText = "Trả lần này",
            FillWeight = 12,
            ReadOnly = false,
            DefaultCellStyle = new DataGridViewCellStyle
            {
                Alignment = DataGridViewContentAlignment.MiddleCenter,
                BackColor = System.Drawing.Color.FromArgb(254, 252, 232)
            }
        };
        var colCondition = new DataGridViewComboBoxColumn
        {
            Name = "ConditionDisplay",
            DataPropertyName = "ConditionDisplay",
            HeaderText = "Tình trạng",
            FillWeight = 14,
            DataSource = new[] { "Tốt", "Hỏng", "Mất" }
        };
        var colFine = new DataGridViewTextBoxColumn
        {
            Name = "Fine",
            DataPropertyName = "Fine",
            HeaderText = "Phạt (VNĐ)",
            FillWeight = 14,
            DefaultCellStyle = new DataGridViewCellStyle
            {
                Alignment = DataGridViewContentAlignment.MiddleRight,
                Format = "N0",
                BackColor = System.Drawing.Color.FromArgb(254, 252, 232)
            }
        };

        this.grid.Columns.AddRange(
            colDetailId, colTitle, colBorrowed, colReturned,
            colRemaining, colReturnNow, colCondition, colFine);

        // Đăng ký events
        this.grid.CellValueChanged += new DataGridViewCellEventHandler(this.grid_CellValueChanged);
        this.grid.CurrentCellDirtyStateChanged += new EventHandler(this.grid_CurrentCellDirtyStateChanged);
        this.grid.DataError += new DataGridViewDataErrorEventHandler(this.grid_DataError);

        // ============================================================
        // pnlFooter - khu vực ngày trả, ghi chú, nút thao tác
        // ============================================================
        this.pnlFooter.BackColor = System.Drawing.Color.FromArgb(245, 247, 251);
        this.pnlFooter.Controls.Add(this.btnConfirm);
        this.pnlFooter.Controls.Add(this.btnCancel);
        this.pnlFooter.Controls.Add(this.lblTotalFine);
        this.pnlFooter.Controls.Add(this.txtNote);
        this.pnlFooter.Controls.Add(this.lblNote);
        this.pnlFooter.Controls.Add(this.dtpReturnDate);
        this.pnlFooter.Controls.Add(this.lblReturnDate);
        this.pnlFooter.Dock = DockStyle.Bottom;
        this.pnlFooter.Height = 180;
        this.pnlFooter.Padding = new Padding(24, 16, 24, 16);

        this.lblReturnDate.AutoSize = true;
        this.lblReturnDate.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
        this.lblReturnDate.ForeColor = System.Drawing.Color.FromArgb(80, 80, 80);
        this.lblReturnDate.Location = new System.Drawing.Point(24, 16);
        this.lblReturnDate.Text = "Ngày trả";

        this.dtpReturnDate.Format = DateTimePickerFormat.Short;
        this.dtpReturnDate.Font = new System.Drawing.Font("Segoe UI", 9.5F);
        this.dtpReturnDate.Location = new System.Drawing.Point(24, 36);
        this.dtpReturnDate.Size = new System.Drawing.Size(150, 25);
        this.dtpReturnDate.MaxDate = DateTime.Today;
        this.dtpReturnDate.Value = DateTime.Today;
        this.dtpReturnDate.ValueChanged += new EventHandler(this.dtpReturnDate_ValueChanged);

        this.lblNote.AutoSize = true;
        this.lblNote.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
        this.lblNote.ForeColor = System.Drawing.Color.FromArgb(80, 80, 80);
        this.lblNote.Location = new System.Drawing.Point(200, 16);
        this.lblNote.Text = "Ghi chú";

        this.txtNote.BorderStyle = BorderStyle.FixedSingle;
        this.txtNote.Font = new System.Drawing.Font("Segoe UI", 9.5F);
        this.txtNote.Location = new System.Drawing.Point(200, 36);
        this.txtNote.MaxLength = 300;
        this.txtNote.Multiline = true;
        this.txtNote.Size = new System.Drawing.Size(580, 50);

        this.lblTotalFine.AutoSize = false;
        this.lblTotalFine.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold);
        this.lblTotalFine.ForeColor = System.Drawing.Color.FromArgb(33, 64, 154);
        this.lblTotalFine.Location = new System.Drawing.Point(24, 100);
        this.lblTotalFine.Size = new System.Drawing.Size(450, 30);
        this.lblTotalFine.Text = "Tổng tiền phạt: 0 VNĐ";

        this.btnCancel.BackColor = System.Drawing.Color.White;
        this.btnCancel.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(200, 200, 200);
        this.btnCancel.FlatStyle = FlatStyle.Flat;
        this.btnCancel.Font = new System.Drawing.Font("Segoe UI Semibold", 10F);
        this.btnCancel.ForeColor = System.Drawing.Color.FromArgb(60, 60, 60);
        this.btnCancel.Location = new System.Drawing.Point(490, 100);
        this.btnCancel.Size = new System.Drawing.Size(140, 38);
        this.btnCancel.Text = "✖ Huỷ";
        this.btnCancel.Cursor = Cursors.Hand;
        this.btnCancel.UseVisualStyleBackColor = false;
        this.btnCancel.DialogResult = DialogResult.Cancel;

        this.btnConfirm.BackColor = System.Drawing.Color.FromArgb(16, 185, 129);
        this.btnConfirm.FlatAppearance.BorderSize = 0;
        this.btnConfirm.FlatStyle = FlatStyle.Flat;
        this.btnConfirm.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold);
        this.btnConfirm.ForeColor = System.Drawing.Color.White;
        this.btnConfirm.Location = new System.Drawing.Point(640, 100);
        this.btnConfirm.Size = new System.Drawing.Size(160, 38);
        this.btnConfirm.Text = "✓ GHI NHẬN TRẢ";
        this.btnConfirm.Cursor = Cursors.Hand;
        this.btnConfirm.UseVisualStyleBackColor = false;
        this.btnConfirm.Click += new EventHandler(this.btnConfirm_Click);

        // ============================================================
        // FrmReturn
        // ============================================================
        this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
        this.AutoScaleMode = AutoScaleMode.Font;
        this.BackColor = System.Drawing.Color.White;
        this.CancelButton = this.btnCancel;
        this.ClientSize = new System.Drawing.Size(820, 600);
        this.Controls.Add(this.grid);
        this.Controls.Add(this.pnlFooter);
        this.Controls.Add(this.lblWarning);
        this.Controls.Add(this.pnlHeader);
        this.Font = new System.Drawing.Font("Segoe UI", 9.5F);
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;
        this.Name = "FrmReturn";
        this.StartPosition = FormStartPosition.CenterParent;
        this.Text = "Ghi nhận trả sách";
        this.Load += new EventHandler(this.FrmReturn_Load);

        ((System.ComponentModel.ISupportInitialize)(this.grid)).EndInit();
        this.pnlHeader.ResumeLayout(false);
        this.pnlFooter.ResumeLayout(false);
        this.pnlFooter.PerformLayout();
        this.ResumeLayout(false);
    }

    #endregion

    // ----------- Control fields -----------
    private Panel pnlHeader = null!;
    private Label lblHeader = null!;
    private Label lblReceiptInfo = null!;
    private Label lblReaderInfo = null!;

    private Label lblWarning = null!;

    private DataGridView grid = null!;

    private Panel pnlFooter = null!;
    private Label lblReturnDate = null!;
    private DateTimePicker dtpReturnDate = null!;
    private Label lblNote = null!;
    private TextBox txtNote = null!;
    private Label lblTotalFine = null!;
    private Button btnCancel = null!;
    private Button btnConfirm = null!;
}
