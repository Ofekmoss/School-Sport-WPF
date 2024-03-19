using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Threading;
using System.Drawing.Printing;

namespace Sport.Printing
{
	public class PrintControllerWithPageForm : PrintController
	{
		public PrintControllerWithPageForm(PrintController underlyingController, int pageCount)
		{
			this.underlyingController = underlyingController;
			this.pageCount = pageCount;
		}

		public override void OnEndPage(PrintDocument document, PrintPageEventArgs e)
		{
			this.underlyingController.OnEndPage(document, e);
			if ((this.backgroundThread != null) && this.backgroundThread.canceled)
			{
				e.Cancel = true;
			}
			this.pageNumber++;
			base.OnEndPage(document, e);

		}

		public override void OnEndPrint(PrintDocument document, PrintEventArgs e)
		{
			this.underlyingController.OnEndPrint(document, e);
			if ((this.backgroundThread != null) && this.backgroundThread.canceled)
			{
				e.Cancel = true;
			}
			if (this.backgroundThread != null)
			{
				this.backgroundThread.Stop();
			}
			base.OnEndPrint(document, e);
		}

		public override Graphics OnStartPage(PrintDocument document, PrintPageEventArgs e)
		{
			base.OnStartPage(document, e);
			if (this.backgroundThread != null)
			{
				this.backgroundThread.UpdateLabel();
			}
			Graphics graphics = this.underlyingController.OnStartPage(document, e);
			if ((this.backgroundThread != null) && this.backgroundThread.canceled)
			{
				e.Cancel = true;
			}
			return graphics;

		}

		public override void OnStartPrint(PrintDocument document, PrintEventArgs e)
		{
			base.OnStartPrint(document, e);
			this.document = document;
			this.pageNumber = 1;
			if (SystemInformation.UserInteractive)
			{
				this.backgroundThread = new PrintControllerWithPageForm.BackgroundThread(this);
			}
			try
			{
				this.underlyingController.OnStartPrint(document, e);
			}
			catch (Exception exception)
			{
				if (this.backgroundThread != null)
				{
					this.backgroundThread.Stop();
				}
				throw exception;
			}
			finally
			{
				if ((this.backgroundThread != null) && this.backgroundThread.canceled)
				{
					e.Cancel = true;
				}
			}
		}

		private BackgroundThread backgroundThread;
		private PrintDocument document;
		private int pageNumber;
		private int pageCount;
		private PrintController underlyingController;

		public bool Canceled
		{
			get
			{
				return backgroundThread == null ? false : backgroundThread.canceled;
			}
		}

		private class BackgroundThread
		{
			internal BackgroundThread(PrintControllerWithPageForm parent)
			{
				this.canceled = false;
				this.alreadyStopped = false;
				this.parent = parent;
				this.thread = new Thread(new ThreadStart(this.Run));
				this.thread.ApartmentState = ApartmentState.STA;
				this.thread.Start();
			}

			private void Run()
			{
				PrintControllerWithPageForm.BackgroundThread _thread;
				try
				{
					lock ((_thread = this))
					{
						if (this.alreadyStopped)
						{
							return;
						}
						this.form = new PageForm(this);
						this.ThreadUnsafeUpdateLabel();
						this.form.Visible = true;
					}
					if (!this.alreadyStopped)
					{
						Application.Run(this.form);
					}
				}
				finally
				{
					lock ((_thread = this))
					{
						if (this.form != null)
						{
							this.form.Dispose();
							this.form = null;
						}
					}
				}

			}

			internal void Stop()
			{
				lock (this)
				{
					if ((this.form != null) && this.form.IsHandleCreated)
					{
						this.form.BeginInvoke(new MethodInvoker(this.form.Close));
					}
					else
					{
						this.alreadyStopped = true;
					}
				}
			}

			private void ThreadUnsafeUpdateLabel()
			{
				string text = "עמוד " + this.parent.pageNumber.ToString();
				
				if (this.parent.pageCount > 0)
					text += " מתוך " + this.parent.pageCount.ToString();

				this.form.labelText.Text = text;
			}

			internal void UpdateLabel()
			{
				if ((this.form != null) && this.form.IsHandleCreated)
				{
					this.form.BeginInvoke(new MethodInvoker(this.ThreadUnsafeUpdateLabel));
				}
			}

			private bool alreadyStopped;
			internal bool canceled;
			private PageForm form;
			private PrintControllerWithPageForm parent;
			private Thread thread;
		}

		/// <summary>
		/// Summary description for LoadForm.
		/// </summary>
		private class PageForm : System.Windows.Forms.Form
		{
			/// <summary>
			/// Required designer variable.
			/// </summary>
			private System.ComponentModel.Container components = null;

			private PrintControllerWithPageForm.BackgroundThread backgroundThread;
			internal PageForm(PrintControllerWithPageForm.BackgroundThread backgroundThread)
			{
				this.backgroundThread = backgroundThread;
				//
				// Required for Windows Form Designer support
				//
				InitializeComponent();

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
				this.panel = new System.Windows.Forms.Panel();
				this.btnCancel = new System.Windows.Forms.Button();
				this.labelText = new System.Windows.Forms.Label();
				this.panel.SuspendLayout();
				this.SuspendLayout();
				// 
				// panel
				// 
				this.panel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
				this.panel.Controls.Add(this.btnCancel);
				this.panel.Controls.Add(this.labelText);
				this.panel.Dock = System.Windows.Forms.DockStyle.Fill;
				this.panel.Location = new System.Drawing.Point(0, 0);
				this.panel.Name = "panel";
				this.panel.Size = new System.Drawing.Size(256, 32);
				this.panel.TabIndex = 0;
				// 
				// btnCancel
				// 
				this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Left;
				this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
				this.btnCancel.Location = new System.Drawing.Point(6, 5);
				this.btnCancel.Name = "btnCancel";
				this.btnCancel.Size = new System.Drawing.Size(56, 20);
				this.btnCancel.TabIndex = 1;
				this.btnCancel.Text = "ביטול";
				this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
				// 
				// labelText
				// 
				this.labelText.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
					| System.Windows.Forms.AnchorStyles.Left) 
					| System.Windows.Forms.AnchorStyles.Right)));
				this.labelText.Location = new System.Drawing.Point(72, 5);
				this.labelText.Name = "labelText";
				this.labelText.Size = new System.Drawing.Size(176, 20);
				this.labelText.TabIndex = 0;
				this.labelText.Text = "label1";
				this.labelText.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
				// 
				// PageForm
				// 
				this.AutoScaleBaseSize = new System.Drawing.Size(6, 13);
				this.ClientSize = new System.Drawing.Size(256, 32);
				this.Controls.Add(this.panel);
				this.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
				this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
				this.Name = "PageForm";
				this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
				this.ShowInTaskbar = false;
				this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
				this.Text = "LoadForm";
				this.panel.ResumeLayout(false);
				this.ResumeLayout(false);

			}
			#endregion

			private System.Windows.Forms.Panel panel;
			internal System.Windows.Forms.Label labelText;
			private System.Windows.Forms.Button btnCancel;

			private void btnCancel_Click(object sender, System.EventArgs e)
			{
				this.btnCancel.Enabled = false;
				this.labelText.Text = "מבטל";
				this.backgroundThread.canceled = true;
			}
		}
	}
}
