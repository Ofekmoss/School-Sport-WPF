using System;
using System.Collections;

namespace Sportsman.Producer
{
	public class MatchVisualizer : Sport.UI.Visualizer
	{
		private Hashtable _funcArrays=new Hashtable();
		private Hashtable _hebWeekDays=new Hashtable();
		public enum MatchField
		{
			Championship = 0,
			Phase,
			Group,
			Round,
			Cycle,
			Tournament,
			Number,
			TeamA,
			TeamB,
			Teams,
			Time,
			Date,
			Hour,
			Place,
			Facility,
			Court,
			Result,
			Supervisors,
			Referees,
			RefereeCount,
			SupervisorNumber,
			FieldCount
		}

		public MatchVisualizer()
		{
			Fields.Add(new Sport.UI.VisualizerField("אליפות", 130));
			Fields.Add(new Sport.UI.VisualizerField("שלב", 60));
			Fields.Add(new Sport.UI.VisualizerField("בית", 40));
			Fields.Add(new Sport.UI.VisualizerField("סיבוב", 40));
			Fields.Add(new Sport.UI.VisualizerField("מחזור", 40));
			Fields.Add(new Sport.UI.VisualizerField("טורניר", 40, System.Drawing.StringAlignment.Center));
			Fields.Add(new Sport.UI.VisualizerField("מספר", 40, System.Drawing.StringAlignment.Center));
			Fields.Add(new Sport.UI.VisualizerField("קבוצה א'", 130));
			Fields.Add(new Sport.UI.VisualizerField("קבוצה ב'", 130));
			Fields.Add(new Sport.UI.VisualizerField("קבוצות", 250));
			Fields.Add(new Sport.UI.VisualizerField("מועד", 100));
			Fields.Add(new Sport.UI.VisualizerField("תאריך", 60));
			Fields.Add(new Sport.UI.VisualizerField("שעה", 40));
			Fields.Add(new Sport.UI.VisualizerField("מיקום", 100));
			Fields.Add(new Sport.UI.VisualizerField("מתקן", 100));
			Fields.Add(new Sport.UI.VisualizerField("מגרש", 100));
			Fields.Add(new Sport.UI.VisualizerField("תוצאה", 80, System.Drawing.StringAlignment.Center));
			Fields.Add(new Sport.UI.VisualizerField("אחראים", 100));
			Fields.Add(new Sport.UI.VisualizerField("שמות שופטים", 100));
			Fields.Add(new Sport.UI.VisualizerField("שופטים מוזמנים", 120));
			Fields.Add(new Sport.UI.VisualizerField("מספר רכז", 80));
		}

		public override string GetText(object o, int field)
		{
			Sport.Championships.Match match = o as Sport.Championships.Match;
			if (match != null)
				return GetText(match, (MatchField) field);
			return null;
		}
		
		public string GetText(Sport.Championships.Match match, MatchField field)
		{
			switch (field)
			{
				case MatchField.Championship:
					return match.Cycle.Round.Group.Phase.Championship.Name;
				case MatchField.Phase:
					return match.Cycle.Round.Group.Phase.Name;
				case MatchField.Group:
					return match.Cycle.Round.Group.Name;
				case MatchField.Round:
					return match.Cycle.Round.Name;
				case MatchField.Cycle:
					return match.Cycle.Name;
				case MatchField.Tournament:
					return match.Tournament == -1 ? null : match.Cycle.Tournaments[match.Tournament].Number.ToString();
				case MatchField.Number:
					return match.Number.ToString();
				case MatchField.TeamA:
					return match.GetTeamAName();
				case MatchField.TeamB:
					return match.GetTeamBName();
				case MatchField.Teams:
					return match.GetTeamAName() + " - " + match.GetTeamBName();
				case MatchField.Time:
					if (match.Time.Year < 1900)
						return null;
					return match.Time.ToString("g");
				case MatchField.Date:
					DateTime date=match.Time;
					if (date.Year < 1900)
						return null;
					string result=date.ToString("d");
					if (_hebWeekDays[date] == null)
						_hebWeekDays[date] = Sport.Common.Tools.GetHebDayOfWeek(date);
					string strHebDay=Sport.Common.Tools.CStrDef(_hebWeekDays[date], "");
					if (strHebDay.Length > 0)
						result += " ("+strHebDay+")";
					return result;
				case MatchField.Hour:
					if (Sport.Common.Tools.IsMinDate(match.Time))
						return null;
					return match.Time.ToString("t");
				case MatchField.Place:
					if (match.Court != null)
					{
						return match.Facility.Name + " - " + match.Court.Name;
					}
					if (match.Facility != null)
					{
						return match.Facility.Name;
					}
					return null;
				case MatchField.Facility:
					return match.Facility == null ? null : match.Facility.Name;
				case MatchField.Court:
					return match.Court == null ? null : match.Court.Name;
				case MatchField.Result:
				switch (match.Outcome)
				{
					case (Sport.Championships.MatchOutcome.Tie):
					case (Sport.Championships.MatchOutcome.WinA):
					case (Sport.Championships.MatchOutcome.WinB):
						return match.TeamBScore.ToString() + "-" + match.TeamAScore.ToString();
					case (Sport.Championships.MatchOutcome.TechnicalA):
						return "ניצחון טכני א (" + match.TeamBScore.ToString() + "-" + match.TeamAScore.ToString() + ")";
					case (Sport.Championships.MatchOutcome.TechnicalB):
						return "ניצחון טכני ב (" + match.TeamBScore.ToString() + "-" + match.TeamAScore.ToString() + ")";
				}
					return null;
				case MatchField.Supervisors:
					return TranslateFunctionaries(match.Functionaries, Sport.Types.FunctionaryType.Coordinator);
				case MatchField.Referees:
					return TranslateFunctionaries(match.Functionaries, Sport.Types.FunctionaryType.Referee);
				case MatchField.RefereeCount:
					return match.RefereeCount.ToString();
				case MatchField.SupervisorNumber:
					return TranslateFunctionaries(match.Functionaries, Sport.Types.FunctionaryType.Coordinator, true);
			}

			return null;
		}
		
		private string TranslateFunctionaries(int[] functionaries, 
			Sport.Types.FunctionaryType type, bool blnGetNumber)
		{
			if ((functionaries == null)||(functionaries.Length == 0))
				return "";
			string key="";
			if (blnGetNumber)
				key += "n_";
			for (int i=0; i<functionaries.Length; i++)
				key += functionaries[i].ToString()+",";
			key += ((int) type).ToString();
			if (_funcArrays[key] != null)
				return _funcArrays[key].ToString();
			string result="";
			for (int i=0; i<functionaries.Length; i++)
			{
				Sport.Entities.Functionary func=null;
				int funcID=functionaries[i];
				try
				{
					func = new Sport.Entities.Functionary(funcID);
				}
				catch (Exception ex)
				{
					System.Diagnostics.Debug.WriteLine("failed to create functionary. ID: "+funcID+" - "+ex.Message);
					System.Diagnostics.Debug.WriteLine(ex.StackTrace);
					func = null;
				}
				if ((func != null)&&(func.Id >= 0))
				{
					if (func.FunctionaryType == type)
					{
						if (blnGetNumber)
						{
							if (func.Number > 0)
								result += func.Number;
						}
						else
						{
							result += func.Name;
							if ((func.CellPhone != null)&&(func.CellPhone.Length > 0))
							{
								result += " ("+func.CellPhone+")";
							}
						}
						result += ",";
					}
				}
			}
			if (result.Length > 0)
				result = result.Substring(0, result.Length-1);
			_funcArrays[key] = result;
			return result;
		}
		
		private string TranslateFunctionaries(int[] functionaries, 
			Sport.Types.FunctionaryType type)
		{
			return TranslateFunctionaries(functionaries, type, false);
		}
	}
}
