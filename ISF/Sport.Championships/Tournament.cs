using System;

namespace Sport.Championships
{
	/// <summary>
	/// The Tournament class divide a single cycle's match list into several
	/// parts. Each tournament defines a number for the tournament and
	/// time, facility and court for the tournament.
	/// </summary>
	public class Tournament : Sport.Common.GeneralCollection.CollectionItem
	{
		private int _number;
		public int Number
		{
			get { return _number; }
			set 
			{ 
				if (!Editable)
					throw new ChampionshipException("Not in edit - cannot be set");

				if (Cycle != null)
					Cycle.Round.Group.SetTournamentNumber(this, _number, value);

				_number = value; 
			}
		}

		public bool Editable
		{
			get
			{
				return Cycle == null || Cycle.Editable;
			}
		}

		internal int _index;
		public int Index
		{
			get { return _index; }
		}

		private DateTime _time;
		public DateTime Time
		{
			get { return _time; }
			set
			{
				if (!Editable)
					throw new ChampionshipException("Not in edit - cannot be set");

				_time = value; 
			}
		}
		
		private Sport.Entities.Facility _facility;
		public Sport.Entities.Facility Facility
		{
			get { return _facility; }
			set
			{
				if (!Editable)
					throw new ChampionshipException("Not in edit - cannot be set");

				_facility = value; 
			}
		}
		
		private Sport.Entities.Court _court;
		public Sport.Entities.Court Court
		{
			get { return _court; }
			set
			{
				if (!Editable)
					throw new ChampionshipException("Not in edit - cannot be set");

				_court = value; 
			}
		}

		public override void OnOwnerChange(object oo, object no)
		{
			if (no == null)
				_index = -1;
		}

		public Cycle Cycle
		{
			get { return ((Cycle) Owner); }
		}

		#region Constructors

		public Tournament(int number)
		{
			_index = -1;
			_number = number;
			_time = new DateTime(0);
			_facility = null;
			_court = null;
		}

		internal Tournament(SportServices.Tournament stournament)
			: this(stournament.Number)
		{
			_time = stournament.Time;

			if (stournament.Facility == -1)
			{
				_facility = null;
			}
			else
			{
				_facility = new Sport.Entities.Facility(stournament.Facility);
				if (!_facility.IsValid())
					_facility = null;
			}

			if (stournament.Court == -1)
			{
				_court = null;
			}
			else
			{
				_court = new Sport.Entities.Court(stournament.Court);
				if (!_court.IsValid())
					_court = null;
			}
		}

		#endregion

        internal SportServices.Tournament Save()
		{
			SportServices.Tournament stournament = new SportServices.Tournament();
			stournament.Number = _number;
			stournament.Time = _time;
			stournament.Facility = _facility == null ? -1 : _facility.Id;
			stournament.Court = _court == null ? -1 : _court.Id;

			return stournament;
		}

		public override string ToString()
		{
			return Cycle.Name + " - טורניר " + _number.ToString();
		}

		public int GetMatchCount()
		{
			int count = 0;
			foreach (Match match in Cycle.Matches)
			{
				if (match.Tournament == _index)
					count++;
			}

			return count;
		}

		public bool Set(int number, DateTime time, Sport.Entities.Facility facility, 
			Sport.Entities.Court court)
		{
			Round round = Cycle.Round;
			Group group = round.Group;
			Phase phase = group.Phase;
			SportServices.ChampionshipService cs = new SportServices.ChampionshipService();
			cs.CookieContainer = Sport.Core.Session.Cookies;
			if (cs.SaveTournament(phase.Championship.ChampionshipCategory.Id,
				phase.Index, group.Index, round.Index, Cycle.Index, _index,
				number, time, 
				facility == null ? -1 : facility.Id, 
				court == null ? -1 : court.Id))
			{
				_time = time;
				Cycle.Round.Group.SetTournamentNumber(this, _number, number);
				_number = number;
				_facility = facility;
				_court = court;
				return true;
			}
			return false;
		}
	}
}
