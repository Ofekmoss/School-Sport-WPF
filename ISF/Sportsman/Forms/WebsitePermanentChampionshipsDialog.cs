using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Sportsman.Forms
{
	public class WebsitePermanentChampionshipsDialog : Form,
		Sport.UI.Controls.IGridSource
	{
		private Sport.UI.Controls.GridControl gridControl;
		private List<DataServices.WebsitePermanentChampionship> websitePermanentChamps = new List<DataServices.WebsitePermanentChampionship>();
		private Button btnClose;
		private Button btnJumpToChamp;
		private Button btnDeletePermanentChamp;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.gridControl = new Sport.UI.Controls.GridControl();
			this.btnClose = new System.Windows.Forms.Button();
			this.btnJumpToChamp = new System.Windows.Forms.Button();
			this.btnDeletePermanentChamp = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// gridControl
			// 
			this.gridControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridControl.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.gridControl.Location = new System.Drawing.Point(10, 10);
			this.gridControl.Name = "gridControl";
			this.gridControl.Size = new System.Drawing.Size(550, 512);
			this.gridControl.TabIndex = 17;
			// 
			// btnClose
			// 
			this.btnClose.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnClose.Location = new System.Drawing.Point(444, 543);
			this.btnClose.Name = "btnClose";
			this.btnClose.Size = new System.Drawing.Size(144, 29);
			this.btnClose.TabIndex = 18;
			this.btnClose.Text = "סגור חלון זה";
			this.btnClose.UseVisualStyleBackColor = true;
			// 
			// btnJumpToChamp
			// 
			this.btnJumpToChamp.BackColor = System.Drawing.Color.Blue;
			this.btnJumpToChamp.Enabled = false;
			this.btnJumpToChamp.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
			this.btnJumpToChamp.ForeColor = System.Drawing.Color.White;
			this.btnJumpToChamp.Location = new System.Drawing.Point(12, 543);
			this.btnJumpToChamp.Name = "btnJumpToChamp";
			this.btnJumpToChamp.Size = new System.Drawing.Size(167, 29);
			this.btnJumpToChamp.TabIndex = 19;
			this.btnJumpToChamp.Text = "עבור לאליפות מסומנת";
			this.btnJumpToChamp.UseVisualStyleBackColor = false;
			this.btnJumpToChamp.Click += new System.EventHandler(this.btnJumpToChamp_Click);
			// 
			// btnDeletePermanentChamp
			// 
			this.btnDeletePermanentChamp.BackColor = System.Drawing.Color.Red;
			this.btnDeletePermanentChamp.Enabled = false;
			this.btnDeletePermanentChamp.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
			this.btnDeletePermanentChamp.ForeColor = System.Drawing.Color.White;
			this.btnDeletePermanentChamp.Location = new System.Drawing.Point(194, 543);
			this.btnDeletePermanentChamp.Name = "btnDeletePermanentChamp";
			this.btnDeletePermanentChamp.Size = new System.Drawing.Size(161, 29);
			this.btnDeletePermanentChamp.TabIndex = 20;
			this.btnDeletePermanentChamp.Text = "בטל אליפות קבועה";
			this.btnDeletePermanentChamp.UseVisualStyleBackColor = false;
			this.btnDeletePermanentChamp.Click += new System.EventHandler(this.btnDeletePermanentChamp_Click);
			// 
			// WebsitePermanentChampionshipsDialog
			// 
			this.AcceptButton = this.btnJumpToChamp;
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnClose;
			this.ClientSize = new System.Drawing.Size(600, 600);
			this.Controls.Add(this.btnDeletePermanentChamp);
			this.Controls.Add(this.btnJumpToChamp);
			this.Controls.Add(this.btnClose);
			this.Controls.Add(this.gridControl);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "WebsitePermanentChampionshipsDialog";
			this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "רשימת אליפויות קבועות באתר";
			this.Load += new System.EventHandler(this.WebsitePermanentChampionshipsDialog_Load);
			this.ResumeLayout(false);

		}

		#endregion

		public WebsitePermanentChampionshipsDialog()
		{
			InitializeComponent();
			gridControl.Grid.Source = this;
			gridControl.Grid.SelectionChanged += new EventHandler(Grid_SelectionChanged);
			gridControl.Grid.DoubleClick += new EventHandler(Grid_DoubleClick);
			ResetGrid();
		}

		void Grid_SelectionChanged(object sender, EventArgs e)
		{
			bool validChamp = TryGetSelectedChamp();
			btnJumpToChamp.Enabled = validChamp;
			btnDeletePermanentChamp.Enabled = validChamp;
		}

		private bool TryGetSelectedChamp(out int champCategoryId, out string champName)
		{
			champCategoryId = -1;
			champName = "";
			if (gridControl.Grid.Selection == null || gridControl.Grid.Selection.Rows == null || gridControl.Grid.Selection.Rows.Length == 0)
				return false;
			int row = gridControl.Grid.Selection.Rows[0];
			champCategoryId = websitePermanentChamps[row].ChampionshipCategoryId;
			if (!TryGetChampName(champCategoryId, out champName))
				return false;
			return true;
		}

		private bool TryGetSelectedChamp()
		{
			int champCategoryId;
			string champName;
			return TryGetSelectedChamp(out champCategoryId, out champName);
		}

		void Grid_DoubleClick(object sender, EventArgs e)
		{
			int champCategoryId;
			string champName;
			if (TryGetSelectedChamp(out champCategoryId, out champName))
			{
				Sport.Championships.Championship championship = null;
				Sport.UI.Dialogs.WaitForm.ShowWait("טוען אליפות '" + champName + "'...", true);
				try
				{
					championship = Sport.Championships.Championship.GetChampionship(champCategoryId);
				}
				catch (Exception ex)
				{
					Sport.UI.Dialogs.WaitForm.HideWait();
					Sport.UI.MessageBox.Error("טעינת אליפות נכשלה\n" + ex.Message + "\n" + ex.StackTrace, "טעינת אליפות");
					return;
				}
				Sport.UI.Dialogs.WaitForm.HideWait();

				if (championship is Sport.Championships.MatchChampionship)
				{
					Sport.UI.ViewManager.OpenView(typeof(Producer.MatchChampionshipEditorView),
						"championshipcategory=" + champCategoryId.ToString());
				}
				else
				{
					Sport.UI.ViewManager.OpenView(typeof(Producer.CompetitionChampionshipEditorView),
						"championshipcategory=" + champCategoryId.ToString());
				}
				this.DialogResult = System.Windows.Forms.DialogResult.OK;
			}
		}



		private void ResetGrid()
		{
			gridControl.Grid.Columns.Clear();
			gridControl.Grid.Columns.Add(0, "אינדקס", 50);
			gridControl.Grid.Columns.Add(1, "כותרת", 250);
			gridControl.Grid.Columns.Add(2, "אליפות", 250);
			gridControl.Grid.RefreshSource();
		}

		private void WebsitePermanentChampionshipsDialog_Load(object sender, EventArgs e)
		{
			SetGridData();
		}

		void Sport.UI.Controls.IGridSource.SetGrid(Sport.UI.Controls.Grid grid)
		{
		}

		int Sport.UI.Controls.IGridSource.GetRowCount()
		{
			return websitePermanentChamps.Count;
		}

		int Sport.UI.Controls.IGridSource.GetFieldCount(int row)
		{
			return 3;
		}

		int Sport.UI.Controls.IGridSource.GetGroup(int row)
		{
			return 0;
		}

		string Sport.UI.Controls.IGridSource.GetText(int row, int field)
		{
			if (row >= 0 && row < websitePermanentChamps.Count)
			{
				DataServices.WebsitePermanentChampionship champ = websitePermanentChamps[row];
				switch (field)
				{
					case (0): // Index
						return champ.Index.ToString();
					case (1): // Title
						return champ.Title;
					case (2): // Championship
						return GetChampName(champ.ChampionshipCategoryId);
				}
			}

			return null;
		}

		public bool TryGetChampName(int champCategoryId, out string champName)
		{
			Sport.Entities.ChampionshipCategory category = null;
			try
			{
				category = new Sport.Entities.ChampionshipCategory(champCategoryId);
			}
			catch
			{
				category = null;
			}

			if (category == null || category.Id != champCategoryId || category.Championship == null || category.Championship.Id < 0)
			{
				champName = "[עונה לא מתאימה]";
				return false;
			}

			champName = category.Championship.Name + " " + category.Name;
			return true;
		}

		public string GetChampName(int champCategoryId)
		{
			string champName;
			TryGetChampName(champCategoryId, out champName);
			return champName;
		}


		Sport.UI.Controls.Style Sport.UI.Controls.IGridSource.GetStyle(int row, int field, Sport.UI.Controls.GridDrawState state)
		{
			return null;
		}

		string Sport.UI.Controls.IGridSource.GetTip(int row)
		{
			return null;
		}

		int[] Sport.UI.Controls.IGridSource.GetSort(int group)
		{
			return null;
		}

		void Sport.UI.Controls.IGridSource.Sort(int group, int[] columns)
		{
		}

		Control Sport.UI.Controls.IGridSource.Edit(int row, int field)
		{
			return null;
		}

		void Sport.UI.Controls.IGridSource.EditEnded(Control control)
		{
		}

		void IDisposable.Dispose()
		{
			
		}

		private void SetGridData()
		{
			websitePermanentChamps.Clear();
			using (DataServices.DataService service = new DataServices.DataService())
			{
				websitePermanentChamps.AddRange(service.GetWebsitePermanentChampionships());
			}
			gridControl.Grid.RefreshSource();
		}

		private void btnJumpToChamp_Click(object sender, EventArgs e)
		{
			Grid_DoubleClick(sender, e);
		}

		private void btnDeletePermanentChamp_Click(object sender, EventArgs e)
		{
			int champCategoryId;
			string champName;
			bool reloadState = false;
			if (TryGetSelectedChamp(out champCategoryId, out champName))
			{
				string message = "האם לבטל הגדרת האליפות '" + champName + "' כקבועה באתר?";
				if (Sport.UI.MessageBox.Ask(message, false))
				{
					using (DataServices.DataService service = new DataServices.DataService())
					{
						service.DeleteWebsitePermanentChampionship(champCategoryId);
						reloadState = true;
						Sport.UI.MessageBox.Success("אליפות קבועה בוטלה", "שמירת נתונים");
					}
				}
			}
			else
			{
				Sport.UI.MessageBox.Error("לא נבחרה אליפות", "שגיאה");
			}

			if (reloadState)
				SetGridData();
		}
	}
}
