using System;

namespace Sportsman.Forms
{
	public class CreateChargesForm : System.Windows.Forms.Form,
		Sport.UI.Controls.IGridSource
	{
		private Sport.UI.Controls.ThemeButton tbCancel;
		private System.Windows.Forms.Label labelRegion;
		private Sport.UI.Controls.NullComboBox cbRegion;
		private Sport.UI.Controls.NullComboBox cbAccount;
		private System.Windows.Forms.Label labelAccount;
		private System.Windows.Forms.Button btnCreateAccount;
		private Sport.UI.Controls.GridControl gridCharges;
		private System.Windows.Forms.Label labelCharges;
		private System.Windows.Forms.Label labelTotal;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox tbSearchAccount;
		private Sport.UI.Controls.ThemeButton tbOk;
	
		private void InitializeComponent()
		{
			this.tbCancel = new Sport.UI.Controls.ThemeButton();
			this.tbOk = new Sport.UI.Controls.ThemeButton();
			this.labelRegion = new System.Windows.Forms.Label();
			this.cbRegion = new Sport.UI.Controls.NullComboBox();
			this.cbAccount = new Sport.UI.Controls.NullComboBox();
			this.labelAccount = new System.Windows.Forms.Label();
			this.btnCreateAccount = new System.Windows.Forms.Button();
			this.gridCharges = new Sport.UI.Controls.GridControl();
			this.labelCharges = new System.Windows.Forms.Label();
			this.labelTotal = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.tbSearchAccount = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// tbCancel
			// 
			this.tbCancel.Alignment = System.Drawing.StringAlignment.Center;
			this.tbCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.tbCancel.AutoSize = false;
			this.tbCancel.Font = new System.Drawing.Font("Arial", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
			this.tbCancel.Hue = 300F;
			this.tbCancel.Image = null;
			this.tbCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.tbCancel.ImageList = null;
			this.tbCancel.ImageSize = new System.Drawing.Size(0, 0);
			this.tbCancel.Location = new System.Drawing.Point(72, 320);
			this.tbCancel.Name = "tbCancel";
			this.tbCancel.Saturation = 0.1F;
			this.tbCancel.Size = new System.Drawing.Size(56, 25);
			this.tbCancel.TabIndex = 11;
			this.tbCancel.Text = "ביטול";
			this.tbCancel.Transparent = System.Drawing.Color.Black;
			this.tbCancel.Click += new System.EventHandler(this.tbCancel_Click);
			// 
			// tbOk
			// 
			this.tbOk.Alignment = System.Drawing.StringAlignment.Center;
			this.tbOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.tbOk.AutoSize = false;
			this.tbOk.Enabled = false;
			this.tbOk.Font = new System.Drawing.Font("Arial", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
			this.tbOk.Hue = 300F;
			this.tbOk.Image = null;
			this.tbOk.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.tbOk.ImageList = null;
			this.tbOk.ImageSize = new System.Drawing.Size(0, 0);
			this.tbOk.Location = new System.Drawing.Point(8, 320);
			this.tbOk.Name = "tbOk";
			this.tbOk.Saturation = 0.1F;
			this.tbOk.Size = new System.Drawing.Size(56, 25);
			this.tbOk.TabIndex = 10;
			this.tbOk.Text = "אישור";
			this.tbOk.Transparent = System.Drawing.Color.Black;
			this.tbOk.Click += new System.EventHandler(this.tbOk_Click);
			// 
			// labelRegion
			// 
			this.labelRegion.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelRegion.Location = new System.Drawing.Point(392, 13);
			this.labelRegion.Name = "labelRegion";
			this.labelRegion.Size = new System.Drawing.Size(40, 16);
			this.labelRegion.TabIndex = 12;
			this.labelRegion.Text = "מחוז:";
			// 
			// cbRegion
			// 
			this.cbRegion.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.cbRegion.Location = new System.Drawing.Point(64, 8);
			this.cbRegion.Name = "cbRegion";
			this.cbRegion.Size = new System.Drawing.Size(320, 22);
			this.cbRegion.Sorted = true;
			this.cbRegion.TabIndex = 13;
			this.cbRegion.SelectedIndexChanged += new System.EventHandler(this.cbRegion_SelectedIndexChanged);
			// 
			// cbAccount
			// 
			this.cbAccount.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.cbAccount.Location = new System.Drawing.Point(64, 36);
			this.cbAccount.Name = "cbAccount";
			this.cbAccount.Size = new System.Drawing.Size(320, 22);
			this.cbAccount.Sorted = true;
			this.cbAccount.TabIndex = 15;
			this.cbAccount.SelectedIndexChanged += new System.EventHandler(this.cbAcount_SelectedIndexChanged);
			// 
			// labelAccount
			// 
			this.labelAccount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelAccount.Location = new System.Drawing.Point(392, 44);
			this.labelAccount.Name = "labelAccount";
			this.labelAccount.Size = new System.Drawing.Size(40, 16);
			this.labelAccount.TabIndex = 14;
			this.labelAccount.Text = "חשבון:";
			// 
			// btnCreateAccount
			// 
			this.btnCreateAccount.Location = new System.Drawing.Point(8, 36);
			this.btnCreateAccount.Name = "btnCreateAccount";
			this.btnCreateAccount.Size = new System.Drawing.Size(56, 21);
			this.btnCreateAccount.TabIndex = 16;
			this.btnCreateAccount.Text = "חדש...";
			this.btnCreateAccount.Click += new System.EventHandler(this.btnCreateAccount_Click);
			// 
			// gridCharges
			// 
			this.gridCharges.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.gridCharges.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.gridCharges.Location = new System.Drawing.Point(8, 120);
			this.gridCharges.Name = "gridCharges";
			this.gridCharges.Size = new System.Drawing.Size(424, 192);
			this.gridCharges.TabIndex = 17;
			// 
			// labelCharges
			// 
			this.labelCharges.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelCharges.Location = new System.Drawing.Point(392, 96);
			this.labelCharges.Name = "labelCharges";
			this.labelCharges.Size = new System.Drawing.Size(40, 16);
			this.labelCharges.TabIndex = 18;
			this.labelCharges.Text = "חיובים:";
			// 
			// labelTotal
			// 
			this.labelTotal.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.labelTotal.Location = new System.Drawing.Point(296, 320);
			this.labelTotal.Name = "labelTotal";
			this.labelTotal.Size = new System.Drawing.Size(136, 23);
			this.labelTotal.TabIndex = 19;
			this.labelTotal.Text = "סה\"כ חיוב:";
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label1.Location = new System.Drawing.Point(328, 64);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(40, 16);
			this.label1.TabIndex = 20;
			this.label1.Text = "חיפוש:";
			// 
			// tbSearchAccount
			// 
			this.tbSearchAccount.Location = new System.Drawing.Point(72, 61);
			this.tbSearchAccount.MaxLength = 150;
			this.tbSearchAccount.Name = "tbSearchAccount";
			this.tbSearchAccount.Size = new System.Drawing.Size(184, 21);
			this.tbSearchAccount.TabIndex = 21;
			this.tbSearchAccount.Text = "";
			this.tbSearchAccount.WordWrap = false;
			this.tbSearchAccount.TextChanged += new System.EventHandler(this.tbSearchAccount_TextChanged);
			// 
			// CreateChargesForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 14);
			this.ClientSize = new System.Drawing.Size(440, 350);
			this.Controls.Add(this.tbSearchAccount);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.labelTotal);
			this.Controls.Add(this.labelCharges);
			this.Controls.Add(this.gridCharges);
			this.Controls.Add(this.btnCreateAccount);
			this.Controls.Add(this.cbAccount);
			this.Controls.Add(this.labelAccount);
			this.Controls.Add(this.cbRegion);
			this.Controls.Add(this.labelRegion);
			this.Controls.Add(this.tbCancel);
			this.Controls.Add(this.tbOk);
			this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.Name = "CreateChargesForm";
			this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "הוספת חיובים";
			this.ResumeLayout(false);

		}
	
		private Sport.Entities.Region _region;
		public new Sport.Entities.Region Region
		{
			get { return _region; }
		}

		private Sport.Entities.Account _account;
		public Sport.Entities.Account Account
		{
			get { return _account; }
		}

		private struct ChargeRow
		{
			public Sport.Entities.Product	Product;
			public int						Amount;
			public double					Price;
			public Sport.Entities.ChampionshipCategory Championship;
		}

		private System.Collections.ArrayList chargeRows;

		public CreateChargesForm(Sport.Entities.Region region, Sport.Entities.Account account)
		{
			InitializeComponent();

			chargeRows = new System.Collections.ArrayList();

			gridCharges.Buttons.Add(Sport.Resources.Images.Plus, "הוסף");
			gridCharges.Buttons.Add(Sport.Resources.Images.Minus, "מחק");
			gridCharges.Buttons[0].Click += new EventHandler(AddChargeClicked);
			gridCharges.Buttons[1].Enabled = false;
			gridCharges.Buttons[1].Click += new EventHandler(RemoveChargeClicked);
			gridCharges.Grid.SelectionChanged += new EventHandler(GridSelectionChanged);

			gridCharges.Grid.Source = this;
			gridCharges.Grid.Columns.Add(0, "סוג חיוב", 100);
			gridCharges.Grid.Columns.Add(1, "כמות", 40);
			gridCharges.Grid.Columns.Add(2, "מחיר", 40);
			gridCharges.Grid.Columns.Add(3, "אליפות", 120);
			
			cbRegion.Items.Clear();
			cbRegion.Items.AddRange(Sport.Entities.Region.Type.GetEntities(null));

			if (region != null)
				cbRegion.SelectedItem = region.Entity;

			Sport.Data.Entity entity = cbRegion.SelectedItem as Sport.Data.Entity;
			_region = entity == null ? null : new Sport.Entities.Region(entity);
			
			SetAccounts();
			
			if (account != null)
				cbAccount.SelectedItem = account;
			
			_account = account;
			tbOk.Enabled = _account != null;
			btnCreateAccount.Enabled = _region != null;
		}

		private void SetAccounts()
		{
			cbAccount.Items.Clear();
			_account = null;
			if (_region != null)
			{
				Sport.Data.EntityFilter filter = 
					new Sport.Data.EntityFilter((int) Sport.Entities.Account.Fields.Region, _region.Id);
				cbAccount.Items.AddRange(Sport.Entities.Account.Type.GetEntities(filter));
			}
		}

		private void tbOk_Click(object sender, System.EventArgs e)
		{
			if (_account != null && _region != null && chargeRows.Count > 0)
			{
				for (int n = 0; n < chargeRows.Count; n++)
				{
					ChargeRow chargeRow = (ChargeRow) chargeRows[n];
					Sport.Entities.Charge charge = new Sport.Entities.Charge(Sport.Entities.Charge.Type.New());
					charge.Region = _region;
					charge.Account = _account;
					charge.Product = chargeRow.Product;
					charge.Amount = chargeRow.Amount;
					charge.Price = chargeRow.Price;
					charge.ChampionshipCategory = chargeRow.Championship;
					Sport.Data.EntityResult result = charge.Save();
					
					if (!result.Succeeded)
					{
						Sport.UI.MessageBox.Show("כשלון בשמירת חיוב '" + charge.Name + "'\n" +
							result.GetMessage(), System.Windows.Forms.MessageBoxIcon.Stop);
					}
				}
			}

			DialogResult = System.Windows.Forms.DialogResult.OK;
		}

		private void tbCancel_Click(object sender, System.EventArgs e)
		{
			DialogResult = System.Windows.Forms.DialogResult.Cancel;
		}

		private void cbRegion_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			Sport.Data.Entity entity = cbRegion.SelectedItem as Sport.Data.Entity;
			_region = entity == null ? null : new Sport.Entities.Region(entity);

			btnCreateAccount.Enabled = _region != null;

			SetAccounts();
			tbOk.Enabled = _account != null;
		}
		
		private void btnCreateAccount_Click(object sender, System.EventArgs e)
		{
			if (_region != null)
			{
				CreateAccountForm caf = new CreateAccountForm(_region);

				if (caf.ShowDialog() == System.Windows.Forms.DialogResult.OK)
				{
					SetAccounts();
					Sport.Entities.Account account = caf.Account;
					cbAccount.SelectedItem = (account == null)?null:account.Entity;
				}
			}
			else
			{
				Sport.UI.MessageBox.Warn("יש לבחור מחוז", "יצירת חשבון");
			}
		}

		private void AddChargeClicked(object sender, EventArgs e)
		{
			Views.ProductsTableView productsView = new Views.ProductsTableView();
			productsView.State[Views.ProductsTableView.SelectionDialog] = "1";
			Sport.UI.EntitySelectionDialog productsSelection = new Sport.UI.EntitySelectionDialog(productsView);

			productsSelection.Multiple = true;

			if (productsSelection.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				Sport.Data.Entity[] entities = productsSelection.Entities;
				ChargeRow chargeRow;
				foreach (Sport.Data.Entity entity in entities)
				{
					chargeRow.Product = new Sport.Entities.Product(entity);
					chargeRow.Amount = 1;
					chargeRow.Price = chargeRow.Product.Price;
					chargeRow.Championship = null;
					chargeRows.Add(chargeRow);
				}
			}

			gridCharges.Grid.RefreshSource();

			ResetTotal();
		}

		private void RemoveChargeClicked(object sender, EventArgs e)
		{
			int[] rows = gridCharges.Grid.Selection.Rows;

			for (int n = rows.Length - 1; n >= 0; n--)
			{
				int index = rows[n];
				if (index >= 0 && index < chargeRows.Count)
					chargeRows.RemoveAt(index);
			}

			gridCharges.Grid.RefreshSource();

			ResetTotal();
		}

		private void GridSelectionChanged(object sender, EventArgs e)
		{
			int[] rows = gridCharges.Grid.Selection.Rows;
			gridCharges.Buttons[1].Enabled = rows.Length > 0;
		}
	
		#region IGridSource Members

		public void SetGrid(Sport.UI.Controls.Grid grid)
		{
		}

		public void Sort(int group, int[] columns)
		{
		}

		public Sport.UI.Controls.Style GetStyle(int row, int field, Sport.UI.Controls.GridDrawState state)
		{
			return null;
		}

		public string GetTip(int row)
		{
			return null;
		}

		public int GetGroup(int row)
		{
			return 0;
		}

		private Sport.UI.Controls.TextControl gridEdit = null;

		private bool settingGridEdit = false;
		private Sport.UI.EntitySelectionDialog championshipDialog;

		public System.Windows.Forms.Control Edit(int row, int field)
		{
			if (field == 0)
				return null;

			settingGridEdit = true;
			if (gridEdit == null)
			{
				gridEdit = new Sport.UI.Controls.TextControl();

				gridEdit.TextChanged += new EventHandler(GridEditTextChanged);
				gridEdit.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
				gridEdit.ShowSpin = true;
			}
			
			if (championshipDialog == null)
			{
				championshipDialog = new Sport.UI.EntitySelectionDialog(
					new Views.ChampionshipCategoryTableView());
				championshipDialog.View.State[Sport.UI.View.SelectionDialog] = "1";
			}
			
			switch (field)
			{
				case (1): // Amount
					gridEdit.Controller = new Sport.UI.Controls.NumberController(0, 1000, 3, 0);
					gridEdit.Value = (double) ((ChargeRow) chargeRows[row]).Amount;
					break;
				case (2): // Price
					gridEdit.Controller = new Sport.UI.Controls.NumberController(-9999, 999999, 7, 2);
					gridEdit.Value = ((ChargeRow) chargeRows[row]).Price;
					break;
				case (3): // Championship
					Sport.UI.Controls.ButtonBox bb = new Sport.UI.Controls.ButtonBox();
					championshipDialog.View.State[Sport.Entities.Region.TypeName] = 
						(cbRegion.SelectedItem == null) ? null : (cbRegion.SelectedItem as Sport.Data.Entity).Id.ToString();
					bb.Value = ((ChargeRow) chargeRows[row]).Championship;
					bb.ValueSelector = new Sport.UI.Controls.ButtonBox.SelectValue(championshipDialog.ValueSelector);
					bb.ValueChanged += new EventHandler(ChampionshipChanged);
					bb.Tag = row;
					settingGridEdit = false;
					return bb;
			}

			settingGridEdit = false;
			return gridEdit;
		}

        public void EditEnded(System.Windows.Forms.Control control)
		{
		}

		public int GetRowCount()
		{
			return chargeRows.Count;
		}

		public int[] GetSort(int group)
		{
			return null;
		}

		public string GetText(int row, int field)
		{
			ChargeRow chargeRow = (ChargeRow) chargeRows[row];

			switch (field)
			{
				case (0): // Product
					return chargeRow.Product.Name;
				case (1): // Amount
					return chargeRow.Amount.ToString();
				case (2): // Price
					return chargeRow.Price.ToString();
				case (3): // Championship
					return (chargeRow.Championship == null) ? null : chargeRow.Championship.Name;
			}

			return null;
		}

		public int GetFieldCount(int row)
		{
			return (3+1);
		}

		#endregion

		private void GridEditTextChanged(object sender, EventArgs e)
		{
			if (settingGridEdit)
				return;
			
			if ((chargeRows == null)||(gridCharges == null)||(gridCharges.Grid == null))
				return;
			
			System.Drawing.Point point = gridCharges.Grid.EditCell;
			int Y = point.Y;
			int X = point.X;
			
			if ((Y < 0)||(Y >= chargeRows.Count))
				return;
			
			if (chargeRows[Y] == null)
				return;
			
			ChargeRow chargeRow = (ChargeRow) chargeRows[Y];
			
			double value = 0;
			if ((gridEdit != null)&&(gridEdit.Value != null))
				value = (double) gridEdit.Value;
			
			if (X == 1) // Amount
			{
				chargeRow.Amount = (int) value;
			}
			else // Price
			{
				chargeRow.Price = value;
			}
			
			/*
			switch (X)
			{
				case (1): // Amount
					chargeRow.Amount = (int) value;
					break;
				case (2): // Price
					chargeRow.Price = value;
					break;
				case (3): //Championship
					chargeRow.Championship = 
			}
			*/
			
			chargeRows[Y] = chargeRow;
			ResetTotal();
		}

		private void ResetTotal()
		{
			double total = 0;
			foreach (ChargeRow chargeRow in chargeRows)
			{
				total += chargeRow.Amount * chargeRow.Price;
			}

			labelTotal.Text = "סה\"כ חיוב: " + total.ToString();
		}

		private void cbAcount_SelectedIndexChanged(object sender, EventArgs e)
		{
			Sport.Data.Entity selAccount = cbAccount.SelectedItem as Sport.Data.Entity;
			_account = (selAccount == null)?null:new Sport.Entities.Account(selAccount);
			tbOk.Enabled = _account != null;
		}

		private void tbSearchAccount_TextChanged(object sender, System.EventArgs e)
		{
			string strAccountName = tbSearchAccount.Text;
			if ((strAccountName != null)&&(strAccountName.Length > 0))
			{
				bool blnFound = false;
				foreach (Sport.Data.Entity entAccount in cbAccount.Items)
				{
					if (entAccount.Name.StartsWith(strAccountName))
					{
						blnFound = true;
						cbAccount.SelectedItem = entAccount;
						break;
					}
				}
				
				if (!blnFound && strAccountName.Length >= 3)
				{
					foreach (Sport.Data.Entity entAccount in cbAccount.Items)
					{
						if (entAccount.Name.IndexOf(strAccountName) >= 0)
						{
							cbAccount.SelectedItem = entAccount;
							break;
						}
					}
				}
			}
		}
		
		private void ChampionshipChanged(object sender, EventArgs e)
		{
			Sport.UI.Controls.ButtonBox bb = sender as Sport.UI.Controls.ButtonBox;
			Sport.Data.Entity champEnt = bb.Value as Sport.Data.Entity;
			Sport.Entities.ChampionshipCategory category = null;
			if (champEnt != null)
				category = new Sport.Entities.ChampionshipCategory(champEnt);
			int row = (int) bb.Tag;
			ChargeRow chargeRow = (ChargeRow) chargeRows[row];
			chargeRow.Championship = category;
			chargeRows[row] = chargeRow;
		}
	}
}
