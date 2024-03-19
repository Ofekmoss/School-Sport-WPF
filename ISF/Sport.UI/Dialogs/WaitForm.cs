using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace Sport.UI.Dialogs
{
	/// <summary>
	/// Summary description for LoadForm.
	/// </summary>
	public class WaitForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label labelTitle;
		private Button btnCancel;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		private WaitForm()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (components != null)
				{
					components.Dispose();
				}
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
			this.labelTitle = new System.Windows.Forms.Label();
			this.btnCancel = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// labelTitle
			// 
			this.labelTitle.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.labelTitle.Dock = System.Windows.Forms.DockStyle.Right;
			this.labelTitle.Location = new System.Drawing.Point(64, 0);
			this.labelTitle.Name = "labelTitle";
			this.labelTitle.Size = new System.Drawing.Size(256, 32);
			this.labelTitle.TabIndex = 0;
			this.labelTitle.Text = "label1";
			this.labelTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// btnCancel
			// 
			this.btnCancel.BackColor = System.Drawing.Color.White;
			this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnCancel.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
			this.btnCancel.Location = new System.Drawing.Point(12, 5);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(46, 23);
			this.btnCancel.TabIndex = 1;
			this.btnCancel.Text = "αιθεμ";
			this.btnCancel.UseVisualStyleBackColor = false;
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// WaitForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(6, 13);
			this.ClientSize = new System.Drawing.Size(320, 32);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.labelTitle);
			this.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Name = "WaitForm";
			this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "LoadForm";
			this.ResumeLayout(false);

		}
		#endregion

		private static WaitForm form = new WaitForm();

		private static bool _changeCursor;
		private static Cursor _oldCursor;
		private static string _originalText;

		private static bool _cancelled = false;
		private static bool _suppressed = false;
		public static bool Cancelled { get { return _cancelled; } }

		public static void Suppress()
		{
			_suppressed = true;
		}

		public static void UnSuppress()
		{
			_suppressed = false;
		}

		public static bool IsSuppressed()
		{
			return _suppressed;
		}

		public static void ShowWait(string text, bool changeCursor)
		{
			if (_suppressed == true)
				return;

			_changeCursor = changeCursor;
			_cancelled = false;
			
			if (changeCursor)
			{
				_oldCursor = Cursor.Current;
				Cursor.Current = Cursors.WaitCursor;
			}

			if (!form.IsDisposed)
			{
				form.labelTitle.Text = text;
				form.btnCancel.Visible = false;
				form.labelTitle.Left = 0;
				form.labelTitle.Dock = DockStyle.Fill;
				form.Show();
				form.Refresh();
			}
		}

		public static void ShowWait(string text)
		{
			ShowWait(text, false);
		}

		public static void HideWait()
		{
			if (_changeCursor)
				Cursor.Current = _oldCursor;
			form.Hide();
		}

		public static void SetProgress(int progress)
		{
			if (_suppressed == true)
				return;

			if ((form == null) || (form.IsDisposed))
				return;

			if (progress < 0)
				progress = 0;
			if (progress >= 100)
			{
				form.labelTitle.Text = _originalText;
				_originalText = null;
			}
			else
			{
				if (_originalText == null)
					_originalText = form.labelTitle.Text;
				form.labelTitle.Text = _originalText + " (" + progress + "%)";
			}

			if (_changeCursor && Cursor.Current != Cursors.WaitCursor)
			{
				_oldCursor = Cursor.Current;
				Cursor.Current = Cursors.WaitCursor;
			}

			if (!form.btnCancel.Visible)
			{
				form.labelTitle.Dock = DockStyle.Right;
				form.labelTitle.Left = 64;
				form.btnCancel.Visible = true;
			}

			form.Show();
			form.Refresh();
			Application.DoEvents();
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			WaitForm._cancelled = true;
			HideWait();
		}
	}
}
