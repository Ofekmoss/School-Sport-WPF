using System;

namespace Sport.Championships
{
	public enum MatchOutcome
	{
		None = -1,
		Tie = 0,
		WinA,
		WinB,
		TechnicalA,
		TechnicalB
	}

	public enum MatchRoleType
	{
		Undefined=-1,
		Supervisor=0,
		Referee=1
	}

	#region match functionary
	public class MatchFunctionary
	{
		private int _id;
		private MatchRoleType _roleType;
		private string _roleName;
		
		public MatchFunctionary()
		{
			_id = -1;
			_roleType = MatchRoleType.Undefined;
			_roleName = "";
		}
		
		public MatchFunctionary(int id, MatchRoleType roleType, string roleName)
			: this()
		{
			Id = id;
			RoleType = roleType;
			RoleName = roleName;
		}
		
		public int Id
		{
			get {return _id;}
			set {_id = value;}
		}

		public MatchRoleType RoleType
		{
			get {return _roleType;}
			set {_roleType = value;}
		}

		public string RoleName
		{
			get {return _roleName;}
			set {_roleName = value;}
		}
	}
	#endregion

	/// <summary>
	/// The Match class holds the information of a single
	/// match in the championship.
	/// Each match contains a name (usually the number of match in the round)
	/// a round that the match belong to, the match teams,
	/// the time of the match, the court and the match result
	/// </summary>
	public class Match : Sport.Common.GeneralCollection.CollectionItem
	{
		#region Properties

		public bool Editable
		{
			get
			{
				return Cycle == null || Cycle.Editable;
			}
		}

		internal int _index;
		public int Index
		{
			get { return _index; }
		}

		private int _number;
		public int Number
		{
			get { return _number; }
			set 
			{ 
				if (!Editable)
					throw new ChampionshipException("Not in edit - cannot be set");

				if (Cycle != null)
					Cycle.Round.Group.SetMatchNumber(this, _number, value);
				_number = value; 
			}
		}

		private int _tournament;
		public int Tournament
		{
			get { return _tournament; }
			set 
			{ 
				/*
				if (!Editable)
					throw new ChampionshipException("Not in edit - cannot be set");
				*/
				_tournament = value; 
			}
		}
		
		private int[] _functionaries;
		public int[] Functionaries
		{
			get {return _functionaries;}
			set
			{
				if (!Editable)
					throw new ChampionshipException("Not in edit - cannot be set");
				_functionaries = value;
			}
		}
		
		private int _teamA;
		public int TeamA
		{
			get { return _teamA; }
			set 
			{ 
				if (!Editable)
					throw new ChampionshipException("Not in edit - cannot be set");

				_teamA = value; 
			}
		}
		
		public MatchTeam GroupTeamA
		{
			get
			{
				if (_teamA != -1 && Cycle != null && Cycle.Round != null)
				{
					MatchGroup group = Cycle.Round.Group;
					if (group != null)
					{
						if ((group.Teams != null)&&(_teamA < group.Teams.Count))
						{
							return group.Teams[_teamA];
						}
					}
				}
				return null;
			}
		}
		
		private MatchChampionship GetCycleChampionship(Sport.Championships.Cycle cycle)
		{
			if (cycle == null)
				return null;

			Sport.Championships.Round round = cycle.Round;
			if (round == null)
				return null;
			
			Sport.Championships.MatchGroup group = round.Group;
			if (group == null)
				return null;

			Sport.Championships.MatchPhase phase = group.Phase;
			if (phase == null)
				return null;

			return phase.Championship;
		}

		private int _relativeTeamA;
		public int RelativeTeamA
		{
			get { return _relativeTeamA; }
			set 
			{ 
				if (!Editable)
					throw new ChampionshipException("Not in edit - cannot be set");

				_relativeTeamA = value; 
			}
		}

		public MatchTeam RelativeGroupTeamA
		{
			get
			{
				if (_relativeTeamA != 0)
				{
					Sport.Championships.MatchChampionship champ = GetCycleChampionship(this.Cycle);
					if (champ != null)
					{
						Sport.Championships.Match relativeMatch = champ.GetMatch(_relativeTeamA < 0 ? -_relativeTeamA : _relativeTeamA);
						
						if (relativeMatch != null)
						{
							if (relativeMatch.Outcome == MatchOutcome.WinA || relativeMatch.Outcome == MatchOutcome.TechnicalA)
							{
								return _relativeTeamA < 0 ? relativeMatch.GroupTeamB : relativeMatch.GroupTeamA;
							}
							else if (relativeMatch.Outcome == MatchOutcome.WinB || relativeMatch.Outcome == MatchOutcome.TechnicalB)
							{
								return _relativeTeamA < 0 ? relativeMatch.GroupTeamA : relativeMatch.GroupTeamB;
							}
						}
					}
				}
				
				return null;
			}
		}

		public MatchTeam MatchTeamA
		{
			get
			{
				MatchTeam team = GroupTeamA;
				if (team == null)
					team = RelativeGroupTeamA;
				return team;
			}
		}

		private int _teamB;
		public int TeamB
		{
			get { return _teamB; }
			set 
			{ 
				if (!Editable)
					throw new ChampionshipException("Not in edit - cannot be set");

				_teamB = value; 
			}
		}
		
		public MatchTeam GroupTeamB
		{
			get 
			{
				if (_teamB != -1 && Cycle != null && Cycle.Round != null)
				{
					MatchGroup group = Cycle.Round.Group;
					if (group != null)
					{
						if ((group.Teams != null)&&(_teamB < group.Teams.Count))
						{
							return group.Teams[_teamB];
						}
					}
				}
				return null;
			}
		}
		
		private int _relativeTeamB;
		public int RelativeTeamB
		{
			get { return _relativeTeamB; }
			set 
			{ 
				if (!Editable)
					throw new ChampionshipException("Not in edit - cannot be set");

				_relativeTeamB = value; 
			}
		}
		
		public MatchTeam RelativeGroupTeamB
		{
			get
			{
				if (_relativeTeamB != 0)
				{
					Sport.Championships.MatchChampionship champ = GetCycleChampionship(this.Cycle);
					if (champ != null)
					{
						Sport.Championships.Match relativeMatch = champ.GetMatch(_relativeTeamB < 0 ? -_relativeTeamB : _relativeTeamB);
						
						if (relativeMatch != null)
						{
							if (relativeMatch.Outcome == MatchOutcome.WinA || relativeMatch.Outcome == MatchOutcome.TechnicalA)
							{
								return _relativeTeamB < 0 ? relativeMatch.GroupTeamB : relativeMatch.GroupTeamA;
							}
							else if (relativeMatch.Outcome == MatchOutcome.WinB || relativeMatch.Outcome == MatchOutcome.TechnicalB)
							{
								return _relativeTeamB < 0 ? relativeMatch.GroupTeamA : relativeMatch.GroupTeamB;
							}
						}
					}
				}
				
				return null;
			}
		}

		public MatchTeam MatchTeamB
		{
			get
			{
				MatchTeam team = GroupTeamB;
				if (team == null)
					team = RelativeGroupTeamB;
				return team;
			}
		}

		private DateTime _time;
		public DateTime Time
		{
			get { return _time; }
			set
			{
				if (!Editable)
					throw new ChampionshipException("Not in edit - cannot be set");

				_time = value;
			}
		}
		
		private string _customTeamA="";
		public string CustomTeamA
		{
			get { return _customTeamA; }
			set { _customTeamA = value;}
		}
		
		private string _customTeamB="";
		public string CustomTeamB
		{
			get { return _customTeamB; }
			set { _customTeamB = value;}
		}
		
		private DateTime _dateChangedDate=DateTime.MinValue;
		public DateTime DateChangedDate
		{
			get { return _dateChangedDate; }
		}
		
		private Sport.Entities.Facility _facility;
		public Sport.Entities.Facility Facility
		{
			get { return _facility; }
			set
			{
				if (!Editable)
					throw new ChampionshipException("Not in edit - cannot be set");

				_facility = value; 
			}
		}
		
		private Sport.Entities.Court _court;
		public Sport.Entities.Court Court
		{
			get { return _court; }
			set
			{
				if (!Editable)
					throw new ChampionshipException("Not in edit - cannot be set");

				_court = value; 
			}
		}
		
		internal double _teamAScore;
		public double TeamAScore
		{
			get { return _teamAScore; }
		}
		
		internal double _teamBScore;
		public double TeamBScore
		{
			get { return _teamBScore; }
		}
		
		internal MatchOutcome _outcome;
		public MatchOutcome Outcome
		{
			get { return _outcome; }
		}
		
		internal string _partsResult;
		public string PartsResult
		{
			get { return _partsResult; }
			set { _partsResult = value; }
		}
		
		internal int _refereeCount;
		public int RefereeCount
		{
			get { return _refereeCount; }
		}
		

		internal GameResult _gameResult = null;
		public GameResult GameResult
		{
			get
			{
				if (_gameResult == null && _partsResult != null)
					_gameResult = new GameResult(_partsResult);
				return _gameResult;
			}
		}
		
		public Cycle Cycle
		{
			get
			{
				return ((Cycle) Owner); 
			}
		}
		#endregion

		#region Constructors

		public Match(int teamA, int teamB, Match match)
		{
			_index = -1;
			_teamA = teamA;
			_teamB = teamB;
			_relativeTeamA = 0;
			_relativeTeamB = 0;
			_number = match.Number;
			_tournament = match.Tournament;
			_time = match.Time;
			_court = match.Court;
			_facility = match.Facility;
			_teamAScore = match.TeamAScore;
			_teamBScore = match.TeamBScore;
			_outcome = match.Outcome;
			_partsResult = match.PartsResult;
			_refereeCount = match.RefereeCount;
			_customTeamA = match.CustomTeamA;
			_customTeamB = match.CustomTeamB;
			_gameResult = null;
			_functionaries = match._functionaries;
		}

		public Match(int number, int teamA, int teamB)
		{
			_index = -1;
			_teamA = teamA;
			_teamB = teamB;
			_relativeTeamA = 0;
			_relativeTeamB = 0;
			_number = number;
			_tournament = -1;
			_time = new DateTime(0);
			_court = null;
			_facility = null;
			_teamAScore = -1;
			_teamBScore = -1;
			_outcome = MatchOutcome.None;
			_partsResult = null;
			_gameResult = null;
			_functionaries = new int[0];
		}

		internal Match(SportServices.Match smatch)
		{
			_index = -1;
			_number = smatch.Number;
			_tournament = smatch.Tournament;
			_teamA = smatch.TeamAPos;
			_teamB = smatch.TeamBPos;
			_relativeTeamA = smatch.RelativeTeamA;
			_relativeTeamB = smatch.RelativeTeamB;
			_time = smatch.Time;
			_functionaries = smatch.Functionaries;
			_dateChangedDate = smatch.DateChangedDate;
			if (_functionaries == null)
				_functionaries = new int[0];
			
			if (smatch.Facility == -1)
			{
				_facility = null;
			}
			else
			{
				_facility = new Sport.Entities.Facility(smatch.Facility);
				if (!_facility.IsValid())
					_facility = null;
			}
            
			if (smatch.Court == -1)
			{
				_court = null;
			}
			else
			{
				_court = new Sport.Entities.Court(smatch.Court);
				if (!_court.IsValid())
					_court = null;
			}

			_teamAScore = smatch.TeamAScore;
			_teamBScore = smatch.TeamBScore;
			_outcome = (MatchOutcome) smatch.Outcome;
			_partsResult = smatch.PartsResult;
			_refereeCount = smatch.RefereeCount;
			_customTeamA = smatch.CustomTeamA;
			_customTeamB = smatch.CustomTeamB;
			_gameResult = null;
		}
		#endregion

		public void Reset()
		{
			_teamAScore = -1;
			_teamBScore = -1;
			_outcome = MatchOutcome.None;
		}

		public override void OnOwnerChange(object oo, object no)
		{
			if (no == null)
			{
				_index = -1;
			}
		}
		
		/// <summary>
		/// returns array with two items - the first is Team A small points,
		/// the second Team B small points. calculated based on the parts result.
		/// </summary>
		public int[] CalcSmallPoints()
		{
			int teamA_smallPoints=0;
			int teamB_smallPoints=0;
			string strPartsResult=this.PartsResult;
			if ((strPartsResult != null)&&(strPartsResult.Length > 0))
			{
				string[] arrGameResults=strPartsResult.Split(new char[] {'|'});
				for (int gameIndex=0; gameIndex<arrGameResults.Length; gameIndex++)
				{
					string gameResult=arrGameResults[gameIndex];
					string[] arrPartsResults=gameResult.Split(new char[] {'-'});
					if (arrPartsResults.Length == 2)
					{
						teamA_smallPoints += Int32.Parse(arrPartsResults[0]);
						teamB_smallPoints += Int32.Parse(arrPartsResults[1]);
					}
				}
			}
			return new int[] {teamA_smallPoints, teamB_smallPoints};
		}

		private void OnScoreChange()
		{
			if (Cycle != null)
			{
				Round round = Cycle.Round;
				if (round != null && round.Group != null)
					round.Group.CalculateTeamsScore();
			}
		}

		internal SportServices.MatchGroupMatchResult GetMatchResult()
		{
			SportServices.MatchGroupMatchResult mgmr = new Sport.Championships.SportServices.MatchGroupMatchResult();
			mgmr.Round = Cycle.Round.Index;
			mgmr.Cycle = Cycle.Index;
			mgmr.Match = Index;
			mgmr.TeamAScore = _teamAScore;
			mgmr.TeamBScore = _teamBScore;
			mgmr.Outcome = (int) _outcome;
			mgmr.PartsResult = _partsResult;

			return mgmr;
		}
		
		internal SportServices.Match Save()
		{
			SportServices.Match smatch = new SportServices.Match();
			smatch.Number = _number;
			smatch.Tournament = _tournament;
			smatch.TeamAPos = _teamA;
			smatch.TeamBPos = _teamB;
			smatch.RelativeTeamA = _relativeTeamA;
			smatch.RelativeTeamB = _relativeTeamB;
			smatch.Time = _time;
			smatch.Facility = _facility == null ? -1 : _facility.Id;
			smatch.Court = _court == null ? -1 : _court.Id;
			smatch.TeamAScore = _teamAScore;
			smatch.TeamBScore = _teamBScore;
			smatch.Outcome = (int) _outcome;
			smatch.PartsResult = _partsResult;
			smatch.RefereeCount = _refereeCount;
			smatch.CustomTeamA = _customTeamA;
			smatch.CustomTeamB = _customTeamB;
			smatch.Functionaries = _functionaries;
			return smatch;
		}
		
		public override string ToString()
		{
			return "\"" + GetTeamAName() + "\" -- \"" + GetTeamBName() + "\"";
		}

		public bool Set(int number, DateTime time, Sport.Entities.Facility facility, 
			Sport.Entities.Court court, int[] functionaries)
		{
			Round round = Cycle.Round;
			Group group = round.Group;
			Phase phase = group.Phase;
			SportServices.ChampionshipService cs = new SportServices.ChampionshipService();
			cs.CookieContainer = Sport.Core.Session.Cookies;
			if (cs.SaveMatch(phase.Championship.ChampionshipCategory.Id,
				phase.Index,
				group.Index,
				round.Index,
				Cycle.Index,
				_index,
				number,
				Sport.Common.Tools.TimeToDatabaseString(time), 
				(facility == null ? -1 : facility.Id), 
				(court == null ? -1 : court.Id), 
				functionaries))
			{
				_time = time;
				Cycle.Round.Group.SetMatchNumber(this, _number, number);
				_number = number;
				_facility = facility;
				_court = court;
				_functionaries = functionaries;
				return true;
			}
			return false;
		}

		public bool SetResult(MatchOutcome outcome, double teamAScore, double teamBScore, string partsResult)
		{
			/* if (Cycle.Round.Group.Phase.Status == Status.Planned)
				throw new ChampionshipException("Phase not started, cannot insert results"); */

			double tas = _teamAScore;
			double tbs = _teamBScore;
			string pr = _partsResult;
			MatchOutcome moc = _outcome;

			_teamAScore = teamAScore;
			_teamBScore = teamBScore;
			_partsResult = partsResult;
			_outcome = outcome;
			OnScoreChange();

            MatchGroup group = Cycle.Round.Group;

			SportServices.MatchResult sresult = new SportServices.MatchResult();
			sresult.ChampionshipCategoryId = group.Phase.Championship.ChampionshipCategory.Id;
			sresult.Phase = group.Phase.Index;
			sresult.Group = group.Index;
			sresult.Round = Cycle.Round.Index;
			sresult.Cycle = Cycle.Index;
			sresult.Match = _index;
			sresult.Outcome = (int) _outcome;
			sresult.PartsResult = _partsResult;
			sresult.TeamAScore = _teamAScore;
			sresult.TeamBScore = _teamBScore;
			sresult.TeamsResult = group.GetTeamResult();

			SportServices.ChampionshipService cs = new SportServices.ChampionshipService();
			cs.CookieContainer = Sport.Core.Session.Cookies;
			if (!cs.SetMatchResult(sresult))
			{
				_teamAScore = tas;
				_teamBScore = tbs;
				_partsResult = pr;
				_outcome = moc;
				OnScoreChange();
				return false;
			}

			_gameResult = null;

			return true;
		}
		
		private SportServices.ChampionshipService _champService=null;
		public bool SetRefereeCount(int count)
		{
			if (_champService == null)
				_champService = new SportServices.ChampionshipService();
			Sport.Championships.Cycle cycle=this.Cycle;
			Sport.Championships.Round round=cycle.Round;
			Sport.Championships.Group group=round.Group;
			Sport.Championships.Phase phase=group.Phase;
			Sport.Championships.Championship championship=phase.Championship;
			Sport.Entities.ChampionshipCategory category=championship.ChampionshipCategory;
			if (_champService.SetRefereeCount(category.Id, phase.Index, group.Index, 
				round.Index, cycle.Index, this.Index, count))
			{
				_refereeCount = count;
			}
			return false;
		}
		
		public string GetTeamAName()
		{
			MatchTeam mt = MatchTeamA;
			if (mt != null)
				return mt.Name;
            
			if (_relativeTeamA != 0)
			{
				if (_relativeTeamA < 0)
				{
					return "מפסידת משחק " + (-_relativeTeamA).ToString();
				}
				else
				{
					return "מנצחת משחק " + _relativeTeamA.ToString();
				}
			}
			
			if ((_customTeamA != null)&&(_customTeamA.Length > 0))
				return _customTeamA;
			
			return null;
		}

		public string GetTeamBName()
		{
			MatchTeam mt = MatchTeamB;
			if (mt != null)
				return mt.Name;
            
			if (_relativeTeamB != 0)
			{
				if (_relativeTeamB < 0)
				{
					return "מפסידת משחק " + (-_relativeTeamB).ToString();
				}
				else
				{
					return "מנצחת משחק " + _relativeTeamB.ToString();
				}
			}
			
			if ((_customTeamB != null)&&(_customTeamB.Length > 0))
				return _customTeamB;
			
			return null;
		}
		
		/// <summary>
		/// try to change cycle - if successful return true otherwise return false.
		/// </summary>
		public bool ChangeCycle(Sport.Championships.Cycle newCycle, bool blnInsertAtEnd)
		{
			//got anything?
			if (newCycle == null)
				return false;
			
			//remove the match from its current cycle:
			this.Cycle.Matches.RemoveAt(this.Index);
			
			//add the match to the new cycle:
			if (blnInsertAtEnd)
				newCycle.Matches.Add(this);
			else
				newCycle.Matches.Insert(0, this);
			
			//make it last or first:
			if (blnInsertAtEnd)
				this.Tournament = newCycle.Tournaments.Count - 1;
			else
				this.Tournament = -1;
			
			//done.
			return true;
		}
		
		/// <summary>
		/// change the match index to the new index.
		/// </summary>
		public void ChangeIndex(int newIndex, bool blnKeepDate)
		{
			//get all cycle matches:
			System.Collections.ArrayList matches=
				new System.Collections.ArrayList(this.Cycle.Matches);
			
			//get cycle:
			Sport.Championships.Cycle cycle=this.Cycle;
			
			//valid index?
			if ((newIndex == this.Index)||(newIndex < 0)||(newIndex >= matches.Count))
				return;
			
			//replace the matches:
			Sport.Championships.Match temp=(Sport.Championships.Match) matches[this.Index];
			matches[this.Index] = matches[newIndex];
			matches[newIndex] = temp;
			
			//replace date and time:
			if (blnKeepDate == true && (matches[this.Index] as Sport.Championships.Match).Time.Year > 1900 && 
				(matches[newIndex] as Sport.Championships.Match).Time.Year > 1900)
			{
				DateTime dtTemp = ((Sport.Championships.Match) matches[this.Index]).Time;
				(matches[this.Index] as Sport.Championships.Match).Time = (matches[newIndex] as Sport.Championships.Match).Time;
				(matches[newIndex] as Sport.Championships.Match).Time = dtTemp;
			}
			
			//remove all matches
			while (cycle.Matches.Count > 0)
				cycle.Matches.RemoveAt(cycle.Matches.Count-1);
			
			//add the matches
			foreach (Sport.Championships.Match match in matches)
				cycle.Matches.Add(match);
		}
		
		public void ChangeIndex(int newIndex)
		{
			ChangeIndex(newIndex, false);
		}
		
		public static int CountDifferentMatches(Match[] arr1, Match[] arr2)
		{
			int result=0;
			if (arr1.Length != arr2.Length)
				return 0;
			for (int i=0; i<arr1.Length; i++)
			{
				if (arr1[i].Index != arr2[i].Index)
					result++;
			}
			return result;
		}
	}

	public class MatchComparer : System.Collections.IComparer
	{
		#region IComparer Members

		public int Compare(object x, object y)
		{
			Sport.Championships.Match mx = (Sport.Championships.Match) x;
			Sport.Championships.Match my = (Sport.Championships.Match) y;

			int d = 0;
			if (mx.Cycle == my.Cycle)
			{
				d = mx.Tournament.CompareTo(my.Tournament);
				if (d == 0)
					d = mx.Index.CompareTo(my.Index);
				/*
				if (d == 0)
					d = mx.Time.CompareTo(my.Time);
				if (d == 0)
					d = mx.Number.CompareTo(my.Number);
				*/
			}
			else if (mx.Cycle.Round == my.Cycle.Round)
			{
				d = mx.Cycle.Index.CompareTo(my.Cycle.Index);
			}
			else if (mx.Cycle.Round.Group == my.Cycle.Round.Group)
			{
				d = mx.Cycle.Round.Index.CompareTo(my.Cycle.Round.Index);
			}
			else
			{
				Group gx = mx.Cycle.Round.Group;
				Group gy = my.Cycle.Round.Group;
				if (gx.Phase == gy.Phase)
				{
					d = gx.Index.CompareTo(my.Index);
				}
				else if (gx.Phase.Championship == gy.Phase.Championship)
				{
					d = gx.Phase.Index.CompareTo(gy.Phase.Index);
				}
				else
				{
					d = gx.Phase.Championship.Name.CompareTo(gy.Phase.Championship.Name);
				}
			}

			return d;
		}

		#endregion
	}
}
