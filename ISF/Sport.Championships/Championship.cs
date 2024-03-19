using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Sport.Core;
using Sport.Rulesets.Rules;

namespace Sport.Championships
{
	public enum Status
	{
		Planned,
		Started,
		Ended
	}

	#region Championship Class

	/// <summary>
	/// The Championship class holds all information about a
	/// single championship, which is the matches or tournaments 
	/// of a single category of a championship entity and the teams competing
	/// int the championships.
	/// </summary>
	public class Championship : IDisposable
	{
		public static bool GotDifferentStructure(Sport.Championships.Championship c1, Sport.Championships.Championship c2)
		{
			if (c1.Phases != null && c2.Phases != null)
			{
				if (c1.Phases.Count != c2.Phases.Count)
					return true;
				bool differentGroups = false;
				for (int i = 0; i < c1.Phases.Count; i++)
				{
					var p1 = c1.Phases[i];
					var p2 = c2.Phases[i];
					if (p1.Groups != null && p2.Groups != null && p1.Groups.Count != p2.Groups.Count)
					{
						differentGroups = true;
						break;
					}
				}
				return differentGroups;
			}
			return false;
		}

		#region Static Championship Storage

		// Class holds hashtable of all loaded championships
		private static System.Collections.Hashtable championships;
		// GetChampionship search for the championship in the memory
		// and loads is if it doesn't
		public static Championship GetChampionship(int id, string strLogPath, bool alwaysLoadFromServer)
		{
			strLogPath = null;

			if (strLogPath != null)
				Sport.Core.LogFiles.AppendToLogFile(strLogPath, "get championship called: " + id);

			if (!Sport.Core.Session.Connected)
			{
				if (strLogPath != null)
					Sport.Core.LogFiles.AppendToLogFile(strLogPath, "not connected");
				//return new CompetitionChampionship(id);
				Championship champ = new Championship(id);
				switch (champ.SportType)
				{
					case Sport.Types.SportType.Match:
						return new MatchChampionship(id);
					case Sport.Types.SportType.Competition:
						return new CompetitionChampionship(id);
				}
				return null;
			}

			bool isLocal = Utils.IsRunningLocal();
			Championship championship = null;

			if (isLocal)
			{
				if (strLogPath != null)
					Sport.Core.LogFiles.AppendToLogFile(strLogPath, "local");

				if (alwaysLoadFromServer)
				{
					championships[id] = GetNewChampionship(id);
				}
				else
				{
					if (championships[id] == null)
					{
						if (strLogPath != null)
							Sport.Core.LogFiles.AppendToLogFile(strLogPath, "loading into memory");
						championships[id] = GetNewChampionship(id);
					}
				}

				championship = (Championship)championships[id];
			}
			else
			{
				if (strLogPath != null)
					Sport.Core.LogFiles.AppendToLogFile(strLogPath, "getting new championship");
				championship = GetNewChampionship(id, strLogPath);
			}

			if (strLogPath != null)
				Sport.Core.LogFiles.AppendToLogFile(strLogPath, "championship loaded: " + ((championship == null) ? "NULL" : championship.Name));
			return championship;
		}

		public static Championship GetChampionship(int id, string strLogPath)
		{
			return GetChampionship(id, strLogPath, false);
		}

		public static Championship GetChampionship(int id, bool alwaysLoadFromServer)
		{
			return GetChampionship(id, null, alwaysLoadFromServer);
		}

		public static Championship GetChampionship(int id)
		{
			return GetChampionship(id, null);
		}

		public static void ResetChampionship(int champID)
		{
			championships[champID] = null;
		}

		private static Championship GetNewChampionship(int categoryID, string strLogPath)
		{
			strLogPath = null;

			Championship result = null;

			if (strLogPath != null)
				Sport.Core.LogFiles.AppendToLogFile(strLogPath, "get new championship called: " + categoryID);

			try
			{
				Sport.Entities.ChampionshipCategory championshipCategory =
					new Sport.Entities.ChampionshipCategory(categoryID);

				if (strLogPath != null)
					Sport.Core.LogFiles.AppendToLogFile(strLogPath, "entity created, championship: " + championshipCategory.Championship.Id);

				if (championshipCategory.IsValid())
				{
					if (strLogPath != null)
						Sport.Core.LogFiles.AppendToLogFile(strLogPath, "valid, sport type is " + championshipCategory.Championship.Sport.SportType);
					if (championshipCategory.Championship.Sport.SportType == Sport.Types.SportType.Match)
					{
						result = new MatchChampionship(championshipCategory, strLogPath);
					}
					else
					{
						result = new CompetitionChampionship(championshipCategory);
					}
				}
			}
			catch (ChampionshipException e)
			{
				if (strLogPath != null)
					Sport.Core.LogFiles.AppendToLogFile(strLogPath, "error: " + e.Message);
				System.Diagnostics.Debug.WriteLine(e.Message);

				result = null;
			}
			if (strLogPath != null)
				Sport.Core.LogFiles.AppendToLogFile(strLogPath, "new championship completed");
			return result;
		}

		private static Championship GetNewChampionship(int categoryID)
		{
			return GetNewChampionship(categoryID, null);
		}

		static Championship()
		{
			championships = new System.Collections.Hashtable();
		}

		#endregion

		#region Properties

		/// <summary>
		/// ChampionshipCategory is the entity of the championship
		/// </summary>
		private Sport.Entities.ChampionshipCategory _championshipCategory = null;
		private int _categoryID = -1;
		public Sport.Entities.ChampionshipCategory ChampionshipCategory
		{
			get
			{
				if (_championshipCategory != null)
					return _championshipCategory;
				return new Sport.Entities.ChampionshipCategory(_categoryID);
			}
		}
		public int CategoryID
		{
			get
			{
				if (this.ChampionshipCategory != null)
					return this.ChampionshipCategory.Id;
				return _categoryID;
			}
		}

		private Sport.Types.SportType _sportType = Sport.Types.SportType.None;
		public Sport.Types.SportType SportType
		{
			get
			{
				if (_championshipCategory != null)
					return _championshipCategory.Championship.Sport.SportType;
				return _sportType;
			}
			set { _sportType = value; }
		}

		/// <summary>
		/// The name of the championship is set to the name of the championship
		/// entity with the category of the championship
		/// </summary>
		private string _name;
		public string Name
		{
			get { return _name; }
		}

		/// <summary>
		/// Timestamp of last read from server
		/// </summary>
		private decimal _timestamp;
		public decimal Timestamp
		{
			get { return _timestamp; }
		}

		#endregion

		#region Rules

		public event EventHandler RulesetChanged;

		internal virtual void OnRulesetChange()
		{
			foreach (Phase phase in _phases)
				phase.OnRulesetChange();

			if (RulesetChanged != null)
				RulesetChanged(this, EventArgs.Empty);
		}

		private int _championshipRulesetId;
		private int _sportRulesetId;

		private void RulesetStored(Sport.Rulesets.RulesetEventArgs e)
		{
			if (e.Id == _championshipRulesetId ||
				e.Id == _sportRulesetId)
			{
				OnRulesetChange();
			}
		}

		private void ChampionshipStored(object sender, Sport.Data.EntityEventArgs e)
		{
			if (_championshipCategory == null || _championshipCategory.Championship == null)
				return;

			if (e != null && e.Entity != null && e.Entity.Id == _championshipCategory.Championship.Id)
			{
				Sport.Entities.Ruleset rs = new Sport.Entities.Championship(e.Entity).Ruleset;

				if ((rs == null && _championshipRulesetId != -1) ||
					(rs != null && _championshipRulesetId != rs.Id))
					OnRulesetChange();

				_championshipRulesetId = rs == null ? -1 : rs.Id;
			}
		}

		private void SportStored(object sender, Sport.Data.EntityEventArgs e)
		{
			if (_championshipCategory == null)
				return;

			if (e.Entity.Id == _championshipCategory.Championship.Sport.Id)
			{
				Sport.Entities.Ruleset rs = new Sport.Entities.Sport(e.Entity).Ruleset;

				if ((rs == null && _sportRulesetId != -1) ||
					(rs != null && _sportRulesetId != rs.Id))
					OnRulesetChange();

				_sportRulesetId = rs == null ? -1 : rs.Id;
			}
		}

		#endregion

		#region Teams

		/// <summary>
		/// Each championship holds a collection of the teams that compete
		/// in the championship.
		/// </summary>

		#region TeamCollection

		public class TeamCollection : Sport.Common.GeneralCollection
		{
			public TeamCollection(Championship championship)
				: base(championship)
			{
			}

			public Championship Championship
			{
				get { return ((Championship)Owner); }
			}

			public Sport.Entities.Team this[int index]
			{
				get { return (Sport.Entities.Team)GetItem(index); }
				set { SetItem(index, value); }
			}

			public void Insert(int index, Sport.Entities.Team value)
			{
				InsertItem(index, value);
			}

			public void Remove(Sport.Entities.Team value)
			{
				RemoveItem(value);
			}

			public bool Contains(Sport.Entities.Team value)
			{
				return base.Contains(value);
			}

			public int IndexOf(Sport.Entities.Team value)
			{
				return base.IndexOf(value);
			}

			public int Add(Sport.Entities.Team value)
			{
				return AddItem(value);
			}
		}

		#endregion

		private TeamCollection _teams;
		public TeamCollection Teams
		{
			get { return _teams; }
		}

		#endregion

		#region Phases

		/// <summary>
		/// Every championship is devided into phases
		/// For matches championships, each phase represent a set of
		/// matches in which the groups and teams structure don't change.
		/// For competition championships, each phase represent a set of
		/// competitions each team in each group will need to take
		/// part of. A phase in competition championships is a tournaments event.
		/// </summary>

		#region PhaseCollection

		public class PhaseCollection : Sport.Common.GeneralCollection
		{
			public PhaseCollection(Championship championship)
				: base(championship)
			{
			}

			protected override void SetItem(int index, object value)
			{
				if (!((Championship)Owner)._editing)
					throw new ChampionshipException("Not in edit - cannot change phases");

				base.SetItem(index, value);
			}

			protected override void InsertItem(int index, object value)
			{
				if (!((Championship)Owner)._editing)
					throw new ChampionshipException("Not in edit - cannot change phases");

				base.InsertItem(index, value);
			}

			protected override void RemoveItem(int index)
			{
				if (!((Championship)Owner)._editing)
					throw new ChampionshipException("Not in structure edit - cannot change phases");

				base.RemoveItem(index);
			}

			public Phase this[int index]
			{
				get { return (Phase)GetItem(index); }
				set { SetItem(index, value); }
			}

			public void Insert(int index, Phase value)
			{
				InsertItem(index, value);
			}

			public void Remove(Phase value)
			{
				RemoveItem(value);
			}

			public bool Contains(Phase value)
			{
				return base.Contains(value);
			}

			public int IndexOf(Phase value)
			{
				return base.IndexOf(value);
			}

			public int Add(Phase value)
			{
				return AddItem(value);
			}
		}

		#endregion

		protected virtual PhaseCollection CreatePhaseCollection()
		{
			return new PhaseCollection(this);
		}

		private PhaseCollection _phases;
		public PhaseCollection Phases
		{
			get { return _phases; }
		}

		protected int _currentPhase;
		public int CurrentPhaseIndex
		{
			get { return _currentPhase; }
		}
		public Phase CurrentPhase
		{
			get { return _currentPhase == -1 ? null : _phases[_currentPhase]; }
		}

		/// <summary>
		/// Ends the current phase and starts the next phase
		/// </summary>
		public string NextPhase()
		{
			if (!Editing)
				throw new ChampionshipException("Cannot change phase - not in edit");

			// Checking that can start next phase
			if (_currentPhase < Phases.Count - 1)
			{
				string strError = Phases[_currentPhase + 1].CanStart();
				if (strError.Length > 0)
					return "שלב '" + Phases[_currentPhase + 1].Name + "' לא יכול להתחיל:\n" + strError;
			}

			if (_currentPhase == -1)
			{
				// Need to store in the entity that the championship is starting
				if (_championshipCategory != null)
				{
					_championshipCategory.Edit();
					_championshipCategory.Status = 1;

					if (!_championshipCategory.Save().Succeeded)
					{
						_championshipCategory.Cancel();
						return "כשלון בעת שמירת נתוני אליפות";
					}
				}
			}
			else
			{
				Phases[_currentPhase].End();
			}
			if (_currentPhase < Phases.Count - 1)
			{
				Phases[_currentPhase + 1].Start();
				_currentPhase++;
			}

			return "";
		}

		/// <summary>
		/// Resets the current phase and sets the current phase
		/// to the previous
		/// </summary>
		public bool PreviousPhase()
		{
			if (!Editing)
				throw new ChampionshipException("Cannot change phase - not in edit");

			if (_currentPhase == -1)
				return true;

			if (_currentPhase == 0)
			{
				// Need to store in the entity that the championship is not started
				if (_championshipCategory != null)
				{
					_championshipCategory.Edit();
					_championshipCategory.Status = 0;

					if (!_championshipCategory.Save().Succeeded)
					{
						_championshipCategory.Cancel();
						return false;
					}
				}
			}

			CurrentPhase.Reset();
			_currentPhase--;
			if (_currentPhase != -1)
				CurrentPhase._status = Status.Started;

			return true;
		}

		private void PhasesChanged(object sender, Sport.Common.CollectionEventArgs e)
		{
			if (_currentPhase >= _phases.Count)
				_currentPhase = _phases.Count - 1;

			for (int i = e.Index; i < _phases.Count; i++)
			{
				_phases[i].Changed();
				_phases[i]._index = i;
			}
		}

		#endregion

		#region Constructors
		protected Championship(int categoryID, string strLogPath)
		{
			strLogPath = null;

			if (strLogPath != null)
				Sport.Core.LogFiles.AppendToLogFile(strLogPath, "championship constructor called: " + categoryID);

			bool isLocal = Utils.IsRunningLocal();

			if (isLocal)
				Sport.UI.Dialogs.WaitForm.SetProgress(5);

			if (strLogPath != null)
				Sport.Core.LogFiles.AppendToLogFile(strLogPath, "progress has been set");

			_timestamp = 0;
			_teams = new TeamCollection(this);

			_phases = CreatePhaseCollection();
			_phases.Changed += new Sport.Common.CollectionEventHandler(PhasesChanged);

			if (isLocal)
				Sport.UI.Dialogs.WaitForm.SetProgress(7);

			if (strLogPath != null)
				Sport.Core.LogFiles.AppendToLogFile(strLogPath, "phases: " + ((_phases == null) ? 0 : _phases.Count));

			_championshipRulesetId = -1;
			_sportRulesetId = -1;
			_name = "";
			_currentPhase = -1;
			_categoryID = categoryID;

			if (Sport.Core.Session.Connected)
			{
				_championshipCategory = new Sport.Entities.ChampionshipCategory(categoryID);

				Sport.Entities.Ruleset rs = _championshipCategory.Championship.Ruleset;
				_championshipRulesetId = rs == null ? -1 : rs.Id;
				rs = _championshipCategory.Championship.Sport.Ruleset;
				_sportRulesetId = rs == null ? -1 : rs.Id;
				Rulesets.Ruleset.RulesetChanged += new Sport.Rulesets.RulesetEventHandler(RulesetStored);
				Sport.Entities.Championship c = _championshipCategory.Championship;

				if (isLocal)
					Sport.UI.Dialogs.WaitForm.SetProgress(10);

				c.Entity.EntityType.EntityStored += new Sport.Data.EntityEventHandler(ChampionshipStored);
				c.Sport.Entity.EntityType.EntityStored += new Sport.Data.EntityEventHandler(SportStored);

				_name = _championshipCategory.Championship.Name + " - " + _championshipCategory.Name;
				Sport.Entities.Team[] teams = _championshipCategory.GetTeams();
				foreach (Sport.Entities.Team team in teams)
				{
					_teams.Add(team);
				}

				if (isLocal)
					Sport.UI.Dialogs.WaitForm.SetProgress(20);

				Sport.Entities.Team.Type.EntityAdded += new Sport.Data.EntityEventHandler(ChampionshipTeamAdded);
				Sport.Entities.Team.Type.EntityRemoved += new Sport.Data.EntityEventHandler(ChampionshipTeamRemoved);
			}

			string strError = this.Load();

			if (strLogPath != null)
				Sport.Core.LogFiles.AppendToLogFile(strLogPath, "done loading, success");

			if (isLocal)
				Sport.UI.Dialogs.WaitForm.SetProgress(100);

			if (strError.Length > 0)
				throw new ChampionshipException("שגיאה בעת טעינת אליפות:\n" + strError);
		}

		protected Championship(int categoryID)
			: this(categoryID, null)
		{
		}

		public Championship(string name, Sport.Entities.Team[] teams)
		{
			_teams = new TeamCollection(this);

			_phases = CreatePhaseCollection();
			_phases.Changed += new Sport.Common.CollectionEventHandler(PhasesChanged);

			_championshipCategory = null;

			_name = name;

			foreach (Sport.Entities.Team team in teams)
			{
				_teams.Add(team);
			}

			Sport.Entities.Team.Type.EntityAdded += new Sport.Data.EntityEventHandler(ChampionshipTeamAdded);
			Sport.Entities.Team.Type.EntityRemoved += new Sport.Data.EntityEventHandler(ChampionshipTeamRemoved);
		}

		#endregion

		#region Load/Save

		private bool _editing;
		public bool Editing
		{
			get { return _editing; }
		}

		public void Edit()
		{
			if (_editing)
				throw new ChampionshipException("Championship already in edit");

			// Checking that no group is in edit
			foreach (Phase phase in Phases)
			{
				foreach (Group group in phase.Groups)
				{
					if (group.Editing)
						throw new ChampionshipException("Cannot edit championship - a group is in edit");
				}
			}

			_editing = true;
		}

		public void CancelEdit()
		{
			_editing = false;
		}

		public void Cancel()
		{
			string strError = this.Load();
			if (strError.Length > 0)
			{
				throw new ChampionshipException("Failed to load championship: " + strError);
			}
			_editing = false;
		}

		protected virtual void Initialize()
		{
		}

		[System.Xml.Serialization.XmlRoot("Championship")]
		public class XmlChampionship
		{
			public SportServices.Phase[] Phases;

			[System.Xml.Serialization.XmlAttribute("id")]
			public int id;

			[System.Xml.Serialization.XmlAttribute("name")]
			public string name;

			[System.Xml.Serialization.XmlAttribute("type")]
			public int type;
		}

		private SportServices.Team GetActualTeam(string matchKey, int position, int relativePosition, 
			Dictionary<string, SportServices.Match> matchMapping, Dictionary<string, SportServices.Group> matchGroupMapping)
		{
			SportServices.Match relativeMatch;
			SportServices.Group group;
			int actualPosition = position;
			if (position < 0 && relativePosition != 0)
			{
				string lookFor = "," + Math.Abs(relativePosition).ToString();
				string relativeKey = matchMapping.Keys.ToList().Find(k => k.EndsWith(lookFor));
				if (relativeKey != null)
				{
					relativeMatch = matchMapping[relativeKey];
					int outcome = relativeMatch.Outcome;
					if (outcome == (int)MatchOutcome.WinA || outcome == (int)MatchOutcome.TechnicalA)
						actualPosition = relativePosition < 0 ? relativeMatch.TeamBPos : relativeMatch.TeamAPos;
					else if (outcome == (int)MatchOutcome.WinB || outcome == (int)MatchOutcome.TechnicalB)
						actualPosition = relativePosition < 0 ? relativeMatch.TeamAPos : relativeMatch.TeamBPos;
				}
			}

			if (actualPosition >= 0 && matchGroupMapping.TryGetValue(matchKey, out group) && actualPosition < group.Teams.Length)
				return group.Teams[actualPosition];
			
			return null;
		}

		protected string Load()
		{
			//got category?
			if (((Sport.Core.Session.Connected) && (ChampionshipCategory == null)) ||
				((!Sport.Core.Session.Connected) && (_categoryID < 0)))
			{
				return "לא מוגדרת קטגורית אליפות";
			}

			//enter edit mode:
			_editing = true;

			//reset current phases:
			_currentPhase = -1;
			_phases.Clear();

			//initialize phases list:
			SportServices.Phase[] sphases = null;

			//connected?
			if (Sport.Core.Session.Connected)
			{
				//get championship from service.
				SportServices.ChampionshipService cs = new SportServices.ChampionshipService();
				cs.CookieContainer = Sport.Core.Session.Cookies;
				sphases = cs.LoadChampionship(_championshipCategory.Id);
			}
			else
			{
				//try to import championship from disk.
				string strError = this.Import(ref sphases);
				if (strError.Length > 0)
				{
					return strError;
				}
			}

			bool isLocal = Utils.IsRunningLocal();
			if (isLocal)
				Sport.UI.Dialogs.WaitForm.SetProgress(75);

			if (sphases == null)
				return "לא מוגדרים שלבים לאליפות זו";

			Dictionary<string, SportServices.Match> matchMapping = new Dictionary<string, SportServices.Match>();
			Dictionary<string, SportServices.Group> matchGroupMapping = new Dictionary<string, SportServices.Group>();
			int phaseNumber = 0;
			int groupNumber = 0;

			if (sphases != null && sphases.Length > 0)
			{
				foreach (SportServices.Phase phase in Array.FindAll<SportServices.Phase>(sphases, p => p != null))
				{
					if (phase.Groups != null)
					{
						foreach (SportServices.Group group in Array.FindAll<SportServices.Group>(phase.Groups, g => g != null))
						{
							if (group.Rounds != null)
							{
								foreach (SportServices.Round round in Array.FindAll<SportServices.Round>(group.Rounds, r => r != null))
								{
									if (round.Cycles != null)
									{
										foreach (SportServices.Cycle cycle in Array.FindAll<SportServices.Cycle>(round.Cycles, c => c != null))
										{
											if (cycle.Matches != null)
											{
												foreach (SportServices.Match match in Array.FindAll<SportServices.Match>(cycle.Matches, m => m != null))
												{
													int matchNumber = match.Number;
													string key = phaseNumber + "," + groupNumber + "," + matchNumber;
													if (!matchMapping.ContainsKey(key))
													{
														matchMapping.Add(key, match);
														matchGroupMapping.Add(key, group);
													}
												}
											}
										}
									}
								}
							}
							groupNumber++;
						}
					}
					phaseNumber++;
				}
			}

			//calculate wins, loses, ties, technical wins and technical loses:
			phaseNumber = 0;
			groupNumber = 0;
			foreach (SportServices.Phase phase in sphases)
			{
				if (phase.Groups != null)
				{
					foreach (SportServices.Group group in phase.Groups)
					{
						if (group.Rounds != null)
						{
							foreach (SportServices.Round round in group.Rounds)
							{
								if (round.Cycles != null)
								{
									foreach (SportServices.Cycle cycle in round.Cycles)
									{
										if (cycle.Matches != null)
										{
											foreach (SportServices.Match match in cycle.Matches)
											{
												int matchNumber = match.Number;
												string key = phaseNumber + "," + groupNumber + "," + matchNumber;
												SportServices.Team teamA = GetActualTeam(key, match.TeamAPos, match.RelativeTeamA, matchMapping, matchGroupMapping);
												SportServices.Team teamB = GetActualTeam(key, match.TeamBPos, match.RelativeTeamB, matchMapping, matchGroupMapping);
												if (teamA != null && teamB != null)
												{
													switch (match.Outcome)
													{
														case ((int)Sport.Championships.MatchOutcome.WinA):
															teamA.Wins++;
															teamB.Loses++;
															break;
														case ((int)Sport.Championships.MatchOutcome.WinB):
															teamB.Wins++;
															teamA.Loses++;
															break;
														case ((int)Sport.Championships.MatchOutcome.Tie):
															teamA.Ties++;
															teamB.Ties++;
															break;
														case ((int)Sport.Championships.MatchOutcome.TechnicalA):
															teamA.TechnicalWins++;
															teamB.TechnicalLoses++;
															break;
														case ((int)Sport.Championships.MatchOutcome.TechnicalB):
															teamB.TechnicalWins++;
															teamA.TechnicalLoses++;
															break;
													}
												}
											}
										}
									}
								}
							}
						}

						foreach (SportServices.Team team in group.Teams)
						{
							team.Games = team.Wins + team.Ties + team.Loses + team.TechnicalWins + team.TechnicalLoses;
						}
						groupNumber++;
					}
				}
				phaseNumber++;
			}

			if (isLocal)
				Sport.UI.Dialogs.WaitForm.SetProgress(90);

			bool matchType = (SportType == Sport.Types.SportType.Match);
			for (int n = 0; n < sphases.Length; n++)
			{
				int pi;
				Sport.Championships.Phase phase = null;
				if (matchType == true)
				{
					phase = new MatchPhase(sphases[n]);
				}
				else
				{
					phase = new CompetitionPhase(sphases[n]);
				}

				//add if it's not null:
				if (phase != null)
				{
					if (matchType == true)
					{
						pi = _phases.Add(phase as MatchPhase);
					}
					else
					{
						pi = _phases.Add(phase as CompetitionPhase);
					}
					if (_phases[pi].Status != Status.Planned)
						_currentPhase = pi;
				}
			}

			if (isLocal)
				Sport.UI.Dialogs.WaitForm.SetProgress(95);

			//done.
			Initialize();
			_editing = false;

			return "";
		}

		private static Championship.XmlChampionship Import(string strFilePath,
			ref string strErrorMsg)
		{
			//got file?
			if (!System.IO.File.Exists(strFilePath))
			{
				strErrorMsg = "קובץ לא קיים: " + strFilePath;
				return null;
			}

			//initialize result:
			XmlChampionship result = null;
			strErrorMsg = "";

			//file stream:
			System.IO.FileStream serializeStream = null;

			//try to import from local file:
			try
			{
				//open file:
				serializeStream = new System.IO.FileStream(
					strFilePath, System.IO.FileMode.OpenOrCreate);

				//initialize XML object:
				System.Xml.Serialization.XmlSerializer xml = new System.Xml.Serialization.XmlSerializer(typeof(XmlChampionship));

				//convert XML to championship:
				result = (XmlChampionship)xml.Deserialize(serializeStream);
			}
			catch (Exception exp)
			{
				System.Diagnostics.Debug.WriteLine("Failed to import championship from file: " + exp.Message);
				strErrorMsg = "שגיאה בעת קריאת קובץ מהדיסק:\n" + exp.Message;
				result = null;
			}

			//close the stream:
			if (serializeStream != null)
				serializeStream.Close();

			//done.
			return result;
		}

		public static SportServices.Phase[] GetOfflinePhases(string strFilePath,
			ref string strErrorMsg)
		{
			//import championship:
			XmlChampionship xc = Import(strFilePath, ref strErrorMsg);

			//got anything?
			if (xc == null)
				return null;

			//done.
			return xc.Phases;
		}

		private string Import(ref SportServices.Phase[] sphases)
		{
			//define path:
			string strResultsFilePath = Sport.Core.Session.GetSeasonCache(false) + System.IO.Path.DirectorySeparatorChar +
				"Championship_" + _categoryID.ToString() + ".xml";
			string strFilePath = strResultsFilePath.Replace("_", "");

			//initialize result string:
			string result = "";

			//try to import phases from local file:
			XmlChampionship xc = Import(strFilePath, ref result);

			//got anything?
			if ((xc == null) || (result.Length > 0))
				return result;

			//read phases:
			sphases = xc.Phases;

			//get name and the sport type:
			_name = xc.name;
			this.SportType = (Sport.Types.SportType)xc.type;

			//got results file?
			if (System.IO.File.Exists(strResultsFilePath))
			{
				//get phases only:
				string strMsg = "";
				SportServices.Phase[] phases = GetOfflinePhases(strResultsFilePath, ref strMsg);

				//got anything?
				if ((phases != null) && (strMsg.Length == 0))
				{
					//initialize tables:
					System.Collections.Hashtable tblCompetitorScores =
						new System.Collections.Hashtable();
					System.Collections.Hashtable tblCompetitorResults =
						new System.Collections.Hashtable();

					//read score and results:
					ReadScoresAndResults(phases, ref tblCompetitorScores,
						ref tblCompetitorResults);

					//update score and results in original phases:
					foreach (SportServices.Phase phase in sphases)
					{
						foreach (SportServices.Group group in phase.Groups)
						{
							foreach (SportServices.Competition competition in group.Competitions)
							{
								foreach (SportServices.Competitor competitor in
									competition.Competitors)
								{
									string key = GetUniqueKey(phase, group, competition, competitor);
									if (tblCompetitorScores[key] != null)
										competitor.Score = (int)tblCompetitorScores[key];
									if (tblCompetitorResults[key] != null)
										competitor.Result = (int)tblCompetitorResults[key];
								}
							} //end loop over competitions
						} //end loop over groups
					} //end loop over phases
				}
				else
				{
					System.Diagnostics.Debug.WriteLine("failed to read results file " + strResultsFilePath + ": " + strMsg);
				}
			} //end if got results file.

			return "";
		}

		public static void ReadScoresAndResults(SportServices.Phase[] phases,
			ref System.Collections.Hashtable scores,
			ref System.Collections.Hashtable results,
			ref Sport.Championships.CompetitionChampionship championship)
		{
			//iterate through phases:
			for (int phaseIndex = 0; phaseIndex < phases.Length; phaseIndex++)
			{
				//get current phase:
				SportServices.Phase phase = phases[phaseIndex];

				//got groups?
				if (phase.Groups == null)
					continue;

				//iterate over groups:
				for (int groupIndex = 0; groupIndex < phase.Groups.Length; groupIndex++)
				{
					//get current group:
					SportServices.Group group = phase.Groups[groupIndex];

					//got competitions?
					if (group.Competitions == null)
						continue;

					//iterate over competitions:
					for (int competitionIndex = 0;
						competitionIndex < group.Competitions.Length;
						competitionIndex++)
					{
						//get current competition:
						SportServices.Competition competition =
							group.Competitions[competitionIndex];

						//got competitors?
						if (competition.Competitors == null)
							continue;

						//iterate over competitors:
						foreach (SportServices.Competitor competitor in
							competition.Competitors)
						{
							//exists in online phases?
							if (championship != null)
							{
								if (phaseIndex < championship.Phases.Count)
								{
									Sport.Championships.CompetitionPhase oPhase = championship.Phases[phaseIndex];
									if (groupIndex < oPhase.Groups.Count)
									{
										Sport.Championships.CompetitionGroup oGroup = oPhase.Groups[groupIndex];
										if (competitionIndex < oGroup.Competitions.Count)
										{
											Sport.Championships.Competition oCompetition = oGroup.Competitions[competitionIndex];
											bool blnExists = false;
											foreach (Sport.Championships.Competitor oCompetitor in oCompetition.Competitors)
											{
												if (oCompetitor.PlayerNumber == competitor.PlayerNumber)
												{
													blnExists = true;
													break;
												}
											}
											if (!blnExists)
											{
												//add
												oCompetition.Competitors.Add(
													new Sport.Championships.Competitor(competitor.PlayerNumber));
											} //end if does not competitor exist
										} //end if valid competition index
									} //end if valid group index
								} //end if valid phase index
							} //end if got online phases

							//get unique key
							string key = GetUniqueKey(phase, group, competition,
								competitor);

							//apply score and result:
							scores[key] = competitor.Score;
							results[key] = competitor.Result;
						} //end loop over competitors
					} //end loop over competitions
				} //end loop over groups
			} //end loop over phases
		} //end function ReadScoresAndResults

		public static void ReadScoresAndResults(SportServices.Phase[] phases,
			ref System.Collections.Hashtable scores,
			ref System.Collections.Hashtable results)
		{
			Sport.Championships.CompetitionChampionship dummy = null;
			ReadScoresAndResults(phases, ref scores, ref results, ref dummy);
		}

		private static string GetUniqueKey(string phase, string group, int sportField,
			int shirtNumber)
		{
			return phase + "_" + group + "_" + sportField + "_" + shirtNumber;
		}

		public static string GetUniqueKey(SportServices.Phase phase,
			SportServices.Group group, SportServices.Competition competition,
			SportServices.Competitor competitor)
		{

			return GetUniqueKey(phase.Name, group.Name, competition.SportField,
				competitor.PlayerNumber);
		}

		public static string GetUniqueKey(Sport.Championships.Phase phase,
			Sport.Championships.Group group, Sport.Championships.Competition competition,
			Sport.Championships.Competitor competitor)
		{
			return GetUniqueKey(phase.Name, group.Name, competition.SportField.Id,
				competitor.PlayerNumber);
		}

		public string Save(bool blnOnlyResults)
		{
			//got category?
			if (this.CategoryID < 0)
				return "לא מוגדרת קטגוריית אליפות";

			//edit mode?
			if (!_editing)
				return "אליפות לא במצב עריכה";

			//initialize phases list:
			SportServices.Phase[] sphases = new SportServices.Phase[_phases.Count];

			//save phases:
			for (int n = 0; n < _phases.Count; n++)
				sphases[n] = _phases[n].Save();

			//connected?
			if (Sport.Core.Session.Connected)
			{
				//save using serivce.
				SportServices.ChampionshipService cs = new SportServices.ChampionshipService();
				cs.CookieContainer = Sport.Core.Session.Cookies;
				string strError = cs.SaveChampionship(ChampionshipCategory.Id, ref sphases);
				if (strError.Length > 0)
					return strError;
			}
			else
			{
				//save to disk.
				string strError = this.Export(sphases, blnOnlyResults);
				if (strError.Length > 0)
				{
					_editing = false;
					return strError;
				}
			}

			//done.
			foreach (Phase phase in _phases)
				phase.Saved();

			_editing = false;
			return string.Empty;
		}

		public string Save()
		{
			return Save(false);
		}

		private string Export(SportServices.Phase[] sphases, bool blnOnlyResults)
		{
			bool blnConnected = Sport.Core.Session.Connected;
			int categoryID = this.CategoryID;

			if (!blnOnlyResults)
			{
				Sport.Core.Session.Connected = true;
				Sport.UI.Dialogs.WaitForm.SetProgress(0);
			}

			System.IO.FileStream serializeStream = null;
			string strFilePath = Sport.Core.Session.GetSeasonCache(false) + System.IO.Path.DirectorySeparatorChar +
				"Championship";
			if (blnOnlyResults)
				strFilePath += "_";

			strFilePath += categoryID + ".xml";
			System.Collections.ArrayList arrFiles =
				new System.Collections.ArrayList();
			arrFiles.Add(strFilePath);
			if (blnOnlyResults)
			{
				arrFiles.Add(strFilePath.Replace("_" + categoryID,
					categoryID.ToString()));
			}
			XmlChampionship xc = new XmlChampionship();
			xc.id = this.CategoryID;
			if (_championshipCategory != null)
				xc.name = _championshipCategory.Championship.Name + " " + _championshipCategory.Name;
			else
				xc.name = this.Name;
			xc.type = (int)this.SportType;
			xc.Phases = sphases;
			for (int i = 0; i < arrFiles.Count; i++)
			{
				try
				{
					serializeStream = new System.IO.FileStream(
						arrFiles[i].ToString(), System.IO.FileMode.Create);
					System.Xml.Serialization.XmlSerializer xml = new System.Xml.Serialization.XmlSerializer(typeof(XmlChampionship));
					xml.Serialize(serializeStream, xc);
					serializeStream.Close();
				}
				catch (Exception exp)
				{
					System.Diagnostics.Debug.WriteLine("Failed to save championship to file: " + exp.Message);
					if (serializeStream != null)
					{
						serializeStream.Close();
					}
					Sport.Core.Session.Connected = blnConnected;
					return "שגיאה בעת כתיבת נתונים לדיסק:\n" + exp.Message + "\n" + exp.StackTrace;
				}
			}

			if (blnOnlyResults)
				return "";

			Sport.UI.Dialogs.WaitForm.SetProgress(20);

			//entities
			ExportEntities();

			//rules

			Sport.UI.Dialogs.WaitForm.SetProgress(80);

			//global rules:
			ExportGlobalRule(typeof(Sport.Rulesets.Rules.Functionaries),
				typeof(Sport.Rulesets.Rules.FunctionariesRule));
			ExportGlobalRule(typeof(Sport.Rulesets.Rules.TechnicalResult),
				typeof(Sport.Rulesets.Rules.TechnicalResultRule));
			ExportGlobalRule(typeof(Sport.Rulesets.Rules.GameStructure),
				typeof(Sport.Rulesets.Rules.GameStructureRule));
			ExportGlobalRule(typeof(Sport.Rulesets.Rules.GameScore),
				typeof(Sport.Rulesets.Rules.GameScoreRule));
			ExportGlobalRule(typeof(Sport.Rulesets.Rules.RankingTables),
				typeof(Sport.Rulesets.Rules.RankingTablesRule));

			Sport.UI.Dialogs.WaitForm.SetProgress(90);

			//competition specific rules:
			ExportCompetitionRule(typeof(Sport.Rulesets.Rules.TeamPhaseScoring),
				typeof(Sport.Rulesets.Rules.TeamPhaseScoringRule));
			ExportCompetitionRule(typeof(Sport.Rulesets.Rules.CompetitionTeamCompetitors),
				typeof(Sport.Rulesets.Rules.CompetitionTeamCompetitorsRule));
			ExportCompetitionRule(typeof(Sport.Rulesets.Rules.TeamScoreCounter),
				typeof(Sport.Rulesets.Rules.TeamScoreCounterRule));
			ExportCompetitionRule(typeof(Sport.Rulesets.Rules.GeneralSportTypeData),
				typeof(Sport.Rulesets.Rules.GeneralSportTypeDataRule));
			ExportCompetitionRule(typeof(Sport.Rulesets.Rules.CompetitorCompetitions),
				typeof(Sport.Rulesets.Rules.CompetitorCompetitionsRule));
			ExportCompetitionRule(typeof(Sport.Rulesets.Rules.ResultType),
				typeof(Sport.Rulesets.Rules.ResultTypeRule));
			ExportCompetitionRule(typeof(Sport.Rulesets.Rules.ScoreTable),
				typeof(Sport.Rulesets.Rules.ScoreTableRule));
			ExportCompetitionRule(typeof(Sport.Rulesets.Rules.TeamRanking),
				typeof(Sport.Rulesets.Rules.TeamRankingRule));
			ExportCompetitionRule(typeof(Sport.Rulesets.Rules.TeamScoreCounters),
				typeof(Sport.Rulesets.Rules.TeamScoreCountersRule));

			Sport.UI.Dialogs.WaitForm.SetProgress(95);
			ExportRuleTypes();

			Sport.UI.Dialogs.WaitForm.SetProgress(100);

			Sport.Core.Session.Connected = blnConnected;
			return "";
		}

		private void ExportRuleTypes()
		{
			Sport.Rulesets.RuleType.Export();
		}

		private void ExportEntities()
		{
			System.Collections.ArrayList arrEntities =
				new System.Collections.ArrayList();
			arrEntities.Add(this.ChampionshipCategory);
			foreach (Sport.Championships.Phase phase in this.Phases)
			{
				Sport.Entities.Championship championship =
					phase.Championship.ChampionshipCategory.Championship;
				if (arrEntities.IndexOf(championship) < 0)
				{
					arrEntities.Add(championship);
					Sport.Entities.Ruleset ruleset = championship.Ruleset;
					if ((ruleset != null) && (arrEntities.IndexOf(ruleset) < 0))
						arrEntities.Add(ruleset);
					Sport.Entities.Sport sport = championship.Sport;
					if (sport != null)
					{
						if (arrEntities.IndexOf(sport) < 0)
						{
							arrEntities.Add(sport);
							ruleset = sport.Ruleset;
							if ((ruleset != null) && (arrEntities.IndexOf(ruleset) < 0))
							{
								arrEntities.Add(ruleset);
							}
						}
					}
				}
				foreach (Sport.Championships.Group group in phase.Groups)
				{
					foreach (Sport.Championships.Team team in group.Teams)
					{
						if (team.TeamEntity == null)
							continue;
						if (arrEntities.IndexOf(team.TeamEntity) >= 0)
							continue;
						arrEntities.Add(team.TeamEntity);
						Sport.Entities.Player[] players = team.TeamEntity.GetPlayers();
						foreach (Sport.Entities.Player player in players)
						{
							if (arrEntities.IndexOf(player) >= 0)
								continue;
							arrEntities.Add(player);
							Sport.Entities.Student student = player.Student;
							if ((student != null) && (arrEntities.IndexOf(student) < 0))
								arrEntities.Add(student);
						}
						if (team.TeamEntity.School == null)
							continue;
						if (arrEntities.IndexOf(team.TeamEntity.School) >= 0)
							continue;
						arrEntities.Add(team.TeamEntity.School);
					}
					if (group is Sport.Championships.MatchGroup)
					{
						foreach (Sport.Championships.Round round in
							(group as Sport.Championships.MatchGroup).Rounds)
						{
							foreach (Sport.Championships.Cycle cycle in round.Cycles)
							{
								foreach (Sport.Championships.Match match in cycle.Matches)
								{
									Sport.Entities.Facility facility = match.Facility;
									Sport.Entities.Court court = match.Court;
									Sport.Entities.Functionary[] functionaries = null;
									if ((match.Functionaries != null) && (match.Functionaries.Length > 0))
									{
										functionaries = new Sport.Entities.Functionary[match.Functionaries.Length];
										for (int i = 0; i < match.Functionaries.Length; i++)
											functionaries[i] = new Sport.Entities.Functionary(match.Functionaries[i]);
									}
									if ((facility != null) && (arrEntities.IndexOf(facility) < 0))
										arrEntities.Add(facility);
									if ((court != null) && (arrEntities.IndexOf(court) < 0))
										arrEntities.Add(court);
									if (functionaries != null)
									{
										foreach (Sport.Entities.Functionary functionary in functionaries)
										{
											if (arrEntities.IndexOf(functionary) >= 0)
												continue;
											arrEntities.Add(functionary);
											Sport.Entities.Region region = functionary.Region;
											if (region == null)
												continue;
											if (arrEntities.IndexOf(region) >= 0)
												continue;
											arrEntities.Add(region);
										}
									} //end if got functionaries
								} //end loop over matches
							} //end loop over cycles
						} //end loop over the round
					} //end if match group
					else if (group is CompetitionGroup)
					{
						foreach (Sport.Championships.Competition competition in
							(group as Sport.Championships.CompetitionGroup).Competitions)
						{
							Sport.Entities.Facility facility = competition.Facility;
							Sport.Entities.Court court = competition.Court;
							Sport.Entities.SportField sportField = competition.SportField;
							Sport.Entities.SportFieldType sportFieldType = null;
							if (sportField != null)
								sportFieldType = sportField.SportFieldType;
							if ((facility != null) && (arrEntities.IndexOf(facility) < 0))
								arrEntities.Add(facility);
							if ((court != null) && (arrEntities.IndexOf(court) < 0))
								arrEntities.Add(court);
							if ((sportField != null) && (arrEntities.IndexOf(sportField) < 0))
								arrEntities.Add(sportField);
							if ((sportFieldType != null) && (arrEntities.IndexOf(sportFieldType) < 0))
								arrEntities.Add(sportFieldType);
						} //end loop over competitions
					} //end if competition group
				} //end loop over groups
			} //end loop over phases

			Sport.UI.Dialogs.WaitForm.SetProgress(25);
			int range = 80 - 25;

			//export:
			double ratio = 0;
			double progress = 0;
			int step = 25;
			if (arrEntities.Count > 0)
				ratio = (((double)range) / ((double)arrEntities.Count));
			foreach (Sport.Data.EntityBase entity in arrEntities)
			{
				entity.Export();
				progress += ratio;
				if (progress >= 1)
				{
					step = (int)(step + progress + 0.5);
					if (step > 80)
						step = 80;
					Sport.UI.Dialogs.WaitForm.SetProgress(step);
					progress = 0;
				}
			}
		}

		private void ExportRule(System.Type valueType, System.Type ruleType,
			Sport.Championships.Competition competition)
		{
			//get rule:
			object rule = null;
			int sportFieldID = (competition == null) ? -1 : competition.SportField.Id;
			if (competition == null)
				rule = this.ChampionshipCategory.GetRule(valueType);
			else
				rule = competition.GetRule(valueType, ruleType);

			//got anything?
			if (rule == null)
				return;

			//save offline rule:
			Sport.Rulesets.Ruleset.SaveOfflineRule(ruleType,
				rule, this.ChampionshipCategory.Id, sportFieldID);
		}

		private void ExportGlobalRule(System.Type valueType, System.Type ruleType)
		{
			ExportRule(valueType, ruleType, null);
		}

		private void ExportCompetitionRule(System.Type valueType, System.Type ruleType)
		{
			System.Collections.ArrayList arrSportFields =
				new System.Collections.ArrayList();
			foreach (Sport.Championships.CompetitionPhase phase in this.Phases)
			{
				foreach (Sport.Championships.CompetitionGroup group in phase.Groups)
				{
					foreach (Sport.Championships.Competition competition in group.Competitions)
					{
						int sportFieldID = competition.SportField.Id;
						if (arrSportFields.IndexOf(sportFieldID) < 0)
						{
							ExportRule(valueType, ruleType, competition);
							arrSportFields.Add(sportFieldID);
						}
					}
				}
			}
		}
		#endregion

		private void ChampionshipTeamAdded(object sender, Sport.Data.EntityEventArgs e)
		{
			if (_championshipCategory == null)
				return;
			Sport.Entities.Team team = new Sport.Entities.Team(e.Entity);
			if (team.Category == _championshipCategory)
			{
				_teams.Add(team);
			}
		}

		private void ChampionshipTeamRemoved(object sender, Sport.Data.EntityEventArgs e)
		{
			if (_championshipCategory == null)
				return;
			Sport.Entities.Team removedTeam = new Sport.Entities.Team(e.Entity);
			if (removedTeam.Category == _championshipCategory)
			{
				_teams.Remove(removedTeam);

				foreach (Phase phase in _phases)
				{
					foreach (Group group in phase.Groups)
					{
						foreach (Team team in group.Teams)
						{
							if (team.TeamEntity == removedTeam)
							{
								team.TeamEntity = null;
							}
						}
					}
				}
			}
		}

		#region IDisposable Members

		public void Dispose()
		{
			if (_championshipCategory != null)
			{
				Rulesets.Ruleset.RulesetChanged -= new Sport.Rulesets.RulesetEventHandler(RulesetStored);
				_championshipCategory.Championship.Entity.EntityType.EntityStored -= new Sport.Data.EntityEventHandler(ChampionshipStored);
				_championshipCategory.Championship.Sport.Entity.EntityType.EntityStored -= new Sport.Data.EntityEventHandler(SportStored);
			}
		}

		#endregion
	}

	#endregion

	#region MatchChampionship Class

	/// <summary>
	/// MatchChampionship extends Championship to use MatchPhase
	/// </summary>
	public class MatchChampionship : Championship
	{
		#region Phases

		#region MatchPhaseCollection

		public class MatchPhaseCollection : Championship.PhaseCollection
		{
			public MatchPhaseCollection(MatchChampionship championship)
				: base(championship)
			{
			}

			public new MatchPhase this[int index]
			{
				get { return (MatchPhase)GetItem(index); }
				set { SetItem(index, value); }
			}

			public void Insert(int index, MatchPhase value)
			{
				InsertItem(index, value);
			}

			public void Remove(MatchPhase value)
			{
				RemoveItem(value);
			}

			public bool Contains(MatchPhase value)
			{
				return base.Contains(value);
			}

			public int IndexOf(MatchPhase value)
			{
				return base.IndexOf(value);
			}

			public int Add(MatchPhase value)
			{
				return AddItem(value);
			}
		}

		#endregion

		protected override PhaseCollection CreatePhaseCollection()
		{
			PhaseCollection phases = new MatchPhaseCollection(this);
			phases.Changed += new Sport.Common.CollectionEventHandler(PhasesChanged);
			return phases;
		}

		public new MatchPhaseCollection Phases
		{
			get { return (MatchPhaseCollection)base.Phases; }
		}

		public new MatchPhase CurrentPhase
		{
			get { return (MatchPhase)base.CurrentPhase; }
		}

		#endregion

		#region Constructors
		internal MatchChampionship(int categoryID, string strLogPath)
			: base(categoryID, strLogPath)
		{
		}

		internal MatchChampionship(int categoryID)
			: base(categoryID, null)
		{
		}

		internal MatchChampionship(Sport.Entities.ChampionshipCategory championshipCategory, string strLogPath)
			: this(championshipCategory.Id, strLogPath)
		{
		}

		internal MatchChampionship(Sport.Entities.ChampionshipCategory championshipCategory)
			: this(championshipCategory.Id, null)
		{
		}

		public MatchChampionship(string name, Sport.Entities.Team[] teams)
			: base(name, teams)
		{
		}

		#endregion

		public override string ToString()
		{
			return Name;
		}

		public int GetMaxMatchNumber()
		{
			int number = 0;

			foreach (Phase phase in Phases)
			{
				foreach (MatchGroup group in phase.Groups)
				{
					foreach (Round round in group.Rounds)
					{
						foreach (Cycle cycle in round.Cycles)
						{
							foreach (Match match in cycle.Matches)
							{
								if (match.Number > number)
									number = match.Number;
							}
						}
					}
				}
			}

			return number;
		}

		public Match GetMatch(int number)
		{
			foreach (Phase phase in Phases)
			{
				foreach (MatchGroup group in phase.Groups)
				{
					foreach (Round round in group.Rounds)
					{
						foreach (Cycle cycle in round.Cycles)
						{
							foreach (Match match in cycle.Matches)
							{
								if (match.Number == number)
									return match;
							}
						}
					}
				}
			}

			return null;
		}

		private void PhasesChanged(object sender, Sport.Common.CollectionEventArgs e)
		{
			// When adding or removing a phase
			// the next phase's teams and events should be removed
			int clearPhase = -1;
			if (e.EventType == Sport.Common.CollectionEventType.Remove)
			{
				if (e.Index < Phases.Count - 1)
					clearPhase = e.Index;
			}
			else if (e.EventType == Sport.Common.CollectionEventType.Insert)
			{
				if (e.Index < Phases.Count - 1)
					clearPhase = e.Index + 1;
			}

			if (clearPhase != -1)
			{
				Sport.Championships.MatchPhase phase = Phases[clearPhase];

				foreach (Sport.Championships.MatchGroup group in phase.Groups)
				{
					group.Teams.Clear();
					foreach (Sport.Championships.Round round in group.Rounds)
					{
						foreach (Sport.Championships.Cycle cycle in round.Cycles)
						{
							cycle.Matches.Clear();
						}
					}
				}
			}
		}
	}

	#endregion

	#region CompetitionChampionship Class

	public class CompetitionChampionship : Championship
	{

		#region Rules

		private Sport.Rulesets.Rules.TeamScoreCounters _teamScoreCounters;
		public Sport.Rulesets.Rules.TeamScoreCounters TeamScoreCounters
		{
			get
			{
				if (_teamScoreCounters == null)
				{

					if (Sport.Core.Session.Connected)
					{
						_teamScoreCounters = ChampionshipCategory.GetRule(typeof(Sport.Rulesets.Rules.TeamScoreCounters))
							as Sport.Rulesets.Rules.TeamScoreCounters;
					}
					else
					{
						object rule = Sport.Rulesets.Ruleset.LoadOfflineRule(
							typeof(Sport.Rulesets.Rules.TeamScoreCountersRule),
							this.CategoryID, -1);
						if (rule != null)
						{
							_teamScoreCounters =
								(Sport.Rulesets.Rules.TeamScoreCounters)rule;
						}
					}
				}

				return _teamScoreCounters;
			}
		}

		private Sport.Rulesets.Rules.TeamPhaseScoring _teamPhaseScoring;
		public Sport.Rulesets.Rules.TeamPhaseScoring TeamPhaseScoring
		{
			get
			{
				if (_teamPhaseScoring == null)
				{

					if (Sport.Core.Session.Connected)
					{
						_teamPhaseScoring = ChampionshipCategory.GetRule(typeof(Sport.Rulesets.Rules.TeamPhaseScoring))
							as Sport.Rulesets.Rules.TeamPhaseScoring;
					}
					else
					{
						int[] arrSportFields = GetAllSportFields();
						object rule = Sport.Rulesets.Ruleset.LoadOfflineRule(
							typeof(Sport.Rulesets.Rules.TeamPhaseScoringRule),
							this.CategoryID, arrSportFields);
						if (rule != null)
						{
							_teamPhaseScoring =
								(Sport.Rulesets.Rules.TeamPhaseScoring)rule;
						}
					}
				}

				return _teamPhaseScoring;
			}
		}

		private int[] GetAllSportFields()
		{
			ArrayList arrSportFields = new ArrayList();
			arrSportFields.Add(-1);
			foreach (Phase phase in this.Phases)
			{
				foreach (CompetitionGroup group in phase.Groups)
				{
					foreach (Competition competition in group.Competitions)
					{
						if (competition.SportField != null)
						{
							int sportFieldId = competition.SportField.Id;
							if (arrSportFields.IndexOf(sportFieldId) < 0)
								arrSportFields.Add(sportFieldId);
						}
					}
				}
			}
			return (int[])arrSportFields.ToArray(typeof(int));
		}

		private int _defaultCounter;
		public int DefaultCounter
		{
			get
			{
				if (_defaultCounter == -1)
				{
					Sport.Rulesets.Rules.TeamScoreCounter teamScoreCounter = null;
					if (Sport.Core.Session.Connected)
					{
						teamScoreCounter = ChampionshipCategory.GetRule(typeof(Sport.Rulesets.Rules.TeamScoreCounter))
							as Sport.Rulesets.Rules.TeamScoreCounter;
					}
					else
					{
						object rule = Sport.Rulesets.Ruleset.LoadOfflineRule(
							typeof(Sport.Rulesets.Rules.TeamScoreCounterRule),
							this.CategoryID, -1);
						if (rule != null)
							teamScoreCounter = (Sport.Rulesets.Rules.TeamScoreCounter)rule;
					}

					if (teamScoreCounter != null)
					{
						for (int n = 0; n < TeamScoreCounters.Counters.Count && _defaultCounter == -1; n++)
						{
							if (TeamScoreCounters.Counters[n].Name == teamScoreCounter.Counter)
								_defaultCounter = n;
						}
					}
				}

				return _defaultCounter;
			}
		}

		internal override void OnRulesetChange()
		{
			_teamScoreCounters = null;
			_defaultCounter = -1;

			base.OnRulesetChange();
		}

		#endregion

		#region Phases

		#region CompetitionPhaseCollection

		public class CompetitionPhaseCollection : Championship.PhaseCollection
		{
			public CompetitionPhaseCollection(CompetitionChampionship championship)
				: base(championship)
			{
			}

			public new CompetitionPhase this[int index]
			{
				get { return (CompetitionPhase)GetItem(index); }
				set { SetItem(index, value); }
			}

			public void Insert(int index, CompetitionPhase value)
			{
				InsertItem(index, value);
			}

			public void Remove(CompetitionPhase value)
			{
				RemoveItem(value);
			}

			public bool Contains(CompetitionPhase value)
			{
				return base.Contains(value);
			}

			public int IndexOf(CompetitionPhase value)
			{
				return base.IndexOf(value);
			}

			public int Add(CompetitionPhase value)
			{
				return AddItem(value);
			}
		}

		#endregion

		protected override PhaseCollection CreatePhaseCollection()
		{
			PhaseCollection phases = new CompetitionPhaseCollection(this);
			return phases;
		}

		public new CompetitionPhaseCollection Phases
		{
			get { return (CompetitionPhaseCollection)base.Phases; }
		}

		public new CompetitionPhase CurrentPhase
		{
			get { return (CompetitionPhase)base.CurrentPhase; }
		}

		#endregion

		#region Constructors
		internal CompetitionChampionship(int categoryID)
			: base(categoryID)
		{
			_teamScoreCounters = null;
			_defaultCounter = -1;
		}

		internal CompetitionChampionship(Sport.Entities.ChampionshipCategory championshipCategory)
			: this(championshipCategory.Id)
		{
		}

		public CompetitionChampionship(string name, Sport.Entities.Team[] teams)
			: base(name, teams)
		{
			_teamScoreCounters = null;
			_defaultCounter = -1;
		}
		#endregion

		protected override void Initialize()
		{
			base.Initialize();

			foreach (CompetitionPhase phase in Phases)
			{
				foreach (CompetitionGroup group in phase.Groups)
				{
					group.CalculateTeamsScore();
				}
			}
		}

		public override string ToString()
		{
			return Name;
		}

		public int GetHeatCount()
		{
			int result = 0;
			foreach (Phase phase in this.Phases)
				foreach (CompetitionGroup group in phase.Groups)
					result += group.GetHeatCount();
			return result;
		}

		public Sport.Documents.Data.Table GetClubCompetitionsTable(
			ref string strError)
		{
			//strError = "test";
			//return Sport.Documents.Data.Table.Empty;

			//get table of all the teams:
			Hashtable tblBestResults = new Hashtable();
			Hashtable tblClubSchools = GetAllClubSchoolData(ref tblBestResults);

			//got anything?
			strError = "";
			if (tblClubSchools.Count == 0)
			{
				strError = "לא נמצאו תוצאות בתחרויות שלב זה";
				return Sport.Documents.Data.Table.Empty;
			}

			//iterate over sport field types, take best results only.
			Hashtable tblScores = new Hashtable();
			Hashtable tblActualResults = new Hashtable();
			ArrayList arrSportFieldTypes = new ArrayList();
			foreach (object oSchool in tblClubSchools.Keys)
			{
				//reset score:
				int totalScore = 0;

				//initialize actual results table:
				tblActualResults[oSchool] = new Hashtable();

				//get sport field types:
				Hashtable tblSportFieldTypes = (Hashtable)tblClubSchools[oSchool];

				//iterate over sport field types:
				ArrayList arrCompetitorsPool = new ArrayList();
				int actualAmount = 0;
				foreach (Sport.Entities.SportFieldType sportFieldType in tblSportFieldTypes.Keys)
				{
					//add to list:
					if (arrSportFieldTypes.IndexOf(sportFieldType) < 0)
						arrSportFieldTypes.Add(sportFieldType);

					//get best results:
					int bestResults = Sport.Common.Tools.CIntDef(
						tblBestResults[sportFieldType.Id], 999);

					//get competitors list:
					ArrayList arrCompetitors = (ArrayList)tblSportFieldTypes[sportFieldType];

					//take best results:
					actualAmount = 0;
					for (int i = 0; i < bestResults; i++)
					{
						//exceeded amount of competitors?
						if (arrCompetitors.Count == 0)
							break;

						//get score:
						int curScore = 0;
						object oComp = arrCompetitors[0];
						if (oComp is Sport.Championships.Competitor)
							curScore = (oComp as Sport.Championships.Competitor).Score;
						else if (oComp is Sport.Entities.OfflinePlayer)
							curScore = (oComp as Sport.Entities.OfflinePlayer).Score;

						//add current score:
						totalScore += curScore;

						//remove from list:
						arrCompetitors.RemoveAt(0);
						actualAmount++;
					}
					(tblActualResults[oSchool] as Hashtable)[sportFieldType.Id] = actualAmount;

					//add all the rest to the pool:
					arrCompetitorsPool.AddRange((object[])arrCompetitors.ToArray());
				} //end loop over sport field types

				//sort by score:
				arrCompetitorsPool.Sort(new CompetitorsScoreComparer(false));

				//get best results from competitors pool:
				int generalBestResults = (int)tblBestResults[-1];
				actualAmount = 0;
				for (int i = 0; i < arrCompetitorsPool.Count; i++)
				{
					//got enough?
					if (i >= generalBestResults)
						break;

					//get score:
					int curScore = 0;
					object oComp = arrCompetitorsPool[i];
					if (oComp is Sport.Championships.Competitor)
						curScore = (oComp as Sport.Championships.Competitor).Score;
					else if (oComp is Sport.Entities.OfflinePlayer)
						curScore = (oComp as Sport.Entities.OfflinePlayer).Score;

					//add current score:
					totalScore += curScore;
					actualAmount++;
				}
				(tblActualResults[oSchool] as Hashtable)[-1] = actualAmount;

				//apply score:
				tblScores[oSchool] = totalScore;
			} //end loop over schools

			//build data table.
			int colCount = 3 + arrSportFieldTypes.Count + 1;
			Sport.Documents.Data.Table table = new Sport.Documents.Data.Table();
			table.Headers = new Sport.Documents.Data.Column[colCount];
			double compColWidth = (0.7 / (arrSportFieldTypes.Count + 1));
			int col = 0;
			table.Headers[col++] = new Sport.Documents.Data.Column("מקום", 0.07);
			table.Headers[col++] = new Sport.Documents.Data.Column("שם מוסד", 0.15);
			foreach (Sport.Entities.SportFieldType sportFieldType in arrSportFieldTypes)
				table.Headers[col++] = new Sport.Documents.Data.Column(sportFieldType.Name, compColWidth);
			table.Headers[col++] = new Sport.Documents.Data.Column("תוצאות נוספות", compColWidth);
			table.Headers[col++] = new Sport.Documents.Data.Column("סה\"כ נקודות", 0.08);
			ArrayList arrTableRows = new ArrayList();
			foreach (object oSchool in tblClubSchools.Keys)
			{
				//get sport field types:
				Hashtable tblSportFieldTypes = (Hashtable)tblClubSchools[oSchool];

				//get score:
				int curScore = (int)tblScores[oSchool];

				//create current row
				Sport.Documents.Data.Row row = new Sport.Documents.Data.Row();

				//initialize cells:
				row.Cells = new Sport.Documents.Data.Cell[colCount];
				col = 0;

				//rank:
				row.Cells[col++] = new Sport.Documents.Data.Cell("");

				//get data:
				string strSchoolName = "";
				if (oSchool is Sport.Entities.School)
				{
					strSchoolName = (oSchool as Sport.Entities.School).Name;
				}
				else if (oSchool is Sport.Entities.OfflineSchool)
				{
					strSchoolName = (oSchool as Sport.Entities.OfflineSchool).Name;
				}

				//school name:
				row.Cells[col++] = new Sport.Documents.Data.Cell(strSchoolName);

				//sport field types
				foreach (Sport.Entities.SportFieldType sportFieldType in arrSportFieldTypes)
				{
					row.Cells[col++] =
						new Sport.Documents.Data.Cell(Sport.Common.Tools.CStrDef(
						(tblActualResults[oSchool] as Hashtable)[sportFieldType.Id],
						"0"));
					row.Cells[col - 1].Alignment = Sport.Documents.TextAlignment.Center;
				}

				//general best results:
				row.Cells[col++] = new Sport.Documents.Data.Cell((tblActualResults[oSchool] as Hashtable)[-1].ToString());
				row.Cells[col - 1].Alignment = Sport.Documents.TextAlignment.Center;

				//total score:
				row.Cells[col++] = new Sport.Documents.Data.Cell(curScore.ToString());

				//add current row
				arrTableRows.Add(row);
			} //end loop over club schools.

			//sort table rows:
			arrTableRows.Sort(new Sport.Documents.Data.DataTableRowsComparer(colCount - 1));

			//apply new rank:
			for (int i = 0; i < arrTableRows.Count; i++)
				((Sport.Documents.Data.Row)arrTableRows[i]).Cells[0].Text = (i + 1).ToString();

			//apply rows in data table:
			table.Rows = (Sport.Documents.Data.Row[])
				arrTableRows.ToArray(typeof(Sport.Documents.Data.Row));

			return table;
		}

		private Hashtable GetAllClubSchoolData(ref Hashtable tblBestResults)
		{
			//initialize result:
			Hashtable tblSchools = new Hashtable();

			//get championship category:
			Sport.Entities.ChampionshipCategory category = this.ChampionshipCategory;

			//general best results:
			int bestResults = 999;
			if (Sport.Core.Session.Connected)
			{
				Sport.Rulesets.Rules.TeamScoreCounters objCountersRule =
					(Sport.Rulesets.Rules.TeamScoreCounters)
					category.GetRule(typeof(Sport.Rulesets.Rules.TeamScoreCounters));
				Sport.Rulesets.Rules.TeamScoreCounter objCounterRule =
					(Sport.Rulesets.Rules.TeamScoreCounter)
					category.GetRule(typeof(Sport.Rulesets.Rules.TeamScoreCounter));
				if ((objCountersRule != null) && (objCounterRule != null))
				{
					string strCounterName = objCounterRule.Counter;
					for (int i = 0; i < objCountersRule.Counters.Count; i++)
					{
						if (objCountersRule.Counters[i].Name == strCounterName)
						{
							bestResults = objCountersRule.Counters[i].Results;
							break;
						}
					}
				}
			}
			tblBestResults[-1] = bestResults;

			//iterate over all phases.
			foreach (CompetitionPhase phase in this.Phases)
			{
				//iterate over the phase groups:
				foreach (CompetitionGroup group in phase.Groups)
				{
					//iterate over the group competitions:
					foreach (Competition competition in group.Competitions)
					{
						//get sport field type:
						Sport.Entities.SportFieldType sportFieldType =
							competition.SportField.SportFieldType;

						//best results:
						if (tblBestResults[sportFieldType.Id] == null)
							tblBestResults[sportFieldType.Id] = competition.GetBestResults();

						//get all competitors and offline players.
						ArrayList arrCompetitors = new ArrayList(competition.Competitors);
						arrCompetitors.AddRange(competition.GetOfflinePlayers());

						//iterate over the competition competitors:
						foreach (object oComp in arrCompetitors)
						{
							//get data:
							object school = null;
							if (oComp is Sport.Championships.Competitor)
							{
								Sport.Championships.Competitor competitor =
									(Sport.Championships.Competitor)oComp;
								if ((competitor.Player != null) &&
									(competitor.Player.CompetitionTeam != null) &&
									(competitor.Player.CompetitionTeam.TeamEntity != null))
								{
									school = competitor.Player.CompetitionTeam.TeamEntity.School;
								}
							}
							else if (oComp is Sport.Entities.OfflinePlayer)
							{
								Sport.Entities.OfflinePlayer oPlayer =
									(Sport.Entities.OfflinePlayer)oComp;
								if (oPlayer.Team != null)
									school = oPlayer.Team.School;
								else if (oPlayer.OfflineTeam != null)
								{
									Sport.Entities.OfflineTeam oTeam = oPlayer.OfflineTeam;
									if (oTeam.School != null)
										school = oTeam.School;
									else if (oTeam.OfflineSchool != null)
										school = oTeam.OfflineSchool;
								}
							}

							//got anything?
							if (school == null)
								continue;

							//need to create team data?
							if (tblSchools[school] == null)
								tblSchools[school] = new Hashtable();

							//grab team data:
							Hashtable tblSportFieldTypes = (Hashtable)tblSchools[school];

							//need to create sport field type data?
							if (tblSportFieldTypes[sportFieldType] == null)
								tblSportFieldTypes[sportFieldType] = new ArrayList();

							//add competitor to the table:
							(tblSportFieldTypes[sportFieldType] as ArrayList).Add(oComp);
						} //end loop over competitors
					} //end loop over competitions
				} //end loop over groups
			} //end loop over phases

			//sort competitors by their score:
			foreach (Hashtable tblSportFieldTypes in tblSchools.Values)
			{
				//sort competitors by their score:
				foreach (ArrayList arrCompetitors in tblSportFieldTypes.Values)
					arrCompetitors.Sort(new CompetitorsScoreComparer(false));
			}

			//done.
			return tblSchools;
		} //end function GetAllTeams
	}

	public class PlayerNumberComparer : IComparer
	{
		public int Compare(object x, object y)
		{
			Sport.Entities.Player p1 = (Sport.Entities.Player)x;
			Sport.Entities.Player p2 = (Sport.Entities.Player)y;
			return p1.Number.CompareTo(p2.Number);
		}
	}


	public class CompetitorsScoreComparer : IComparer
	{
		private int _multiplyer = 1;

		public CompetitorsScoreComparer(bool blnScoreIsRank)
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
			if (x is Sport.Championships.Competitor)
				s1 = (x as Sport.Championships.Competitor).Score;
			else if (x is Sport.Entities.OfflinePlayer)
				s1 = (x as Sport.Entities.OfflinePlayer).Score;
			if (y is Sport.Championships.Competitor)
				s2 = (y as Sport.Championships.Competitor).Score;
			else if (y is Sport.Entities.OfflinePlayer)
				s2 = (y as Sport.Entities.OfflinePlayer).Score;
			return -1 * (_multiplyer) * (s1.CompareTo(s2));
		}
	}
	#endregion

	#region ChampionshipException Class

	public class ChampionshipException : Exception
	{
		public ChampionshipException()
		{
		}

		public ChampionshipException(string s)
			: base(s)
		{
		}

		public ChampionshipException(System.Runtime.Serialization.SerializationInfo info,
			System.Runtime.Serialization.StreamingContext context)
			: base(info, context)
		{
		}
	}

	#endregion
}
