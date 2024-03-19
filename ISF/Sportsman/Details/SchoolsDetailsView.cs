using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Sport.UI;

namespace Sportsman.Details
{
	/// <summary>
	/// Inherits DetailView for schools details
	/// </summary>
	public class SchoolsDetailsView : DetailsView
	{
		private System.ComponentModel.Container components = null;

		#region Initialization

		#region Constructor

		public SchoolsDetailsView()
		{
			
			DialogForm.ActiveForm.Text = "���� ��� ����";
			EntityType = Sport.Entities.School.Type;
		}

		#endregion

		#endregion

		#region DetailsView Operations

		protected override void handleSpecialItems()
		{
			if (specialItems == null)
				return;
			PageItem regionItem = specialItems[0];
			PageItem cityItem = specialItems[1];
			PageItem mailCityItem = specialItems[2];

			if (EntityId > 0)
			{
				Sport.Entities.School currentSchool = new Sport.Entities.School(EntityId);
			
                regionItem.Values = Sport.Entities.Region.Type.GetEntities(null);
				regionItem.SelectedValue = currentSchool.Region;
			
				cityItem.Values = currentSchool.Region.GetCities();
				cityItem.SelectedValue = currentSchool.City;
			
				mailCityItem.Values = currentSchool.Region.GetCities();
				mailCityItem.SelectedValue = currentSchool.MailCity;
			}
			
		}

		#endregion

		#region View Operations

		public override void Open()
		{
			EntityType = Sport.Entities.School.Type;

			// Clear previous pages
			if (searchers != null)
				searchers.Clear();
			if (Pages != null)
				Pages.Clear();

			// Set the searchers
			searchers = new PageItemCollection(this);
			searchers.Add(new PageItem((int)Sport.Entities.School.Fields.Symbol,"���:",200,PageItem.PageItemType.SearcherItem));

			// Set the fields
			
			Page currentPage = new Page("���� ����");
			
			specialItems = new PageItem[3];
	
			PageItem regionItem = new PageItem((int)Sport.Entities.School.Fields.Region,"����:",200);
            specialItems[0] = regionItem;

			PageItem cityItem = new PageItem((int)Sport.Entities.School.Fields.City,"����:",200);
			specialItems[1] = cityItem;

			PageItem mailCityItem = new PageItem((int)Sport.Entities.School.Fields.MailCity,"���� ���:",200);
			specialItems[2] = mailCityItem;

			currentPage.Items.Add(new PageItem((int)Sport.Entities.School.Fields.Symbol,"���:",200));
			currentPage.Items.Add(new PageItem((int)Sport.Entities.School.Fields.Name,"��:",200));
			currentPage.Items.Add(new PageItem((int)Sport.Entities.School.Fields.Address,"�����:",200));
			currentPage.Items.Add(cityItem);
			currentPage.Items.Add(mailCityItem);
			currentPage.Items.Add(new PageItem((int)Sport.Entities.School.Fields.ZipCode,"�����:",200));
			currentPage.Items.Add(new PageItem((int)Sport.Entities.School.Fields.MailAddress,"���:",200));
			currentPage.Items.Add(regionItem);
			currentPage.Items.Add(new PageItem((int)Sport.Entities.School.Fields.Sector,"����:",200));
			currentPage.Items.Add(new PageItem((int)Sport.Entities.School.Fields.Phone,"�����:",200));
			currentPage.Items.Add(new PageItem((int)Sport.Entities.School.Fields.Fax,"���:",200));
			currentPage.Items.Add(new PageItem((int)Sport.Entities.School.Fields.Email,"���\"�:",200));
			currentPage.Items.Add(new PageItem((int)Sport.Entities.School.Fields.ManagerName,"����:",200));
			currentPage.Items.Add(new PageItem((int)Sport.Entities.School.Fields.PlayerNumberFrom,"����� �����:",200));
			currentPage.Items.Add(new PageItem((int)Sport.Entities.School.Fields.PlayerNumberTo,"�� ���� �����:",200));
			currentPage.Items.Add(new PageItem((int)Sport.Entities.School.Fields.FromGrade,"�����:",200));
			currentPage.Items.Add(new PageItem((int)Sport.Entities.School.Fields.ToGrade,"�� ����:",200));
			currentPage.Items.Add(new PageItem((int)Sport.Entities.School.Fields.Supervision,"��� �����:",200));

			this.Pages.Add(currentPage);
			this.Title = "���� ��� ����";
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
