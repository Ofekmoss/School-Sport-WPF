using System;
using System.Collections;

namespace Sport.Championships
{
	#region Group Class

	public class Group : Sport.Common.GeneralCollection.CollectionItem
	{
		#region Properties

		private string _name;
		public string Name
		{
			get { return _name; }
			set 
			{ 
				if (!Editable)
					throw new ChampionshipException("Not in edit - cannot be set");

				_name = value; 
			}
		}

		internal int _index;
		public int Index
		{
			get { return _index; }
		}

		private bool _editing;
		public bool Editing
		{
			get { return _editing; }
		}

		public bool Editable
		{
			get
			{
				return Phase == null || Phase.Editable || _editing;
			}
		}

		#endregion

		#region Rules

		internal virtual void OnRulesetChange()
		{
			CalculateTeamsScore();
		}

		#endregion

		#region CollectionItem Members

		public override void OnOwnerChange(object oo, object no)
		{
			if (no == null)
				_index = -1;
		}

		public Phase Phase
		{
			get { return ((Phase) Owner); }
		}

		#endregion

		#region Teams

		#region TeamCollection

		public class TeamCollection : Sport.Common.GeneralCollection
		{
			public TeamCollection(Group group)
				: base(group)
			{
			}

			protected override void SetItem(int index, object value)
			{
				if (!Group.Editable)
					throw new ChampionshipException("Not in structure edit - cannot change teams");

				base.SetItem (index, value);
			}

			protected override void InsertItem(int index, object value)
			{
				if (!Group.Editable)
					throw new ChampionshipException("Not in structure edit - cannot change teams");

				base.InsertItem (index, value);
			}

			protected override void RemoveItem(int index)
			{
				if (!Group.Editable)
					throw new ChampionshipException("Not in structure edit - cannot change teams");

				base.RemoveItem (index);
			}

			public Group Group
			{
				get { return ((Group) Owner); }
			}

			public Team this[int index]
			{
				get { return (Team) GetItem(index); }
				set { SetItem(index, value); }
			}

			public void Insert(int index, Team value)
			{
				InsertItem(index, value);
			}

			public void Remove(Team value)
			{
				RemoveItem(value);
			}

			public bool Contains(Team value)
			{
				return base.Contains(value);
			}

			public int IndexOf(Team value)
			{
				return base.IndexOf(value);
			}

			public int Add(Team value)
			{
				return AddItem(value);
			}
		}

		#endregion

		protected virtual TeamCollection CreateTeamsCollection()
		{
			return new TeamCollection(this);
		}

		private TeamCollection _teams;
		public TeamCollection Teams
		{
			get { return _teams; }
		}

		private void TeamsChanged(object sender, Sport.Common.CollectionEventArgs e)
		{
			if (e.New != null)
			{
				if (((Team) e.New).Position == -1)
					((Team) e.New)._position = e.Index;
			}

			for (int i = e.Index; i < _teams.Count; i++)
				_teams[i]._index = i;
		}

		protected virtual bool SetTeamsResult()
		{
			return false;
		}

		public bool ReplaceTeams(int teamA, int teamB)
		{
			int positionA = Teams[teamA].Position;
			int positionB = Teams[teamB].Position;

			Teams[teamA]._position = positionB;
			Teams[teamB]._position = positionA;

			if (!SetTeamsResult())
			{
				Teams[teamA]._position = positionA;
				Teams[teamB]._position = positionB;
				return false;
			}

			return true;
		}
		
		public event EventHandler TeamsScoreCalculated;

		internal void CalculateTeamsScore()
		{
			DoCalculateTeamsScore();

			if (TeamsScoreCalculated != null)
				TeamsScoreCalculated(this, EventArgs.Empty);
		}

		protected virtual void DoCalculateTeamsScore()
		{
		}

		#endregion

		#region Status

		public virtual bool HasResults()
		{
			return false;
		}

		public virtual void Reset()
		{
		}

		#endregion

		#region Constructors

		public Group(string name)
		{
			_index = -1;
			_name = name;
			_teams = CreateTeamsCollection();
			_teams.Changed += new Sport.Common.CollectionEventHandler(TeamsChanged);
		}

		#endregion

		#region Save

		internal virtual SportServices.Group Save()
		{
			return null;
		}

		#endregion

		#region Edit

		public void EditGroup()
		{
			if (_editing)
				throw new ChampionshipException("Group already in edit");

			if (Phase.Championship.Editing)
				throw new ChampionshipException("Cannot edit group - championship is in edit");

			_editing = true;
		}

		public bool SaveGroup()
		{
			if (_editing)
			{
				SportServices.Group sgroup = Save();

				SportServices.ChampionshipService cs = new SportServices.ChampionshipService();
				cs.CookieContainer = Sport.Core.Session.Cookies;

				Phase phase = Phase;
				Championship championship = phase.Championship;

				if (!cs.SaveGroup(championship.ChampionshipCategory.Id, phase.Index,
					_index, ref sgroup))
					return false;

				_editing = false;
			}

			return true;
		}

		public void CancelGroup()
		{
			if (!LoadGroup())
				throw new ChampionshipException("Failed to load group");

			_editing = false;
		}

		protected virtual void LoadGroup(SportServices.Group sgroup)
		{
			_name = sgroup.Name;
		}

		protected bool LoadGroup()
		{
			_editing = true;

			SportServices.ChampionshipService cs = new SportServices.ChampionshipService();
			cs.CookieContainer = Sport.Core.Session.Cookies;
			Phase phase = Phase;
			Championship championship = phase.Championship;
			SportServices.Group sgroup = cs.LoadGroup(championship.ChampionshipCategory.Id, phase.Index, _index);

			if (sgroup == null)
				return false;

			LoadGroup(sgroup);

			_editing = false;

			return true;
		}

		#endregion
	}

	#endregion

	#region MatchGroup Class

	#region MatchResult Class
	
	public class MatchResult
	{
		private int _matchNumber;
		public int MatchNumber
		{
			get { return _matchNumber; }
		}

		private MatchOutcome _outcome;
		public MatchOutcome Outcome
		{
			get { return _outcome; }
		}

		private double _teamAScore;
		public double TeamAScore
		{
			get { return _teamAScore; }
		}

		private double _teamBScore;
		public double TeamBScore
		{
			get { return _teamBScore; }
		}

		private GameResult _gameResult;
		public GameResult GameResult
		{
			get { return _gameResult; }
		}

		public MatchResult(int matchNumber, MatchOutcome outcome,
			double teamAScore, double teamBScore, GameResult gameResult)
		{
			_matchNumber = matchNumber;
			_outcome = outcome;
			_teamAScore = teamAScore;
			_teamBScore = teamBScore;
			if (_outcome != MatchOutcome.None)
			{
				if (_teamAScore < 0)
					_teamAScore = 0;
				if (_teamBScore < 0)
					_teamBScore = 0;
			}
			_gameResult = gameResult;
		}

		public MatchResult(int matchNumber, MatchOutcome outcome,
			double teamAScore, double teamBScore)
			: this(matchNumber, outcome, teamAScore, teamBScore, null)
		{
		}
			
		public MatchResult(MatchResult result)
			: this(result.MatchNumber, result.Outcome, result.TeamAScore, 
			result.TeamBScore, result.GameResult == null ? null : (GameResult) result.GameResult.Clone())
		{
		}

		public MatchResult(int matchNumber, double teamAScore, double teamBScore)
		{
			_matchNumber = matchNumber;
			_teamAScore = teamAScore;
			_teamBScore = teamBScore;
			if (_teamAScore > _teamBScore)
			{
				_outcome = MatchOutcome.WinA;
			}
			else if (_teamAScore == _teamBScore)
			{
				_outcome = MatchOutcome.Tie;
			}
			else
			{
				_outcome = MatchOutcome.WinB;
			}

			if (_teamAScore < 0)
				_teamAScore = 0;
			if (_teamBScore < 0)
				_teamBScore = 0;

			_gameResult = null;
		}
	}

	#endregion

	/// <summary>
	/// The MatchGroup class holds the definition of a group in one
	/// of the championship phases in a matches championship.
	/// Each group have a name and belong to a single phase.
	/// Each group containts an intial teams list and rounds
	/// collection that contains the matches of the group.
	/// </summary>
	public class MatchGroup : Group
	{
		public override bool HasResults()
		{
			foreach (Round round in _rounds)
			{
				if (round.HasResults())
					return true;
			}

			return false;
		}


		/// <summary>
		/// returns the first round in the group having the given name.
		/// </summary>
		public Round GetFirstRound(string roundName)
		{
			Round result=null;
			foreach (Round round in this.Rounds)
			{
				if (round.Name == roundName)
				{
					result = round;
					break;
				}
			}
			return result;
		}

		public new MatchPhase Phase
		{
			get { return ((MatchPhase) base.Phase); }
		}

		#region Teams

		#region MatchTeamCollection

		public class MatchTeamCollection : Group.TeamCollection
		{
			public MatchTeamCollection(MatchGroup group)
				: base(group)
			{
			}

			public new MatchGroup Group
			{
				get { return ((MatchGroup) base.Group); }
			}

			public new MatchTeam this[int index]
			{
				get { return (MatchTeam) GetItem(index); }
				set { SetItem(index, value); }
			}

			public void Insert(int index, MatchTeam value)
			{
				InsertItem(index, value);
			}

			public void Remove(MatchTeam value)
			{
				RemoveItem(value);
			}

			public bool Contains(MatchTeam value)
			{
				return base.Contains(value);
			}

			public int IndexOf(MatchTeam value)
			{
				return base.IndexOf(value);
			}

			public int Add(MatchTeam value)
			{
				return AddItem(value);
			}
		}

		#endregion

		protected override TeamCollection CreateTeamsCollection()
		{
			TeamCollection teams = new MatchTeamCollection(this);
			teams.Changed += new Sport.Common.CollectionEventHandler(TeamsChanged);
			return teams;
		}

		public new MatchTeamCollection Teams
		{
			get { return (MatchTeamCollection) base.Teams; }
		}

		#endregion

		#region Rounds

		#region RoundCollection

		public class RoundCollection : Sport.Common.GeneralCollection
		{
			public RoundCollection(MatchGroup group)
				: base(group)
			{
			}

			internal void ClearRoundNumbers(Round round)
			{
				foreach (Cycle cycle in round.Cycles)
				{
					round.Cycles.ClearNumbers(cycle);
				}
			}

			internal void SetRoundNumbers(Round round)
			{
				foreach (Cycle cycle in round.Cycles)
				{
					round.Cycles.SetNumbers(cycle);
				}
			}

			protected override void SetItem(int index, object value)
			{
				if (!((Group) Owner).Editable)
					throw new ChampionshipException("Not in edit - cannot change rounds");

				ClearRoundNumbers(this[index]);
				base.SetItem (index, value);
				SetRoundNumbers((Round) value);
			}

			protected override void InsertItem(int index, object value)
			{
				if (!((Group) Owner).Editable)
					throw new ChampionshipException("Not in edit - cannot change rounds");

				base.InsertItem (index, value);
				SetRoundNumbers((Round) value);
			}

			protected override void RemoveItem(int index)
			{
				if (!((Group) Owner).Editable)
					throw new ChampionshipException("Not in edit - cannot change rounds");

				ClearRoundNumbers(this[index]);
				base.RemoveItem (index);
			}

			public Round this[int index]
			{
				get { return (Round) GetItem(index); }
				set { SetItem(index, value); }
			}

			public void Insert(int index, Round value)
			{
				InsertItem(index, value);
			}

			public void Remove(Round value)
			{
				RemoveItem(value);
			}

			public bool Contains(Round value)
			{
				return base.Contains(value);
			}

			public int IndexOf(Round value)
			{
				return base.IndexOf(value);
			}

			public int Add(Round value)
			{
				return AddItem(value);
			}
		}

		#endregion

		private RoundCollection _rounds;
		public RoundCollection Rounds
		{
			get { return _rounds; }
		}

		private void RoundsChanged(object sender, Sport.Common.CollectionEventArgs e)
		{
			for (int i = e.Index; i < _rounds.Count; i++)
				_rounds[i]._index = i;
		}

		#endregion

		private System.Collections.Hashtable matchNumbers;
		internal void SetMatchNumber(Match match, int on, int nn)
		{
			if (on != -1)
			{
				matchNumbers.Remove(on);
			}

			if (nn != -1)
			{
				matchNumbers[nn] = match;
			}
		}

		public Match GetMatchByNumber(int number)
		{
			return matchNumbers[number] as Match;
		}

		private System.Collections.Hashtable tournamentNumbers;
		internal void SetTournamentNumber(Tournament tournament, int on, int nn)
		{
			if (on != -1)
			{
				tournamentNumbers.Remove(on);
			}

			if (nn != -1)
			{
				tournamentNumbers[nn] = tournament;
			}
		}

		public Tournament GetTournamentByNumber(int number)
		{
			return tournamentNumbers[number] as Tournament;
		}

		public MatchGroup(string name)
			: base(name)
		{
			_rounds = new RoundCollection(this);
			_rounds.Changed += new Sport.Common.CollectionEventHandler(RoundsChanged);
			matchNumbers = new System.Collections.Hashtable();
			tournamentNumbers = new System.Collections.Hashtable();
		}

		internal MatchGroup(SportServices.Group group)
			: this(group.Name)
		{
			foreach (SportServices.Team steam in group.Teams)
			{
				MatchTeam matchTeam=null;
				matchTeam = new MatchTeam(steam);
				if (ValidateTeam(matchTeam))
				{
					Teams.Add(matchTeam);
				}
				else
				{
					System.Diagnostics.Debug.WriteLine("invalid match team found. team id: "+steam.TeamId);
				}
			}

			foreach (SportServices.Round sround in group.Rounds)
			{
				_rounds.Add(new Round(sround));
			}
		}

		private bool ValidateTeam(MatchTeam team)
		{
			//maybe no team at all...
			if (team == null)
				return false;
			
			//team exist, check actual team entity - if exists we're all done.
			if (team.TeamEntity != null)
				return true;
			
			//team entity does not exist, maybe from previous phase?
			if ((team.PreviousGroup >= 0)&&(team.PreviousPosition >= 0))
				return true;

			//invalid team.
			return false;

		}

		public bool SetResults(MatchResult[] results)
		{
			if (Phase.Status == Status.Planned)
				throw new ChampionshipException("Phase not started, cannot insert results");
            
			MatchResult[] oldResults = new MatchResult[results.Length];
			SportServices.MatchGroupMatchResult[] sresults = new Sport.Championships.SportServices.MatchGroupMatchResult[results.Length];
			for (int n = 0; n < results.Length; n++)
			{
				MatchResult result = results[n];
				oldResults[n] = new MatchResult(result);
				Match match = GetMatchByNumber(result.MatchNumber);
				match._teamAScore = result.TeamAScore;
				match._teamBScore = result.TeamBScore;
				match._outcome = result.Outcome;
				match._partsResult = result.GameResult == null ? null : result.GameResult.ToString();

				sresults[n] = match.GetMatchResult();
			}

			CalculateTeamsScore();

			SportServices.MatchGroupResult sresult = new SportServices.MatchGroupResult();
			sresult.ChampionshipCategoryId = Phase.Championship.ChampionshipCategory.Id;
			sresult.Phase = Phase.Index;
			sresult.Group = Index;
			sresult.TeamsResult = GetTeamResult();
			sresult.MatchResults = sresults;

			SportServices.ChampionshipService cs = new SportServices.ChampionshipService();
			cs.CookieContainer = Sport.Core.Session.Cookies;
			if (!cs.SetMatchGroupResult(sresult))
			{
				for (int n = 0; n < oldResults.Length; n++)
				{
					MatchResult result = oldResults[n];
					Match match = GetMatchByNumber(result.MatchNumber);
					match._teamAScore = result.TeamAScore;
					match._teamBScore = result.TeamBScore;
					match._outcome = result.Outcome;
					match._partsResult = result.GameResult.ToString();
				}

				CalculateTeamsScore();

				return false;
			}
				
			return true;
		}

		internal override SportServices.Group Save()
		{
			SportServices.Group sgroup = new SportServices.Group();
			sgroup.Name = Name;

			sgroup.Teams = new SportServices.Team[Teams.Count];

			for (int n = 0; n < Teams.Count; n++)
			{
				sgroup.Teams[n] = Teams[n].Save();
			}
			
			sgroup.Rounds = new SportServices.Round[_rounds.Count];

			for (int n = 0; n < _rounds.Count; n++)
			{
				sgroup.Rounds[n] = _rounds[n].Save();
			}

			return sgroup;
		}

		protected override void LoadGroup(SportServices.Group sgroup)
		{
			base.LoadGroup (sgroup);

			Teams.Clear();

			foreach (SportServices.Team steam in sgroup.Teams)
			{
				MatchTeam matchTeam=null;
				matchTeam = new MatchTeam(steam);
				if (ValidateTeam(matchTeam))
				{
					Teams.Add(matchTeam);
				}
				else
				{
					System.Diagnostics.Debug.WriteLine("invalid match team found. team id: "+steam.TeamId);
				}
			}

			_rounds.Clear();

			foreach (SportServices.Round sround in sgroup.Rounds)
			{
				_rounds.Add(new Round(sround));
			}
		}

		public override void Reset()
		{
			foreach (Round round in _rounds)
				round.Reset();
			if (Phase.Index > 0)
			{
				foreach (MatchTeam team in Teams)
				{
					team.Reset();
				}
			}
		}

		public override string ToString()
		{
			return Phase.Name + " - " + Name;
		}

		private void TeamsChanged(object sender, Sport.Common.CollectionEventArgs e)
		{
			// Deleting all team matches
			if (e.EventType == Sport.Common.CollectionEventType.Remove)
			{
				foreach (Round round in _rounds)
				{
					foreach (Cycle cycle in round.Cycles)
					{
						int n = 0;
						while (n < cycle.Matches.Count)
						{
							if (cycle.Matches[n].TeamA == e.Index ||
								cycle.Matches[n].TeamB == e.Index)
							{
								cycle.Matches.RemoveAt(n);
							}
							else
							{
								if (cycle.Matches[n].TeamA > e.Index)
									cycle.Matches[n].TeamA--;
								if (cycle.Matches[n].TeamB > e.Index)
									cycle.Matches[n].TeamB--;
								n++;
							}
						}
					}
				}
			}
			else if (e.EventType == Sport.Common.CollectionEventType.Insert)
			{
				foreach (Round round in _rounds)
				{
					foreach (Cycle cycle in round.Cycles)
					{
						foreach (Match match in cycle.Matches)
						{
							if (match.TeamA >= e.Index)
								match.TeamA++;
							if (match.TeamB >= e.Index)
								match.TeamB++;
						}
					}
				}
			}
		}

		private void RankTeams(MatchTeam[] teams, object[] ranks, 
			int first, int last, Sport.Rulesets.Rules.TeamRanking teamRanking, int level)
		{
			TeamRanker ranker;
			if (level == -1)
			{
				ranker = new ScoreTeamRanker(this, false);
			}
			else 
			{
				if (teamRanking == null)
					return ;

				ranker = TeamRanker.CreateRanker(this, teamRanking.RankingMethods[level]);
			}

			// Trying to rank teams according to current ranker
			if (ranker.RankTeams(teams, ranks, first, last))
			{
				// Sorting teams by ranking
				Array.Sort(ranks, teams, first, last - first + 1);

				// Searching for equal ranks
				int lastFirst = first;
				IComparable lastObject = ranks[first] as IComparable;

				for (int n = first + 1; n <= last; n++)
				{
					IComparable current = ranks[n] as IComparable;
					bool equal = lastObject == null ? current == null : lastObject.CompareTo(current) == 0;

					// If current object unequal to last set object
					if (!equal)
					{
						// If more than one ranks are equal
						if (lastFirst < n - 1)
						{
							// Rerank equals
							RankTeams(teams, ranks, lastFirst, n - 1, teamRanking, 0);
						}
						//current = lastObject;
						lastObject = current;
						lastFirst = n;
					}
				}

				// If more than one ranks are equal
				if (lastFirst < last)
				{
					// If all ranks are equal...
					if (lastFirst == first)
					{
						// Rerank using next level rank
						if (teamRanking != null && teamRanking.RankingMethods.Count - 1 > level)
						{
							RankTeams(teams, ranks, first, last, teamRanking, level + 1);
						}
						else
						{
							ranker = new PositionTeamRanker(this, true);
							ranker.RankTeams(teams, ranks, first, last);
							Array.Sort(ranks, teams, first, last - first + 1);
						}
					}
					else
					{
						// otherwise, rerank equals
						RankTeams(teams, ranks, lastFirst, last, teamRanking, 0);
					}
				}
			}
			else
			{
				// if rank failed rerank using next level rank
				if (teamRanking != null && teamRanking.RankingMethods.Count - 1 > level)
				{
					RankTeams(teams, ranks, first, last, teamRanking, level + 1);
				}
				else
				{
					ranker = new PositionTeamRanker(this, true);
					ranker.RankTeams(teams, ranks, first, last);
					Array.Sort(ranks, teams, first, last - first + 1);
				}
			}
		}

		public SportServices.MatchTeamResult[] GetTeamResult()
		{
			SportServices.MatchTeamResult[] teamResult = new SportServices.MatchTeamResult[Teams.Count];
			for (int n = 0; n < Teams.Count; n++)
			{
				teamResult[n] = new SportServices.MatchTeamResult();
				teamResult[n].Score = Teams[n].Score;
				teamResult[n].Position = Teams[n].Position;
				teamResult[n].Games = Teams[n].Games;
				teamResult[n].Points = Teams[n].Points;
				teamResult[n].PointsAgainst = Teams[n].PointsAgainst;
				teamResult[n].SmallPoints = Teams[n].SmallPoints;
				teamResult[n].SmallPointsAgainst = Teams[n].SmallPointsAgainst;
				teamResult[n].Wins = Teams[n].Wins;
				teamResult[n].Loses = Teams[n].Loses;
				teamResult[n].Ties = Teams[n].Ties;
				teamResult[n].TechnicalWins = Teams[n].TechnicalWins;
				teamResult[n].TechnicalLoses = Teams[n].TechnicalLoses;
			}

			return teamResult;
		}

		protected override bool SetTeamsResult()
		{
			SportServices.ChampionshipService cs = new SportServices.ChampionshipService();
			cs.CookieContainer = Sport.Core.Session.Cookies;
			return cs.SetMatchTeamsResult(Phase.Championship.ChampionshipCategory.Id,
				Phase.Index, _index, GetTeamResult());
		}

		protected override void DoCalculateTeamsScore()
		{
			for (int n = 0; n < Teams.Count; n++)
			{
				MatchTeam team = Teams[n] as MatchTeam;
				if (team != null)
				{
					team._score = 0;
					team._games = 0;
					team._points = 0;
					team._pointsAgainst = 0;
					team._smallPoints = 0;
					team._smallPointsAgainst = 0;
					team._wins = 0;
					team._loses = 0;
					team._ties = 0;
					team._technicalWins = 0;
					team._technicalLoses = 0;
				}
			}

			Sport.Rulesets.Rules.GameScore gs = Phase.Championship.ChampionshipCategory.GetRule(
				typeof(Sport.Rulesets.Rules.GameScore)) as Sport.Rulesets.Rules.GameScore;

			if (gs == null)
				return ;

			foreach (Round round in _rounds)
			{
				foreach (Cycle cycle in round.Cycles)
				{
					foreach (Match match in cycle.Matches)
					{
						MatchTeam teamA = match.MatchTeamA;
						MatchTeam teamB = match.MatchTeamB;
						if (teamA != null && teamB != null)
						{
							if (match.Outcome != Sport.Championships.MatchOutcome.None)
							{
								teamA._games++;
								teamB._games++;

								teamA._points += match.TeamAScore;
								teamB._points += match.TeamBScore;
								teamA._pointsAgainst += match.TeamBScore;
								teamB._pointsAgainst += match.TeamAScore;
								int[] arrSmallPoints=match.CalcSmallPoints();
								teamA._smallPoints += arrSmallPoints[0];
								teamB._smallPoints += arrSmallPoints[1];
								teamA._smallPointsAgainst += arrSmallPoints[1];
								teamB._smallPointsAgainst += arrSmallPoints[0];
								
								switch (match.Outcome)
								{
									case (Sport.Championships.MatchOutcome.WinA):
										teamA._score += gs.Win;
										teamB._score += gs.Lose;
										teamA._wins++;
										teamB._loses++;
										break;
									case (Sport.Championships.MatchOutcome.WinB):
										teamB._score += gs.Win;
										teamA._score += gs.Lose;
										teamB._wins++;
										teamA._loses++;
										break;
									case (Sport.Championships.MatchOutcome.Tie):
										teamA._score += gs.Tie;
										teamB._score += gs.Tie;
										teamA._ties++;
										teamB._ties++;
										break;
									case (Sport.Championships.MatchOutcome.TechnicalA):
										teamA._score += gs.TechnicalWin;
										teamB._score += gs.TechnicalLose;
										teamA._technicalWins++;
										teamB._technicalLoses++;
										break;
									case (Sport.Championships.MatchOutcome.TechnicalB):
										teamB._score += gs.TechnicalWin;
										teamA._score += gs.TechnicalLose;
										teamB._technicalWins++;
										teamA._technicalLoses++;
										break;
								}
							}
						}
					}
				}
			}

			MatchTeam[] teams = new MatchTeam[Teams.Count];
			Teams.CopyTo(teams, 0);
			object[] ranks = new object[Teams.Count];

			Sport.Rulesets.Rules.TeamRanking teamRanking = Phase.Championship.ChampionshipCategory.GetRule(
				typeof(Sport.Rulesets.Rules.TeamRanking)) as Sport.Rulesets.Rules.TeamRanking;

			int level=-1;
			if (teamRanking != null)
			{
				if (teamRanking.RankingMethods.Count > 0)
					level = 0;
			}
			RankTeams(teams, ranks, 0, teams.Length - 1, teamRanking, level);

			for (int n = 0; n < teams.Length; n++)
			{
				Teams[teams[n].Index]._position = teams.Length - n - 1;
			}
		}

		public int GetMaxTournamentNumber()
		{
			int number = 0;

			foreach (Round round in _rounds)
			{
				foreach (Cycle cycle in round.Cycles)
				{
					foreach (Tournament tournament in cycle.Tournaments)
					{
						if (tournament.Number > number)
							number = tournament.Number;
					}
				}
			}

			return number;
		}
	}

	#endregion

	#region CompetitionGroup Class

	#region CompetitionPlayer Class

	public class CompetitionPlayer
	{
		private CompetitionGroup _group;
		public CompetitionGroup Group
		{
			get { return _group; }
		}

		private int _team;
		public int Team
		{
			get { return _team; }
		}

		private int _number;
		public int Number
		{
			get { return _number; }
		}

		private Sport.Entities.Player _playerEntity;
		public Sport.Entities.Player PlayerEntity
		{
			get { return _playerEntity; }
		}

		public CompetitionTeam CompetitionTeam
		{
			get { return _group.Teams[_team]; }
		}


		internal int _competitonCount;
		public int CompetitionCount
		{
			get { return _competitonCount; }
		}


		public string Name
		{
			get
			{
				if (_playerEntity != null)
					return _playerEntity.Name;

				return CompetitionTeam.Name + " - " + _number.ToString();
			}
		}


		private CompetitionPlayer(CompetitionGroup group, int number, int team)
		{
			_group = group;
			_number = number;
			_team = team;
			_playerEntity = CompetitionTeam.TeamEntity.GetPlayerByNumber(_number);
			_competitonCount = 0;
		}

		public static CompetitionPlayer FromPlayerNumber(CompetitionGroup group, int number)
		{
			int team = group.GetPlayerTeam(number);

			if (team == -1)
				return null;

			return new CompetitionPlayer(group, number, team);
		}
	}

	#endregion

	public class CompetitionGroup : Group
	{
		public override bool HasResults()
		{
			foreach (Competition competition in _competitions)
			{
				if (competition.HasResults())
					return true;
			}

			return false;
		}

		public new CompetitionPhase Phase
		{
			get { return ((CompetitionPhase) base.Phase); }
		}

		#region Rules

		internal override void OnRulesetChange()
		{
			foreach (Competition competition in _competitions)
				competition.OnRulesetChange();

			base.OnRulesetChange ();
		}


		#endregion

		#region Teams

		#region CompetitionTeamCollection

		public class CompetitionTeamCollection : Group.TeamCollection
		{
			public CompetitionTeamCollection(CompetitionGroup group)
				: base(group)
			{
			}

			public new CompetitionGroup Group
			{
				get { return ((CompetitionGroup) base.Group); }
			}

			public new CompetitionTeam this[int index]
			{
				get { return (CompetitionTeam) GetItem(index); }
				set { SetItem(index, value); }
			}

			public void Insert(int index, CompetitionTeam value)
			{
				InsertItem(index, value);
			}

			public void Remove(CompetitionTeam value)
			{
				RemoveItem(value);
			}

			public bool Contains(CompetitionTeam value)
			{
				return base.Contains(value);
			}

			public int IndexOf(CompetitionTeam value)
			{
				return base.IndexOf(value);
			}

			public int Add(CompetitionTeam value)
			{
				return AddItem(value);
			}

			public void ResetPositions()
			{
				//sort by score.
				ArrayList arrTeams=new ArrayList(this);
				IComparer comparer=new ScoreComparer();
				arrTeams.Sort(comparer);
				for (int pos=0; pos<arrTeams.Count; pos++)
					(arrTeams[pos] as CompetitionTeam)._position = pos;
			}

			private class ScoreComparer : IComparer
			{
				//sort by score
				public int Compare(Object o1, Object o2)
				{
					CompetitionTeam team1=(CompetitionTeam) o1;
					CompetitionTeam team2=(CompetitionTeam) o2;
					return -1*(team1.Score.CompareTo(team2.Score));
				}
			} //end class ComboValueComparer
		}

		#endregion

		protected override TeamCollection CreateTeamsCollection()
		{
			TeamCollection teams = new CompetitionTeamCollection(this);
			teams.Changed += new Sport.Common.CollectionEventHandler(TeamsChanged);
			return teams;
		}

		public new CompetitionTeamCollection Teams
		{
			get { return (CompetitionTeamCollection) base.Teams; }
		}

		public int GetPlayerTeam(int number)
		{
			Sport.Entities.Team te;
			for (int t = 0; t < Teams.Count; t++)
			{
				te = Teams[t].TeamEntity;
				if (te.PlayerNumberFrom <= number &&
					te.PlayerNumberTo >= number)
				{
					return t;
				}
			}

			return -1;
		}

/*		public bool GetTeamPlayer(int number, 
			out int team, out Sport.Entities.Player player)
		{
			Sport.Entities.Team te;
			for (int t = 0; t < Teams.Count; t++)
			{
				te = Teams[t].TeamEntity;
				if (te.PlayerNumberFrom <= number &&
					te.PlayerNumberTo >= number)
				{
					team = t;
					player = te.GetPlayerByNumber(number);

					return true;
				}
			}

			team = -1;
			player = null;

			return false;
		}*/

		#endregion

		#region PlayerCollection

		public class PlayerCollection : ICollection
		{
			private CompetitionGroup	_group;
			private Hashtable			_players;

			public PlayerCollection(CompetitionGroup group)
			{
				_group = group;
				_players = new Hashtable();
			}

			public CompetitionPlayer this[int number]
			{
				get 
				{
					if (_players.ContainsKey(number))
						return (CompetitionPlayer) _players[number];

					CompetitionPlayer player = CompetitionPlayer.FromPlayerNumber(_group, number);
					if (player != null)
						_players[number] = player;

					return player;
				}
			}

			public bool Contains(int number)
			{
				return _players.ContainsKey(number);
			}

			#region ICollection Members

			public bool IsSynchronized
			{
				get
				{
					return false;
				}
			}

			public int Count
			{
				get
				{
					return _players.Count;
				}
			}

			public void CopyTo(Array array, int index)
			{
				_players.CopyTo(array, index);
			}

			public object SyncRoot
			{
				get
				{
					return _players.SyncRoot;
				}
			}

			#endregion

			#region IEnumerable Members

			public IEnumerator GetEnumerator()
			{
				return _players.Values.GetEnumerator();
			}

			#endregion
		}

		#endregion

		private PlayerCollection _players;
		public PlayerCollection Players
		{
			get { return _players; }
		}


		#region Competitions

		#region CompetitionCollection

		public class CompetitionCollection : Sport.Common.GeneralCollection
		{
			public CompetitionCollection(CompetitionGroup group)
				: base(group)
			{
			}

			protected override void SetItem(int index, object value)
			{
				if (!((Group) Owner).Editable)
					throw new ChampionshipException("Not in edit - cannot change competitions");

				base.SetItem (index, value);
			}

			protected override void InsertItem(int index, object value)
			{
				if (!((Group) Owner).Editable)
					throw new ChampionshipException("Not in edit - cannot change competitions");

				base.InsertItem (index, value);
			}

			protected override void RemoveItem(int index)
			{
				if (!((Group) Owner).Editable)
					throw new ChampionshipException("Not in edit - cannot change competitions");

				base.RemoveItem (index);
			}

			public Competition this[int index]
			{
				get { return (Competition) GetItem(index); }
				set { SetItem(index, value); }
			}

			public void Insert(int index, Competition value)
			{
				InsertItem(index, value);
			}

			public void Remove(Competition value)
			{
				RemoveItem(value);
			}

			public bool Contains(Competition value)
			{
				return base.Contains(value);
			}

			public int IndexOf(Competition value)
			{
				return base.IndexOf(value);
			}

			public int Add(Competition value)
			{
				return AddItem(value);
			}
		}

		#endregion

		private CompetitionCollection _competitions;
		public CompetitionCollection Competitions
		{
			get { return _competitions; }
		}

		private void CompetitionsChanged(object sender, Sport.Common.CollectionEventArgs e)
		{
			for (int i = e.Index; i < _competitions.Count; i++)
				_competitions[i]._index = i;
		}

		#endregion

		#region Constructors

		public CompetitionGroup(string name)
			: base(name)
		{
			_competitions = new CompetitionCollection(this);
			_competitions.Changed += new Sport.Common.CollectionEventHandler(CompetitionsChanged);

			_players = new PlayerCollection(this);
		}

		internal CompetitionGroup(SportServices.Group group)
			: this(group.Name)
		{
			foreach (SportServices.Team steam in group.Teams)
			{
				CompetitionTeam compTeam=null;
				compTeam = new CompetitionTeam(steam);
				if ((compTeam != null)&&(compTeam.TeamEntity != null))
				{
					Teams.Add(compTeam);
				}
			}

			foreach (SportServices.Competition scompetition in group.Competitions)
			{
				_competitions.Add(new Competition(scompetition));
			}

			// Setting competitors team
			foreach (Competition competition in _competitions)
			{
				foreach (Competitor competitor in competition.Competitors)
				{
					competitor._team = -1;
					Sport.Entities.Team team;
					for (int t = 0; t < Teams.Count && competitor._team == -1; t++)
					{
						team = Teams[t].TeamEntity;
						if (team.PlayerNumberFrom <= competitor.PlayerNumber &&
							team.PlayerNumberTo >= competitor.PlayerNumber)
						{
							competitor._team = t;
						}
					}
				}
			}
		}

		#endregion

		internal override SportServices.Group Save()
		{
			SportServices.Group sgroup = new SportServices.Group();
			sgroup.Name = Name;

			sgroup.Teams = new SportServices.Team[Teams.Count];

			for (int n = 0; n < Teams.Count; n++)
			{
				sgroup.Teams[n] = Teams[n].Save();
			}

			sgroup.Competitions = new SportServices.Competition[_competitions.Count];

			for (int n = 0; n < _competitions.Count; n++)
			{
				sgroup.Competitions[n] = _competitions[n].Save();
			}

			return sgroup;
		}

		protected override void LoadGroup(SportServices.Group sgroup)
		{
			base.LoadGroup (sgroup);

			Teams.Clear();

			foreach (SportServices.Team steam in sgroup.Teams)
			{
				CompetitionTeam compTeam=null;
				compTeam = new CompetitionTeam(steam);
				if ((compTeam != null)&&(compTeam.TeamEntity != null))
				{
					Teams.Add(compTeam);
				}
			}

			_competitions.Clear();

			foreach (SportServices.Competition scompetition in sgroup.Competitions)
			{
				_competitions.Add(new Competition(scompetition));
			}

			// Setting competitors team
			foreach (Competition competition in _competitions)
			{
				foreach (Competitor competitor in competition.Competitors)
				{
					competitor._team = -1;
					Sport.Entities.Team team;
					for (int t = 0; t < Teams.Count && competitor._team == -1; t++)
					{
						team = Teams[t].TeamEntity;
						if (team.PlayerNumberFrom <= competitor.PlayerNumber &&
							team.PlayerNumberTo >= competitor.PlayerNumber)
						{
							competitor._team = t;
						}
					}
				}
			}
		}

		public override void Reset()
		{
			foreach (Competition competition in _competitions)
				competition.Reset();

			if (Phase.Index > 0)
			{
				foreach (CompetitionTeam team in Teams)
				{
					team.Reset();
				}
			}
		}

		public override string ToString()
		{
			return Phase.Name + " - " + Name;
		}

		private void TeamsChanged(object sender, Sport.Common.CollectionEventArgs e)
		{
			// Resetting competitors team index
			if (e.EventType == Sport.Common.CollectionEventType.Remove)
			{
				// and removing removed team competitors
				foreach (Competition competition in _competitions)
				{
					int n = 0;
					while (n < competition.Competitors.Count)
					{
						if (competition.Competitors[n].Team == e.Index)
						{
							competition.Competitors.RemoveAt(n);
						}
						else
						{
							if (competition.Competitors[n].Team > e.Index)
								competition.Competitors[n]._team--;
							n++;
						}
					}
				}
			}
			else if (e.EventType == Sport.Common.CollectionEventType.Insert)
			{
				foreach (Competition competition in _competitions)
				{
					foreach (Competitor competitor in competition.Competitors)
					{
						if (competitor._team >= e.Index)
							competitor._team++;
					}
				}
			}
		}

		public SportServices.CompetitionTeamResult[] GetTeamResult()
		{
			SportServices.CompetitionTeamResult[] teamResult = new SportServices.CompetitionTeamResult[Teams.Count];
			for (int n = 0; n < Teams.Count; n++)
			{
				teamResult[n] = new SportServices.CompetitionTeamResult();
				teamResult[n].Score = (int) Teams[n].Score;
				teamResult[n].Position = Teams[n].Position;
			}

			return teamResult;
		}

		protected override bool SetTeamsResult()
		{
			SportServices.ChampionshipService cs = new SportServices.ChampionshipService();
			cs.CookieContainer = Sport.Core.Session.Cookies;
			return cs.SetCompetitionTeamsResult(Phase.Championship.ChampionshipCategory.Id,
				Phase.Index, _index, GetTeamResult());
		}

		private class ScoreCompetitorList
		{
			private Competitor[] competitors;
			public Competitor[] Competitors
			{
				get { return competitors; }
			}

			public ScoreCompetitorList(int size)
			{
				competitors = new Competitor[size];
			}

			// Checking if given competitor can be placed in list
			// returns the extracted competitor if was one or
			// the given competitor if he could not be placed
			public Competitor SetScore(Competitor competitor)
			{
				for (int n = 0; n < competitors.Length; n++)
				{
					if (competitors[n] == null) // score is empty
					{
						competitors[n] = competitor;
						return null; // no competitor removed
					}

					if (competitor.Score > competitors[n].Score)
					{
						Competitor result = competitors[competitors.Length - 1];
						for (int i = competitors.Length - 1; i > n; i--)
						{
							competitors[i] = competitors[i - 1];
						}
						competitors[n] = competitor;
						return result;
					}
				}

				return competitor;
			}
		}

		private class GroupScoreCalculator
		{
			private ScoreCompetitorList[,] _scoreLists;
			private Sport.Rulesets.Rules.TeamScoreCounters _teamScoreCounters;
			private int _defaultCounter;

			public GroupScoreCalculator(CompetitionGroup group)
			{
				_teamScoreCounters = group.Phase.Championship.TeamScoreCounters;
				_defaultCounter = group.Phase.Championship.DefaultCounter;

				_scoreLists = new ScoreCompetitorList[group.Teams.Count,
					_teamScoreCounters.Counters.Count];
				for (int t = 0; t < group.Teams.Count; t++)
				{
					for (int c = 0; c < _teamScoreCounters.Counters.Count; c++)
					{
						_scoreLists[t,c] = new ScoreCompetitorList(_teamScoreCounters.Counters[c].Results);
					}
				}
			}

			public void SetTeam(CompetitionTeam team)
			{
				team._score = 0;
				team._totalCounter = 0;
				for (int n = 0; n < _teamScoreCounters.Counters.Count; n++)
				{
					team.Counters[n] = 0;
					team.Scores[n] = 0;

					foreach (Competitor c in _scoreLists[team.Index, n].Competitors)
					{
						if (c != null)
						{
							team._totalCounter++;
							team.Counters[n]++;
							team.Scores[n] += c.Score;
							team._score += c.Score;

						}
					}
				}
			}

			public void SetScore(Competitor competitor)
			{
				int counter = competitor.Competition.SportFieldCounter;
				if (counter == -1)
					return ;
				Competitor c = _scoreLists[competitor._team, counter].SetScore(competitor);

				if (c == competitor && counter != competitor.Competition.SportFieldTypeCounter) // competitor was not set
				{
					counter = competitor.Competition.SportFieldTypeCounter;
					if (counter != -1)
					{
						// trying sport field type counter
						c = _scoreLists[competitor._team, counter].SetScore(competitor);
					}
				}

				if (c == competitor && counter != _defaultCounter)
				{
					c = _scoreLists[competitor._team, _defaultCounter].SetScore(competitor);
				}
				
				if (c != null && c != competitor) // A competitor was removed by new competitor
				{
					SetScore(c);
				}
			}
		}

		protected override void DoCalculateTeamsScore()
		{
			for (int n = 0; n < Teams.Count; n++)
			{
				CompetitionTeam team = Teams[n] as CompetitionTeam;
				if (team != null)
				{
					team._score = 0;
				}
			}

			// Reseting player information
			foreach (CompetitionPlayer player in _players)
			{
				player._competitonCount = 0;
			}

			Sport.Rulesets.Rules.TeamScoreCounters teamScoreCounters = Phase.Championship.TeamScoreCounters;

			if (teamScoreCounters == null)
			{
				// No counters - just add scores
				foreach (Team team in Teams)
				{
					team._score = 0;
				}

				foreach (Competition competition in _competitions)
				{
					foreach (Competitor competitor in competition.Competitors)
					{
						competitor.Player._competitonCount++;
						if (competitor.Score > 0)
							Teams[competitor._team]._score += competitor.Score;
					}
				}
			}
			else
			{
				// Having counters - add score by sport field counter
				GroupScoreCalculator calculator = new GroupScoreCalculator(this);

				foreach (Competition competition in _competitions)
				{
					foreach (Competitor competitor in competition.Competitors)
					{
						competitor.Player._competitonCount++;
						if (competitor.Score > 0)
							calculator.SetScore(competitor);
					}
				}

				for (int n = 0; n < Teams.Count; n++)
				{
					calculator.SetTeam(Teams[n]);
				}
			}

			CompetitionTeam[] teams = new CompetitionTeam[Teams.Count];
			Teams.CopyTo(teams, 0);
			double[] scores = new double[Teams.Count];
			for (int n = 0; n < scores.Length; n++)
				scores[n] = teams[n].Score;

			Array.Sort(scores, teams);

			for (int n = 0; n < teams.Length; n++)
			{
				Teams[teams[n].Index]._position = teams.Length - n - 1;
			}
		}
	}

	#endregion
}
