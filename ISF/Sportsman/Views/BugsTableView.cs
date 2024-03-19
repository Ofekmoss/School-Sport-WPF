using System;
using System.Windows.Forms;
using Sport.Data;
using Sport.UI;

namespace Sportsman.Views
{
	/// <summary>
	/// Summary description for BugsTableView.
	/// </summary>
	public class BugsTableView : TableView
	{

		ComboBoxFilter userFilter;
		ComboBoxFilter statusFilter;
		ComboBoxFilter typeFilter;
		
		private enum ColumnTitles
		{
			Title=0,
			Date,
			Status,
			Description,
			User,
			Type,
			LastModified
		}


		public BugsTableView()
		{
			Items.Add((int) Sport.Entities.Bug.Fields.Title, "כותרת", 300);
			Items.Add((int) Sport.Entities.Bug.Fields.Date, "תאריך", 120);
			Items.Add((int) Sport.Entities.Bug.Fields.Status, "סטטוס", 100);
			Items.Add((int) Sport.Entities.Bug.Fields.Description, "תיאור", 500, 150);
			Items.Add((int) Sport.Entities.Bug.Fields.User, "נכתב ע\"י", 120);
			Items.Add((int) Sport.Entities.Bug.Fields.Type, "סוג הערה", 120);
			Items.Add((int) Sport.Entities.Bug.Fields.LastModified, "תאריך שינוי אחרון", 120);
		}

		public override void Open()
		{
			EntityListView = new EntityListView(Sport.Entities.Bug.TypeName);

			Columns = new int[] { (int) ColumnTitles.User, (int) ColumnTitles.Title, 
						(int) ColumnTitles.Date, (int) ColumnTitles.Type, 
						(int) ColumnTitles.Status };
			
			Details = new int[] { (int) ColumnTitles.Title, 
								(int) ColumnTitles.Description, 
								(int) ColumnTitles.LastModified};
			
			userFilter = new ComboBoxFilter("נכתב ע\"י:", GetUsers(), null, "<כל המשתמשים>");
			userFilter.FilterChanged += new EventHandler(UserFilterChanged);
			Filters.Add(userFilter);
			
			statusFilter = new ComboBoxFilter("סטטוס:", Sport.Types.BugStatusLookup.statuses, null, "<הכל>");
			statusFilter.FilterChanged += new EventHandler(StatusFilterChanged);
			Filters.Add(statusFilter);
			
			typeFilter = new ComboBoxFilter("סוג:", Sport.Types.BugTypeLookup.types, null, "<הכל>");
			typeFilter.FilterChanged += new EventHandler(TypeFilterChanged);
			Filters.Add(typeFilter);

			Requery();

			base.Open ();
		}

		protected override void OnSelectEntity(Entity entity)
		{
			Fields[(int) Sport.Entities.Bug.Fields.Description].CanEdit = true;
			Fields[(int) Sport.Entities.Bug.Fields.Title].CanEdit = true;
			if ((entity != null)&&(entity.Id > 0))
			{
				//don't let user change other user's remark...
				int loggedInUserID=Core.UserManager.CurrentUser.Id;
				int selectedUserID=Sport.Common.Tools.CIntDef(
					entity.Fields[(int) Sport.Entities.Bug.Fields.User], -1);
				if ((!Sport.Core.PermissionsManager.IsSuperUser(loggedInUserID))&&
					(selectedUserID >= 0))
				{
					bool canEdit=(loggedInUserID == selectedUserID);
					Fields[(int) Sport.Entities.Bug.Fields.Description].CanEdit = canEdit;
					Fields[(int) Sport.Entities.Bug.Fields.Title].CanEdit = canEdit;
				}
			}
		}


		/// <summary>
		/// returns all users who wrote remarks
		/// </summary>
		private Entity[] GetUsers()
		{
			Entity[] bugs=Sport.Entities.Bug.Type.GetEntities(null);
			System.Collections.ArrayList usersList=new System.Collections.ArrayList();
			Entity[] result;
			
			for (int i=0; i<bugs.Length; i++)
			{
				Sport.Entities.Bug bug=new Sport.Entities.Bug(bugs[i]);
				if (bug.WrittenBy >= 0)
				{
					Sport.Entities.User user=new Sport.Entities.User(bug.WrittenBy);
					if ((user.IsValid())&&(!usersList.Contains(user)))
					{
						usersList.Add(user);
					}
				}
			}
			result = new Entity[usersList.Count];
			for (int j=0; j<usersList.Count; j++)
				result[j] = (usersList[j] as Sport.Entities.User).Entity;

			return result;
		}

		protected override bool OnDeleteEntity(Entity entity)
		{
			if ((entity == null)||(entity.Id < 0))
				return false;
			
			//don't let user delete remark of user with higher permissions...
			int loggedInUserID=Core.UserManager.CurrentUser.Id;
			int loggedInUserPermissions=Core.UserManager.CurrentUser.Permissions;
			int selectedUserID=Sport.Common.Tools.CIntDef(
				entity.Fields[(int) Sport.Entities.Bug.Fields.User], -1);
			int selectedUserPermissions=0;
			try
			{
				Sport.Entities.User selectedUser=
					new Sport.Entities.User(selectedUserID);
				selectedUserPermissions=selectedUser.Permissions;
			}
			catch{}
			if ((!Sport.Core.PermissionsManager.IsSuperUser(loggedInUserID))&&
				(selectedUserID >= 0))
			{
				if ((selectedUserPermissions > loggedInUserPermissions)||
					((selectedUserPermissions == loggedInUserPermissions)&&
					(selectedUserID != loggedInUserID)))
				{
					Sport.UI.MessageBox.Error("אינך מורשה למחוק הערה זו", "מחיקת הערות");
					return false;
				}
			}
			return Sport.UI.MessageBox.Ask("האם למחוק את ההערה '" + entity.Name + "'?", false);
		}

		protected override void OnNewEntity(EntityEdit entityEdit)
		{
			EntityField entField;
			
			//change the user field:
			entField = EntityListView.EntityType.Fields[(int) Sport.Entities.Bug.Fields.User];
			entField.SetValue(EntityListView.EntityEdit, Sport.Entities.User.Type.Lookup(Core.UserManager.CurrentUser.Id));
		}

		private void UserFilterChanged(object sender, EventArgs e)
		{
			Requery();
		}

		private void StatusFilterChanged(object sender, EventArgs e)
		{
			Requery();
		}

		private void TypeFilterChanged(object sender, EventArgs e)
		{
			Requery();
		}

		private void Requery()
		{
			Cursor c = Cursor.Current;
			Cursor.Current = Cursors.WaitCursor;
			string title="הערות";

			EntityFilter filter = new EntityFilter();

			if (userFilter.Value != null)
			{
				filter.Add(new EntityFilterField((int) Sport.Entities.Bug.Fields.User, 
					(userFilter.Value as Entity).Id));
				title += " - " + userFilter.Value.ToString();
			}

			if (statusFilter.Value != null)
			{
				filter.Add(new EntityFilterField((int) Sport.Entities.Bug.Fields.Status, 
					((LookupItem)statusFilter.Value).Id));
				title += " - " + statusFilter.Value.ToString();
			}
			else
			{
				//display all except old remarks...
				filter.Add(new EntityFilterField((int) Sport.Entities.Bug.Fields.Status, 
					(int) Sport.Types.BugStatus.Old, true));
			}
			
			if (typeFilter.Value != null)
			{
				filter.Add(new EntityFilterField((int) Sport.Entities.Bug.Fields.Type, 
					((LookupItem)typeFilter.Value).Id));
				title += " - " + typeFilter.Value.ToString();
			}

			Title = title;
			EntityListView.Read(filter);
			Cursor.Current = c;
		}
	}
}
