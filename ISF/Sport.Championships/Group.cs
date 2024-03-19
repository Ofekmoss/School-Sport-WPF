using System;
using System.Linq;
using System.Collections;
using Sport.Common;
using Sport.Rulesets.Rules;
using System.Xml;
using System.Collections.Generic;
using Sport.Core;

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
			get { return ((Phase)Owner); }
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

				base.SetItem(index, value);
			}

			protected override void InsertItem(int index, object value)
			{
				if (!Group.Editable)
					throw new ChampionshipException("Not in structure edit - cannot change teams");

				base.InsertItem(index, value);
			}

			protected override void RemoveItem(int index)
			{
				if (!Group.Editable)
					throw new ChampionshipException("Not in structure edit - cannot change teams");

				base.RemoveItem(index);
			}

			public Group Group
			{
				get { return ((Group)Owner); }
			}

			public Team this[int index]
			{
				get { return (Team)GetItem(index); }
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
				if (((Team)e.New).Position == -1)
					((Team)e.New)._position = e.Index;
			}

			for (int i = e.Index; i < _teams.Count; i++)
				_teams[i]._index = i;
		}

		protected virtual string SetTeamsResult()
		{
			return string.Empty;
		}

		public string ReplaceTeams(int teamA, int teamB)
		{
			int positionA = Teams[teamA].Position;
			int positionB = Teams[teamB].Position;

			Teams[teamA]._position = positionB;
			Teams[teamB]._position = positionA;

			string strError = SetTeamsResult();
			if (strError.Length > 0)
			{
				Teams[teamA]._position = positionA;
				Teams[teamB]._position = positionB;
				return strError;
			}

			return string.Empty;
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

		public string SaveGroup()
		{
			if (_editing)
			{
				SportServices.Group sgroup = Save();

				SportServices.ChampionshipService cs = new SportServices.ChampionshipService();
				cs.CookieContainer = Sport.Core.Session.Cookies;

				Phase phase = Phase;
				Championship championship = phase.Championship;

				string strError = cs.SaveGroup(championship.ChampionshipCategory.Id, phase.Index, _index, ref sgroup);
				if (strError.Length > 0)
					return strError;

				_editing = false;
			}

			return string.Empty;
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

		public Sport.Entities.OfflineTeam[] GetOfflineTeams()
		{
			Sport.Data.OfflineEntity[] arrAllTeams =
				Sport.Data.OfflineEntity.LoadAllEntities(
				typeof(Sport.Entities.OfflineTeam));
			ArrayList result = new ArrayList();
			if ((arrAllTeams != null) && (arrAllTeams.Length > 0))
			{
				int champCategoryID = this.Phase.Championship.CategoryID;
				foreach (Sport.Entities.OfflineTeam team in arrAllTeams)
				{
					if (team.ChampionshipCategory != champCategoryID)
						continue;
					if (team.Phase != this.Phase.Index)
						continue;
					if (team.Group != this.Index)
						continue;
					result.Add(team);
				}
			}
			return (Sport.Entities.OfflineTeam[])
				result.ToArray(typeof(Sport.Entities.OfflineTeam));
		}

		public override string ToString()
		{
			return this.Name;
		}
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
			result.TeamBScore, result.GameResult == null ? null : (GameResult)result.GameResult.Clone())
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
			Round result = null;
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
			get { return ((MatchPhase)base.Phase); }
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
				get { return ((MatchGroup)base.Group); }
			}

			public new MatchTeam this[int index]
			{
				get { return (MatchTeam)GetItem(index); }
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
			get { return (MatchTeamCollection)base.Teams; }
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
				if (!((Group)Owner).Editable)
					throw new ChampionshipException("Not in edit - cannot change rounds");

				ClearRoundNumbers(this[index]);
				base.SetItem(index, value);
				SetRoundNumbers((Round)value);
			}

			protected override void InsertItem(int index, object value)
			{
				if (!((Group)Owner).Editable)
					throw new ChampionshipException("Not in edit - cannot change rounds");

				base.InsertItem(index, value);
				SetRoundNumbers((Round)value);
			}

			protected override void RemoveItem(int index)
			{
				if (!((Group)Owner).Editable)
					throw new ChampionshipException("Not in edit - cannot change rounds");

				ClearRoundNumbers(this[index]);
				base.RemoveItem(index);
			}

			public Round this[int index]
			{
				get { return (Round)GetItem(index); }
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
				MatchTeam matchTeam = null;
				matchTeam = new MatchTeam(steam);
				if (ValidateTeam(matchTeam))
				{
					Teams.Add(matchTeam);
				}
				else
				{
					System.Diagnostics.Debug.WriteLine("invalid match team found. team id: " + steam.TeamId);
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
			if ((team.PreviousGroup >= 0) && (team.PreviousPosition >= 0))
				return true;

			//invalid team.
			return false;

		}

		public bool SetResults(MatchResult[] results)
		{
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
			base.LoadGroup(sgroup);

			Teams.Clear();

			foreach (SportServices.Team steam in sgroup.Teams)
			{
				MatchTeam matchTeam = null;
				matchTeam = new MatchTeam(steam);
				if (ValidateTeam(matchTeam))
				{
					Teams.Add(matchTeam);
				}
				else
				{
					System.Diagnostics.Debug.WriteLine("invalid match team found. team id: " + steam.TeamId);
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
			int first, int last, TeamRanking teamRanking, int level)
		{
			TeamRanker ranker;
			if (level == -1)
			{
				ranker = new ScoreTeamRanker(this, false);
			}
			else
			{
				if (teamRanking == null)
					return;

				ranker = TeamRanker.CreateRanker(this, teamRanking.RankingMethods[level]);
			}

			// Trying to rank teams according to current ranker
			if (ranker.RankTeams(teams, ranks, first, last))
			{
				// Sorting teams by ranking
				Array.Sort(ranks, teams, first, last - first + 1);

				// Searching for equal ranks
				int lastFirst = first;
				IComparable lastObject = (first >= 0 && first < ranks.Length) ? ranks[first] as IComparable : null;

				for (int n = first + 1; n <= last; n++)
				{
					IComparable current = (n >= 0 && n < ranks.Length) ? ranks[n] as IComparable : null;
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

		protected override string SetTeamsResult()
		{
			SportServices.ChampionshipService cs = new SportServices.ChampionshipService();
			cs.CookieContainer = Sport.Core.Session.Cookies;
			if (Phase == null)
				return "לא מוגדר שלב";
			if (Phase.Championship == null)
				return "לא מוגדרת אליפות";
			if (Phase.Championship.ChampionshipCategory == null)
				return "לא מוגדרת קטגורית אליפות";
			bool blnSuccess = cs.SetMatchTeamsResult(Phase.Championship.ChampionshipCategory.Id,
				Phase.Index, _index, GetTeamResult());
			return (blnSuccess) ? string.Empty : "קביעת תוצאות נכשלה";
		}

		private bool TryCalcScoreByParts(GameResult.PartResultCollection resultCollection, PartScore partScoreRule, ScoreByParts scoreByPartsRule,
			out int teamA_overrideScore, out int teamB_overrideScore, out int teamA_PartScore, out int teamB_PartScore)
		{
			teamA_overrideScore = 0;
			teamB_overrideScore = 0;
			teamA_PartScore = 0;
			teamB_PartScore = 0;
			
			if (resultCollection == null || partScoreRule == null)
				return false;
			
			for (int i = 0; i < resultCollection.Count; i++)
			{
				int currentPartResult_A = resultCollection[i][0];
				int currentPartResult_B = resultCollection[i][1];
				if (currentPartResult_A > currentPartResult_B)
				{
					//WinA
					teamA_PartScore += partScoreRule.Win;
					teamB_PartScore += partScoreRule.Lose;
				}
				else if (currentPartResult_B > currentPartResult_A)
				{
					//WinB
					teamA_PartScore += partScoreRule.Lose;
					teamB_PartScore += partScoreRule.Win;
				}
				else
				{
					//Tie
					teamA_PartScore += partScoreRule.Draw;
					teamB_PartScore += partScoreRule.Draw;
				}
			}

			if (scoreByPartsRule == null)
				return false;
			
			for (int i = 0; i < scoreByPartsRule.Values.Length; i++)
			{
				ScoreByParts.Value currentValue = scoreByPartsRule.Values[i];
				if (currentValue.TeamA_GameScore == teamA_PartScore && currentValue.TeamB_GameScore == teamB_PartScore)
				{
					teamA_overrideScore = currentValue.TeamA_PartScore;
					teamB_overrideScore = currentValue.TeamB_PartScore;
					break;
				}
				else if (currentValue.TeamA_GameScore == teamB_PartScore && currentValue.TeamB_GameScore == teamA_PartScore)
				{
					teamA_overrideScore = currentValue.TeamB_PartScore;
					teamB_overrideScore = currentValue.TeamA_PartScore;
					break;
				}
			}

			return teamA_overrideScore > 0 || teamB_overrideScore > 0;
		}

		private void CalculateMatchScore(Match match, GameScore gameScoreRule, PartScore partScoreRule, ScoreByParts scoreByPartsRule)
		{
			if (match.MatchTeamA == null || match.MatchTeamB == null || match.Outcome == MatchOutcome.None)
				return;

			int teamA_overrideScore, teamB_overrideScore, teamA_PartScore, teamB_PartScore;
			MatchTeam teamA = match.MatchTeamA;
			MatchTeam teamB = match.MatchTeamB;
			MatchOutcome outcome = match.Outcome;
			GameResult gameResult = (match.PartsResult != null && match.PartsResult.Length > 0) ? new GameResult(match.PartsResult) : null;
			GameResult.PartResultCollection resultCollection = (gameResult != null && gameResult.Games > 0 && gameResult[0].Count > 0) ? gameResult[0] : null;
			bool scoreByPartsOverride = TryCalcScoreByParts(resultCollection, partScoreRule, scoreByPartsRule,
				out teamA_overrideScore, out teamB_overrideScore, out teamA_PartScore, out teamB_PartScore);
			
			//games:
			teamA._games++;
			teamB._games++;

			//points:
			double pointsA = (partScoreRule != null && partScoreRule.Win > 0) ? teamA_PartScore : match.TeamAScore;
			double pointsB = (partScoreRule != null && partScoreRule.Win > 0) ? teamB_PartScore : match.TeamBScore;
			teamA._points += pointsA;
			teamB._points += pointsB;
			teamA._pointsAgainst += pointsB;
			teamB._pointsAgainst += pointsA;

			//small points:
			int[] arrSmallPoints = match.CalcSmallPoints();
			teamA._smallPoints += arrSmallPoints[0];
			teamB._smallPoints += arrSmallPoints[1];
			teamA._smallPointsAgainst += arrSmallPoints[1];
			teamB._smallPointsAgainst += arrSmallPoints[0];

			//score by parts?
			if (scoreByPartsOverride)
			{
				teamA._score += teamA_overrideScore;
				teamB._score += teamB_overrideScore;
			}
			
			//score, wins, loses, ties:
			switch (match.Outcome)
			{
				case (Sport.Championships.MatchOutcome.WinA):
					if (!scoreByPartsOverride)
					{
						teamA._score += gameScoreRule.Win;
						if (gameScoreRule.VolleyballShortGame)
							teamB._score += (((int)match.TeamBScore) == 0) ? 0 : 1;
						else
							teamB._score += gameScoreRule.Lose;
					}
					teamA._wins++;
					teamB._loses++;
					break;
				case (Sport.Championships.MatchOutcome.WinB):
					if (!scoreByPartsOverride)
					{
						teamB._score += gameScoreRule.Win;
						if (gameScoreRule.VolleyballShortGame)
							teamA._score += (((int)match.TeamAScore) == 0) ? 0 : 1;
						else
							teamA._score += gameScoreRule.Lose;
					}
					teamB._wins++;
					teamA._loses++;
					break;
				case (Sport.Championships.MatchOutcome.Tie):
					if (!scoreByPartsOverride)
					{
						teamA._score += gameScoreRule.Tie;
						teamB._score += gameScoreRule.Tie;
					}
					teamA._ties++;
					teamB._ties++;
					break;
				case (Sport.Championships.MatchOutcome.TechnicalA):
					teamA._score += gameScoreRule.TechnicalWin;
					teamB._score += gameScoreRule.TechnicalLose;
					teamA._technicalWins++;
					teamB._technicalLoses++;
					break;
				case (Sport.Championships.MatchOutcome.TechnicalB):
					teamB._score += gameScoreRule.TechnicalWin;
					teamA._score += gameScoreRule.TechnicalLose;
					teamB._technicalWins++;
					teamA._technicalLoses++;
					break;
			}
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
					team._sets = 0;
					team._setsAgainst = 0;
					team._wins = 0;
					team._loses = 0;
					team._ties = 0;
					team._technicalWins = 0;
					team._technicalLoses = 0;
				}
			}

			GameScore gs = null;
			PartScore partScoreRule = null;
			ScoreByParts scoreByPartsRule = null;
			if (Sport.Core.Session.Connected)
			{
				gs = Phase.Championship.ChampionshipCategory.GetRule(typeof(GameScore)) as GameScore;
				partScoreRule = Phase.Championship.ChampionshipCategory.GetRule(typeof(PartScore)) as PartScore;
				scoreByPartsRule = Phase.Championship.ChampionshipCategory.GetRule(typeof(ScoreByParts)) as ScoreByParts;
			}
			else
			{
				object rule = Sport.Rulesets.Ruleset.LoadOfflineRule(
					typeof(GameScoreRule),
					this.Phase.Championship.CategoryID, -1);
				if (rule != null)
					gs = (GameScore)rule;
			}

			if (gs == null)
				return;

			foreach (Round round in _rounds)
			{
				foreach (Cycle cycle in round.Cycles)
				{
					foreach (Match match in cycle.Matches)
						CalculateMatchScore(match, gs, partScoreRule, scoreByPartsRule);

					if (gs.ScoreEqualsPoints)
					{
						foreach (Match match in cycle.Matches)
						{
							if (match.Outcome == MatchOutcome.WinA || match.Outcome == MatchOutcome.WinB || match.Outcome == MatchOutcome.Tie)
							{
								MatchTeam teamA = match.MatchTeamA;
								MatchTeam teamB = match.MatchTeamB;
								if (teamA != null && teamB != null)
								{
									teamA._score = teamA.Points;
									teamB._score = teamB.Points;
								}
							}
						}
					}
				}
			}

			MatchTeam[] teams = new MatchTeam[Teams.Count];
			Teams.CopyTo(teams, 0);
			object[] ranks = new object[Teams.Count];

			TeamRanking teamRanking = null;
			if (Sport.Core.Session.Connected)
			{
				teamRanking = Phase.Championship.ChampionshipCategory.GetRule(
					typeof(TeamRanking)) as TeamRanking;
			}
			else
			{
				object rule = Sport.Rulesets.Ruleset.LoadOfflineRule(
					typeof(TeamRankingRule),
					this.Phase.Championship.CategoryID, -1);
				if (rule != null)
					teamRanking = (TeamRanking)rule;
			}

			int level = -1;
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
			set { _playerEntity = value; }
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

				if (CompetitionTeam != null)
					return CompetitionTeam.Name + " - " + _number.ToString();

				return "מספר " + _number.ToString();
			}
		}

		private static Hashtable tblOfflinePlayers = null;

		private CompetitionPlayer(CompetitionGroup group, int number, int team)
		{
			_group = group;
			_number = number;
			_team = team;
			_playerEntity = CompetitionTeam.TeamEntity.GetPlayerByNumber(_number);
			if (_playerEntity == null && !Sport.Core.Session.Connected)
			{
				if (tblOfflinePlayers == null)
					BuildOfflinePlayerNumbersTable();

				string key = CompetitionTeam.TeamEntity.Id + "," + number;
				if (tblOfflinePlayers[key] != null)
					_playerEntity = new Sport.Entities.Player((int)tblOfflinePlayers[key]);
			}
			//this.Group.Competitions[0].GetTeamCompetitors(CompetitionTeam.TeamEntity.Id, false);
			_competitonCount = 0;
		}

		private static void BuildOfflinePlayerNumbersTable()
		{
			tblOfflinePlayers = new Hashtable();
			string strXmlPath = Sport.Core.Session.GetSeasonCache(false) + System.IO.Path.DirectorySeparatorChar + "players.xml";
			if (System.IO.File.Exists(strXmlPath))
			{
				XmlDocument document = new XmlDocument();
				document.Load(strXmlPath);
				foreach (XmlNode node in document.DocumentElement.ChildNodes)
				{
					if (node.Name.StartsWith("Entity_"))
					{
						string[] arrTemp = node.Name.Split('_');
						if (arrTemp.Length > 1)
						{
							int playerID = Int32.Parse(arrTemp[1]);
							int teamID = Int32.Parse(node.ChildNodes[(int)Sport.Entities.Player.Fields.Team].InnerText);
							int playerNumber = Int32.Parse(node.ChildNodes[(int)Sport.Entities.Player.Fields.Number].InnerText);
							string key = teamID + "," + playerNumber;
							tblOfflinePlayers[key] = playerID;
						}
					}
				}
			}
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
		public Dictionary<int, Sport.Championships.Competition> MapCompetitions()
		{
			Dictionary<int, Sport.Championships.Competition> mapping = new Dictionary<int, Sport.Championships.Competition>();
			if (this.Competitions != null && this.Competitions.Count > 0)
			{
				this.Competitions.OfType<Sport.Championships.Competition>().ToList().ForEach(competition =>
				{
					int sportFieldId = competition.SportField.Id;
					if (!mapping.ContainsKey(sportFieldId))
						mapping.Add(sportFieldId, competition);
				});
			}
			return mapping;
		}

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
			get { return ((CompetitionPhase)base.Phase); }
		}

		public Sport.Entities.Facility GetGroupFacility()
		{
			if ((this.Competitions == null) || (this.Competitions.Count == 0))
				return null;
			Sport.Entities.Facility result = null;
			foreach (Sport.Championships.Competition competition in this.Competitions)
			{
				Sport.Entities.Facility facility = competition.Facility;
				if (facility != null)
				{
					if (result == null)
					{
						result = facility;
					}
					else
					{
						if (result.Id != facility.Id)
						{
							result = null;
							break;
						}
					}
				}
			}
			return result;
		}

		public DateTime GetGroupTime()
		{
			if ((this.Competitions == null) || (this.Competitions.Count == 0))
				return DateTime.MinValue;
			DateTime result = DateTime.MinValue;
			foreach (Sport.Championships.Competition competition in this.Competitions)
			{
				DateTime time = competition.Time;
				if (time.Year > 1900)
				{
					if (result.Year < 1900)
					{
						result = time;
					}
					else
					{
						if (!Sport.Common.Tools.IsSameDate(result, time))
						{
							result = DateTime.MinValue;
							break;
						}
					}
				}
			}
			return result;
		}

		#region Rules

		internal override void OnRulesetChange()
		{
			foreach (Competition competition in _competitions)
				competition.OnRulesetChange();

			base.OnRulesetChange();
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
				get { return ((CompetitionGroup)base.Group); }
			}

			public new CompetitionTeam this[int index]
			{
				get { return (CompetitionTeam)GetItem(index); }
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
				ArrayList arrTeams = new ArrayList(this);
				IComparer comparer = new ScoreComparer();
				arrTeams.Sort(comparer);
				for (int pos = 0; pos < arrTeams.Count; pos++)
					(arrTeams[pos] as CompetitionTeam)._position = pos;
			}

			private class ScoreComparer : IComparer
			{
				//sort by score
				public int Compare(Object o1, Object o2)
				{
					CompetitionTeam team1 = (CompetitionTeam)o1;
					CompetitionTeam team2 = (CompetitionTeam)o2;
					return -1 * (team1.Score.CompareTo(team2.Score));
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
			get { return (CompetitionTeamCollection)base.Teams; }
		}

		private static Hashtable tblTeamPlayerNumbers = new Hashtable();

		public static int[] GetPlayerNumbers(CompetitionTeamCollection teams, int teamID)
		{
			if (tblTeamPlayerNumbers[teamID] == null)
			{
				tblTeamPlayerNumbers[teamID] = new int[] { }; //team.GetPlayers();
				if (teams != null && teams.Count > 0)
				{
					if (Sport.Core.Session.Connected)
					{
						int[] teamIds = new int[teams.Count];
						for (int t = 0; t < teams.Count; t++)
						{
							Sport.Entities.Team team = teams[t].TeamEntity;
							teamIds[t] = team.Id;
						}

						DataServices.DataService service = new DataServices.DataService();
						DataServices.TeamPlayerNumbers[] arrData = service.GetPlayerNumbers(teamIds);
						if (arrData != null)
						{
							foreach (DataServices.TeamPlayerNumbers objNumbers in arrData)
							{
								tblTeamPlayerNumbers[objNumbers.TeamId] = objNumbers.PlayerNumbers;
							}
						}
					}
					else
					{
						foreach (CompetitionTeam compTeam in teams)
						{
							if (compTeam.TeamEntity != null)
							{
								Sport.Entities.Player[] arrPlayers = compTeam.TeamEntity.GetPlayers();
								if (arrPlayers != null)
								{
									int[] numbers = new int[arrPlayers.Length];
									for (int i = 0; i < arrPlayers.Length; i++)
									{
										numbers[i] = arrPlayers[i].Number;
									}
									tblTeamPlayerNumbers[compTeam.TeamEntity.Id] = numbers;
								}
							}
						}
					}
				}

			}
			return (int[])tblTeamPlayerNumbers[teamID];
		}

		public static void ResetPlayerNumbers(int teamId)
		{
			tblTeamPlayerNumbers[teamId] = null;
		}

		public int GetPlayerTeam(int number)
		{
			Sport.Entities.Team te;
			for (int t = 0; t < Teams.Count; t++)
			{
				te = Teams[t].TeamEntity;

				int[] numbers = CompetitionGroup.GetPlayerNumbers(this.Teams, te.Id);
				foreach (int n in numbers)
				{
					if (n == number)
					{
						return t;
					}
				}

				if (te.PlayerNumberFrom <= number && te.PlayerNumberTo >= number)
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
			private CompetitionGroup _group;
			private Hashtable _players;

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
						return (CompetitionPlayer)_players[number];

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
				if (!((Group)Owner).Editable)
					throw new ChampionshipException("Not in edit - cannot change competitions");

				base.SetItem(index, value);
			}

			protected override void InsertItem(int index, object value)
			{
				if (!((Group)Owner).Editable)
					throw new ChampionshipException("Not in edit - cannot change competitions");

				base.InsertItem(index, value);
			}

			protected override void RemoveItem(int index)
			{
				if (!((Group)Owner).Editable)
					throw new ChampionshipException("Not in edit - cannot change competitions");

				base.RemoveItem(index);
			}

			public Competition this[int index]
			{
				get { return (Competition)GetItem(index); }
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

		public int GetHeatCount()
		{
			int result = 0;
			foreach (Competition competition in this.Competitions)
			{
				if (competition.Heats != null)
					result += competition.Heats.Count;
			}
			return result;
		}

		public int GetCompetitorCompetitions(int shirtNumber)
		{
			int result = 0;
			foreach (Sport.Championships.Competition competition in this.Competitions)
			{
				foreach (Sport.Championships.Competitor competitor in competition.Competitors)
				{
					if (competitor.PlayerNumber == shirtNumber)
					{
						result++;
						break;
					}
				}
			}
			return result;
		}

		public int GetTeamCompetitionCompetitors(CompetitionTeam team, int index)
		{
			if (this.Competitions == null)
				return 0;
			if ((index < 0) || (index >= this.Competitions.Count))
				return 0;
			Sport.Championships.Competition competition = this.Competitions[index];
			if ((competition.Competitors == null) || (competition.Competitors.Count == 0))
				return 0;
			int result = 0;
			foreach (Sport.Championships.Competitor competitor in competition.Competitors)
			{
				CompetitionPlayer player = competitor.Player;
				if (player != null)
				{
					if (player.CompetitionTeam != null)
					{
						if (competitor.Player.CompetitionTeam.Id == team.Id)
							result++;
					}
				}
			}
			return result;
		}

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
				CompetitionTeam compTeam = null;
				compTeam = new CompetitionTeam(steam);
				if ((compTeam != null) && (compTeam.TeamEntity != null))
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

						int[] numbers = CompetitionGroup.GetPlayerNumbers(this.Teams, team.Id);
						foreach (int number in numbers)
						{
							if (number == competitor.PlayerNumber)
							{
								competitor._team = t;
								break;
							}
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
			base.LoadGroup(sgroup);

			Teams.Clear();

			foreach (SportServices.Team steam in sgroup.Teams)
			{
				CompetitionTeam compTeam = null;
				compTeam = new CompetitionTeam(steam);
				if ((compTeam != null) && (compTeam.TeamEntity != null))
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

						int[] numbers = CompetitionGroup.GetPlayerNumbers(this.Teams, team.Id);
						foreach (int number in numbers)
						{
							if (number == competitor.PlayerNumber)
							{
								competitor._team = t;
								break;
							}
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
				teamResult[n].Score = (int)Teams[n].Score;
				teamResult[n].Position = Teams[n].Position;
			}

			return teamResult;
		}

		protected override string SetTeamsResult()
		{
			SportServices.ChampionshipService cs = new SportServices.ChampionshipService();
			cs.CookieContainer = Sport.Core.Session.Cookies;
			return cs.SetCompetitionTeamsResult(Phase.Championship.ChampionshipCategory.Id,
				Phase.Index, _index, GetTeamResult());
		}

		public ScoringPlan GetScoringPlan()
		{
			Sport.Rulesets.RuleType type = Sport.Rulesets.RuleType.GetRuleType(typeof(TeamPhaseScoring));
			if (type == null)
				return null;

			int ruleTypeId = type.Id;
			string scoringPlanName = Phase.Definitions.Get(ruleTypeId, TeamPhaseScoringRule.PhaseScoring);
			if (scoringPlanName == null)
				return null;

			TeamPhaseScoring teamPhaseScoring = Phase.Championship.TeamPhaseScoring;
			if (teamPhaseScoring == null)
				return null;

			return teamPhaseScoring.GetScoringPlan(scoringPlanName);
		}

		private abstract class GroupScoreCalculator
		{
			private ArrayList[] _freeTeamsCompetitors;

			public GroupScoreCalculator(CompetitionGroup group)
			{
				// Creating arrays to store free competitors for later use
				_freeTeamsCompetitors = new ArrayList[group.Teams.Count];

			}

			protected void AddFreeCompetitor(Competitor competitor)
			{
				// The competitor is free - adding it to the free competitors array
				if (_freeTeamsCompetitors[competitor.Team] == null)
				{
					_freeTeamsCompetitors[competitor.Team] = new ArrayList();
				}
				_freeTeamsCompetitors[competitor.Team].Add(competitor);
			}

			// Gets not-calculated competitors for team
			public Competitor[] GetFreeCompetitors(int teamIndex)
			{
				Competitor[] result;
				ArrayList list = _freeTeamsCompetitors[teamIndex];
				if (list == null)
				{
					result = new Competitor[0];
				}
				else
				{
					result = new Competitor[list.Count];
					list.CopyTo(result, 0);
				}

				return result;
			}

			// Sets the competitor score into the calculator
			public abstract void SetScore(Competitor competitor);
			// Sets the team group score from the calculator
			public abstract void SetTeam(CompetitionTeam team);
		}

		#region Counters Calculator

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

		private class GroupScoreCountersCalculator : GroupScoreCalculator
		{
			private ScoreCompetitorList[,] _scoreLists;
			private ScoringPlan _scoringPlan;
			private int[,] _competitionCounters;

			public GroupScoreCountersCalculator(CompetitionGroup group)
				: base(group)
			{
				// Getting scoring plan
				_scoringPlan = group.GetScoringPlan();

				// Getting competitions counters - for sport field and sport field type
				_competitionCounters = new int[group.Competitions.Count, 2];
				for (int n = 0; n < group.Competitions.Count; n++)
				{
					_competitionCounters[n, 0] = GetCounterIndex(group.Competitions[n].SportFieldCounter);
					_competitionCounters[n, 1] = GetCounterIndex(group.Competitions[n].SportFieldTypeCounter);
				}

				_scoreLists = new ScoreCompetitorList[group.Teams.Count,
					_scoringPlan.Counters.Count + 1];

				for (int t = 0; t < group.Teams.Count; t++)
				{
					for (int c = 0; c < _scoringPlan.Counters.Count; c++)
					{
						// Creating competitor list for the counter
						_scoreLists[t, c] = new ScoreCompetitorList(_scoringPlan.Counters[c].Results);
					}

					// Creating competitor list for the default counter
					_scoreLists[t, _scoringPlan.Counters.Count] = new ScoreCompetitorList(_scoringPlan.AdditionalResults);
				}
			}

			private int GetCounterIndex(string name)
			{
				if (name != string.Empty)
				{
					for (int n = 0; n < _scoringPlan.Counters.Count; n++)
					{
						if (_scoringPlan.Counters[n].Name == name)
							return n;
					}
				}

				return _scoringPlan.Counters.Count; // default counter
			}

			public override void SetTeam(CompetitionTeam team)
			{
				team._score = 0;
				team._totalCounter = 0;
				team._counters = new int[_scoringPlan.Counters.Count + 1];
				/*
				if (team.Name == "רבין באר שבע")
					Sport.Common.Tools.WriteToLog("team counters created at SetTeam (" + team._counters.Length + ")");
				*/
				team._scores = new int[team._counters.Length];

				for (int n = 0; n < _scoringPlan.Counters.Count + 1; n++)
				{
					team._counters[n] = 0;
					team._scores[n] = 0;

					foreach (Competitor c in _scoreLists[team.Index, n].Competitors)
					{
						if (c != null)
						{
							team._totalCounter++;
							team._counters[n]++;
							team._scores[n] += c.Score;
							team._score += c.Score;
						}
					}
				}
			}

			public override void SetScore(Competitor competitor)
			{
				try
				{
					int sportFieldCounter = _competitionCounters[competitor.Competition.Index, 0];
					int sportFieldTypeCounter = _competitionCounters[competitor.Competition.Index, 1];

					Competitor c = null;

					// Trying to set score with sport field counter
					c = _scoreLists[competitor.Team, sportFieldCounter].SetScore(competitor);

					//	If competitor was not set and the sport field type counter is a different counter...
					if (c == competitor && sportFieldCounter != sportFieldTypeCounter)
					{
						// Trying to set score with sport field type counter
						c = _scoreLists[competitor.Team, sportFieldTypeCounter].SetScore(competitor);
					}

					// If competitor was still not set...
					if (c == competitor)
					{
						// Trying to set score with default counter (additional results)
						c = _scoreLists[competitor.Team, _scoringPlan.Counters.Count].SetScore(competitor);
					}

					// Checking if a competitor was not used or removed
					if (c != null)
					{
						// Checking is a competitor was removed from a score list because
						// of the setting of the competitor
						if (c != competitor)
						{
							// Resetting competitor
							SetScore(c);
						}
						else
						{
							AddFreeCompetitor(c);
						}
					}
				}
				catch (Exception ex)
				{
					System.Diagnostics.Debug.WriteLine("error in SetScore: " + ex.Message + "\n" + ex.StackTrace);
				}
			}
		}

		#endregion

		#region Multi Challenge Calculator

		private class TeamScoreMultiChallengeCalculator : IComparer
		{
			private SortedArray _competitors;

			private struct CompetitorResults
			{
				public int PlayerNumber;
				public int Competitions;
				public int Score;
				public CompetitorResults(int playerNumber)
				{
					PlayerNumber = playerNumber;
					Competitions = 0;
					Score = 0;
				}
			}

			private Hashtable _competitorsResults;
			private static Dictionary<string, RankingTable> _rankingTableMapping = new Dictionary<string, RankingTable>();
			private static Dictionary<int, List<SportServices.TeamCompetitionCompetitors>> _teamCompetitorsMapping = new Dictionary<int, List<SportServices.TeamCompetitionCompetitors>>();

			private static void OverrideCounters(CompetitionTeam team, RankingTable rankingTable)
			{
				if (rankingTable == null)
					return;

				int categoryId = team.Group.Phase.Championship.CategoryID;
				List<SportServices.TeamCompetitionCompetitors> allCompetitors;
				if (!_teamCompetitorsMapping.TryGetValue(categoryId, out allCompetitors))
				{
					allCompetitors = new List<SportServices.TeamCompetitionCompetitors>();
					using (SportServices.ChampionshipService service = new SportServices.ChampionshipService())
					{
						allCompetitors.AddRange(service.ReadCompetitionCompetitors(categoryId));
					}
					_teamCompetitorsMapping.Add(categoryId, allCompetitors);
				}

				Dictionary<string, int> sportFieldIndexMapping = new Dictionary<string, int>();
				rankingTable.Fields.OfType<RankField>().ToList().ForEach(rankField =>
				{
					string curTitle = rankField.Title;
					string curValue = rankField.Value;
					int curIndex;
					if (!sportFieldIndexMapping.ContainsKey(curTitle) && curValue.Length > 3 && curValue[1] == 'C')
					{
						if (Int32.TryParse(curValue.Substring(2, 1), out curIndex))
						{
							sportFieldIndexMapping.Add(curTitle, curIndex);
						}
					}
				});
				var teamCompetiros = allCompetitors.FindAll(tcc => tcc.TeamId == team.Id && tcc.Phase == team.Group.Phase.Index && tcc.Group == team.Group.Index);
				teamCompetiros.ForEach(tcc =>
				{
					string sportFieldType = tcc.SportFieldType;
					int fieldIndex;
					if (sportFieldIndexMapping.TryGetValue(sportFieldType, out fieldIndex) || sportFieldIndexMapping.TryGetValue(sportFieldType.FirstWord(), out fieldIndex))
					{
						int actualIndex = fieldIndex - 1;
						if (actualIndex >= 0 && actualIndex < team._counters.Length)
							team._counters[actualIndex] = tcc.Competitors;
					}
				});
			}

			private static RankingTable GetRankingTable(CompetitionTeam team)
			{
				//SportServices.TeamCompetitionCompetitors
				string key = team.Group.Phase.Championship.CategoryID + "_" + team.Group.Phase.Index;
				if (!_rankingTableMapping.ContainsKey(key))
				{
					RankingTable rankingTable = null;
					RankingTables rankingTables = team.Group.Phase.Championship.ChampionshipCategory.GetRule(typeof(RankingTables)) as RankingTables;
					if (rankingTables != null)
					{
						Sport.Rulesets.RuleType type = Sport.Rulesets.RuleType.GetRuleType(typeof(RankingTables));
						if (type != null)
						{
							string tableName = team.Group.Phase.Definitions.Get(type.Id, RankingTablesRule.PhaseTable);
							if (tableName == null)
							{
								rankingTable = rankingTables.DefaultRankingTable;
							}
							else
							{
								for (int n = 0; n < rankingTables.Tables.Count; n++)
								{
									if (rankingTables.Tables[n].Name == tableName)
									{
										rankingTable = rankingTables.Tables[n];
										break;
									}
								}
							}
						}
						_rankingTableMapping.Add(key, rankingTable);
					}
				}
				return _rankingTableMapping[key];
			}

			public TeamScoreMultiChallengeCalculator()
			{
				_competitors = new SortedArray(this);
				_competitorsResults = new Hashtable();
			}

			public void SetScore(Competitor competitor)
			{
				_competitors.Add(competitor);
				CompetitorResults competitorResults;
				if (_competitorsResults.ContainsKey(competitor.PlayerNumber))
				{
					competitorResults = (CompetitorResults)_competitorsResults[competitor.PlayerNumber];
				}
				else
				{
					competitorResults = new CompetitorResults(competitor.PlayerNumber);
				}

				competitorResults.Score += competitor.Score;
				competitorResults.Competitions++;

				_competitorsResults[competitor.PlayerNumber] = competitorResults;
			}

			public void CalculateScore(CompetitionTeam team, ArrayList freeCompetitors)
			{
				ScoringPlan scoringPlan = team.Group.GetScoringPlan();

				int multiChallengeResults = scoringPlan.MultiChallengeResults;

				// Sorting competitors results by result
				SortedList sortedResults = new SortedList();
				foreach (DictionaryEntry entry in _competitorsResults)
				{
					int curScore = ((CompetitorResults)entry.Value).Score;
					if (!sortedResults.ContainsKey(curScore))
					{
						sortedResults.Add(curScore, entry.Value);
					}
				}

				team._score = 0;
				team._totalCounter = 0;
				team._counters = new int[multiChallengeResults + 1];
				/*
				if (team.Name == "רבין באר שבע")
					Sport.Common.Tools.WriteToLog("team counters created at CalculateScore (" + team._counters.Length + ")");
				*/
				team._scores = new int[multiChallengeResults + 1];

				// Adding multi challenge results
				for (int n = 0; n < multiChallengeResults && n < sortedResults.Count; n++)
				{
					CompetitorResults competitorResults = (CompetitorResults)sortedResults.GetByIndex(n);
					team._score += competitorResults.Score;
					team._scores[n] = competitorResults.Score;
					team._counters[n] = competitorResults.Competitions;
					team._totalCounter++;
				}

				RankingTable rankingTable = GetRankingTable(team);
				if (rankingTable != null)
				{
					//override counters...
					OverrideCounters(team, rankingTable);
				}

				// Adding additional results
				int index = 0;
				int additionalResults = scoringPlan.AdditionalResults;
				while (index < _competitors.Count)
				{
					Competitor competitor = (Competitor)_competitors[index];
					// Checking competitor was not used for multi challenge results
					bool used = false;
					for (int n = 0; n < multiChallengeResults && n < sortedResults.Count && !used; n++)
					{
						CompetitorResults competitorResults = (CompetitorResults)sortedResults.GetByIndex(n);
						if (competitorResults.PlayerNumber == competitor.PlayerNumber)
						{
							used = true;
						}
					}

					if (!used)
					{
						if (additionalResults > 0)
						{
							// If competitor was not used - adding its score and 
							// decreasing additional results
							team._score += competitor.Score;
							team._scores[multiChallengeResults + 1] += competitor.Score;
							team._counters[multiChallengeResults + 1]++;
							team._totalCounter++;
							additionalResults--;
						}
						else
						{
							freeCompetitors.Add(competitor);
						}
					}

					index++;
				}
			}

			#region IComparer Members

			// Compating competitor scores for the sorted array
			public int Compare(object x, object y)
			{
				Competitor cx = (Competitor)x;
				Competitor cy = (Competitor)y;

				if (cx.Score > cy.Score)
				{
					return -1;
				}
				if (cx.Score == cy.Score)
				{
					return 0;
				}

				return 1;
			}

			#endregion
		}

		private class GroupScoreMultiChallengeCalculator : GroupScoreCalculator
		{
			private TeamScoreMultiChallengeCalculator[] _teamScoreMultiChallengeCalculators;
			private ScoringPlan _scoringPlan;
			private ArrayList[] _freeCompetitors;

			public GroupScoreMultiChallengeCalculator(CompetitionGroup group)
				: base(group)
			{
				// Getting scoring plan
				_scoringPlan = group.GetScoringPlan();

				// Creating arrays to store free competitors for later use
				_freeCompetitors = new ArrayList[group.Teams.Count];

				// Initializing team score multi challenge calculators
				_teamScoreMultiChallengeCalculators = new TeamScoreMultiChallengeCalculator[group.Teams.Count];
				for (int n = 0; n < group.Teams.Count; n++)
				{
					_teamScoreMultiChallengeCalculators[n] = new TeamScoreMultiChallengeCalculator();
				}
			}

			public override void SetTeam(CompetitionTeam team)
			{
				ArrayList freeCompetitors = new ArrayList();
				_teamScoreMultiChallengeCalculators[team.Index].CalculateScore(team, freeCompetitors);
				foreach (Competitor competitor in freeCompetitors)
				{
					AddFreeCompetitor(competitor);
				}
			}

			public override void SetScore(Competitor competitor)
			{
				if (competitor != null && competitor.Team >= 0 && competitor.Team < _teamScoreMultiChallengeCalculators.Length)
				{
					_teamScoreMultiChallengeCalculators[competitor.Team].SetScore(competitor);
				}
			}
		}

		#endregion

		protected override void DoCalculateTeamsScore()
		{
			if ((Teams == null) || (_players == null) || (_competitions == null))
				return;

			if ((Phase == null) || (Phase.Championship == null))
				return;

			for (int n = 0; n < Teams.Count; n++)
			{
				CompetitionTeam team = Teams[n] as CompetitionTeam;
				if (team != null)
					team._score = 0;
			}

			// Reseting player information
			foreach (CompetitionPlayer player in _players)
				player._competitonCount = 0;

			// Getting scoring plan calculator
			GroupScoreCalculator calculator = null;
			ScoringPlan scoringPlan = GetScoringPlan();
			if (scoringPlan != null)
			{
				if (scoringPlan.PlanType == ScoringPlanType.Counters)
				{
					calculator = new GroupScoreCountersCalculator(this);
				}
				else if (scoringPlan.PlanType == ScoringPlanType.MultiChallenge)
				{
					calculator = new GroupScoreMultiChallengeCalculator(this);
				}
			}

			if (calculator == null)
			{
				// No scoring plan calculator - just add scores
				foreach (Team team in Teams)
					team._score = 0;

				foreach (Competition competition in _competitions)
				{
					ArrayList arrCompetitors = new ArrayList();
					if (competition.Competitors != null)
						arrCompetitors.AddRange(competition.Competitors);
					Sport.Entities.OfflinePlayer[] arrOfflinePlayers =
						competition.GetOfflinePlayers();
					arrCompetitors.AddRange(arrOfflinePlayers);
					if (arrCompetitors.Count == 0)
						continue;
					foreach (object oComp in arrCompetitors)
					{
						if (oComp is Sport.Championships.Competitor)
						{
							Sport.Championships.Competitor competitor =
								(Sport.Championships.Competitor)oComp;
							if (competitor.Player != null)
								competitor.Player._competitonCount++;
							if (competitor.Score > 0)
							{
								int index = competitor._team;
								if ((index >= 0) && (index < Teams.Count) && (Teams[index] != null))
									Teams[index]._score += competitor.Score;
							}
						}
						else if (oComp is Sport.Entities.OfflinePlayer)
						{
							Sport.Entities.OfflinePlayer oPlayer =
								(Sport.Entities.OfflinePlayer)oComp;
							if (oPlayer.Score <= 0)
								continue;
							if (oPlayer.Team != null)
							{
								foreach (Sport.Championships.CompetitionTeam
											 compTeam in this.Teams)
								{
									if ((compTeam.TeamEntity != null) &&
										(compTeam.TeamEntity.Id == oPlayer.Team.Id))
									{
										compTeam._score += oPlayer.Score;
										break;
									}
								}
							}
						} //end if current item is offline player
					} //end loop over competitors

					/*
					foreach (Competitor competitor in competition.Competitors)
					{
						if (competitor.Player != null)
							competitor.Player._competitonCount++;
						
						if (competitor.Score > 0)
						{
							int index=competitor._team;
							if ((index >= 0)&&(index < Teams.Count)&&(Teams[index] != null))
								Teams[index]._score += competitor.Score;
						}
					}
					*/
				}
			}
			else
			{
				/*
				if (this.Phase.Index == 3)
					Sport.Common.Tools.WriteToLog("have calculator and " + this.Teams.Count +  " teams");
				*/

				foreach (Competition competition in _competitions)
				{
					if (competition.Competitors == null)
						continue;

					foreach (Competitor competitor in competition.Competitors)
					{
						if (competitor.Player != null)
						{
							competitor.Player._competitonCount++;
						}

						if (competitor.Score > 0)
						{
							calculator.SetScore(competitor);
						}
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
			{
				if (teams[n] != null)
					scores[n] = teams[n].Score;
			}

			Array.Sort(scores, teams);

			for (int n = 0; n < teams.Length; n++)
			{
				int index = -1;
				if (teams[n] != null)
					index = teams[n].Index;
				if ((index >= 0) && (index < Teams.Count) && (Teams[index] != null))
				{
					Teams[index]._position = teams.Length - n - 1;
				}
			}

			/*
			string s = "";
			for (int n = 0; n < scores.Length; n++)
			{
				s += "id: " + teams[n].TeamEntity.Name + ", score: " + scores[n] + "\n";
			}
			Sport.UI.MessageBox.Show(s);
			*/
		}

		public Sport.Documents.Data.Table GetTeamsTable()
		{
			//initialize local variables:
			Hashtable tblTeamCompetitors = new Hashtable();
			Hashtable tblTeamFaults = new Hashtable();
			Hashtable tblResultTypes = new Hashtable();
			bool blnMultiCompetition = false;
			bool blnGroupCompetition = false;
			bool blnScoreIsRank = false;
			int nBestResultsCount = 999;

			if (this.Phase.Championship.ChampionshipCategory != null && this.Competitions.Count > 0 && this.Competitions[0].SportField.SportFieldType != null)
			{
				Rulesets.Rules.TeamScoreCounters objScoreCountersRule = null;
				Sport.Entities.SportFieldType objSportFieldType = this.Competitions[0].SportField.SportFieldType;
				int nSportFieldType = objSportFieldType.Id;
				if (Sport.Core.Session.Connected)
				{
					Sport.Entities.ChampionshipCategory champCategory = this.Phase.Championship.ChampionshipCategory;
					Sport.Rulesets.Ruleset ruleset = champCategory.GetRuleset();
					if (ruleset != null)
					{
						objScoreCountersRule = (Rulesets.Rules.TeamScoreCounters)
							ruleset.GetRule(new Rulesets.RuleScope(champCategory.Category, nSportFieldType), typeof(Rulesets.Rules.TeamScoreCounters), false);
					}
					//objScoreCountersRule = this.Phase.Championship.ChampionshipCategory.GetRule(typeof(Rulesets.Rules.TeamScoreCounters), objSportFieldType) 
					//	as Rulesets.Rules.TeamScoreCounters;
				}
				else
				{
					objScoreCountersRule = Sport.Rulesets.Ruleset.LoadOfflineRule(
						typeof(Rulesets.Rules.TeamScoreCountersRule),
						this.Phase.Championship.CategoryID, nSportFieldType) as Rulesets.Rules.TeamScoreCounters;
				}

				if (objScoreCountersRule != null && objScoreCountersRule.Counters.Count > 0)
					nBestResultsCount = objScoreCountersRule.Counters[0].Results;

				/*
				
				*/
			}

			//multi or group competition type?
			foreach (Competition competition in this.Competitions)
			{
				Sport.Types.CompetitionType compType = competition.SportField.SportFieldType.CompetitionType;
				if (compType == Sport.Types.CompetitionType.MultiCompetition)
					blnMultiCompetition = true;
				else if (compType == Sport.Types.CompetitionType.Group)
					blnGroupCompetition = true;
			}

			/*
			string s = "";
			for (int n = 0; n < this.Teams.Count; n++)
			{
				s += "id: " + this.Teams[n].TeamEntity.Name + ", score: " + this.Teams[n].Score + "\n";
			}
			Sport.UI.MessageBox.Show(s);
			*/

			//get all teams:
			ArrayList arrTeams = new ArrayList(this.Teams);

			/*
			//reset score:
			foreach (CompetitionTeam compTeam in arrTeams)
				compTeam.CustomScore = 0;
			*/

			//add offline teams:
			arrTeams.AddRange(this.GetOfflineTeams());

			//multi competition?
			if (blnMultiCompetition)
			{
				tblTeamCompetitors = this.GetGroupMutliCompetitionCompetitors();

				/*
				foreach (int shirtNumber in tblTeamCompetitors.Keys)
				{
					Hashtable tblCompetitions=(Hashtable) tblTeamCompetitors[shirtNumber];
					foreach (object oComp in tblCompetitions.Values)
					{
						int curScore = 0;
						object curTeam = null;
						if (oComp is Sport.Championships.Competitor)
						{
							Competitor competitor = (Competitor) oComp;
							curScore = competitor.Score;
							if (competitor.Player != null)
								curTeam = competitor.Player.CompetitionTeam;
						}
						else if (oComp is Sport.Entities.OfflinePlayer)
						{
							Sport.Entities.OfflinePlayer oPlayer = 
								(Sport.Entities.OfflinePlayer) oComp;
							curScore = oPlayer.Score;
							if (oPlayer.Team != null)
							{
								foreach (CompetitionTeam objCompTeam in this.Teams)
								{
									if (objCompTeam.TeamEntity.Id == oPlayer.Team.Id)
									{
										curTeam = objCompTeam;
										break;
									}
								}
							}
							else if (oPlayer.OfflineTeam != null)
								curTeam = oPlayer.OfflineTeam;
						} //end if offline player
						
						if (curTeam != null)
						{
							int index = arrTeams.IndexOf(curTeam);
							if (index >= 0)
							{
								object team = arrTeams[index];
								if (team is Sport.Championships.CompetitionTeam)
									(team as Sport.Championships.CompetitionTeam).CustomScore += curScore;
								else if (team is Sport.Entities.OfflineTeam)
									(team as Sport.Entities.OfflineTeam).Score += curScore;
							}
						}
					} //end loop over competitors
				} //end loop over shirt numbers
				*/
			} //end if multi competition

			//iterate over group competitions to build list of competitors:
			foreach (Competition competition in this.Competitions)
			{
				//multi competition?
				if (competition.SportField.SportFieldType.CompetitionType == Sport.Types.CompetitionType.MultiCompetition)
					continue;

				//declare minimum and maximum competitors:
				int minCompetitors = 0;
				int maxCompetitors = 999;

				//declare general variables:
				blnScoreIsRank = false;
				int resultsCount = 99999;

				//group competition?
				if (blnGroupCompetition)
					resultsCount = 1;

				//ordinary report?
				if ((blnMultiCompetition == false) && (blnGroupCompetition == false))
				{
					//get rule:
					CompetitionTeamCompetitors objRule =
						(CompetitionTeamCompetitors)
						competition.GetRule(typeof(CompetitionTeamCompetitors),
						typeof(CompetitionTeamCompetitorsRule));

					//get minimum and maximum competitors:
					if (objRule != null)
					{
						minCompetitors = objRule.Minimum;
						maxCompetitors = objRule.Maximum;
					}

					//score is rank?
					blnScoreIsRank = competition.ScoreIsRank();

					//best results count:
					resultsCount = competition.GetBestResults();
				} //end if not multi competition or group competition

				//build list of all competitors:
				ArrayList arrCompetitors = new ArrayList(competition.Competitors);
				arrCompetitors.AddRange(competition.GetOfflinePlayers());

				//sort by score:
				arrCompetitors.Sort(new CompetitorsScoreComparer(blnScoreIsRank));

				//calculate score of each team:
				foreach (object ot in arrTeams)
				{
					//need to create?
					if (tblTeamCompetitors[ot] == null)
						tblTeamCompetitors[ot] = new ArrayList();

					//get data:
					Sport.Championships.CompetitionTeam compTeam = null;
					Sport.Entities.OfflineTeam oTeam = null;
					if (ot is Sport.Championships.CompetitionTeam)
						compTeam = (Sport.Championships.CompetitionTeam)ot;
					else if (ot is Sport.Entities.OfflineTeam)
						oTeam = (Sport.Entities.OfflineTeam)ot;
					int teamID = -1;
					if (compTeam != null)
						teamID = compTeam.TeamEntity.Id;
					else if (oTeam != null)
						teamID = oTeam.OfflineID;

					//got anything?
					if (teamID < 0)
						continue;

					/*
					Hashtable tblBestResults = this.GetBestResults(teamID);
					if (tblBestResults != null && tblBestResults[competition.Index] != null)
						resultsCount = (int) tblBestResults[competition.Index];
					*/

					//violated rule?
					ArrayList arrTeamCompetitors = new ArrayList();
					foreach (object oComp in arrCompetitors)
					{
						int curTeamID = -1;
						if (oComp is Sport.Championships.Competitor)
						{
							Sport.Championships.Competitor competitor =
								(Sport.Championships.Competitor)oComp;
							if ((competitor.Player != null) &&
								(competitor.Player.CompetitionTeam != null) &&
								(competitor.Player.CompetitionTeam.TeamEntity != null))
							{
								curTeamID = competitor.Player.CompetitionTeam.TeamEntity.Id;
							}
						}
						else if (oComp is Sport.Entities.OfflinePlayer)
						{
							Sport.Entities.OfflinePlayer oPlayer =
								(Sport.Entities.OfflinePlayer)oComp;
							if (oPlayer.Team != null)
								curTeamID = oPlayer.Team.Id;
							else if (oPlayer.OfflineTeam != null)
								curTeamID = oPlayer.OfflineTeam.OfflineID;
						}
						if (curTeamID < 0)
							continue;
						if (curTeamID == teamID)
						{
							ArrayList arr = (ArrayList)tblTeamCompetitors[ot];
							if (arr.IndexOf(oComp) < 0)
								arr.Add(oComp);
							tblTeamCompetitors[ot] = arr;
							tblResultTypes[oComp] = competition.ResultType;
							arrTeamCompetitors.Add(oComp);
						}
					}
					if ((arrTeamCompetitors.Count < minCompetitors) ||
						(arrTeamCompetitors.Count > maxCompetitors))
					{
						if (blnScoreIsRank)
						{
							if (compTeam != null)
								compTeam.Score = 99999;
							else if (oTeam != null)
								oTeam.Score = 99999;
						}
						if (arrTeamCompetitors.Count < minCompetitors)
							tblTeamFaults[ot] = "פחות מידי מתמודדים";
						else
							tblTeamFaults[ot] = "יותר מידי מתמודדים";
						continue;
					}

					/*
					//take good results:
					int count=0;
					for (int i=0; i<arrTeamCompetitors.Count; i++)
					{
						//got enough results?
						if (count >= resultsCount)
							break;
						
						//get current score:
						int curScore = 0;
						object oComp = arrTeamCompetitors[i];
						if (oComp is Sport.Championships.Competitor)
							curScore = (oComp as Sport.Championships.Competitor).Score;
						else if (oComp is Sport.Entities.OfflinePlayer)
							curScore = (oComp as Sport.Entities.OfflinePlayer).Score;
						
						//valid?
						if (curScore > 0)
						{
							//add current score:
							if (compTeam != null)
								compTeam.CustomScore += curScore;
							else if (oTeam != null)
								oTeam.Score += curScore;
							count++;
						}
					} //end if ordinary competition
					*/
				} //end loop over teams.
			} //end loop over competitions

			//apply best score rule if needed
			if (blnScoreIsRank && nBestResultsCount < 999 && nBestResultsCount > 0)
			{
				foreach (object ot in arrTeams)
				{
					//ignore faulted teams
					if (tblTeamFaults[ot] != null && tblTeamFaults[ot].ToString().Length > 0)
						continue;

					//get data:
					Sport.Championships.CompetitionTeam compTeam = null;
					Sport.Entities.OfflineTeam oTeam = null;
					if (ot is Sport.Championships.CompetitionTeam)
						compTeam = (Sport.Championships.CompetitionTeam)ot;
					else if (ot is Sport.Entities.OfflineTeam)
						oTeam = (Sport.Entities.OfflineTeam)ot;

					//reset score
					if (compTeam != null)
						compTeam.Score = 0;
					else if (oTeam != null)
						oTeam.Score = 0;

					//recalculate
					if (tblTeamCompetitors[ot] != null)
					{
						ArrayList arrCompetitors = (ArrayList)tblTeamCompetitors[ot];
						for (int i = 0; i < nBestResultsCount; i++)
						{
							if (i >= arrCompetitors.Count)
								break;
							int curScore = 0;
							object oComp = arrCompetitors[i];
							if (oComp is Sport.Championships.Competitor)
								curScore = (oComp as Sport.Championships.Competitor).ResultPosition + 1; //.Position + 1; //.Score;
							else if (oComp is Sport.Entities.OfflinePlayer)
								curScore = (oComp as Sport.Entities.OfflinePlayer).Rank + 1; //.Score;

							if (compTeam != null)
								compTeam.Score += curScore;
							else if (oTeam != null)
								oTeam.Score += curScore;
						}
					}
				}
			}

			//sort competition teams by score...
			arrTeams.Sort(new TeamScoreComparer(blnScoreIsRank));

			//build teams table.
			int colCount = 5;
			Sport.Documents.Data.Table table = new Sport.Documents.Data.Table();
			table.Headers = new Sport.Documents.Data.Column[colCount];
			table.Headers[0] = new Sport.Documents.Data.Column("דירוג", 0.12);
			table.Headers[1] = new Sport.Documents.Data.Column("ניקוד", 0.13);
			table.Headers[2] = new Sport.Documents.Data.Column("סמל בית ספר", 0.15);
			table.Headers[3] = new Sport.Documents.Data.Column("שם קבוצה", 0.25);
			table.Headers[4] = new Sport.Documents.Data.Column("פסול", 0.35);
			if (blnScoreIsRank)
			{
				table.Headers[1].RelativeWidth = 0.15;
				table.Headers[4].RelativeWidth = 0.33;
				for (int i = 0; i < table.Headers.Length; i++)
				{
					table.Headers[i].ShowBorder = false;
				}
			}

			//iterate over competitors:
			ArrayList rows = new ArrayList();
			for (int t = 0; t < arrTeams.Count; t++)
			{
				//get current team:
				object ot = arrTeams[t];
				Sport.Championships.CompetitionTeam compTeam = null;
				Sport.Entities.OfflineTeam oTeam = null;
				int curScore = 0;
				string strSchoolSymbol = "";
				string strTeamName = "";
				if (ot is Sport.Championships.CompetitionTeam)
				{
					compTeam = (Sport.Championships.CompetitionTeam)ot;
					curScore = (int)compTeam.Score; //.CustomScore;
					if ((compTeam.TeamEntity != null) && (compTeam.TeamEntity.School != null))
						strSchoolSymbol = compTeam.TeamEntity.School.Symbol;
					strTeamName = compTeam.Name;
				}
				else if (ot is Sport.Entities.OfflineTeam)
				{
					oTeam = (Sport.Entities.OfflineTeam)ot;
					curScore = oTeam.Score;
					if (oTeam.School != null)
						strSchoolSymbol = oTeam.School.Symbol;
					else if (oTeam.OfflineSchool != null)
						strSchoolSymbol = oTeam.OfflineSchool.Symbol;
					strTeamName = oTeam.ToString();
				}

				//build current row
				Sport.Documents.Data.Row objRow = new Sport.Documents.Data.Row();

				//initialize cells text:
				ArrayList arrTexts = new ArrayList();

				//get competitors:
				ArrayList arrCompetitors = (ArrayList)tblTeamCompetitors[ot];

				//rank:
				string strPosition = (t + 1).ToString();
				arrTexts.Add(strPosition);

				//fault?
				string strFault = "";
				if (!blnMultiCompetition)
				{
					strFault = Sport.Common.Tools.CStrDef(tblTeamFaults[ot], "");
				}

				//score
				string strScore = curScore.ToString();
				if (strFault.Length > 0)
					strScore = (blnScoreIsRank) ? "1000" : "0";
				arrTexts.Add(strScore);

				//symbol:
				if (strSchoolSymbol != null)
					arrTexts.Add(strSchoolSymbol);

				//team name:
				if (strTeamName != null)
					arrTexts.Add(strTeamName);

				//add fault:
				if (strFault != null)
					arrTexts.Add(strFault);

				//add cells:
				objRow.AddCells((string[])arrTexts.ToArray(typeof(string)), true);

				//add borders:
				for (int cell = 0; cell < objRow.Cells.Length; cell++)
				{
					objRow.Cells[cell].Borders = (Sport.Documents.Borders.Top | Sport.Documents.Borders.Bottom);
				}

				//add to array:
				rows.Add(objRow);

				//competitors?
				if ((blnScoreIsRank) && (strFault.Length == 0))
				{
					//totals:
					int totalResult = 0;
					int totalScore = 0;

					//header:
					objRow = new Sport.Documents.Data.Row();
					objRow.AddCells(new string[] {"מ. חזה", "שם משפחה", "שם פרטי", 
									 "תוצאה", "דירוג"}, false);
					rows.Add(objRow);

					//add row for each competitor:
					//int compIndex=0;
					ResultType lastResultType = null;
					for (int k = 0; k < arrCompetitors.Count; k++)
					{
						//apply best results rule:
						if (k >= nBestResultsCount)
							break;

						object oComp = arrCompetitors[k];

						//get current data.
						int curResult = 0;
						int curShirtNumber = -1;
						string curFirstName = "";
						string curLastName = "";
						ResultType resultType = null;
						if (tblResultTypes[oComp] != null)
						{
							resultType = (ResultType)
								tblResultTypes[oComp];
						}
						if (resultType != null)
							lastResultType = resultType;
						int curCompScore = 0;
						if (oComp is Sport.Championships.Competitor)
						{
							Sport.Championships.Competitor competitor =
								(Sport.Championships.Competitor)oComp;
							curResult = competitor.Result;
							curShirtNumber = competitor.PlayerNumber;
							if ((competitor.Player != null) && (competitor.Player.PlayerEntity != null))
							{
								Sport.Entities.Student student = competitor.Player.PlayerEntity.Student;
								curFirstName = student.FirstName;
								curLastName = student.LastName;
							}
							curCompScore = competitor.ResultPosition + 1; //.Position + 1; //.Score;
						}
						else if (oComp is Sport.Entities.OfflinePlayer)
						{
							Sport.Entities.OfflinePlayer oPlayer =
								(Sport.Entities.OfflinePlayer)oComp;
							curResult = oPlayer.Result;
							curShirtNumber = oPlayer.ShirtNumber;
							if (oPlayer.Student != null)
							{
								curFirstName = oPlayer.Student.FirstName;
								curLastName = oPlayer.Student.LastName;
							}
							else if (oPlayer.OfflineStudent != null)
							{
								curFirstName = oPlayer.OfflineStudent.FirstName;
								curLastName = oPlayer.OfflineStudent.LastName;
							}
							curCompScore = oPlayer.Rank + 1; //.Score;
						}

						//got result?
						if (curResult <= 0)
							continue;

						//create row for current competitor:
						objRow = new Sport.Documents.Data.Row();
						arrTexts.Clear();

						//shirt number:
						arrTexts.Add(curShirtNumber.ToString());

						//last name:
						if (curLastName != null)
							arrTexts.Add(curLastName);

						//first name:
						if (curFirstName != null)
							arrTexts.Add(curFirstName);

						//result:
						string strResult = "";
						if (resultType != null)
							strResult = resultType.FormatResult(curResult);
						if (strResult != null)
							arrTexts.Add(strResult);

						//rank:
						arrTexts.Add(curCompScore.ToString());

						//add cells:
						objRow.AddCells(
							(string[])arrTexts.ToArray(typeof(string)), false);

						//add to array:
						rows.Add(objRow);

						//totals:
						totalScore += curCompScore;
						totalResult += curResult;

						//next competitor:
						//compIndex++;
					}

					//create row for the totals:
					objRow = new Sport.Documents.Data.Row();
					string strTotalResult = "";
					if (lastResultType != null)
						strTotalResult = lastResultType.FormatResult(totalResult);
					objRow.AddCells(new string[] {"", "", "סה\"כ מצטבר", 
								 strTotalResult, totalScore.ToString()}, false);
					rows.Add(objRow);
				} //end if no fault
			} //end loop over competitors

			//add table rows:
			table.Rows = new Sport.Documents.Data.Row[rows.Count];
			for (int row = 0; row < rows.Count; row++)
			{
				table.Rows[row] = (Sport.Documents.Data.Row)rows[row];
			}

			//done.
			return table;
		}

		public Hashtable GetBestResults(int teamID)
		{
			Hashtable tblCompetitionBestResults = new Hashtable();
			Sport.Rulesets.Rules.ScoringPlan plan = this.GetScoringPlan();
			if (plan != null)
			{
				ArrayList arrAllCompetitors = new ArrayList();

				if (plan.AdditionalResults > 0)
				{
					foreach (Competition competition in this.Competitions)
						arrAllCompetitors.AddRange(competition.GetTeamCompetitors(teamID));
					arrAllCompetitors.Sort(new CompetitorsScoreComparer(false));
				}

				Hashtable tblCompetionNames = new Hashtable();
				foreach (Competition competition in this.Competitions)
				{
					string key = competition.SportField.Name;
					if (competition.SportField.SportFieldType != null)
					{
						key += " " + competition.SportField.SportFieldType.Name;
					}
					tblCompetionNames[key] = competition.Index;
				}

				int additionalResults = plan.AdditionalResults;
				if (plan.Counters != null)
				{
					for (int i = 0; i < plan.Counters.Count; i++)
					{
						foreach (string strCompName in tblCompetionNames.Keys)
						{
							if (strCompName.IndexOf(plan.Counters[i].Name) >= 0)
							{
								int competitionIndex = (int)tblCompetionNames[strCompName];
								int curResults = plan.Counters[i].Results;
								tblCompetitionBestResults[competitionIndex] = curResults;
								if (additionalResults > 0)
								{
									for (int j = 0; j < curResults; j++)
									{
										int indexToRemove = -1;
										for (int k = 0; k < arrAllCompetitors.Count; k++)
										{
											if (GetCompetitionIndex(arrAllCompetitors[k]) == competitionIndex)
											{
												indexToRemove = k;
												break;
											}
										}
										if (indexToRemove >= 0)
											arrAllCompetitors.RemoveAt(indexToRemove);
									}
								}
								break;
							}
						}
					}
				}

				if (additionalResults > 0)
				{
					for (int i = 0; i < additionalResults; i++)
					{
						if (i >= arrAllCompetitors.Count)
							break;
						int competitionIndex = GetCompetitionIndex(arrAllCompetitors[i]);
						if (competitionIndex >= 0)
						{
							int curBestResults = 0;
							if (tblCompetitionBestResults[competitionIndex] != null)
							{
								curBestResults = (int)tblCompetitionBestResults[competitionIndex];
							}
							tblCompetitionBestResults[competitionIndex] = (curBestResults + 1);
						}
					}
				}
			}

			return tblCompetitionBestResults;
		}

		private int GetCompetitionIndex(object oComp)
		{
			if (oComp is Sport.Championships.Competitor)
				return (oComp as Sport.Championships.Competitor).Competition.Index;

			if (oComp is Sport.Entities.OfflinePlayer)
				return (oComp as Sport.Entities.OfflinePlayer).Competition;

			return -1;
		}

		public Hashtable GetGroupMutliCompetitionCompetitors()
		{
			//initialize result:
			Hashtable tblCompetitors = new Hashtable();

			//get how many competitions to take:
			int bestCompetitions = 999;
			foreach (Competition competition in this.Competitions)
			{
				//multi competition?
				if (competition.SportField.SportFieldType.CompetitionType != Sport.Types.CompetitionType.MultiCompetition)
					continue;

				//get rule:
				CompetitorCompetitions objRule =
					competition.CompetitorCompetitions;

				//got anything?
				if (objRule != null)
				{
					bestCompetitions = objRule.Maximum;
					break;
				}
			} //end loop over competitions

			//iterate over competitions:
			foreach (Competition competition in this.Competitions)
			{
				//multiply competition?
				if (competition.SportField.SportFieldType.CompetitionType != Sport.Types.CompetitionType.MultiCompetition)
					continue;

				//get all competitors and offline players.
				ArrayList arrCompetitors = new ArrayList(competition.Competitors);
				arrCompetitors.AddRange(competition.GetOfflinePlayers());

				//iterate over competitors:
				foreach (object oComp in arrCompetitors)
				{
					//get shirt number and score:
					int shirtNumber = -1;
					int score = 0;

					if (oComp is Sport.Championships.Competitor)
					{
						shirtNumber = (oComp as Sport.Championships.Competitor).PlayerNumber;
						score = (oComp as Sport.Championships.Competitor).Score;
					}
					else if (oComp is Sport.Entities.OfflinePlayer)
					{
						shirtNumber = (oComp as Sport.Entities.OfflinePlayer).ShirtNumber;
						score = (oComp as Sport.Entities.OfflinePlayer).Score;
					}

					//got any score?
					if (score <= 0)
						continue;

					//need to create?
					if (tblCompetitors[shirtNumber] == null)
						tblCompetitors[shirtNumber] = new Hashtable();

					//add current competition:
					(tblCompetitors[shirtNumber] as Hashtable)[competition] = oComp;
				}
			} //end loop over competitions

			//loop over competitors to calculate score:
			foreach (int shirtNumber in tblCompetitors.Keys)
			{
				//get list of competitions:
				Hashtable tblCompetitions = (Hashtable)tblCompetitors[shirtNumber];

				//get competitor and get list of results:
				ArrayList arrCompetitionResults = new ArrayList();
				foreach (Competition competition in tblCompetitions.Keys)
				{
					int score = -1;
					object oComp = tblCompetitions[competition];
					if (oComp is Sport.Championships.Competitor)
						score = (oComp as Sport.Championships.Competitor).Score;
					else if (oComp is Sport.Entities.OfflinePlayer)
						score = (oComp as Sport.Entities.OfflinePlayer).Score;
					if (score >= 0)
						arrCompetitionResults.Add(score);
				}

				//need to take only part of the results?
				if (bestCompetitions < arrCompetitionResults.Count)
				{
					//sort:
					arrCompetitionResults.Sort();
					arrCompetitionResults.Reverse();

					//remove:
					while (arrCompetitionResults.Count > bestCompetitions)
					{
						//remove the last:
						int index = arrCompetitionResults.Count - 1;

						//get score:
						int curScore = (int)arrCompetitionResults[index];

						//remove the competition where competitor had that score:
						object key = null;
						foreach (Competition competition in tblCompetitions.Keys)
						{
							int score = -1;
							object oComp = tblCompetitions[competition];
							if (oComp is Sport.Championships.Competitor)
								score = (oComp as Sport.Championships.Competitor).Score;
							else if (oComp is Sport.Entities.OfflinePlayer)
								score = (oComp as Sport.Entities.OfflinePlayer).Score;
							if (score == curScore)
							{
								key = competition;
								break;
							}
						}
						if (key != null)
							tblCompetitions.Remove(key);

						//remove from the array:
						arrCompetitionResults.RemoveAt(index);
					}
				} //end if need to take only part of the results
			} //end loop over competitors

			//done.
			return tblCompetitors;
		} //end function GetGroupMutliCompetitionCompetitors

		public class TeamScoreComparer : IComparer
		{
			private int _multiplyer = 1;

			public TeamScoreComparer(bool blnScoreIsRank)
			{
				if (blnScoreIsRank)
					_multiplyer = -1;
			}

			public int Compare(object x, object y)
			{
				if ((x == null) && (y == null))
					return 0;
				if (x == null)
					return 1;
				if (y == null)
					return -1;
				int s1 = 0;
				int s2 = 0;
				if (x is Sport.Championships.CompetitionTeam)
					s1 = (int)(x as Sport.Championships.CompetitionTeam).Score; //.CustomScore;
				else if (x is Sport.Entities.OfflineTeam)
					s1 = (x as Sport.Entities.OfflineTeam).Score;
				if (y is Sport.Championships.CompetitionTeam)
					s2 = (int)(y as Sport.Championships.CompetitionTeam).Score; //.CustomScore;
				else if (y is Sport.Entities.OfflineTeam)
					s2 = (y as Sport.Entities.OfflineTeam).Score;
				return -1 * (_multiplyer) * (s1.CompareTo(s2));
			}
		}
	}

	#endregion
}
