using System;
using System.Collections;

namespace Sport.Championships
{
	/// <summary>
	/// The Cycle class divide a single group's match list into several
	/// parts. Each cycle defines a name for the cycle and
	/// contains an match collection.
	/// </summary>
	public class Cycle : Sport.Common.GeneralCollection.CollectionItem
	{
		#region MatchCollection

		public class MatchCollection : Sport.Common.GeneralCollection
		{
			public MatchCollection(Cycle cycle)
				: base(cycle)
			{
			}

			protected override void SetItem(int index, object value)
			{
				if (!((Cycle) Owner).Editable)
					throw new ChampionshipException("Not in group edit - cannot change matches");

				Round round = ((Cycle) Owner).Round;
				if (round != null && round.Group != null)
				{
					round.Group.SetMatchNumber(this[index], this[index].Number, -1);
                    round.Group.SetMatchNumber((Match) value, -1, ((Match) value).Number);
				}

				base.SetItem (index, value);
			}

			protected override void InsertItem(int index, object value)
			{
				if (!((Cycle) Owner).Editable)
					throw new ChampionshipException("Not in group edit - cannot change matches");

				Round round = ((Cycle) Owner).Round;
				if (round != null && round.Group != null)
				{
					round.Group.SetMatchNumber((Match) value, -1, ((Match) value).Number);
				}

				base.InsertItem (index, value);
			}

			protected override void RemoveItem(int index)
			{
				if (!((Cycle) Owner).Editable)
					throw new ChampionshipException("Not in group edit - cannot change matches");

				Round round = ((Cycle) Owner).Round;
				if (round != null && round.Group != null)
				{
					round.Group.SetMatchNumber(this[index], this[index].Number, -1);
				}

				base.RemoveItem (index);
			}

			public Match this[int index]
			{
				get { return (Match) GetItem(index); }
				set { SetItem(index, value); }
			}

			public void Insert(int index, Match value)
			{
				InsertItem(index, value);
			}

			public void Remove(Match value)
			{
				RemoveItem(value);
			}

			public bool Contains(Match value)
			{
				return base.Contains(value);
			}

			public int IndexOf(Match value)
			{
				return base.IndexOf(value);
			}

			public int Add(Match value)
			{
				return AddItem(value);
			}
		}

		#endregion

		#region TournamentCollection

		public class TournamentCollection : Sport.Common.GeneralCollection
		{
			public TournamentCollection(Cycle cycle)
				: base(cycle)
			{
			}

			protected override void SetItem(int index, object value)
			{
				if (!((Cycle) Owner).Editable)
					throw new ChampionshipException("Not in group edit - cannot change tournaments");

				Round round = ((Cycle) Owner).Round;
				if (round != null && round.Group != null)
				{
					round.Group.SetTournamentNumber(this[index], this[index].Number, -1);
					round.Group.SetTournamentNumber((Tournament) value, -1, ((Tournament) value).Number);
				}

				base.SetItem (index, value);
			}

			protected override void InsertItem(int index, object value)
			{
				if (!((Cycle) Owner).Editable)
					throw new ChampionshipException("Not in group edit - cannot change tournaments");

				Round round = ((Cycle) Owner).Round;
				if (round != null && round.Group != null)
				{
					round.Group.SetTournamentNumber((Tournament) value, -1, ((Tournament) value).Number);
				}

				base.InsertItem (index, value);
			}

			protected override void RemoveItem(int index)
			{
				if (!((Cycle) Owner).Editable)
					throw new ChampionshipException("Not in group edit - cannot change tournaments");

				Round round = ((Cycle) Owner).Round;
				if (round != null && round.Group != null)
				{
					round.Group.SetTournamentNumber(this[index], this[index].Number, -1);
				}

				base.RemoveItem (index);
			}

			public Tournament this[int index]
			{
				get { return (Tournament) GetItem(index); }
				set { SetItem(index, value); }
			}

			public void Insert(int index, Tournament value)
			{
				InsertItem(index, value);
			}

			public void Remove(Tournament value)
			{
				RemoveItem(value);
			}

			public bool Contains(Tournament value)
			{
				return base.Contains(value);
			}

			public int IndexOf(Tournament value)
			{
				return base.IndexOf(value);
			}

			public int Add(Tournament value)
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
				return Round == null || Round.Editable;
			}
		}

		internal int _index;
		public int Index
		{
			get { return _index; }
		}

		public bool HasResults()
		{
			foreach (Match match in _matches)
			{
				if (match.Outcome != MatchOutcome.None)
					return true;
			}

			return false;
		}

		public void Reset()
		{
			foreach (Match match in _matches)
			{
				match.Reset();
			}
		}

		public override void OnOwnerChange(object oo, object no)
		{
			if (no == null)
				_index = -1;
		}


		private MatchCollection _matches;
		public MatchCollection Matches
		{
			get { return _matches; }
		}

		private TournamentCollection _tournaments;
		public TournamentCollection Tournaments
		{
			get { return _tournaments; }
		}

		public Round Round
		{
			get { return ((Round) Owner); }
		}

		public Cycle(string name)
		{
			_index = -1;
			_name = name;

			_matches = new MatchCollection(this);
			_matches.Changed += new Sport.Common.CollectionEventHandler(MatchesChanged);

			_tournaments = new TournamentCollection(this);
			_tournaments.Changed += new Sport.Common.CollectionEventHandler(TournamentsChanged);
		}

		internal Cycle(SportServices.Cycle cycle)
			: this(cycle.Name)
		{
			foreach (SportServices.Tournament stournament in cycle.Tournaments)
			{
				_tournaments.Add(new Tournament(stournament));
			}

			foreach (SportServices.Match smatch in cycle.Matches)
			{
				_matches.Add(new Match(smatch));
			}
		}

		internal SportServices.Cycle Save()
		{
			SportServices.Cycle scycle = new SportServices.Cycle();
			scycle.Name = _name;

			scycle.Tournaments = new SportServices.Tournament[_tournaments.Count];
			for (int n = 0; n < _tournaments.Count; n++)
			{
				scycle.Tournaments[n] = _tournaments[n].Save();
			}

			scycle.Matches = new SportServices.Match[_matches.Count];
			for (int n = 0; n < _matches.Count; n++)
			{
				scycle.Matches[n] = _matches[n].Save();
			}

			return scycle;
		}

		public override string ToString()
		{
			return Round.Name + " - " + _name;
		}

		public Cycle GetNextCycle()
		{
			if (_index == Round.Cycles.Count - 1)
			{
				Sport.Championships.MatchGroup group = Round.Group;

				for (int n = Round.Index + 1; n < group.Rounds.Count; n++)
				{
					Sport.Championships.Round round = group.Rounds[n];
					if (round.Cycles.Count > 0)
						return round.Cycles[0];
				}

				return null;
			}
			
			return Round.Cycles[_index + 1];
		}

		public Cycle GetPrevCycle()
		{
			if (_index == 0)
			{
				Sport.Championships.MatchGroup group = Round.Group;

				for (int n = Round.Index - 1; n >= 0; n--)
				{
					Sport.Championships.Round round = group.Rounds[n];
					if (round.Cycles.Count > 0)
						return round.Cycles[round.Cycles.Count - 1];
				}

				return null;
			}
			
			return Round.Cycles[_index - 1];
		}

		private void MatchesChanged(object sender, Sport.Common.CollectionEventArgs e)
		{
			for (int i = e.Index; i < _matches.Count; i++)
				_matches[i]._index = i;

			if (e.EventType == Sport.Common.CollectionEventType.Insert)
			{
				if (((Match) e.New).Tournament >= _tournaments.Count)
					((Match) e.New).Tournament = -1;
			}
		}

		private void TournamentsChanged(object sender, Sport.Common.CollectionEventArgs e)
		{
			for (int i = e.Index; i < _tournaments.Count; i++)
				_tournaments[i]._index = i;

			if (e.EventType == Sport.Common.CollectionEventType.Remove)
			{
				foreach (Match match in _matches)
				{
					if (match.Tournament == e.Index)
						match.Tournament = -1;
					else if (match.Tournament > e.Index)
						match.Tournament--;
				}
			}
		}
		
		public ArrayList GetTournamentMatches()
		{
			ArrayList result=new ArrayList();
			Hashtable tblMatches=new Hashtable();
			foreach (Match match in this.Matches)
			{
				int curTournament=match.Tournament;
				if (tblMatches[curTournament] == null)
					tblMatches[curTournament] = new ArrayList();
				ArrayList arrMatches=(ArrayList) tblMatches[curTournament];
				arrMatches.Add(match);
				tblMatches[curTournament] = arrMatches;
			}
			foreach (object key in tblMatches.Keys)
			{
				ArrayList arrMatches=(ArrayList) tblMatches[key];
				result.Add((Match[]) arrMatches.ToArray(typeof(Match)));
			}
			return result;
		}
	}
}
