using System;
using System.Windows.Forms;
using System.Drawing.Printing;

namespace Sport.UI.Dialogs
{
	public class PrintSettingsForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.GroupBox gbPrinter;
		private System.Windows.Forms.ComboBox cbName;
		private System.Windows.Forms.Button btnOk;
		private System.Windows.Forms.GroupBox gbOrientation;
		private System.Windows.Forms.RadioButton rbPortrait;
		private System.Windows.Forms.RadioButton rbLandscape;
		private System.Windows.Forms.GroupBox gbPageSize;
		private System.Windows.Forms.ComboBox cbPageSize;
		private System.Windows.Forms.TextBox tbWidth;
		private System.Windows.Forms.Label labelWidth;
		private System.Windows.Forms.Label labelHeight;
		private System.Windows.Forms.TextBox tbHeight;
		private System.Windows.Forms.PictureBox pbOrientation;
		private System.Windows.Forms.ImageList imageList;
		private System.ComponentModel.IContainer components;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.CheckBox cbPreview;

		private System.Collections.Hashtable paperSizes;
		private System.Windows.Forms.RadioButton rbCM;
		private System.Windows.Forms.RadioButton rbInch;

		private bool _landscape=false;
		public bool Landscape
		{
			get {return _landscape;}
			set {_landscape = value;}
		}
		
		private bool _portrait=false;
		private System.Windows.Forms.Button btnCustomText;
		private System.Windows.Forms.CheckBox chkForceRegion;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.NumericUpDown nudCopies;

		private double? _overrideWidth_cm = null;
		private double? _overrideHeight_cm = null;
	
		public bool Portrait
		{
			get {return _portrait;}
			set {_portrait = value;}
		}
		
		private PrinterSettings _settings;
		public PrinterSettings Settings
		{
			get { return _settings; }
		}

		public bool ShowPreview
		{
			get { return cbPreview.Checked; }
			set { cbPreview.Checked = value; }
		}

		public PrintSettingsForm(PrinterSettings settings, double? overrideWidth_cm, double? overrideHeight_cm)
		{
			paperSizes = new System.Collections.Hashtable();
			_settings = settings;
			//_overrideWidth_cm = overrideWidth_cm;
			//_overrideHeight_cm = overrideHeight_cm;
			InitializeComponent();
			cbName.Items.AddRange(Sport.Core.Utils.GetInstalledPrinters().ToArray());

			//System.Configuration.ConfigurationSettings.AppSettings.Add("LastPrinter", "testing");

			cbName.SelectedItem = settings.PrinterName;
			SetPageSizes();
			SetSettings();
		}

		public PrintSettingsForm(PrinterSettings settings)
			: this(settings, null, null)
		{
		}

		private class PaperItem
		{
			private PaperKind _kind = PaperKind.Custom;
			public PaperKind Kind
			{
				get { return _kind; }
			}

			private string _name = "ללא הגדרה";
			public string Name
			{
				get { return _name; }
			}

			public PaperItem(PaperKind kind, string name)
			{
				_kind = kind;
				_name = name;
			}

			public PaperItem(PaperSize paperSize)
			{
				if (paperSize != null)
				{
					_kind = paperSize.Kind;
					_name = paperSize.PaperName;
				}
			}

			public override bool Equals(object obj)
			{
				PaperItem pi = obj as PaperItem;
				if (pi != null)
				{
					if (_kind == pi._kind)
					{
						if (_kind != PaperKind.Custom ||
							_name == pi._name)
							return true;
					}
				}
				else if (obj is PaperKind)
				{
					return _kind != PaperKind.Custom && _kind == (PaperKind) obj;
				}
                
				return false;
			}

			public override int GetHashCode()
			{
				return (int) _kind;
			}

			public override string ToString()
			{
				return _name;
			}
		}

		private void SetPageSizes()
		{
			cbPageSize.Items.Clear();
			paperSizes.Clear();

			if (_settings.PaperSizes != null)
			{
				foreach (PaperSize ps in _settings.PaperSizes)
				{
					PaperItem item = new PaperItem(ps);
					paperSizes[item] = ps;
					cbPageSize.Items.Add(item);
				}
			}
		}

		private System.Drawing.Printing.PaperSize GetKindPaperSize(PaperKind kind)
		{
			foreach (PaperSize ps in _settings.PaperSizes)
			{
				if (ps.Kind == kind)
					return ps;
			}

			return null;
		}

		private int GetCopiesCount()
		{
			int copies = 1;
			try
			{
				copies = (int) _settings.Copies;
			}
			catch
			{
				copies = 1;
			}
			return copies;
		}

		private PaperSize GetDefaultSize()
		{
			if (_settings.PaperSizes != null && _settings.PaperSizes.Count > 0)
				return _settings.PaperSizes[0];
			return new PaperSize("A4", 827, 1169);
		}

		private PaperSize GetPaperSize(PageSettings page)
		{
			try
			{
				return page.PaperSize;
			}
			catch
			{
				return GetDefaultSize();
			}
		}

		private void SetSettings()
		{
			nudCopies.Value = Math.Max(GetCopiesCount(), (int) 1);
			PageSettings page = _settings.DefaultPageSettings;

            cbPageSize.SelectedItem = new PaperItem(GetPaperSize(page));
			if (cbPageSize.SelectedItem == null)
			{
				PaperSize ps = GetKindPaperSize(PaperKind.A4);
				if (ps == null)
				{
					page.PaperSize = GetDefaultSize();
				}
				else
				{
					page.PaperSize = ps;
				}
				cbPageSize.SelectedItem = new PaperItem(ps);
			}

			double width, height;

			if ((_overrideWidth_cm.HasValue && _overrideWidth_cm.Value > 0) ||
				(_overrideHeight_cm.HasValue && _overrideHeight_cm.Value > 0))
			{
				rbCM.Checked = true;
			}

			if (rbCM.Checked)
			{
				width = System.Math.Round((double) GetPaperSize(page).Width / 100 * 2.56, 2); // Converting to cm
				height = System.Math.Round((double)GetPaperSize(page).Height / 100 * 2.56, 2); // Converting to cm
			}
			else
			{
				width = (double) GetPaperSize(page).Width / 100;
				height = (double) GetPaperSize(page).Height / 100;
			}

			//rbLandscape.Checked = true;
			pbOrientation.Image = imageList.Images[1];
			tbHeight.Text = width.ToString();
			tbWidth.Text = height.ToString();

			if (page.Landscape)
			{
				rbLandscape.Checked = true;
				pbOrientation.Image = imageList.Images[1];
				tbHeight.Text = width.ToString();
				tbWidth.Text = height.ToString();
			}
			else
			{
				rbPortrait.Checked = true;
				pbOrientation.Image = imageList.Images[0];
				tbWidth.Text = width.ToString();
				tbHeight.Text = height.ToString();
			}

			if (_overrideWidth_cm.HasValue && _overrideWidth_cm.Value > 0)
			{
				tbWidth.Text = _overrideWidth_cm.Value.ToString();
				PageSizeChanged(false);
			}

			if (_overrideHeight_cm.HasValue && _overrideHeight_cm.Value > 0)
			{
				tbHeight.Text = _overrideHeight_cm.Value.ToString();
				PageSizeChanged(false);
			}
		}

		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PrintSettingsForm));
			this.gbPrinter = new System.Windows.Forms.GroupBox();
			this.cbName = new System.Windows.Forms.ComboBox();
			this.btnOk = new System.Windows.Forms.Button();
			this.gbOrientation = new System.Windows.Forms.GroupBox();
			this.pbOrientation = new System.Windows.Forms.PictureBox();
			this.rbLandscape = new System.Windows.Forms.RadioButton();
			this.rbPortrait = new System.Windows.Forms.RadioButton();
			this.gbPageSize = new System.Windows.Forms.GroupBox();
			this.rbInch = new System.Windows.Forms.RadioButton();
			this.rbCM = new System.Windows.Forms.RadioButton();
			this.labelHeight = new System.Windows.Forms.Label();
			this.tbHeight = new System.Windows.Forms.TextBox();
			this.labelWidth = new System.Windows.Forms.Label();
			this.tbWidth = new System.Windows.Forms.TextBox();
			this.cbPageSize = new System.Windows.Forms.ComboBox();
			this.imageList = new System.Windows.Forms.ImageList(this.components);
			this.btnCancel = new System.Windows.Forms.Button();
			this.cbPreview = new System.Windows.Forms.CheckBox();
			this.btnCustomText = new System.Windows.Forms.Button();
			this.chkForceRegion = new System.Windows.Forms.CheckBox();
			this.label1 = new System.Windows.Forms.Label();
			this.nudCopies = new System.Windows.Forms.NumericUpDown();
			this.gbPrinter.SuspendLayout();
			this.gbOrientation.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pbOrientation)).BeginInit();
			this.gbPageSize.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudCopies)).BeginInit();
			this.SuspendLayout();
			// 
			// gbPrinter
			// 
			this.gbPrinter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gbPrinter.Controls.Add(this.cbName);
			this.gbPrinter.Location = new System.Drawing.Point(8, 5);
			this.gbPrinter.Name = "gbPrinter";
			this.gbPrinter.Size = new System.Drawing.Size(328, 48);
			this.gbPrinter.TabIndex = 0;
			this.gbPrinter.TabStop = false;
			this.gbPrinter.Text = "מדפסת";
			// 
			// cbName
			// 
			this.cbName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.cbName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbName.Location = new System.Drawing.Point(8, 23);
			this.cbName.Name = "cbName";
			this.cbName.Size = new System.Drawing.Size(312, 21);
			this.cbName.TabIndex = 1;
			this.cbName.SelectedIndexChanged += new System.EventHandler(this.cbName_SelectedIndexChanged);
			// 
			// btnOk
			// 
			this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnOk.Location = new System.Drawing.Point(8, 194);
			this.btnOk.Name = "btnOk";
			this.btnOk.Size = new System.Drawing.Size(75, 23);
			this.btnOk.TabIndex = 1;
			this.btnOk.Text = "אישור";
			this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
			// 
			// gbOrientation
			// 
			this.gbOrientation.Controls.Add(this.pbOrientation);
			this.gbOrientation.Controls.Add(this.rbLandscape);
			this.gbOrientation.Controls.Add(this.rbPortrait);
			this.gbOrientation.Location = new System.Drawing.Point(8, 55);
			this.gbOrientation.Name = "gbOrientation";
			this.gbOrientation.Size = new System.Drawing.Size(152, 80);
			this.gbOrientation.TabIndex = 2;
			this.gbOrientation.TabStop = false;
			this.gbOrientation.Text = "תצוגה";
			// 
			// pbOrientation
			// 
			this.pbOrientation.Location = new System.Drawing.Point(15, 40);
			this.pbOrientation.Name = "pbOrientation";
			this.pbOrientation.Size = new System.Drawing.Size(32, 32);
			this.pbOrientation.TabIndex = 2;
			this.pbOrientation.TabStop = false;
			// 
			// rbLandscape
			// 
			this.rbLandscape.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.rbLandscape.Checked = true;
			this.rbLandscape.Location = new System.Drawing.Point(80, 49);
			this.rbLandscape.Name = "rbLandscape";
			this.rbLandscape.Size = new System.Drawing.Size(64, 24);
			this.rbLandscape.TabIndex = 1;
			this.rbLandscape.TabStop = true;
			this.rbLandscape.Text = "אופקית";
			this.rbLandscape.CheckedChanged += new System.EventHandler(this.OrientationChanged);
			// 
			// rbPortrait
			// 
			this.rbPortrait.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.rbPortrait.Location = new System.Drawing.Point(80, 24);
			this.rbPortrait.Name = "rbPortrait";
			this.rbPortrait.Size = new System.Drawing.Size(64, 24);
			this.rbPortrait.TabIndex = 0;
			this.rbPortrait.Text = "אנכית";
			this.rbPortrait.CheckedChanged += new System.EventHandler(this.OrientationChanged);
			// 
			// gbPageSize
			// 
			this.gbPageSize.Controls.Add(this.rbInch);
			this.gbPageSize.Controls.Add(this.rbCM);
			this.gbPageSize.Controls.Add(this.labelHeight);
			this.gbPageSize.Controls.Add(this.tbHeight);
			this.gbPageSize.Controls.Add(this.labelWidth);
			this.gbPageSize.Controls.Add(this.tbWidth);
			this.gbPageSize.Controls.Add(this.cbPageSize);
			this.gbPageSize.Location = new System.Drawing.Point(168, 55);
			this.gbPageSize.Name = "gbPageSize";
			this.gbPageSize.Size = new System.Drawing.Size(168, 112);
			this.gbPageSize.TabIndex = 3;
			this.gbPageSize.TabStop = false;
			this.gbPageSize.Text = "גודל דף";
			// 
			// rbInch
			// 
			this.rbInch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.rbInch.Location = new System.Drawing.Point(60, 48);
			this.rbInch.Name = "rbInch";
			this.rbInch.Size = new System.Drawing.Size(48, 16);
			this.rbInch.TabIndex = 6;
			this.rbInch.Text = "אינץ\'";
			this.rbInch.CheckedChanged += new System.EventHandler(this.SizeTypeChanged);
			// 
			// rbCM
			// 
			this.rbCM.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.rbCM.Checked = true;
			this.rbCM.Location = new System.Drawing.Point(112, 48);
			this.rbCM.Name = "rbCM";
			this.rbCM.Size = new System.Drawing.Size(48, 16);
			this.rbCM.TabIndex = 5;
			this.rbCM.TabStop = true;
			this.rbCM.Text = "ס\"מ";
			this.rbCM.CheckedChanged += new System.EventHandler(this.SizeTypeChanged);
			// 
			// labelHeight
			// 
			this.labelHeight.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelHeight.Location = new System.Drawing.Point(120, 99);
			this.labelHeight.Name = "labelHeight";
			this.labelHeight.Size = new System.Drawing.Size(40, 16);
			this.labelHeight.TabIndex = 4;
			this.labelHeight.Text = "גובה:";
			// 
			// tbHeight
			// 
			this.tbHeight.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tbHeight.Location = new System.Drawing.Point(8, 85);
			this.tbHeight.Name = "tbHeight";
			this.tbHeight.Size = new System.Drawing.Size(104, 21);
			this.tbHeight.TabIndex = 3;
			this.tbHeight.Text = "textBox2";
			this.tbHeight.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.SizeKeyPress);
			this.tbHeight.Validated += new System.EventHandler(this.SizeValidated);
			// 
			// labelWidth
			// 
			this.labelWidth.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelWidth.Location = new System.Drawing.Point(120, 75);
			this.labelWidth.Name = "labelWidth";
			this.labelWidth.Size = new System.Drawing.Size(40, 16);
			this.labelWidth.TabIndex = 2;
			this.labelWidth.Text = "רוחב:";
			// 
			// tbWidth
			// 
			this.tbWidth.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tbWidth.Location = new System.Drawing.Point(8, 67);
			this.tbWidth.Name = "tbWidth";
			this.tbWidth.Size = new System.Drawing.Size(104, 21);
			this.tbWidth.TabIndex = 1;
			this.tbWidth.Text = "textBox1";
			this.tbWidth.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.SizeKeyPress);
			this.tbWidth.Validated += new System.EventHandler(this.SizeValidated);
			// 
			// cbPageSize
			// 
			this.cbPageSize.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.cbPageSize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbPageSize.Location = new System.Drawing.Point(8, 24);
			this.cbPageSize.Name = "cbPageSize";
			this.cbPageSize.Size = new System.Drawing.Size(152, 21);
			this.cbPageSize.TabIndex = 0;
			this.cbPageSize.SelectedIndexChanged += new System.EventHandler(this.cbPageSize_SelectedIndexChanged);
			// 
			// imageList
			// 
			this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
			this.imageList.TransparentColor = System.Drawing.Color.Blue;
			this.imageList.Images.SetKeyName(0, "");
			this.imageList.Images.SetKeyName(1, "");
			// 
			// btnCancel
			// 
			this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(96, 194);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75, 23);
			this.btnCancel.TabIndex = 4;
			this.btnCancel.Text = "ביטול";
			// 
			// cbPreview
			// 
			this.cbPreview.Checked = true;
			this.cbPreview.CheckState = System.Windows.Forms.CheckState.Checked;
			this.cbPreview.Location = new System.Drawing.Point(48, 170);
			this.cbPreview.Name = "cbPreview";
			this.cbPreview.Size = new System.Drawing.Size(112, 16);
			this.cbPreview.TabIndex = 5;
			this.cbPreview.Text = "תצוגה מקדימה";
			// 
			// btnCustomText
			// 
			this.btnCustomText.BackColor = System.Drawing.Color.White;
			this.btnCustomText.ForeColor = System.Drawing.Color.Blue;
			this.btnCustomText.Location = new System.Drawing.Point(216, 195);
			this.btnCustomText.Name = "btnCustomText";
			this.btnCustomText.Size = new System.Drawing.Size(120, 23);
			this.btnCustomText.TabIndex = 6;
			this.btnCustomText.Text = "עריכת טקסט חופשי";
			this.btnCustomText.UseVisualStyleBackColor = false;
			this.btnCustomText.Visible = false;
			this.btnCustomText.Click += new System.EventHandler(this.btnCustomText_Click);
			// 
			// chkForceRegion
			// 
			this.chkForceRegion.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.chkForceRegion.Location = new System.Drawing.Point(192, 195);
			this.chkForceRegion.Name = "chkForceRegion";
			this.chkForceRegion.Size = new System.Drawing.Size(136, 16);
			this.chkForceRegion.TabIndex = 7;
			this.chkForceRegion.Text = "אלץ מחוז משתמש";
			this.chkForceRegion.Visible = false;
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label1.Location = new System.Drawing.Point(80, 144);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(80, 16);
			this.label1.TabIndex = 8;
			this.label1.Text = "מספר עותקים:";
			// 
			// nudCopies
			// 
			this.nudCopies.Location = new System.Drawing.Point(32, 141);
			this.nudCopies.Maximum = new decimal(new int[] {
            99,
            0,
            0,
            0});
			this.nudCopies.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.nudCopies.Name = "nudCopies";
			this.nudCopies.Size = new System.Drawing.Size(40, 21);
			this.nudCopies.TabIndex = 9;
			this.nudCopies.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.nudCopies.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.nudCopies.ValueChanged += new System.EventHandler(this.nudCopies_ValueChanged);
			// 
			// PrintSettingsForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 14);
			this.ClientSize = new System.Drawing.Size(344, 224);
			this.Controls.Add(this.nudCopies);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.chkForceRegion);
			this.Controls.Add(this.btnCustomText);
			this.Controls.Add(this.cbPreview);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.gbPageSize);
			this.Controls.Add(this.gbOrientation);
			this.Controls.Add(this.btnOk);
			this.Controls.Add(this.gbPrinter);
			this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "PrintSettingsForm";
			this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "הדפסה";
			this.Load += new System.EventHandler(this.PrintSettingsForm_Load);
			this.gbPrinter.ResumeLayout(false);
			this.gbOrientation.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pbOrientation)).EndInit();
			this.gbPageSize.ResumeLayout(false);
			this.gbPageSize.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudCopies)).EndInit();
			this.ResumeLayout(false);

		}

		private void OrientationChanged(object sender, System.EventArgs e)
		{
			if (rbLandscape.Checked)
			{
				_settings.DefaultPageSettings.Landscape = true;
			}
			else
			{
				_settings.DefaultPageSettings.Landscape = false;
			}

			SetSettings();
		}

		private void cbName_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			_settings.PrinterName = (string) cbName.SelectedItem;

			SetPageSizes();
			SetSettings();
		}

		private void cbPageSize_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			PaperSize ps = paperSizes[cbPageSize.SelectedItem] as PaperSize;
			if (ps != null)
				_settings.DefaultPageSettings.PaperSize = ps;

			SetSettings();
		}

		private void PageSizeChanged(bool setSettings)
		{
			PageSettings page = _settings.DefaultPageSettings;
			int width = GetPaperSize(page).Width;
			int height = GetPaperSize(page).Height;

			System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex("\\d*(\\.\\d*)?");
			string str = tbWidth.Text;
			System.Text.RegularExpressions.Match match = regex.Match(str);
			if (match.Success && match.Length > 0)
			{
				str = str.Substring(0, match.Length);
				double val = Double.Parse(str);
				val = val * 100;
				if (rbCM.Checked)
					val = val / 2.56;
				if (page.Landscape)
					height = (int) val;
				else
					width = (int) val;
			}

			str = tbHeight.Text;
			match = regex.Match(str);
			if (match.Success && match.Length > 0)
			{
				str = str.Substring(0, match.Length);
				double val = Double.Parse(str);
				val = val * 100;
				if (rbCM.Checked)
					val = val / 2.56;
				if (page.Landscape)
					width = (int) val;
				else
					height = (int) val;
			}

			page.PaperSize = new PaperSize(page.PaperSize.PaperName, width, height);

			if (setSettings)
				SetSettings();
		}

		private void PageSizeChanged()
		{
			PageSizeChanged(true);
		}

		private void SizeKeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
		{
			if (e.KeyChar == (char) System.Windows.Forms.Keys.Enter)
			{
				PageSizeChanged();
			}
		}

		private void SizeValidated(object sender, System.EventArgs e)
		{
			PageSizeChanged();
	
		}

		private void SizeTypeChanged(object sender, System.EventArgs e)
		{
			SetSettings();
		}

		private void PrintSettingsForm_Load(object sender, System.EventArgs e)
		{
			rbLandscape.Checked = true;
			_landscape = true;
			_portrait = false;

			/*
			//read config file:
			string strOrientation=Sport.Core.Configuration.ReadString("Printing", "Orientation");
			if (strOrientation != null)
			{
				if (strOrientation == "landscape")
				{
					rbLandscape.Checked = true;
				}
				else
				{
					if (strOrientation == "portrait")
						rbPortrait.Checked = true;
				}
			}
			
			
			if (_landscape)
				rbLandscape.Checked = true;
			
			if (_portrait)
				rbPortrait.Checked = true;
			*/
			
			//read config file:
			string strPrinterName=Sport.Core.Configuration.ReadString("Printing", "PrinterName");
			if (strPrinterName != null)
			{
				cbName.SelectedItem = strPrinterName;
			}
			
			if ((_customTextKey != null)&&(_customTextKey.Length > 0))
				btnCustomText.Visible = true;
			
			if (_allowRegionChange)
				chkForceRegion.Visible = true;
		}

		private void btnOk_Click(object sender, EventArgs e)
		{
			//save to config file:
			string strOrientation="";
			if (rbLandscape.Checked)
				strOrientation = "landscape";
			if (rbPortrait.Checked)
				strOrientation = "portrait";
			if (Sport.Core.Configuration.ReadString("Printing", "Orientation") != strOrientation)
				Sport.Core.Configuration.WriteString("Printing", "Orientation", strOrientation);
			string strPrinterName="";
			if (cbName.SelectedIndex >= 0)
				strPrinterName = cbName.Items[cbName.SelectedIndex].ToString();
			if (Sport.Core.Configuration.ReadString("Printing", "PrinterName") != strPrinterName)
				Sport.Core.Configuration.WriteString("Printing", "PrinterName", strPrinterName);
			this.DialogResult = System.Windows.Forms.DialogResult.OK;
		}
		
		private string _customTextKey="";
		public string CustomTextKey
		{
			get {return _customTextKey;}
			set {_customTextKey = value;}
		}

		private bool _allowRegionChange=false;
		public bool AllowRegionChange
		{
			get {return _allowRegionChange;}
			set {_allowRegionChange = value;}
		}
		public bool ForceRegionChecked
		{
			get {return chkForceRegion.Checked;}
		}
		
		private void btnCustomText_Click(object sender, System.EventArgs e)
		{
			if ((_customTextKey == null)||(_customTextKey.Length == 0))
				return;
			string strCurText=Core.Configuration.ReadString("CustomText", _customTextKey);
			if (strCurText != null)
				strCurText = strCurText.Replace("|", "\n");
			InputTextForm objForm=new InputTextForm(strCurText);
			if (objForm.ShowDialog(this) == DialogResult.OK)
			{
				string strText=objForm.GetText();
				strText = strText.Replace("\n\r", "\n");
				strText = strText.Replace("\r\n", "\n");
				strText = strText.Replace("\n", "|");
				Core.Configuration.WriteString("CustomText", _customTextKey, strText);
			}
		}

		private void nudCopies_ValueChanged(object sender, System.EventArgs e)
		{
			_settings.Copies = (short) ((double) nudCopies.Value);
		}
	}
}
