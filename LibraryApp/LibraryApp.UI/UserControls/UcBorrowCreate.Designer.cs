namespace LibraryApp.UI.UserControls;

partial class UcBorrowCreate
{
    private System.ComponentModel.IContainer? components = null;

    protected override void Dispose(bool disposing)
    {
        if (disposing && components is not null)
            components.Dispose();
        base.Dispose(disposing);
    }

    #region Designer-generated code

    private void InitializeComponent()
    {
        this.components = new System.ComponentModel.Container();

        // ===== Reader panel (top-left) =====
        this.pnlReader = new Panel();
        this.lblReaderHeader = new Label();
        this.lblCardSearch = new Label();
        this.txtCardSearch = new TextBox();
        this.btnLookupReader = new Button();
        this.pnlReaderInfo = new Panel();
        this.lblReaderName = new Label();
        this.lblReaderCard = new Label();
        this.lblReaderStatus = new Label();
        this.lblReaderExpiry = new Label();
        this.lblReaderActive = new Label();

        // ===== Receipt info panel (top-right) =====
        this.pnlReceipt = new Panel();
        this.lblReceiptHeader = new Label();
        this.lblBorrowDate = new Label();
        this.dtpBorrowDate = new DateTimePicker();
        this.lblDueDate = new Label();
        this.dtpDueDate = new DateTimePicker();
        this.lblNote = new Label();
        this.txtNote = new TextBox();

        // ===== Book search row =====
        this.pnlBookSearch = new Panel();
        this.lblPickBook = new Label();
        this.lblBookKeyword = new Label();
        this.txtBookKeyword = new TextBox();
        this.cboBookCandidates = new ComboBox();
        this.lblQty = new Label();
        this.numQty = new NumericUpDown();
        this.btnAddToCart = new Button();

        // ===== Cart grid =====
        this.lblCartHeader = new Label();
        this.dgvCart = new DataGridView();

        // ===== Action footer =====
        this.pnlFooter = new Panel();
        this.lblTotalQty = new Label();
        this.btnRemoveItem = new Button();
        this.btnClearCart = new Button();
        this.btnCreate = new Button();

        this.errorProvider = new ErrorProvider(this.components);

        ((System.ComponentModel.ISupportInitialize)(this.numQty)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.dgvCart)).BeginInit();
        this.pnlReader.SuspendLayout();
        this.pnlReaderInfo.SuspendLayout();
        this.pnlReceipt.SuspendLayout();
        this.pnlBookSearch.SuspendLayout();
        this.pnlFooter.SuspendLayout();
        this.SuspendLayout();

        // ============================================================
        // pnlReader (top-left)
        // ============================================================
        this.pnlReader.BackColor = System.Drawing.Color.White;
        this.pnlReader.Controls.AddRange(new Control[] {
            this.lblReaderHeader, this.lblCardSearch, this.txtCardSearch,
            this.btnLookupReader, this.pnlReaderInfo
        });
        this.pnlReader.Location = new System.Drawing.Point(20, 20);
        this.pnlReader.Padding = new Padding(20);
        this.pnlReader.Size = new System.Drawing.Size(540, 200);

        this.lblReaderHeader.AutoSize = true;
        this.lblReaderHeader.Font = new System.Drawing.Font("Segoe UI Semibold", 11F, System.Drawing.FontStyle.Bold);
        this.lblReaderHeader.ForeColor = System.Drawing.Color.FromArgb(33, 33, 33);
        this.lblReaderHeader.Location = new System.Drawing.Point(20, 12);
        this.lblReaderHeader.Text = "👤 Độc giả";

        this.lblCardSearch.AutoSize = true;
        this.lblCardSearch.Font = new System.Drawing.Font("Segoe UI", 9F);
        this.lblCardSearch.ForeColor = System.Drawing.Color.FromArgb(80, 80, 80);
        this.lblCardSearch.Location = new System.Drawing.Point(20, 45);
        this.lblCardSearch.Text = "Số thẻ:";

        this.txtCardSearch.BorderStyle = BorderStyle.FixedSingle;
        this.txtCardSearch.Font = new System.Drawing.Font("Segoe UI", 9.5F);
        this.txtCardSearch.Location = new System.Drawing.Point(75, 42);
        this.txtCardSearch.PlaceholderText = "Quét/nhập số thẻ rồi Enter";
        this.txtCardSearch.Size = new System.Drawing.Size(290, 25);
        this.txtCardSearch.KeyDown += new KeyEventHandler(this.txtCardSearch_KeyDown);

        this.btnLookupReader.BackColor = System.Drawing.Color.FromArgb(33, 64, 154);
        this.btnLookupReader.FlatAppearance.BorderSize = 0;
        this.btnLookupReader.FlatStyle = FlatStyle.Flat;
        this.btnLookupReader.Font = new System.Drawing.Font("Segoe UI Semibold", 9F);
        this.btnLookupReader.ForeColor = System.Drawing.Color.White;
        this.btnLookupReader.Location = new System.Drawing.Point(372, 41);
        this.btnLookupReader.Size = new System.Drawing.Size(130, 28);
        this.btnLookupReader.Text = "🔍 Tra cứu";
        this.btnLookupReader.Cursor = Cursors.Hand;
        this.btnLookupReader.UseVisualStyleBackColor = false;
        this.btnLookupReader.Click += new EventHandler(this.btnLookupReader_Click);

        // pnlReaderInfo - khu vực hiển thị thông tin độc giả sau khi tra cứu
        this.pnlReaderInfo.BackColor = System.Drawing.Color.FromArgb(245, 247, 251);
        this.pnlReaderInfo.Location = new System.Drawing.Point(20, 82);
        this.pnlReaderInfo.Padding = new Padding(15, 10, 10, 10);
        this.pnlReaderInfo.Size = new System.Drawing.Size(482, 88);
        this.pnlReaderInfo.Controls.AddRange(new Control[] {
            this.lblReaderName, this.lblReaderCard,
            this.lblReaderStatus, this.lblReaderExpiry, this.lblReaderActive
        });

        this.lblReaderName.AutoSize = true;
        this.lblReaderName.Font = new System.Drawing.Font("Segoe UI Semibold", 11F, System.Drawing.FontStyle.Bold);
        this.lblReaderName.ForeColor = System.Drawing.Color.FromArgb(33, 64, 154);
        this.lblReaderName.Location = new System.Drawing.Point(15, 10);
        this.lblReaderName.Text = "Chưa chọn độc giả";

        this.lblReaderCard.AutoSize = true;
        this.lblReaderCard.Font = new System.Drawing.Font("Segoe UI", 9F);
        this.lblReaderCard.ForeColor = System.Drawing.Color.FromArgb(80, 80, 80);
        this.lblReaderCard.Location = new System.Drawing.Point(15, 36);
        this.lblReaderCard.Text = "Số thẻ: -";

        this.lblReaderStatus.AutoSize = true;
        this.lblReaderStatus.Font = new System.Drawing.Font("Segoe UI", 9F);
        this.lblReaderStatus.ForeColor = System.Drawing.Color.FromArgb(80, 80, 80);
        this.lblReaderStatus.Location = new System.Drawing.Point(150, 36);
        this.lblReaderStatus.Text = "Trạng thái: -";

        this.lblReaderExpiry.AutoSize = true;
        this.lblReaderExpiry.Font = new System.Drawing.Font("Segoe UI", 9F);
        this.lblReaderExpiry.ForeColor = System.Drawing.Color.FromArgb(80, 80, 80);
        this.lblReaderExpiry.Location = new System.Drawing.Point(305, 36);
        this.lblReaderExpiry.Text = "Hết hạn: -";

        this.lblReaderActive.AutoSize = true;
        this.lblReaderActive.Font = new System.Drawing.Font("Segoe UI", 9F);
        this.lblReaderActive.ForeColor = System.Drawing.Color.FromArgb(120, 120, 120);
        this.lblReaderActive.Location = new System.Drawing.Point(15, 60);
        this.lblReaderActive.Text = "Đang mượn: -";

        // ============================================================
        // pnlReceipt (top-right)
        // ============================================================
        this.pnlReceipt.BackColor = System.Drawing.Color.White;
        this.pnlReceipt.Controls.AddRange(new Control[] {
            this.lblReceiptHeader,
            this.lblBorrowDate, this.dtpBorrowDate,
            this.lblDueDate, this.dtpDueDate,
            this.lblNote, this.txtNote
        });
        this.pnlReceipt.Location = new System.Drawing.Point(580, 20);
        this.pnlReceipt.Padding = new Padding(20);
        this.pnlReceipt.Size = new System.Drawing.Size(440, 200);

        this.lblReceiptHeader.AutoSize = true;
        this.lblReceiptHeader.Font = new System.Drawing.Font("Segoe UI Semibold", 11F, System.Drawing.FontStyle.Bold);
        this.lblReceiptHeader.ForeColor = System.Drawing.Color.FromArgb(33, 33, 33);
        this.lblReceiptHeader.Location = new System.Drawing.Point(20, 12);
        this.lblReceiptHeader.Text = "📋 Thông tin phiếu mượn";

        this.lblBorrowDate.AutoSize = true;
        this.lblBorrowDate.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
        this.lblBorrowDate.ForeColor = System.Drawing.Color.FromArgb(80, 80, 80);
        this.lblBorrowDate.Location = new System.Drawing.Point(20, 50);
        this.lblBorrowDate.Text = "Ngày mượn";

        this.dtpBorrowDate.Font = new System.Drawing.Font("Segoe UI", 9.5F);
        this.dtpBorrowDate.Format = DateTimePickerFormat.Short;
        this.dtpBorrowDate.Location = new System.Drawing.Point(20, 70);
        this.dtpBorrowDate.Size = new System.Drawing.Size(180, 25);
        this.dtpBorrowDate.MaxDate = DateTime.Today;
        this.dtpBorrowDate.Value = DateTime.Today;
        this.dtpBorrowDate.ValueChanged += new EventHandler(this.dtpBorrowDate_ValueChanged);

        this.lblDueDate.AutoSize = true;
        this.lblDueDate.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
        this.lblDueDate.ForeColor = System.Drawing.Color.FromArgb(80, 80, 80);
        this.lblDueDate.Location = new System.Drawing.Point(220, 50);
        this.lblDueDate.Text = "Hạn trả";

        this.dtpDueDate.Font = new System.Drawing.Font("Segoe UI", 9.5F);
        this.dtpDueDate.Format = DateTimePickerFormat.Short;
        this.dtpDueDate.Location = new System.Drawing.Point(220, 70);
        this.dtpDueDate.Size = new System.Drawing.Size(180, 25);
        this.dtpDueDate.Value = DateTime.Today.AddDays(14);

        this.lblNote.AutoSize = true;
        this.lblNote.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
        this.lblNote.ForeColor = System.Drawing.Color.FromArgb(80, 80, 80);
        this.lblNote.Location = new System.Drawing.Point(20, 108);
        this.lblNote.Text = "Ghi chú";

        this.txtNote.BorderStyle = BorderStyle.FixedSingle;
        this.txtNote.Font = new System.Drawing.Font("Segoe UI", 9.5F);
        this.txtNote.Location = new System.Drawing.Point(20, 128);
        this.txtNote.Multiline = true;
        this.txtNote.Size = new System.Drawing.Size(380, 50);
        this.txtNote.MaxLength = 300;

        // ============================================================
        // pnlBookSearch - dòng tìm sách + thêm vào giỏ
        // ============================================================
        this.pnlBookSearch.BackColor = System.Drawing.Color.White;
        this.pnlBookSearch.Controls.AddRange(new Control[] {
            this.lblPickBook,
            this.lblBookKeyword, this.txtBookKeyword,
            this.cboBookCandidates,
            this.lblQty, this.numQty,
            this.btnAddToCart
        });
        this.pnlBookSearch.Location = new System.Drawing.Point(20, 240);
        this.pnlBookSearch.Padding = new Padding(20, 15, 20, 15);
        this.pnlBookSearch.Size = new System.Drawing.Size(1000, 110);

        this.lblPickBook.AutoSize = true;
        this.lblPickBook.Font = new System.Drawing.Font("Segoe UI Semibold", 11F, System.Drawing.FontStyle.Bold);
        this.lblPickBook.ForeColor = System.Drawing.Color.FromArgb(33, 33, 33);
        this.lblPickBook.Location = new System.Drawing.Point(20, 12);
        this.lblPickBook.Text = "📚 Chọn sách";

        this.lblBookKeyword.AutoSize = true;
        this.lblBookKeyword.Font = new System.Drawing.Font("Segoe UI", 9F);
        this.lblBookKeyword.ForeColor = System.Drawing.Color.FromArgb(80, 80, 80);
        this.lblBookKeyword.Location = new System.Drawing.Point(20, 50);
        this.lblBookKeyword.Text = "Tìm:";

        this.txtBookKeyword.BorderStyle = BorderStyle.FixedSingle;
        this.txtBookKeyword.Font = new System.Drawing.Font("Segoe UI", 9.5F);
        this.txtBookKeyword.Location = new System.Drawing.Point(58, 47);
        this.txtBookKeyword.PlaceholderText = "Mã hoặc tên sách";
        this.txtBookKeyword.Size = new System.Drawing.Size(220, 25);
        this.txtBookKeyword.TextChanged += new EventHandler(this.txtBookKeyword_TextChanged);

        this.cboBookCandidates.DropDownStyle = ComboBoxStyle.DropDownList;
        this.cboBookCandidates.FlatStyle = FlatStyle.Flat;
        this.cboBookCandidates.Font = new System.Drawing.Font("Segoe UI", 9.5F);
        this.cboBookCandidates.Location = new System.Drawing.Point(290, 47);
        this.cboBookCandidates.Size = new System.Drawing.Size(440, 26);

        this.lblQty.AutoSize = true;
        this.lblQty.Font = new System.Drawing.Font("Segoe UI", 9F);
        this.lblQty.ForeColor = System.Drawing.Color.FromArgb(80, 80, 80);
        this.lblQty.Location = new System.Drawing.Point(745, 50);
        this.lblQty.Text = "SL:";

        this.numQty.Font = new System.Drawing.Font("Segoe UI", 9.5F);
        this.numQty.Location = new System.Drawing.Point(775, 47);
        this.numQty.Size = new System.Drawing.Size(70, 25);
        this.numQty.Minimum = 1;
        this.numQty.Maximum = 99;
        this.numQty.Value = 1;

        this.btnAddToCart.BackColor = System.Drawing.Color.FromArgb(16, 185, 129);
        this.btnAddToCart.FlatAppearance.BorderSize = 0;
        this.btnAddToCart.FlatStyle = FlatStyle.Flat;
        this.btnAddToCart.Font = new System.Drawing.Font("Segoe UI Semibold", 9F);
        this.btnAddToCart.ForeColor = System.Drawing.Color.White;
        this.btnAddToCart.Location = new System.Drawing.Point(855, 45);
        this.btnAddToCart.Size = new System.Drawing.Size(125, 30);
        this.btnAddToCart.Text = "➕ Thêm vào giỏ";
        this.btnAddToCart.Cursor = Cursors.Hand;
        this.btnAddToCart.UseVisualStyleBackColor = false;
        this.btnAddToCart.Click += new EventHandler(this.btnAddToCart_Click);

        // ============================================================
        // Cart label + DataGridView
        // ============================================================
        this.lblCartHeader.AutoSize = true;
        this.lblCartHeader.Font = new System.Drawing.Font("Segoe UI Semibold", 11F, System.Drawing.FontStyle.Bold);
        this.lblCartHeader.ForeColor = System.Drawing.Color.FromArgb(33, 33, 33);
        this.lblCartHeader.Location = new System.Drawing.Point(20, 365);
        this.lblCartHeader.Text = "🛒 Giỏ sách mượn";

        this.dgvCart.AllowUserToAddRows = false;
        this.dgvCart.AllowUserToDeleteRows = false;
        this.dgvCart.AllowUserToResizeRows = false;
        this.dgvCart.AutoGenerateColumns = false;
        this.dgvCart.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        this.dgvCart.BackgroundColor = System.Drawing.Color.White;
        this.dgvCart.BorderStyle = BorderStyle.None;
        this.dgvCart.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
        this.dgvCart.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
        {
            BackColor = System.Drawing.Color.FromArgb(245, 247, 251),
            ForeColor = System.Drawing.Color.FromArgb(60, 60, 60),
            Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold),
            Padding = new Padding(8, 0, 0, 0)
        };
        this.dgvCart.ColumnHeadersHeight = 38;
        this.dgvCart.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
        this.dgvCart.DefaultCellStyle = new DataGridViewCellStyle
        {
            BackColor = System.Drawing.Color.White,
            ForeColor = System.Drawing.Color.FromArgb(33, 33, 33),
            Font = new System.Drawing.Font("Segoe UI", 9.5F),
            SelectionBackColor = System.Drawing.Color.FromArgb(218, 232, 252),
            SelectionForeColor = System.Drawing.Color.FromArgb(33, 33, 33),
            Padding = new Padding(8, 0, 0, 0)
        };
        this.dgvCart.RowHeadersVisible = false;
        this.dgvCart.RowTemplate.Height = 32;
        this.dgvCart.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        this.dgvCart.MultiSelect = false;
        this.dgvCart.ReadOnly = true;
        this.dgvCart.EnableHeadersVisualStyles = false;
        this.dgvCart.GridColor = System.Drawing.Color.FromArgb(232, 234, 240);
        this.dgvCart.Location = new System.Drawing.Point(20, 392);
        this.dgvCart.Size = new System.Drawing.Size(1000, 240);

        // Cột giỏ sách
        var colBookId = new DataGridViewTextBoxColumn
        { DataPropertyName = "BookId", HeaderText = "ID", Name = "BookId", Visible = false };
        var colBookCode = new DataGridViewTextBoxColumn
        { DataPropertyName = "BookCode", HeaderText = "Mã sách", Name = "BookCode", FillWeight = 15 };
        var colBookTitle = new DataGridViewTextBoxColumn
        { DataPropertyName = "Title", HeaderText = "Tên sách", Name = "Title", FillWeight = 45 };
        var colBookAuthor = new DataGridViewTextBoxColumn
        { DataPropertyName = "Author", HeaderText = "Tác giả", Name = "Author", FillWeight = 25 };
        var colBookQty = new DataGridViewTextBoxColumn
        {
            DataPropertyName = "Quantity",
            HeaderText = "Số lượng",
            Name = "Quantity",
            FillWeight = 10,
            DefaultCellStyle = new DataGridViewCellStyle
            { Alignment = DataGridViewContentAlignment.MiddleCenter }
        };
        var colAvail = new DataGridViewTextBoxColumn
        {
            DataPropertyName = "AvailableQty",
            HeaderText = "Tồn",
            Name = "AvailableQty",
            FillWeight = 8,
            DefaultCellStyle = new DataGridViewCellStyle
            { Alignment = DataGridViewContentAlignment.MiddleCenter, ForeColor = System.Drawing.Color.FromArgb(120, 120, 120) }
        };

        this.dgvCart.Columns.AddRange(colBookId, colBookCode, colBookTitle, colBookAuthor, colBookQty, colAvail);

        // ============================================================
        // pnlFooter
        // ============================================================
        this.pnlFooter.BackColor = System.Drawing.Color.White;
        this.pnlFooter.Controls.AddRange(new Control[] {
            this.lblTotalQty, this.btnRemoveItem, this.btnClearCart, this.btnCreate
        });
        this.pnlFooter.Location = new System.Drawing.Point(20, 645);
        this.pnlFooter.Padding = new Padding(20, 15, 20, 15);
        this.pnlFooter.Size = new System.Drawing.Size(1000, 70);

        this.lblTotalQty.AutoSize = false;
        this.lblTotalQty.Font = new System.Drawing.Font("Segoe UI Semibold", 11F, System.Drawing.FontStyle.Bold);
        this.lblTotalQty.ForeColor = System.Drawing.Color.FromArgb(33, 64, 154);
        this.lblTotalQty.Location = new System.Drawing.Point(20, 22);
        this.lblTotalQty.Size = new System.Drawing.Size(280, 25);
        this.lblTotalQty.Text = "Tổng: 0 cuốn";

        this.btnRemoveItem.BackColor = System.Drawing.Color.White;
        this.btnRemoveItem.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(200, 200, 200);
        this.btnRemoveItem.FlatStyle = FlatStyle.Flat;
        this.btnRemoveItem.Font = new System.Drawing.Font("Segoe UI Semibold", 9F);
        this.btnRemoveItem.ForeColor = System.Drawing.Color.FromArgb(60, 60, 60);
        this.btnRemoveItem.Location = new System.Drawing.Point(340, 18);
        this.btnRemoveItem.Size = new System.Drawing.Size(150, 34);
        this.btnRemoveItem.Text = "🗑 Bỏ dòng đã chọn";
        this.btnRemoveItem.Cursor = Cursors.Hand;
        this.btnRemoveItem.UseVisualStyleBackColor = false;
        this.btnRemoveItem.Click += new EventHandler(this.btnRemoveItem_Click);

        this.btnClearCart.BackColor = System.Drawing.Color.White;
        this.btnClearCart.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(200, 200, 200);
        this.btnClearCart.FlatStyle = FlatStyle.Flat;
        this.btnClearCart.Font = new System.Drawing.Font("Segoe UI Semibold", 9F);
        this.btnClearCart.ForeColor = System.Drawing.Color.FromArgb(60, 60, 60);
        this.btnClearCart.Location = new System.Drawing.Point(498, 18);
        this.btnClearCart.Size = new System.Drawing.Size(120, 34);
        this.btnClearCart.Text = "♻ Xoá giỏ";
        this.btnClearCart.Cursor = Cursors.Hand;
        this.btnClearCart.UseVisualStyleBackColor = false;
        this.btnClearCart.Click += new EventHandler(this.btnClearCart_Click);

        this.btnCreate.BackColor = System.Drawing.Color.FromArgb(33, 64, 154);
        this.btnCreate.FlatAppearance.BorderSize = 0;
        this.btnCreate.FlatStyle = FlatStyle.Flat;
        this.btnCreate.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold);
        this.btnCreate.ForeColor = System.Drawing.Color.White;
        this.btnCreate.Location = new System.Drawing.Point(770, 14);
        this.btnCreate.Size = new System.Drawing.Size(210, 42);
        this.btnCreate.Text = "✓ LẬP PHIẾU MƯỢN";
        this.btnCreate.Cursor = Cursors.Hand;
        this.btnCreate.UseVisualStyleBackColor = false;
        this.btnCreate.Click += new EventHandler(this.btnCreate_Click);

        // ============================================================
        // UserControl
        // ============================================================
        this.BackColor = System.Drawing.Color.FromArgb(245, 247, 251);
        this.Controls.AddRange(new Control[] {
            this.pnlReader, this.pnlReceipt,
            this.pnlBookSearch,
            this.lblCartHeader, this.dgvCart,
            this.pnlFooter
        });
        this.Name = "UcBorrowCreate";
        this.Size = new System.Drawing.Size(1040, 740);

        ((System.ComponentModel.ISupportInitialize)(this.numQty)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.dgvCart)).EndInit();
        this.pnlReader.ResumeLayout(false);
        this.pnlReader.PerformLayout();
        this.pnlReaderInfo.ResumeLayout(false);
        this.pnlReaderInfo.PerformLayout();
        this.pnlReceipt.ResumeLayout(false);
        this.pnlReceipt.PerformLayout();
        this.pnlBookSearch.ResumeLayout(false);
        this.pnlBookSearch.PerformLayout();
        this.pnlFooter.ResumeLayout(false);
        this.ResumeLayout(false);
    }

    #endregion

    // ----------- Fields -----------
    private Panel pnlReader = null!;
    private Label lblReaderHeader = null!;
    private Label lblCardSearch = null!;
    private TextBox txtCardSearch = null!;
    private Button btnLookupReader = null!;
    private Panel pnlReaderInfo = null!;
    private Label lblReaderName = null!;
    private Label lblReaderCard = null!;
    private Label lblReaderStatus = null!;
    private Label lblReaderExpiry = null!;
    private Label lblReaderActive = null!;

    private Panel pnlReceipt = null!;
    private Label lblReceiptHeader = null!;
    private Label lblBorrowDate = null!;
    private DateTimePicker dtpBorrowDate = null!;
    private Label lblDueDate = null!;
    private DateTimePicker dtpDueDate = null!;
    private Label lblNote = null!;
    private TextBox txtNote = null!;

    private Panel pnlBookSearch = null!;
    private Label lblPickBook = null!;
    private Label lblBookKeyword = null!;
    private TextBox txtBookKeyword = null!;
    private ComboBox cboBookCandidates = null!;
    private Label lblQty = null!;
    private NumericUpDown numQty = null!;
    private Button btnAddToCart = null!;

    private Label lblCartHeader = null!;
    private DataGridView dgvCart = null!;

    private Panel pnlFooter = null!;
    private Label lblTotalQty = null!;
    private Button btnRemoveItem = null!;
    private Button btnClearCart = null!;
    private Button btnCreate = null!;

    private ErrorProvider errorProvider = null!;
}
