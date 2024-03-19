using System;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Data;
using Sport.Data;
using Sport.UI;
using Sportsman.Core;
using Sport.UI.Controls;

namespace Sportsman.Views
{
	/// <summary>
	/// Summary description for MessagesTableView.
	/// </summary>
	public class MessagesTableView : Sport.UI.TableView
	{
		private LookupType lookupMessages;
		private LookupType lookupStatuses;
		
		private Sport.UI.TableView.ComboBoxFilter userFilter;
		private Sport.UI.TableView.ComboBoxFilter messageTypeFilter;
		private Sport.UI.TableView.ComboBoxFilter messageStatusFilter;
		
		private EntityFilter filter;
		
		public MessagesTableView()
		{
			Items.Add((int) Sport.Entities.Message.Fields.User, "משתמש", 100);
			Items.Add((int) Sport.Entities.Message.Fields.Type, "סוג הודעה", 80);
			Items.Add((int) Sport.Entities.Message.Fields.Text, "פרטים", 200, 70);
			Items.Add((int) Sport.Entities.Message.Fields.TimeSent, "זמן שליחה", 100);
			Items.Add((int) Sport.Entities.Message.Fields.TimeRead, "זמן קריאה", 100);
			Items.Add((int) Sport.Entities.Message.Fields.Status, "סטטוס", 80);
		}
		
		public override void Open()
		{
			Title = "הודעות";
			EntityListView = new EntityListView(Sport.Entities.Message.Type);
			this.CanInsert = false;
			
			Fields[(int) Sport.Entities.Message.Fields.User].Values = Sport.Entities.User.Type.GetEntities(null);
			lookupMessages = new Sport.Types.MessageTypeLookup();
			lookupStatuses = new Sport.Types.MessageStatusLookup();
			
			Columns = new int[] { 0, 1, 2, 3, 4, 5 };
			Details = new int[] { 2 };
			
			userFilter = new ComboBoxFilter("משתמש:", GetAllUsers(), Sport.Entities.User.Type.Lookup(UserManager.CurrentUser.Id), "<כל המשתמשים>");
			userFilter.FilterChanged += new System.EventHandler(UserFiltered);
			Filters.Add(userFilter);
			
			messageTypeFilter = new ComboBoxFilter("סוג הודעה:", lookupMessages.Items, null, "<כל הסוגים>");
			messageTypeFilter.FilterChanged += new System.EventHandler(MessageTypeFiltered);
			Filters.Add(messageTypeFilter);
			
			Sport.Data.LookupItem newStatusItem=null;
			foreach (Sport.Data.LookupItem item in lookupStatuses.Items)
			{
				if (item.Id == (int) Sport.Types.MessageStatus.New)
				{
					newStatusItem = item;
					break;
				}
			}
			messageStatusFilter = new ComboBoxFilter("סטטוס הודעה:", lookupStatuses.Items, newStatusItem, "<כל הסוגים>");
			messageStatusFilter.FilterChanged += new System.EventHandler(MessageStatusFiltered);
			Filters.Add(messageStatusFilter);
			
			State[Sport.Entities.User.TypeName] = UserManager.CurrentUser.Id.ToString();
			State["MessageStatus"] = ((int) Sport.Types.MessageStatus.New).ToString();
			
			RefreshFilters();
			Requery();
			
			base.Open();
		} //end function Open
		
		/// <summary>
		/// returns list of all users which have any messages, to be used as filter.
		/// </summary>
		private Entity[] GetAllUsers()
		{
			System.Collections.ArrayList usersList=new System.Collections.ArrayList();
			Entity[] result;
			EntityFilter newMessageFilter=new EntityFilter((int) Sport.Entities.Message.Fields.Status,
				(int) Sport.Types.MessageStatus.New);
			Entity[] messages=Sport.Entities.Message.Type.GetEntities(newMessageFilter);
			for (int i=0; i<messages.Length; i++)
			{

				Entity user=null;
				try
				{
					user = Sport.Entities.User.Type.Lookup((int) messages[i].Fields[(int) Sport.Entities.Message.Fields.User]);
					if (usersList.Contains(user) == false)
						usersList.Add(user);
				}
				catch
				{}
			}
			result = new Entity[usersList.Count];
			for (int j=0; j<usersList.Count; j++)
				result[j] = (Entity) usersList[j];

			return result;
		}
		
		protected override bool OnDeleteEntity(Entity entity)
		{
			return false;
			//return Sport.UI.MessageBox.Ask("האם למחוק את המשתמש '" + entity.Name + "'?", false);
		}
		
		public override MenuItem[] GetContextMenu(Sport.UI.TableView.SelectionType selectionType)
		{
			MenuItem[] menuItems = null;
			switch (selectionType)
			{
				case (SelectionType.Single):
					menuItems = new MenuItem[2];
					menuItems[0] = new MenuItem("פתח", new System.EventHandler(OnOpenMessage));
					menuItems[0].DefaultItem = true;
					menuItems[1] = new MenuItem("-");
					break;
			}
			
			return menuItems;
		}
		
		private void RefreshFilters()
		{
			filter = new EntityFilter();
			
			object user = Core.Tools.GetStateValue(State[Sport.Entities.User.TypeName]);
			object messageType = Core.Tools.GetStateValue(State["MessageType"]);
			object messageStatus = Core.Tools.GetStateValue(State["MessageStatus"]);
			
			if (user != null)
			{
				filter.Add(new EntityFilterField((int) Sport.Entities.Message.Fields.User, (int) user));
				userFilter.Value = Sport.Entities.User.Type.Lookup((int) user);
			}
			else
			{
				userFilter.Value = null;
			}
			
			if (messageType != null)
			{
				filter.Add(new EntityFilterField((int) Sport.Entities.Message.Fields.Type, (int) messageType));
				//messageTypeFilter.Value = lookupMessages.Lookup((int) messageType);
			}
			else
			{
				messageTypeFilter.Value = null;
			}
			
			if (messageStatus != null)
			{
				filter.Add(new EntityFilterField((int) Sport.Entities.Message.Fields.Status, (int) messageStatus));
				//messageStatusFilter.Value = lookupStatuses.Lookup((int) messageStatus);
			}
			else
			{
				messageStatusFilter.Value = null;
			}
		} //end function RefreshFilters

		private void UserFiltered(object sender, EventArgs e)
		{
			if (userFilter.Value == null)
			{
				State[Sport.Entities.User.TypeName] = null;
			}
			else
			{
				State[Sport.Entities.User.TypeName] = ((Entity)userFilter.Value).Id.ToString();
			}
			
			RefreshFilters();
			Requery();
		}

		private void MessageTypeFiltered(object sender, EventArgs e)
		{
			if (messageTypeFilter.Value == null)
			{
				State["MessageType"] = null;
			}
			else
			{
				State["MessageType"] = ((LookupItem) messageTypeFilter.Value).Id.ToString();
			}
			
			RefreshFilters();
			Requery();
		}
		
		private void MessageStatusFiltered(object sender, EventArgs e)
		{
			if (messageStatusFilter.Value == null)
			{
				State["MessageStatus"] = null;
			}
			else
			{
				State["MessageStatus"] = ((LookupItem) messageStatusFilter.Value).Id.ToString();
			}
			
			RefreshFilters();
			Requery();
		}
		
		private void Requery()
		{
			Cursor c = Cursor.Current;
			Cursor.Current = Cursors.WaitCursor;
			EntityListView.Read(filter);
			
			Title = "הודעות מערכת";
			if ((userFilter != null)&&(userFilter.Value != null))
				Title += " - "+(userFilter.Value as Entity).Name;
			
			Cursor.Current = c;
		} //end function Requery

		private void OnOpenMessage(object sender, EventArgs e)
		{
			//open details of selected message in new dialog.
		}
	} //end class MessagesTableView
}
