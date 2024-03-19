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
	public class ChampionshipsDetailsView : DetailsView
	{
		private System.ComponentModel.Container components = null;
		private EntitySelectionDialog userDialog;

		#region Initialization

		#region Constructor

		public ChampionshipsDetailsView()
		{
			DialogForm.ActiveForm.Text = "פרטי האליפות";
			EntityType = Sport.Entities.Championship.Type;
		}

		#endregion

		#endregion

		#region DetailsView Operations

		protected override void handleSpecialItems()
		{
			if (specialItems == null)
				return;
			PageItem supervisorItem = specialItems[0];
			PageItem regionItem = specialItems[1];
			PageItem seasonItem = specialItems[2];
			PageItem sportItem = specialItems[3];
			PageItem rulesetItem = specialItems[4];

			if (EntityId > 0)
			{
				Sport.Entities.Championship currentChampionship = new Sport.Entities.Championship(EntityId);
				
				userDialog = new EntitySelectionDialog(new Views.UsersTableView());
				userDialog.View.State[ViewState.SelectionDialog] = "1";
				userDialog.View.State["UserType"] = ((int) Sport.Types.UserType.Internal).ToString();
				Sport.Entities.User supervisor = currentChampionship.Supervisor;
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
					userDialog.View.State[Sport.Entities.Region.TypeName] = currentChampionship.Region.Id.ToString();
					userDialog.View.State[Sport.Entities.School.TypeName] = null;
				}


				// Button items
				supervisorItem.ItemType = Sport.UI.Controls.GenericItemType.Button;
				supervisorItem.Values = Sport.UI.Controls.GenericItem.ButtonValues(new Sport.UI.Controls.ButtonBox.SelectValue(userDialog.ValueSelector));
				supervisorItem.SelectedValue = supervisor;

				// Selection Items
				regionItem.Values = Sport.Entities.Region.Type.GetEntities(null);
				regionItem.SelectedValue = currentChampionship.Region;

				seasonItem.Values = Sport.Entities.Season.Type.GetEntities(null);
				seasonItem.SelectedValue = currentChampionship.Season;

				sportItem.Values = Sport.Entities.Sport.Type.GetEntities(null);
				sportItem.SelectedValue = currentChampionship.Sport;

				rulesetItem.Values = Sport.Entities.Ruleset.Type.GetEntities(null);
				rulesetItem.SelectedValue = currentChampionship.Ruleset;


			}
			
		}

		#endregion


		#region View Operations

		public override void Open()
		{
			// Clear previous pages
			if (searchers != null)
				searchers.Clear();
			if (Pages != null)
				Pages.Clear();

			Page currentPage = new Page("מידע כללי");
			
			specialItems = new PageItem[5];
            
			PageItem supervisorItem = new PageItem((int)Sport.Entities.Championship.Fields.Supervisor,"אחראי:",200);
			specialItems[0] = supervisorItem;
			PageItem regionItem = new PageItem((int)Sport.Entities.Championship.Fields.Region,"מחוז:",200);
			specialItems[1] = regionItem;
			PageItem seasonItem = new PageItem((int)Sport.Entities.Championship.Fields.Season,"עונה:",200);
			specialItems[2] = seasonItem;
			PageItem sportItem = new PageItem((int)Sport.Entities.Championship.Fields.Sport,"ענף:",200);
			specialItems[3] = sportItem;
			PageItem rulesetItem = new PageItem((int)Sport.Entities.Championship.Fields.Sport,"תקנון:",200);
			specialItems[4] = rulesetItem;
			
			GridPageItem categoriesItem = new GridPageItem((int)Sport.Entities.ChampionshipCategory.Fields.Category,"קטגוריות:",new Size(350,80));
			Sport.UI.Controls.Grid.GridColumnCollection columns = categoriesItem.Columns;
			columns.Add((int) Sport.Entities.ChampionshipCategory.Fields.Category, "קטגוריה", 200);
			columns.Add((int) Sport.Entities.ChampionshipCategory.Fields.RegistrationPrice, "תעריף רישום", 200);
			columns.Add((int) Sport.Entities.ChampionshipCategory.Fields.Status, "מצב", 200);
			
			//categoriesItem.Item.Values = Sport.UI.Controls.GenericItem.ButtonValues(new Sport.UI.Controls.ButtonBox.SelectValue(Forms.CategorySelectionDialog.ValueSelector));

			currentPage.Items.Add(supervisorItem);
			currentPage.Items.Add(new PageItem((int)Sport.Entities.Championship.Fields.IsClubs,"מועדון?:",200));
			currentPage.Items.Add(regionItem);
			currentPage.Items.Add(new PageItem((int)Sport.Entities.Championship.Fields.Number,"מספר אליפות:",200));
			currentPage.Items.Add(new PageItem((int)Sport.Entities.Championship.Fields.IsOpen,"סוג אליפות:",200));
			currentPage.Items.Add(new PageItem((int)Sport.Entities.Championship.Fields.Status,"סטטוס:",200));
			currentPage.Items.Add(seasonItem);
			currentPage.Items.Add(sportItem);
			currentPage.Items.Add(categoriesItem);
			currentPage.Items.Add(new PageItem((int)Sport.Entities.Championship.Fields.Name,"שם:",200));
			currentPage.Items.Add(new PageItem((int)Sport.Entities.Championship.Fields.LastModified,"תאריך שינוי אחרון:",200));
			currentPage.Items.Add(rulesetItem);
		

			this.Pages.Add(currentPage);
			this.Title = "פרטי האליפות";

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
