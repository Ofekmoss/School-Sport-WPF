using System;

namespace Sport.Championships
{
	public class Heat : Sport.Common.GeneralCollection.CollectionItem
	{
		#region Properties

		internal int _index;
		public int Index
		{
			get { return _index; }
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

		#endregion

		#region CollectionItem Members

		public override void OnOwnerChange(object oo, object no)
		{
			if (no == null)
				_index = -1;
		}

		public Competition Competition
		{
			get { return ((Competition) Owner); }
		}

		#endregion

		#region Constructors

		public Heat()
		{
			_index = -1;
			_time = DateTime.MinValue;
			_court = null;
			_facility = null;
		}

		internal Heat(SportServices.Heat sheat)
		{
			_index = -1;
			_time = sheat.Time;

			if (sheat.Facility == -1)
			{
				_facility = null;
			}
			else
			{
				_facility = new Sport.Entities.Facility(sheat.Facility);
				if (!_facility.IsValid())
					_facility = null;
			}
            
			if (sheat.Court == -1)
			{
				_court = null;
			}
			else
			{
				_court = new Sport.Entities.Court(sheat.Court);
				if (!_court.IsValid())
					_court = null;
			}
		}

		#endregion

		internal SportServices.Heat Save()
		{
			SportServices.Heat sheat = new SportServices.Heat();
			sheat.Time = _time;
			sheat.Facility = _facility == null ? -1 : _facility.Id;
			sheat.Court = _court == null ? -1 : _court.Id;

			return sheat;
		}
		
		public bool Set(DateTime time, Sport.Entities.Facility facility, 
			Sport.Entities.Court court)
		{
			CompetitionPhase phase=this.Competition.Group.Phase;
			CompetitionChampionship champ=phase.Championship;
			int ccid=champ.ChampionshipCategory.Id;
			int facilityID=(facility == null)?-1:facility.Id;
			int courtID=(court == null)?-1:court.Id;
			SportServices.ChampionshipService cs = new SportServices.ChampionshipService();
			cs.CookieContainer = Sport.Core.Session.Cookies;
			if (cs.SaveHeat(ccid, phase.Index, this.Competition.Group.Index, 
				Competition.Index, this._index, time, facilityID, courtID))
			{
				_time = time;
				_facility = facility;
				_court = court;
				return true;
			}
			return false;
		}

		public override string ToString()
		{
			return Competition.ToString() + " - מקצה " + (_index + 1).ToString();
		}
	}
}
