using System;

namespace Sport.Producer
{
	public class PhasePattern
	{
		public enum GenerateType
		{
			Position,
			Group
		}

		public class Group : Sport.Common.GeneralCollection.CollectionItem
		{
			private string _name;
			public string Name
			{
				get { return _name; }
				set 
				{
					_name = value; 
					_name.Replace("|", "");
					OnChange();
				}
			}

			private System.Collections.ArrayList teams;
			public int Size
			{
				get { return teams.Count; }
			}
			public int this[int position]
			{
				get { return (int) teams[position]; }
				set 
				{
					if (value < 1)
						throw new ArgumentOutOfRangeException("value", "Team number must be more than 0");

					teams[position] = value; 
					OnChange();
				}
			}
			public int Add(int team)
			{
				int i = teams.Add(team);
				OnChange();
				return i;
			}
			public void Insert(int position, int team)
			{
				teams.Insert(position, team);
			}
			public void RemoveAt(int position)
			{
				teams.RemoveAt(position);
				OnChange();
			}
			public void Remove(int team)
			{
				teams.Remove(team);
				OnChange();
			}

			private void OnChange()
			{
				if (Owner != null)
					((PhaseItem) Owner).OnChange();
			}

			public Group()
			{
				teams = new System.Collections.ArrayList();
			}

			public override string ToString()
			{
				return _name;
			}
		}

		public class Team
		{
			private Group _group;
			public Group Group
			{
				get { return _group; }
			}

			private int _position;
			public int Position
			{
				get { return _position; }
			}

			public Team(Group group, int position)
			{
				_group = group;
				_position = position;
			}
		}

		public class PhaseItem : RangeItem
		{
			#region GroupCollection

			public class GroupCollection : Sport.Common.GeneralCollection
			{
				public GroupCollection(PhaseItem phase)
					: base(phase)
				{
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

			private GroupCollection groups;
			public GroupCollection Groups
			{
				get { return groups; }
			}

			public Phase Phase
			{
				get { return (Phase) Owner; }
			}

			protected override void RangeChanging(int min, int max)
			{
				foreach (Group group in groups)
				{
					int p = 0;
					while (p < group.Size)
					{
						if (group[p] > max)
							group.RemoveAt(p);
						else
							p++;
					}
				}

				int length = max > result.Length ? result.Length : max;
				Team[] newResult = new Team[max];
				for (int n = 0; n < length; n++)
				{
					if (result[n] != null)
					{
						if (result[n].Position <= result[n].Group.Size)
							newResult[n] = result[n];
					}
				}

				result = newResult;

				base.RangeChanging (min, max);

				Phase.CalculateRanges();

				OnChange();
			}


			private Team[] result;
			public Team this[int position]
			{
				get
				{
					if (position >= 0 && position < result.Length)
						return result[position];
					return null;
				}
				set
				{
					if (position >= 0 && position < result.Length)
					{
						result[position] = value;
						OnChange();
					}
				}
			}

			private Sport.Entities.GameBoard gameBoard;
			public Sport.Entities.GameBoard GameBoard
			{
				get { return gameBoard; }
				set 
				{ 
					gameBoard = value; 
					OnChange();
				}
			}

			internal void OnChange()
			{
				for (int n = 0; n < Max; n++)
				{
					if (n < result.Length)
					{
						if (result[n] != null && result[n].Position >= result[n].Group.Size)
						{
							result[n] = null;
						}
					}
				}
				if (Owner != null)
					((Phase) Owner).OnChange();
			}

			public PhaseItem(int min, int max)
				: base(min, max)
			{
				groups = new GroupCollection(this);
				groups.Changed += new Sport.Common.CollectionEventHandler(GroupsChanged);
				result = new Team[max];
			}

			private void GroupsChanged(object sender, Sport.Common.CollectionEventArgs e)
			{
				if (e.EventType == Sport.Common.CollectionEventType.Remove)
				{
					Group group = (Group) e.Old;
					for (int n = 0; n < Max; n++)
					{
						if (result[n] != null && result[n].Group == group)
							result[n] = null;
					}
				}
				OnChange();
			}

			private void ResultChanged(object sender, Sport.Common.CollectionEventArgs e)
			{
				OnChange();
			}

			public void GenerateGroups(GenerateType type)
			{
				int pc = Max / groups.Count;
				if (Max % groups.Count != 0)
					pc++;
				bool[] positions = new bool[Max];
				foreach (Group group in groups)
				{
					for (int p = 0; p < group.Size; p++)
						positions[group[p] - 1] = true;
				}

				int gi = 0;
				while (gi < groups.Count && groups[gi].Size >= pc)
					gi++;
				for (int p = 0; p < Max; p++)
				{
					if (!positions[p])
					{
						groups[gi].Add(p + 1);
						if (type == GenerateType.Group)
						{
							if (groups[gi].Size >= pc)
							{
								gi++;
								while (gi < groups.Count && groups[gi].Size >= pc)
									gi++;
							}
						}
						else
						{
							gi++;
							while (gi < groups.Count && groups[gi].Size >= pc)
								gi++;
							if (gi == groups.Count)
								gi = 0;
							while (gi < groups.Count && groups[gi].Size >= pc)
								gi++;
						}
					}
				}
			}

			public void GenerateResult(GenerateType type)
			{
				int max = 0;
				foreach (Group group in groups)
				{
					if (group.Size > max)
						max = group.Size;
				}

				int i = 0;
				bool[,] positions = new bool[groups.Count, max];
				for (int n = 0; n < Max; n++)
				{
					if (result[n] != null)
					{
						int gi = groups.IndexOf(result[n].Group);
						positions[gi, result[n].Position] = true;
						if (n == i)
							i++;
					}
				}

				if (type == GenerateType.Group)
				{
					for (int g = 0; g < groups.Count && i < Max; g++)
					{
						Group group = groups[g];
						for (int p = 0; p < group.Size && i < Max; p++)
						{
							if (!positions[g,p])
							{
								result[i] = new Team(group, p);
								i++;
								while (i < Max && result[i] != null)
									i++;
							}
						}
					}
				}
				else
				{
					for (int p = 0; p < max && i < Max; p++)
					{
						for (int g = 0; g < groups.Count && i < Max; g++)
						{
							Group group = groups[g];
							if (group.Size > p && !positions[g,p])
							{
								result[i] = new Team(group, p);
								i++;
								while (i < Max && result[i] != null)
									i++;
							}
						}
					}
				}

				OnChange();
			}


			/*
			PhaseItem string:
				groupName1|groupName2|groupName3...
group1			pos|pos|pos|pos...
group2			pos|pos|pos|pos...
group3			pos|pos|pos|pos...
...
result			g-pos|g-pos|g-pos...
				gameBoardId
		*/
			public void Read(System.IO.StringReader reader)
			{
				char[] split = new char[] {'|'};
				string line = reader.ReadLine();
				string[] gs = line.Split(split);
				string[] s;

				groups.Clear();

				for (int g = 0; g < gs.Length; g++)
				{
					Group group = new Group();
					group.Name = gs[g];
					line = reader.ReadLine();
					if (line.Length > 0)
					{
						s = line.Split(split);
						for (int p = 0; p < s.Length; p++)
						{
							group.Add(Int32.Parse(s[p]));
						}
					}
					groups.Add(group);
				}

				line = reader.ReadLine();
				s = line.Split(split);
				if (s.Length != Max)
					throw new Exception("Result don't match phase item size");

				string[] res;
				for (int t = 0; t < Max; t++)
				{
					if (s[t] == null || s[t].Length == 0)
					{
						result[t] = null;
					}
					else
					{
						res = s[t].Split(new char[] {'-'});
						int group = Int32.Parse(res[0]);
						int position = Int32.Parse(res[1]);
						result[t] = new Team(groups[group], position);
					}
				}

				line = reader.ReadLine();
				gameBoard = null;
				if (line.Length > 0)
				{
					// The game board is stored with all pattern data so
					// it has no foreign key to the GAME_BOARDS table
					// so if lookup failed it was probably deleted
					try
					{
						Sport.Data.Entity entity = Sport.Entities.GameBoard.Type.Lookup(Int32.Parse(line));
						if (entity != null)
							gameBoard = new Sport.Entities.GameBoard(entity);
					}
					catch (Exception e)
					{
						System.Diagnostics.Debug.WriteLine("Failed to read game board " + line + ": " + e.Message);
					}
				}
			}

			public void Write(System.IO.StringWriter writer)
			{
				string[] gs = new string[groups.Count];
				for (int g = 0; g < groups.Count; g++)
				{
					gs[g] = groups[g].Name;
				}
				writer.WriteLine(String.Join("|", gs));

				string[] s;

				for (int g = 0; g < groups.Count; g++)
				{
					s = new string[groups[g].Size];
					for (int p = 0; p < groups[g].Size; p++)
					{
						s[p] = groups[g][p].ToString();
					}
					writer.WriteLine(String.Join("|", s));
				}

				s = new string[Max];
				for (int t = 0; t < Max; t++)
				{
					if (result[t] != null)
					{
						int gi = groups.IndexOf(result[t].Group);
						s[t] = gi.ToString() + "-" + 
							result[t].Position.ToString();
					}
				}

				writer.WriteLine(String.Join("|", s));
				writer.WriteLine(gameBoard == null ? "" : gameBoard.Id.ToString());
			}
		}

		public class Phase : Sport.Common.GeneralCollection.CollectionItem,
			System.Collections.IEnumerable
		{
			private string _name;
			public string Name
			{
				get { return _name; }
				set 
				{ 
					_name = value; 
					_name.Replace("|", "");
					OnChange();
				}
			}

			internal void OnChange()
			{
				if (Owner != null)
					((PhasePattern) Owner).OnChange();
			}

			private RangeCollection _items;
			
			private Sport.Common.RangeArray ranges;
			public Sport.Common.RangeArray Ranges
			{
				get { return ranges; }
			}

			public Phase()
			{
				_items = new RangeCollection(this);
				ranges = new Sport.Common.RangeArray();
			}

			public void CalculateRanges()
			{
				ranges.Clear();
				foreach (PhaseItem item in _items)
				{
					ranges.Add(item.Min, item.Max);
				}
			}

			public PhaseItem this[int index]
			{
				get { return (PhaseItem) _items[index]; }
			}

			public PhaseItem GetPhaseItem(int teams)
			{
				return (PhaseItem) _items.GetRangeItem(teams);
			}

			public void Remove(PhaseItem value)
			{
				_items.Remove(value);
				CalculateRanges();
				OnChange();
			}

			public bool Contains(PhaseItem value)
			{
				return _items.Contains(value);
			}

			public int IndexOf(PhaseItem value)
			{
				return _items.IndexOf(value);
			}

			public int Add(PhaseItem value)
			{
				int i = _items.Add(value);
				ranges.Add(value.Min, value.Max);
				OnChange();
				return i;
			}

			public void Clear()
			{
				_items.Clear();
				ranges.Clear();
				OnChange();
			}

			public int Count
			{
				get { return _items.Count; }
			}

			public bool Fit(int min, int max)
			{
				return _items.Fit(min, max);
			}

			#region IEnumerable Members

			public System.Collections.IEnumerator GetEnumerator()
			{
				return _items.GetEnumerator();
			}

			#endregion

			/*
			Phase string:
				name|items
				min|max
				PhaseItem
				min|max
				PhaseItem
				...
		*/
			public void Read(System.IO.StringReader reader)
			{
				char[] split = new char[] {'|'};
				string line = reader.ReadLine();
				string[] s = line.Split(split);

				Clear();

				if (s.Length != 2)
					throw new Exception("Failed to read phase from reader");

				_name = s[0];
				int count = Int32.Parse(s[1]);

				for (int p = 0; p < count; p++)
				{
					line = reader.ReadLine();
					s = line.Split(split);
					if (s.Length != 2)
						throw new Exception("Failed to read phase item range");

					int min = Int32.Parse(s[0]);
					int max = Int32.Parse(s[1]);
					PhaseItem item = new PhaseItem(min, max);
					item.Read(reader);
					Add(item);
				}
			}

			public void Write(System.IO.StringWriter writer)
			{
				writer.WriteLine(_name + "|" + _items.Count);

				foreach (PhaseItem item in _items)
				{
					writer.WriteLine(item.Min.ToString() + "|" + item.Max.ToString());
					item.Write(writer);
				}
			}

			public override string ToString()
			{
				return _name;
			}
		}

		#region PhaseCollection

		public class PhaseCollection : Sport.Common.GeneralCollection
		{
			public PhaseCollection(PhasePattern pattern)
				: base(pattern)
			{
			}

			public Phase this[int index]
			{
				get { return (Phase) GetItem(index); }
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

		private PhaseCollection phases;
		public PhaseCollection Phases
		{
			get { return phases; }
			set { phases = value; }
		}

		private Sport.Common.RangeArray ranges;
		public Sport.Common.RangeArray Ranges
		{
			get { return ranges; }
		}

		public void CalculateRanges()
		{
			ranges.Clear();
			foreach (Phase phase in phases)
			{
				ranges.Union(phase.Ranges);
			}
		}

		public event EventHandler Changed;
		internal void OnChange()
		{
			CalculateRanges();
			if (Changed != null)
				Changed(this, EventArgs.Empty);
		}

		public PhasePattern()
		{
			phases = new PhaseCollection(this);
			phases.Changed += new Sport.Common.CollectionEventHandler(PhasesChanged);
			ranges = new Sport.Common.RangeArray();
		}

		public bool Load(int phasePatternId)
		{
			SportServices.ProducerService ps = new SportServices.ProducerService();
			ps.CookieContainer = Sport.Core.Session.Cookies;
			string data = ps.GetPhasePattern(phasePatternId);

			if (data == null)
				return false;

			System.IO.StringReader reader = new System.IO.StringReader(data);
			/*
				PhasePattern string:
					Phase
					Phase
					...
				*/
			phases.Clear();

			while (reader.Peek() != -1)
			{
				Phase phase = new Phase();
				phase.Read(reader);
				phases.Add(phase);
			}

			return true;
		}

		public bool Save(int phasePatternId)
		{
			System.IO.StringWriter writer = new System.IO.StringWriter();

			foreach (Phase phase in phases)
			{
				phase.Write(writer);
			}

			SportServices.ProducerService ps = new SportServices.ProducerService();
			ps.CookieContainer = Sport.Core.Session.Cookies;
			return ps.SetPhasePattern(phasePatternId, ranges.ToString(), writer.ToString());
		}

		private void PhasesChanged(object sender, Sport.Common.CollectionEventArgs e)
		{
			OnChange();
		}


		public bool BuildChampionship(Sport.Championships.MatchChampionship championship)
		{
			if (!ranges[championship.Teams.Count])
				return false;

			Sport.Championships.MatchTeam[] teamsList = new Sport.Championships.MatchTeam[championship.Teams.Count];

			for (int n = 0; n < championship.Teams.Count; n++)
			{
				teamsList[n] = new Sport.Championships.MatchTeam(championship.Teams[n]);
			}

			championship.Phases.Clear();

			foreach (Phase phase in phases)
			{
				PhaseItem phaseItem = phase.GetPhaseItem(teamsList.Length);

				if (phaseItem != null)
				{
					Sport.Championships.MatchPhase cp = new Sport.Championships.MatchPhase(phase.Name, Sport.Championships.Status.Planned);

					foreach (Group group in phaseItem.Groups)
					{
						Sport.Championships.MatchGroup cg = new Sport.Championships.MatchGroup(group.Name);

						for (int n = 0; n < group.Size; n++)
						{
							// Position in group is 1-based
							int team = group[n] - 1;
							if (team < teamsList.Length)
							{
								if (teamsList[team] == null)
									return false;
								cg.Teams.Add(teamsList[team]);
							}
						}

						if (phaseItem.GameBoard != null)
						{
							GameBoard gb = new GameBoard();
							gb.Load(phaseItem.GameBoard.Id);
							gb.CreateMatches(cg);
						}

						cp.Groups.Add(cg);
					}

					for (int n = 0; n < teamsList.Length; n++)
					{
						if (phaseItem[n] == null)
						{
							teamsList[n] = null;
						}
						else
						{
							int gi = phaseItem.Groups.IndexOf(phaseItem[n].Group);
							teamsList[n] = new Sport.Championships.MatchTeam(gi, phaseItem[n].Position);
						}
					}

					championship.Phases.Add(cp);
				}
			}


			// Resetting matches numbers - ordering them first by round
			// then by group

			int number = 1;
			foreach (Sport.Championships.Phase phase in championship.Phases)
			{
				int round = 0;
				bool roundFound = true;
				while (roundFound)
				{
					roundFound = false;
					foreach (Sport.Championships.MatchGroup group in phase.Groups)
					{
						if (round < group.Rounds.Count)
						{
							roundFound = true;

							foreach (Sport.Championships.Cycle cycle in group.Rounds[round].Cycles)
							{
								foreach (Sport.Championships.Match match in cycle.Matches)
								{
									match.Number = number;
									number++;
								}
							}
						}
					}
					round++;
				}
			}

			return true;
		}
	}
}
