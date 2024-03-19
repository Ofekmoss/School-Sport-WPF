using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Sport.Championships
{
	#region CompetitorResult Class

	public class CompetitorResult
	{
		private int _playerNumber;
		public int PlayerNumber
		{
			get { return _playerNumber; }
		}

		private int _result;
		public int Result
		{
			get { return _result; }
		}

		private int[] _sharedResultNumbers;
		public int[] SharedResultNumbers
		{
			get { return _sharedResultNumbers; }
		}

		private int _lastResultDisqualifications;
		public int LastResultDisqualifications
		{
			get { return _lastResultDisqualifications; }
		}

		private int _totalDisqualifications;
		public int TotalDisqualifications
		{
			get { return _totalDisqualifications; }
		}

		private string _wind;
		public string Wind
		{
			get { return _wind; }
		}


		public CompetitorResult(int playerNumber, int result, int[] sharedNumbers, int lastResultDisqualifications, int totalDisqualifications, string wind)
		{
			_playerNumber = playerNumber;
			_result = result;
			_sharedResultNumbers = sharedNumbers;
			_lastResultDisqualifications = lastResultDisqualifications;
			_totalDisqualifications = totalDisqualifications;
			_wind = wind;
		}

		public CompetitorResult(int playerNumber, int result)
			:this(playerNumber, result, null, 0, 0, "")
		{
		}
	}

	#endregion

	public class Competition : Sport.Common.GeneralCollection.CollectionItem
	{
		#region Properties

		internal int _index;
		public int Index
		{
			get { return _index; }
		}

		private Sport.Entities.SportField _sportField;
		public Sport.Entities.SportField SportField
		{
			get { return _sportField; }
			set
			{
				if (!Editable)
					throw new ChampionshipException("Not in edit - cannot be set");

				_sportField = value;
			}
		}

		private DateTime _time;
		public DateTime Time
		{
			get { return _time; }
		}

		private Sport.Entities.Facility _facility;
		public Sport.Entities.Facility Facility
		{
			get { return _facility; }
		}

		private Sport.Entities.Court _court;
		public Sport.Entities.Court Court
		{
			get { return _court; }
		}

		private int _laneCount;
		public int LaneCount
		{
			get
			{
				if (_laneCount < 1)
					return 1;
				return _laneCount;
			}
			set { _laneCount = value; }
		}

		public string Name
		{
			get { return _sportField.Name; }
		}

		public bool Editable
		{
			get
			{
				return Group == null || Group.Editable;
			}
		}

		public bool IsRelayRace()
		{
			if (this.SportField != null && this.SportField.SportFieldType != null)
				return (this.SportField.SportFieldType.Name.IndexOf("שליחים") >= 0);
			return false;
		}
		#endregion

		public Dictionary<int, Sport.Championships.Competitor> MapCompetitors()
		{
			Dictionary<int, Sport.Championships.Competitor> mapping = new Dictionary<int, Sport.Championships.Competitor>();
			if (this.Competitors != null && this.Competitors.Count > 0)
			{
				this.Competitors.OfType<Sport.Championships.Competitor>().ToList().ForEach(competitor =>
				{
					int playerNumber = competitor.PlayerNumber;
					if (playerNumber > 0 && !mapping.ContainsKey(playerNumber))
						mapping.Add(playerNumber, competitor);
				});
			}
			return mapping;
		}

		#region Rules
		public object GetRule(System.Type valueType, System.Type ruleType)
		{
			if (!Sport.Core.Session.Connected)
			{
				int categoryID = this.Group.Phase.Championship.CategoryID;
				return Sport.Rulesets.Ruleset.LoadOfflineRule(ruleType,
					categoryID, this.SportField.Id);
			}
			if (this.Group == null || this.Group.Phase == null || this.Group.Phase.Championship == null || this.Group.Phase.Championship.ChampionshipCategory == null)
				return null;
			return Group.Phase.Championship.ChampionshipCategory.GetRule(valueType, SportField);
		}

		private Sport.Rulesets.Rules.ResultType _resultType;
		public Sport.Rulesets.Rules.ResultType ResultType
		{
			get
			{
				if (_resultType == null)
				{
					object rawResultType = GetRule(typeof(Sport.Rulesets.Rules.ResultType),
						typeof(Sport.Rulesets.Rules.ResultTypeRule));
					if (rawResultType is Sport.Rulesets.Rules.ResultType)
					{
						_resultType = (Sport.Rulesets.Rules.ResultType)rawResultType;
					}
				}

				return _resultType;
			}
		}

		private Sport.Rulesets.Rules.GeneralSportTypeData _generalData = null;
		public Sport.Rulesets.Rules.GeneralSportTypeData GeneralData
		{
			get
			{
				if (_generalData == null)
				{
					_generalData = (Sport.Rulesets.Rules.GeneralSportTypeData)
						GetRule(typeof(Sport.Rulesets.Rules.GeneralSportTypeData),
						typeof(Sport.Rulesets.Rules.GeneralSportTypeDataRule));
				}

				return _generalData;
			}
		}

		private Sport.Rulesets.Rules.TeamScoreCounters _teamScoreCounters = null;
		public Sport.Rulesets.Rules.TeamScoreCounters ScoreCounters
		{
			get
			{
				if (_teamScoreCounters == null)
				{
					_teamScoreCounters = (Sport.Rulesets.Rules.TeamScoreCounters)
						GetRule(typeof(Sport.Rulesets.Rules.TeamScoreCounters),
						typeof(Sport.Rulesets.Rules.TeamScoreCountersRule));
				}

				return _teamScoreCounters;
			}
		}

		private Sport.Rulesets.Rules.CompetitorCompetitions _competitorCompetitions = null;
		public Sport.Rulesets.Rules.CompetitorCompetitions CompetitorCompetitions
		{
			get
			{
				if (_competitorCompetitions == null)
				{
					_competitorCompetitions = (Sport.Rulesets.Rules.CompetitorCompetitions)
						GetRule(typeof(Sport.Rulesets.Rules.CompetitorCompetitions),
						typeof(Sport.Rulesets.Rules.CompetitorCompetitionsRule));
				}

				return _competitorCompetitions;
			}
		}

		private Sport.Rulesets.Rules.TeamScoreCounter _teamScoreCounter = null;
		public Sport.Rulesets.Rules.TeamScoreCounter ScoreCounter
		{
			get
			{
				if (_teamScoreCounter == null)
				{
					_teamScoreCounter = (Sport.Rulesets.Rules.TeamScoreCounter)
						GetRule(typeof(Sport.Rulesets.Rules.TeamScoreCounter),
						typeof(Sport.Rulesets.Rules.TeamScoreCounterRule));
				}

				return _teamScoreCounter;
			}
		}

		private Sport.Rulesets.Rules.ScoreTable _scoreTable;
		public Sport.Rulesets.Rules.ScoreTable ScoreTable
		{
			get
			{
				if (_scoreTable == null)
				{
					object rule = GetRule(typeof(Sport.Rulesets.Rules.ScoreTable),
						typeof(Sport.Rulesets.Rules.ScoreTableRule));
					if (rule == null)
						return null;
					_scoreTable = (Sport.Rulesets.Rules.ScoreTable)rule;
				}

				return _scoreTable;
			}
		}

		private string _sportFieldCounter = null;
		public string SportFieldCounter
		{
			get
			{
				if (_sportFieldCounter == null)
				{
					Sport.Rulesets.Rules.TeamScoreCounter teamScoreCounter = (Sport.Rulesets.Rules.TeamScoreCounter)
						GetRule(typeof(Sport.Rulesets.Rules.TeamScoreCounter),
						typeof(Sport.Rulesets.Rules.TeamScoreCounterRule));

					if (teamScoreCounter == null)
					{
						_sportFieldCounter = string.Empty;
					}
					else
					{
						_sportFieldCounter = teamScoreCounter.Counter;
					}
				}

				return _sportFieldCounter;
			}
		}
		private string _sportFieldTypeCounter = null;
		public string SportFieldTypeCounter
		{
			get
			{
				if (_sportFieldTypeCounter == null)
				{
					Sport.Rulesets.Rules.TeamScoreCounter teamScoreCounter = null;
					if (Sport.Core.Session.Connected)
					{
						teamScoreCounter = (Sport.Rulesets.Rules.TeamScoreCounter)
							Group.Phase.Championship.ChampionshipCategory.GetRule(
							typeof(Sport.Rulesets.Rules.TeamScoreCounter), SportField.SportFieldType);
					}
					else
					{
						object rule = Sport.Rulesets.Ruleset.LoadOfflineRule(
							typeof(Sport.Rulesets.Rules.TeamScoreCounterRule),
							this.Group.Phase.Championship.CategoryID, this.SportField.Id);
						if (rule != null)
						{
							teamScoreCounter =
								(Sport.Rulesets.Rules.TeamScoreCounter)rule;
						}
					}

					if (teamScoreCounter == null)
					{
						_sportFieldTypeCounter = string.Empty;
					}
					else
					{
						_sportFieldTypeCounter = teamScoreCounter.Counter;
					}
				}

				return _sportFieldTypeCounter;
			}
		}

		internal void OnRulesetChange()
		{
			_resultType = null;
			_scoreTable = null;
			_sportFieldCounter = null;
			_sportFieldTypeCounter = null;
		}
		#endregion

		#region Competition Information

		// The participance of the teams in the competition
		private int[] _teamsParticipance = null;
		public int[] TeamsParticipance
		{
			get
			{
				if (_teamsParticipance == null)
				{
					_teamsParticipance = new int[Group.Teams.Count];

					foreach (Competitor competitor in _competitors)
					{
						_teamsParticipance[competitor.Team]++;
					}
				}

				return _teamsParticipance;
			}
		}

		#endregion

		#region CollectionItem Members

		public override void OnOwnerChange(object oo, object no)
		{
			if (no == null)
				_index = -1;
			_resultType = null;
		}

		public CompetitionGroup Group
		{
			get { return ((CompetitionGroup)Owner); }
		}

		#endregion

		#region Heats

		#region HeatCollection

		public class HeatCollection : Sport.Common.GeneralCollection
		{
			public HeatCollection(Competition competition)
				: base(competition)
			{
			}

			protected override void SetItem(int index, object value)
			{
				if (!((Competition)Owner).Editable)
					throw new ChampionshipException("Not in group edit - cannot change heats");

				base.SetItem(index, value);
			}

			protected override void InsertItem(int index, object value)
			{
				if (!((Competition)Owner).Editable)
					throw new ChampionshipException("Not in group edit - cannot change heats");

				base.InsertItem(index, value);
			}

			protected override void RemoveItem(int index)
			{
				if (!((Competition)Owner).Editable)
					throw new ChampionshipException("Not in group edit - cannot change heats");

				base.RemoveItem(index);
			}

			public Heat this[int index]
			{
				get
				{
					if ((index < 0) || (index >= this.Count))
						return null;
					return (Heat)GetItem(index);
				}
				set { SetItem(index, value); }
			}

			public void Insert(int index, Heat value)
			{
				InsertItem(index, value);
			}

			public void Remove(Heat value)
			{
				RemoveItem(value);
			}

			public bool Contains(Heat value)
			{
				return base.Contains(value);
			}

			public int IndexOf(Heat value)
			{
				return base.IndexOf(value);
			}

			public int Add(Heat value)
			{
				return AddItem(value);
			}
		}

		#endregion

		private HeatCollection _heats;
		public HeatCollection Heats
		{
			get { return _heats; }
		}

		private void HeatsChanged(object sender, Sport.Common.CollectionEventArgs e)
		{
			if (e.EventType == Sport.Common.CollectionEventType.Remove)
			{
				foreach (Competitor competitor in _competitors)
				{
					if (competitor.Heat == e.Index)
						competitor.Heat = -1;
				}
			}

			for (int i = e.Index; i < _heats.Count; i++)
				_heats[i]._index = i;
		}

		#endregion

		#region Competitors

		#region CompetitorCollection

		public class CompetitorCollection : Sport.Common.GeneralCollection
		{
			private System.Collections.Hashtable numbersIndex;
			public CompetitorCollection(Competition competition)
				: base(competition)
			{
				numbersIndex = new System.Collections.Hashtable();
			}

			protected override void SetItem(int index, object value)
			{
				/* if (!((Competition) Owner).Editable)
					throw new ChampionshipException("Not in group edit - cannot change competitors"); */

				numbersIndex.Remove(this[index].PlayerNumber);
				base.SetItem(index, value);
				numbersIndex[((Competitor)value).PlayerNumber] = value;
			}

			protected override void InsertItem(int index, object value)
			{
				/* if (!((Competition) Owner).editingResult && !((Competition) Owner).Editable)
					throw new ChampionshipException("Not in group edit - cannot change competitors"); */

				base.InsertItem(index, value);
				numbersIndex[((Competitor)value).PlayerNumber] = value;
			}

			protected override void RemoveItem(int index)
			{
				/* if (!((Competition) Owner).Editable)
					throw new ChampionshipException("Not in group edit - cannot change competitors"); */

				numbersIndex.Remove(this[index].PlayerNumber);
				base.RemoveItem(index);
			}

			public Competitor this[int index]
			{
				get { return (Competitor)GetItem(index); }
				set { SetItem(index, value); }
			}

			public void Insert(int index, Competitor value)
			{
				InsertItem(index, value);
			}

			public void Remove(Competitor value)
			{
				RemoveItem(value);
			}

			public bool Contains(Competitor value)
			{
				return base.Contains(value);
			}

			public int IndexOf(Competitor value)
			{
				return base.IndexOf(value);
			}

			public int Add(Competitor value)
			{
				return AddItem(value);
			}

			public Competitor GetCompetitorByNumber(int number)
			{
				return numbersIndex[number] as Competitor;
			}
		}

		#endregion

		private CompetitorCollection _competitors;
		public CompetitorCollection Competitors
		{
			get { return _competitors; }
		}

		private void CompetitorsChanged(object sender, Sport.Common.CollectionEventArgs e)
		{
			for (int i = e.Index; i < _competitors.Count; i++)
				_competitors[i]._index = i;

			_teamsParticipance = null;
		}

		public void ResetCompetitorsPosition()
		{
			int[] positions = new int[_heats.Count + 1];
			for (int i = 0; i < positions.Length; i++)
				positions[i] = 0;
			foreach (Competitor competitor in _competitors)
			{
				int index = competitor.Heat + 1;
				if (index < 0)
					index = 0;
				if (index >= positions.Length)
					index = positions.Length - 1;
				competitor._position = positions[index];
				positions[index]++;
			}
		}

		#endregion

		#region Status

		public bool HasResults()
		{
			foreach (Competitor competitor in _competitors)
			{
				if (competitor.Result >= 0)
					return true;
			}

			return false;
		}

		public void Reset()
		{
			foreach (Competitor competitor in _competitors)
			{
				competitor.Reset();
			}
		}

		#endregion

		#region Constructors

		public Competition(Sport.Entities.SportField sportField)
		{
			_heats = new HeatCollection(this);
			_heats.Changed += new Sport.Common.CollectionEventHandler(HeatsChanged);

			_competitors = new CompetitorCollection(this);
			_competitors.Changed += new Sport.Common.CollectionEventHandler(CompetitorsChanged);

			_sportFieldCounter = null;
			_sportFieldTypeCounter = null;
			_index = -1;
			_sportField = sportField;
			_time = DateTime.MinValue;
			_court = null;
			_laneCount = 0;
			_facility = null;
		}

		internal Competition(SportServices.Competition scompetition)
			: this(new Sport.Entities.SportField(scompetition.SportField))
		{
			_sportFieldCounter = null;
			_sportFieldTypeCounter = null;
			_time = scompetition.Time;
			_laneCount = scompetition.LaneCount;

			if (scompetition.Facility == -1)
			{
				_facility = null;
			}
			else
			{
				_facility = new Sport.Entities.Facility(scompetition.Facility);
				if (!_facility.IsValid())
					_facility = null;
			}

			if (scompetition.Court == -1)
			{
				_court = null;
			}
			else
			{
				_court = new Sport.Entities.Court(scompetition.Court);
				if (!_court.IsValid())
					_court = null;
			}

			foreach (SportServices.Heat sheat in scompetition.Heats)
			{
				_heats.Add(new Heat(sheat));
			}

			foreach (SportServices.Competitor scompetitor in scompetition.Competitors)
			{
				_competitors.Add(new Competitor(scompetitor));
			}
		}

		#endregion

		public Sport.Documents.Data.Table GetCompetitorsTable()
		{
			//get competition type:
			Sport.Types.CompetitionType compType =
				this.SportField.SportFieldType.CompetitionType;

			//group competition type?
			bool blnGroupCompetition =
				(compType == Sport.Types.CompetitionType.Group);

			//get competitors having postition:
			ArrayList arrCompetitors = new ArrayList();
			foreach (Competitor competitor in this.Competitors)
				if (competitor.Result > 0)
					arrCompetitors.Add(competitor);
			Sport.Entities.OfflinePlayer[] arrOfflinePlayers =
				this.GetOfflinePlayers();
			foreach (Sport.Entities.OfflinePlayer offlinePlayer in arrOfflinePlayers)
				if (offlinePlayer.Result > 0)
					arrCompetitors.Add(offlinePlayer);

			//sort competitors by result:
			Sport.Core.Data.ResultDirection direction =
				Sport.Core.Data.ResultDirection.Most;
			if (this.ResultType != null)
				direction = this.ResultType.Direction;
			arrCompetitors.Sort(new Competition.CompetitorResultComparer(direction));

			//build competitors table.

			int colCount = 6;
			Sport.Documents.Data.Table table = new Sport.Documents.Data.Table();
			table.Headers = new Sport.Documents.Data.Column[colCount];
			table.Headers[0] = new Sport.Documents.Data.Column("דירוג", 0.08);
			table.Headers[1] = new Sport.Documents.Data.Column("חזה", 0.1);
			table.Headers[2] = new Sport.Documents.Data.Column(
				((blnGroupCompetition) ? "שם מוסד" : "שם משתתף"), 0.28);
			table.Headers[3] = new Sport.Documents.Data.Column("שם הקבוצה", 0.28);
			table.Headers[4] = new Sport.Documents.Data.Column("תוצאה", 0.15);
			table.Headers[5] = new Sport.Documents.Data.Column("ניקוד", 0.11);
			table.Rows = new Sport.Documents.Data.Row[arrCompetitors.Count];

			//iterate over competitors:
			int row = 0;
			//int position = 0;
			//int prevScore = 0;
			//int sameScoreCounter = 0;
			int curPosition = -1;
			int lastResult = -1;
			foreach (object oComp in arrCompetitors)
			{
				//initialize column index:
				int col = 0;

				//initialize data:
				string strPlayerName = "";
				string strTeamName = "";
				string shirtNumber = "";
				int curResult = 0;
				int curScore = 0;

				//what we got here?
				if (oComp is Sport.Championships.Competitor)
				{
					//get competitor:
					Competitor competitor = (Competitor)oComp;

					//player and team:
					if (competitor.Player != null)
					{
						//player:
						if (competitor.Player.PlayerEntity != null)
							strPlayerName = competitor.Player.PlayerEntity.Name;

						//team:
						if ((competitor.Player.CompetitionTeam != null) && (competitor.Player.CompetitionTeam.TeamEntity != null))
							strTeamName = competitor.Player.CompetitionTeam.TeamEntity.TeamName();
					}

					//shirt number:
					shirtNumber = competitor.PlayerNumber.ToString();

					if (competitor.SharedResultNumbers != null && competitor.SharedResultNumbers.Length > 1)
					{
						shirtNumber = string.Join(", ", competitor.SharedResultNumbers);
						strPlayerName = "[תוצאה משותפת]";
						string names = competitor.GetSharedResultNames();
						if (names.Length > 0)
							strPlayerName += " " + names;
					}

					//result and score:
					curResult = competitor.Result;
					curScore = competitor.Score;
				}
				else if (oComp is Sport.Entities.OfflinePlayer)
				{
					//get offline player:
					Sport.Entities.OfflinePlayer oPlayer =
						(Sport.Entities.OfflinePlayer)oComp;

					strPlayerName = oPlayer.ToString();
					if (oPlayer.Team != null)
						strTeamName = oPlayer.Team.TeamName();
					else if (oPlayer.OfflineTeam != null)
						strTeamName = oPlayer.OfflineTeam.ToString();
					shirtNumber = oPlayer.ShirtNumber.ToString();
					curResult = oPlayer.Result;
					curScore = oPlayer.Score;
				}

				//got any team?
				if ((strTeamName == null) || (strTeamName.Length == 0))
					continue;

				//got score?
				if (curScore <= 0)
					continue;

				//higher position?
				if (lastResult < 0 || curResult != lastResult)
					curPosition++;

				//build current row
				table.Rows[row] = new Sport.Documents.Data.Row();
				table.Rows[row].Cells = new Sport.Documents.Data.Cell[colCount];

				//rank:
				string strPosition = (row + 1).ToString(); //(curPosition + 1).ToString();

				/*
				if (curScore == prevScore)
				{
					strPosition = (position - sameScoreCounter).ToString();
					sameScoreCounter++;
				}
				else
				{
					sameScoreCounter = 0;
					strPosition = (position + 1).ToString();
				}
				*/
				table.Rows[row].Cells[col++] = new Sport.Documents.Data.Cell(strPosition);

				//shirt number
				if (blnGroupCompetition)
					shirtNumber = "";
				table.Rows[row].Cells[col++] = new Sport.Documents.Data.Cell(shirtNumber);

				//competitor name:
				if (blnGroupCompetition)
				{
					strPlayerName = strTeamName;
				}
				table.Rows[row].Cells[col++] = new Sport.Documents.Data.Cell(strPlayerName);

				//team name:
				table.Rows[row].Cells[col++] = new Sport.Documents.Data.Cell(strTeamName);

				//result
				string strResult = "";
				if ((this.ResultType != null) && (curResult > 0))
					strResult = this.ResultType.FormatResult(curResult);
				table.Rows[row].Cells[col++] = new Sport.Documents.Data.Cell(strResult);

				//score
				string strScore = curScore.ToString();
				table.Rows[row].Cells[col++] = new Sport.Documents.Data.Cell(strScore);

				//advance to next row...
				row++;
				//position++;

				//save score:
				//prevScore = curScore;
				lastResult = curResult;
			} //end loop over competitors

			//done.
			return table;
		}

		public Sport.Entities.OfflinePlayer[] GetOfflinePlayers()
		{
			Sport.Data.OfflineEntity[] arrAllPlayers =
				Sport.Data.OfflineEntity.LoadAllEntities(
				typeof(Sport.Entities.OfflinePlayer));
			ArrayList result = new ArrayList();
			if ((arrAllPlayers != null) && (arrAllPlayers.Length > 0))
			{
				int champCategoryID = this.Group.Phase.Championship.CategoryID;
				foreach (Sport.Entities.OfflinePlayer player in arrAllPlayers)
				{
					int curCategoryID = -1;
					if (player.Team != null)
						curCategoryID = player.Team.Category.Id;
					else if (player.OfflineTeam != null)
						curCategoryID = player.OfflineTeam.ChampionshipCategory;
					if (curCategoryID != champCategoryID)
						continue;
					if (player.Competition != this.Index)
						continue;
					result.Add(player);
				}
			}
			return (Sport.Entities.OfflinePlayer[])
				result.ToArray(typeof(Sport.Entities.OfflinePlayer));
		}

		internal SportServices.Competition Save()
		{
			SportServices.Competition scompetition = new SportServices.Competition();
			scompetition.SportField = _sportField.Id;
			scompetition.Time = _time;
			scompetition.Facility = _facility == null ? -1 : _facility.Id;
			scompetition.Court = _court == null ? -1 : _court.Id;
			scompetition.LaneCount = _laneCount;

			scompetition.Heats = new SportServices.Heat[_heats.Count];

			for (int n = 0; n < _heats.Count; n++)
			{
				scompetition.Heats[n] = _heats[n].Save();
			}

			scompetition.Competitors = new SportServices.Competitor[_competitors.Count];
			for (int n = 0; n < _competitors.Count; n++)
			{
				scompetition.Competitors[n] = _competitors[n].Save();
			}
			scompetition.Competitors = (SportServices.Competitor[])Sport.Common.Tools.RemoveNullValues(scompetition.Competitors, typeof(SportServices.Competitor));

			return scompetition;
		}

		public bool Set(DateTime time, Sport.Entities.Facility facility, Sport.Entities.Court court, bool doNotSave)
		{
			if (doNotSave)
			{
				_time = time;
				_facility = facility;
				_court = court;
				return true;
			}

			CompetitionPhase phase = this.Group.Phase;
			CompetitionChampionship champ = phase.Championship;
			int ccid = champ.ChampionshipCategory.Id;
			int facilityID = (facility == null) ? -1 : facility.Id;
			int courtID = (court == null) ? -1 : court.Id;
			SportServices.ChampionshipService cs = new SportServices.ChampionshipService();
			cs.CookieContainer = Sport.Core.Session.Cookies;
			if (cs.SaveCompetition(ccid, phase.Index, this.Group.Index,
				this.SportField.Id, this._index, time, facilityID, courtID))
			{
				_time = time;
				_facility = facility;
				_court = court;
				return true;
			}
			return false;
		}

		public bool Set(DateTime time, Sport.Entities.Facility facility, Sport.Entities.Court court)
		{
			return Set(time, facility, court, false);
		}

		public override string ToString()
		{
			return Name;
		}

		public int[] GetCompetitorsPosition()
		{
			int[] competitorsPosition = new int[_competitors.Count];
			for (int n = 0; n < _competitors.Count; n++)
				competitorsPosition[n] = _competitors[n].ResultPosition;
			return competitorsPosition;
		}

		private class ResultComparer : System.Collections.IComparer
		{
			Sport.Rulesets.Rules.ResultType _resultType;
			public ResultComparer(Sport.Rulesets.Rules.ResultType resultType)
			{
				_resultType = resultType;
			}

			#region IComparer Members
			public int Compare(object x, object y)
			{
				if ((x == null) && (y == null))
					return 0;

				if (x == null)
					return -1;

				if (y == null)
					return 1;

				int result1 = (int)x;
				int result2 = (int)y;

				if (result1 == result2)
					return 0;

				if (result1 < 0)
					return -1;

				if (result2 < 0)
					return 1;

				if (_resultType.Direction == Sport.Core.Data.ResultDirection.Most)
				{
					if (result1 > result2)
						return 1;
				}
				else
				{
					if (result1 < result2)
						return 1;
				}
				return -1;
			}
			#endregion
		}

		public void CalculateCompetitorsPosition()
		{
			ArrayList arrCompetitors = new ArrayList(_competitors);
			arrCompetitors.AddRange(this.GetOfflinePlayers());
			Sport.Core.Data.ResultDirection direction =
				Sport.Core.Data.ResultDirection.Most;
			if (this.ResultType != null)
				direction = this.ResultType.Direction;
			arrCompetitors.Sort(new CompetitorResultComparer(direction));
			int curPosition = -1;
			int lastResult = -1;
			int prevLastResultDisqualifications = -1;
			int prevTotalDisqualifications = -1;
			for (int i = 0; i < arrCompetitors.Count; i++)
			{
				object oComp = arrCompetitors[i];
				int curResult = GetResult(oComp);
				int curLastResultDisqualifications = -1;
				int curTotalDisqualifications = -1;
				if (oComp is Sport.Championships.Competitor)
				{
					curLastResultDisqualifications = (oComp as Sport.Championships.Competitor).LastResultDisqualifications;
					curTotalDisqualifications = (oComp as Sport.Championships.Competitor).TotalDisqualifications;
				}

				if (lastResult < 0 || curResult != lastResult || 
					(curResult == lastResult && (curLastResultDisqualifications != prevLastResultDisqualifications || curTotalDisqualifications != prevTotalDisqualifications)))
					curPosition++;

				if (oComp is Sport.Championships.Competitor)
				{
					(oComp as Sport.Championships.Competitor)._resultPosition = curPosition;
				}
				else if (oComp is Sport.Entities.OfflinePlayer)
				{
					(oComp as Sport.Entities.OfflinePlayer).Rank = curPosition;
					(oComp as Sport.Entities.OfflinePlayer).Save();
				}

				lastResult = curResult;
				prevLastResultDisqualifications = curLastResultDisqualifications;
				prevTotalDisqualifications = curTotalDisqualifications;
			}

			/*
			Competitor[] competitors = new Competitor[_competitors.Count];
			_competitors.CopyTo(competitors, 0);
			int[] results = new int[Competitors.Count];
			for (int n = 0; n < results.Length; n++)
				results[n] = _competitors[n].Result;
			
			if (this.ResultType != null)
				Array.Sort(results, competitors, new ResultComparer(this.ResultType));
			
			for (int n = 0; n < competitors.Length; n++)
			{
				_competitors[competitors[n].Index]._resultPosition = 
					competitors.Length - n - 1;
			}
			*/
		}

		private int GetResult(object oCompetitor)
		{
			if (oCompetitor is Sport.Championships.Competitor)
			{
				return (oCompetitor as Sport.Championships.Competitor).Result;
			}
			else if (oCompetitor is Sport.Entities.OfflinePlayer)
			{
				return (oCompetitor as Sport.Entities.OfflinePlayer).Result;
			}
			return 0;
		}

		//private bool editingResult = false;

		public string SetResults(int heat, CompetitorResult[] results, bool sendToServer)
		{
			// Storing old values
			int prevCount = _competitors.Count; // on rollback, all competitors after prevCount will be deleted
			int[] prevResults = new int[prevCount];
			for (int n = 0; n < prevCount; n++)
				prevResults[n] = _competitors[n].Result;

			//editingResult = true;

			// Setting results
			Competitor competitor;
			Sport.Entities.OfflinePlayer[] arrOfflinePlayers =
				this.GetOfflinePlayers();
			Hashtable tblOfflinePlayers = new Hashtable();
			foreach (Sport.Entities.OfflinePlayer offlinePlayer in arrOfflinePlayers)
				tblOfflinePlayers[offlinePlayer.ShirtNumber] = offlinePlayer;
			Sport.Rulesets.Rules.ScoreTable scoreTable = this.ScoreTable;
			Sport.Rulesets.Rules.GeneralSportTypeData generalData = this.GeneralData;
			for (int n = 0; n < results.Length; n++)
			{
				//get current shirt number:
				int curPlayerNumber = results[n].PlayerNumber;

				//get current result:
				int curResult = results[n].Result;

				int[] sharedResultNumbers = results[n].SharedResultNumbers;
				int lastResultDisqualifications = results[n].LastResultDisqualifications;
				int totalDisqualifications = results[n].TotalDisqualifications;
				string wind = results[n].Wind;

				//get current score:
				int curScore = (scoreTable == null) ? 0 : scoreTable.GetScore(curResult);

				//double?
				if ((generalData != null) && (generalData.DoubleScore))
					curScore = curScore * 2;

				//offline player?
				if (tblOfflinePlayers[curPlayerNumber] != null)
				{
					//update score and save.
					Sport.Entities.OfflinePlayer curOfflinePlayer =
						(Sport.Entities.OfflinePlayer)tblOfflinePlayers[curPlayerNumber];
					curOfflinePlayer.Result = curResult;
					curOfflinePlayer.Score = curScore;
					curOfflinePlayer.Save();
					continue;
				}

				//search competitor in the competition:
				competitor = _competitors.GetCompetitorByNumber(curPlayerNumber);

				//got anything?
				if (competitor == null)
				{
					// inserting new competitor
					competitor = new Competitor(curPlayerNumber);
					_competitors.Add(competitor);
				}

				//assign heat?
				if (heat != -1)
					competitor._heat = heat;

				//assign result and score to current competitor:
				competitor._result = curResult;
				competitor._sharedResultNumbers = sharedResultNumbers;
				competitor._lastResultDisqualifications = lastResultDisqualifications;
				competitor._totalDisqualifications = totalDisqualifications;
				competitor._wind = wind;
				competitor._score = curScore;
			} //end loop over results.

			//score is rank?
			if ((generalData != null) && (generalData.ScoreIsRank))
			{
				//initialize list of competitors having result:
				ArrayList arrCompetitorsWithResult = new ArrayList();

				int abNormalResults = 998;
				foreach (Sport.Championships.Competitor curCompetitor in this.Competitors)
				{
					if (curCompetitor.Result > 0)
						arrCompetitorsWithResult.Add(curCompetitor);
					else
					{
						curCompetitor._score = abNormalResults++;
						curCompetitor._position = abNormalResults;
						curCompetitor._customPosition = abNormalResults;
						arrCompetitorsWithResult.Add(curCompetitor);
					}
				}

				//add offline players:
				foreach (Sport.Entities.OfflinePlayer curOfflinePlayer in tblOfflinePlayers.Values)
				{
					if (curOfflinePlayer.Result > 0)
						arrCompetitorsWithResult.Add(curOfflinePlayer);
				}

				//sort by result:
				Sport.Core.Data.ResultDirection direction =
					Sport.Core.Data.ResultDirection.Most;
				if (this.ResultType != null)
					direction = this.ResultType.Direction;
				arrCompetitorsWithResult.Sort(new CompetitorResultComparer(direction));

				//assign the new score according to position:
				for (int i = 0; i < arrCompetitorsWithResult.Count; i++)
				{
					int curScore = (i + 1);
					if (arrCompetitorsWithResult[i] is Sport.Championships.Competitor)
					{
						(arrCompetitorsWithResult[i] as Competitor)._score = curScore;
					}
					else if (arrCompetitorsWithResult[i] is Sport.Entities.OfflinePlayer)
					{
						(arrCompetitorsWithResult[i] as Sport.Entities.OfflinePlayer).Score = curScore;
						(arrCompetitorsWithResult[i] as Sport.Entities.OfflinePlayer).Save();
					}
				}
			} //end if score is rank.

			//editingResult = false;

			ResetCompetitorsPosition();
			CalculateCompetitorsPosition();
			Group.CalculateTeamsScore();

			if (sendToServer)
			{
				if (Sport.Core.Session.Connected)
				{
					SportServices.CompetitionResult sresult = new SportServices.CompetitionResult();
					sresult.ChampionshipCategoryId = Group.Phase.Championship.ChampionshipCategory.Id;
					sresult.Phase = Group.Phase.Index;
					sresult.Group = Group.Index;
					sresult.Competition = _index;
					sresult.TeamsResult = Group.GetTeamResult();
					sresult.Competitors = new SportServices.Competitor[_competitors.Count];

					for (int n = 0; n < _competitors.Count; n++)
						sresult.Competitors[n] = _competitors[n].Save();

					SportServices.ChampionshipService cs = new SportServices.ChampionshipService();
					cs.CookieContainer = Sport.Core.Session.Cookies;
					string strError = string.Empty;
					try
					{
						strError = cs.SetCompetitionResult(sresult);
					}
					catch (Exception ex)
					{
						strError = "general error while setting result: " + ex.Message;
						if (ex.Message.IndexOf("The underlying connection was closed") < 0)
							Sport.Data.AdvancedTools.ReportExcpetion(ex);
					}
					if (strError.Length > 0)
					{
						// Setting to previous result
						for (int n = 0; n < prevCount; n++)
						{
							_competitors[n]._result = prevResults[n];
							_competitors[n]._score = ScoreTable == null ? 0 :
								ScoreTable.GetScore(prevResults[n]);
						}

						while (_competitors.Count > prevCount)
							_competitors.RemoveAt(_competitors.Count - 1);

						ResetCompetitorsPosition();
						CalculateCompetitorsPosition();
						Group.CalculateTeamsScore();

						return strError;
					}
				}
				else
				{
					Sport.Championships.Championship championship =
						this.Group.Phase.Championship;
					championship.Edit();
					string strError = championship.Save(true);
					if (strError.Length > 0)
					{
						Sport.UI.MessageBox.Error("כשלון בשמירת תוצאות:\n" + strError,
							"תוצאות");
						championship.Cancel();
						return strError;
					}
				}
			}

			return string.Empty;
		} //end function SetResults

		public string SetResults(int heat, CompetitorResult[] results)
		{
			return SetResults(heat, results, true);
		}

		public bool ScoreIsRank()
		{
			Sport.Rulesets.Rules.GeneralSportTypeData objGeneralRule = this.GeneralData;
			return (objGeneralRule == null) ? false : objGeneralRule.ScoreIsRank;
		}

		public ArrayList GetTeamCompetitors(int teamID, bool blnResultsOnly)
		{
			//get all competitors and offline players.
			ArrayList arrAllCompetitors = new ArrayList(this.Competitors);
			arrAllCompetitors.AddRange(this.GetOfflinePlayers());

			//initialize result:
			ArrayList arrTeamCompetitors = new ArrayList();

			//iterate over competitors:
			foreach (object oComp in arrAllCompetitors)
			{
				//get data:
				int curResult = 0;
				int curTeamID = -1;
				if (oComp is Sport.Championships.Competitor)
				{
					Sport.Championships.Competitor competitor =
						(Sport.Championships.Competitor)oComp;
					curResult = competitor.Result;
					if ((competitor.Player != null) && (competitor.Player.CompetitionTeam != null))
						curTeamID = competitor.Player.CompetitionTeam.TeamEntity.Id;
				}
				else if (oComp is Sport.Entities.OfflinePlayer)
				{
					Sport.Entities.OfflinePlayer oPlayer =
						(Sport.Entities.OfflinePlayer)oComp;
					curResult = oPlayer.Result;
					if (oPlayer.Team != null)
						curTeamID = oPlayer.Team.Id;
				}

				//got result?
				if (blnResultsOnly && curResult <= 0)
					continue;

				//have any team?
				if (curTeamID < 0)
					continue;

				//belong to the team?
				if (teamID < 0 || curTeamID == teamID)
					arrTeamCompetitors.Add(oComp);
			} //end loop over competitors

			return arrTeamCompetitors;
		}

		public ArrayList GetTeamCompetitors(int teamID)
		{
			return this.GetTeamCompetitors(teamID, true);
		}

		public int GetBestResults()
		{
			//highest value by default
			int result = 99999;

			//get counter rules:
			Sport.Rulesets.Rules.TeamScoreCounters objCountersRule =
				this.ScoreCounters;
			Sport.Rulesets.Rules.TeamScoreCounter objCounterRule =
				this.ScoreCounter;

			//get best results count:
			if ((objCountersRule != null) && (objCounterRule != null))
			{
				string strCounterName = objCounterRule.Counter;
				for (int i = 0; i < objCountersRule.Counters.Count; i++)
				{
					if (objCountersRule.Counters[i].Name == strCounterName)
					{
						result = objCountersRule.Counters[i].Results;
						break;
					}
				}
			}

			//done.
			return result;
		} //end function GetBestResults

		public class CompetitorResultComparer : IComparer
		{
			private Sport.Core.Data.ResultDirection _direction;

			public CompetitorResultComparer(Sport.Core.Data.ResultDirection direction)
			{
				_direction = direction;
			}

			public int Compare(object x, object y)
			{
				int r1 = 0;
				int r2 = 0;
				int c1 = -1;
				int c2 = -1;
				int s1 = 0;
				int s2 = 0;
				int p1 = 0;
				int p2 = 0;
				int lrd1 = 0;
				int lrd2 = 0;
				int td1 = 0;
				int td2 = 0;
				if (x is Sport.Championships.Competitor)
				{
					r1 = (x as Sport.Championships.Competitor).Result;
					c1 = (x as Sport.Championships.Competitor).CustomPosition;
					s1 = (x as Sport.Championships.Competitor).PlayerNumber;
					p1 = (x as Sport.Championships.Competitor).ResultPosition; //.Position;
					lrd1 = (x as Sport.Championships.Competitor).LastResultDisqualifications;
					td1 = (x as Sport.Championships.Competitor).TotalDisqualifications;
				}
				else if (x is Sport.Entities.OfflinePlayer)
				{
					r1 = (x as Sport.Entities.OfflinePlayer).Result;
					c1 = (x as Sport.Entities.OfflinePlayer).CustomPosition;
					s1 = (x as Sport.Entities.OfflinePlayer).ShirtNumber;
					p1 = (x as Sport.Entities.OfflinePlayer).Rank;
				}
				if (y is Sport.Championships.Competitor)
				{
					r2 = (y as Sport.Championships.Competitor).Result;
					c2 = (y as Sport.Championships.Competitor).CustomPosition;
					s2 = (y as Sport.Championships.Competitor).PlayerNumber;
					p2 = (y as Sport.Championships.Competitor).ResultPosition; //.Position;
					lrd2 = (y as Sport.Championships.Competitor).LastResultDisqualifications;
					td2 = (y as Sport.Championships.Competitor).TotalDisqualifications;
				}
				else if (y is Sport.Entities.OfflinePlayer)
				{
					r2 = (y as Sport.Entities.OfflinePlayer).Result;
					c2 = (y as Sport.Entities.OfflinePlayer).CustomPosition;
					s2 = (y as Sport.Entities.OfflinePlayer).ShirtNumber;
					p2 = (y as Sport.Entities.OfflinePlayer).Rank;
				}
				if (s1 == s2)
					return 0;
				if (c1 >= 0 || c2 >= 0)
				{
					int n1 = (c1 >= 0) ? c1 : p1;
					int n2 = (c2 >= 0) ? c2 : p2;
					return n1.CompareTo(n2);
				}
				int multiplier = (_direction == Sport.Core.Data.ResultDirection.Least) ? 1 : -1;
				if (r1 != r2)
					return (multiplier * r1.CompareTo(r2));
				if (td1 > 0 || td2 > 0)
				{
					if (lrd1 != lrd2 && (lrd1 > 0 || lrd2 > 0))
						return lrd1.CompareTo(lrd2);
					return td1.CompareTo(td2);
				}
				return c1.CompareTo(c2);
			}
		}
	}
}
