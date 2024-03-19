using System;
using System.Windows.Forms;
using Sport.UI;
using Sport.UI.Controls;
using Sport.Data;
using Sportsman.Core;

namespace Sportsman.Views
{
	/// <summary>
	/// Summary description for TeacherCourseView.
	/// </summary>
	[Sport.UI.ViewPermissions(Sport.Types.Permission.General)]
	public class TeacherCourseView : TableView
	{
		private System.Windows.Forms.ToolBarButton tbbTeacherCourseProduct;
		private System.Windows.Forms.ToolBarButton tbbTeacherCourseSports;
		
		private enum ColumnTitles
		{
			FirstName = 0,
			LastName,
			IdNumber,
			BirthDay,
			Address,
			CityName,
			ZipCode,
			Email,
			CellPhone,
			HomePhone,
			FaxNumber,
			Gender,
			SchoolName,
			SchoolCityName,
			SchoolAddress,
			FirstSport,
			SecondSport,
			Veteranship,
			Expertise,
			TeamAgeRange,
			CourseHoliday,
			CourseYear,
			CourseSport,
			IsConfirmed,
			CoachTrainingType,
			CoachTrainingHours,
			LastModified
		}

		public TeacherCourseView()
		{
			Items.Add((int) Sport.Entities.TeacherCourse.Fields.FirstName, "�� ����", 100);
			Items.Add((int) Sport.Entities.TeacherCourse.Fields.LastName, "�� �����", 100);
			Items.Add((int) Sport.Entities.TeacherCourse.Fields.IdNumber, "�\"�", 110);
			Items.Add((int) Sport.Entities.TeacherCourse.Fields.BirthDay, "����� ����", 120);
			Items.Add((int) Sport.Entities.TeacherCourse.Fields.Address, "�����", 120);
			Items.Add((int) Sport.Entities.TeacherCourse.Fields.CityName, "���", 80);
			Items.Add((int) Sport.Entities.TeacherCourse.Fields.ZipCode, "�����", 60);
			Items.Add((int) Sport.Entities.TeacherCourse.Fields.Email, "���� ��������", 120);
			Items.Add((int) Sport.Entities.TeacherCourse.Fields.CellPhone, "����� ������", 100);
			Items.Add((int) Sport.Entities.TeacherCourse.Fields.HomePhone, "����� ���", 100);
			Items.Add((int) Sport.Entities.TeacherCourse.Fields.FaxNumber, "���� ���", 100);
			Items.Add((int) Sport.Entities.TeacherCourse.Fields.Gender, "���", 100);
			Items.Add((int) Sport.Entities.TeacherCourse.Fields.SchoolName, "�� ��� ���", 110);
			Items.Add((int) Sport.Entities.TeacherCourse.Fields.SchoolCityName, "����� ��� ���", 90);
			Items.Add((int) Sport.Entities.TeacherCourse.Fields.SchoolAddress, "����� ��� ���", 120);
			Items.Add((int) Sport.Entities.TeacherCourse.Fields.FirstSport, "��� ������", 110);
			Items.Add((int) Sport.Entities.TeacherCourse.Fields.SecondSport, "��� ������ ����", 110);
			Items.Add((int) Sport.Entities.TeacherCourse.Fields.Veteranship, "��� �����", 90);
			Items.Add((int) Sport.Entities.TeacherCourse.Fields.Expertise, "�����", 120);
			Items.Add((int) Sport.Entities.TeacherCourse.Fields.TeamAgeRange, "����� �����", 120);
			Items.Add((int) Sport.Entities.TeacherCourse.Fields.CourseHoliday, "���� �������", 110);
			Items.Add((int) Sport.Entities.TeacherCourse.Fields.CourseYear, "��� �������", 110);
			Items.Add((int) Sport.Entities.TeacherCourse.Fields.CourseSport, "��� �������", 110);
			Items.Add((int) Sport.Entities.TeacherCourse.Fields.IsConfirmed, "����� �����", 110);
			Items.Add((int) Sport.Entities.TeacherCourse.Fields.CoachTrainingType, "������� ������ - ���", 190);
			Items.Add((int) Sport.Entities.TeacherCourse.Fields.CoachTrainingHours, "���� �������", 110);
			Items.Add((int) Sport.Entities.TeacherCourse.Fields.LastModified, "����� ����� �����", 110);
			
			//
			// toolBar
			//
			tbbTeacherCourseProduct = new ToolBarButton();
			tbbTeacherCourseProduct.Text = "����� ��� ����";
			tbbTeacherCourseSports = new ToolBarButton();
			tbbTeacherCourseSports.Text = "����� ���� �����";
			
			toolBar.Buttons.Add(tbbTeacherCourseProduct);
			toolBar.Buttons.Add(tbbTeacherCourseSports);
			toolBar.ButtonClick += new System.Windows.Forms.ToolBarButtonClickEventHandler(ToolBarButtonClicked);
		}

		private void ToolBarButtonClicked(object sender, System.Windows.Forms.ToolBarButtonClickEventArgs e)
		{
			if (e.Button == tbbTeacherCourseProduct)
				OpenProductDialog();
			else if (e.Button == tbbTeacherCourseSports)
				OpenSportsDialog();
		}
		
		public override void Open()
		{
			EntityListView = new EntityListView(Sport.Entities.TeacherCourse.TypeName);
			
			Sport.Data.Entity[] sports = Sport.Entities.Sport.Type.GetEntities(null);
			
			EntityListView.Fields[(int) Sport.Entities.TeacherCourse.Fields.FirstSport].Values = sports;
			EntityListView.Fields[(int) Sport.Entities.TeacherCourse.Fields.SecondSport].Values = sports;
			EntityListView.Fields[(int) Sport.Entities.TeacherCourse.Fields.CourseSport].Values = sports;
				
			EntityListView.Fields[(int) Sport.Entities.TeacherCourse.Fields.LastModified].CanEdit = false;
			
			Columns = new int[] { (int) ColumnTitles.FirstName, (int) ColumnTitles.LastName, (int) ColumnTitles.IdNumber, 
						(int) ColumnTitles.CourseHoliday, (int) ColumnTitles.CourseYear, (int) ColumnTitles.CourseSport, 
						(int) ColumnTitles.SchoolName, (int) ColumnTitles.FirstSport, (int) ColumnTitles.SecondSport, 
						(int) ColumnTitles.Veteranship, (int) ColumnTitles.Expertise, (int) ColumnTitles.IsConfirmed };
			
			Details = new int[] { (int) ColumnTitles.BirthDay, (int) ColumnTitles.Address, (int) ColumnTitles.CityName, 
						(int) ColumnTitles.ZipCode, (int) ColumnTitles.Email, (int) ColumnTitles.CellPhone, 
						(int) ColumnTitles.HomePhone, (int) ColumnTitles.FaxNumber, (int) ColumnTitles.Gender, 
						(int) ColumnTitles.SchoolCityName, (int) ColumnTitles.SchoolAddress, (int) ColumnTitles.TeamAgeRange };
			
			Requery();

			base.Open();
		} //end function Open
		
		protected override bool OnDeleteEntity(Entity entity)
		{
			if (entity == null)
				return false;
			Sport.Entities.TeacherCourse teacherCourse = new Sport.Entities.TeacherCourse(entity);

			bool result;
			result = Sport.UI.MessageBox.Ask("��� ����� �� ����� �������� '" + teacherCourse.FirstName + " " + 
				teacherCourse.LastName + "'?", false);
			
			//delete charages as well...
			/*
			if (result == true)
			{
				Sport.Entities.Charge[] charges = teacherCourse.GetCharges();
				if (charges != null)
				{
					foreach (Sport.Entities.Charge charge in charges)
						charge.Entity.Delete();
					EntityFilter filter = new EntityFilter(
						(int) Sport.Entities.Charge.Fields.Additional, teacherCourse.Id);
					Sport.Entities.Charge.Type.Reset(filter);
				}
			}
			*/
			
			return result;
		}

		protected override void OnValueChange(EntityEdit entityEdit, EntityField entityField)
		{
			/*
				//get numbers range:
				int pNumFromField = (int) Sport.Entities.Team.Fields.PlayerNumberFrom;
				int pNumToField = (int) Sport.Entities.Team.Fields.PlayerNumberTo;
				int oldPlayerNumberFrom = Sport.Common.Tools.CIntDef(
					entityEdit.Entity.Fields[pNumFromField], -1);
				int oldPlayerNumberTo = Sport.Common.Tools.CIntDef(
					entityEdit.Entity.Fields[pNumToField], -1);
				int newPlayerNumberFrom = Sport.Common.Tools.CIntDef(
					entityEdit.Fields[pNumFromField], -1);
				int newPlayerNumberTo = Sport.Common.Tools.CIntDef(
					entityEdit.Fields[pNumToField], -1);
			*/
			base.OnValueChange (entityEdit, entityField);
		}
		
		protected override void OnSaveEntity(Entity entity)
		{
			if (entity == null)
				return;
			
			Sport.Entities.TeacherCourse teacherCourse = new Sport.Entities.TeacherCourse(entity);
			
			//add charge?
			if (teacherCourse.IsConfirmed)
			{
				Sport.Entities.Charge charge = teacherCourse.GetCharge();
				if (charge == null)
				{
					string strError = teacherCourse.DoCharge();
					if (strError != null && strError.Length > 0)
						Sport.UI.MessageBox.Warn("����� ��� ����� ���� ���:\n" + strError, "����� ������");
				}
			}
		} //end function OnSaveEntity
		
		/*
		public override MenuItem[] GetContextMenu(Sport.UI.TableView.SelectionType selectionType)
		{
			MenuItem[] menuItems = null;
			
			Entity team=this.Current;
			if (team == null)
				return null;
			
			int champID=(int) team.Fields[(int) Sport.Entities.Team.Fields.Championship];
			EntityType champType=Sport.Entities.Championship.Type;
			Entity champ=champType.Lookup(champID);
			switch (selectionType)
			{
				case (SelectionType.Single):
					ArrayList arrItems=new ArrayList();
					arrItems.Add(new MenuItem("���", new System.EventHandler(TeamDetailsClick)));
					arrItems.Add(new MenuItem("-"));
	
					//get players for given team.
					
					//first, define filter:
					EntityFilter filter=new EntityFilter(
						new EntityFilterField((int) Sport.Entities.Player.Fields.Team, team.Id));
					//get players list:
					Entity[] players=Sport.Entities.Player.Type.GetEntities(filter);
					if (players.Length > 0)
						arrItems.Add(new MenuItem("������ ����", new System.EventHandler(PrintPlayerCards)));


					arrItems.Add(new MenuItem("������", new System.EventHandler(TeamPlayersClick)));
					arrItems.Add(new MenuItem("���� ������� ����", new System.EventHandler(CopyTeamClick)));
					
					menuItems = new MenuItem[arrItems.Count];
					for (int i=0; i<arrItems.Count; i++)
						menuItems[i] = (MenuItem) arrItems[i];
					menuItems[0].DefaultItem = true;
					break;
			}

			return menuItems;
		}
		*/
		
		protected override void OnNewEntity(EntityEdit entityEdit)
		{
			EntityField entityField;
			
			//change year to current:
			entityField = EntityListView.EntityType.Fields[(int) Sport.Entities.TeacherCourse.Fields.CourseYear];
			entityField.SetValue(EntityListView.EntityEdit, DateTime.Now.Year);
			
			//change is confirmed to false:
			entityField = EntityListView.EntityType.Fields[(int) Sport.Entities.TeacherCourse.Fields.IsConfirmed];
			entityField.SetValue(EntityListView.EntityEdit, 0);
		}
		
		/*
		protected override void NewEntity()
		{

		}
		*/
		
		/// <summary>
		/// reload the view according to current filters. read from database.
		/// </summary>
		private void Requery()
		{
			Cursor c = Cursor.Current;
			
			Cursor.Current = Cursors.WaitCursor;
			EntityListView.Read(null);
			Title = "����� ����������";
			
			Cursor.Current = c;
		} //end function Requery
		
		private void OpenProductDialog()
		{
			ProductsTableView view = new ProductsTableView();
			view.State[Sport.UI.View.SelectionDialog] = "1";
			Sport.Entities.Product product = Sport.Entities.TeacherCourse.GetProduct();
			EntitySelectionDialog dialog = new EntitySelectionDialog(view, false);
			if (product != null)
				dialog.Entity = product.Entity;
			if (dialog.ShowDialog() == DialogResult.OK)
			{
				Sport.Data.Entity entity = dialog.Entity;
				if (entity != null)
					Sport.Entities.TeacherCourse.SetProduct(entity.Id);
			}
		}

		private void OpenSportsDialog()
		{
			SportsTableView view = new SportsTableView();
			view.State[Sport.UI.View.SelectionDialog] = "1";
			Sport.Entities.Sport[] sports = Sport.Entities.TeacherCourse.GetSports();
			EntitySelectionDialog dialog = new EntitySelectionDialog(view, true);
			Sport.Data.Entity[] entities;
			if (sports != null)
			{
				entities = new Entity[sports.Length];
				for (int i = 0; i < sports.Length; i++)
					entities[i] = sports[i].Entity;
				dialog.Entities = entities;
			}
			if (dialog.ShowDialog() == DialogResult.OK)
			{
				entities = dialog.Entities;
				if (entities != null)
				{
					int[] arrSportsId = new int[entities.Length];
					for (int i = 0; i < entities.Length; i++)
						arrSportsId[i] = entities[i].Id;
					Sport.Entities.TeacherCourse.SetSports(arrSportsId);
				}
			}
		}
	}
}
