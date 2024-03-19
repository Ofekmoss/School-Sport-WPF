using System;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Data;
using Sport.Data;
using Sport.UI;
using Sportsman.Core;
using Sport.UI.Controls;
using System.Collections;
using System.Collections.Generic;

namespace Sportsman.Views
{
	/// <summary>
	/// Summary description for UsersTableView.
	/// </summary>
	[Sport.UI.ViewPermissions(Sport.Types.Permission.Administration, true)]
	public class UsersTableView : Sport.UI.TableView
	{
		private ComboBoxFilter			regionFilter;
		private ComboBoxFilter			typeFilter;
		private EntityFilter			filter;
		private EntitySelectionDialog	schoolDialog;
		private Sport.UI.Controls.Style _noPasswordStyle = new Style(
			new System.Drawing.SolidBrush(System.Drawing.Color.Red), null, null);
		private ArrayList				_arrNoPassword=new ArrayList();

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
		}

		public UsersTableView()
		{
			Items.Add((int) Sport.Entities.User.Fields.Login, "זיהוי", 50);
			Items.Add((int) Sport.Entities.User.Fields.FirstName, "שם פרטי", 80);
			Items.Add((int) Sport.Entities.User.Fields.LastName, "שם משפחה", 80);
			Items.Add((int) Sport.Entities.User.Fields.Region, "מחוז", 120);
			Items.Add((int) Sport.Entities.User.Fields.School, "בית ספר", 150);
			Items.Add((int) Sport.Entities.User.Fields.UserType, "סוג", 120);
			Items.Add((int) Sport.Entities.User.Fields.Permissions, "הרשאות", 80);
			Items.Add((int) Sport.Entities.User.Fields.LastModified, "תאריך שינוי אחרון", 120);
			Items.Add((int) Sport.Entities.User.Fields.Email, "דואר אלקטרוני", 150);

			InitializeComponent();

			// search
			SearchBarEnabled = true;
		}
		
		protected override void OnSelectEntity(Entity entity)
		{
			if (entity != null)
			{
				Sport.Entities.User user = new Sport.Entities.User(entity);
				if (user.Region != null && schoolDialog != null)
				{				
					schoolDialog.View.State[Sport.Entities.Region.TypeName] = user.Region.Id.ToString();
					// Disabling school for central region
					if (user.Region.Id == Sport.Entities.Region.CentralRegion)
					{
						Fields[(int) Sport.Entities.User.Fields.School].CanEdit = false;
					}
					else
					{
						Fields[(int) Sport.Entities.User.Fields.School].CanEdit = true;
					}
				}
			}
		}
		
		public override void Open()
		{
			Title = "משתמשים";
			
			EntityListView = new EntityListView(Sport.Entities.User.TypeName);
			
			if (State[SelectionDialog] == "1")
				this.CanInsert = false;

			Fields[(int) Sport.Entities.User.Fields.Region].Values = Sport.Entities.Region.Type.GetEntities(null);

			if (IsViewPermitted(typeof(SchoolsTableView)))
			{
				schoolDialog = new EntitySelectionDialog(new SchoolsTableView());
				schoolDialog.View.State[SelectionDialog] = "1";

				Fields[(int) Sport.Entities.User.Fields.School].GenericItemType = Sport.UI.Controls.GenericItemType.Button;
				Fields[(int) Sport.Entities.User.Fields.School].Values = Sport.UI.Controls.GenericItem.ButtonValues(new ButtonBox.SelectValue(schoolDialog.ValueSelector));
			}
			else
			{
				Fields[(int) Sport.Entities.User.Fields.School].CanEdit = false;
			}

			EntityListView.Field field = Fields[(int) Sport.Entities.User.Fields.Permissions];
			field.GenericItemType = Sport.UI.Controls.GenericItemType.Button;
			field.Values = Sport.UI.Controls.GenericItem.ButtonValues(new ButtonBox.SelectValue(Forms.PermissionsSelectionDialog.ValueSelector));

			if (State["ParentView"] == "Regions")
				Columns = new int[] { 0, 1, 2, 5, 8 };
			else
				Columns = new int[] { 0, 1, 2, 3, 4, 5, 6, 8 };
			Details = new int[] { 0, 1, 2, 3, 4, 5, 6, 8 };
			//Sort	= new int[] { 6 };

			Searchers.Add(new Searcher("זיהוי משתמש:", EntityListView.EntityType.Fields[(int)Sport.Entities.User.Fields.Login], 100));

			EntityType regionType = Sport.Entities.Region.Type;
			Entity[] regions = regionType.GetEntities(null);
			regionFilter = new ComboBoxFilter("מחוז:", regions, null, "<כל המחוזות>");
			regionFilter.FilterChanged += new System.EventHandler(RegionFiltered);
			Filters.Add(regionFilter);
			
			typeFilter = new ComboBoxFilter("סוג:", new Sport.Types.UserTypeLookup().Items, null, "<כל הסוגים>");
			typeFilter.FilterChanged += new EventHandler(TypeFiltered);
			Filters.Add(typeFilter);
			
			RefreshFilters();
			
			Requery();

			if (this.CanInsert && this.EntityListView != null && this.EntityListView.Count > 0)
			{
				List<Entity> externalUsersWithoutSchool = new List<Entity>();
				for (var i = 0; i < this.EntityListView.Count; i++)
				{
					Entity curUser = this.EntityListView[i];
					var isExternal = (int)curUser.Fields[(int)Sport.Entities.User.Fields.UserType] == (int)Sport.Types.UserType.External;
					var missingSchool = Sport.Common.Tools.CIntDef(curUser.Fields[(int)Sport.Entities.User.Fields.School], -1) == -1;
					if (isExternal && missingSchool)
					{
						externalUsersWithoutSchool.Add(curUser);
					}
				}
				if (externalUsersWithoutSchool.Count > 0)
				{
					var strMessage = "קיימים משתמשים חיצוניים עבורם לא מוגדר בית ספר. משתמשים אלו לא יכולים לעשות שימוש במערכת הרישום. להלן הרשימה:\n";
					strMessage += string.Join("\n", externalUsersWithoutSchool.ConvertAll<string>(u =>
					{
						return u.Fields[(int)Sport.Entities.User.Fields.Login] + " (" + u.Name + ")";
					}).ToArray());
					Sport.UI.MessageBox.Show(strMessage, "אזהרה", MessageBoxIcon.Warning);
				}
			}

			base.Open();
		} //end function Open
		
		protected override Style GetGridStyle(int row, int field, GridDrawState state)
		{
			if ((state != GridDrawState.Selected)&&(row >= 0)&&
				(this.EntityListView != null)&&(row < this.EntityListView.Count))
			{
				Entity userEnt=this.EntityListView[row];
				if (_arrNoPassword.IndexOf(userEnt.Id) >= 0)
					return _noPasswordStyle;
			}
			
			return base.GetGridStyle(row, field, state);
		}

		protected override void OnValueChange(EntityEdit entityEdit, EntityField entityField)
		{
			if (entityEdit.Entity != null && entityEdit.Entity.Id > 0 && entityField.Index == (int)Sport.Entities.User.Fields.UserType)
			{
				//prevent user from changing type to admin due to weak password risk.
				if ((int)entityEdit.Fields[(int)Sport.Entities.User.Fields.UserType] == (int)Sport.Types.UserType.Internal)
				{
					Sport.UI.MessageBox.Error("על מנת להגדיר משתמש התאחדות יש ליצור משתמש חדש", "שגיאה");
					entityEdit.Fields[(int)Sport.Entities.User.Fields.UserType] = entityEdit.Entity.Fields[(int)Sport.Entities.User.Fields.UserType];
				}
				
			}
			base.OnValueChange(entityEdit, entityField);
		}

		protected override bool OnDeleteEntity(Entity entity)
		{
			if (State[SelectionDialog] == "1")
				return false;
			if (entity == null)
				return false;
			if (entity.Id == Core.UserManager.CurrentUser.Id)
			{
				Sport.UI.MessageBox.Error("לא ניתן למחוק את עצמך", "מחיקת משתמש");
				return false;
			}
			
			Sport.Entities.User user=new Sport.Entities.User(entity);
			
			string strMessage=user.CanDelete();
			if (strMessage.Length > 0)
			{
				Sport.UI.MessageBox.Show(strMessage, "מחיקת משתמש", MessageBoxIcon.Warning);
				return false;
			}
			
			//instant messages
			Sport.Entities.InstantMessage[] messages=user.GetInstantMessages();
			if ((messages != null)&&(messages.Length > 0))
			{
				int sentCount=0;
				int receivedCount=0;
				foreach (Sport.Entities.InstantMessage message in messages)
				{
					sentCount += (message.Sender.Id == user.Id)?1:0;
					receivedCount += (message.Recipient.Id == user.Id)?1:0;
				}
				string strWarning="אזהרה: מחיקת משתמש זה תוביל גם למחיקת";
				if (sentCount > 0)
				{
					strWarning += " "+Sport.Common.Tools.BuildOneOrMany(
						"הודעה", "הודעות", sentCount, false)+" אשר שלח המשתמש";
				}
				if (receivedCount > 0)
				{
					strWarning += " ";
					if (sentCount > 0)
						strWarning += "ו";
					strWarning += " "+Sport.Common.Tools.BuildOneOrMany(
						"הודעה", "הודעות", receivedCount, false)+" אשר נשלחו אל המשתמש";
				}
				Sport.UI.MessageBox.Warn(strWarning, "מחיקת משתמש");
			}
			
			bool userAns=
				Sport.UI.MessageBox.Ask("האם למחוק את המשתמש '" + entity.Name + "'?", false);
			if ((userAns)&&(messages != null)&&(messages.Length > 0))
			{
				foreach (Sport.Entities.InstantMessage message in messages)
				{
					message.Entity.Delete();
				}
			}
			return userAns;
		}
		
		public override MenuItem[] GetContextMenu(Sport.UI.TableView.SelectionType selectionType)
		{
			if (State[SelectionDialog] == "1")
				return null;
			
			MenuItem[] menuItems = null;
			switch (selectionType)
			{
				case (SelectionType.Single):
					menuItems = new MenuItem[3];
					menuItems[0] = new MenuItem("פתח", new System.EventHandler(OnOpenUser));
					menuItems[0].DefaultItem = true;
					menuItems[1] = new MenuItem("-");
					menuItems[2] = new MenuItem("שינוי סיסמא", new System.EventHandler(OnChangePassword));
					break;
			}
			
			return menuItems;
		}
		
		private void RefreshFilters()
		{
			if (filter == null)
				filter = new EntityFilter();
			filter.Clear();
			
			object region = Core.Tools.GetStateValue(State[Sport.Entities.Region.TypeName]);
			object userType = Core.Tools.GetStateValue(State["UserType"]);
			
			if (region != null)
			{
				filter.Add(new EntityFilterField((int) Sport.Entities.User.Fields.Region, (int) region));
				regionFilter.Value = Sport.Entities.Region.Type.Lookup((int) region);
			}
			
			if (userType != null)
			{
				int type=(int) userType;
				filter.Add(new EntityFilterField((int) Sport.Entities.User.Fields.UserType, type));
				LookupItem[] items=new Sport.Types.UserTypeLookup().Items;
				foreach (LookupItem item in items)
				{
					if (item.Id == type)
					{
						typeFilter.Value = item;
						break;
					}
				}
			}
		} //end function RefreshFilters
		
		private void Requery()
		{
			Cursor c = Cursor.Current;
			Cursor.Current = Cursors.WaitCursor;
			
			EntityListView.Read(filter);
			
			_arrNoPassword.Clear();
			/*
			int[] arrUsersWithoutPassword=
				Sport.Entities.User.GetUsersWithoutPassword();
			foreach (int userID in arrUsersWithoutPassword)
				_arrNoPassword.Add(userID);
			*/
			if ((EntityListView != null)&&(EntityListView.EntityList != null))
			{
				string names="";
				for (int i=0; i<EntityListView.EntityList.Count; i++)
				{
					Entity curEntity=EntityListView.EntityList[i];
					int curUserID=curEntity.Id;
					if (_arrNoPassword.IndexOf(curUserID) >= 0)
					{
						int firstNameIndex=(int) Sport.Entities.User.Fields.FirstName;
						int lastNameIndex=(int) Sport.Entities.User.Fields.LastName;
						string strFirstName=Sport.Common.Tools.CStrDef(
							curEntity.Fields[firstNameIndex], "");
						string strLastName=Sport.Common.Tools.CStrDef(
							curEntity.Fields[lastNameIndex], "");
						string userName=strFirstName;
						if (strLastName.Length > 0)
							userName += " "+strLastName;
						names += userName+"\n";
					}
				}
				
				if (names.Length > 0)
				{
					Sport.UI.MessageBox.Warn("לא מוגדרת סיסמא עבור המשתמשים הבאים:\n"+names+"\n\n"+
						"משתמשים אלו לא יוכלו להשתמש בתוכנה או בכלי הניהול באתר", "ניהול משתמשים");
				}
			}
			
			Entity region=null;
			if (regionFilter != null)
				region = (Entity)regionFilter.Value;
			string strTitle="";
			if (State[SelectionDialog] == "1")
				strTitle += "בחירת משתמש";
			else
				strTitle += "משתמשים";
			strTitle += (region != null)?" - "+region.Name:"";
			Title = strTitle;
			Cursor.Current = c;
		} //end function Requery
		
		private void TypeFiltered(object sender, EventArgs e)
		{
			if (typeFilter.Value == null)
			{
				State["UserType"] = null;
			}
			else
			{
				State["UserType"] = ((LookupItem) typeFilter.Value).Id.ToString();
			}
			RefreshFilters();
			Requery();
		}

		private void RegionFiltered(object sender, EventArgs e)
		{
			if (regionFilter.Value == null)
			{
				State[Sport.Entities.Region.TypeName] = null;
			}
			else
			{
				State[Sport.Entities.Region.TypeName] = ((Entity)regionFilter.Value).Id.ToString();
			}
			RefreshFilters();
			Requery();
		}
		
		private void OnOpenUser(object sender, EventArgs e)
		{
			//open details of selected user in new dialog.
		}
		
		private void OnChangePassword(object sender, EventArgs e)
		{
			Entity ent=Current;
			if (ent != null)
			{
				//int userid=ent.Id;
				string username=ent.Fields[(int)Sport.Entities.User.Fields.Login].ToString();
				if (UserManager.ChangeUserPassword(username))
				{
					Sport.UI.MessageBox.Show("סיסמא שונתה בהצלחה", "הודעת מערכת", 
						MessageBoxButtons.OK, MessageBoxIcon.Information);
				}
			}
		}
	} //end class UsersTableView
}
