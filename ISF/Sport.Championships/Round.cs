using System;

namespace Sport.Championships
{
	/// <summary>
	/// The Round class divide a single group's match list into several
	/// parts. Each round defines a name for the round and
	/// contains cycles collection.
	/// </summary>
	public class Round : Sport.Common.GeneralCollection.CollectionItem
	{
		#region CycleCollection

		public class CycleCollection : Sport.Common.GeneralCollection
		{
			public CycleCollection(Round round)
				: base(round)
			{
			}

			internal void ClearNumbers(Cycle cycle)
			{
				if (((Round) Owner).Group != null)
				{
					foreach (Tournament tournament in cycle.Tournaments)
					{
						((Round) Owner).Group.SetTournamentNumber(tournament, tournament.Number, -1);
					}

					foreach (Match match in cycle.Matches)
					{
						((Round) Owner).Group.SetMatchNumber(match, match.Number, -1);
					}
				}
			}

			internal void SetNumbers(Cycle cycle)
			{
				if (((Round) Owner).Group != null)
				{
					foreach (Tournament tournament in cycle.Tournaments)
					{
						((Round) Owner).Group.SetTournamentNumber(tournament, -1, tournament.Number);
					}

					foreach (Match match in cycle.Matches)
					{
						((Round) Owner).Group.SetMatchNumber(match, -1, match.Number);
					}
				}
			}

			protected override void SetItem(int index, object value)
			{
				if (!((Round) Owner).Editable)
					throw new ChampionshipException("Not in group edit - cannot change cycles");

				ClearNumbers(this[index]);
				base.SetItem (index, value);
				SetNumbers((Cycle) value);
			}

			protected override void InsertItem(int index, object value)
			{
				if (!((Round) Owner).Editable)
					throw new ChampionshipException("Not in group edit - cannot change cycles");

				base.InsertItem (index, value);
				SetNumbers((Cycle) value);
			}

			protected override void RemoveItem(int index)
			{
				if (!((Round) Owner).Editable)
					throw new ChampionshipException("Not in group edit - cannot change cycles");

				ClearNumbers(this[index]);
				base.RemoveItem (index);
			}

			public Cycle this[int index]
			{
				get { return (Cycle) GetItem(index); }
				set { SetItem(index, value); }
			}

			public void Insert(int index, Cycle value)
			{
				InsertItem(index, value);
			}

			public void Remove(Cycle value)
			{
				RemoveItem(value);
			}

			public bool Contains(Cycle value)
			{
				return base.Contains(value);
			}

			public int IndexOf(Cycle value)
			{
				return base.IndexOf(value);
			}

			public int Add(Cycle value)
			{
				return AddItem(value);
			}
		}

		#endregion

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

		public bool Editable
		{
			get
			{
				return Group == null || Group.Editable;
			}
		}

		internal int _index;
		public int Index
		{
			get { return _index; }
		}

		public bool HasResults()
		{
			foreach (Cycle cycle in _cycles)
			{
				if (cycle.HasResults())
					return true;
			}

			return false;
		}

		public void Reset()
		{
			foreach (Cycle cycle in _cycles)
			{
				cycle.Reset();
			}
		}

		public override void OnOwnerChange(object oo, object no)
		{
			if (no == null)
				_index = -1;
		}


		private CycleCollection _cycles;
		public CycleCollection Cycles
		{
			get { return _cycles; }
		}

		public MatchGroup Group
		{
			get { return ((MatchGroup) Owner); }
		}

		public Round(string name)
		{
			_index = -1;
			_name = name;

			_cycles = new CycleCollection(this);
			_cycles.Changed += new Sport.Common.CollectionEventHandler(CyclesChanged);
		}

		internal Round(SportServices.Round round)
			: this(round.Name)
		{
			foreach (SportServices.Cycle scycle in round.Cycles)
			{
				_cycles.Add(new Cycle(scycle));
			}
		}

		internal SportServices.Round Save()
		{
			SportServices.Round sround = new SportServices.Round();
			sround.Name = _name;
			sround.Cycles = new SportServices.Cycle[_cycles.Count];
			for (int n = 0; n < _cycles.Count; n++)
			{
				sround.Cycles[n] = _cycles[n].Save();
			}

			return sround;
		}

		public override string ToString()
		{
			return Group.Name + " - " + _name;
		}

		private void CyclesChanged(object sender, Sport.Common.CollectionEventArgs e)
		{
			for (int i = e.Index; i < _cycles.Count; i++)
				_cycles[i]._index = i;
		}
		
		public void ArrangeMatchesByDate()
		{
			//get all matches:
			System.Collections.ArrayList matches=new System.Collections.ArrayList();
			foreach (Cycle cycle in this.Cycles)
			{
				foreach (Match match in cycle.Matches)
					matches.Add(match);
			}
			
			//sort by date:
			matches.Sort(new MatchDateTimeComparer());
			
			while (this.Cycles.Count > 0)
				this.Cycles.RemoveAt(0);
			
			DateTime dtLastDate=new DateTime(1920, 1, 1, 0, 0, 0);
			System.Collections.ArrayList arrCurMatches=new System.Collections.ArrayList();
			int curCycleIndex=0;
			int curMatchIndex=0;
			Cycle curCycle=new Cycle("מחזור "+(curCycleIndex+1));
			curCycle._index = curCycleIndex;
			foreach (Match match in matches)
			{
				DateTime dtCurDate=match.Time;
				if (!Sport.Common.Tools.IsSameDate(dtLastDate, dtCurDate))
				{
					if (arrCurMatches.Count > 0)
					{
						foreach (Match curMatch in arrCurMatches)
							curCycle.Matches.Add(curMatch);
						this.Cycles.Add(curCycle);
						curCycleIndex++;
						curCycle = new Cycle("מחזור "+(curCycleIndex+1));
						curCycle._index = curCycleIndex;
					}
					curMatchIndex = 0;
					arrCurMatches.Clear();
				}
				match._index = curMatchIndex;
				match.Tournament = -1;
				arrCurMatches.Add(match);
				curMatchIndex++;
				dtLastDate = dtCurDate;
			}
		}
		
		private class MatchDateTimeComparer : System.Collections.IComparer
		{
			public int Compare(object x, object y)
			{
				Sport.Championships.Match m1=(Sport.Championships.Match) x;
				Sport.Championships.Match m2=(Sport.Championships.Match) y;
				return m1.Time.CompareTo(m2.Time);
			}
		}
	}
}
