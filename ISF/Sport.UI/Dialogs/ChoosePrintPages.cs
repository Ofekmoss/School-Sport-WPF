using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace Sport.UI.Dialogs
{
	/// <summary>
	/// Summary description for ChoosePrintPages.
	/// </summary>
	public class ChoosePrintPagesForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ListBox lbAvailablePages;
		private System.Windows.Forms.ListBox lbPagesToPrint;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button btnAddPrint;
		private System.Windows.Forms.Button btnRemovePrint;
		private System.Windows.Forms.Button btnPrintAll;
		private System.Windows.Forms.Button btnRemoveAll;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button btnConfirm;

		private int[] _availablePages=null;
		private int[] _pagesToPrint=null;
		
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public ChoosePrintPagesForm(int[] pagesToPrint, int pageCount)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			_pagesToPrint = pagesToPrint;
			ArrayList arrAvailablePages=new ArrayList();
			ArrayList arrPagesToPrint=new ArrayList(pagesToPrint);
			for (int n=0; n<pageCount; n++)
			{
				if (arrPagesToPrint.IndexOf(n) < 0)
					arrAvailablePages.Add(n);
			}
			_availablePages = (int[]) arrAvailablePages.ToArray(typeof(int));
		}
		
		public int[] PagesToPrint
		{
			get
			{
				ArrayList result=new ArrayList();
				for (int i=0; i<lbPagesToPrint.Items.Count; i++)
				{
					string strPage=lbPagesToPrint.Items[i].ToString();
					int iPage=Int32.Parse(strPage)-1;
					result.Add(iPage);
				}
				return (int[]) result.ToArray(typeof(int));
			}
		}
		
		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.label1 = new System.Windows.Forms.Label();
			this.lbAvailablePages = new System.Windows.Forms.ListBox();
			this.lbPagesToPrint = new System.Windows.Forms.ListBox();
			this.label2 = new System.Windows.Forms.Label();
			this.btnAddPrint = new System.Windows.Forms.Button();
			this.btnRemovePrint = new System.Windows.Forms.Button();
			this.btnPrintAll = new System.Windows.Forms.Button();
			this.btnRemoveAll = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.btnConfirm = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(232, 16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(104, 23);
			this.label1.TabIndex = 0;
			this.label1.Text = "עמודים זמינים";
			// 
			// lbAvailablePages
			// 
			this.lbAvailablePages.Font = new System.Drawing.Font("Tahoma", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.lbAvailablePages.ForeColor = System.Drawing.Color.Blue;
			this.lbAvailablePages.ItemHeight = 23;
			this.lbAvailablePages.Location = new System.Drawing.Point(264, 40);
			this.lbAvailablePages.Name = "lbAvailablePages";
			this.lbAvailablePages.Size = new System.Drawing.Size(64, 211);
			this.lbAvailablePages.TabIndex = 1;
			this.lbAvailablePages.DoubleClick += new System.EventHandler(this.btnAddPrint_Click);
			this.lbAvailablePages.SelectedIndexChanged += new System.EventHandler(this.lbAvailablePages_SelectedIndexChanged);
			// 
			// lbPagesToPrint
			// 
			this.lbPagesToPrint.Font = new System.Drawing.Font("Tahoma", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.lbPagesToPrint.ForeColor = System.Drawing.Color.Blue;
			this.lbPagesToPrint.ItemHeight = 23;
			this.lbPagesToPrint.Location = new System.Drawing.Point(24, 40);
			this.lbPagesToPrint.Name = "lbPagesToPrint";
			this.lbPagesToPrint.Size = new System.Drawing.Size(64, 211);
			this.lbPagesToPrint.TabIndex = 3;
			this.lbPagesToPrint.DoubleClick += new System.EventHandler(this.btnRemovePrint_Click);
			this.lbPagesToPrint.SelectedIndexChanged += new System.EventHandler(this.lbAvailablePages_SelectedIndexChanged);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(8, 16);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(120, 23);
			this.label2.TabIndex = 2;
			this.label2.Text = "עמודים להדפסה";
			// 
			// btnAddPrint
			// 
			this.btnAddPrint.Enabled = false;
			this.btnAddPrint.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.btnAddPrint.Location = new System.Drawing.Point(96, 56);
			this.btnAddPrint.Name = "btnAddPrint";
			this.btnAddPrint.Size = new System.Drawing.Size(160, 23);
			this.btnAddPrint.TabIndex = 4;
			this.btnAddPrint.Text = "שלח עמוד זה להדפסה>>";
			this.btnAddPrint.Click += new System.EventHandler(this.btnAddPrint_Click);
			// 
			// btnRemovePrint
			// 
			this.btnRemovePrint.Enabled = false;
			this.btnRemovePrint.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.btnRemovePrint.Location = new System.Drawing.Point(96, 88);
			this.btnRemovePrint.Name = "btnRemovePrint";
			this.btnRemovePrint.Size = new System.Drawing.Size(160, 23);
			this.btnRemovePrint.TabIndex = 5;
			this.btnRemovePrint.Text = "<<לא להדפיס עמוד זה";
			this.btnRemovePrint.Click += new System.EventHandler(this.btnRemovePrint_Click);
			// 
			// btnPrintAll
			// 
			this.btnPrintAll.Font = new System.Drawing.Font("Tahoma", 7.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.btnPrintAll.Location = new System.Drawing.Point(248, 256);
			this.btnPrintAll.Name = "btnPrintAll";
			this.btnPrintAll.Size = new System.Drawing.Size(96, 23);
			this.btnPrintAll.TabIndex = 6;
			this.btnPrintAll.Text = "[שלח את כולם]";
			this.btnPrintAll.Click += new System.EventHandler(this.btnPrintAll_Click);
			// 
			// btnRemoveAll
			// 
			this.btnRemoveAll.Font = new System.Drawing.Font("Tahoma", 7.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.btnRemoveAll.Location = new System.Drawing.Point(8, 256);
			this.btnRemoveAll.Name = "btnRemoveAll";
			this.btnRemoveAll.Size = new System.Drawing.Size(96, 23);
			this.btnRemoveAll.TabIndex = 7;
			this.btnRemoveAll.Text = "[שלח את כולם]";
			this.btnRemoveAll.Click += new System.EventHandler(this.btnRemoveAll_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.btnCancel.ForeColor = System.Drawing.Color.Red;
			this.btnCancel.Location = new System.Drawing.Point(16, 304);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.TabIndex = 8;
			this.btnCancel.Text = "ביטול";
			// 
			// btnConfirm
			// 
			this.btnConfirm.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnConfirm.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.btnConfirm.ForeColor = System.Drawing.Color.Blue;
			this.btnConfirm.Location = new System.Drawing.Point(104, 304);
			this.btnConfirm.Name = "btnConfirm";
			this.btnConfirm.TabIndex = 9;
			this.btnConfirm.Text = "אישור";
			// 
			// ChoosePrintPagesForm
			// 
			this.AcceptButton = this.btnConfirm;
			this.AutoScaleBaseSize = new System.Drawing.Size(7, 17);
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(346, 336);
			this.Controls.Add(this.btnConfirm);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnRemoveAll);
			this.Controls.Add(this.btnPrintAll);
			this.Controls.Add(this.btnRemovePrint);
			this.Controls.Add(this.btnAddPrint);
			this.Controls.Add(this.lbPagesToPrint);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.lbAvailablePages);
			this.Controls.Add(this.label1);
			this.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ChoosePrintPagesForm";
			this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "בחירת עמודים להדפסה";
			this.Load += new System.EventHandler(this.ChoosePrintPagesForm_Load);
			this.ResumeLayout(false);

		}
		#endregion
		
		private void AddAndSort(ListBox combo, string value)
		{
			ArrayList arrItems=new ArrayList(combo.Items);
			arrItems.Add(value);
			arrItems.Sort(new NumericComparer());
			combo.Items.Clear();
			combo.Items.AddRange(arrItems.ToArray());
		}

		private void SetButtons()
		{
			btnAddPrint.Enabled = (lbAvailablePages.SelectedIndex >= 0);
			btnRemovePrint.Enabled = (lbPagesToPrint.SelectedIndex >= 0);
		}
		
		private void lbAvailablePages_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			SetButtons();
		}

		private void btnAddPrint_Click(object sender, System.EventArgs e)
		{
			if (btnAddPrint.Enabled == false)
				return;
			int selIndex=lbAvailablePages.SelectedIndex;
			if (selIndex < 0)
				return;
			string strPage=lbAvailablePages.Items[selIndex].ToString();
			AddAndSort(lbPagesToPrint, strPage);
			lbAvailablePages.Items.RemoveAt(selIndex);
			lbAvailablePages.SelectedIndex = -1;
			lbAvailablePages_SelectedIndexChanged(null, EventArgs.Empty);
		}

		private void btnRemovePrint_Click(object sender, System.EventArgs e)
		{
			if (btnRemovePrint.Enabled == false)
				return;
			int selIndex=lbPagesToPrint.SelectedIndex;
			if (selIndex < 0)
				return;
			string strPage=lbPagesToPrint.Items[selIndex].ToString();
			AddAndSort(lbAvailablePages, strPage);
			lbPagesToPrint.Items.RemoveAt(selIndex);
			lbPagesToPrint.SelectedIndex = -1;
			lbAvailablePages_SelectedIndexChanged(null, EventArgs.Empty);
		}

		private void btnPrintAll_Click(object sender, System.EventArgs e)
		{
			while (lbAvailablePages.Items.Count > 1)
			{
				string strPage=lbAvailablePages.Items[0].ToString();
				lbPagesToPrint.Items.Add(strPage);
				lbAvailablePages.Items.RemoveAt(0);
			}
			if (lbAvailablePages.Items.Count == 1)
				AddAndSort(lbPagesToPrint, lbAvailablePages.Items[0].ToString());
			lbAvailablePages.Items.Clear();
			lbAvailablePages.SelectedIndex = -1;
			lbAvailablePages_SelectedIndexChanged(null, EventArgs.Empty);
		}

		private void btnRemoveAll_Click(object sender, System.EventArgs e)
		{
			while (lbPagesToPrint.Items.Count > 1)
			{
				string strPage=lbPagesToPrint.Items[0].ToString();
				lbAvailablePages.Items.Add(strPage);
				lbPagesToPrint.Items.RemoveAt(0);
			}
			if (lbPagesToPrint.Items.Count == 1)
				AddAndSort(lbAvailablePages, lbPagesToPrint.Items[0].ToString());
			lbPagesToPrint.Items.Clear();
			lbPagesToPrint.SelectedIndex = -1;
			lbAvailablePages_SelectedIndexChanged(null, EventArgs.Empty);
		}

		private void ChoosePrintPagesForm_Load(object sender, System.EventArgs e)
		{
			lbAvailablePages.Items.Clear();
			lbPagesToPrint.Items.Clear();
			if (_availablePages != null)
				lbAvailablePages.Items.AddRange(Sport.Common.Tools.ToStringArray(Sport.Common.Tools.AddIntegerOffset(_availablePages, 1)));
			if (_pagesToPrint != null)
				lbPagesToPrint.Items.AddRange(Sport.Common.Tools.ToStringArray(Sport.Common.Tools.AddIntegerOffset(_pagesToPrint, 1)));
		}
		
		private class NumericComparer : IComparer
		{
			public int Compare(object x, object y)
			{
				int n1=Int32.Parse(x.ToString());
				int n2=Int32.Parse(y.ToString());
				return n1.CompareTo(n2);
			}
		}
	}
}
