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
			Items.Add((int) Sport.Entities.Ruleset.Fields.Name, "��", 250);
			Items.Add((int) Sport.Entities.Ruleset.Fields.Sport, "�����", 150);
			this._editorViewName = typeof(Producer.RulesetEditorView).Name;
		}

		public override void Open()
		{
			Title = "�������";

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
				Sport.UI.MessageBox.Warn("������ '"+ruleset.Name+"' ���� "+ruleCount.ToString()+
				" �����\n" + "�� ����� ����� ���", "����� �����");
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
				Sport.UI.MessageBox.Warn("������ '"+ruleset.Name+"' ����� ���� ��������� �����: "+
					"\n"+names+"�� ����� �� ������ ��������� ���", "����� �����");
				return false;
			}
			
			return Sport.UI.MessageBox.Ask("��� ����� �� ������ '" + ruleset.Name + "'?", false);
		}

	}
}
