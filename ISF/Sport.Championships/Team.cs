using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Sport.Championships
{
	#region Team Class

	public class Team : Sport.Common.GeneralCollection.CollectionItem
	{
		public virtual void Reset()
		{
			_score = 0;
			_position = -1;
		}

		public virtual bool IsConfirmed()
		{
			if (_team == null)
				return false;

			if (_team.Status != Sport.Types.TeamStatusType.Confirmed)
				return false;

			return true;
		}

		public virtual bool IsValidShirtRange()
		{
			return true;
		}

		#region Properties

		internal int _index;
		public int Index
		{
			get { return _index; }
		}

		private Sport.Entities.Team _team;
		public Sport.Entities.Team TeamEntity
		{
			get { return _team; }
			set { _team = value; }
		}

		public int Id
		{
			get { return _team == null ? -1 : _team.Id; }
		}

		public virtual string Name
		{
			get
			{
				string result = "(ריק)";
				if ((this.TeamEntity != null) && (this.TeamEntity.School != null))
				{
					result = this.TeamEntity.TeamName();
					/*
					object objIndex=this.TeamEntity.Entity.Fields[
						(int) Sport.Entities.Team.Fields.Index];
					int index=Sport.Common.Tools.CIntDef(objIndex, 0);
					if (index > 0)
					{
						string letter=Sport.Common.Tools.ToHebLetter(index);
						result += " "+letter;
						if (letter.Length == 1)
						{
							result += "'";
						}
					}
					*/
				}

				return result;
			}
		}

		internal double _score;
		public double Score
		{
			get { return _score; }
			set { _score = value; }
		}

		private double _customScore = 0;
		public double CustomScore
		{
			get { return _customScore; }
			set { _customScore = value; }
		}

		internal int _position;
		public int Position
		{
			get { return _position; }
		}
		#endregion

		#region CollectionItem Members

		public Group Group
		{
			get { return ((Group)Owner); }
		}

		public override void OnOwnerChange(object oo, object no)
		{
			if (no == null)
				_index = -1;
		}

		#endregion

		#region Constructors

		public Team(Sport.Entities.Team team)
		{
			if ((team != null) && (team.IsValid()))
			{
				_team = team;
			}
			else
			{
				_team = null;
			}
			_score = 0;
			_position = -1;
		}

		public Team(Team team)
		{
			_team = team.TeamEntity;
			_score = team.Score;
			_position = team.Position;
		}

		#endregion

		#region Save

		internal virtual SportServices.Team Save()
		{
			return null;
		}

		#endregion

		#region Team Comparison

		public virtual bool Equals(Team team)
		{
			if (_team == null)
				return team._team == null;
			if (team._team == null)
				return false;
			return _team.Id == team.TeamEntity.Id;
		}

		#endregion

		#region Fields

		public virtual void SetFields(Sport.Common.EquationVariables variables)
		{
			variables["R"] = _position;
			variables["S"] = _score;
		}

		#endregion
	}

	#endregion

	#region MatchTeam Class

	/// <summary>
	/// The MatchTeam holds information of a team in a group in a matches
	/// championship
	/// </summary>
	public class MatchTeam : Team
	{
		#region Properties

		private int _previousGroup;
		public int PreviousGroup
		{
			get { return _previousGroup; }
		}

		private int _previousPosition;
		public int PreviousPosition
		{
			get { return _previousPosition; }
		}

		public new MatchGroup Group
		{
			get { return ((MatchGroup)base.Group); }
		}

		#endregion

		#region Team Information

		internal int _games;
		public int Games
		{
			get { return _games; }
		}

		internal double _points;
		public double Points
		{
			get { return _points; }
		}

		internal double _pointsAgainst;
		public double PointsAgainst
		{
			get { return _pointsAgainst; }
		}

		internal int _sets;
		public int Sets
		{
			get { return _sets; }
		}

		internal int _setsAgainst;
		public int SetsAgainst
		{
			get { return _setsAgainst; }
		}

		internal int _smallPoints;
		public int SmallPoints
		{
			get { return _smallPoints; }
		}

		internal int _smallPointsAgainst;
		public int SmallPointsAgainst
		{
			get { return _smallPointsAgainst; }
		}

		internal int _wins;
		public int Wins
		{
			get { return _wins; }
		}

		internal int _loses;
		public int Loses
		{
			get { return _loses; }
		}

		internal int _ties;
		public int Ties
		{
			get { return _ties; }
		}

		internal int _technicalWins;
		public int TechnicalWins
		{
			get { return _technicalWins; }
		}

		internal int _technicalLoses;
		public int TechnicalLoses
		{
			get { return _technicalLoses; }
		}

		public override void SetFields(Sport.Common.EquationVariables variables)
		{
			base.SetFields(variables);

			variables["G"] = _games;
			variables["P"] = _points;
			variables["C"] = _pointsAgainst;
			variables["M"] = _sets;
			variables["N"] = _setsAgainst;
			variables["T"] = _smallPoints;
			variables["Y"] = _smallPointsAgainst;
			variables["W"] = _wins;
			variables["L"] = _loses;
			variables["E"] = _technicalWins;
			variables["F"] = _technicalLoses;
			variables["D"] = _ties;
		}

		#endregion

		public override void Reset()
		{
			base.Reset();
			// If the team is relative to previous phase, need
			// to clear current team
			if (_previousGroup != -1)
				TeamEntity = null;
			_games = 0;
			_points = 0;
			_pointsAgainst = 0;
			_sets = 0;
			_setsAgainst = 0;
			_smallPoints = 0;
			_smallPointsAgainst = 0;
			_wins = 0;
			_loses = 0;
			_ties = 0;
			_technicalWins = 0;
			_technicalLoses = 0;
		}

		public MatchTeam(Sport.Entities.Team team)
			: base(team)
		{
			_previousGroup = -1;
			_previousPosition = -1;
		}

		public MatchTeam(int previousGroup, int previousPosition)
			: base((Sport.Entities.Team)null)
		{
			_previousGroup = previousGroup;
			_previousPosition = previousPosition;
		}

		internal MatchTeam(SportServices.Team steam)
			: base(steam.TeamId == -1 ? null : new Sport.Entities.Team(steam.TeamId))
		{
			if ((steam.TeamId != -1) && (this.TeamEntity != null))
			{
				_score = steam.Score;
				_position = steam.ResultPosition;

				_games = steam.Games;
				_points = steam.Points;
				_pointsAgainst = steam.PointsAgainst;
				_sets = steam.Sets;
				_setsAgainst = steam.SetsAgainst;
				_smallPoints = steam.SmallPoints;
				_smallPointsAgainst = steam.SmallPointsAgainst;
				_wins = steam.Wins;
				_loses = steam.Loses;
				_ties = steam.Ties;
				_technicalWins = steam.TechnicalWins;
				_technicalLoses = steam.TechnicalLoses;
			}

			_previousGroup = steam.PreviousGroup;
			_previousPosition = steam.PreviousPosition;
		}

		public MatchTeam(MatchTeam team)
			: base(team)
		{
			_previousGroup = team._previousGroup;
			_previousPosition = team._previousPosition;
			_games = team._games;
			_points = team._points;
			_pointsAgainst = team._pointsAgainst;
			_sets = team._sets;
			_setsAgainst = team._setsAgainst;
			_wins = team._wins;
			_loses = team._loses;
			_ties = team._ties;
			_technicalWins = team._technicalWins;
			_technicalLoses = team._technicalLoses;
		}

		internal override SportServices.Team Save()
		{
			SportServices.Team steam = new SportServices.Team();
			steam.TeamId = Id;
			steam.Games = _games;
			steam.Points = _points;
			steam.PointsAgainst = _pointsAgainst;
			steam.Sets = _sets;
			steam.SetsAgainst = _setsAgainst;
			steam.SmallPoints = _smallPoints;
			steam.SmallPointsAgainst = _smallPointsAgainst;
			steam.Score = Score;
			steam.Wins = _wins;
			steam.Loses = _loses;
			steam.Ties = _ties;
			steam.TechnicalWins = _technicalWins;
			steam.TechnicalLoses = _technicalLoses;
			steam.ResultPosition = Position;
			steam.PreviousGroup = _previousGroup;
			steam.PreviousPosition = _previousPosition;

			return steam;
		}

		public MatchGroup GetPreviousGroup()
		{
			if (_previousGroup == -1)
				return null;

			if (Group == null)
				return null;

			MatchPhase phase = Group.Phase;
			if ((phase == null) || (phase.Index == 0) || (phase.Championship == null) ||
				(phase.Championship.Phases == null))
			{
				return null;
			}

			MatchPhase prevPhase = phase.Championship.Phases[phase.Index - 1];
			if ((prevPhase == null) || (prevPhase.Groups == null))
				return null;

			return prevPhase.Groups[_previousGroup];
		}

		public override string Name
		{
			get
			{
				if (TeamEntity == null && _previousGroup != -1 && _previousPosition != -1)
				{
					MatchGroup prevGroup = GetPreviousGroup();
					if (prevGroup != null)
						return prevGroup.Name + " מיקום " + (_previousPosition + 1).ToString();
				}

				return base.Name;
			}
		}

		public override bool Equals(Team team)
		{
			MatchTeam mteam = team as MatchTeam;

			if (mteam == null)
				return false;

			if (TeamEntity != null || team.TeamEntity != null)
				return base.Equals(team);


			return Group.Phase == mteam.Group.Phase &&
				_previousGroup == mteam._previousGroup &&
				_previousPosition == mteam._previousPosition;
		}

		public override string ToString()
		{
			return Name;
		}
	}

	#endregion

	#region Team Comparers

	public class TeamPositionComparer : System.Collections.IComparer
	{
		#region IComparer Members

		public int Compare(object x, object y)
		{
			if ((x == null) && (y == null))
				return 0;
			if (x == null)
				return 1;
			if (y == null)
				return -1;
			bool blnIsTeam1 = (x is Team);
			bool blnIsTeam2 = (y is Team);
			if ((blnIsTeam1 == false) && (blnIsTeam2 == false))
				return 0;
			if (blnIsTeam1 == false)
				return 1;
			if (blnIsTeam2 == false)
				return -1;
			Team teamX = x as Team;
			Team teamY = y as Team;
			if (teamX.Position < teamY.Position)
				return -1;
			if (teamX.Position > teamY.Position)
				return 1;
			return 0;
		}

		#endregion
	}

	public class TeamScoreComparer : System.Collections.IComparer
	{
		#region IComparer Members

		public int Compare(object x, object y)
		{
			Team teamx = x as Team;
			Team teamy = y as Team;

			if (teamx.Score > teamy.Score)
				return -1;
			if (teamx.Score < teamy.Score)
				return 1;
			return 0;
		}

		#endregion
	}

	#endregion

	#region Team Rankers

	public abstract class TeamRanker
	{
		protected MatchGroup _group;
		public MatchGroup Group
		{
			get { return _group; }
		}

		protected bool _matchedTeams;
		public bool MatchedTeams
		{
			get { return _matchedTeams; }
		}

		public TeamRanker(MatchGroup group, bool matchedTeams)
		{
			_group = group;
			_matchedTeams = matchedTeams;
		}

		public static TeamRanker CreateRanker(MatchGroup group, Sport.Rulesets.Rules.RankingMethod method)
		{
			switch (method.MethodValue)
			{
				case (Sport.Rulesets.Rules.RankingMethodValue.Score):
					return new ScoreTeamRanker(group, method.MatchedTeams);
				case (Sport.Rulesets.Rules.RankingMethodValue.MostPoints):
					return new PointsTeamRanker(group, method.MatchedTeams);
				case (Sport.Rulesets.Rules.RankingMethodValue.PointDifference):
					return new PointsDifferenceRanker(group, method.MatchedTeams);
				case (Sport.Rulesets.Rules.RankingMethodValue.PointRatio):
					return new PointsRatioRanker(group, method.MatchedTeams);
				case (Sport.Rulesets.Rules.RankingMethodValue.MostSmallPoints):
					return new SmallPointsRanker(group, method.MatchedTeams);
			}

			return null;
		}

		// Called by RankTeams
		protected virtual void RankTeam(object[] ranks, int teamIndex, MatchTeam team)
		{
			throw new ChampionshipException("RankTeam not implemented");
		}

		// Called to give ranks to teams
		public virtual bool RankTeams(MatchTeam[] teams, object[] ranks, int first, int last)
		{
			if (!_matchedTeams)
			{
				for (int n = first; n <= last; n++)
				{
					RankTeam(ranks, n, teams[n]);
				}
			}
			else
			{
				for (int n = first; n <= last; n++)
					ranks[n] = null;

				RankMatches(teams, ranks, first, last);
			}

			return true;
		}

		// Called by RankMatches to add rank for a match
		protected virtual void RankMatch(object[] ranks, int teamA, int teamB, Match match)
		{
			throw new ChampionshipException("RankMatch not implemented");
		}

		// Called by RankTeams to rank selected teams matches
		protected void RankMatches(MatchTeam[] teams, object[] ranks, int first, int last)
		{
			foreach (Round round in Group.Rounds)
			{
				foreach (Cycle cycle in round.Cycles)
				{
					foreach (Match match in cycle.Matches)
					{
						MatchTeam teamA = match.MatchTeamA;
						MatchTeam teamB = match.MatchTeamB;
						int teamAIndex = -1;
						int teamBIndex = -1;
						for (int n = first; n <= last &&
							(teamAIndex == -1 || teamBIndex == -1); n++)
						{
							if (teams[n] == teamA)
								teamAIndex = n;
							else if (teams[n] == teamB)
								teamBIndex = n;
						}

						if (teamAIndex != -1 && teamBIndex != -1)
						{
							RankMatch(ranks, teamAIndex, teamBIndex, match);
						}
					}
				}
			}
		}
	}

	public class ScoreTeamRanker : TeamRanker
	{
		private Sport.Rulesets.Rules.GameScore gameScore;

		public ScoreTeamRanker(MatchGroup group, bool matchedTeams)
			: base(group, matchedTeams)
		{
			gameScore = null;
			if (Sport.Core.Session.Connected)
			{
				gameScore = Group.Phase.Championship.ChampionshipCategory.GetRule(
					typeof(Sport.Rulesets.Rules.GameScore)) as Sport.Rulesets.Rules.GameScore;
			}
			else
			{
				object rule = Sport.Rulesets.Ruleset.LoadOfflineRule(
					typeof(Sport.Rulesets.Rules.GameScore),
					this.Group.Phase.Championship.CategoryID, -1);
				if (rule != null)
					gameScore = (Sport.Rulesets.Rules.GameScore)rule;
			}
		}

		public override bool RankTeams(MatchTeam[] teams, object[] ranks, int first, int last)
		{
			if (gameScore == null)
				return false;

			return base.RankTeams(teams, ranks, first, last);
		}

		protected override void RankTeam(object[] ranks, int teamIndex, MatchTeam team)
		{
			ranks[teamIndex] = team.Score;
		}

		protected override void RankMatch(object[] ranks, int teamA, int teamB, Match match)
		{
			if (match.Outcome != Sport.Championships.MatchOutcome.None)
			{
				double teamAScore = ranks[teamA] == null ? 0 : (double)ranks[teamA];
				double teamBScore = ranks[teamB] == null ? 0 : (double)ranks[teamB];
				switch (match.Outcome)
				{
					case (Sport.Championships.MatchOutcome.WinA):
						teamAScore += gameScore.Win;
						teamBScore += gameScore.Lose;
						break;
					case (Sport.Championships.MatchOutcome.WinB):
						teamBScore += gameScore.Win;
						teamAScore += gameScore.Lose;
						break;
					case (Sport.Championships.MatchOutcome.Tie):
						teamAScore += gameScore.Tie;
						teamBScore += gameScore.Tie;
						break;
					case (Sport.Championships.MatchOutcome.TechnicalA):
						teamAScore += gameScore.TechnicalWin;
						teamBScore += gameScore.TechnicalLose;
						break;
					case (Sport.Championships.MatchOutcome.TechnicalB):
						teamBScore += gameScore.TechnicalWin;
						teamAScore += gameScore.TechnicalLose;
						break;
				}

				ranks[teamA] = teamAScore;
				ranks[teamB] = teamBScore;
			}
		}
	}

	public class PositionTeamRanker : TeamRanker
	{
		public PositionTeamRanker(MatchGroup group, bool matchedTeams)
			: base(group, matchedTeams)
		{
		}

		public override bool RankTeams(MatchTeam[] teams, object[] ranks, int first, int last)
		{
			for (int n = first; n <= last; n++)
			{
				ranks[n] = teams[n].Position;
			}

			return true;
		}
	}

	public class PointsTeamRanker : TeamRanker
	{
		public PointsTeamRanker(MatchGroup group, bool matchedTeams)
			: base(group, matchedTeams)
		{
		}

		protected override void RankTeam(object[] ranks, int teamIndex, MatchTeam team)
		{
			ranks[teamIndex] = team.Points;
		}

		protected override void RankMatch(object[] ranks, int teamA, int teamB, Match match)
		{
			if (match.Outcome != Sport.Championships.MatchOutcome.None)
			{
				double teamAPoints = ranks[teamA] == null ? 0 : (double)ranks[teamA];
				double teamBPoints = ranks[teamB] == null ? 0 : (double)ranks[teamB];
				teamAPoints += match.TeamAScore;
				teamBPoints += match.TeamBScore;
				ranks[teamA] = teamAPoints;
				ranks[teamB] = teamBPoints;
			}
		}

	}

	public class PointsDifferenceRanker : TeamRanker
	{
		public PointsDifferenceRanker(MatchGroup group, bool matchedTeams)
			: base(group, matchedTeams)
		{
		}

		protected override void RankTeam(object[] ranks, int teamIndex, MatchTeam team)
		{
			ranks[teamIndex] = team.Points - team.PointsAgainst;
		}

		protected override void RankMatch(object[] ranks, int teamA, int teamB, Match match)
		{
			if (match.Outcome != Sport.Championships.MatchOutcome.None)
			{
				double teamAPoints = ranks[teamA] == null ? 0 : (double)ranks[teamA];
				double teamBPoints = ranks[teamB] == null ? 0 : (double)ranks[teamB];
				teamAPoints += match.TeamAScore;
				teamBPoints += match.TeamBScore;
				teamAPoints -= match.TeamBScore;
				teamBPoints -= match.TeamAScore;
				ranks[teamA] = teamAPoints;
				ranks[teamB] = teamBPoints;
			}
		}

	}

	public class SmallPointsRanker : TeamRanker
	{
		public SmallPointsRanker(MatchGroup group, bool matchedTeams)
			: base(group, matchedTeams)
		{
		}

		protected override void RankTeam(object[] ranks, int teamIndex, MatchTeam team)
		{
			ranks[teamIndex] = team.SmallPoints;
		}

		protected override void RankMatch(object[] ranks, int teamA, int teamB, Match match)
		{
			if (match.Outcome != Sport.Championships.MatchOutcome.None)
			{
				int teamAPoints = ranks[teamA] == null ? 0 : (int)ranks[teamA];
				int teamBPoints = ranks[teamB] == null ? 0 : (int)ranks[teamB];
				int[] arrSmallPoints = match.CalcSmallPoints();
				teamAPoints += arrSmallPoints[0];
				teamBPoints += arrSmallPoints[1];
				ranks[teamA] = teamAPoints;
				ranks[teamB] = teamBPoints;
			}
		}
	}

	public class PointsRatioRanker : TeamRanker
	{
		private class TeamPoints : IComparable
		{
			public double Points;
			public double PointsAgainst;

			public float Ratio
			{
				get
				{
					if (PointsAgainst <= 0)
						return (float)(Points + 1);
					return (float)Points / (float)PointsAgainst;
				}
			}

			public TeamPoints(double points, double pointsAgainst)
			{
				Points = points;
				PointsAgainst = pointsAgainst;
			}

			#region IComparable Members

			public int CompareTo(object obj)
			{
				if (obj == null)
					return 1;
				float ro = ((TeamPoints)obj).Ratio;
				float rt = Ratio;

				if (ro > rt)
					return -1;
				if (ro < rt)
					return 1;

				return 0;
			}

			#endregion
		}

		public PointsRatioRanker(MatchGroup group, bool matchedTeams)
			: base(group, matchedTeams)
		{
		}

		protected override void RankTeam(object[] ranks, int teamIndex, MatchTeam team)
		{
			ranks[teamIndex] = new TeamPoints(team.Points, team.PointsAgainst);
		}

		protected override void RankMatch(object[] ranks, int teamA, int teamB, Match match)
		{
			if (match.Outcome != Sport.Championships.MatchOutcome.None)
			{
				if (ranks[teamA] == null)
				{
					ranks[teamA] = new TeamPoints(match.TeamAScore, match.TeamBScore);
				}
				else
				{
					((TeamPoints)ranks[teamA]).Points += match.TeamAScore;
					((TeamPoints)ranks[teamA]).PointsAgainst += match.TeamBScore;
				}
				if (ranks[teamB] == null)
				{
					ranks[teamB] = new TeamPoints(match.TeamBScore, match.TeamAScore);
				}
				else
				{
					((TeamPoints)ranks[teamB]).Points += match.TeamBScore;
					((TeamPoints)ranks[teamB]).PointsAgainst += match.TeamAScore;
				}
			}
		}
	}

	#endregion

	#region CompetitionTeam Class

	public class CompetitionTeam : Team
	{
		#region Properties

		public new CompetitionGroup Group
		{
			get { return ((CompetitionGroup)base.Group); }
		}

		public CompetitionTeam(Sport.Entities.Team team)
			: base(team)
		{
		}

		#endregion

		#region Team Information

		internal int _totalCounter;
		public int TotalCounter
		{
			get { return _totalCounter; }
		}

		internal int[] _counters;
		public int[] Counters
		{
			get
			{
				return _counters;
			}
		}

		internal int[] _scores;
		public int[] Scores
		{
			get
			{
				return _scores;
			}
		}

		public override void SetFields(Sport.Common.EquationVariables variables)
		{
			base.SetFields(variables);

			variables["C"] = _totalCounter;

			if (Counters != null)
			{
				for (int n = 0; n < Counters.Length; n++)
				{
					string c = (n + 1).ToString();
					variables["C" + c] = Counters[n];
					variables["S" + c] = Scores[n];
				}
			}
		}

		#endregion

		public override void Reset()
		{
			base.Reset();
			_totalCounter = 0;
			_counters = null;
			/*
			if (this.Name == "רבין באר שבע")
				Sport.Common.Tools.WriteToLog("team counters cleared (Reset)");
			*/
			_scores = null;
		}

		public override bool IsValidShirtRange()
		{
			if (this.TeamEntity == null)
				return false;
			int pNumFrom = this.TeamEntity.PlayerNumberFrom;
			int pNumTo = this.TeamEntity.PlayerNumberTo;
			if ((pNumFrom < 0) || (pNumTo < 0))
				return false;
			if (pNumTo < pNumFrom)
				return false;
			return true;
		}

		internal CompetitionTeam(SportServices.Team steam)
			: base(steam.TeamId == -1 ? null : new Sport.Entities.Team(steam.TeamId))
		{
			this._score = steam.Score;
			this._position = steam.ResultPosition;

			_counters = null;
			/*
			if (this.Name == "רבין באר שבע")
				Sport.Common.Tools.WriteToLog("team counters cleared (CompetitionTeam(SportServices.Team steam))");
			*/
			_scores = null;
		}

		public CompetitionTeam(CompetitionTeam team)
			: base(team)
		{
			this._score = team.Score;
			this._position = team.Position;

			_counters = null;
			/*
			if (this.Name == "רבין באר שבע")
				Sport.Common.Tools.WriteToLog("team counters cleared (CompetitionTeam(CompetitionTeam team))");
			*/
			_scores = null;
		}

		internal override SportServices.Team Save()
		{
			SportServices.Team steam = new SportServices.Team();
			steam.TeamId = Id;
			steam.Score = Score;
			steam.ResultPosition = Position;

			return steam;
		}

		public override bool Equals(Team team)
		{
			CompetitionTeam cteam = team as CompetitionTeam;

			if (cteam == null)
				return false;

			return base.Equals(team);
		}

		public override string ToString()
		{
			return Name;
		}

		public Sport.Documents.Data.Table[] GetFullReportTables(ref int teamTotalScore)
		{
			//got any team?
			if (this.TeamEntity == null)
				return null;

			//get team competitions:
			Hashtable tblTeamCompetitions = new Hashtable();
			int teamID = this.TeamEntity.Id;
			foreach (Competition competition in this.Group.Competitions)
			{
				//get competitors:
				ArrayList arrCompetitors = competition.GetTeamCompetitors(teamID);

				if (arrCompetitors.Count > 0)
				{
					//build list if does not exists:
					if (tblTeamCompetitions[competition] == null)
						tblTeamCompetitions[competition] = new ArrayList();

					//add competitor to the list:
					tblTeamCompetitions[competition] = arrCompetitors;
				}
			} //end loop over competitions

			//got anything?
			if (tblTeamCompetitions.Count == 0)
				return null;


			Hashtable tblCompetitionBestResults = this.Group.GetBestResults(teamID);

			//build data tables.
			int colCount = 10;
			string[] captions = new string[] {"חזה", "שם משפחה", "שם פרטי", 
											   "כיתה", "מקצה", "מסלול", "תוצאה", "ניקוד", "לחישוב?", "אישי"};
			double[] widths = new double[] {0.08, 0.14, 0.14, 0.08, 0.08, 0.09, 
											 0.11, 0.1, 0.1, 0.08};
			Sport.Documents.Data.Table[] tables =
				new Sport.Documents.Data.Table[tblTeamCompetitions.Count];
			int index = 0;
			teamTotalScore = 0;
			foreach (Competition competition in tblTeamCompetitions.Keys)
			{
				//score is rank?
				bool blnScoreIsRank = competition.ScoreIsRank();

				//best results:
				int resultsCount = 0;
				if (tblCompetitionBestResults[competition.Index] != null)
					resultsCount = (int)tblCompetitionBestResults[competition.Index];
				else
					resultsCount = competition.GetBestResults();

				//create new table:
				Sport.Documents.Data.Table table = new Sport.Documents.Data.Table();
				table.Caption = "פירוט תוצאות למקצוע " + competition.SportField.Name;

				//assign headers:
				table.Headers = new Sport.Documents.Data.Column[colCount];
				for (int i = 0; i < captions.Length; i++)
				{
					table.Headers[i] = new Sport.Documents.Data.Column(
						captions[i], widths[i], false);
				}

				//get competitors:
				ArrayList arrCompetitors = (ArrayList)tblTeamCompetitions[competition];

				//sort by score:
				arrCompetitors.Sort(new CompetitorsScoreComparer(blnScoreIsRank));

				//create rows:
				table.Rows = new Sport.Documents.Data.Row[arrCompetitors.Count + 1];

				//add new row for each competitor:
				int curScore = 0;
				Sport.Documents.Data.Row row;
				for (int i = 0; i < arrCompetitors.Count; i++)
				{
					//get data:
					object oComp = arrCompetitors[i];
					int score = 0;
					if (oComp is Sport.Championships.Competitor)
					{
						Sport.Championships.Competitor competitor =
							(Sport.Championships.Competitor)oComp;
						score = competitor.Score;
					}
					else if (oComp is Sport.Entities.OfflinePlayer)
					{
						Sport.Entities.OfflinePlayer oPlayer =
							(Sport.Entities.OfflinePlayer)oComp;
						score = oPlayer.Score;
					}

					//add to score:
					if (i < resultsCount)
						curScore += score;

					//build new row
					row = new Sport.Documents.Data.Row();

					//create cells and assign row
					row.Cells = CreateCompetitorCells(oComp, i, colCount, competition, resultsCount);
					table.Rows[i] = row;
				} //end loop over competitors

				//add row for the total score:
				row = new Sport.Documents.Data.Row();
				row.AddCells(new string[] {"", "", "", "", "", "", 
											  "מצטבר", curScore.ToString(), "", ""}, false);
				table.Rows[table.Rows.Length - 1] = row;

				//add to total team score:
				teamTotalScore += curScore;

				//assign table:
				tables[index] = table;
				index++;
			} //end loop over competitions

			//done.
			return tables;
		}

		public Sport.Documents.Data.Table GetCompetitorsTable()
		{
			//got any team?
			if (this.TeamEntity == null)
				return Sport.Documents.Data.Table.Empty;

			//get team competitions:
			Hashtable tblPlayerCompetitions = new Hashtable();
			int teamID = this.TeamEntity.Id;
			List<string> arrCaptions = new List<string>();
			arrCaptions.Add("שם");
			arrCaptions.Add("חזה");
			foreach (Competition competition in this.Group.Competitions)
			{
				string strSportField = competition.SportField.Name;
				ArrayList arrCompetitors = competition.GetTeamCompetitors(teamID, false);
				int playersWithValidId = 0;
				foreach (object oCompetitor in arrCompetitors)
				{
					List<string> idNumbers = new List<string>();
					if (oCompetitor is Sport.Championships.Competitor)
					{
						Sport.Championships.Competitor competitor = (Sport.Championships.Competitor)oCompetitor;
						if (competitor.SharedResultNumbers != null && competitor.SharedResultNumbers.Length > 1)
						{
							if (competitor.GroupTeam != null && competitor.GroupTeam.TeamEntity != null)
							{
								competitor.SharedResultNumbers.ToList().ForEach(curNumber =>
								{
									Sport.Entities.Player curPlayer = competitor.GroupTeam.TeamEntity.GetPlayerByNumber(curNumber);
									if (curPlayer != null)
										idNumbers.Add(curPlayer.IdNumber);
								});
							}
						}
						else if (competitor.Player != null && competitor.Player.PlayerEntity != null)
						{
							idNumbers.Add(competitor.Player.PlayerEntity.Student.IdNumber);
						}
					}
					else if (oCompetitor is Sport.Entities.OfflinePlayer)
					{
						Sport.Entities.OfflinePlayer offlinePlayer = (Sport.Entities.OfflinePlayer)oCompetitor;
						if (offlinePlayer.Student != null)
							idNumbers.Add(offlinePlayer.Student.IdNumber);
						else if (offlinePlayer.OfflineStudent != null)
							idNumbers.Add(offlinePlayer.OfflineStudent.IdNumber);
					}
					idNumbers.ForEach(strIdNumber =>
					{
						if (tblPlayerCompetitions[strIdNumber] == null)
							tblPlayerCompetitions[strIdNumber] = new ArrayList();
						(tblPlayerCompetitions[strIdNumber] as ArrayList).Add(strSportField);
						playersWithValidId++;
					});
				}
				if (playersWithValidId > 0)
					arrCaptions.Add(strSportField);
			} //end loop over competitions

			//build data tables.
			int colCount = arrCaptions.Count;
			string[] captions = arrCaptions.ToArray();
			int[] widths = new int[colCount];
			widths[0] = 125;
			widths[1] = 50;
			for (int i = 2; i < widths.Length; i++)
				widths[i] = 65;

			//create new table:
			Sport.Documents.Data.Table table = new Sport.Documents.Data.Table();

			//caption
			int categoryID = this.Group.Phase.Championship.CategoryID;
			Sport.Entities.ChampionshipCategory category = new Sport.Entities.ChampionshipCategory(categoryID);
			table.Caption = "רישום לתחרות " + category.Championship.Name + " " + category.Name;

			//assign headers:
			table.Headers = new Sport.Documents.Data.Column[colCount];
			for (int i = 0; i < captions.Length; i++)
			{
				table.Headers[i] = new Sport.Documents.Data.Column(captions[i], widths[i], true);
			}

			//get players
			ArrayList arrPlayers = new ArrayList(this.TeamEntity.GetPlayers());

			//sort by number:
			arrPlayers.Sort(new PlayerNumberComparer());

			//create rows:
			table.Rows = new Sport.Documents.Data.Row[arrPlayers.Count];

			//add new row for each competitor:
			Sport.Documents.Data.Row row;
			for (int i = 0; i < arrPlayers.Count; i++)
			{
				//get data:
				Sport.Entities.Player oPlayer = (Sport.Entities.Player)arrPlayers[i];

				//build new row
				row = new Sport.Documents.Data.Row();

				//create cells and assign row
				row.Cells = CreateCompetitorCells(oPlayer, widths, captions, tblPlayerCompetitions);
				table.Rows[i] = row;
			} //end loop over competitors

			//done.
			return table;
		}

		private Sport.Documents.Data.Cell[] CreateCompetitorCells(Sport.Entities.Player player, int[] widths,
			string[] captions, Hashtable tblPlayerCompetitions)
		{
			//initialize result:
			Sport.Documents.Data.Cell[] cells = new Sport.Documents.Data.Cell[widths.Length];

			//get competitions list:
			string[] arrPlayerSportFields = null;
			if (tblPlayerCompetitions[player.Student.IdNumber] != null)
			{
				arrPlayerSportFields = (string[])(tblPlayerCompetitions[player.Student.IdNumber] as ArrayList).ToArray(typeof(string));
			}

			//add cells:
			cells[0] = new Sport.Documents.Data.Cell(player.Name, widths[0], true);
			cells[1] = new Sport.Documents.Data.Cell(player.Number.ToString(), widths[1], true);
			for (int i = 2; i < captions.Length; i++)
			{
				string strSportField = captions[i];
				string strCellText = "";
				if (arrPlayerSportFields != null)
				{
					if (Array.IndexOf(arrPlayerSportFields, strSportField) >= 0)
					{
						strCellText = "משתתף";
					}
				}
				cells[i] = new Sport.Documents.Data.Cell(strCellText, widths[i], true);
			}

			//done.
			return cells;
		}

		private Sport.Documents.Data.Cell[] CreateCompetitorCells(object oComp,
			int index, int colCount, Competition competition, int resultsCount)
		{
			//initialize result:
			Sport.Documents.Data.Cell[] cells = new Sport.Documents.Data.Cell[colCount];

			//add cells:
			for (int i = 0; i < colCount; i++)
				cells[i] = CreateCompetitorCell(oComp, i, index, competition, resultsCount);

			//done.
			return cells;
		}

		Sport.Types.GradeTypeLookup _gradeLookup = null;
		private Sport.Documents.Data.Cell CreateCompetitorCell(object oComp,
			int field, int index, Competition competition, int resultsCount)
		{
			//build text:
			string strCellText = "";

			//create lookup?
			if (_gradeLookup == null)
				_gradeLookup = new Sport.Types.GradeTypeLookup(true);

			//get data:
			Sport.Rulesets.Rules.ResultType resultType = competition.ResultType;
			int shirtNumber = -1;
			string strFirstName = "";
			string strLastName = "";
			int grade = 0;
			int heat = -1;
			int position = -1;
			int curResult = 0;
			int curScore = 0;
			int rank = -1;
			Sport.Entities.Student student = null;
			if (oComp is Sport.Championships.Competitor)
			{
				Sport.Championships.Competitor competitor =
					(Sport.Championships.Competitor)oComp;
				shirtNumber = competitor.PlayerNumber;
				if ((competitor.Player != null) && (competitor.Player.PlayerEntity != null))
					student = competitor.Player.PlayerEntity.Student;
				heat = competitor.Heat;
				position = competitor.Position;
				curResult = competitor.Result;
				curScore = competitor.Score;
				rank = competitor.ResultPosition;
			}
			else if (oComp is Sport.Entities.OfflinePlayer)
			{
				Sport.Entities.OfflinePlayer oPlayer =
					(Sport.Entities.OfflinePlayer)oComp;
				shirtNumber = oPlayer.ShirtNumber;
				if (oPlayer.Student != null)
					student = oPlayer.Student;
				else if (oPlayer.OfflineStudent != null)
				{
					strFirstName = oPlayer.OfflineStudent.FirstName;
					strLastName = oPlayer.OfflineStudent.LastName;
					grade = oPlayer.OfflineStudent.Grade;
				}
				curResult = oPlayer.Result;
				curScore = oPlayer.Score;
				rank = oPlayer.Rank;
			}
			if (student != null)
			{
				strFirstName = student.FirstName;
				strLastName = student.LastName;
				grade = student.Grade;
			}

			//decide text:
			switch (field)
			{
				case 0:
					//shirt number
					strCellText = shirtNumber.ToString();
					break;
				case 1:
				case 2:
				case 3:
					//last name, first name or grade.
					if ((strFirstName != null) && (strFirstName.Length > 0))
					{
						switch (field)
						{
							case 1:
								//last name:
								strCellText = strLastName;
								break;
							case 2:
								//first name:
								strCellText = strFirstName;
								break;
							case 3:
								//grade:
								strCellText = _gradeLookup.Lookup(grade);
								break;
						}
					}
					break;
				case 4:
					//heat:
					if (heat >= 0)
					{
						strCellText = (heat + 1).ToString();
					}
					break;
				case 5:
					//lane:
					strCellText = (position + 1).ToString();
					break;
				case 6:
					//result:
					if ((resultType != null) && (curResult > 0))
					{
						strCellText = resultType.FormatResult(curResult);
					}
					break;
				case 7:
					//score:
					strCellText = curScore.ToString();
					break;
				case 8:
					//result counted in team score?
					strCellText = ((index + 1) <= resultsCount) ? "כן" : "לא";
					break;
				case 9:
					//personal rank.
					strCellText = (rank + 1).ToString();
					break;
			}

			//build new cell:
			Sport.Documents.Data.Cell cell =
				new Sport.Documents.Data.Cell(strCellText, false);

			//border?
			if (index == 0)
			{
				cell.ShowBorder = true;
				cell.Borders = Sport.Documents.Borders.Top;
			}

			//done.
			return cell;
		}
	}

	#endregion

}
