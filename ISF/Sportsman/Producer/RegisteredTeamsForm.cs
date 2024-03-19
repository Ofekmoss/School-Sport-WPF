using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace Sportsman.Producer
{
	/// <summary>
	/// Summary description for RegisteredTeamsForm.
	/// </summary>
	public class RegisteredTeamsForm : System.Windows.Forms.Form
	{
		private RegisteredTeamsGridPanel _teamsPanel;
		private Sport.Entities.ChampionshipCategory _category=null;
		private System.Windows.Forms.Button btnPrint;
		private System.Windows.Forms.Panel pnlPrint;
		private System.Windows.Forms.Panel pnlBottom;
		private System.Windows.Forms.Button btnClose;
		
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		
		public RegisteredTeamsForm(Sport.Entities.ChampionshipCategory category)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			
			_teamsPanel = new RegisteredTeamsGridPanel();
			this.Controls.Add(_teamsPanel);
			_category = category;
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(RegisteredTeamsForm));
			this.pnlPrint = new System.Windows.Forms.Panel();
			this.btnPrint = new System.Windows.Forms.Button();
			this.pnlBottom = new System.Windows.Forms.Panel();
			this.btnClose = new System.Windows.Forms.Button();
			this.pnlPrint.SuspendLayout();
			this.pnlBottom.SuspendLayout();
			this.SuspendLayout();
			// 
			// pnlPrint
			// 
			this.pnlPrint.Controls.Add(this.btnPrint);
			this.pnlPrint.Location = new System.Drawing.Point(96, 96);
			this.pnlPrint.Name = "pnlPrint";
			this.pnlPrint.Size = new System.Drawing.Size(292, 24);
			this.pnlPrint.TabIndex = 0;
			// 
			// btnPrint
			// 
			this.btnPrint.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(192)), ((System.Byte)(255)), ((System.Byte)(192)));
			this.btnPrint.Dock = System.Windows.Forms.DockStyle.Right;
			this.btnPrint.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.btnPrint.ForeColor = System.Drawing.Color.Blue;
			this.btnPrint.Location = new System.Drawing.Point(217, 0);
			this.btnPrint.Name = "btnPrint";
			this.btnPrint.Size = new System.Drawing.Size(75, 24);
			this.btnPrint.TabIndex = 0;
			this.btnPrint.Text = "הדפס";
			this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
			// 
			// pnlBottom
			// 
			this.pnlBottom.Controls.Add(this.btnClose);
			this.pnlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.pnlBottom.Location = new System.Drawing.Point(0, 442);
			this.pnlBottom.Name = "pnlBottom";
			this.pnlBottom.Size = new System.Drawing.Size(542, 24);
			this.pnlBottom.TabIndex = 1;
			// 
			// btnClose
			// 
			this.btnClose.BackColor = System.Drawing.Color.White;
			this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnClose.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.btnClose.ForeColor = System.Drawing.Color.Black;
			this.btnClose.Location = new System.Drawing.Point(224, 0);
			this.btnClose.Name = "btnClose";
			this.btnClose.Size = new System.Drawing.Size(75, 24);
			this.btnClose.TabIndex = 0;
			this.btnClose.Text = "סגור";
			// 
			// RegisteredTeamsForm
			// 
			this.AcceptButton = this.btnClose;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.btnClose;
			this.ClientSize = new System.Drawing.Size(542, 466);
			this.Controls.Add(this.pnlBottom);
			this.Controls.Add(this.pnlPrint);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Location = new System.Drawing.Point(250, 250);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "RegisteredTeamsForm";
			this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "קבוצות רשומות לאליפות";
			this.Closing += new System.ComponentModel.CancelEventHandler(this.RegisteredTeamsForm_Closing);
			this.Load += new System.EventHandler(this.RegisteredTeamsForm_Load);
			this.Closed += new System.EventHandler(this.RegisteredTeamsForm_Closed);
			this.pnlPrint.ResumeLayout(false);
			this.pnlBottom.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion
		
		private void RegisteredTeamsForm_Load(object sender, System.EventArgs e)
		{
			pnlPrint.Dock = DockStyle.Top;
			_teamsPanel.Dock = DockStyle.Top;
			_teamsPanel.Height = this.Height-(pnlPrint.Height+pnlBottom.Height+30);
			_teamsPanel.ChampionshipCategory = _category;
			_teamsPanel.Rebuild();
		}

		private void RegisteredTeamsForm_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			//e.Cancel = true;
		}
		

		private void RegisteredTeamsForm_Closed(object sender, System.EventArgs e)
		{
			
		}

		private void btnPrint_Click(object sender, System.EventArgs e)
		{
			System.Drawing.Printing.PrinterSettings ps;
			Sport.UI.Dialogs.PrintSettingsForm settingsForm;
			if (!Sport.UI.Helpers.GetPrinterSettings(out ps, out settingsForm))
			{
				return;
			}

			//let user choose printer settings:
			settingsForm.Landscape = false;
			if (settingsForm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				//build print document:
				Sport.Documents.Document document = CreatePrintDocument(ps);
				
				//preview?
				if (settingsForm.ShowPreview)
				{
					Sport.UI.Dialogs.PrintForm printForm = new Sport.UI.Dialogs.PrintForm(document, ps);
					
					if (!printForm.Canceled)
						printForm.ShowDialog();
					
					printForm.Dispose();
				}
				else
				{
					System.Drawing.Printing.PrintDocument pd = document.CreatePrintDocument(ps);
					pd.PrintController = new Sport.UI.PrintControllerWithPageForm(pd.PrintController, 0);
					pd.Print();
				}
			}
		}
		
		/// <summary>
		/// build the print document for the registered teams.
		/// </summary>
		private Sport.Documents.Document CreatePrintDocument(System.Drawing.Printing.PrinterSettings settings)
		{
			//show the wait cursor while building:
			Cursor cur = Cursor.Current;
			Cursor.Current = Cursors.WaitCursor;
			
			string strTitle=_category.Championship.Name+" "+_category.Name;
			Sport.Documents.TableItem ti = Tools.GridToTable(_teamsPanel.Grid, new Rectangle(0, 0, 500, 500));
			Documents.BaseDocumentBuilder baseDocumentBuilder = new Documents.BaseDocumentBuilder(settings);
			Sport.Documents.DocumentBuilder documentBuilder = baseDocumentBuilder.CreateDocumentBuilder(strTitle, ti, true);
			Sport.Documents.Document document = documentBuilder.CreateDocument();
			
			Cursor.Current = cur;
			return document;
		} //end function CreatePrintDocument
		
		public Sport.Entities.ChampionshipCategory ChampCategory
		{
			get { return _category; }
		}
	}
}
