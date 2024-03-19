using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Sport.UI;

namespace Sportsman.Details
{
	/// <summary>
	/// Inherits DetailView for students details
	/// </summary>
	public class TeamsDetailsView : DetailsView
	{
		private System.ComponentModel.Container components = null;
		private EntitySelectionDialog userDialog;
		private EntitySelectionDialog schoolDialog;

		#region Initialization

		#region Constructor

		public TeamsDetailsView()
		{
			if (DialogForm.ActiveForm != null)
				DialogForm.ActiveForm.Text = "פרטי הקבוצה";
			EntityType = Sport.Entities.Team.Type;
		}

		#endregion

		#endregion

		#region DetailsView Operations

		protected override void handleSpecialItems()
		{
			if (specialItems == null)
				return;
			
			PageItem supervisorItem = specialItems[0];
			PageItem championshipItem = specialItems[1];
			PageItem schoolItem = specialItems[2];
			PageItem sportItem = specialItems[3];
			PageItem categoryItem = specialItems[4];

			if (EntityId > 0)
			{
				Sport.Entities.Team currentTeam = new Sport.Entities.Team(EntityId);
				
				userDialog = new EntitySelectionDialog(new Views.UsersTableView());
				userDialog.View.State[ViewState.SelectionDialog] = "1";
				userDialog.View.State["UserType"] = ((int) Sport.Types.UserType.Internal).ToString();
				Sport.Entities.User supervisor = currentTeam.Supervisor;
				if (supervisor != null)
				{
					userDialog.View.State[Sport.Entities.Region.TypeName] = supervisor.Region.Id.ToString();
					if (supervisor.School != null)
						userDialog.View.State[Sport.Entities.School.TypeName] = supervisor.School.Id.ToString();
					else
						userDialog.View.State[Sport.Entities.School.TypeName] = null;
				}
				else
				{
					userDialog.View.State[Sport.Entities.Region.TypeName] = currentTeam.Championship.Region.Id.ToString();
					userDialog.View.State[Sport.Entities.School.TypeName] = null;
				}

				schoolDialog = new EntitySelectionDialog(new Views.SchoolsTableView());
				schoolDialog.View.State[ViewState.SelectionDialog] = "1";

				Sport.Entities.School school = currentTeam.School;
				
				schoolDialog.View.State[Sport.Entities.Region.TypeName] = currentTeam.School.Region.Id.ToString();
				schoolDialog.View.State[Sport.Entities.City.TypeName] = currentTeam.School.City.Id.ToString();
				schoolDialog.View.State["school"] = currentTeam.School.Id.ToString();

				// Button items
				supervisorItem.ItemType = Sport.UI.Controls.GenericItemType.Button;
				supervisorItem.Values = Sport.UI.Controls.GenericItem.ButtonValues(new Sport.UI.Controls.ButtonBox.SelectValue(userDialog.ValueSelector));
				supervisorItem.SelectedValue = supervisor;

				schoolItem.ItemType = Sport.UI.Controls.GenericItemType.Button;
				schoolItem.Values = Sport.UI.Controls.GenericItem.ButtonValues(new Sport.UI.Controls.ButtonBox.SelectValue(schoolDialog.ValueSelector));
				schoolItem.SelectedValue = school;

				// Selection items
				championshipItem.Values = Sport.Entities.Championship.Type.GetEntities(null);
				championshipItem.SelectedValue = currentTeam.Championship;

				//sportItem.Values = Sport.Entities.Sport.Type.GetEntities(null);
				sportItem.SelectedValue = currentTeam.Championship.Sport;

				Sport.Data.EntityFilter categoryFilter=new Sport.Data.EntityFilter((int) Sport.Entities.ChampionshipCategory.Fields.Championship, currentTeam.Championship.Id);
				categoryItem.Values = Sport.Entities.ChampionshipCategory.Type.GetEntities(categoryFilter);
				categoryItem.SelectedValue = currentTeam.Category;
			}
			
		}

		#endregion


		#region View Operations

		public override void Open()
		{
			if (searchers != null)
				searchers.Clear();
			if (Pages != null)
				Pages.Clear();
			
			if (Pages != null)
				Pages.Clear();

			// Fields
			Page currentPage = new Page("מידע כללי");
			
			specialItems = new PageItem[5];

			PageItem supervisorItem = new PageItem((int)Sport.Entities.Team.Fields.Supervisor,"אחראי:",200);
			specialItems[0] = supervisorItem;
			PageItem championshipItem = new PageItem((int)Sport.Entities.Team.Fields.Championship,"אליפות:",200);
			specialItems[1] = championshipItem;
			PageItem schoolItem = new PageItem((int)Sport.Entities.Team.Fields.School,"בית הספר:",200);
			specialItems[2] = schoolItem;
			PageItem sportItem = new PageItem((int)Sport.Entities.Team.Fields.Sport,"ענף:",200,Sport.Entities.Team.Type.Fields[(int)Sport.Entities.Team.Fields.Sport].CanEdit);
			specialItems[3] = sportItem;
			PageItem categoryItem = new PageItem((int)Sport.Entities.Team.Fields.Category,"קטגוריה:",200,Sport.Entities.Team.Type.Fields[(int)Sport.Entities.Team.Fields.Category].CanEdit);
			specialItems[4] = categoryItem;
			
			
			currentPage.Items.Add(new PageItem((int)Sport.Entities.Team.Fields.SchoolSymbol,"סמל בית הספר:",200,Sport.Entities.Team.Type.Fields[(int)Sport.Entities.Team.Fields.SchoolSymbol].CanEdit));
			currentPage.Items.Add(sportItem);
			
			
			currentPage.Items.Add(supervisorItem);
			currentPage.Items.Add(championshipItem);
			currentPage.Items.Add(schoolItem);
			currentPage.Items.Add(new PageItem((int)Sport.Entities.Team.Fields.Charge,"חיוב:",200,Sport.Entities.Team.Type.Fields[(int)Sport.Entities.Team.Fields.Charge].CanEdit));
			currentPage.Items.Add(new PageItem((int)Sport.Entities.Team.Fields.PlayerNumberFrom,"ממספר חולצה:",200));
			currentPage.Items.Add(new PageItem((int)Sport.Entities.Team.Fields.PlayerNumberTo,"עד מספר חולצה:",200));
			currentPage.Items.Add(new PageItem((int)Sport.Entities.Team.Fields.Index,"מספר סידורי:",200));
			currentPage.Items.Add(new PageItem((int)Sport.Entities.Team.Fields.Status,"סטטוס:",200));
			currentPage.Items.Add(categoryItem);
			currentPage.Items.Add(new PageItem((int)Sport.Entities.Team.Fields.RegisterDate,"תאריך רישום:",200,Sport.Entities.Team.Type.Fields[(int)Sport.Entities.Team.Fields.RegisterDate].CanEdit));
			currentPage.Items.Add(new PageItem((int)Sport.Entities.Team.Fields.LastModified,"תאריך שינוי אחרון:",200,Sport.Entities.Team.Type.Fields[(int)Sport.Entities.Team.Fields.LastModified].CanEdit));
			
			
			this.Pages.Add(currentPage);
			this.Title = "פרטי הקבוצה";
			base.Open();
		}

		#endregion

		#region Dispose

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

		#endregion
	}
}
