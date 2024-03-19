using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Sport.UI.Controls;

namespace Sport.UI
{
	/// <summary>
	/// Summary description for DialogForm.
	/// </summary>
	public class DialogForm : System.Windows.Forms.Form, ICommandTarget
	{
		private static Hashtable buttonText;
		static DialogForm()
		{
			buttonText = new Hashtable();
			buttonText[DialogResult.None] = "סגור";
			buttonText[DialogResult.OK] = "אישור";
			buttonText[DialogResult.Cancel] = "ביטול";
			buttonText[DialogResult.Yes] = "כן";
			buttonText[DialogResult.No] = "לא";
			buttonText[DialogResult.Abort] = "הפסק";
			buttonText[DialogResult.Ignore] = "התעלם";
			buttonText[DialogResult.Retry] = "נסה שנית";
		}

		protected System.Windows.Forms.Panel viewPanel;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		protected View _view;
		public View View
		{
			get { return _view; }
		}

		public DialogForm(View view, DialogResult[] buttons, string[] texts, Size clientSize)
		{
			Font = new Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular);

			_view = view;
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			viewPanel.Controls.Add(_view);

			Text = _view.Title;
			_view.TitleChanged += new System.EventHandler(ViewTitleChanged);

			if (clientSize != Size.Empty)
				this.ClientSize = clientSize;

			int left = 8;

			if (texts == null)
			{
				foreach (DialogResult dr in buttons)
				{
					ThemeButton button = BuildButton(dr, left, (buttonText[dr] == null)?"סגור":buttonText[dr].ToString());
					if (button != null)
					{
						button.Click += new System.EventHandler(ButtonClicked);
						Controls.Add(button);
						left += button.Width + 8;
					}
				}
			}
			else
			{
				for (int n = 0; n < buttons.Length; n++)
				{
					ThemeButton button = BuildButton(buttons[n], left, texts[n]);
					if (button != null)
					{
						button.Click += new System.EventHandler(ButtonClicked);
						Controls.Add(button);
						left += button.Width + 8;
					}
				}
			}
		}

		public DialogForm(View view, DialogResult[] buttons, string[] texts)
			: this(view, buttons, texts, Size.Empty)
		{
		}

		public DialogForm(View view, DialogResult[] buttons, Size clientSize)
			: this(view, buttons, null, clientSize)
		{
		}

		public DialogForm(View view, DialogResult[] buttons)
			: this(view, buttons, null)
		{
		}

		public DialogForm(View view, Size clientSize)
			: this(view, new DialogResult[] { DialogResult.None }, clientSize)
		{
		}

		public DialogForm(View view)
			: this(view, new DialogResult[] { DialogResult.None })
		{
		}
		
		private ThemeButton BuildButton(DialogResult dr, int left, string text)
		{
			ThemeButton button = null;
			int tryCount = 0;
			while (button == null && tryCount < 10)
			{
				try
				{
					button = new ThemeButton();
				}
				catch
				{
				}
				tryCount++;
			}

			if (button == null)
				return null;

			button.Tag = dr;
			button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			button.Location = new System.Drawing.Point(left, 320);
			Button tempButton=new Button();
			tempButton.Text = text;
			button.Hue = 300;
			button.Saturation = 0.1f;
			this.Controls.Add(tempButton);
			button.Size = new Size(tempButton.Width, tempButton.Height);
			this.Controls.Remove(tempButton);
			//button.AutoSize = true;
			button.TabIndex = 0;
			button.Text = text;
			return button;
		}
		
		private void ViewTitleChanged(object sender, EventArgs e)
		{
			Text = _view.Title;
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
			this.viewPanel = new System.Windows.Forms.Panel();
			this.SuspendLayout();
			// 
			// viewPanel
			// 
			this.viewPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.viewPanel.Location = new System.Drawing.Point(8, 8);
			this.viewPanel.Name = "viewPanel";
			this.viewPanel.Size = new System.Drawing.Size(384, 304);
			this.viewPanel.TabIndex = 1;
			// 
			// DialogForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(400, 350);
			this.Controls.Add(this.viewPanel);
			this.Name = "DialogForm";
			this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "DialogForm";
			this.Closing += new System.ComponentModel.CancelEventHandler(this.DialogFormClosing);
			this.Load += new System.EventHandler(this.DialogFormLoaded);
			this.Closed += new System.EventHandler(this.DialogFormClosed);
			this.ResumeLayout(false);

		}
		#endregion

		private bool saveSize = false;

		protected virtual void OnDialogClosed()
		{
			saveSize = false;
			_view.Deactivate();
			_view.Close();
		}

		private void DialogFormClosed(object sender, System.EventArgs e)
		{
			OnDialogClosed();
		}

		protected virtual void OnDialogLoaded()
		{
			// Reading dialog size
			Sport.Core.Configuration viewConfiguration = _view.GetConfiguration();
			Sport.Core.Configuration dialogSize = viewConfiguration.GetConfiguration("DialogSize");
			int[] size = Sport.Common.Tools.ToIntArray(dialogSize.Value, ',');
			if (size != null && size.Length == 2)
			{
				ClientSize = new Size(size[0], size[1]);
				CenterToScreen();
			}
			saveSize = true;

			_view.Focus();

			_view.Open();
			_view.Activate();
		}

		protected override void OnResize(EventArgs e)
		{
			base.OnResize (e);

			if (saveSize)
			{
				// Storing dialog size
				Sport.Core.Configuration view = _view.GetConfiguration();
				Sport.Core.Configuration dialogSize = view.GetConfiguration("DialogSize");
				dialogSize.Value = ClientSize.Width.ToString() + "," + ClientSize.Height.ToString();
				Sport.Core.Configuration.Save();
			}
		}

		private void DialogFormLoaded(object sender, System.EventArgs e)
		{
			OnDialogLoaded();
		}

		private void ButtonClicked(object sender, System.EventArgs e)
		{
			if (sender is ThemeButton)
			{
				DialogResult = (DialogResult) ((ThemeButton)sender).Tag;
				Close();
			}
		}

		private void DialogFormClosing(object sender, CancelEventArgs e)
		{
			try
			{
				if (!_view.Closing())
					e.Cancel = true;
			}
			catch
			{
				e.Cancel = false;
			}
		}

		#region ICommandTarget Members
		public virtual bool ExecuteCommand(string command)
		{
			if (command == Sport.Core.Data.KeyCommands.Cancel)
			{
				this.DialogResult = DialogResult.Cancel;
			}
			else
			{
				ICommandTarget ct = _view as ICommandTarget;
				if (ct != null)
					return ct.ExecuteCommand(command);
				return false;
			}
			
			return true;
		}
		#endregion
	}
}
