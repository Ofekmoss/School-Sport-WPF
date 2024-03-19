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
	public class StudentsDetailsView : DetailsView
	{
		private System.ComponentModel.Container components = null;
		private EntitySelectionDialog schoolDialog;

		#region Initialization

		#region Constructor

		public StudentsDetailsView()
		{
			
			if (DialogForm.ActiveForm != null)
				DialogForm.ActiveForm.Text = "פרטי התלמיד";
			EntityType = Sport.Entities.Student.Type;
		}
		
		#endregion

		#endregion

		#region DetailsView Operations

		protected override void handleSpecialItems()
		{
			if (specialItems == null)
				return;
			PageItem schoolItem = specialItems[0];
			
			if (EntityId > 0)
			{
				Sport.Entities.Student currentStudent = new Sport.Entities.Student(EntityId);

				schoolDialog = new EntitySelectionDialog(new Views.SchoolsTableView());
				schoolDialog.View.State[ViewState.SelectionDialog] = "1";

				Sport.Entities.School school = currentStudent.School;
				
				schoolDialog.View.State[Sport.Entities.Region.TypeName] = currentStudent.School.Region.Id.ToString();
				schoolDialog.View.State[Sport.Entities.City.TypeName] = currentStudent.School.City.Id.ToString();
				schoolDialog.View.State["school"] = currentStudent.School.Id.ToString();
				
				// Button items
				schoolItem.ItemType = Sport.UI.Controls.GenericItemType.Button;
				schoolItem.SelectedValue = school;
				schoolItem.Values = Sport.UI.Controls.GenericItem.ButtonValues(new Sport.UI.Controls.ButtonBox.SelectValue(schoolDialog.ValueSelector));
			}
			
		}

		#endregion


		#region View Operations

		public override void Open()
		{
			EntityType = Sport.Entities.Student.Type;

			if (searchers != null)
				searchers.Clear();
			if (Pages != null)
				Pages.Clear();
			
			// Set the searchers
			searchers = new PageItemCollection(this);
			searchers.Add(new PageItem((int)Sport.Entities.Student.Fields.IdNumber,"ת.ז.:",50,PageItem.PageItemType.SearcherItem));

			// Fields

			Page currentPage = new Page("מידע כללי");

			specialItems = new PageItem[1];

			PageItem schoolItem = new PageItem((int)Sport.Entities.Student.Fields.School,"בית ספר:",150);
			specialItems[0] = schoolItem;

			currentPage.Items.Add(schoolItem);
			currentPage.Items.Add(new PageItem((int)Sport.Entities.Student.Fields.Grade,"כיתה:",150));
			currentPage.Items.Add(new PageItem((int)Sport.Entities.Student.Fields.SexType,"מין:",150));
			currentPage.Items.Add(new PageItem((int)Sport.Entities.Student.Fields.SchoolSymbol,"סמל:",150,Sport.Entities.Student.Type.Fields[(int)Sport.Entities.Student.Fields.SchoolSymbol].CanEdit));
			currentPage.Items.Add(new PageItem((int)Sport.Entities.Student.Fields.LastName,"שם משפחה:",150));
			currentPage.Items.Add(new PageItem((int)Sport.Entities.Student.Fields.FirstName,"שם פרטי:",150));
			currentPage.Items.Add(new PageItem((int)Sport.Entities.Student.Fields.BirthDate,"תאריך לידה:",150));
			currentPage.Items.Add(new PageItem((int)Sport.Entities.Student.Fields.IdNumber,"ת.ז.:",150));
			currentPage.Items.Add(new PageItem((int)Sport.Entities.Student.Fields.LastModified,"תאריך שינוי אחרון:",150,Sport.Entities.Student.Type.Fields[(int)Sport.Entities.Student.Fields.LastModified].CanEdit));

			this.Pages.Add(currentPage);
			this.Title = "פרטי התלמיד";
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
