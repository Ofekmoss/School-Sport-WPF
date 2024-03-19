using System;
using Sport.Core;

namespace Sport.Common
{
	/// <summary>
	/// Global data structures
	/// </summary>
	/// <summary>
	/// key-value struct, define object with id and name only.
	/// </summary>
	public struct SimpleData
	{
		public int ID;
		public string Name;
		public static readonly SimpleData Empty;
		
		public SimpleData(int id, string name)
		{
			this.ID = id;
			this.Name = name;
		}

		static SimpleData()
		{
			Empty = new SimpleData(-1, "");
		}
	}

	public class WebsitePermanentChampionship
	{
		public int Index { get; set; }
		public int ChampionshipCategoryId { get; set; }
		public string Title { get; set; }
	}

	public class PlayerCardData
	{
		public int PlayerId { get; set; }
		public int StudentEntityId { get; set; }
		public string StudentIdNumber { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string SchoolName { get; set; }
		public string SchoolSymbol { get; set; }
		public string SportName { get; set; }
		public string Birthdate { get; set; }
		public string Grade { get; set; }
		public byte[] RawPicture { get; set; }
		public bool IsOverAge { get; set; }
		public bool GotValidPicture { get; set; }
	}

	public class SimplePlayerData
	{
		public int ChampionshipId { get; set; }
		public int TeamId { get; set; }
		public int PlayerId { get; set; }
		public string PlayerName { get; set; }
		public string IdNumber { get; set; }
		public int PlayerStatus { get; set; }
		public DateTime BirthDate { get; set; }
		public bool IsMale { get; set; }
		public string TeamName { get; set; }
		public string CityName { get; set; }
	}

	public class PlayerCountData
	{
		public int TeamId { get; set; }
		public int ChampionshipId { get; set; }
		public int ChampionshipCategoryId { get; set; }
		public int TotalPlayersCount { get; set; }
		public int ConfirmedPlayersCount { get; set; }
		public int UnconfirmedPlayersCount { get; set; }
	}

	public class RegionData
	{
		public int ID=-1;
		public string Name="";
		public string Address="";
		public string Phone="";
		public string Fax="";

		public RegionData() {}

		public RegionData(int id, string name)
		{
			this.ID = id;
			this.Name = name;
		}
	}

	public class SportData
	{
		public int ID=-1;
		public string Name="";
		public int Type=-1;
		public RulesetData Ruleset=null;
		public SportData() {}

		public SportData(int id, string name)
		{
			this.ID = id;
			this.Name = name;
		}
	}

	public class SchoolData
	{
		public int ID=-1;
		public string Name="";
		public string Symbol="000000";
		public string City="";
		public int FromGrade=0;
		public int ToGrade=0;
		public RegionData Region=null;
		public bool IsClub=false;
		
		public SchoolData() {}

		public SchoolData(int id, string name)
		{
			this.ID = id;
			this.Name = name;
		}
	}

	public class StudentData
	{
		public int ID=-1;
		public string Name="";
		public string IdNumber="";
		public string FirstName="";
		public string LastName="";
		public DateTime Birthdate=DateTime.MinValue;
		public SchoolData School=null;
		public int Grade=-1;
		public int SexType=-1;
		
		public StudentData() {}

		public StudentData(int id, string fname, string lname)
		{
			this.ID = id;
			this.FirstName = fname;
			this.LastName = lname;
		}
	}

	public class FacilityData
	{
		public int ID=-1;
		public string Name="";
		public RegionData Region=null;
		public SchoolData School=null;
		public string Address="";
		public string Phone="";
		public string Fax="";
		
		public FacilityData() {}

		public FacilityData(int id, string name)
		{
			this.ID = id;
			this.Name = name;
		}
	}
	
	public class CourtData
	{
		public int ID=-1;
		public string Name="";
		public FacilityData Facility=null;
		public SimpleData CourtType=SimpleData.Empty;

		public CourtData() {}

		public CourtData(int id, string name)
		{
			this.ID = id;
			this.Name = name;
		}
	}
	
	public class RulesetData
	{
		public int ID=-1;
		public string Name="";
		public  SportData Sport=null;
		public RegionData Region=null;
		
		public RulesetData() {}

		public RulesetData(int id, string name)
		{
			this.ID = id;
			this.Name = name;
		}
	}

	public class PlayerData
	{
		public int ID=-1;
		public StudentData Student=null;
		public TeamData Team=null;
		public int ShirtNumber=0;
		public int ChipNumber=0;
		public int Status=-1;
		public string Remarks="";
		public DateTime RegistrationDate=DateTime.MinValue;
		
		public PlayerData() {}

		public PlayerData(int id, StudentData student, TeamData team)
		{
			this.ID = id;
			this.Student = student;
			this.Team = team;
		}
	}
	
	public class TeamData
	{
		public int ID=-1;
		public SchoolData School=null;
		public ChampionshipData Championship=null;
		public CategoryData ChampionshipCategory=null;
		public int Status=-1;
		public int TeamIndex=0;
		public string Supervisor="";
		public DateTime RegistrationDate=DateTime.MinValue;
		
		public TeamData() {}

		public TeamData(int id, SchoolData school, CategoryData category)
		{
			this.ID = id;
			this.School = school;
			this.ChampionshipCategory = category;
		}
	}

	public class ChampionshipData
	{
		public int ID=-1;
		public int Season=-1;
		public string Name="";
		public RegionData Region=null;
		public SportData Sport=null;
		public bool IsClubs=false;
		public DateTime LastRegistrationDate=DateTime.MinValue;
		public DateTime StartDate=DateTime.MinValue;
		public DateTime EndDate=DateTime.MinValue;
		public DateTime AltStartDate=DateTime.MinValue;
		public DateTime AltEndDate=DateTime.MinValue;
		public DateTime FinalsDate=DateTime.MinValue;
		public DateTime AltFinalsDate=DateTime.MinValue;
		public RulesetData Ruleset=null;
		public bool IsOpen=true;
		public int Status=-1;
		public string Supervisor="";
		public SimpleData StandardChampionship=SimpleData.Empty;
		public CategoryData[] Categories=null;
		public string GameStructureRule=null;
		
		public ChampionshipData() {}
	}
	
	public class CategoryData
	{
		public int ID=-1;
		public int Championship=-1;
		public int Category=-1;
		public PhaseData[] Phases=null;
		public CategoryData() {}

		public CategoryData(int id, int championship, int category)
		{
			this.ID = id;
			this.Championship = championship;
			this.Category = category;
		}
	}
	
	public class SportFieldTypeData
	{
		public int ID=-1;
		public string Name="";
		public SportData Sport=null;
		
		public SportFieldTypeData() {}

		public SportFieldTypeData(int id, string name)
		{
			this.ID = id;
			this.Name = name;
		}
	}

	public class SportFieldData
	{
		public int ID=-1;
		public string Name="";
		public SportFieldTypeData SportFieldType=null;

		public SportFieldData() {}

		public SportFieldData(int id, string name)
		{
			this.ID = id;
			this.Name = name;
		}
	}

	public class PhaseData
	{
		public int CategoryID=-1;
		public int PhaseIndex=-1;
		public string PhaseName="";
		public int Status=-1;
		public GroupData[] Groups=null;
	}

	public class GroupData
	{
		public PhaseData Phase=null;
		public int GroupIndex=-1;
		public string GroupName="";
		public int Status=-1;
		public RoundData[] Rounds=null;
		public CompetitionData[] Competitions=null;
	}

	public class RoundData
	{
		public GroupData Group=null;
		public int RoundIndex=-1;
		public string RoundName="";
		public int Status=-1;
		public MatchData[] Matches=null;
	}

	public class MatchData
	{
		public ChampionshipData Championship=null;
		public RoundData Round=null;
		public int Cycle=0;
		public int MatchIndex=-1;
		public TeamData TeamA=null;
		public TeamData TeamB=null;
		public DateTime Time=DateTime.MinValue;
		public FacilityData Facility=null;
		public CourtData Court=null;
		public double TeamA_Score=0;
		public double TeamB_Score=0;
		public int Result=-1;
		public string PartsResult="";
		public int CategoryID=-1;
		public int PhaseIndex=-1;
		public int GroupIndex=-1;
		public string Supervisor="";
		public string Referee="";
	}

	public class CompetitionData
	{
		public GroupData Group=null;
		public int CompetitionIndex=-1;
		public SportFieldData SportField=null;
		public DateTime Time=DateTime.Now;
		public FacilityData Facility=null;
		public CourtData Court=null;
		public HeatData[] Heats=null;
		public CompetitorData[] Competitors=null;
		public int CategoryID=-1;
		public int PhaseIndex=-1;
		public int GroupIndex=-1;
	}
	
	public class HeatData
	{
		public CompetitionData Competition=null;
		public int HeatIndex=-1;
		public DateTime Time=DateTime.MinValue;
		public FacilityData Facility=null;
		public CourtData Court=null;
		public int CategoryID=-1;
	}

	public class CompetitorData
	{
		public CompetitionData Competition=null;
		public int CompetitorIndex=-1;
		public PlayerData Player=null;
		public HeatData Heat=null;
		public int Position=-1;
		public int ResultPosition=-1;
		public string Result="";
		public int Score=0;
		public string Rule="";
		public string MeasureType="";

		public string ResultFormat
		{
			get
			{
				if ((Rule == null)||(Rule.Length == 0))
					return "";
				
				string[] arrTmp=Rule.Split(new char[] {Data.ResultFormatSeperator});
				if (arrTmp.Length == 2)
				{
					return arrTmp[1];
				}
				return "";
			}
		}

		/// <summary>
		/// parse the raw result according to the result type rule.
		/// </summary>
		public string ParseResult()
		{
			//get rule fit for this competitor:
			string strRule=Tools.CStrDef(this.Rule, "");

			//nothing to do if the rule is empty - result is illegal.
			if (strRule.Length == 0)
				return null;

			//check if we have any result...
			if ((Result == null)||(Result.Length == 0))
				return null;

			////////////////////////
			//rule structure:
			//Value-Direction-Measure
			////////////////////////

			//get rule parts:
			string[] ruleParts=strRule.Split(new char[] {'-', Data.ResultFormatSeperator});

			//rule must have at least Value, Direction and Measure parts!
			if (ruleParts.Length < 3)
				return null;
			
			//get individual part values: (no need for direction in this method)
			int iRuleValue=Int32.Parse(ruleParts[0]);
			int iRuleMeasure=Int32.Parse(ruleParts[2]);

			//convert value measure to the proper type:
			Data.ResultMeasure resultMeasure=
				(Data.ResultMeasure) iRuleMeasure;
			Data.ResultValue resultValue=
				(Data.ResultValue) iRuleValue;
			
			//split measures into single array:
			System.Collections.ArrayList measures=
				Tools.GetResultMeasures(resultValue, resultMeasure);
			
			//get raw result:
			string strResult=this.Result;
			
			//decide result format according to the result value:
			MeasureType = "";
			switch (resultValue)
			{
				case Data.ResultValue.Distance:
					//seperator is the dot (e.g. 05.50 meter)
					strResult = strResult.Replace(
						Core.Data.ResultSeperator.ToString(), ".");
					//decide the maximum result measure:
					if ((resultMeasure & Core.Data.ResultMeasure.Kilometers) != 0)
						MeasureType = "קילומטר";
					else if ((resultMeasure & Core.Data.ResultMeasure.Meters) != 0)
						MeasureType = "מטר";
					else if ((resultMeasure & Core.Data.ResultMeasure.Centimeters) != 0)
						MeasureType = "סנטימטר";
					else if ((resultMeasure & Core.Data.ResultMeasure.Milimeters) != 0)
						MeasureType = "מילימטר";
					break;
				case Core.Data.ResultValue.Time:
					//seperator is the ":" character (e.g. 02:30.150 minutes)
					strResult = strResult.Replace(
						Core.Data.ResultSeperator.ToString(), ":");
					//miliseconds problem: seperator is "."
					if ((resultMeasure & Core.Data.ResultMeasure.Miliseconds) != 0)
					{
						//valid only if there are any miliseconds...
						string[] arrTemp=strResult.Split(new char[] {':'});
						if (arrTemp.Length == measures.Count)
						{
							strResult = "";
							for (int j=0; j<arrTemp.Length; j++)
							{
								strResult += arrTemp[j];
								if (j < (arrTemp.Length-1))
								{
									strResult += (j == (arrTemp.Length-2))?".":":";
								}
							}
						}
					}
					
					//decide the maximum result measure:
					if ((resultMeasure & Core.Data.ResultMeasure.Days) != 0)
						MeasureType = "ימים";
					else if ((resultMeasure & Core.Data.ResultMeasure.Hours) != 0)
						MeasureType = "שעות";
					else if ((resultMeasure & Core.Data.ResultMeasure.Minutes) != 0)
						MeasureType = "דקות";
					else if ((resultMeasure & Core.Data.ResultMeasure.Seconds) != 0)
						MeasureType = "שניות";
					else if ((resultMeasure & Core.Data.ResultMeasure.Miliseconds) != 0)
						MeasureType = "אלפיות שניה";
					break;
				case Data.ResultValue.Points:
					//most simple.
					MeasureType = "נקודות";
					break;
			} //end switch iRuleValue

			//add measure type:
			if (MeasureType.Length > 0)
				strResult += " "+MeasureType;
			
			return strResult;
		}
	}
	
	public class GroupTeamData
	{
		public GroupData Group=null;
		public int Position=-1;
		public int PreviousGroup=-1;
		public int PreviousPosition=-1;
		public TeamData Team=null;
		public int Games=0;
		public int Points=0;
		public int PointsAgainst=0;
		public int SmallPoints=0;
		public int SmallPointsAgainst=0;
		public double Score=0;
		public int ResultPosition=-1;
	}

	public class SeasonData
	{
		public int Season { get; set; }
		public string Name { get; set; }
		public int Status { get; set; }
		public DateTime Start { get; set; }
		public DateTime End { get; set; }
	}

	public class RuleData
	{
		public int RuleID=-1;
		public RulesetData Ruleset=null;
		public SimpleData RuleType=SimpleData.Empty;
		public string Value="";
		public SportFieldTypeData SportFieldType=null;
		public SportFieldData SportField=null;
		public int Category=-1;

		public RuleData() {}
	}

	public class ScoreRangeData
	{
		public int ID=-1;
		public SportFieldTypeData SportFieldType=null;
		public SportFieldData SportField=null;
		public int Category=-1;
		public int LowerLimit=0;
		public int UpperLimit=0;
		public int Score=0;

		public ScoreRangeData() {}
	}
}
