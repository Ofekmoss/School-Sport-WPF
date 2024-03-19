using System;

namespace Sportsman.Forms
{
	public class PermissionsSelectionDialog : System.Windows.Forms.Form
	{
		private class PermissionItem
		{
			private Sport.Types.Permission _permission;
			public Sport.Types.Permission Permission
			{
				get { return _permission; }
			}

			private string _name;
			public string Name
			{
				get { return _name; }
			}

			public PermissionItem(Sport.Types.Permission permission, string name)
			{
				_permission = permission;
				_name = name;
			}

			public override string ToString()
			{
				return _name;
			}

		}

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.CheckedListBox clbPermissions;

		private int _permissions;
		public int Permissions
		{
			get { return _permissions; }
			set { _permissions = value; }
		}

		public PermissionsSelectionDialog()
		{
			_permissions = 0;

			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			// Adding permissions
			for (int n = 0; n < Sport.Types.PermissionTypeLookup.Permissions.Length; n++)
			{
				clbPermissions.Items.Add(new PermissionItem((Sport.Types.Permission) n, Sport.Types.PermissionTypeLookup.Permissions[n]));
			}
		}
        
		public PermissionsSelectionDialog(int permissions)
			: this()
		{
			_permissions = permissions;
			SetPermissions();
		}

		private bool settingPermissions = false;
		private void SetPermissions()
		{
			settingPermissions = true;
			for (int n = 0; n < clbPermissions.Items.Count; n++)
			{
				PermissionItem pi = (PermissionItem) clbPermissions.Items[n];
				clbPermissions.SetItemChecked(n, Sport.Types.PermissionTypeLookup.Contains(_permissions, pi.Permission));
			}
			settingPermissions = false;
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
			this.clbPermissions = new System.Windows.Forms.CheckedListBox();
			this.tbOk = new System.Windows.Forms.Button();
			this.tbCancel = new System.Windows.Forms.Button();
			this.panel.SuspendLayout();
			this.SuspendLayout();
			// 
			// panel
			// 
			this.panel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel.Controls.Add(this.clbPermissions);
			this.panel.Controls.Add(this.tbOk);
			this.panel.Controls.Add(this.tbCancel);
			this.panel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.panel.Location = new System.Drawing.Point(0, 0);
			this.panel.Name = "panel";
			this.panel.Size = new System.Drawing.Size(216, 192);
			this.panel.TabIndex = 7;
			this.panel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.panelMouseUp);
			this.panel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panelMouseMove);
			this.panel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panelMouseDown);
			// 
			// clbPermissions
			// 
			this.clbPermissions.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.clbPermissions.Location = new System.Drawing.Point(8, 8);
			this.clbPermissions.Name = "clbPermissions";
			this.clbPermissions.Size = new System.Drawing.Size(200, 146);
			this.clbPermissions.TabIndex = 12;
			this.clbPermissions.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.clbPermissions_ItemCheck);
			// 
			// tbOk
			// 
			this.tbOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.tbOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.tbOk.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.tbOk.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.tbOk.Location = new System.Drawing.Point(8, 166);
			this.tbOk.Name = "tbOk";
			this.tbOk.Size = new System.Drawing.Size(68, 20);
			this.tbOk.TabIndex = 11;
			this.tbOk.Text = "אישור";
			// 
			// tbCancel
			// 
			this.tbCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.tbCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.tbCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.tbCancel.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.tbCancel.Location = new System.Drawing.Point(80, 166);
			this.tbCancel.Name = "tbCancel";
			this.tbCancel.Size = new System.Drawing.Size(68, 20);
			this.tbCancel.TabIndex = 10;
			this.tbCancel.Text = "ביטול";
			// 
			// PermissionsSelectionDialog
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(216, 192);
			this.ControlBox = false;
			this.Controls.Add(this.panel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Name = "PermissionsSelectionDialog";
			this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "שדות";
			this.panel.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private System.Windows.Forms.Button tbOk;
		private System.Windows.Forms.Button tbCancel;
		private System.Windows.Forms.Panel panel;

		private void clbPermissions_ItemCheck(object sender, System.Windows.Forms.ItemCheckEventArgs e)
		{
			if (!settingPermissions)
			{
				PermissionItem pi = (PermissionItem) clbPermissions.Items[e.Index];
				if (e.NewValue == System.Windows.Forms.CheckState.Checked)
				{
					_permissions = Sport.Types.PermissionTypeLookup.Add(_permissions, pi.Permission);
				}
				else
				{
					_permissions = Sport.Types.PermissionTypeLookup.Remove(_permissions, pi.Permission);
				}
			}
		}

		bool buttonDown = false;
		System.Drawing.Point buttonPoint;

		protected override void OnLostFocus(EventArgs e)
		{
			buttonDown = false;
			base.OnLostFocus (e);
		}

		private void panelMouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (buttonDown)
			{
				System.Drawing.Point pt = PointToScreen(new System.Drawing.Point(e.X, e.Y));
				Location = new System.Drawing.Point(pt.X - buttonPoint.X, pt.Y - buttonPoint.Y);
			}
		
		}

		private void panelMouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (e.Button == System.Windows.Forms.MouseButtons.Left)
			{
				buttonDown = true;
				buttonPoint = new System.Drawing.Point(e.X, e.Y);
			}
		}

		private void panelMouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			buttonDown = false;
		}

	
		public static bool EditPermissions(ref int permissions)
		{
			PermissionsSelectionDialog psd = new PermissionsSelectionDialog(permissions);

			if (psd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				permissions = psd.Permissions;

				return true;
			}

			return false;
		}

		public static object ValueSelector(Sport.UI.Controls.ButtonBox buttonBox, object value)
		{
			int permissions;
			if (value == null)
				permissions = 0;
			else if (value is int)
				permissions = (int) value;
			else if (value is Sport.Data.LookupItem)
				permissions = ((Sport.Data.LookupItem) value).Id;
			else
				return value;

			if (EditPermissions(ref permissions))
			{
				if (value is int)
					return permissions;
				return Sport.Types.PermissionTypeLookup.ToLookupItem(permissions);
			}

			return value;
		}
	}
}
