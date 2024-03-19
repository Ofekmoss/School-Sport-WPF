using System;
using Sport.UI;
using Sport.Data;
using System.Windows.Forms;
using System.Collections;
using Sportsman.Core;
using Sport.UI.Controls;

namespace Sportsman.Views
{
	/// <summary>
	/// Summary description for EquipmentTypesTableView.
	/// </summary>
	[Sport.UI.ViewPermissions(Sport.Types.Permission.General, true)]
	public class EquipmentTypesTableView : TableView
	{
		private EntityFilter _filter;
		
		private enum ColumnTitles
		{
			EquipmentName=0,
			EquipmentType,
			BasePrice
		}
		
		public EquipmentTypesTableView()
		{
			Items.Add((int) Sport.Entities.EquipmentType.Fields.Name, "��� ����", 200);
			Items.Add((int) Sport.Entities.EquipmentType.Fields.Type, "���� �����", 130);
			Items.Add((int) Sport.Entities.EquipmentType.Fields.BasePrice, "���� ����", 100);
			
			FilterBarEnabled = false;
		}
		
		public override void Open()
		{
			EntityListView = new EntityListView(Sport.Entities.EquipmentType.TypeName);


			Columns = new int[] { (int) ColumnTitles.EquipmentName, 
									(int) ColumnTitles.EquipmentType, 
									(int) ColumnTitles.BasePrice };
			
			if (State[SelectionDialog] == "1")
			{
				this.Editable = false;
				this.CanInsert = false;
			}

			//Details = new int[] { };
			
			//Filters:
			
			ReloadView();
			base.Open();
		} //end function Open
		
		protected override bool OnDeleteEntity(Entity entity)
		{
			if (entity == null)
				return false;
			Sport.Entities.EquipmentType type=new Sport.Entities.EquipmentType(entity);
			
			if (State[SelectionDialog] == "1")
				return false;
			
			//begin checkings... first check if this type is used by equipment:
			int equipCount=type.GetEquipments().Length;
			
			if (equipCount > 0)
			{
				Sport.UI.MessageBox.Show("��� ����� '"+type.Name+"' ���� �-"+equipCount+" "+
					"������ ����. \n�� ����� ������ ��� ���� ��.", 
					"����� ��� ����", MessageBoxIcon.Warning);
				return false;
			}
			
			return Sport.UI.MessageBox.Ask("��� ����� �� ��� ����� '" + type.Name + "'?", false);
		}
		
		#region Filters
		/// <summary>
		/// refresh all filters of this view.
		/// </summary>
		private void RefreshFilters()
		{
			_filter = new EntityFilter();
		}
		#endregion

		/// <summary>
		/// reload the view according to current filters. read from database.
		/// </summary>
		private void Requery()
		{
			Cursor c = Cursor.Current;
			Cursor.Current = Cursors.WaitCursor;
			
			EntityListView.Read(_filter); 
			
			/*
			Entity team = (Entity) teamFilter.Value;
			if (team == null)
			{
				EntityListView.Clear();
			}
			else
			{
				EntityListView.Read(filter); 
			}
			*/
			
			Title = "����� ����";
			Cursor.Current = c;
		} //end function Requery

		private void ReloadView()
		{
			//refresh all filters and requery the information.
			RefreshFilters();
			Requery();
		}
	} //end class EquipmentTypesTableView\
}
