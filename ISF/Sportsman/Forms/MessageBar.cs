using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace Sportsman.Forms
{
	/// <summary>
	/// Summary description for MessageBar.
	/// </summary>
	public class MessageBar : System.Windows.Forms.Control
	{
		private Sport.UI.Controls.RichTextBoxEx rtbMessages;

		private System.Collections.ArrayList links;
		private System.Windows.Forms.Label labelMessages;
		private Sport.UI.Controls.ThemeButton tbClose;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public MessageBar()
		{
			InitializeComponent();

			links = new ArrayList();
		}

		protected override void OnVisibleChanged(EventArgs e)
		{
			base.OnVisibleChanged(e);

			if (Visible)
				FillMessages();
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(MessageBar));
			this.rtbMessages = new Sport.UI.Controls.RichTextBoxEx();
			this.labelMessages = new System.Windows.Forms.Label();
			this.tbClose = new Sport.UI.Controls.ThemeButton();
			this.SuspendLayout();
			// 
			// rtbMessages
			// 
			this.rtbMessages.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
			this.rtbMessages.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
				| System.Windows.Forms.AnchorStyles.Left)
				| System.Windows.Forms.AnchorStyles.Right)));
			this.rtbMessages.BackColor = System.Drawing.SystemColors.Control;
			this.rtbMessages.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.rtbMessages.Location = new System.Drawing.Point(8, 28);
			this.rtbMessages.Name = "rtbMessages";
			this.rtbMessages.ReadOnly = true;
			this.rtbMessages.SelectionLink = false;
			this.rtbMessages.Size = new System.Drawing.Size(696, 540);
			this.rtbMessages.TabIndex = 0;
			this.rtbMessages.Text = "rtbMessages";
			this.rtbMessages.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.rtbMessages_LinkClicked);
			// 
			// labelMessages
			// 
			this.labelMessages.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
			this.labelMessages.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
				| System.Windows.Forms.AnchorStyles.Right)));
			this.labelMessages.BackColor = System.Drawing.SystemColors.Highlight;
			this.labelMessages.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.labelMessages.ForeColor = System.Drawing.SystemColors.HighlightText;
			this.labelMessages.Location = new System.Drawing.Point(26, 4);
			this.labelMessages.Name = "labelMessages";
			this.labelMessages.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.labelMessages.Size = new System.Drawing.Size(678, 16);
			this.labelMessages.TabIndex = 1;
			this.labelMessages.Text = "הודעות";
			// 
			// tbClose
			// 
			this.tbClose.Alignment = System.Drawing.StringAlignment.Center;
			this.tbClose.Font = new System.Drawing.Font("Arial", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
			this.tbClose.Hue = 0F;
			this.tbClose.Image = ((System.Drawing.Image)(resources.GetObject("tbClose.Image")));
			this.tbClose.ImageAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.tbClose.ImageList = null;
			this.tbClose.Location = new System.Drawing.Point(4, 1);
			this.tbClose.Name = "tbClose";
			this.tbClose.Saturation = 0.8F;
			this.tbClose.Size = new System.Drawing.Size(21, 21);
			this.tbClose.TabIndex = 2;
			this.tbClose.Transparent = System.Drawing.Color.Black;
			this.tbClose.Click += new System.EventHandler(this.tbClose_Click);
			// 
			// MessageBar
			// 
			this.Controls.Add(this.tbClose);
			this.Controls.Add(this.labelMessages);
			this.Controls.Add(this.rtbMessages);
			this.Name = "MessageBar";
			this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.Size = new System.Drawing.Size(710, 569);
			this.Text = "MessageBar";
			this.ResumeLayout(false);

		}

		#endregion

		private void FillMessages()
		{
			rtbMessages.Rtf = null;

			links.Clear();

			Sport.Data.EntityFilter filter = new Sport.Data.EntityFilter();

			filter.Add(new Sport.Data.EntityFilterField((int)Sport.Entities.Message.Fields.User, Sport.Core.Session.User.Id));
			filter.Add(new Sport.Data.EntityFilterField((int)Sport.Entities.Message.Fields.Status, (int)Sport.Types.MessageStatus.New));

			Sport.Data.Entity[] messages = new Sport.Data.Entity[] { };
			try
			{
				//removed 18/05/2014 - not used, not needed any more.
				//messages = Sport.Entities.Message.Type.GetEntities(filter);
			}
			catch (Exception e)
			{
				Sport.UI.MessageBox.Show("אזהרה: קריאת הודעות חדשות נכשלה:\n" + e.Message,
					"ספורטסמן", MessageBoxIcon.Warning);
				return;
			}

			Sport.Data.ExpandString es;

			string[] vars;

			for (int m = 0; m < messages.Length; m++)
			{
				Sport.Entities.Message message = new Sport.Entities.Message(messages[m]);
				es = message.ExpandText();

				if (es.Expanded.Length == 0)
				{
					Sport.Entities.Message.Type.Delete(message.Entity);
					//DataServices.DataService _service=new DataServices.DataService();
					//_service.CookieContainer = Sport.Core.Session.Cookies;
					//_service.DeleteEntity(Sport.Entities.Message.TypeName, message.Id);
					Sport.Entities.Message.Type.Reset(null);
					Sport.Entities.Message.Type.DeleteDatFile();
					continue;
				}

				int lineStart = rtbMessages.TextLength;
				rtbMessages.SelectionStart = lineStart;

				vars = new string[es.Variables.Length];
				for (int n = 0; n < es.Variables.Length; n++)
				{
					vars[n] = "\\v #\\v0 " + es.Variables[n].ToString() + "\\v #" + links.Add(es.Variables[n]).ToString() + "#\\v0";
				}

				rtbMessages.SelectedRtf = "{\\rtf1\\ansi\\ansicpg1255 \\rtlpar \\pn \\pnlvlblt " + String.Format(es.Expanded, vars) + " \\par}";
				rtbMessages.Select(lineStart, rtbMessages.TextLength - lineStart);

				string text = rtbMessages.SelectedText;
				int start = text.IndexOf('#');
				int mid, end;
				while (start >= 0)
				{
					mid = text.IndexOf('#', start + 1);
					end = text.IndexOf('#', mid + 1);

					rtbMessages.Select(start + lineStart, end - start + 1);

					rtbMessages.SelectionLink = true;

					start = text.IndexOf('#', end + 1);
				}
			}

			rtbMessages.SelectAll();
			rtbMessages.Select(0, 0);
		}

		private void rtbMessages_LinkClicked(object sender, System.Windows.Forms.LinkClickedEventArgs e)
		{
			int lp = e.LinkText.LastIndexOf('#', e.LinkText.Length - 2);
			string ls = e.LinkText.Substring(lp + 1, e.LinkText.Length - lp - 2);

			int link = Int32.Parse(ls);
			OpenLink(links[link]);
		}

		private void OpenLink(object link)
		{
			Sport.Data.Entity entity = link as Sport.Data.Entity;

			if (entity != null)
			{
				if (entity.EntityType == Sport.Entities.Championship.Type)
				{
					Sport.UI.ViewManager.OpenView(typeof(Views.TeamsTableView),
						Sport.Entities.Championship.TypeName + "=" + entity.Id.ToString());
				}
			}
		}

		private void tbClose_Click(object sender, System.EventArgs e)
		{
			Sport.UI.Display.StateItem si = Sport.UI.Display.StateManager.States["messagebar"];
			si.Checked = false;
		}
	}
}
