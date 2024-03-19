using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Sport.UI.Controls;

namespace Sportsman.Forms
{
	/// <summary>
	/// Summary description for PaymentForm.
	/// </summary>
	public class PaymentForm : System.Windows.Forms.Form, Sport.UI.ICommandTarget
	{
		private Sport.UI.Controls.ThemeButton tbOk;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.Label labelSchool;
		private System.Windows.Forms.Label labelNumber;
		private System.Windows.Forms.Label labelSum;
		private System.Windows.Forms.Label labelDate;
		private System.Windows.Forms.Label labelType;
		private System.Windows.Forms.Label labelDescription;
		private System.Windows.Forms.Label labelPaidBy;
		private System.Windows.Forms.Label labelCharges;
		private Sport.UI.Controls.GridControl gridCharges;
		private Sport.UI.Controls.TextControl tcSchool;
		private Sport.UI.Controls.TextControl tcNumber;
		private System.Windows.Forms.DateTimePicker dtpDate;
		private Sport.UI.Controls.NullComboBox ncbType;
		private Sport.UI.Controls.TextControl tcPaidBy;
		private Sport.UI.Controls.TextControl tcDescription;
		private Sport.UI.Controls.TextControl tcSum;
		private Sport.UI.Controls.ThemeButton tbCancel;

		private Sport.Entities.School _school;

		public PaymentForm(Sport.Entities.School school)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			_school = school;

			tcNumber.Controller = new NumberController(0d, 99999999);
			tcSum.Controller = new NumberController(0, 99999999, 6, 2);

			tcSum.Value = (double) 0;

			ncbType.Items.AddRange(new Sport.Types.PaymentTypeLookup().Items);
			ncbType.SelectedIndex = 0;

			tcSchool.Text = _school.Name;

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
			this.gridCharges = new Sport.UI.Controls.GridControl();
			this.tbOk = new Sport.UI.Controls.ThemeButton();
			this.tbCancel = new Sport.UI.Controls.ThemeButton();
			this.labelSchool = new System.Windows.Forms.Label();
			this.tcSchool = new Sport.UI.Controls.TextControl();
			this.tcNumber = new Sport.UI.Controls.TextControl();
			this.labelNumber = new System.Windows.Forms.Label();
			this.labelSum = new System.Windows.Forms.Label();
			this.labelDate = new System.Windows.Forms.Label();
			this.labelType = new System.Windows.Forms.Label();
			this.labelDescription = new System.Windows.Forms.Label();
			this.labelPaidBy = new System.Windows.Forms.Label();
			this.labelCharges = new System.Windows.Forms.Label();
			this.dtpDate = new System.Windows.Forms.DateTimePicker();
			this.ncbType = new Sport.UI.Controls.NullComboBox();
			this.tcPaidBy = new Sport.UI.Controls.TextControl();
			this.tcDescription = new Sport.UI.Controls.TextControl();
			this.tcSum = new Sport.UI.Controls.TextControl();
			this.SuspendLayout();
			// 
			// gridCharges
			// 
			this.gridCharges.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.gridCharges.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.gridCharges.Location = new System.Drawing.Point(8, 160);
			this.gridCharges.Name = "gridCharges";
			this.gridCharges.Size = new System.Drawing.Size(384, 152);
			this.gridCharges.TabIndex = 7;
			// 
			// tbOk
			// 
			this.tbOk.Alignment = System.Drawing.StringAlignment.Center;
			this.tbOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.tbOk.AutoSize = false;
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
			this.tbOk.TabIndex = 8;
			this.tbOk.Text = "אישור";
			this.tbOk.Transparent = System.Drawing.Color.Black;
			this.tbOk.Click += new System.EventHandler(this.tbOk_Click);
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
			this.tbCancel.Location = new System.Drawing.Point(71, 320);
			this.tbCancel.Name = "tbCancel";
			this.tbCancel.Saturation = 0.1F;
			this.tbCancel.Size = new System.Drawing.Size(56, 25);
			this.tbCancel.TabIndex = 9;
			this.tbCancel.Text = "ביטול";
			this.tbCancel.Transparent = System.Drawing.Color.Black;
			this.tbCancel.Click += new System.EventHandler(this.tbCancel_Click);
			// 
			// labelSchool
			// 
			this.labelSchool.Location = new System.Drawing.Point(336, 16);
			this.labelSchool.Name = "labelSchool";
			this.labelSchool.Size = new System.Drawing.Size(56, 16);
			this.labelSchool.TabIndex = 3;
			this.labelSchool.Text = "בית ספר:";
			// 
			// tcSchool
			// 
			this.tcSchool.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.tcSchool.Controller = null;
			this.tcSchool.Location = new System.Drawing.Point(8, 11);
			this.tcSchool.Name = "tcSchool";
			this.tcSchool.ReadOnly = true;
			this.tcSchool.SelectionLength = 0;
			this.tcSchool.SelectionStart = 0;
			this.tcSchool.ShowSpin = false;
			this.tcSchool.Size = new System.Drawing.Size(296, 21);
			this.tcSchool.TabIndex = 0;
			this.tcSchool.Value = "";
			// 
			// tcNumber
			// 
			this.tcNumber.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.tcNumber.Controller = null;
			this.tcNumber.Location = new System.Drawing.Point(240, 35);
			this.tcNumber.Name = "tcNumber";
			this.tcNumber.ReadOnly = false;
			this.tcNumber.SelectionLength = 0;
			this.tcNumber.SelectionStart = 0;
			this.tcNumber.ShowSpin = true;
			this.tcNumber.Size = new System.Drawing.Size(64, 21);
			this.tcNumber.TabIndex = 1;
			this.tcNumber.Value = "";
			// 
			// labelNumber
			// 
			this.labelNumber.Location = new System.Drawing.Point(336, 40);
			this.labelNumber.Name = "labelNumber";
			this.labelNumber.Size = new System.Drawing.Size(56, 16);
			this.labelNumber.TabIndex = 5;
			this.labelNumber.Text = "מספר:";
			// 
			// labelSum
			// 
			this.labelSum.Location = new System.Drawing.Point(304, 112);
			this.labelSum.Name = "labelSum";
			this.labelSum.Size = new System.Drawing.Size(88, 16);
			this.labelSum.TabIndex = 7;
			this.labelSum.Text = "סכום התשלום:";
			// 
			// labelDate
			// 
			this.labelDate.Location = new System.Drawing.Point(304, 64);
			this.labelDate.Name = "labelDate";
			this.labelDate.Size = new System.Drawing.Size(88, 16);
			this.labelDate.TabIndex = 8;
			this.labelDate.Text = "תאריך התשלום:";
			// 
			// labelType
			// 
			this.labelType.Location = new System.Drawing.Point(152, 40);
			this.labelType.Name = "labelType";
			this.labelType.Size = new System.Drawing.Size(80, 16);
			this.labelType.TabIndex = 9;
			this.labelType.Text = "צורת התשלום:";
			// 
			// labelDescription
			// 
			this.labelDescription.Location = new System.Drawing.Point(312, 88);
			this.labelDescription.Name = "labelDescription";
			this.labelDescription.Size = new System.Drawing.Size(80, 16);
			this.labelDescription.TabIndex = 10;
			this.labelDescription.Text = "פרטי התשלום:";
			// 
			// labelPaidBy
			// 
			this.labelPaidBy.Location = new System.Drawing.Point(144, 64);
			this.labelPaidBy.Name = "labelPaidBy";
			this.labelPaidBy.Size = new System.Drawing.Size(56, 16);
			this.labelPaidBy.TabIndex = 11;
			this.labelPaidBy.Text = "שולם ע\"י:";
			// 
			// labelCharges
			// 
			this.labelCharges.Location = new System.Drawing.Point(296, 136);
			this.labelCharges.Name = "labelCharges";
			this.labelCharges.Size = new System.Drawing.Size(96, 16);
			this.labelCharges.TabIndex = 12;
			this.labelCharges.Text = "חיובים:";
			// 
			// dtpDate
			// 
			this.dtpDate.CustomFormat = "dd/MM/yyyy";
			this.dtpDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
			this.dtpDate.Location = new System.Drawing.Point(208, 59);
			this.dtpDate.Name = "dtpDate";
			this.dtpDate.Size = new System.Drawing.Size(96, 21);
			this.dtpDate.TabIndex = 3;
			// 
			// ncbType
			// 
			this.ncbType.Location = new System.Drawing.Point(8, 34);
			this.ncbType.Name = "ncbType";
			this.ncbType.Size = new System.Drawing.Size(136, 22);
			this.ncbType.Sorted = true;
			this.ncbType.TabIndex = 2;
			// 
			// tcPaidBy
			// 
			this.tcPaidBy.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.tcPaidBy.Controller = null;
			this.tcPaidBy.Location = new System.Drawing.Point(8, 59);
			this.tcPaidBy.Name = "tcPaidBy";
			this.tcPaidBy.ReadOnly = false;
			this.tcPaidBy.SelectionLength = 0;
			this.tcPaidBy.SelectionStart = 0;
			this.tcPaidBy.ShowSpin = false;
			this.tcPaidBy.Size = new System.Drawing.Size(136, 21);
			this.tcPaidBy.TabIndex = 4;
			this.tcPaidBy.Value = "";
			// 
			// tcDescription
			// 
			this.tcDescription.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.tcDescription.Controller = null;
			this.tcDescription.Location = new System.Drawing.Point(8, 83);
			this.tcDescription.Name = "tcDescription";
			this.tcDescription.ReadOnly = false;
			this.tcDescription.SelectionLength = 0;
			this.tcDescription.SelectionStart = 0;
			this.tcDescription.ShowSpin = false;
			this.tcDescription.Size = new System.Drawing.Size(296, 21);
			this.tcDescription.TabIndex = 5;
			this.tcDescription.Value = "";
			// 
			// tcSum
			// 
			this.tcSum.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.tcSum.Controller = null;
			this.tcSum.Location = new System.Drawing.Point(224, 107);
			this.tcSum.Name = "tcSum";
			this.tcSum.ReadOnly = false;
			this.tcSum.SelectionLength = 0;
			this.tcSum.SelectionStart = 0;
			this.tcSum.ShowSpin = true;
			this.tcSum.Size = new System.Drawing.Size(80, 21);
			this.tcSum.TabIndex = 6;
			this.tcSum.Value = "";
			// 
			// PaymentForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 14);
			this.ClientSize = new System.Drawing.Size(400, 350);
			this.Controls.Add(this.tcSum);
			this.Controls.Add(this.tcDescription);
			this.Controls.Add(this.tcPaidBy);
			this.Controls.Add(this.ncbType);
			this.Controls.Add(this.dtpDate);
			this.Controls.Add(this.labelCharges);
			this.Controls.Add(this.labelPaidBy);
			this.Controls.Add(this.labelDescription);
			this.Controls.Add(this.labelType);
			this.Controls.Add(this.labelDate);
			this.Controls.Add(this.labelSum);
			this.Controls.Add(this.tcNumber);
			this.Controls.Add(this.labelNumber);
			this.Controls.Add(this.tcSchool);
			this.Controls.Add(this.labelSchool);
			this.Controls.Add(this.tbCancel);
			this.Controls.Add(this.tbOk);
			this.Controls.Add(this.gridCharges);
			this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.Name = "PaymentForm";
			this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "תשלום";
			this.ResumeLayout(false);

		}
		#endregion

		#region ICommandTarget Members

		public virtual bool ExecuteCommand(string command)
		{
			if (command == Sport.Core.Data.KeyCommands.Cancel)
			{
				this.DialogResult = DialogResult.Cancel;
				return true;
			}

			return false;
		}

		#endregion

		private void tbOk_Click(object sender, System.EventArgs e)
		{
			DialogResult = DialogResult.OK;
		}

		private void tbCancel_Click(object sender, System.EventArgs e)
		{
			DialogResult = DialogResult.Cancel;
		}

		#region Charge Selection Dialog

		private static string chargeString = "חיוב";
		private static string productString = "מוצר";

		public static object SelectCharge(Sport.Entities.School school)
		{
			Sport.UI.Dialogs.GenericEditDialog ged = new Sport.UI.Dialogs.GenericEditDialog("בחר חיוב או מוצר");
			ged.Items.Add(Sport.UI.Controls.GenericItemType.Selection, chargeString, new object[] { chargeString, productString });
			ged.Items.Add(Sport.UI.Controls.GenericItemType.Selection);
			ged.Items[0].ValueChanged += new EventHandler(SelectChargeTypeChanged);
			ged.Items[1].ValueChanged += new EventHandler(SelectChargeChargeChanged);
			ged.Items[0].Nullable = false;
			ged.Items[1].Nullable = false;
			ged.Confirmable = false;
			ged.Tag = school;

			SelectChargeTypeChanged(ged.Items[0], EventArgs.Empty);

			if (ged.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				object v = ged.Items[1].Value;
				if (v is Sport.Entities.Charge)
				{
					return v;
				}
				else
				{
					return new Sport.Entities.Product((Sport.Data.Entity) v);
				}
			}

			return null;
		}

		private static void SelectChargeTypeChanged(object sender, EventArgs e)
		{
			Sport.UI.Controls.GenericItem gi = (Sport.UI.Controls.GenericItem) sender;
			Sport.UI.Dialogs.GenericEditDialog ged = (Sport.UI.Dialogs.GenericEditDialog) gi.Container;
			if (gi.Value == (object) productString)
			{
				ged.Items[1].Values = Sport.Entities.Product.Type.GetEntities(null);
			}
			else
			{
				System.Collections.ArrayList al = new System.Collections.ArrayList();
				foreach (Sport.Entities.Charge charge in ((Sport.Entities.Account) ged.Tag).GetCharges())
				{
					//if (charge.Payment == null)
						al.Add(charge);
				}

				ged.Items[1].Values = (Sport.Entities.Charge[]) al.ToArray(typeof(Sport.Entities.Charge));
			}

			ged.Confirmable = false;
		}

		private static void SelectChargeChargeChanged(object sender, EventArgs e)
		{
			Sport.UI.Controls.GenericItem gi = (Sport.UI.Controls.GenericItem) sender;
			Sport.UI.Dialogs.GenericEditDialog ged = (Sport.UI.Dialogs.GenericEditDialog) gi.Container;
			ged.Confirmable = gi.Value != null;
		}

		#endregion
	}
}
