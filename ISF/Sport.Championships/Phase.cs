using System;
using System.Collections;

namespace Sport.Championships
{
	#region Phase Class

	public class Phase : Sport.Common.GeneralCollection.CollectionItem
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
				Changed();
			}
		}

		internal Status _status;
		public Status Status
		{
			get { return _status; }
		}

		internal int _index;
		public int Index
		{
			get { return _index; }
		}

		public bool Editable
		{
			get 
			{
				return Championship == null || Championship.Editing;
			}
		}
        
		#endregion

		#region CollectionItem Members

		public Championship Championship
		{
			get { return ((Championship) Owner); }
		}

		public override void OnOwnerChange(object oo, object no)
		{
			if (no == null)
				_index = -1;
		}

		#endregion

		#region Groups

		#region GroupCollection

		public class GroupCollection : Sport.Common.GeneralCollection
		{
			public GroupCollection(Phase phase)
				: base(phase)
			{
			}

			protected override void SetItem(int index, object value)
			{
				if (!((Phase) Owner).Editable)
					throw new ChampionshipException("Not in structure edit - cannot change groups");

				base.SetItem (index, value);
			}

			protected override void InsertItem(int index, object value)
			{
				if (!((Phase) Owner).Editable)
					throw new ChampionshipException("Not in structure edit - cannot change groups");

				base.InsertItem (index, value);
			}

			protected override void RemoveItem(int index)
			{
				if (!((Phase) Owner).Editable)
					throw new ChampionshipException("Not in structure edit - cannot change groups");

				base.RemoveItem (index);
			}

			public Group this[int index]
			{
				get { return (Group) GetItem(index); }
				set { SetItem(index, value); }
			}

			public void Insert(int index, Group value)
			{
				InsertItem(index, value);
			}

			public void Remove(Group value)
			{
				RemoveItem(value);
			}

			public bool Contains(Group value)
			{
				return base.Contains(value);
			}

			public int IndexOf(Group value)
			{
				return base.IndexOf(value);
			}

			public int Add(Group value)
			{
				return AddItem(value);
			}
		}

		#endregion

		private GroupCollection _groups;
		public GroupCollection Groups
		{
			get { return _groups; }
		}

		protected virtual GroupCollection CreateGroupCollection()
		{
			return new GroupCollection(this);
		}

		private void GroupsChanged(object sender, Sport.Common.CollectionEventArgs e)
		{
			for (int i = e.Index; i < _groups.Count; i++)
				_groups[i]._index = i;

			Changed();
		}

		#endregion

		#region Definitions

		#region DefinitionTable

		public class DefinitionKey
		{
			private int _ruleType;
			public int RuleType
			{
				get { return _ruleType; }
			}
			private string _definition;
			public string Definition
			{
				get { return _definition; }
			}

			public DefinitionKey(int ruleType, string definition)
			{
				_ruleType = ruleType;
				_definition = definition;
			}

			public override bool Equals(object obj)
			{
				DefinitionKey o = obj as DefinitionKey;
				if (o == null)
					return false;

				return o._ruleType == _ruleType && o._definition == _definition;
			}

			public override int GetHashCode()
			{
				return _definition.GetHashCode();
			}
		}

		public class DefinitionTable : IEnumerable
		{
			public DefinitionTable(Phase phase)
			{
				_definitions = new Hashtable();
				_phase = phase;
			}

			private Phase _phase;
			private Hashtable _definitions;

			public string this[DefinitionKey key]
			{
				get { return Get(key); }
				set { Set(key, value); }
			}

			public int Count
			{
				get { return _definitions.Count; }
			}

			public string Get(DefinitionKey key)
			{
				return _definitions[key] as string;
			}

			public string Get(int ruleType, string definition)
			{
				return Get(new DefinitionKey(ruleType, definition));
			}
            
			public void Set(DefinitionKey key, string value)
			{
				if (!_phase.Editable)
					throw new ChampionshipException("Not in structure edit - cannot change definitions");

				if (value == null)
				{
					_definitions.Remove(key);
				}
				else
				{
					_definitions[key] = value;
				}
			}

			public void Set(int ruleType, string definition, string value)
			{
				Set(new DefinitionKey(ruleType, definition), value);
			}

			#region IEnumerable Members

			public IEnumerator GetEnumerator()
			{
				return _definitions.GetEnumerator();
			}

			#endregion
		}

		#endregion

		private DefinitionTable _definitions;
		public DefinitionTable Definitions
		{
			get { return _definitions; }
		}

		#endregion

		#region Rules

		internal virtual void OnRulesetChange()
		{
			foreach (Group group in _groups)
				group.OnRulesetChange();
		}

		#endregion

		#region Status

		public bool HasResults()
		{
			foreach (Group group in Groups)
			{
				if (group.HasResults())
					return true;
			}

			return false;
		}

		public virtual string CanStart()
		{
			// Phase must have at least one group with
			// at least two teams in each group
			if (_groups.Count == 0)
				return "לשלב זה אין בתים";

			foreach (Group group in _groups)
			{
				if (group.Teams.Count < 2)
					return "בבית '"+group.Name+"' יש פחות משתי קבוצות";
				
				// And for the first phase, every team must be checked
				if (_index == 0)
				{
					foreach (Team team in group.Teams)
					{
						if (!team.IsConfirmed())
							return "הקבוצה '"+team.Name+"' לא עברה אישור";
					}
				}
			}
			
			return "";
		}

		public virtual void Reset()
		{
			/* if (Status == Status.Planned)
				throw new ChampionshipException("Phase not started"); */

			foreach (Group group in Groups)
			{
				group.Reset();
			}
            
			_status = Status.Planned;
		}

        
		public virtual void Start()
		{
			_status = Status.Started;
			Changed();
		}

		public virtual void End()
		{
			_status = Status.Ended;
			Changed();
		}

		#endregion

		#region Constructors

		public Phase(string name, Status status)
		{
			_index = -1;
			_name = name;
			_status = status;
			_changed = false;

			_groups = CreateGroupCollection();
			_groups.Changed += new Sport.Common.CollectionEventHandler(GroupsChanged);

			_definitions = new DefinitionTable(this);
		}

		#endregion

		#region Save

		internal bool _changed;
		internal void Changed()
		{
			_changed = true;
		}

		internal void Saved()
		{
			_changed = false;
		}

		internal virtual SportServices.Phase Save()
		{
			SportServices.Phase sphase = new SportServices.Phase();
			sphase.Name = Name;
			sphase.Status = (int) Status;

			sphase.Groups = new SportServices.Group[Groups.Count];

			for (int n = 0; n < Groups.Count; n++)
			{
				sphase.Groups[n] = Groups[n].Save();
			}

			sphase.Definitions = new SportServices.PhaseDefinition[Definitions.Count];
			int index = 0;
			foreach (DictionaryEntry entry in Definitions)
			{
				SportServices.PhaseDefinition definition = new SportServices.PhaseDefinition();
				DefinitionKey key = (DefinitionKey) entry.Key;
				definition.RuleType = key.RuleType;
				definition.Definition = key.Definition;
				definition.Value = (string) entry.Value;
				sphase.Definitions[index] = definition;
				index++;
			}

			return sphase;
		}

		#endregion
		
		#region general
		public  Sport.Championships.Team[] GetTeams()
		{
			System.Collections.ArrayList teams=new System.Collections.ArrayList();
			if (this.Groups != null)
			{
				foreach (Sport.Championships.Group group in this.Groups)
				{
					if (group.Teams != null)
						teams.AddRange(group.Teams);
				}
			}
			return (Sport.Championships.Team[])
				teams.ToArray(typeof(Sport.Championships.Team));
		}
		#endregion

		public override string ToString()
		{
			return this.Name;
		}
	}

	#endregion

	#region MatchPhase Class

	/// <summary>
	/// The Phase class defines a part of a championship in which the
	/// groups and teams definition remains the same.
	/// Each phase defines the phase name and contains
	/// groups collection.
	/// </summary>
	public class MatchPhase : Phase
	{
		#region Groups 

		#region MatchGroupCollection

		public class MatchGroupCollection : Phase.GroupCollection
		{
			public MatchGroupCollection(MatchPhase phase)
				: base(phase)
			{
			}

			public new MatchGroup this[int index]
			{
				get { return (MatchGroup) GetItem(index); }
				set { SetItem(index, value); }
			}

			public void Insert(int index, MatchGroup value)
			{
				InsertItem(index, value);
			}

			public void Remove(MatchGroup value)
			{
				RemoveItem(value);
			}

			public bool Contains(MatchGroup value)
			{
				return base.Contains(value);
			}

			public int IndexOf(MatchGroup value)
			{
				return base.IndexOf(value);
			}

			public int Add(MatchGroup value)
			{
				return AddItem(value);
			}
		}

		#endregion

		public new MatchGroupCollection Groups
		{
			get { return (MatchGroupCollection) base.Groups; }
		}

		protected override GroupCollection CreateGroupCollection()
		{
			GroupCollection groups = new MatchGroupCollection(this);
			groups.Changed += new Sport.Common.CollectionEventHandler(GroupsChanged);
			return groups;
		}

		#endregion

		public new MatchChampionship Championship
		{
			get { return ((MatchChampionship) base.Championship); }
		}

		public MatchPhase(string name, Status status)
			: base(name, status)
		{
		}

		internal MatchPhase(SportServices.Phase phase)
			: this(phase.Name, (Status) phase.Status)
		{
			foreach (SportServices.Group sgroup in phase.Groups)
			{
				Groups.Add(new MatchGroup(sgroup));
			}

			foreach (SportServices.PhaseDefinition definition in phase.Definitions)
			{
				Definitions.Set(definition.RuleType, definition.Definition, definition.Value);
			}
		}

		public override string ToString()
		{
			return Name;
		}

		private void GroupsChanged(object sender, Sport.Common.CollectionEventArgs e)
		{
			// When removing a group
			// the next phase's teams and events related to
			// that group should be removed
			if (e.EventType == Sport.Common.CollectionEventType.Remove)
			{
				if (_index < Championship.Phases.Count - 1)
				{
					MatchGroup removed = (MatchGroup) e.Old;
					MatchPhase phase = Championship.Phases[_index + 1];

					foreach (MatchGroup group in phase.Groups)
					{
						int team = 0;
						while (team < group.Teams.Count)
						{
							MatchTeam t = group.Teams[team];
							if (t != null && t.GetPreviousGroup() == removed)
							{
								group.Teams.RemoveAt(team);
							}
							else
							{
								team++;
							}
						}
					}
				}
			}
		}

		public override void Start()
		{
			/* if (Status != Status.Planned)
				throw new ChampionshipException("Phase already been started"); */
			
			/*
			foreach (MatchGroup group in Groups)
			{
				foreach (MatchTeam team in group.Teams)
				{
					if (team.TeamEntity == null)
					{
						//throw new ChampionshipException("Phase team is missing");
						//blnNoTeams = true;
						//break;
					}
				}
			}
			*/
            
			_status = Status.Started;
			Changed();
		}

		public override void End()
		{
			if (Status != Status.Started)
			{
				if (Status == Status.Ended)
					_status = Status.Started;
				else
					throw new Exception("Phase not started ("+this.Status.ToString()+")");
			}

			if (Index < Championship.Phases.Count - 1)
			{
				System.Collections.IComparer comparer = new TeamPositionComparer();
				// Filling next phase teams

				// Creating phase result
				System.Collections.ArrayList al = new System.Collections.ArrayList();
				foreach (MatchGroup group in Groups)
				{
					// Creating group result
					MatchTeam[] teams = new MatchTeam[group.Teams.Count];
					group.Teams.CopyTo(teams, 0);

					Array.Sort(teams, comparer);

					al.Add(teams);
				}

				MatchPhase next = Championship.Phases[_index + 1];
				foreach (MatchGroup group in next.Groups)
				{
					foreach (MatchTeam team in group.Teams)
					{
						if (team.TeamEntity != null)
							continue;

						if (team.PreviousGroup >= al.Count)
						{
							//throw new ChampionshipException("Previous group not found");
							System.Diagnostics.Debug.WriteLine("Previous group not found");
							continue;
						}
						
						MatchTeam[] teams = (MatchTeam[]) al[team.PreviousGroup];
						if (team.PreviousPosition >= teams.Length)
						{
							//throw new ChampionshipException("Previous position not found");
							System.Diagnostics.Debug.WriteLine("Previous position not found");
							continue;
						}
						
						team.TeamEntity = teams[team.PreviousPosition].TeamEntity;
					}
				}
			}

			_status = Status.Ended;
			Changed();
		}
	}

	#endregion

	#region CompetitionPhase Class

	/// <summary>
	/// The Phase class defines a part of a championship in which the
	/// groups and teams definition remains the same.
	/// Each phase defines the phase name and contains
	/// groups collection.
	/// </summary>
	public class CompetitionPhase : Phase
	{
		#region Groups 

		#region CompetitionGroupCollection

		public class CompetitionGroupCollection : Phase.GroupCollection
		{
			public CompetitionGroupCollection(CompetitionPhase phase)
				: base(phase)
			{
			}

			public new CompetitionGroup this[int index]
			{
				get { return (CompetitionGroup) GetItem(index); }
				set { SetItem(index, value); }
			}

			public void Insert(int index, CompetitionGroup value)
			{
				InsertItem(index, value);
			}

			public void Remove(CompetitionGroup value)
			{
				RemoveItem(value);
			}

			public bool Contains(CompetitionGroup value)
			{
				return base.Contains(value);
			}

			public int IndexOf(CompetitionGroup value)
			{
				return base.IndexOf(value);
			}

			public int Add(CompetitionGroup value)
			{
				return AddItem(value);
			}
		}

		#endregion

		public new CompetitionGroupCollection Groups
		{
			get { return (CompetitionGroupCollection) base.Groups; }
		}

		protected override GroupCollection CreateGroupCollection()
		{
			GroupCollection groups = new CompetitionGroupCollection(this);
			return groups;
		}

		#endregion

		public new CompetitionChampionship Championship
		{
			get { return ((CompetitionChampionship) base.Championship); }
		}

		public CompetitionPhase(string name, Status status)
			: base(name, status)
		{
		}

		internal CompetitionPhase(SportServices.Phase phase)
			: this(phase.Name, (Status) phase.Status)
		{
			foreach (SportServices.Group sgroup in phase.Groups)
			{
				Groups.Add(new CompetitionGroup(sgroup));
			}

			foreach (SportServices.PhaseDefinition definition in phase.Definitions)
			{
				Definitions.Set(definition.RuleType, definition.Definition, definition.Value);
			}
		}

		public override string ToString()
		{
			return Name;
		}

		public override void Start()
		{
			/* if (Status != Status.Planned)
				throw new ChampionshipException("Phase already been started"); */

			/*foreach (MatchGroup group in Groups)
			{
				foreach (MatchTeam team in group.Teams)
				{
					if (team.TeamEntity == null)
						throw new ChampionshipException("Phase team is missing");
				}
			}*/
            
			_status = Status.Started;
			Changed();
		}

		public override void End()
		{
			if (Status != Status.Started)
				throw new Exception("Phase not started");

/*			if (Index < Championship.Phases.Count - 1)
			{
				System.Collections.IComparer comparer = new TeamPositionComparer();
				// Filling next phase teams

				// Creating phase result
				System.Collections.ArrayList al = new System.Collections.ArrayList();
				foreach (MatchGroup group in Groups)
				{
					// Creating group result
					MatchTeam[] teams = new MatchTeam[group.Teams.Count];
					group.Teams.CopyTo(teams, 0);

					Array.Sort(teams, comparer);

					al.Add(teams);
				}

				MatchPhase next = Championship.Phases[_index + 1];
				foreach (MatchGroup group in next.Groups)
				{
					foreach (MatchTeam team in group.Teams)
					{
						if (team.TeamEntity == null)
						{
							if (team.PreviousGroup >= al.Count)
								throw new ChampionshipException("Previous group not found");
							MatchTeam[] teams = (MatchTeam[]) al[team.PreviousGroup];
							if (team.PreviousPosition >= teams.Length)
								throw new ChampionshipException("Previous position not found");
							team.TeamEntity = teams[team.PreviousPosition].TeamEntity;
						}
					}
				}
			}*/

			_status = Status.Ended;
			Changed();
		}
	}

	#endregion
}
