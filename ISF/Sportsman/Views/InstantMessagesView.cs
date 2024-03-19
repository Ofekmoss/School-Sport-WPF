using System;
using Sport.UI;
using Sportsman.Details;

namespace Sportsman.Views
{
	/// <summary>
	/// Summary description for InstantMessagesView.
	/// </summary>
	[Sport.UI.ViewPermissions(Sport.Types.Permission.General)]
	public class InstantMessagesView : TableView2
	{
		public InstantMessagesView()
			: base (new Entities.InstantMessageView())
		{
			//constructor code here.
		}
		
		#region Filters
		private ComboBoxFilter userTypeFilter;
		private ComboBoxFilter recipientFilter;
		private ComboBoxFilter senderFilter;
		
		private void CreateFilters()
		{
			Sport.Types.UserTypeLookup userLookup=new Sport.Types.UserTypeLookup();
			userTypeFilter = new ComboBoxFilter("סוג משתמש:", userLookup.Items, null, "<בחר סוג משתמש>", 180);
			userTypeFilter.FilterChanged += new EventHandler(UserTypeFiltered);
			
			recipientFilter = new ComboBoxFilter("נשלח אל:", null, null, "<כל המשתמשים>", 180);
			recipientFilter.Parameters.Add(EntityListView.EntityQuery.Parameters[0]);
			recipientFilter.FilterChanged += new EventHandler(RecipientFiltered);
			
			Sport.Data.EntityFilter filter = new Sport.Data.EntityFilter(
				(int) Sport.Entities.User.Fields.Id, Core.UserManager.CurrentUser.Id);
			Sport.Data.Entity[] senders=Sport.Entities.User.Type.GetEntities(filter);
			senderFilter = new ComboBoxFilter("מאת:", senders, senders[0], "<בחר משתמש>", 180);
			senderFilter.Parameters.Add(EntityListView.EntityQuery.Parameters[1]);
			senderFilter.FilterChanged += new EventHandler(SenderFiltered);
			
			Filters.Add(userTypeFilter);
			Filters.Add(recipientFilter);
			Filters.Add(senderFilter);
			
			foreach (Sport.Data.LookupItem item in userLookup.Items)
			{
				if (item.Id == (int) Sport.Types.UserType.Internal)
				{
					userTypeFilter.Value = item;
					break;
				}
			}
		}
		
		private void UserTypeFiltered(object sender, EventArgs e)
		{
			recipientFilter.SetValues(null);
			if (userTypeFilter.Value != null)
			{
				int userType=((Sport.Data.LookupItem) userTypeFilter.Value).Id;
				Sport.Data.EntityFilter filter=new Sport.Data.EntityFilter(
					(int) Sport.Entities.User.Fields.UserType, userType);
				Sport.Data.Entity[] users=Sport.Entities.User.Type.GetEntities(filter);
				recipientFilter.SetValues(users);
			}
			recipientFilter.Value = null;
		}
		
		private void RecipientFiltered(object sender, EventArgs e)
		{
			Sport.Data.EntityFilter filter=new Sport.Data.EntityFilter(
				(int) Sport.Entities.User.Fields.Id, Core.UserManager.CurrentUser.Id);
			Sport.Data.Entity[] senders=Sport.Entities.User.Type.GetEntities(filter);
			if (recipientFilter.Value == null)
			{
				State["recipient"] = null;
				senderFilter.SetValues(senders);
				senderFilter.Value = senders[0];
			}
			else
			{
				int recipientID=((Sport.Data.Entity) recipientFilter.Value).Id;
				State["recipient"] = recipientID.ToString();
				if (recipientID == Core.UserManager.CurrentUser.Id)
				{
					filter = new Sport.Data.EntityFilter(
						(int) Sport.Entities.User.Fields.UserType, (int) Sport.Types.UserType.Internal);
					senderFilter.SetValues(Sport.Entities.User.Type.GetEntities(filter));
					senderFilter.Value = null;
				}
				else
				{
					senderFilter.SetValues(senders);
					senderFilter.Value = senders[0];
				}
			}
			Sport.Entities.InstantMessage.NewRecipient = (Sport.Data.Entity) recipientFilter.Value;
		}
		
		private void SenderFiltered(object sender, EventArgs e)
		{
			if (senderFilter.Value == null)
			{
				State["sender"] = null;
			}
			else
			{
				State["sender"] = ((Sport.Data.Entity) senderFilter.Value).Id.ToString();
			}
		}
		#endregion
		
		public override void Open()
		{
			// Default columns
			Columns = new int[] { 
									(int) Sport.Entities.InstantMessage.Fields.Sender,
									(int) Sport.Entities.InstantMessage.Fields.Recipient,
									(int) Sport.Entities.InstantMessage.Fields.DateSent,
									(int) Sport.Entities.InstantMessage.Fields.DateRead
								};

			// Default sort columns
			Sort = new int[] {
								 (int) Sport.Entities.InstantMessage.Fields.DateSent
							 };
			
			// Details fields
			Details = new int[] {
									(int) Sport.Entities.InstantMessage.Fields.Contents
								};

			//DetailsView = typeof(Forms.ReceiptForm);

			//
			// Query
			//
			EntityListView.EntityQuery.Parameters.Add((int) Sport.Entities.InstantMessage.Fields.Recipient, false);
			EntityListView.EntityQuery.Parameters.Add((int) Sport.Entities.InstantMessage.Fields.Sender);
			
			//
			// Filters
			//
			CreateFilters();
			
			Sport.Entities.User recipient=Recipient;
			if (recipient != null)
			{
				recipientFilter.Value = recipient;
				EntityListView.EntityQuery.Parameters[0].Value = recipient;
			}
			
			Sport.Entities.User sender=Sender;
			if (sender != null)
			{
				senderFilter.Value = sender;
				EntityListView.EntityQuery.Parameters[1].Value = sender;
			}
			
			base.Open ();
		}

		#region State Properties
		public Sport.Entities.User Recipient
		{
			get
			{
				if (State["recipient"] == null)
					return null;
				if (State["recipient"] == "@user")
					return new Sport.Entities.User(Core.UserManager.CurrentUser.Id);
				return new Sport.Entities.User(Int32.Parse(State["recipient"]));
			}
			set
			{
				if (value == null)
					State["recipient"] = null;
				else
					State["recipient"] = value.Id.ToString();
			}
		}
		
		public Sport.Entities.User Sender
		{
			get
			{
				if (State["sender"] == null)
					return null;
				return new Sport.Entities.User(Int32.Parse(State["sender"]));
			}
			set
			{
				if (value == null)
					State["sender"] = null;
				else
					State["sender"] = value.Id.ToString();
			}
		}
		#endregion

		protected override void NewEntity()
		{
			/*
			Sport.Entities.User recipient = Recipient;
			if ((recipient == null)||(recipient.Id < 0))
			{
				Sport.UI.MessageBox.Show("יש לבחור משתמש אליו ברצונך לשלוח את הההודעה");
				return;
			}
			*/
			
			base.NewEntity();
		}
		
		public override void OnContextMenu(Sport.UI.TableView2.SelectionType selectionType, Sport.UI.Controls.RightContextMenu menu)
		{
			/*
			base.OnContextMenu (selectionType, menu);

			if (selectionType == Sport.UI.TableView2.SelectionType.Single)
			{
				Sport.Entities.Receipt receipt = new Sport.Entities.Receipt(Current);
				if (receipt.Account.School != null)
					menu.MenuItems.Add(new System.Windows.Forms.MenuItem("בית ספר", new System.EventHandler(ReceiptSchoolClicked)));
			}
			*/
		}
	}
}
