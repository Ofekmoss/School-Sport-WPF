using System;
using System.Collections;
using Sport.UI;
using Sport.Data;
using System.Windows.Forms;
using Sportsman.Core;
using Sport.UI.Controls;
using Sport.Types;
using Sportsman.PermissionServices;

namespace Sportsman.Views
{
	/// <summary>
	/// Summary description for SeasonsTableView.
	/// </summary>
	[Sport.UI.ViewPermissions(Sport.Types.Permission.Administration, true)]
	public class SeasonsTableView : TableView
	{
		private enum ColumnTitles
		{
			Name=0,
			Status
		}	
		
		public SeasonsTableView()
		{
			Items.Add((int) Sport.Entities.Season.Fields.Name, "עונה", 150);
			Items.Add((int) Sport.Entities.Season.Fields.Status, "סטטוס", 150);
		}
		
		public override void Open()
		{
			EntityListView = new EntityListView(Sport.Entities.Season.TypeName);
			Columns = new int[] { (int) ColumnTitles.Name, (int) ColumnTitles.Status };
			Details = new int[] { (int) ColumnTitles.Name, (int) ColumnTitles.Status };
			Requery();
			base.Open();

		}
		
		protected override void NewEntity()
		{
			base.NewEntity();
		}

		protected override bool OnDeleteEntity(Entity entity)
		{
			if (entity == null)
				return false;
			Sport.Entities.Season season=new Sport.Entities.Season(entity);
			
			if (entity.Id == Sport.Core.Session.Season)
			{
				Sport.UI.MessageBox.Error("לא ניתן למחוק עונה נוכחית. אנא סגור את התוכנה והכנס תוך שימוש בעונה אחרת", 
					"מחיקת עונה");
				return false;
			}

			//check championships:
			EntityFilter filter=new EntityFilter(
				(int) Sport.Entities.Championship.Fields.Season, season.Id);
			Entity[] champs=Sport.Entities.Championship.Type.GetEntities(filter);
			string names="";
			for (int i=0; i<champs.Length; i++)
			{
				names += champs[i].Name+"\n";
				if (i >= 15)
				{
					names += "...\n";
					break;
				}
			}
			
			if (champs.Length > 0)
			{
				Sport.UI.MessageBox.Show("עונת '"+season.Name+"' מכילה את האליפויות הבאות: "+
					"\n"+names+"יש להסיר אליפויות אלו ממאגר הנתונים", 
					"מחיקת עונה", MessageBoxIcon.Warning);
				return false;
			}

			return Sport.UI.MessageBox.Ask("האם למחוק את עונת '" + season.Name + "'?", false);
		}

		protected override void OnNewEntity(EntityEdit entityEdit)
		{
			EntityField entityField;
			int seasonStartField=(int) Sport.Entities.Season.Fields.Start;
			int seasonFieldIndex=(int) Sport.Entities.Season.Fields.Season;
			
			int maxSeason = 0;
			DateTime maxEnd = DateTime.MinValue;
			GetMaxData(ref maxSeason, ref maxEnd);
			
			//change the season start value:
			entityField = EntityListView.EntityType.Fields[seasonStartField];
			entityField.SetValue(EntityListView.EntityEdit, maxEnd.AddDays(1));

			//change the season value:
			entityField = EntityListView.EntityType.Fields[seasonFieldIndex];
			entityField.SetValue(EntityListView.EntityEdit, maxSeason+1);
		}
		
		private void GetMaxData(ref int maxSeason, ref DateTime maxEnd)
		{
			Entity[] seasons=Sport.Entities.Season.Type.GetEntities(null);
			
			maxSeason = -1;
			maxEnd = DateTime.MinValue;
			
			for (int i=0; i<seasons.Length; i++)
			{
				Sport.Entities.Season curSeason = new Sport.Entities.Season(seasons[i]);
				
				if (curSeason.Id > maxSeason)
					maxSeason = seasons[i].Id;
				
				if (curSeason.End > maxEnd)
					maxEnd = curSeason.End;
			}
			
			if (maxSeason < 0)
				maxSeason = DateTime.Now.Year-Sport.Entities.Season.ZeroYear-1;
			
			if (maxEnd.Year < 1900)
				maxEnd = DateTime.Now.AddDays(-1);
		}

		private void Requery()
		{
			Cursor c = Cursor.Current;
			Cursor.Current = Cursors.WaitCursor;
			EntityListView.Read(null);
			Title = "ניהול עונות";
			Cursor.Current = c;
		}
	}
}
