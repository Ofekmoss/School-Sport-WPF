using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Sport.Data;
using Sport.Common;
using Sportsman.PermissionServices;

namespace Sportsman.Forms
{
	/// <summary>
	/// Summary description for PermissionsForm.
	/// </summary>
	public class PermissionsForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.ComboBox cbUsers;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Panel pnlPermissions;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button button2;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		private readonly string _strReadOnly="קריאה בלבד";
		private readonly string _strFullPermissions="שינוי/עריכה";
		private ArrayList arrPermissionPanels=null; //array of PermissionPanel
		private bool _dataChanged=false;
		private int _lastUserIndex=-1;
		private bool _changedByCode=false;
		
		public PermissionsForm()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
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
			this.panel1 = new System.Windows.Forms.Panel();
			this.button1 = new System.Windows.Forms.Button();
			this.pnlPermissions = new System.Windows.Forms.Panel();
			this.cbUsers = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.button2 = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// panel1
			// 
			this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel1.Location = new System.Drawing.Point(10, 42);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(570, 1);
			this.panel1.TabIndex = 2;
			// 
			// button1
			// 
			this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.button1.Location = new System.Drawing.Point(8, 344);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(64, 23);
			this.button1.TabIndex = 4;
			this.button1.Text = "סגור";
			// 
			// pnlPermissions
			// 
			this.pnlPermissions.AutoScroll = true;
			this.pnlPermissions.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.pnlPermissions.Location = new System.Drawing.Point(8, 64);
			this.pnlPermissions.Name = "pnlPermissions";
			this.pnlPermissions.Size = new System.Drawing.Size(568, 264);
			this.pnlPermissions.TabIndex = 5;
			// 
			// cbUsers
			// 
			this.cbUsers.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbUsers.Location = new System.Drawing.Point(264, 8);
			this.cbUsers.Name = "cbUsers";
			this.cbUsers.Size = new System.Drawing.Size(216, 21);
			this.cbUsers.TabIndex = 3;
			this.cbUsers.SelectedIndexChanged += new System.EventHandler(this.cbUsers_SelectedIndexChanged);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(488, 8);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(80, 24);
			this.label1.TabIndex = 2;
			this.label1.Text = "בחר משתמש: ";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(528, 48);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(41, 16);
			this.label2.TabIndex = 6;
			this.label2.Text = "הרשאות";
			// 
			// button2
			// 
			this.button2.Location = new System.Drawing.Point(160, 8);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(88, 23);
			this.button2.TabIndex = 7;
			this.button2.Text = "שמור נתונים";
			this.button2.Click += new System.EventHandler(this.button2_Click);
			// 
			// PermissionsForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.button1;
			this.ClientSize = new System.Drawing.Size(592, 373);
			this.Controls.Add(this.button2);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.pnlPermissions);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.cbUsers);
			this.Controls.Add(this.label1);
			this.Name = "PermissionsForm";
			this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "ניהול הרשאות משתמשים";
			this.Load += new System.EventHandler(this.PermissionsForm_Load);
			this.ResumeLayout(false);

		}
		#endregion

		private void PermissionsForm_Load(object sender, System.EventArgs e)
		{
			//fill all forms:
			FillForms();
			
			//fill all users:
			FillUsers();
			
			/*
			Button objButton;
			objButton=new Button();
			objButton.Text = "button1";
			objButton.Left = 100;
			objButton.Top = 250;
			pnlPermissions.Controls.Add(objButton);
			objButton=new Button();
			objButton.Text = "button2";
			objButton.Left = 100;
			objButton.Top = 550;
			pnlPermissions.Controls.Add(objButton);
			objButton=new Button();
			objButton.Text = "button3";
			objButton.Left = 100;
			objButton.Top = 850;
			pnlPermissions.Controls.Add(objButton);
			*/
		}

		private void FillUsers()
		{
			int i;
			cbUsers.Items.Clear();
			
			//get users array:
			ArrayList arrUsers=new ArrayList();
			Sport.Data.Entity[] users=Sport.Entities.User.Type.GetEntities(
				new EntityFilter((int) Sport.Entities.User.Fields.UserType, 
				(int) Sport.Types.UserType.Internal));
			for (i=0; i<users.Length; i++)
			{
				Sport.Entities.User user=new Sport.Entities.User(users[i]);
				if (CanChangePermissions(user, true))
					arrUsers.Add(user);
			}

			//sort by permissions:
			arrUsers.Sort(new PermissionsComparer());
			
			//add the users:
			foreach (Sport.Entities.User user in arrUsers)
				cbUsers.Items.Add(new ListItem(user.Login+" ("+user.Name+")", user.Id));
			
			//select is needed:
			int userid=Core.UserManager.CurrentUser.Id;
			if (Sport.Core.PermissionsManager.IsSuperUser(userid))
			{
				for (i=0; i<cbUsers.Items.Count; i++)
				{
					if (((int) (cbUsers.Items[i] as ListItem).Value) == userid)
					{
						cbUsers.SelectedIndex = i;
						break;
					}
				}
			}
			else
			{
				pnlPermissions.Enabled = false;
			}
		}

		private void FillForms()
		{
			//initialize service:
			PermissionService _service=new PermissionService();
			_service.CookieContainer = Sport.Core.Session.Cookies;
			
			/*
			//get forms:
			FormData[] arrForms=_service.GetFormsInfo();
			arrPermissionPanels = new ArrayList();

			//define dimensions:
			int elementsPerLine=3;
			int elementWidth=(int) (((double) pnlPermissions.Width)/(double) elementsPerLine);
			int captionHeight=40;
			int checkboxHeight=20;
			int elementHeight=captionHeight+(2*checkboxHeight)+2;
			int posLeft=pnlPermissions.Width-elementWidth;
			int posTop=0;
			
			for (int i=0; i<arrForms.Length; i++)
			{
				string strClassName=arrForms[i].ClassName;
				string strDescription=arrForms[i].Description;
				int checkboxWidth=((i%elementsPerLine) == 0)?(elementWidth-20):(elementWidth-5);

				//build panel:
				PermissionPanel objPermissionPanel = new PermissionPanel();
				objPermissionPanel.formClassname = strClassName;
				Panel objPanel=new Panel();
				objPanel.BorderStyle = BorderStyle.FixedSingle;
				objPanel.Left = posLeft;
				objPanel.Top = posTop;
				objPanel.Width = elementWidth;
				objPanel.Height = elementHeight;
				pnlPermissions.Controls.Add(objPanel);
				objPermissionPanel.panel = objPanel;
				
				//build caption:
				Label objLabel=new Label();
				//objLabel.Dock = DockStyle.Top;
				objLabel.Left = 0;
				objLabel.Top = 0;
				objLabel.Width = checkboxWidth;
				objLabel.Height = captionHeight;
				objLabel.TextAlign = ContentAlignment.MiddleCenter;
				Sport.Common.Tools.AssignFontSize(objLabel, 14);
				objLabel.Text = strDescription;
				Sport.Common.Tools.AttachTooltip(objLabel, strClassName);
				objPanel.Controls.Add(objLabel);

				//build checkbox: (readonly)
				CheckBox objCheckBox=new CheckBox();
				objCheckBox.Left = 0;
				objCheckBox.Top = captionHeight;
				objCheckBox.Width = checkboxWidth;
				objCheckBox.Height = checkboxHeight;
				//objCheckBox.TextAlign = ContentAlignment.MiddleCenter;
				objCheckBox.RightToLeft = RightToLeft.Yes;
				objCheckBox.Text = _strReadOnly;
				objCheckBox.CheckedChanged += new EventHandler(PermissionCheckbox_Changed);
				objPanel.Controls.Add(objCheckBox);
				objPermissionPanel.chkReadOnly = objCheckBox;
				
				//build checkbox: (full permission)
				objCheckBox=new CheckBox();
				objCheckBox.Left = 0;
				objCheckBox.Top = captionHeight+checkboxHeight;
				objCheckBox.Width = checkboxWidth;
				objCheckBox.Height = checkboxHeight;
				//objCheckBox.TextAlign = ContentAlignment.MiddleCenter;
				objCheckBox.RightToLeft = RightToLeft.Yes;
				objCheckBox.Text = _strFullPermissions;
				objCheckBox.CheckedChanged += new EventHandler(PermissionCheckbox_Changed);
				objPanel.Controls.Add(objCheckBox);
				objPermissionPanel.chkFullPerm = objCheckBox;
				
				//update position:
				if ((i > 0)&&(((i+1) % elementsPerLine) == 0))
				{
					posLeft = pnlPermissions.Width-elementWidth;
					posTop += elementHeight;
				}
				else
				{
					posLeft -= elementWidth;
					if (posLeft < 0)
						posLeft = 0;
				}
				
				arrPermissionPanels.Add(objPermissionPanel);
			}
			*/
		}

		/// <summary>
		/// load the permissions for the given user. 
		/// enables the permissions the current user can changes, 
		/// disables those he can't change.
		/// </summary>
		private void ApplyUserPermissions(int userid)
		{
			//get current user details:
			int currentUserID=Core.UserManager.CurrentUser.Id;
			bool blnSuperUser=Sport.Core.PermissionsManager.IsSuperUser(currentUserID);
			
			//initialize service:
			PermissionService _service=new PermissionService();
			_service.CookieContainer = Sport.Core.Session.Cookies;
			
			/*
			//get permissions list:
			PermissionData[] arrPermissions=null; //_service.GetUserPermissions(userid);
			PermissionData[] arrCurrentPermissions=null;
			if (currentUserID != userid)
				arrCurrentPermissions=null; //_service.GetUserPermissions(currentUserID);
			
			//iterate over the permission panels:
			foreach (PermissionPanel permissionPanel in arrPermissionPanels)
			{
				//reset checkboxes:
				permissionPanel.chkReadOnly.Checked = false;
				permissionPanel.chkFullPerm.Checked = false;

				//enable by default:
				permissionPanel.panel.Enabled = true;
				
				//search for current permission in the user permissions:
				foreach (PermissionData permissionData in arrPermissions)
				{
					if (permissionPanel.formClassname == permissionData.FormClassName)
					{
						switch (permissionData.Type)
						{
							case PermissionType.ReadOnly:
								permissionPanel.chkReadOnly.Checked = true;
								break;
							case PermissionType.Full:
								permissionPanel.chkReadOnly.Checked = true;
								permissionPanel.chkFullPerm.Checked = true;
								break;
						}
						break;
					}
				} //end loop over permissions from database
				
				//check if the current user can edit permission.

				//super user can do whatever he want!
				if (blnSuperUser)
					continue;

				//current user is not allowed to change his own permissions:
				if (currentUserID != userid)
				{
					//search for permission:
					bool canEdit=false;
					foreach (PermissionData curUserPerm in arrCurrentPermissions)
					{
						if (curUserPerm.FormClassName == permissionPanel.formClassname)
						{
							if (curUserPerm.Type == PermissionType.Full)
								canEdit = true;
							break;
						}
					}
					if (canEdit)
						continue;
				}
				
				//sorry, no luck:
				permissionPanel.panel.Enabled = false;
			} //end loop over permission panels

			_dataChanged = false;
			*/
		}

		/// <summary>
		/// return if current logged in user can change the given user permissions
		/// </summary>
		private bool CanChangePermissions(Sport.Entities.User user, bool viewOnly)
		{
			int currentUserID=Core.UserManager.CurrentUser.Id;
			int currentUserPermissions=Core.UserManager.CurrentUser.Permissions;
			
			//super user can do whatever he want!
			if (Sport.Core.PermissionsManager.IsSuperUser(currentUserID))
				return true;
			
			//user can't change his own permissions, but can view them:
			if (user.Id == currentUserID)
				return viewOnly;
			
			//user can't change permission or view permissions of those higher:
			return (currentUserPermissions > user.Permissions);
		}

		private void cbUsers_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (_changedByCode)
			{
				_changedByCode = false;
				return;
			}
			
			int selIndex=cbUsers.SelectedIndex;
			if (selIndex < 0)
			{
				pnlPermissions.Enabled = false;
				return;
			}

			//check if anything has changed:
			if ((_dataChanged)&&(_lastUserIndex >= 0))
			{
				//ask to confirm:
				bool userAns=Sport.UI.MessageBox.Ask("נתונים אחרונים לא נשמרו, האם להמשיך?", 
											"ניהול הרשאות", false);
				if (!userAns)
				{
					_changedByCode = true;
					cbUsers.SelectedIndex = _lastUserIndex;
					return;
				}
			}

			//store last selection:
			_dataChanged = false;
			_lastUserIndex = selIndex;
			
			Sport.UI.Dialogs.WaitForm.ShowWait("טוען נתונים אנא המתן...");
			cbUsers.Enabled = false;
			pnlPermissions.Enabled = true;
			int userid=(int) (cbUsers.Items[selIndex] as ListItem).Value;
			ApplyUserPermissions(userid);
			cbUsers.Enabled = true;
			Sport.UI.Dialogs.WaitForm.HideWait();
		}

		private void button2_Click(object sender, System.EventArgs e)
		{
			_dataChanged = false;
			
			//maybe not authorized?
			if ((pnlPermissions.Enabled == false)||(cbUsers.SelectedIndex < 0))
				return;
			
			//get user id:
			int userid=(int) (cbUsers.Items[cbUsers.SelectedIndex] as ListItem).Value;
			
			//save all permissions:
			ArrayList arrPermissionData=new ArrayList(); //array of PermissionData
			
			foreach (PermissionPanel permissionPanel in arrPermissionPanels)
			{
				if (permissionPanel.panel.Enabled)
				{
					/*
					PermissionData objPermissionData=new PermissionData();
					objPermissionData.User = userid;
					objPermissionData.FormID = -1;
					objPermissionData.FormClassName = permissionPanel.formClassname;
					bool blnReadOnly=permissionPanel.chkReadOnly.Checked;
					bool blnFullPerm=permissionPanel.chkFullPerm.Checked;
					if (blnFullPerm)
						objPermissionData.Type = PermissionType.Full;
					else if (blnReadOnly)
						objPermissionData.Type = PermissionType.ReadOnly;
					else
						objPermissionData.Type = PermissionType.None;
					arrPermissionData.Add(objPermissionData);
					*/
				}
			}

			/*
			if (arrPermissionData.Count > 0)
			{
				PermissionData[] arrPermissions=(PermissionData[]) 
					arrPermissionData.ToArray(typeof(PermissionData));
				
				//initialize service:
				PermissionService _service=new PermissionService();
				_service.CookieContainer = Sport.Core.Session.Cookies;
				
				//update all permissions:
				_service.UpdateUserPermissions(arrPermissions);

				Sport.UI.MessageBox.Show("נתונים נשמרו בהצלחה", "ניהול הרשאות", 
					MessageBoxIcon.Information);
			}
			*/
		}
		
		private void PermissionCheckbox_Changed(object sender, EventArgs e)
		{
			_dataChanged = true;

			if (sender == null)
				return;

			if (sender is CheckBox)
			{
				CheckBox objCheckbox=(CheckBox) sender;
				if (objCheckbox.Text == _strFullPermissions)
				{
					foreach (Control control in objCheckbox.Parent.Controls)
					{
						if (control is CheckBox)
						{
							if ((control as CheckBox).Text == _strReadOnly)
							{
								if (objCheckbox.Checked)
								{
									(control as CheckBox).Checked = true;
									(control as CheckBox).Enabled = false;
								}
								else
								{
									(control as CheckBox).Enabled = true;
								}
								break;
							} //end if readonly
						} //end if checkbox
					} //end loop over parent controls
				} //end if sender is the full permissions checkbox
			} //end if sender is checkbox
		}

		private class PermissionPanel
		{
			public string formClassname="";
			
			public Panel panel=null;
			public CheckBox chkReadOnly=null;
			public CheckBox chkFullPerm=null;

			public override bool Equals(object obj)
			{
				if (obj == null)
					return false;
				if (obj is PermissionPanel)
					return this.formClassname.Equals((obj as PermissionPanel).formClassname);
				if (obj is string)
					return this.formClassname.Equals(obj.ToString());
				return false;
			}

			public override int GetHashCode()
			{
				return formClassname.GetHashCode ();
			}
			
			public override string ToString()
			{
				return formClassname.ToString();
			}
		}

		private class PermissionsComparer : System.Collections.IComparer
		{
			public int Compare(object o1, object o2)
			{
				Sport.Entities.User user1 = (Sport.Entities.User) o1;
				Sport.Entities.User user2 = (Sport.Entities.User) o2;
				return user1.Permissions.CompareTo(user2.Permissions)*(-1);
			}
		}
	}
}
