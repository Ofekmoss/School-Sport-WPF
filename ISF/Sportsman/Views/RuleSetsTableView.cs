using System;
using Sport.UI;
using Sport.Data;

namespace Sportsman.Views
{
	/// <summary>
	/// Summary description for SportsTableView.
	/// </summary>
	[Sport.UI.ViewPermissions(Sport.Types.Permission.Sports)]
	public class RulesetsTableView : TableView
	{
		public RulesetsTableView()
		{
			Items.Add((int) Sport.Entities.Ruleset.Fields.Name, "שם", 250);
			Items.Add((int) Sport.Entities.Ruleset.Fields.Sport, "ספורט", 150);
			this._editorViewName = typeof(Producer.RulesetEditorView).Name;
		}

		public override void Open()
		{
			Title = "תקנונים";

			EntityListView = new EntityListView(Sport.Entities.Ruleset.TypeName);

			EntityListView.Fields[(int) Sport.Entities.Ruleset.Fields.Sport].Values =
				Sport.Entities.Sport.Type.GetEntities(null);

			Columns = new int[] { 0, 1 };

			EntityListView.Read(null);

			base.Open();
		}

		protected override bool OnDeleteEntity(Sport.Data.Entity entity)
		{
			if (entity == null)
				return false;
			Sport.Entities.Ruleset re=new Sport.Entities.Ruleset(entity);
			
			//begin checkings... first check if the Ruleset contain any rules:
			Sport.Rulesets.Ruleset ruleset = Sport.Rulesets.Ruleset.LoadRuleset(re.Id);
			int ruleCount = ruleset.GetRuleCount();
			if (ruleCount > 0)
			{
				Sport.UI.MessageBox.Warn("התקנון '"+ruleset.Name+"' מכיל "+ruleCount.ToString()+
				" חוקים\n" + "יש להסיר חוקים אלו", "מחיקת תקנון");
				return false;
			}
			
			//check if any chapmionships has this ruleset:
			Sport.Entities.Championship[] champs=re.GetChapmionships();
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
				Sport.UI.MessageBox.Warn("התקנון '"+ruleset.Name+"' מוגדר עבור האליפויות הבאות: "+
					"\n"+names+"יש להסיר את התקנון מאליפויות אלו", "מחיקת תקנון");
				return false;
			}
			
			return Sport.UI.MessageBox.Ask("האם למחוק את התקנון '" + ruleset.Name + "'?", false);
		}

	}
}
