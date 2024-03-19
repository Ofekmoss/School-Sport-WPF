using System;
using System.Collections;
using Sport.Entities;

namespace Sport.Championships
{
	#region Event class - event data
	public class Event
	{
		#region private members
		private ChampionshipCategory _champCategory=null;
		private Court _court=null;
		private Facility _facility=null;
		private SportField _sportField=null;
		private Sport.Entities.Team _teamA=null; //for match championship
		private Sport.Entities.Team _teamB=null; //for match championship
		private Sport.Championships.MatchOutcome _matchResult; //for match championship
		private DateTime _date=DateTime.MinValue;
		private string _groupName=null; //bait!
		private string _phaseName=null;
		private string _referee=null;
		private string _supervisor=null;
		private string _phone=null;
		private string _teamA_name=null;
		private string _teamB_name=null;
		private double _teamA_score=Double.MinValue; //for match championship
		private double _teamB_score=Double.MinValue; //for match championship
		private int _groupIndex=-1;  //needed for comparing
		private int _phaseIndex=-1;  //needed for comparing
		private int _roundIndex=-1;
		private int _cycleIndex=-1;
		private int _matchIndex=-1;
		#endregion
		
		#region public properties
		public DateTime Date
		{
			get {return _date;}
		}
		
		public ChampionshipCategory ChampCategory
		{
			get {return _champCategory;}
		}
		
		public int GroupIndex
		{
			get {return _groupIndex;}
		}
		
		public string GroupName
		{
			get {return _groupName;}
		}
		
		public Sport.Entities.Team TeamA
		{
			get {return _teamA;}
		}
		
		public Sport.Entities.Team TeamB
		{
			get	{return _teamB;}
		}
		
		public string TeamName_A
		{
			get	{return (this.TeamA == null)?_teamA_name:this.TeamA.Name;}
		}
		
		public string TeamName_B
		{
			get	{return (this.TeamB == null)?_teamB_name:this.TeamB.Name;}
		}
		
		public SportField SportField
		{
			get {return _sportField;}
		}
		
		public Facility Facility
		{
			get {return _facility;}
		}
		
		public Court Court
		{
			get {return _court;}
		}
		
		public string Referee
		{
			get {return _referee;}
		}
		
		public string Supervisor
		{
			get {return _supervisor;}
		}
		
		public string Phone
		{
			get {return _phone;}
		}
		
		public Sport.Championships.MatchOutcome MatchResult
		{
			get {return _matchResult;}
		}
		
		public double TeamA_Score
		{
			get {return _teamA_score;}
		}
		
		public double TeamB_Score
		{
			get {return _teamB_score;}
		}
		
		public int PhaseIndex
		{
			get {return _phaseIndex;}
		}
		
		public string PhaseName
		{
			get {return _phaseName;}
		}
		
		public int RoundIndex
		{
			get {return _roundIndex;}
		}
		
		public int CycleIndex
		{
			get {return _cycleIndex;}
		}
		public int MatchIndex
		{
			get {return _matchIndex;}
		}		
		#endregion

		#region public methods
		#region get match
		public Sport.Championships.Match GetMatch(ref string strError)
		{
			//match championship?
			if (this.SportField != null)
			{
				strError = "ענף ספורט לא מסוג תחרות";
				return null;
			}
			
			//load the championship:
			Championship championship=Championship.GetChampionship(
				this.ChampCategory.Id);
			
			//valid phase?
			if ((this.PhaseIndex >= 0)&&(this.PhaseIndex < championship.Phases.Count))
			{
				//get phase:
				Phase phase=championship.Phases[this.PhaseIndex];
				
				//valid group?
				if ((this.GroupIndex >= 0)&&(this.GroupIndex < phase.Groups.Count))
				{
					//get group:
					MatchGroup group=(MatchGroup) phase.Groups[this.GroupIndex];
					
					//valid round?
					if ((this.RoundIndex >= 0)&&(this.RoundIndex < group.Rounds.Count))
					{
						//get round:
						Round round=group.Rounds[this.RoundIndex];
						
						//valid cycle?
						if ((this.CycleIndex >= 0)&&(this.CycleIndex < round.Cycles.Count))
						{
							//get cycle:
							Cycle cycle=round.Cycles[this.CycleIndex];
							
							//valid match?
							if ((this.MatchIndex >= 0)&&(this.MatchIndex < cycle.Matches.Count))
								return cycle.Matches[this.MatchIndex];
							else
								strError = "משחק לא בטווח";
						}
						else
						{
							strError = "מחזור לא בטווח";
						}
					}
					else
					{
						strError = "סיבוב לא בטווח";
					}
				}
				else
				{
					strError = "בית לא בטווח";
				}
			}
			else
			{
				strError = "שלב אליפות לא בטווח";
			}
			
			//invalid.
			return null;
		}
		#endregion
		
		#region Get Place
		public string GetPlace()
		{
			string result="";
			if (this.Facility != null)
				result += this.Facility.Name;
			if (this.Court != null)
			{
				if (result.Length > 0)
					result += " ";
				result += this.Court.Name;
			}
			return result;
		}
		#endregion
		
		#region get teams or competition names
		public string GetTeamsOrCompetition()
		{
			if (this.SportField != null)
				return this.SportField.Name;
			string strName1=this.TeamName_A;
			string strName2=this.TeamName_B;
			return strName1+" - "+strName2;
		}
		#endregion
		
		#region get score
		public string GetScore(bool reverse)
		{
			if (this.MatchResult == Sport.Championships.MatchOutcome.None)
				return "";
			if (reverse)
				return this.TeamB_Score+":"+this.TeamA_Score;
			return this.TeamA_Score+":"+this.TeamB_Score;
		}
		
		public string GetScore()
		{
			return GetScore(false);
		}
		#endregion
		
		#region get phase and group together
		public string GetPhaseAndGroup()
		{
			return this.PhaseName+" - "+this.GroupName;
		}
		#endregion
		#endregion
		
		#region constructors
		/// <summary>
		/// given data must be DataServices.MatchData or DataServices.CompetitionData
		/// </summary>
		public Event(object data)
		{
			if ((data is DataServices.MatchData)||(data is DataServices.CompetitionData))
			{
				this._champCategory = TryGetCategory(ExtractCategory(data));
				this._date = ExtractDate(data);
				this._court = TryGetCourt(ExtractCourt(data));
				this._facility = TryGetFacility(ExtractFacility(data));
				this._groupIndex = ExtractGroupIndex(data);
				this._groupName = ExtractGroupName(data);
				this._matchResult = ExtractMatchResult(data);
				this._teamA_score = ExtractTeamA_Score(data);
				this._teamB_score = ExtractTeamB_Score(data);
				this._sportField = TryGetSportField(ExtractSportField(data));
				this._phaseIndex = ExtractPhaseIndex(data);
				this._phaseName = ExtractPhaseName(data);
				this._supervisor = ExtractSupervisor(data);
				this._referee = ExtractReferee(data);
				this._roundIndex = ExtractRoundIndex(data);
				this._cycleIndex = ExtractCycleIndex(data);
				this._matchIndex = ExtractMatchIndex(data);
				ExtractTeams(data);
			}
			else
			{
				throw new Exception("Event: got invalid object: "+data.GetType().Name);
			}
		}
		#endregion

		#region private tools
		#region try get entities - return null in case of problems
		private Sport.Entities.ChampionshipCategory TryGetCategory(int id)
		{
			if (id < 0)
				return null;
			Sport.Entities.ChampionshipCategory result=null;
			try
			{
				result = new Sport.Entities.ChampionshipCategory(id);
			}
			catch {}
			if ((result != null)&&((result.Id < 0)||(result.Championship == null)))
				result = null;
			return result;
		}
		
		private Sport.Entities.Court TryGetCourt(int id)
		{
			if (id < 0)
				return null;
			Sport.Entities.Court result=null;
			try
			{
				result = new Sport.Entities.Court(id);
			}
			catch {}
			if ((result != null)&&(result.Id < 0))
				result = null;
			return result;
		}
		
		private Sport.Entities.Facility TryGetFacility(int id)
		{
			if (id < 0)
				return null;
			Sport.Entities.Facility result=null;
			try
			{
				result = new Sport.Entities.Facility(id);
			}
			catch {}
			if ((result != null)&&(result.Id < 0))
				result = null;
			return result;
		}
		
		private Sport.Entities.SportField TryGetSportField(int id)
		{
			if (id < 0)
				return null;
			Sport.Entities.SportField result=null;
			try
			{
				result = new Sport.Entities.SportField(id);
			}
			catch {}
			if ((result != null)&&((result.Id < 0)||(result.SportFieldType == null)))
				result = null;
			return result;
		}
		
		private Sport.Entities.Team TryGetTeam(int id)
		{
			if (id < 0)
				return null;
			Sport.Entities.Team result=null;
			try
			{
				result = new Sport.Entities.Team(id);
			}
			catch {}
			if ((result != null)&&((result.Id < 0)||(result.School == null)))
				result = null;
			return result;
		}
		#endregion
		
		#region extract data from MatchData or CompetitionData
		private int ExtractCategory(object o)
		{
			if (o is DataServices.MatchData)
				return (o as DataServices.MatchData).CategoryID;
			return (o as DataServices.CompetitionData).CategoryID;
		}
		
		private DateTime ExtractDate(object o)
		{
			if (o is DataServices.MatchData)
				return (o as DataServices.MatchData).Time;
			return (o as DataServices.CompetitionData).Time;
		}
		
		private int ExtractCourt(object o)
		{
			DataServices.CourtData court=null;
			if (o is DataServices.MatchData)
				court = (o as DataServices.MatchData).Court;
			else
				court = (o as DataServices.CompetitionData).Court;
			if (court != null)
				return court.ID;
			return -1;
		}
		
		private int ExtractFacility(object o)
		{
			DataServices.FacilityData facility=null;
			if (o is DataServices.MatchData)
				facility = (o as DataServices.MatchData).Facility;
			else
				facility = (o as DataServices.CompetitionData).Facility;
			if (facility != null)
				return facility.ID;
			return -1;
		}
		
		private int ExtractGroupIndex(object o)
		{
			if (o is DataServices.MatchData)
				return (o as DataServices.MatchData).Round.Group.GroupIndex;
			return (o as DataServices.CompetitionData).GroupIndex;
		}
		
		private string ExtractGroupName(object o)
		{
			if (o is DataServices.MatchData)
				return (o as DataServices.MatchData).Round.Group.GroupName;
			return (o as DataServices.CompetitionData).Group.GroupName;
		}
		
		private Sport.Championships.MatchOutcome ExtractMatchResult(object o)
		{
			if (o is DataServices.MatchData)
			{
				try
				{
					return (Sport.Championships.MatchOutcome) (o as DataServices.MatchData).Result;
				}
				catch
				{
					return Sport.Championships.MatchOutcome.None;
				}
			}
			return Sport.Championships.MatchOutcome.None;
		}
		
		private double ExtractTeamA_Score(object o)
		{
			if (o is DataServices.MatchData)
				return (o as DataServices.MatchData).TeamA_Score;
			return -99999;
		}
		
		private double ExtractTeamB_Score(object o)
		{
			if (o is DataServices.MatchData)
				return (o as DataServices.MatchData).TeamB_Score;
			return -99999;
		}
		private int ExtractSportField(object o)
		{
			if (o is DataServices.MatchData)
				return -1;
			return (o as DataServices.CompetitionData).SportField.ID;
		}
		
		private void ExtractTeams(object o)
		{
			if (o == null)
				return;
			if (o is DataServices.MatchData)
			{
				DataServices.MatchData match=(DataServices.MatchData) o;
				int teamA_id=-1;
				int teamB_id=-1;
				_teamA_name = "";
				if (match.TeamA != null)
					teamA_id = match.TeamA.ID;
				if (match.TeamB != null)
					teamB_id = match.TeamB.ID;
				if (teamA_id < 0)
				{
					if ((match.TeamA != null)&&(match.TeamA.School != null))
						_teamA_name = match.TeamA.School.Name;
					_teamA = null;
				}
				else
				{
					_teamA = TryGetTeam(teamA_id);
					if (_teamA != null)
						_teamA_name = _teamA.Name;
				}
				if (teamB_id < 0)
				{
					if ((match.TeamB != null)&&(match.TeamB.School != null))
						_teamB_name = match.TeamB.School.Name;
					_teamB = null;
				}
				else
				{
					_teamB = TryGetTeam(teamB_id);
					if (_teamB != null)
						_teamB_name = _teamB.Name;
				}
			}
		}
		
		private int ExtractPhaseIndex(object o)
		{
			if (o is DataServices.MatchData)
				return (o as DataServices.MatchData).Round.Group.Phase.PhaseIndex;
			return (o as DataServices.CompetitionData).PhaseIndex;
		}
		
		private int ExtractRoundIndex(object o)
		{
			if (o is DataServices.MatchData)
				return (o as DataServices.MatchData).Round.RoundIndex;
			return -1;
		}
		
		private int ExtractCycleIndex(object o)
		{
			if (o is DataServices.MatchData)
				return (o as DataServices.MatchData).Cycle;
			return -1;
		}
		
		private int ExtractMatchIndex(object o)
		{
			if (o is DataServices.MatchData)
				return (o as DataServices.MatchData).MatchIndex;
			return -1;
		}
		
		private string ExtractPhaseName(object o)
		{
			if (o is DataServices.MatchData)
				return (o as DataServices.MatchData).Round.Group.Phase.PhaseName;
			return (o as DataServices.CompetitionData).Group.Phase.PhaseName;
		}
		
		private string ExtractSupervisor(object o)
		{
			if (o is DataServices.MatchData)
				return (o as DataServices.MatchData).Supervisor;
			return "";
		}
		
		private string ExtractReferee(object o)
		{
			if (o is DataServices.MatchData)
				return (o as DataServices.MatchData).Referee;
			return "";
		}
		#endregion
		#endregion
	}
	#endregion
	
	/// <summary>
	/// Handle all kinds of events
	/// </summary>
	public class Events
	{
		#region constructor
		public Events()
		{
			//
			// TODO: Add constructor logic here
			//
		}
		#endregion
		
		#region public methods
		#region Get Events
		/// <summary>
		/// get all events in given date range. event is either match or competition.
		/// </summary>
		/// <param name="start">starting date and time</param>
		/// <param name="end">ending date and time</param>
		/// <returns>collection of events in the time range</returns>
		public static Event[] GetEventsInRange(DateTime start, DateTime end, int region, 
			int sport, int championship, int category, int facility, int city, int school)
		{
			if (!Sport.Core.Session.Connected)
				return null;

			//initialize result collection:
			ArrayList result=new ArrayList();
			
			//initialize service and give permissions:
			DataServices.DataService _service=new DataServices.DataService();
			_service.CookieContainer = Sport.Core.Session.Cookies;
			
			//when we have school, region is not relevant
			if (school >= 0)
				region = -1;
			
			//get matches from database:
			DataServices.MatchData[] matches=_service.GetRawMatchesByDate(start, end, region, 
				sport, championship, category, facility);
			
			//get competitions from database:
			DataServices.CompetitionData[] competitions=_service.GetRawCompetitionsByDate(start, end, 
				region, sport, championship, facility, category);
			
			//iterate over matches and add to result collection:
			if (matches != null)
			{
				foreach (DataServices.MatchData match in matches)
				{
					//build current event and add if valid:
					Event e=new Event(match);
					if (e.ChampCategory != null)
					{
						bool blnAdd=true;
						if (city >= 0)
						{
							blnAdd = false;
							if ((e.Facility != null)&&(e.Facility.City != null))
								blnAdd = (e.Facility.City.Id == city);
						}
						if (school >= 0)
						{
							blnAdd = false;
							if ((e.TeamA != null)&&(e.TeamB != null))
							{
								if ((e.TeamA.School != null)&&(e.TeamA.School.Id == school))
									blnAdd = true;
								if ((e.TeamB.School != null)&&(e.TeamB.School.Id == school))
									blnAdd = true;
							}
						}
						if (blnAdd)
							result.Add(e);
					}
				} //end loop over matches
			}
			
			//iterate over competitions and add to result collection:
			if (competitions != null)
			{
				foreach (DataServices.CompetitionData competition in competitions)
				{
					//build current event and add if valid:
					Event e=new Event(competition);
					if (e.ChampCategory != null)
					{
						result.Add(e);
					}
				} //end loop over competitions
			}

			//sort:
			result.Sort(new EventComparer());
			
			return (Event[]) result.ToArray(typeof(Event));
		}
		
		/// <summary>
		/// get all events in given day
		/// </summary>
		public static Event[] GetDayEvents(DateTime day)
		{
			DateTime start=Sport.Common.Tools.SetTime(day, 0, 0, 0);
			DateTime end=Sport.Common.Tools.SetTime(day, 23, 59, 59);
			return GetEventsInRange(start, end, -1, -1, -1, -1, -1, -1, -1);
		}
		#endregion
		#endregion
		
		#region event comparer
		#region general comparer
		private class EventComparer : IComparer
		{
			public int Compare(object x, object y)
			{
				Event e1=(Event) x;
				Event e2=(Event) y;

				//System.Diagnostics.Debug.WriteLine("group1: "+e1.GroupIndex+", group2: "+e2.GroupIndex);

				//first by championship
				if (e1.ChampCategory.Id == e2.ChampCategory.Id)
				{
					//getting here means the same championship. second is by phase:
					if (e1.PhaseIndex == e2.PhaseIndex)
					{
						//getting here means the same phase. third is by group:
						if (e1.GroupIndex == e2.GroupIndex)
						{
							//getting here means the same group. last is by date:
							return e1.Date.CompareTo(e2.Date);
						}
						return e1.GroupIndex.CompareTo(e2.GroupIndex);
					}
					return e1.PhaseIndex.CompareTo(e2.PhaseIndex);
				}
				return e1.ChampCategory.Id.CompareTo(e2.ChampCategory.Id);
			}
		}
		#endregion
		
		#region date comparer
		public class EventDateComparer : IComparer
		{
			private bool _champCompare=true;
			
			public EventDateComparer(bool blnCompareChampionship)
			{
				_champCompare = blnCompareChampionship;
			}
			
			public EventDateComparer()
				: this(true)
			{
			}
			
			public int Compare(object x, object y)
			{
				//get events:
				Sport.Championships.Event e1=(Sport.Championships.Event) x;
				Sport.Championships.Event e2=(Sport.Championships.Event) y;
				
				//get championships:
				int category1=e1.ChampCategory.Id;
				int category2=e2.ChampCategory.Id;
				
				//get dates:
				DateTime date1=e1.Date;
				DateTime date2=e2.Date;
				
				//first level - not in the same day
				if (!Sport.Common.Tools.IsSameDate(date1, date2))
					return date1.CompareTo(date2);
				
				//second level - championship
				if ((_champCompare)&&(category1 != category2))
					return category1.CompareTo(category2);
				
				//get places:
				string p1=Sport.Common.Tools.CStrDef(e1.GetPlace(), "");
				string p2=Sport.Common.Tools.CStrDef(e2.GetPlace(), "");
				
				//third level - place
				if (p1 != p2)
					return p1.CompareTo(p2);
				
				//last level - date.
				return date1.CompareTo(date2);
			}
		}
		#endregion
		#endregion
	}
}
