using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Web;
using System.Web.Services;
using Sport.Core;
using System.Linq;
using ISF.DataLayer;
using System.Collections.Generic;

namespace SportServices
{
	[WebService(Namespace = "http://www.mir.co.il/")]
	public class ChampionshipService : System.Web.Services.WebService
	{
		public ChampionshipService()
		{
			InitializeComponent();
		}

		#region Component Designer generated code
		//Required by the Web Services Designer 
		private IContainer components = null;

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#endregion

		#region Championships Structure
		public class Match
		{
			public int Number;
			public int Tournament;
			public int TeamAPos;
			public int TeamBPos;
			public int RelativeTeamA;
			public int RelativeTeamB;
			public DateTime Time;
			public int Facility;
			public int Court;
			public double TeamAScore;
			public double TeamBScore;
			public int Outcome;
			public string PartsResult;
			public int[] Functionaries;
			public int RefereeCount;
			public string CustomTeamA;
			public string CustomTeamB;
			public DateTime DateChangedDate;
		}

		public class Tournament
		{
			public int Number;
			public DateTime Time;
			public int Facility;
			public int Court;
		}

		public class Cycle
		{
			public string Name;
			public Tournament[] Tournaments;
			public Match[] Matches;
		}

		public class Round
		{
			public string Name;
			public Cycle[] Cycles;
			public int Status;
		}

		public class Team
		{
			public int PreviousGroup;
			public int PreviousPosition;
			public int TeamId;
			public int Games;
			public double Points;
			public double PointsAgainst;
			public int Sets;
			public int SetsAgainst;
			public int SmallPoints;
			public int SmallPointsAgainst;
			public int Wins;
			public int Loses;
			public int Ties;
			public int TechnicalWins;
			public int TechnicalLoses;
			public double Score;
			public int ResultPosition;
		}

		public class Competitor
		{
			public int PlayerNumber;
			public int Heat;
			public int Position;
			public int ResultPosition;
			public int Result;
			public int Score;
			public int CustomPosition;
			public int[] SharedResultNumbers;
			public int LastResultDisqualifications;
			public int TotalDisqualifications;
			public string Wind;
		}

		public class Heat
		{
			public DateTime Time;
			public int Facility;
			public int Court;
		}

		public class Competition
		{
			public int SportField;
			public DateTime Time;
			public int Facility;
			public int Court;
			public Competitor[] Competitors;
			public Heat[] Heats;
			public int championshipCategory;
			public int LaneCount;

			public Competition()
			{
				this.SportField = -1;
				this.Time = DateTime.MinValue;
				this.Facility = -1;
				this.Court = -1;
				this.Competitors = null;
				this.Heats = null;
				this.championshipCategory = -1;
				this.LaneCount = 1;
			}

			public Competition(SimpleRow dataRow)
				: this()
			{
				this.championshipCategory = (int)dataRow["CHAMPIONSHIP_CATEGORY_ID"];
				if (dataRow.ContainsField("SPORT_FIELD_ID"))
					this.SportField = dataRow.GetIntOrDefault("SPORT_FIELD_ID", -1);
				if (dataRow.ContainsField("COURT_ID"))
					this.Court = dataRow.GetIntOrDefault("COURT_ID", -1);
				if (dataRow.ContainsField("FACILITY_ID"))
					this.Facility = dataRow.GetIntOrDefault("FACILITY_ID", -1);
				if (dataRow.ContainsField("TIME"))
					this.Time = (DateTime)dataRow["TIME"];
				if (dataRow.ContainsField("LANE_COUNT"))
					this.LaneCount = (int)dataRow["LANE_COUNT"];
			}
		}

		public class Group
		{
			public string Name;
			public int Status;
			public Team[] Teams;
			public Round[] Rounds;
			public Competition[] Competitions;
		}

		public class PhaseDefinition
		{
			public int RuleType;
			public string Definition;
			public string Value;
		}

		public class Phase
		{
			public string Name;
			public Group[] Groups;
			public PhaseDefinition[] Definitions;
			public int Status;
		}

		public class TeamCompetitionCompetitors
		{
			public int ChampionshipCategoryId { get; set; }
			public int TeamId { get; set; }
			public int Phase { get; set; }
			public int Group { get; set; }
			public string SportFieldType { get; set; }
			public int Competitors { get; set; }
		}

		#endregion

		#region Championship Read
		private void ReadFunctionaries(int ccid, int iphase, int igroup, int iround, int icycle, int imatch, Match match)
		{
			string strSQL = "SELECT FUNCTIONARY_ID, ROLE " +
				"FROM CHAMPIONSHIP_MATCH_FUNCTIONARIES " +
				"WHERE CHAMPIONSHIP_CATEGORY_ID = @0 AND " +
				" PHASE = @1 AND " +
				" NGROUP = @2 AND " +
				" ROUND = @3 AND " +
				" CYCLE = @4 AND " +
				" MATCH = @5 " +
				"ORDER BY ROLE ";

			//read results:
			SimpleTable table = DB.Instance.GetDataBySQL(strSQL,
				new SimpleParameter("@0", ccid),
				new SimpleParameter("@1", iphase),
				new SimpleParameter("@2", igroup),
				new SimpleParameter("@3", iround),
				new SimpleParameter("@4", icycle),
				new SimpleParameter("@5", imatch));
			List<int> arrFunctionaries = new List<int>();
			table.Rows.ForEach(row =>
			{
				arrFunctionaries.Add((int)row["FUNCTIONARY_ID"]);
			});

			match.Functionaries = arrFunctionaries.ToArray();
		}

		private void ReadMatches(int ccid, int iphase, int igroup, int iround, int icycle, Cycle cycle)
		{
			string strSQL = "SELECT TEAM_A, TEAM_B, TIME, FACILITY_ID, COURT_ID, TEAM_A_SCORE, " +
				"TEAM_B_SCORE, RESULT, PARTS_RESULT, MATCH_NUMBER, RELATIVE_TEAM_A, " +
				"RELATIVE_TEAM_B, TOURNAMENT, REFEREE_COUNT, CUSTOM_TEAM_A, " +
				"CUSTOM_TEAM_B, DATE_CHANGED_DATE " +
				"FROM CHAMPIONSHIP_MATCHES " +
				"WHERE CHAMPIONSHIP_CATEGORY_ID = @0 AND " +
				" PHASE = @1 AND " +
				" NGROUP = @2 AND " +
				" ROUND = @3 AND " +
				" CYCLE = @4 " +
				"ORDER BY MATCH ";

			//read results:
			SimpleTable table = DB.Instance.GetDataBySQL(strSQL,
				new SimpleParameter("@0", ccid),
				new SimpleParameter("@1", iphase),
				new SimpleParameter("@2", igroup),
				new SimpleParameter("@3", iround),
				new SimpleParameter("@4", icycle));
			List<Match> matches = new List<Match>();
			table.Rows.ForEach(row =>
			{
				Match match = new Match();
				match.Number = (int)row[9];
				match.Tournament = row.GetIntOrDefault(12, -1);
				match.RelativeTeamA = (int)row[10];
				match.RelativeTeamB = (int)row[11];
				match.TeamAPos = row.GetIntOrDefault(0, -1);
				match.TeamBPos = row.GetIntOrDefault(1, -1);
				match.Time = (DateTime)row[2];
				match.Facility = row.GetIntOrDefault(3, -1);
				match.Court = row.GetIntOrDefault(4, -1);
				match.TeamAScore = Double.Parse(row["TEAM_A_SCORE"].ToString());
				match.TeamBScore = Double.Parse(row["TEAM_B_SCORE"].ToString());
				match.Outcome = row.GetIntOrDefault(7, -1);;
				match.PartsResult = row[8].ToString();
				match.RefereeCount = (int)row[13];
				match.CustomTeamA = row[14].ToString();
				match.CustomTeamB = row[15].ToString();
				match.DateChangedDate = (DateTime)row[16];
				matches.Add(match);
			});

			cycle.Matches = matches.ToArray();
			for (int n = 0; n < cycle.Matches.Length; n++)
				ReadFunctionaries(ccid, iphase, igroup, iround, icycle, n, cycle.Matches[n]);
		}

		private void ReadTournaments(int ccid, int iphase, int igroup, int iround, int icycle, Cycle cycle)
		{
			string strSQL = "SELECT NUMBER, TIME, FACILITY_ID, COURT_ID " +
				"FROM CHAMPIONSHIP_TOURNAMENTS " +
				"WHERE CHAMPIONSHIP_CATEGORY_ID = @0 AND " +
					" PHASE = @1 AND " +
					" NGROUP = @2 AND " +
					" ROUND = @3 AND " +
					" CYCLE = @4 " +
				"ORDER BY TOURNAMENT";

			//read results:
			SimpleTable table = DB.Instance.GetDataBySQL(strSQL,
				new SimpleParameter("@0", ccid),
				new SimpleParameter("@1", iphase),
				new SimpleParameter("@2", igroup),
				new SimpleParameter("@3", iround),
				new SimpleParameter("@4", icycle));
			List<Tournament> tournaments = new List<Tournament>();
			table.Rows.ForEach(row =>
			{
				Tournament tournament = new Tournament();
				tournament.Number = (int)row[0];
				tournament.Time = (DateTime)row[1];
				tournament.Facility = row.GetIntOrDefault(2, -1);
				tournament.Court = row.GetIntOrDefault(3, -1);
				tournaments.Add(tournament);
			});

			cycle.Tournaments = tournaments.ToArray();
		}

		private void ReadCycles(int ccid, int iphase, int igroup, int iround, Round round)
		{
			string strSQL = "SELECT CYCLE_NAME " +
				"FROM CHAMPIONSHIP_CYCLES " +
				"WHERE CHAMPIONSHIP_CATEGORY_ID = @0 AND " +
				" PHASE = @1 AND " +
				" NGROUP = @2 AND " +
				" ROUND = @3 " +
				"ORDER BY CYCLE";

			//read results:
			SimpleTable table = DB.Instance.GetDataBySQL(strSQL,
				new SimpleParameter("@0", ccid),
				new SimpleParameter("@1", iphase),
				new SimpleParameter("@2", igroup),
				new SimpleParameter("@3", iround));
			List<Cycle> cycles = new List<Cycle>();
			table.Rows.ForEach(row =>
			{
				Cycle cycle = new Cycle();
				cycle.Name = row[0].ToString();
				cycles.Add(cycle);
			});

			round.Cycles = cycles.ToArray();
			for (int n = 0; n < round.Cycles.Length; n++)
			{
				ReadTournaments(ccid, iphase, igroup, iround, n, round.Cycles[n]);
				ReadMatches(ccid, iphase, igroup, iround, n, round.Cycles[n]);
			}
		}

		private void ReadRounds(int ccid, int iphase, int igroup, Group group)
		{
			string strSQL = "SELECT ROUND_NAME, STATUS " +
				"FROM CHAMPIONSHIP_ROUNDS " +
				"WHERE CHAMPIONSHIP_CATEGORY_ID = @0 AND " +
					" PHASE = @1 AND " +
					" NGROUP = @2 " +
				"ORDER BY ROUND";

			//read results:
			SimpleTable table = DB.Instance.GetDataBySQL(strSQL,
				new SimpleParameter("@0", ccid),
				new SimpleParameter("@1", iphase),
				new SimpleParameter("@2", igroup));
			List<Round> rounds = new List<Round>();
			table.Rows.ForEach(row =>
			{
				Round round = new Round();
				round.Name = row[0].ToString();
				round.Status = (int)row[1];
				rounds.Add(round);
			});

			group.Rounds = rounds.ToArray();
			for (int n = 0; n < group.Rounds.Length; n++)
				ReadCycles(ccid, iphase, igroup, n, group.Rounds[n]);
		}

		private void ReadTeams(int ccid, int iphase, int igroup, Group group)
		{
			string strSQL = "SELECT PREVIOUS_GROUP, PREVIOUS_POSITION, TEAM_ID, GAMES, POINTS, " +
				" POINTS_AGAINST, SMALL_POINTS, SMALL_POINTS_AGAINST,  " +
				" SCORE, RESULT_POSITION, SETS, SETS_AGAINST " +
				"FROM CHAMPIONSHIP_GROUP_TEAMS " +
				"WHERE CHAMPIONSHIP_CATEGORY_ID = @0 AND " +
				" PHASE = @1 AND " +
				" NGROUP = @2 " +
				"ORDER BY POSITION";

			//read results:
			SimpleTable table = DB.Instance.GetDataBySQL(strSQL,
				new SimpleParameter("@0", ccid),
				new SimpleParameter("@1", iphase),
				new SimpleParameter("@2", igroup));
			List<Team> teams = new List<Team>();
			table.Rows.ForEach(row =>
			{
				Team team = new Team();
				team.PreviousGroup = row.GetIntOrDefault("PREVIOUS_GROUP", -1);
				team.PreviousPosition = row.GetIntOrDefault("PREVIOUS_POSITION", -1);
				team.TeamId = row.GetIntOrDefault("TEAM_ID", -1);
				team.Games = (int)row["GAMES"];
				team.Points = (int)row["POINTS"];
				team.PointsAgainst = (int)row["POINTS_AGAINST"];
				team.Sets = row.GetIntOrDefault("SETS", 0);
				team.SetsAgainst = row.GetIntOrDefault("SETS_AGAINST", 0);
				team.SmallPoints = (int)row["SMALL_POINTS"];
				team.SmallPointsAgainst = (int)row["SMALL_POINTS_AGAINST"];
				team.Score = Double.Parse(row["SCORE"].ToString());
				team.ResultPosition = (int)row["RESULT_POSITION"];
				teams.Add(team);
			});

			group.Teams = teams.ToArray();
		}

		private void ReadHeats(int ccid, int iphase, int igroup, int icompetition, Competition competition)
		{
			string strSQL = "SELECT TIME, FACILITY_ID, COURT_ID " +
				"FROM CHAMPIONSHIP_COMPETITION_HEATS " +
				"WHERE CHAMPIONSHIP_CATEGORY_ID = @0 AND " +
					" PHASE = @1 AND " +
					" NGROUP = @2 AND " +
					" COMPETITION = @3 " +
				"ORDER BY HEAT";

			//read results:
			SimpleTable table = DB.Instance.GetDataBySQL(strSQL,
				new SimpleParameter("@0", ccid),
				new SimpleParameter("@1", iphase),
				new SimpleParameter("@2", igroup),
				new SimpleParameter("@3", icompetition));
			List<Heat> heats = new List<Heat>();
			table.Rows.ForEach(row =>
			{
				Heat heat = new Heat();
				heat.Time = (DateTime)row[0];
				heat.Facility = row.GetIntOrDefault(1, -1);
				heat.Court = row.GetIntOrDefault(2, -1);
				heats.Add(heat);
			});
			
			competition.Heats = heats.ToArray();
		}

		private void ReadCompetitors(int ccid, int iphase, int igroup, int icompetition, Competition competition)
		{
			string strSQL = "SELECT PLAYER_NUMBER, HEAT, POSITION, RESULT_POSITION, RESULT, SCORE, CUSTOM_POSITION, " +
				"	SharedResultNumbers, LastResultDisqualifications, TotalDisqualifications, Wind " +
				"FROM CHAMPIONSHIP_COMPETITION_COMPETITORS " +
				"WHERE CHAMPIONSHIP_CATEGORY_ID = @0 AND " +
					" PHASE = @1 AND " +
					" NGROUP = @2 AND " +
					" COMPETITION = @3 " +
				"ORDER BY COMPETITOR";

			//read results:
			SimpleTable table = DB.Instance.GetDataBySQL(strSQL,
				new SimpleParameter("@0", ccid),
				new SimpleParameter("@1", iphase),
				new SimpleParameter("@2", igroup),
				new SimpleParameter("@3", icompetition));
			List<Competitor> competitors = new List<Competitor>();
			table.Rows.ForEach(row =>
			{
				Competitor competitor = new Competitor();
				competitor.PlayerNumber = (int)row[0];
				competitor.Heat = row.GetIntOrDefault(1, -1);
				competitor.Position = row.GetIntOrDefault(2, -1);
				competitor.ResultPosition = row.GetIntOrDefault(3, -1);
				competitor.Result = row.GetIntOrDefault(4, -1);
				competitor.Score = row.GetIntOrDefault(5, -1);
				competitor.CustomPosition = row.GetIntOrDefault(6, -1);
				competitor.SharedResultNumbers = Sport.Common.Tools.ToIntArray(row[7] + "", ',', true);
				competitor.LastResultDisqualifications = row.GetIntOrDefault(8, 0);
				competitor.TotalDisqualifications = row.GetIntOrDefault(9, 0);
				competitor.Wind = row[10] + "";
				competitors.Add(competitor);
			});

			competition.Competitors = competitors.ToArray();
		}

		private void ReadCompetitions(int ccid, int iphase, int igroup, Group group)
		{
			string strSQL = "SELECT SPORT_FIELD_ID, TIME, FACILITY_ID, COURT_ID, LANE_COUNT " +
				"FROM CHAMPIONSHIP_COMPETITIONS " +
				"WHERE CHAMPIONSHIP_CATEGORY_ID = @0 AND " +
					" PHASE = @1 AND " +
					" NGROUP = @2 " +
				"ORDER BY COMPETITION";

			//read results:
			SimpleTable table = DB.Instance.GetDataBySQL(strSQL,
				new SimpleParameter("@0", ccid),
				new SimpleParameter("@1", iphase),
				new SimpleParameter("@2", igroup));
			List<Competition> competitions = new List<Competition>();
			table.Rows.ForEach(row =>
			{
				Competition competition = new Competition();
				competition.SportField = row.GetIntOrDefault("SPORT_FIELD_ID", -1);
				competition.Time = (DateTime)row["TIME"];
				competition.Facility = row.GetIntOrDefault("FACILITY_ID", -1);
				competition.Court = row.GetIntOrDefault("COURT_ID", -1);
				competition.LaneCount = (int)row["LANE_COUNT"];
				competitions.Add(competition);
			});
			
			group.Competitions = competitions.ToArray();
			for (int n = 0; n < group.Competitions.Length; n++)
			{
				ReadHeats(ccid, iphase, igroup, n, group.Competitions[n]);
				ReadCompetitors(ccid, iphase, igroup, n, group.Competitions[n]);
			}
		}

		private void ReadGroups(int ccid, int iphase, Phase phase)
		{
			string strSQL = "SELECT GROUP_NAME, STATUS " +
				"FROM CHAMPIONSHIP_GROUPS " +
				"WHERE CHAMPIONSHIP_CATEGORY_ID = @0 AND PHASE = @1 " +
				"ORDER BY NGROUP";

			//read results:
			SimpleTable table = DB.Instance.GetDataBySQL(strSQL,
				new SimpleParameter("@0", ccid),
				new SimpleParameter("@1", iphase));
			List<Group> groups = new List<Group>();
			table.Rows.ForEach(row =>
			{
				Group group = new Group();
				group.Name = row[0].ToString();
				group.Status = (int)row[1];
				groups.Add(group);
			});

			phase.Groups = groups.ToArray();
			for (int n = 0; n < phase.Groups.Length; n++)
			{
				ReadTeams(ccid, iphase, n, phase.Groups[n]);
				ReadRounds(ccid, iphase, n, phase.Groups[n]);
				ReadCompetitions(ccid, iphase, n, phase.Groups[n]);
			}
		}

		private void ReadDefinitions(int ccid, int iphase, Phase phase)
		{
			string strSQL = "SELECT RULE_TYPE_ID, DEFINITION, VALUE " +
				"FROM CHAMPIONSHIP_PHASE_DEFINITIONS " +
				"WHERE CHAMPIONSHIP_CATEGORY_ID = @0 AND PHASE = @1 " +
				"ORDER BY DEFINITION";

			//read results:
			SimpleTable table = DB.Instance.GetDataBySQL(strSQL,
				new SimpleParameter("@0", ccid),
				new SimpleParameter("@1", iphase));
			List<PhaseDefinition> definitions = new List<PhaseDefinition>();
			table.Rows.ForEach(row =>
			{
				PhaseDefinition definition = new PhaseDefinition();
				definition.RuleType = (int)row[0];
				definition.Definition = row[1].ToString();
				definition.Value = row[2].ToString();
				definitions.Add(definition);
			});

			phase.Definitions = definitions.ToArray();
		}

		[WebMethod]
		public TeamCompetitionCompetitors[] ReadCompetitionCompetitors(int championshipCategoryId)
		{
			string strSQL = "Select ccc.PHASE, ccc.NGROUP, ccc.COMPETITION, ccc.COMPETITOR, ccc.PLAYER_NUMBER, " + 
					"	dbo.ExtractPlayerTeam(ccc.PLAYER_NUMBER, ccc.CHAMPIONSHIP_CATEGORY_ID) As TEAM_ID, " + 
					"	dbo.GetActualSportFieldTypeName(sft.SPORT_FIELD_TYPE_NAME, sf.SPORT_FIELD_NAME) As SPORT_FIELD_TYPE_NAME " + 
					"From CHAMPIONSHIP_COMPETITION_COMPETITORS ccc Inner Join CHAMPIONSHIP_COMPETITIONS cc On ccc.CHAMPIONSHIP_CATEGORY_ID=cc.CHAMPIONSHIP_CATEGORY_ID And ccc.PHASE=cc.PHASE And ccc.NGROUP=cc.NGROUP And ccc.COMPETITION=cc.COMPETITION " + 
					"	Inner Join SPORT_FIELDS sf On cc.SPORT_FIELD_ID=sf.SPORT_FIELD_ID " + 
					"	Inner Join SPORT_FIELD_TYPES sft On sf.SPORT_FIELD_TYPE_ID=sft.SPORT_FIELD_TYPE_ID " + 
					"Where ccc.DATE_DELETED Is Null And cc.DATE_DELETED Is Null And ccc.PLAYER_NUMBER Is Not Null " + 
					"	And ccc.CHAMPIONSHIP_CATEGORY_ID=@category";

			//read results:
			SimpleTable table = DB.Instance.GetDataBySQL(strSQL,
				new SimpleParameter("@category", championshipCategoryId));
			List<TeamCompetitionCompetitors> teamCompetitionCompetitors = new List<TeamCompetitionCompetitors>();
			table.Rows.ForEach(row =>
			{
				int teamId = (int)row["TEAM_ID"];
				int phase = (int)row["PHASE"];
				int group = (int)row["NGROUP"];
				string sportFieldType = row["SPORT_FIELD_TYPE_NAME"] + "";
				TeamCompetitionCompetitors competitionCompetitor = teamCompetitionCompetitors.Find(tcc => tcc.TeamId == teamId && tcc.Phase == phase && tcc.Group == group && tcc.SportFieldType == sportFieldType);
				if (competitionCompetitor == null)
				{
					competitionCompetitor = new TeamCompetitionCompetitors
					{
						ChampionshipCategoryId = championshipCategoryId, 
						TeamId = teamId, 
						Phase = phase, 
						Group = group, 
						SportFieldType = sportFieldType, 
						Competitors = 0
					};
					teamCompetitionCompetitors.Add(competitionCompetitor);
				}
				competitionCompetitor.Competitors++;
			});
			return teamCompetitionCompetitors.ToArray();
		}

		[WebMethod]
		public Phase[] LoadChampionship(int championshipCategoryId)
		{
			string strSQL = "SELECT PHASE_NAME, STATUS " +
				"FROM CHAMPIONSHIP_PHASES " +
				"WHERE CHAMPIONSHIP_CATEGORY_ID = @0 " +
				"ORDER BY PHASE";

			//read results:
			SimpleTable table = DB.Instance.GetDataBySQL(strSQL,
				new SimpleParameter("@0", championshipCategoryId));
			List<Phase> lstPhases = new List<Phase>();
			table.Rows.ForEach(row =>
			{
				Phase phase = new Phase();
				phase.Name = row[0].ToString();
				phase.Status = (int)row[1];
				lstPhases.Add(phase);
			});
			
			Phase[] phases = lstPhases.ToArray();
			for (int n = 0; n < phases.Length; n++)
			{
				ReadGroups(championshipCategoryId, n, phases[n]);
				ReadDefinitions(championshipCategoryId, n, phases[n]);
			}

			return phases;
		}

		private Group ReadGroup(int ccid, int iphase, int igroup)
		{
			string strSQL = "SELECT GROUP_NAME, STATUS " +
				"FROM CHAMPIONSHIP_GROUPS " +
				"WHERE CHAMPIONSHIP_CATEGORY_ID = @0 AND PHASE = @1 AND NGROUP = @2 " +
				"ORDER BY NGROUP";

			//read results:
			SimpleTable table = DB.Instance.GetDataBySQL(strSQL,
				new SimpleParameter("@0", ccid),
				new SimpleParameter("@1", iphase),
				new SimpleParameter("@2", igroup));
			if (table.Rows.Count == 0)
				return null;

			SimpleRow row = table.Rows[0];
			Group group = new Group();
			group.Name = row[0].ToString();
			group.Status = (int)row[1];
			ReadTeams(ccid, iphase, igroup, group);
			ReadRounds(ccid, iphase, igroup, group);
			ReadCompetitions(ccid, iphase, igroup, group);
			
			return group;
		}

		[WebMethod]
		public Group LoadGroup(int championshipCategoryId, int iphase, int igroup)
		{
			return ReadGroup(championshipCategoryId, iphase, igroup);
		}

		#endregion

		#region Selected Read
		public class DateChampionship
		{
			public DateTime Day;
			public int ChampionshipCategory;
		}

		[WebMethod]
		public DateChampionship[] GetDateChampionships(DateTime from, DateTime to)
		{
			string strSQL = "SELECT MIN(TIME), CHAMPIONSHIP_CATEGORY_ID " +
				"FROM (SELECT TIME, CHAMPIONSHIP_CATEGORY_ID " +
					" FROM CHAMPIONSHIP_MATCHES " +
					" WHERE TIME >= @0 AND TIME <= @1 " +
					" UNION " +
					" SELECT TIME, CHAMPIONSHIP_CATEGORY_ID " +
					" FROM CHAMPIONSHIP_COMPETITIONS " +
					" WHERE TIME >= @0 AND TIME <= @1 " +
					" UNION " +
					" SELECT TIME, CHAMPIONSHIP_CATEGORY_ID " +
					" FROM CHAMPIONSHIP_COMPETITION_HEATS " +
					" WHERE TIME >= @0 AND TIME <= @1) C " +
				"GROUP BY DATEDIFF(day, '01-JAN-1900', TIME), CHAMPIONSHIP_CATEGORY_ID";

			//read results:
			SimpleTable table = DB.Instance.GetDataBySQL(strSQL,
				new SimpleParameter("@0", from),
				new SimpleParameter("@1", to));
			List<DateChampionship> championships = new List<DateChampionship>();
			table.Rows.ForEach(row =>
			{
				DateChampionship dc = new DateChampionship();
				dc.Day = (DateTime)row[0];
				dc.ChampionshipCategory = (int)row[1];
				championships.Add(dc);
			});

			return championships.ToArray();
		}

		#endregion

		#region Championship Save
		private string SaveCompetitors(int ccid, int iphase, int igroup, int icompetition, Competitor[] competitors,
			SqlCommand command)
		{
			command.CommandText = "IF NOT EXISTS(SELECT * FROM CHAMPIONSHIP_COMPETITION_COMPETITORS WHERE CHAMPIONSHIP_CATEGORY_ID=@0 AND PHASE=@1 AND NGROUP=@2 AND COMPETITION=@3 AND PLAYER_NUMBER=@5) " +
				"INSERT INTO CHAMPIONSHIP_COMPETITION_COMPETITORS (" +
				"CHAMPIONSHIP_CATEGORY_ID, PHASE, NGROUP, COMPETITION, COMPETITOR, PLAYER_NUMBER, HEAT, POSITION, RESULT_POSITION, " +
				"RESULT, SCORE, CUSTOM_POSITION, SharedResultNumbers, LastResultDisqualifications, TotalDisqualifications, Wind) VALUES " +
				"(@0, @1, @2, @3, @4, @5, @6, @7, @8, @9, @10, @11, @12, @13, @14, @15)";
			command.Parameters.Clear();
			command.Parameters.AddWithValue("@0", ccid);
			command.Parameters.AddWithValue("@1", iphase);
			command.Parameters.AddWithValue("@2", igroup);
			command.Parameters.AddWithValue("@3", icompetition);
			command.Parameters.AddWithValue("@4", 0);
			command.Parameters.AddWithValue("@5", DBNull.Value);
			command.Parameters.AddWithValue("@6", DBNull.Value);
			command.Parameters.AddWithValue("@7", DBNull.Value);
			command.Parameters.AddWithValue("@8", DBNull.Value);
			command.Parameters.AddWithValue("@9", DBNull.Value);
			command.Parameters.AddWithValue("@10", DBNull.Value);
			command.Parameters.AddWithValue("@11", DBNull.Value);
			command.Parameters.AddWithValue("@12", DBNull.Value);
			command.Parameters.AddWithValue("@13", DBNull.Value);
			command.Parameters.AddWithValue("@14", DBNull.Value);
			command.Parameters.AddWithValue("@15", DBNull.Value);
			string strError = "";
			for (int n = 0; n < competitors.Length; n++)
			{
				Competitor curCompetitor = competitors[n];
				command.Parameters["@4"].Value = n;
				command.Parameters["@5"].Value = curCompetitor.PlayerNumber;
				command.Parameters["@6"].Value = DBNull.Value;
				command.Parameters["@7"].Value = DBNull.Value;
				command.Parameters["@8"].Value = DBNull.Value;
				command.Parameters["@9"].Value = DBNull.Value;
				command.Parameters["@10"].Value = DBNull.Value;
				command.Parameters["@11"].Value = DBNull.Value;
				command.Parameters["@12"].Value = DBNull.Value;
				if (curCompetitor.Heat >= 0)
					command.Parameters["@6"].Value = curCompetitor.Heat;
				if (curCompetitor.Position >= 0)
					command.Parameters["@7"].Value = curCompetitor.Position;
				if (curCompetitor.ResultPosition >= 0)
					command.Parameters["@8"].Value = curCompetitor.ResultPosition;
				if (curCompetitor.Result >= 0)
					command.Parameters["@9"].Value = curCompetitor.Result;
				if (curCompetitor.Score >= 0)
					command.Parameters["@10"].Value = curCompetitor.Score;
				if (curCompetitor.CustomPosition >= 0)
					command.Parameters["@11"].Value = curCompetitor.CustomPosition;
				if (curCompetitor.SharedResultNumbers != null && curCompetitor.SharedResultNumbers.Length > 1)
					command.Parameters["@12"].Value = string.Join(",", curCompetitor.SharedResultNumbers);
				command.Parameters["@13"].Value = curCompetitor.LastResultDisqualifications;
				command.Parameters["@14"].Value = curCompetitor.TotalDisqualifications;
				command.Parameters["@15"].Value = curCompetitor.Wind;
				try
				{
					command.ExecuteNonQuery();
				}
				catch (Exception ex)
				{
					strError = "Failed to insert competitor #" + (n + 1) + ": " + ex.Message;
					break;
				}
			}

			return strError;
		}

		private string SaveHeats(int ccid, int iphase, int igroup, int icompetition, Heat[] heats,
			SqlCommand command)
		{
			command.CommandText = "INSERT INTO CHAMPIONSHIP_COMPETITION_HEATS(CHAMPIONSHIP_CATEGORY_ID, PHASE, NGROUP, COMPETITION, " +
				" HEAT, TIME, FACILITY_ID, COURT_ID) " +
				"VALUES(@0, @1, @2, @3, @4, @5, @6, @7)";
			command.Parameters.Clear();
			command.Parameters.AddWithValue("@0", ccid);
			command.Parameters.AddWithValue("@1", iphase);
			command.Parameters.AddWithValue("@2", igroup);
			command.Parameters.AddWithValue("@3", icompetition);
			command.Parameters.AddWithValue("@4", 0);
			command.Parameters.AddWithValue("@5", DBNull.Value);
			command.Parameters.AddWithValue("@6", DBNull.Value);
			command.Parameters.AddWithValue("@7", DBNull.Value);
			string strError = "";
			for (int n = 0; n < heats.Length; n++)
			{
				command.Parameters["@4"].Value = n;
				command.Parameters["@5"].Value = DBNull.Value;
				command.Parameters["@6"].Value = DBNull.Value;
				command.Parameters["@7"].Value = DBNull.Value;
				if (heats[n].Time.Year > 1900)
					command.Parameters["@5"].Value = heats[n].Time;
				if (heats[n].Facility >= 0)
					command.Parameters["@6"].Value = heats[n].Facility;
				if (heats[n].Court >= 0)
					command.Parameters["@7"].Value = heats[n].Court;
				try
				{
					command.ExecuteNonQuery();
				}
				catch (Exception ex)
				{
					strError = "Failed to insert heat #" + (n + 1) + ": " + ex.Message;
					break;
				}
			}

			return strError;
		}

		private string SaveCompetitions(int ccid, int iphase, int igroup,
			Competition[] competitions, SqlCommand command)
		{
			string strError = "";
			string strSQL = "INSERT INTO CHAMPIONSHIP_COMPETITIONS(CHAMPIONSHIP_CATEGORY_ID, PHASE, NGROUP, COMPETITION, " +
					" SPORT_FIELD_ID, TIME, FACILITY_ID, COURT_ID, LANE_COUNT) " +
					"VALUES(@0, @1, @2, @3, @4, @5, @6, @7, @8)";
			for (int n = 0; n < competitions.Length; n++)
			{
				command.CommandText = strSQL;
				command.Parameters.Clear();
				command.Parameters.AddWithValue("@0", ccid);
				command.Parameters.AddWithValue("@1", iphase);
				command.Parameters.AddWithValue("@2", igroup);
				command.Parameters.AddWithValue("@3", n);
				command.Parameters.AddWithValue("@4", competitions[n].SportField);
				command.Parameters.AddWithValue("@5", DBNull.Value);
				command.Parameters.AddWithValue("@6", DBNull.Value);
				command.Parameters.AddWithValue("@7", DBNull.Value);
				command.Parameters.AddWithValue("@8", DBNull.Value);
				if (competitions[n].Time.Year > 1900)
					command.Parameters["@5"].Value = competitions[n].Time;
				if (competitions[n].Facility >= 0)
					command.Parameters["@6"].Value = competitions[n].Facility;
				if (competitions[n].Court >= 0)
					command.Parameters["@7"].Value = competitions[n].Court;
				if (competitions[n].LaneCount >= 0)
					command.Parameters["@8"].Value = competitions[n].LaneCount;
				try
				{
					command.ExecuteNonQuery();
				}
				catch (Exception ex)
				{
					strError = "Failed to insert competition #" + (n + 1) + ": " + ex.Message;
					break;
				}

				strError = SaveHeats(ccid, iphase, igroup, n, competitions[n].Heats, command);
				if (strError.Length > 0)
					break;

				strError = SaveCompetitors(ccid, iphase, igroup, n, competitions[n].Competitors, command);
				if (strError.Length > 0)
					break;
			}

			return strError;
		}

		private string SaveTeams(int ccid, int iphase, int igroup, Team[] teams, SqlCommand command)
		{
			command.CommandText = "INSERT INTO CHAMPIONSHIP_GROUP_TEAMS(CHAMPIONSHIP_CATEGORY_ID, " +
				"PHASE, NGROUP, POSITION, PREVIOUS_GROUP, PREVIOUS_POSITION, " +
				"TEAM_ID, GAMES, POINTS, POINTS_AGAINST, SCORE, RESULT_POSITION, " +
				"SMALL_POINTS, SMALL_POINTS_AGAINST, SETS, SETS_AGAINST) " +
				"VALUES(@0, @1, @2, @3, @4, @5, @6, @7, @8, @9, @10, @11, @12, @13, @14, @15) ";
			command.Parameters.Clear();
			command.Parameters.AddWithValue("@0", ccid);
			command.Parameters.AddWithValue("@1", iphase);
			command.Parameters.AddWithValue("@2", igroup);
			command.Parameters.AddWithValue("@3", 0);
			command.Parameters.AddWithValue("@4", DBNull.Value);
			command.Parameters.AddWithValue("@5", DBNull.Value);
			command.Parameters.AddWithValue("@6", DBNull.Value);
			command.Parameters.AddWithValue("@7", DBNull.Value);
			command.Parameters.AddWithValue("@8", DBNull.Value);
			command.Parameters.AddWithValue("@9", DBNull.Value);
			command.Parameters.AddWithValue("@10", DBNull.Value);
			command.Parameters.AddWithValue("@11", DBNull.Value);
			command.Parameters.AddWithValue("@12", DBNull.Value);
			command.Parameters.AddWithValue("@13", DBNull.Value);
			command.Parameters.AddWithValue("@14", DBNull.Value);
			command.Parameters.AddWithValue("@15", DBNull.Value);
			string strError = "";
			for (int n = 0; n < teams.Length; n++)
			{
				command.Parameters["@3"].Value = n;
				command.Parameters["@4"].Value = DBNull.Value;
				command.Parameters["@5"].Value = DBNull.Value;
				command.Parameters["@6"].Value = DBNull.Value;
				command.Parameters["@7"].Value = DBNull.Value;
				command.Parameters["@8"].Value = DBNull.Value;
				command.Parameters["@9"].Value = DBNull.Value;
				command.Parameters["@10"].Value = DBNull.Value;
				command.Parameters["@11"].Value = DBNull.Value;
				command.Parameters["@12"].Value = DBNull.Value;
				command.Parameters["@13"].Value = DBNull.Value;
				command.Parameters["@14"].Value = DBNull.Value;
				command.Parameters["@15"].Value = DBNull.Value;
				if (teams[n].PreviousGroup >= 0)
				{
					command.Parameters["@4"].Value = teams[n].PreviousGroup;
					command.Parameters["@5"].Value = teams[n].PreviousPosition;	
				}
				if (teams[n].TeamId >= 0)
				{
					command.Parameters["@6"].Value = teams[n].TeamId;
					command.Parameters["@7"].Value = teams[n].Games;
					command.Parameters["@8"].Value = teams[n].Points;
					command.Parameters["@9"].Value = teams[n].PointsAgainst;
					command.Parameters["@10"].Value = teams[n].Score;
					command.Parameters["@11"].Value = teams[n].ResultPosition;
					command.Parameters["@12"].Value = teams[n].SmallPoints;
					command.Parameters["@13"].Value = teams[n].SmallPointsAgainst;
					command.Parameters["@14"].Value = teams[n].Sets;
					command.Parameters["@15"].Value = teams[n].SetsAgainst;
				}

				try
				{
					command.ExecuteNonQuery();
				}
				catch (Exception ex)
				{
					strError = "Failed to insert team #" + (n + 1) + ": " + ex.Message;
					break;
				}
			}

			return strError;
		}

		private bool TeamExists(int ccid, int iphase, int igroup, int position)
		{
			string strSQL = "SELECT * FROM CHAMPIONSHIP_GROUP_TEAMS " +
				"WHERE CHAMPIONSHIP_CATEGORY_ID=@0 AND " +
				"PHASE=@1 AND " +
				"NGROUP= @2 AND " +
				"POSITION= @3";
			SimpleTable table = DB.Instance.GetDataBySQL(strSQL,
				new SimpleParameter("@0", ccid),
				new SimpleParameter("@1", iphase),
				new SimpleParameter("@2", igroup),
				new SimpleParameter("@3", position));
			return table.Rows.Count > 0;
		}

		private string SaveFunctionaries(int ccid, int iphase, int igroup, int iround, int icycle, int imatch,
			int[] functionaries, SqlCommand command)
		{
			command.CommandText = "INSERT INTO CHAMPIONSHIP_MATCH_FUNCTIONARIES (" +
				"CHAMPIONSHIP_CATEGORY_ID, PHASE, NGROUP, ROUND, CYCLE, MATCH, " +
				"ROLE, FUNCTIONARY_ID) " +
				"VALUES " +
				"(@0, @1, @2, @3, @4, @5, @6, @7)";
			command.Parameters.Clear();
			command.Parameters.AddWithValue("@0", ccid);
			command.Parameters.AddWithValue("@1", iphase);
			command.Parameters.AddWithValue("@2", igroup);
			command.Parameters.AddWithValue("@3", iround);
			command.Parameters.AddWithValue("@4", icycle);
			command.Parameters.AddWithValue("@5", imatch);
			command.Parameters.AddWithValue("@6", -1);
			command.Parameters.AddWithValue("@7", DBNull.Value);
			string strError = "";
			for (int n = 0; n < functionaries.Length; n++)
			{
				int funcID = functionaries[n];
				command.Parameters["@6"].Value = n;
				command.Parameters["@7"].Value = Sport.Common.Tools.IIF((funcID < 0), DBNull.Value, funcID);
				try
				{
					command.ExecuteNonQuery();
				}
				catch (Exception ex)
				{
					strError = "Failed to insert functionary #" + (n + 1) + ": " + ex.Message;
					break;
				}
			}

			return strError;
		}

		private string SaveMatches(int ccid, int iphase, int igroup, int iround, int icycle,
			Match[] matches, SqlCommand command)
		{
			string strSQL = "INSERT INTO CHAMPIONSHIP_MATCHES (" +
				"CHAMPIONSHIP_CATEGORY_ID, PHASE, NGROUP, ROUND, CYCLE, MATCH, " +
				"TEAM_A, TEAM_B, TIME, FACILITY_ID, COURT_ID, TEAM_A_SCORE, " +
				"TEAM_B_SCORE, RESULT, PARTS_RESULT, MATCH_NUMBER, " +
				"RELATIVE_TEAM_A, RELATIVE_TEAM_B, TOURNAMENT, REFEREE_COUNT, " +
				"CUSTOM_TEAM_A, CUSTOM_TEAM_B) " +
				"VALUES " +
				"(@0, @1, @2, @3, @4, @5, @6, @7, @8, @9, @10, @11, @12, @13, " +
				"@14, @15, @16, @17, @18, @19, @20, @21)";
			string strError = "";
			for (int n = 0; n < matches.Length; n++)
			{
				command.CommandText = strSQL;
				command.Parameters.Clear();
				command.Parameters.AddWithValue("@0", ccid);
				command.Parameters.AddWithValue("@1", iphase);
				command.Parameters.AddWithValue("@2", igroup);
				command.Parameters.AddWithValue("@3", iround);
				command.Parameters.AddWithValue("@4", icycle);
				command.Parameters.AddWithValue("@5", n);
				command.Parameters.AddWithValue("@6", DBNull.Value);
				command.Parameters.AddWithValue("@7", DBNull.Value);
				command.Parameters.AddWithValue("@8", DBNull.Value);
				command.Parameters.AddWithValue("@9", DBNull.Value);
				command.Parameters.AddWithValue("@10", DBNull.Value);
				command.Parameters.AddWithValue("@11", DBNull.Value);
				command.Parameters.AddWithValue("@12", DBNull.Value);
				command.Parameters.AddWithValue("@13", DBNull.Value);
				command.Parameters.AddWithValue("@14", DBNull.Value);
				command.Parameters.AddWithValue("@15", matches[n].Number);
				command.Parameters.AddWithValue("@16", DBNull.Value);
				command.Parameters.AddWithValue("@17", DBNull.Value);
				command.Parameters.AddWithValue("@18", DBNull.Value);
				command.Parameters.AddWithValue("@19", matches[n].RefereeCount);
				command.Parameters.AddWithValue("@20", matches[n].CustomTeamA + "");
				command.Parameters.AddWithValue("@21", matches[n].CustomTeamB + "");
				if (matches[n].TeamAPos >= 0)
					command.Parameters["@6"].Value = matches[n].TeamAPos;
				if (matches[n].TeamBPos >= 0)
					command.Parameters["@7"].Value = matches[n].TeamBPos;
				if (matches[n].Time.Year > 1900)
					command.Parameters["@8"].Value = matches[n].Time;
				if (matches[n].Facility >= 0)
					command.Parameters["@9"].Value = matches[n].Facility;
				if (matches[n].Court >= 0)
					command.Parameters["@10"].Value = matches[n].Court;
				if (matches[n].TeamAScore >= 0)
					command.Parameters["@11"].Value = matches[n].TeamAScore;
				if (matches[n].TeamBScore >= 0)
					command.Parameters["@12"].Value = matches[n].TeamBScore;
				if (matches[n].Outcome >= 0)
					command.Parameters["@13"].Value = matches[n].Outcome;
				if (matches[n].PartsResult != null)
					command.Parameters["@14"].Value = matches[n].PartsResult;
				if (matches[n].RelativeTeamA != 0)
					command.Parameters["@16"].Value = matches[n].RelativeTeamA;
				if (matches[n].RelativeTeamB != 0)
					command.Parameters["@17"].Value = matches[n].RelativeTeamB;
				if (matches[n].Tournament >= 0)
					command.Parameters["@18"].Value = matches[n].Tournament;
				try
				{
					command.ExecuteNonQuery();
				}
				catch (Exception ex)
				{
					strError = "Failed to insert match #" + (n + 1) + ": " + ex.Message;
					break;
				}

				strError = SaveFunctionaries(ccid, iphase, igroup, iround, icycle, n, matches[n].Functionaries, command);
				if (strError.Length > 0)
					break;
			}

			return strError;
		}

		private string SaveTournaments(int ccid, int iphase, int igroup, int iround, int icycle,
			Tournament[] tournaments, SqlCommand command)
		{
			command.CommandText = "INSERT INTO CHAMPIONSHIP_TOURNAMENTS (" +
				"CHAMPIONSHIP_CATEGORY_ID, PHASE, NGROUP, ROUND, CYCLE, TOURNAMENT, " +
				" NUMBER, TIME, FACILITY_ID, COURT_ID) " +
				"VALUES " +
				"(@0, @1, @2, @3, @4, @5, @6, @7, @8, @9)";
			command.Parameters.Clear();
			command.Parameters.AddWithValue("@0", ccid);
			command.Parameters.AddWithValue("@1", iphase);
			command.Parameters.AddWithValue("@2", igroup);
			command.Parameters.AddWithValue("@3", iround);
			command.Parameters.AddWithValue("@4", icycle);
			command.Parameters.AddWithValue("@5", 0);
			command.Parameters.AddWithValue("@6", 0);
			command.Parameters.AddWithValue("@7", DBNull.Value);
			command.Parameters.AddWithValue("@8", DBNull.Value);
			command.Parameters.AddWithValue("@9", DBNull.Value);
			string strError = "";
			for (int n = 0; n < tournaments.Length; n++)
			{
				command.Parameters["@5"].Value = n;
				command.Parameters["@6"].Value = tournaments[n].Number;
				if (tournaments[n].Time.Year < 1900)
					command.Parameters["@7"].Value = DBNull.Value;
				else
					command.Parameters["@7"].Value = tournaments[n].Time;
				if (tournaments[n].Facility == -1)
					command.Parameters["@8"].Value = DBNull.Value;
				else
					command.Parameters["@8"].Value = tournaments[n].Facility;
				if (tournaments[n].Court == -1)
					command.Parameters["@9"].Value = DBNull.Value;
				else
					command.Parameters["@9"].Value = tournaments[n].Court;
				try
				{
					command.ExecuteNonQuery();
				}
				catch (Exception ex)
				{
					strError = "Failed to insert tournament #" + (n + 1) + ": " + ex.Message;
					break;
				}
			}

			return strError;
		}

		private string SaveCycles(int ccid, int iphase, int igroup, int iround,
			Cycle[] cycles, SqlCommand command)
		{
			string strSQL = "INSERT INTO CHAMPIONSHIP_CYCLES(CHAMPIONSHIP_CATEGORY_ID, PHASE, NGROUP, ROUND, CYCLE, CYCLE_NAME) " +
				"VALUES(@0, @1, @2, @3, @4, @5)";
			string strError = "";
			for (int n = 0; n < cycles.Length; n++)
			{
				command.CommandText = strSQL;
				command.Parameters.Clear();
				command.Parameters.AddWithValue("@0", ccid);
				command.Parameters.AddWithValue("@1", iphase);
				command.Parameters.AddWithValue("@2", igroup);
				command.Parameters.AddWithValue("@3", iround);
				command.Parameters.AddWithValue("@4", n);
				command.Parameters.AddWithValue("@5", cycles[n].Name);
				try
				{
					command.ExecuteNonQuery();
				}
				catch (Exception ex)
				{
					strError = "Failed to insert cycle #" + (n + 1) + ": " + ex.Message;
					break;
				}

				strError = SaveTournaments(ccid, iphase, igroup, iround, n, cycles[n].Tournaments, command);
				if (strError.Length > 0)
					break;

				strError = SaveMatches(ccid, iphase, igroup, iround, n, cycles[n].Matches, command);
				if (strError.Length > 0)
					break;
			}

			return strError;
		}

		private string SaveRounds(int ccid, int iphase, int igroup,
			Round[] rounds, SqlCommand command)
		{
			string strSQL = "INSERT INTO CHAMPIONSHIP_ROUNDS(CHAMPIONSHIP_CATEGORY_ID, PHASE, NGROUP, ROUND, ROUND_NAME, STATUS) " +
				"VALUES(@0, @1, @2, @3, @4, @5)";
			string strError = "";
			for (int n = 0; n < rounds.Length; n++)
			{
				command.CommandText = strSQL;
				command.Parameters.Clear();
				command.Parameters.AddWithValue("@0", ccid);
				command.Parameters.AddWithValue("@1", iphase);
				command.Parameters.AddWithValue("@2", igroup);
				command.Parameters.AddWithValue("@3", n);
				command.Parameters.AddWithValue("@4", rounds[n].Name);
				command.Parameters.AddWithValue("@5", rounds[n].Status);
				try
				{
					command.ExecuteNonQuery();
				}
				catch (Exception ex)
				{
					strError = "Failed to insert round #" + (n + 1) + ": " + ex.Message;
					break;
				}

				strError = SaveCycles(ccid, iphase, igroup, n, rounds[n].Cycles, command);
				if (strError.Length > 0)
					break;
			}

			return strError;
		}

		private string SaveGroups(int ccid, int iphase, Group[] groups, SqlCommand command)
		{
			string strSQL = "INSERT INTO CHAMPIONSHIP_GROUPS(CHAMPIONSHIP_CATEGORY_ID, PHASE, NGROUP, GROUP_NAME, STATUS) " +
				"VALUES(@0, @1, @2, @3, @4)";
			string strError = "";
			for (int n = 0; n < groups.Length; n++)
			{
				command.CommandText = strSQL;
				command.Parameters.Clear();
				command.Parameters.AddWithValue("@0", ccid);
				command.Parameters.AddWithValue("@1", iphase);
				command.Parameters.AddWithValue("@2", n);
				command.Parameters.AddWithValue("@3", groups[n].Name);
				command.Parameters.AddWithValue("@4", groups[n].Status);
				try
				{
					command.ExecuteNonQuery();
				}
				catch (Exception ex)
				{
					strError = "Failed to insert group #" + (n + 1) + ": " + ex.Message;
					break;
				}

				strError = SaveTeams(ccid, iphase, n, groups[n].Teams, command);
				if (strError.Length > 0)
					break;

				if (groups[n].Rounds != null)
				{
					strError = SaveRounds(ccid, iphase, n, groups[n].Rounds, command);
					if (strError.Length > 0)
						break;
				}

				if (groups[n].Competitions != null)
				{
					strError = SaveCompetitions(ccid, iphase, n, groups[n].Competitions, command);
					if (strError.Length > 0)
						break;
				}
			}

			return strError;
		}

		private string SaveDefinitions(int ccid, int iphase, PhaseDefinition[] definitions,
			SqlCommand command)
		{
			command.CommandText = "INSERT INTO CHAMPIONSHIP_PHASE_DEFINITIONS(CHAMPIONSHIP_CATEGORY_ID, PHASE, RULE_TYPE_ID, DEFINITION, VALUE) " +
				"VALUES(@0, @1, @2, @3, @4)";
			command.Parameters.Clear();
			command.Parameters.AddWithValue("@0", ccid);
			command.Parameters.AddWithValue("@1", iphase);
			command.Parameters.AddWithValue("@2", 0);
			command.Parameters.AddWithValue("@3", DBNull.Value);
			command.Parameters.AddWithValue("@4", DBNull.Value);
			string strError = "";
			for (int n = 0; n < definitions.Length; n++)
			{
				command.Parameters["@2"].Value = definitions[n].RuleType;
				command.Parameters["@3"].Value = definitions[n].Definition;
				command.Parameters["@4"].Value = definitions[n].Value;
				try
				{
					command.ExecuteNonQuery();
				}
				catch (Exception ex)
				{
					strError = "Failed to insert phase definition #" + (n + 1) + ": " + ex.Message;
					break;
				}
			}

			return strError;
		}

		private string SavePhases(int ccid, Phase[] phases, SqlCommand command)
		{
			string strSQL = "INSERT INTO CHAMPIONSHIP_PHASES(CHAMPIONSHIP_CATEGORY_ID, PHASE, PHASE_NAME, STATUS) " +
				"VALUES(@0, @1, @2, @3)";
			string strError = "";
			for (int n = 0; n < phases.Length; n++)
			{
				command.CommandText = strSQL;
				command.Parameters.Clear();
				command.Parameters.AddWithValue("@0", ccid);
				command.Parameters.AddWithValue("@1", n);
				command.Parameters.AddWithValue("@2", phases[n].Name);
				command.Parameters.AddWithValue("@3", phases[n].Status);
				try
				{
					command.ExecuteNonQuery();
				}
				catch (Exception ex)
				{
					strError = "Failed to insert phase #" + (n + 1) + ": " + ex.Message;
					break;
				}

				strError = SaveGroups(ccid, n, phases[n].Groups, command);
				if (strError.Length > 0)
					break;

				if (phases[n].Definitions != null)
				{
					strError = SaveDefinitions(ccid, n, phases[n].Definitions, command);
					if (strError.Length > 0)
						break;
				}
			}

			return strError;
		}

		[WebMethod]
		public string SaveChampionship(int championshipCategoryId, ref Phase[] phases)
		{
			SqlConnection connection = DB.Instance.GetOpenConnection();
			SqlTransaction transaction = connection.BeginTransaction();
			SqlCommand command = new SqlCommand();
			command.Connection = connection;
			command.Transaction = transaction;
			command.CommandType = CommandType.Text;
			command.Parameters.AddWithValue("@0", championshipCategoryId);
			string strError = "";
			string strSQL = "DELETE {0} WHERE CHAMPIONSHIP_CATEGORY_ID = @0";
			string[] tables = new string[] { "CHAMPIONSHIP_MATCH_FUNCTIONARIES", 
				"PENDING_MATCH_SCORES", "CHAMPIONSHIP_MATCHES", "CHAMPIONSHIP_TOURNAMENTS", 
				"CHAMPIONSHIP_CYCLES", "CHAMPIONSHIP_ROUNDS", "CHAMPIONSHIP_COMPETITION_COMPETITORS", 
				"CHAMPIONSHIP_COMPETITION_HEATS", "CHAMPIONSHIP_COMPETITIONS", "CHAMPIONSHIP_GROUP_TEAMS", 
				"CHAMPIONSHIP_GROUPS", "CHAMPIONSHIP_PHASE_DEFINITIONS", "CHAMPIONSHIP_PHASES" };

			// Deleting all championship information
			for (int i = 0; i < tables.Length; i++)
			{
				string tableName = tables[i];
				command.CommandText = string.Format(strSQL, tableName);
				try
				{
					command.ExecuteNonQuery();
				}
				catch (Exception ex)
				{
					strError = "Error deleting " + tableName + ": " + ex.ToString();
					break;
				}
			}
			
			if (strError.Length == 0)
			{
				lock (this)
				{
					try
					{
						strError = SavePhases(championshipCategoryId, phases, command);
					}
					catch (Exception ex)
					{
						strError = "General error saving phases: " + ex.ToString();
					}
				}
			}

			if (strError.Length == 0)
			{
				try
				{
					transaction.Commit();
				}
				catch (Exception ex)
				{
					strError = "Error committing: " + ex.ToString();
				}
			}
			else
			{
				try
				{
					transaction.Rollback();
				}
				catch
				{ }
			}

			command.Dispose();
			transaction.Dispose();
			connection.Close();
			connection.Dispose();
			
			return strError;
		}

		[WebMethod]
		public string SaveGroup(int championshipCategoryId, int phase, int ngroup,
			ref Group group)
		{
			SqlConnection connection = DB.Instance.GetOpenConnection();
			SqlTransaction transaction = connection.BeginTransaction();
			SqlCommand command = new SqlCommand();
			command.Connection = connection;
			command.Transaction = transaction;
			command.CommandType = CommandType.Text;
			command.Parameters.AddWithValue("@0", championshipCategoryId);
			command.Parameters.AddWithValue("@1", phase);
			command.Parameters.AddWithValue("@2", ngroup);
			string strError = "";
			string strSQL = "DELETE {0} " +
				"WHERE CHAMPIONSHIP_CATEGORY_ID = @0 AND " +
					"  PHASE = @1 AND " +
					"  NGROUP = @2";
			string[] tables = new string[] { "CHAMPIONSHIP_MATCHES", 
				"CHAMPIONSHIP_TOURNAMENTS", "CHAMPIONSHIP_CYCLES", "CHAMPIONSHIP_ROUNDS", 
				"CHAMPIONSHIP_COMPETITION_COMPETITORS", "CHAMPIONSHIP_COMPETITION_HEATS", 
				"CHAMPIONSHIP_COMPETITIONS", "CHAMPIONSHIP_GROUP_TEAMS", "CHAMPIONSHIP_GROUPS" };

			// Deleting all championship information
			for (int i = 0; i < tables.Length; i++)
			{
				string tableName = tables[i];
				command.CommandText = string.Format(strSQL, tableName);
				try
				{
					command.ExecuteNonQuery();
				}
				catch (Exception ex)
				{
					strError = "Error deleting " + tableName + ": " + ex.ToString();
					break;
				}
			}

			if (strError.Length == 0)
			{
				command.CommandText = "INSERT INTO CHAMPIONSHIP_GROUPS(CHAMPIONSHIP_CATEGORY_ID, PHASE, NGROUP, GROUP_NAME, STATUS) " +
					"VALUES(@0, @1, @2, @3, @4)";
				command.Parameters.Clear();
				command.Parameters.AddWithValue("@0", championshipCategoryId);
				command.Parameters.AddWithValue("@1", phase);
				command.Parameters.AddWithValue("@2", ngroup);
				command.Parameters.AddWithValue("@3", group.Name);
				command.Parameters.AddWithValue("@4", group.Status);
				try
				{
					command.ExecuteNonQuery();
				}
				catch (Exception ex)
				{
					strError = "Error inserting new group: " + ex.ToString();
				}
			}

			if (strError.Length == 0)
			{
				try
				{
					strError = SaveTeams(championshipCategoryId, phase, ngroup, group.Teams, command);
				}
				catch (Exception ex)
				{
					strError = "General error saving teams: " + ex.ToString();
				}

				if (strError.Length == 0 && group.Rounds != null)
				{
					try
					{
						strError = SaveRounds(championshipCategoryId, phase, ngroup, group.Rounds, command);
					}
					catch (Exception ex)
					{
						strError = "General error saving rounds: " + ex.ToString();
					}
				}

				if (strError.Length == 0 && group.Competitions != null)
				{
					try
					{
						strError = SaveCompetitions(championshipCategoryId, phase, ngroup, group.Competitions, command);
					}
					catch (Exception ex)
					{
						strError = "General error saving competitions: " + ex.ToString();
					}
				}
			}

			if (strError.Length == 0)
			{
				try
				{
					transaction.Commit();
				}
				catch (Exception ex)
				{
					strError = "Error committing: " + ex.ToString();
				}
			}
			else
			{
				try
				{
					transaction.Rollback();
				}
				catch
				{ }
			}

			command.Dispose();
			transaction.Dispose();
			connection.Close();
			connection.Dispose();
			
			return strError;
		}

		#endregion

		#region Match/Competition Save
		[WebMethod]
		public bool SaveMatch(int championshipCategoryId, int phase,
			int group, int round, int cycle, int match, int number, string strTime, int facility,
			int court, int[] functionaries)
		{
			DateTime time = Sport.Common.Tools.DatabaseStringToDateTime(strTime);
			bool success = true;
			SqlConnection connection = DB.Instance.GetOpenConnection();
			SqlTransaction transaction = connection.BeginTransaction();

			// Deleting all championship information
			string strSQL = "DELETE CHAMPIONSHIP_MATCH_FUNCTIONARIES " +
				"WHERE CHAMPIONSHIP_CATEGORY_ID = @C AND " +
				"PHASE = @P AND " +
				"NGROUP = @G AND " +
				"ROUND = @R AND " +
				"CYCLE = @Y AND " +
				"MATCH = @M";
			SqlCommand command = new SqlCommand(strSQL, connection, transaction);
			command.Parameters.AddWithValue("@C", championshipCategoryId);
			command.Parameters.AddWithValue("@P", phase);
			command.Parameters.AddWithValue("@G", group);
			command.Parameters.AddWithValue("@R", round);
			command.Parameters.AddWithValue("@Y", cycle);
			command.Parameters.AddWithValue("@M", match);
			try
			{
				command.ExecuteNonQuery();
			}
			catch
			{
				success = false;
			}

			if (success)
			{
				command.Parameters.AddWithValue("@0", DBNull.Value);
				command.Parameters.AddWithValue("@1", DBNull.Value);
				command.Parameters.AddWithValue("@2", DBNull.Value);
				command.Parameters.AddWithValue("@3", number);
				command.CommandText = "UPDATE CHAMPIONSHIP_MATCHES " +
					"SET TIME = @0, " +
						"FACILITY_ID = @1, " +
						"COURT_ID = @2, " +
						"MATCH_NUMBER = @3 " +
					"WHERE CHAMPIONSHIP_CATEGORY_ID = @C AND " +
						" PHASE = @P AND " +
						" NGROUP = @G AND " +
						" ROUND = @R AND " +
						" CYCLE = @Y AND " +
						" MATCH = @M ";
				if (time.Year > 1900)
					command.Parameters["@0"].Value = time;
				if (facility >= 0)
					command.Parameters["@1"].Value = facility;
				if (court >= 0)
					command.Parameters["@2"].Value = court;

				try
				{
					command.ExecuteNonQuery();
				}
				catch
				{
					success = false;
				}

				if (success)
				{
					command.CommandText = "INSERT INTO CHAMPIONSHIP_MATCH_FUNCTIONARIES (" +
							"CHAMPIONSHIP_CATEGORY_ID, PHASE, NGROUP, ROUND, CYCLE, MATCH, " +
							"ROLE, FUNCTIONARY_ID) " +
							"VALUES " +
							"(@C, @P, @G, @R, @Y, @M, @1, @2)";
					for (int n = 0; n < functionaries.Length; n++)
					{
						int funcID = functionaries[n];
						command.Parameters["@1"].Value = n;
						command.Parameters["@2"].Value = Sport.Common.Tools.IIF((funcID < 0), DBNull.Value, funcID);
						try
						{
							command.ExecuteNonQuery();
						}
						catch
						{
							success = false;
							break;
						}
					}
				}
			}

			if (success)
			{
				try
				{
					transaction.Commit();
				}
				catch
				{
					success = false;
				}
			}
			else
			{
				try
				{
					transaction.Rollback();
				}
				catch
				{ }
			}

			command.Dispose();
			transaction.Dispose();
			connection.Close();
			connection.Dispose();
			
			return success;
		}

		[WebMethod]
		public bool SaveTournament(int championshipCategoryId, int phase,
			int group, int round, int cycle, int tournament, int number, DateTime time, int facility,
			int court)
		{
			string strSQL = "UPDATE CHAMPIONSHIP_TOURNAMENTS " +
				"SET TIME = @0, " +
				"FACILITY_ID = @1, " +
				"COURT_ID = @2, " +
				"NUMBER = @3 " +
				"WHERE CHAMPIONSHIP_CATEGORY_ID = @C AND " +
				" PHASE = @P AND " +
				" NGROUP = @G AND " +
				" ROUND = @R AND " +
				" CYCLE = @Y AND " +
				" TOURNAMENT = @T";
			List<SimpleParameter> parameters = new List<SimpleParameter>();
			parameters.Add(new SimpleParameter("@C", championshipCategoryId));
			parameters.Add(new SimpleParameter("@P", phase));
			parameters.Add(new SimpleParameter("@G", group));
			parameters.Add(new SimpleParameter("@R", round));
			parameters.Add(new SimpleParameter("@Y", cycle));
			parameters.Add(new SimpleParameter("@T", tournament));
			parameters.Add(new SimpleParameter("@0", time.Year < 1900, DBNull.Value, time));
			parameters.Add(new SimpleParameter("@1", facility == -1, DBNull.Value, facility));
			parameters.Add(new SimpleParameter("@2", court == -1, DBNull.Value, court));
			parameters.Add(new SimpleParameter("@3", number));
			return (DB.Instance.Execute(strSQL, parameters.ToArray()) > 0);
		}

		[WebMethod]
		public bool SaveCompetition(int championshipCategoryId, int phase, int group,
			int sportField, int competition, DateTime time, int facility, int court)
		{
			string strSQL = "UPDATE CHAMPIONSHIP_COMPETITIONS " +
				"SET TIME = @0, " +
				"FACILITY_ID = @1, " +
				"COURT_ID = @2 " +
				"WHERE CHAMPIONSHIP_CATEGORY_ID = @CC AND " +
				" PHASE = @PH AND " +
				" NGROUP = @GR AND " +
				" SPORT_FIELD_ID = @SP AND " +
				" COMPETITION = @CO";
			List<SimpleParameter> parameters = new List<SimpleParameter>();
			parameters.Add(new SimpleParameter("@CC", championshipCategoryId));
			parameters.Add(new SimpleParameter("@PH", phase));
			parameters.Add(new SimpleParameter("@GR", group));
			parameters.Add(new SimpleParameter("@SP", sportField));
			parameters.Add(new SimpleParameter("@CO", competition));
			parameters.Add(new SimpleParameter("@0", time.Year < 1900, DBNull.Value, time));
			parameters.Add(new SimpleParameter("@1", facility == -1, DBNull.Value, facility));
			parameters.Add(new SimpleParameter("@2", court == -1, DBNull.Value, court));
			return (DB.Instance.Execute(strSQL, parameters.ToArray()) > 0);
		}

		[WebMethod]
		public bool SaveHeat(int championshipCategoryId, int phase, int group,
			int competition, int heat, DateTime time, int facility, int court)
		{
			string strSQL = "UPDATE CHAMPIONSHIP_COMPETITION_HEATS " +
				"SET TIME = @0, " +
				"FACILITY_ID = @1, " +
				"COURT_ID = @2 " +
				"WHERE CHAMPIONSHIP_CATEGORY_ID = @CC AND " +
				" PHASE = @PH AND " +
				" NGROUP = @GR AND " +
				" COMPETITION = @CO AND " +
				" HEAT = @HE";
			List<SimpleParameter> parameters = new List<SimpleParameter>();
			parameters.Add(new SimpleParameter("@CC", championshipCategoryId));
			parameters.Add(new SimpleParameter("@PH", phase));
			parameters.Add(new SimpleParameter("@GR", group));
			parameters.Add(new SimpleParameter("@CO", competition));
			parameters.Add(new SimpleParameter("@HE", heat));
			parameters.Add(new SimpleParameter("@0", time.Year < 1900, DBNull.Value, time));
			parameters.Add(new SimpleParameter("@1", facility == -1, DBNull.Value, facility));
			parameters.Add(new SimpleParameter("@2", court == -1, DBNull.Value, court));
			return (DB.Instance.Execute(strSQL, parameters.ToArray()) > 0);
		}

		#endregion

		#region Result Set
		public class MatchTeamResult
		{
			public int Games;
			public double Points;
			public double PointsAgainst;
			public int Sets;
			public int SetsAgainst;
			public int SmallPoints;
			public int SmallPointsAgainst;
			public double Score;
			public int Wins;
			public int Loses;
			public int Ties;
			public int TechnicalWins;
			public int TechnicalLoses;
			public int Position;
		}

		public class MatchResult
		{
			public int ChampionshipCategoryId;
			public int Phase;
			public int Group;
			public MatchTeamResult[] TeamsResult;

			public int Round;
			public int Cycle;
			public int Match;
			public double TeamAScore;
			public double TeamBScore;
			public int Outcome;
			public string PartsResult;
		}

		public class MatchGroupMatchResult
		{
			public int Round;
			public int Cycle;
			public int Match;
			public double TeamAScore;
			public double TeamBScore;
			public int Outcome;
			public string PartsResult;
		}

		public class MatchGroupResult
		{
			public int ChampionshipCategoryId;
			public int Phase;
			public int Group;
			public MatchTeamResult[] TeamsResult;
			public MatchGroupMatchResult[] MatchResults;
		}

		private string SaveMatchTeamsResult(int cc, int phase, int group,
			MatchTeamResult[] teamsResult, SqlCommand command)
		{
			if (teamsResult == null || teamsResult.Length == 0)
				return "no team results";

			string strSQL = "SELECT COUNT(*) " +
				"FROM CHAMPIONSHIP_GROUP_TEAMS " +
				"WHERE CHAMPIONSHIP_CATEGORY_ID = @0 AND " +
				" PHASE = @1 AND NGROUP = @2";
			SimpleTable table = DB.Instance.GetDataBySQL(strSQL,
				new SimpleParameter("@0", cc),
				new SimpleParameter("@1", phase),
				new SimpleParameter("@2", group));
			int teamCount = table.Rows.Count > 0 ? (int)table.Rows[0][0] : 0;
			if (teamsResult.Length != teamCount)
				return "team results " + teamsResult.Length + " does not match team count " + teamCount;

			command.CommandText = "UPDATE CHAMPIONSHIP_GROUP_TEAMS " +
				"SET GAMES = @0, POINTS = @1, POINTS_AGAINST = @2, " +
				"SCORE = @3, RESULT_POSITION = @4, SMALL_POINTS = @5, " +
				"SMALL_POINTS_AGAINST = @6, SETS=@7, SETS_AGAINST=@8 " +
				"WHERE CHAMPIONSHIP_CATEGORY_ID = @C AND " +
				" PHASE = @P AND NGROUP = @G AND POSITION = @T";
			command.Parameters.Clear();
			command.Parameters.AddWithValue("@C", cc);
			command.Parameters.AddWithValue("@P", phase);
			command.Parameters.AddWithValue("@G", group);
			command.Parameters.AddWithValue("@T", 0);
			command.Parameters.AddWithValue("@0", 0);
			command.Parameters.AddWithValue("@1", 0);
			command.Parameters.AddWithValue("@2", 0);
			command.Parameters.AddWithValue("@3", 0);
			command.Parameters.AddWithValue("@4", 0);
			command.Parameters.AddWithValue("@5", 0);
			command.Parameters.AddWithValue("@6", 0);
			command.Parameters.AddWithValue("@7", 0);
			command.Parameters.AddWithValue("@8", 0);
			string strError = "";
			for (int t = 0; t < teamCount; t++)
			{
				command.Parameters["@T"].Value = t;
				command.Parameters["@0"].Value = teamsResult[t].Games;
				command.Parameters["@1"].Value = teamsResult[t].Points;
				command.Parameters["@2"].Value = teamsResult[t].PointsAgainst;
				command.Parameters["@3"].Value = teamsResult[t].Score;
				command.Parameters["@4"].Value = teamsResult[t].Position;
				command.Parameters["@5"].Value = teamsResult[t].SmallPoints;
				command.Parameters["@6"].Value = teamsResult[t].SmallPointsAgainst;
				command.Parameters["@7"].Value = teamsResult[t].Sets;
				command.Parameters["@8"].Value = teamsResult[t].SetsAgainst;
				try
				{
					command.ExecuteNonQuery();
				}
				catch (Exception ex)
				{
					strError = "Error updating group team #" + (t + 1) + ": " + ex.Message;
					break;
				}
			}

			return strError;
		}

		private string SaveMatchGroupResults(int cc, int phase, int group,
			MatchGroupMatchResult[] groupResults, SqlCommand command)
		{
			if (groupResults == null || groupResults.Length == 0)
				return "no group results";

			command.CommandText = "UPDATE CHAMPIONSHIP_MATCHES " +
				"SET TEAM_A_SCORE = @0, " +
					"TEAM_B_SCORE = @1, " +
					"RESULT = @2, " +
					"PARTS_RESULT = @3 " +
				"WHERE CHAMPIONSHIP_CATEGORY_ID = @C AND " +
					" PHASE = @P AND " +
					" NGROUP = @G AND " +
					" ROUND = @R AND " +
					" CYCLE = @Y AND " +
					" MATCH = @M ";
			command.Parameters.Clear();
			command.Parameters.AddWithValue("@C", cc);
			command.Parameters.AddWithValue("@P", phase);
			command.Parameters.AddWithValue("@G", group);
			command.Parameters.AddWithValue("@R", 0);
			command.Parameters.AddWithValue("@Y", 0);
			command.Parameters.AddWithValue("@M", 0);
			command.Parameters.AddWithValue("@0", 0);
			command.Parameters.AddWithValue("@1", 0);
			command.Parameters.AddWithValue("@2", 0);
			command.Parameters.AddWithValue("@3", "");
			string strError = "";
			for (int r = 0; r < groupResults.Length; r++)
			{
				MatchGroupMatchResult result = groupResults[r];
				command.Parameters["@R"].Value = result.Round;
				command.Parameters["@Y"].Value = result.Cycle;
				command.Parameters["@M"].Value = result.Match;
				command.Parameters["@0"].Value = DBNull.Value;
				command.Parameters["@1"].Value = DBNull.Value;
				command.Parameters["@2"].Value = DBNull.Value;
				command.Parameters["@3"].Value = DBNull.Value;
				if (result.TeamAScore >= 0)
					command.Parameters["@0"].Value = result.TeamAScore;
				if (result.TeamBScore >= 0)
					command.Parameters["@1"].Value = result.TeamBScore;
				if (result.Outcome >= 0)
					command.Parameters["@2"].Value = result.Outcome;
				if (result.PartsResult != null)
					command.Parameters["@3"].Value = result.PartsResult;
				try
				{
					command.ExecuteNonQuery();
				}
				catch (Exception ex)
				{
					strError = "Error updating championship matches: " + ex.Message;
					break;
				}
			}

			return strError;
		}

		[WebMethod]
		public bool SetMatchTeamsResult(int championshipCategoryId, int phase,
			int group, MatchTeamResult[] teamsResult)
		{
			SqlConnection connection = DB.Instance.GetOpenConnection();
			SqlTransaction transaction = connection.BeginTransaction();
			SqlCommand command = new SqlCommand();
			command.Connection = connection;
			command.Transaction = transaction;
			command.CommandType = CommandType.Text;
			string strError = SaveMatchTeamsResult(championshipCategoryId, phase, group, teamsResult, command);
			if (strError.Length == 0)
			{
				try
				{
					transaction.Commit();
				}
				catch (Exception ex)
				{
					strError = "Error committing: " + ex.ToString();
				}
			}
			else
			{
				try
				{
					transaction.Rollback();
				}
				catch
				{ }
			}

			command.Dispose();
			transaction.Dispose();
			connection.Close();
			connection.Dispose();

			if (strError.Length > 0)
				throw new Exception("Failed to set match team results: " + strError);

			return true;
		}

		[WebMethod]
		public bool SetMatchResult(MatchResult result)
		{
			SqlConnection connection = DB.Instance.GetOpenConnection();
			SqlTransaction transaction = connection.BeginTransaction();
			SqlCommand command = new SqlCommand();
			command.Connection = connection;
			command.Transaction = transaction;
			command.CommandType = CommandType.Text;
			string strError = SaveMatchTeamsResult(result.ChampionshipCategoryId, result.Phase,
				result.Group, result.TeamsResult, command);

			if (strError.Length == 0)
			{
				command.CommandText = "UPDATE CHAMPIONSHIP_MATCHES " +
					"SET TEAM_A_SCORE = @0, " +
						"TEAM_B_SCORE = @1, " +
						"RESULT = @2, " +
						"PARTS_RESULT = @3 " +
					"WHERE CHAMPIONSHIP_CATEGORY_ID = @C AND " +
						" PHASE = @P AND " +
						" NGROUP = @G AND " +
						" ROUND = @R AND " +
						" CYCLE = @Y AND " +
						" MATCH = @M";
				command.Parameters.Clear();
				command.Parameters.AddWithValue("@C", result.ChampionshipCategoryId);
				command.Parameters.AddWithValue("@P", result.Phase);
				command.Parameters.AddWithValue("@G", result.Group);
				command.Parameters.AddWithValue("@R", result.Round);
				command.Parameters.AddWithValue("@Y", result.Cycle);
				command.Parameters.AddWithValue("@M", result.Match);
				command.Parameters.AddWithValue("@0", DBNull.Value);
				command.Parameters.AddWithValue("@1", DBNull.Value);
				command.Parameters.AddWithValue("@2", DBNull.Value);
				command.Parameters.AddWithValue("@3", DBNull.Value);
				if (result.TeamAScore >= 0)
					command.Parameters["@0"].Value = result.TeamAScore;
				if (result.TeamBScore >= 0)
					command.Parameters["@1"].Value = result.TeamBScore;
				if (result.Outcome >= 0)
					command.Parameters["@2"].Value = result.Outcome;
				if (result.PartsResult != null)
					command.Parameters["@3"].Value = result.PartsResult;
				try
				{
					command.ExecuteNonQuery();
				}
				catch (Exception ex)
				{
					strError = "Error updating championship matches table: " + ex.Message;
				}
			}

			if (strError.Length == 0)
			{
				try
				{
					transaction.Commit();
				}
				catch (Exception ex)
				{
					strError = "Error committing: " + ex.ToString();
				}
			}
			else
			{
				try
				{
					transaction.Rollback();
				}
				catch
				{ }
			}

			command.Dispose();
			transaction.Dispose();
			connection.Close();
			connection.Dispose();

			if (strError.Length > 0)
				throw new Exception("Failed to set match result: " + strError);

			return true;
		}

		[WebMethod]
		public bool SetRefereeCount(int cc, int phase, int group, int round, int cycle,
			int match, int refereeCount)
		{
			if ((refereeCount < 0) || (refereeCount > 999))
				return false;

			string strSQL = "UPDATE CHAMPIONSHIP_MATCHES " +
				"SET REFEREE_COUNT = @count " +
				"WHERE CHAMPIONSHIP_CATEGORY_ID = @C AND " +
				" PHASE = @P AND " +
				" NGROUP = @G AND " +
				" ROUND = @R AND " +
				" CYCLE = @Y AND " +
				" MATCH = @M";
			List<SimpleParameter> parameters = new List<SimpleParameter>();
			parameters.Add(new SimpleParameter("@C", cc));
			parameters.Add(new SimpleParameter("@P", phase));
			parameters.Add(new SimpleParameter("@G", group));
			parameters.Add(new SimpleParameter("@R", round));
			parameters.Add(new SimpleParameter("@Y", cycle));
			parameters.Add(new SimpleParameter("@M", match));
			parameters.Add(new SimpleParameter("@count", refereeCount));
			return (DB.Instance.Execute(strSQL, parameters.ToArray()) > 0);
		}

		[WebMethod]
		public bool SetMatchGroupResult(MatchGroupResult result)
		{
			SqlConnection connection = DB.Instance.GetOpenConnection();
			SqlTransaction transaction = connection.BeginTransaction();
			SqlCommand command = new SqlCommand();
			command.Connection = connection;
			command.Transaction = transaction;
			command.CommandType = CommandType.Text;
			string strError = SaveMatchTeamsResult(result.ChampionshipCategoryId, result.Phase,
				result.Group, result.TeamsResult, command);

			if (strError.Length == 0)
			{
				try
				{
					strError = SaveMatchGroupResults(result.ChampionshipCategoryId, result.Phase,
						result.Group, result.MatchResults, command);
				}
				catch (Exception ex)
				{
					strError = "General error saving match group results: " + ex.ToString();
				}
			}

			if (strError.Length == 0)
			{
				try
				{
					transaction.Commit();
				}
				catch (Exception ex)
				{
					strError = "Error committing: " + ex.ToString();
				}
			}
			else
			{
				try
				{
					transaction.Rollback();
				}
				catch
				{ }
			}

			command.Dispose();
			transaction.Dispose();
			connection.Close();
			connection.Dispose();

			if (strError.Length > 0)
				throw new Exception("Failed to set match group results: " + strError);

			return true;
		}

		public class CompetitionTeamResult
		{
			public int Score;
			public int Position;
		}

		public class CompetitorResult
		{
			public int ChampionshipCategoryId;
			public int Phase;
			public int Group;
			public CompetitionTeamResult[] TeamsResult;

			public int Competition;
			public int[] CompetitorsPosition;
			public int Competitor;
			public int Result;
			public int Score;
		}

		public class CompetitionResult
		{
			public int ChampionshipCategoryId;
			public int Phase;
			public int Group;
			public CompetitionTeamResult[] TeamsResult;

			public int Competition;
			public Competitor[] Competitors;
		}

		private string SaveCompetitionTeamsResult(int cc, int phase, int group,
			CompetitionTeamResult[] teamsResult, SqlCommand command)
		{
			if (teamsResult == null)
				return "SaveCompetitionTeamsResult: teamsResult is null";

			if (teamsResult.Length == 0)
				return "SaveCompetitionTeamsResult: teamsResult has no items";

			string strSQL = "SELECT COUNT(*) " +
				"FROM CHAMPIONSHIP_GROUP_TEAMS " +
				"WHERE CHAMPIONSHIP_CATEGORY_ID = @0 AND " +
				" PHASE = @1 AND NGROUP = @2";
			SimpleTable table = DB.Instance.GetDataBySQL(strSQL, 
				new SimpleParameter("@0", cc), 
				new SimpleParameter("@1", phase), 
				new SimpleParameter("@2", group));
			int teamCount = (table.Rows.Count > 0) ? (int)table.Rows[0][0] : 0;
			if (teamsResult.Length != teamCount)
				return string.Format("SaveCompetitionTeamsResult: teamsResult length is {0} but teamCount is {1}", teamsResult.Length, teamCount);

			command.CommandText = "UPDATE CHAMPIONSHIP_GROUP_TEAMS " +
				"SET SCORE = @0, RESULT_POSITION = @1 " +
				"WHERE CHAMPIONSHIP_CATEGORY_ID = @C AND " +
				" PHASE = @P AND NGROUP = @G AND POSITION = @T";
			command.Parameters.Clear();
			command.Parameters.AddWithValue("@C", cc);
			command.Parameters.AddWithValue("@P", phase);
			command.Parameters.AddWithValue("@G", group);
			command.Parameters.AddWithValue("@T", 0);
			command.Parameters.AddWithValue("@0", 0);
			command.Parameters.AddWithValue("@1", 0);
			string strError = "";
			for (int t = 0; t < teamCount; t++)
			{
				command.Parameters["@T"].Value = t;
				command.Parameters["@0"].Value = teamsResult[t].Score;
				command.Parameters["@1"].Value = teamsResult[t].Position;
				try
				{
					command.ExecuteNonQuery();
				}
				catch (Exception ex)
				{
					strError = "Failed to update champion group team #" + (t + 1) + ": " + ex.Message;
					break;
				}
			}

			return strError;
		}

		[WebMethod]
		public string SetCompetitionTeamsResult(int championshipCategoryId, int phase,
			int group, CompetitionTeamResult[] teamsResult)
		{
			SqlConnection connection = DB.Instance.GetOpenConnection();
			SqlTransaction transaction = connection.BeginTransaction();
			SqlCommand command = new SqlCommand();
			command.Connection = connection;
			command.Transaction = transaction;
			command.CommandType = CommandType.Text;
			string strError = SaveCompetitionTeamsResult(championshipCategoryId, phase, group, teamsResult, command);
			if (strError.Length == 0)
			{
				try
				{
					transaction.Commit();
				}
				catch (Exception ex)
				{
					strError = "Error committing: " + ex.ToString();
				}
			}
			else
			{
				try
				{
					transaction.Rollback();
				}
				catch
				{ }
			}

			command.Dispose();
			transaction.Dispose();
			connection.Close();
			connection.Dispose();
			
			return strError;
		}

		private string SaveCompetitorsPosition(int cc, int phase, int group, int competition,
			int[] competitorsPosition, SqlCommand command)
		{
			if (competitorsPosition == null)
				return "SaveCompetitorsPosition: competitorsPosition is null";

			if (competitorsPosition.Length == 0)
				return "SaveCompetitorsPosition: competitorsPosition has no items";


			string strSQL = "SELECT COUNT(*) " +
				"FROM CHAMPIONSHIP_COMPETITION_COMPETITORS " +
				"WHERE CHAMPIONSHIP_CATEGORY_ID = @0 AND " +
				" PHASE = @1 AND NGROUP = @2 AND COMPETITION = @3";
			SimpleTable table = DB.Instance.GetDataBySQL(strSQL,
				new SimpleParameter("@0", cc),
				new SimpleParameter("@1", phase),
				new SimpleParameter("@2", group),
				new SimpleParameter("@3", competition));
			int competitorCount = (table.Rows.Count > 0) ? (int)table.Rows[0][0] : 0;
			if (competitorsPosition.Length != competitorCount)
				return string.Format("SaveCompetitorsPosition: competitorsPosition length is {0} but competitorCount is {1}", competitorsPosition.Length, competitorCount); ;

			command.CommandText = "UPDATE CHAMPIONSHIP_COMPETITION_COMPETITORS " +
				"SET RESULT_POSITION = @0 " +
				"WHERE CHAMPIONSHIP_CATEGORY_ID = @C AND " +
				" PHASE = @P AND NGROUP = @G AND COMPETITION = @M AND " +
				" COMPETITOR = @T";
			command.Parameters.Clear();
			command.Parameters.AddWithValue("@C", cc);
			command.Parameters.AddWithValue("@P", phase);
			command.Parameters.AddWithValue("@G", group);
			command.Parameters.AddWithValue("@M", competition);
			command.Parameters.AddWithValue("@T", 0);
			command.Parameters.AddWithValue("@0", 0);
			string strError = "";
			for (int c = 0; c < competitorCount; c++)
			{
				command.Parameters["@T"].Value = c;
				command.Parameters["@0"].Value = competitorsPosition[c];
				try
				{
					command.ExecuteNonQuery();
				}
				catch (Exception ex)
				{
					strError = "Error updating championship competition competitor #" + (c + 1) + ": " + ex.Message;
					break;
				}
			}

			return strError;
		}

		[WebMethod]
		public string SetCompetitorsPosition(int championshipCategoryId, int phase,
			int group, int competition, int[] competitorsPosition)
		{
			SqlConnection connection = DB.Instance.GetOpenConnection();
			SqlTransaction transaction = connection.BeginTransaction();
			SqlCommand command = new SqlCommand();
			command.Connection = connection;
			command.Transaction = transaction;
			command.CommandType = CommandType.Text;
			string strError = SaveCompetitorsPosition(championshipCategoryId, phase, group, competition, competitorsPosition, command);
			if (strError.Length == 0)
			{
				try
				{
					transaction.Commit();
				}
				catch (Exception ex)
				{
					strError = "Error committing: " + ex.ToString();
				}
			}
			else
			{
				try
				{
					transaction.Rollback();
				}
				catch
				{ }
			}

			command.Dispose();
			transaction.Dispose();
			connection.Close();
			connection.Dispose();

			return strError;
		}

		[WebMethod]
		public string SetCompetitorResult(CompetitorResult result)
		{
			SqlConnection connection = DB.Instance.GetOpenConnection();
			SqlTransaction transaction = connection.BeginTransaction();
			SqlCommand command = new SqlCommand();
			command.Connection = connection;
			command.Transaction = transaction;
			command.CommandType = CommandType.Text;
			string strError = SaveCompetitionTeamsResult(result.ChampionshipCategoryId, result.Phase, result.Group, result.TeamsResult, command);

			if (strError.Length == 0)
			{
				try
				{
					strError = SaveCompetitorsPosition(result.ChampionshipCategoryId, result.Phase, result.Group, result.Competition, result.CompetitorsPosition, command);
				}
				catch (Exception ex)
				{
					strError = "General error saving competitor position: " + ex.ToString();
				}

				if (strError.Length == 0)
				{
					command.CommandText = "UPDATE CHAMPIONSHIP_COMPETITION_COMPETITORS " +
						"SET RESULT = @0, " +
						"SCORE = @1 " +
						"WHERE CHAMPIONSHIP_CATEGORY_ID = @C AND " +
						" PHASE = @P AND " +
						" NGROUP = @G AND " +
						" COMPETITION = @M AND " +
						" COMPETITOR = @T";
					command.Parameters.Clear();
					command.Parameters.AddWithValue("@C", result.ChampionshipCategoryId);
					command.Parameters.AddWithValue("@P", result.Phase);
					command.Parameters.AddWithValue("@G", result.Group);
					command.Parameters.AddWithValue("@M", result.Competition);
					command.Parameters.AddWithValue("@T", result.Competitor);
					command.Parameters.AddWithValue("@0", DBNull.Value);
					command.Parameters.AddWithValue("@1", DBNull.Value);
					if (result.Result >= 0)
						command.Parameters["@0"].Value = result.Result;
					if (result.Score >= 0)
						command.Parameters["@1"].Value = result.Score;
					try
					{
						command.ExecuteNonQuery();
					}
					catch (Exception ex)
					{
						strError = "Error updating championship competition competitors: " + ex.Message;
					}
				}
			}

			if (strError.Length == 0)
			{
				try
				{
					transaction.Commit();
				}
				catch (Exception ex)
				{
					strError = "Error committing: " + ex.ToString();
				}
			}
			else
			{
				try
				{
					transaction.Rollback();
				}
				catch
				{ }
			}

			command.Dispose();
			transaction.Dispose();
			connection.Close();
			connection.Dispose();

			return strError;
		}

		[WebMethod]
		public bool SetCompetitorCustomPosition(int category, int phase, int group,
			int competition, int competitor, int heat, int customPosition)
		{
			string strSQL = "UPDATE CHAMPIONSHIP_COMPETITION_COMPETITORS " +
				"SET CUSTOM_POSITION = @position, RESULT_POSITION=-1 " +
				"WHERE CHAMPIONSHIP_CATEGORY_ID = @C AND " +
				"PHASE = @P AND " +
				"NGROUP = @G AND " +
				"COMPETITION = @M AND " +
				"COMPETITOR = @T AND " +
				"HEAT = @H";

			List<SimpleParameter> parameters = new List<SimpleParameter>();
			parameters.Add(new SimpleParameter("@position", customPosition));
			parameters.Add(new SimpleParameter("@C", category));
			parameters.Add(new SimpleParameter("@P", phase));
			parameters.Add(new SimpleParameter("@G", group));
			parameters.Add(new SimpleParameter("@M", competition));
			parameters.Add(new SimpleParameter("@T", competitor));
			parameters.Add(new SimpleParameter("@H", heat));
			return (DB.Instance.Execute(strSQL, parameters.ToArray()) > 0);
		}

		[WebMethod]
		public string SetCompetitionResult(CompetitionResult result)
		{
			SqlConnection connection = DB.Instance.GetOpenConnection();
			SqlTransaction transaction = connection.BeginTransaction();
			SqlCommand command = new SqlCommand();
			command.Connection = connection;
			command.Transaction = transaction;
			command.CommandType = CommandType.Text;
			string strError = SaveCompetitionTeamsResult(result.ChampionshipCategoryId, result.Phase, result.Group, result.TeamsResult, command);

			if (strError.Length == 0)
			{
				// Deleting competitors
				command.CommandText = "DELETE CHAMPIONSHIP_COMPETITION_COMPETITORS " +
					"WHERE CHAMPIONSHIP_CATEGORY_ID = @C AND " +
						" PHASE = @P AND NGROUP = @G AND COMPETITION = @M";
				command.Parameters.Clear();
				command.Parameters.AddWithValue("@C", result.ChampionshipCategoryId);
				command.Parameters.AddWithValue("@P", result.Phase);
				command.Parameters.AddWithValue("@G", result.Group);
				command.Parameters.AddWithValue("@M", result.Competition);
				try
				{
					command.ExecuteNonQuery();
				}
				catch (Exception ex)
				{
					strError = "Failed to delete championship competition competitors: " + ex.Message;
				}

				if (strError.Length == 0)
				{
					if (result.Competitors != null)
					{
						foreach (Competitor competitor in result.Competitors)
						{
							competitor.CustomPosition = -1;
						}
					}

					try
					{
						strError = SaveCompetitors(result.ChampionshipCategoryId, result.Phase, result.Group, result.Competition, result.Competitors, command);
					}
					catch (Exception ex)
					{
						strError = "General error saving competitors: " + ex.ToString();
					}
				}
			}

			if (strError.Length == 0)
			{
				try
				{
					transaction.Commit();
				}
				catch (Exception ex)
				{
					strError = "Error committing: " + ex.ToString();
				}
			}
			else
			{
				try
				{
					transaction.Rollback();
				}
				catch
				{ }
			}

			command.Dispose();
			transaction.Dispose();
			connection.Close();
			connection.Dispose();

			return strError;
		}

		#endregion
	}
}
