using System;
using System.Linq;
using Sport.Rulesets;
using System.Collections.Generic;

namespace Sport.Championships
{
	public class Competitor : Sport.Common.GeneralCollection.CollectionItem
	{
		#region Properties

		private int _playerNumber;
		public int PlayerNumber
		{
			get { return _playerNumber; }
		}

		internal int[] _sharedResultNumbers = null;
		public int[] SharedResultNumbers
		{
			get { return _sharedResultNumbers; }
		}

		internal int _lastResultDisqualifications = 0;
		public int LastResultDisqualifications
		{
			get { return _lastResultDisqualifications; }
		}

		internal int _totalDisqualifications = 0;
		public int TotalDisqualifications
		{
			get { return _totalDisqualifications; }
		}

		internal string _wind = "";
		public string Wind
		{
			get { return _wind; }
		}

		internal int _team;
		public int Team
		{
			get { return _team; }
			set { _team = value; }
		}

		private string _sharedResultNames = null;

		private void RecalculateSharedResultNames()
		{
			_sharedResultNames = "";
			if (this.SharedResultNumbers != null && this.SharedResultNumbers.Length > 1)
			{
				if (this.GroupTeam != null && this.GroupTeam.TeamEntity != null)
				{
					_sharedResultNames = string.Join(", ", this.SharedResultNumbers.ToList().ConvertAll(curNumber =>
					{
						Sport.Entities.Player curPlayer = this.GroupTeam.TeamEntity.GetPlayerByNumber(curNumber);
						return (curPlayer == null) ? "" : curPlayer.Name.Replace(",", "");
					}).FindAll(s => s.Length > 0));
				}
			}
		}

		public string GetSharedResultNames()
		{
			if (_sharedResultNames == null)
				RecalculateSharedResultNames();
			return _sharedResultNames;
		}

		public bool Editable
		{
			get
			{
				return Competition == null || Competition.Editable;
			}
		}

		public CompetitionTeam GroupTeam
		{
			get
			{
				if (Competition != null)
				{
					CompetitionGroup group = Competition.Group;
					if (group != null && _team != -1)
					{
						return group.Teams[_team];
					}
				}

				return null;
			}
		}

		internal int _heat;
		public int Heat
		{
			get { return _heat; }
			set 
			{ 
				if (!Editable)
					throw new ChampionshipException("Not in edit - cannot be set");

				_heat = value; 
			}
		}

		internal int _position;
		public int Position
		{
			get { return _position; }
			set 
			{ 
				if (!Editable)
					throw new ChampionshipException("Not in edit - cannot be set");

				_position = value; 
			}
		}

		internal int _resultPosition;
		public int ResultPosition
		{
			get
			{
				if (_resultPosition >= 0)
					return _resultPosition;
				return CustomPosition;
			}
		}

		internal int _result;
		public int Result
		{
			get { return _result; }
			set { _result = value; }
		}

		internal int _score;
		public int Score
		{
			get {return _score; }
			set { _score = value; }
		}
		
		/*
		private int _customScore=0;
		public int CustomScore
		{
			get { return _customScore; }
			set { _customScore = value; }
		}
		*/
		
		internal int _customPosition;
		public int CustomPosition
		{
			get {return _customPosition;}
		}
		
        internal int _index;
		public int Index
		{
			get { return _index; }
		}

		public CompetitionPlayer Player
		{
			get 
			{ 
				if (_playerNumber < 0)
					return null;
				Sport.Championships.Competition competition=this.Competition;
				if (competition != null)
				{
					CompetitionGroup group=competition.Group;
					if (group != null)
					{
						if (group.Players != null)
							return group.Players[_playerNumber];
					}
				}
				return null;
			}
		}


		public string Name
		{
			get 
			{ 
				CompetitionPlayer player = Player;
				if (player != null)
					return player.Name;

				return "מספר " + _playerNumber.ToString(); 
			}
		}

		#endregion

		#region CollectionItem Members

		public override void OnOwnerChange(object oo, object no)
		{
			if (no == null)
			{
				_index = -1;
				_team = -1;
			}
			else
			{
				Competition competition = no as Competition;
				if (competition.Group != null)
				{
					_team = competition.Group.GetPlayerTeam(_playerNumber);
				}
			}
		}

		public Competition Competition
		{
			get { return ((Competition) Owner); }
		}

		#endregion

		#region Constructors

		public Competitor(int playerNumber)
		{
			_index = -1;
			_team = -1;
			_playerNumber = playerNumber;
			_heat = -1;
			_position = -1;
			_resultPosition = -1;
			_result = -1;
			_score = -1;
			_customPosition = -1;
		}

		internal Competitor(SportServices.Competitor scompetitor)
		{
			_index = -1;
			_playerNumber = scompetitor.PlayerNumber;
			_heat = scompetitor.Heat;
			_position = scompetitor.Position;
			_resultPosition = scompetitor.ResultPosition;
			_result = scompetitor.Result;
			_score = scompetitor.Score;
			_customPosition = scompetitor.CustomPosition;
			_sharedResultNumbers = scompetitor.SharedResultNumbers;
			_lastResultDisqualifications = scompetitor.LastResultDisqualifications;
			_totalDisqualifications = scompetitor.TotalDisqualifications;
			_wind = scompetitor.Wind;
		}

		#endregion

		public void Reset()
		{
			_resultPosition = -1;
			_result = -1;
			_score = -1;
			_customPosition = -1;
		}

		private void OnScoreChange()
		{
			if (Competition != null)
			{
				Competition.CalculateCompetitorsPosition();
				Competition.Group.CalculateTeamsScore();
			}
		}

		internal SportServices.Competitor Save()
		{
			SportServices.Competitor scompetitor = new SportServices.Competitor();
			scompetitor.PlayerNumber = _playerNumber;
			scompetitor.Heat = _heat;
			scompetitor.Position = _position;
			scompetitor.ResultPosition = _resultPosition;
			scompetitor.Result = _result;
			scompetitor.Score = _score;
			scompetitor.CustomPosition = _customPosition;
			scompetitor.SharedResultNumbers = _sharedResultNumbers;
			scompetitor.LastResultDisqualifications = _lastResultDisqualifications;
			scompetitor.TotalDisqualifications = _totalDisqualifications;
			scompetitor.Wind = _wind;
			
			return scompetitor;
		}
		
		/*
		public bool SetResult(int result)
		{
			// Storing old values
			int pr = _result;
			int ps = _score;

			_result = result;
			_score = Competition.ScoreTable == null ? 0 : Competition.ScoreTable.GetScore(result);
			OnScoreChange();
			
			CompetitionGroup group = Competition.Group;

			SportServices.CompetitorResult sresult = new SportServices.CompetitorResult();
			sresult.ChampionshipCategoryId = group.Phase.Championship.ChampionshipCategory.Id;
			sresult.Phase = group.Phase.Index;
			sresult.Group = group.Index;
			sresult.Competition = Competition.Index;
			sresult.Competitor = _index;
			sresult.Result = _result;
			sresult.Score = _score;
			sresult.CompetitorsPosition = Competition.GetCompetitorsPosition();
			sresult.TeamsResult = group.GetTeamResult();

			SportServices.ChampionshipService cs = new SportServices.ChampionshipService();
			cs.CookieContainer = Sport.Core.Session.Cookies;
			if (!cs.SetCompetitorResult(sresult))
			{
				_result = pr;
				_score = ps;
				OnScoreChange();
				return false;
			}

			return true;
		}
		*/
		
		public bool SetCustomPosition(int position)
		{
			// Storing old values
			int cp = _customPosition;
			int rp = _resultPosition;
			
			if (Sport.Core.Session.Connected)
			{
				CompetitionGroup group = Competition.Group;
			
				int categoryID = group.Phase.Championship.ChampionshipCategory.Id;
				int phase = group.Phase.Index;
				int groupIndex = group.Index;
				int competition = Competition.Index;
				int heat = _heat;
				int competitor = _index;
			
				SportServices.ChampionshipService cs = new SportServices.ChampionshipService();
				cs.CookieContainer = Sport.Core.Session.Cookies;
				bool blnSuccess = false;
				try
				{
					blnSuccess = cs.SetCompetitorCustomPosition(categoryID, phase, groupIndex, 
						competition, competitor, heat, position);
				}
				catch (Exception ex)
				{
					Sport.Data.AdvancedTools.ReportExcpetion(ex);
					blnSuccess = false;
				}
				if (!blnSuccess)
				{
					_customPosition = cp;
					return false;
				}
			
				this._customPosition = position;
				this._resultPosition = -1;
			
				return true;
			}
			else
			{
				this._customPosition = position;
				this._resultPosition = -1;
				Sport.UI.Dialogs.WaitForm.ShowWait("שומר אליפות אנא המתן...", true);
				this.Competition.Group.Phase.Championship.Edit();
				string strError = this.Competition.Group.Phase.Championship.Save(true);
				Sport.UI.Dialogs.WaitForm.HideWait();
				if (strError.Length > 0)
				{
					Sport.UI.MessageBox.Error("שגיאה בעת קביעת דירוג ידני:\n"+strError, "בניית אליפות");
					_customPosition = cp;
					_resultPosition = rp;
				}
				return true;
			}
		}
		
		public override string ToString()
		{
			return Name;
		}
	}
}
