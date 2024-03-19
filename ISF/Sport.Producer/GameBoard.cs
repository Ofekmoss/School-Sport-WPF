using System;

namespace Sport.Producer
{
	public class GameBoardItem : RangeItem
	{
		#region Game Class

		public class Game : ICloneable
		{
			private int _teamA;
			public int TeamA
			{
				get { return _teamA; }
				set 
				{ 
					if (value < 0)
						throw new ArgumentOutOfRangeException("value", "Team must be 0 or more");

					_teamA = value; 
				}
			}

			private int _teamB;
			public int TeamB
			{
				get { return _teamB; }
				set 
				{ 
					if (value < 0)
						throw new ArgumentOutOfRangeException("value", "Team must be 0 or more");

					_teamB = value; 
				}
			}

			public Game(int teamA, int teamB)
			{
				TeamA = teamA;
				TeamB = teamB;
			}

			public Game()
			{
				_teamA = 0;
				_teamB = 0;
			}

			public override string ToString()
			{
				string ta = _teamA == 0 ? null : _teamA.ToString();
				string tb = _teamB == 0 ? null : _teamB.ToString();
				return ta + "-" + tb;
			}

			public static Game Parse(string s)
			{
				string[] teams = s.Split(new char[] {'-'});
				if (teams.Length == 2)
				{
					Game game = new Game();
					if (teams[0].Length > 0)
					{
						try
						{
							game.TeamA = Int32.Parse(teams[0]);
						}
						catch
						{
						}
					}

					if (teams[1].Length > 0)
					{
						try
						{
							game.TeamB = Int32.Parse(teams[1]);
						}
						catch
						{
						}
					}

					return game;
				}

				return null;
			}

			#region ICloneable Members

			public object Clone()
			{
				return new Game(_teamA, _teamB);
			}

			#endregion
		}

		#endregion

		#region Match Class

		public class Match : ICloneable
		{
			private int[] _teams;
			public int[] Teams
			{
				get { return _teams; }
				set { _teams = value; }
			}

			public Match(int[] teams)
			{
				_teams = teams;
			}

			public Match()
			{
				_teams = null;
			}

			public override string ToString()
			{
				if (_teams == null || _teams.Length == 0)
					return null;

				// Making the teams string reverse
				// so the first team will be on the right - "3-2-1"
				// because numbers are displayed from right to left
				string[] st = new string[_teams.Length];
				for (int n = 0; n < _teams.Length; n++)
					st[_teams.Length - n - 1] = _teams[n].ToString();
				return String.Join("-", st);
			}

			public static Match Parse(string s)
			{
				if (s == null || s.Length == 0)
					return null;

				// Making the teams string reverse
				// so the first team will be on the right - "3-2-1"
				// because numbers are displayed from right to left
				string[] st = s.Split(new char[] {'-'});
				int[] teams = new int[st.Length];
				for (int n = 0; n < st.Length; n++)
				{
					try
					{
						teams[n] = Int32.Parse(st[st.Length - n - 1]);
					}
					catch
					{
						return null;
					}
				}

				return new Match(teams);
			}

			#region ICloneable Members

			public object Clone()
			{
				return new Match((int[]) _teams.Clone());
			}

			#endregion
		}

		#endregion

		private void OnChange()
		{
			GameBoard.OnChange();
		}

		public GameBoard GameBoard
		{
			get { return (GameBoard) Owner; }
		}

		private void ResetTeams(Match match, int max)
		{
			int count = 0;
			for (int n = 0; n < match.Teams.Length; n++)
			{
				if (match.Teams[n] > max)
				{
					count++;
					match.Teams[n] = 0;
				}
			}

			if (count > 0)
			{
				if (count == match.Teams.Length)
				{
					match.Teams = null;
				}
				else
				{
					int[] teams = new int[match.Teams.Length - count];
					int i = 0;
					for (int n = 0; n < match.Teams.Length; n++)
					{
						if (match.Teams[n] != 0)
						{
							teams[i] = match.Teams[n];
							i++;
						}
					}

					match.Teams = teams;
				}
			}
		}

		protected override void RangeChanging(int min, int max)
		{
			if (Max > max)
			{
				for (int r = 0; r < _rounds; r++)
				{
					for (int c = 0; c < _cycles; c++)
					{
						for (int m = 0; m < _matchCount; m++)
						{
							Match match = GetMatch(r, c, m);

							if (match != null)
								ResetTeams(match, max);
						}
					}
				}
			}

			base.RangeChanging (min, max);

			GameBoard.CalculateRanges();

			OnChange();
		}

		public bool CheckGame(Game game)
		{
			return game.TeamA <= _matchTeams && game.TeamB <= _matchTeams;
		}

		public bool CheckMatch(Match match)
		{
			for (int n = 0; n < match.Teams.Length; n++)
				if (match.Teams[n] > Max)
					return false;
			return true;
		}

		private string[] _roundNames;
		public string GetRoundName(int round)
		{
			return _roundNames[round];
		}
		public void SetRoundName(int round, string name)
		{
			name.Replace("|", "");
			_roundNames[round] = name;
			OnChange();
		}

		private int _rounds;
		public int Rounds
		{
			get { return _rounds; }
			set 
			{
				if (value < 1)
					throw new ArgumentOutOfRangeException("value", "Rounds must be more than 0");

				if (value > _matches.GetLength(0))
				{
					Match[,,] matches = new Match[value, _cycles, _matchCount];
					for (int r = 0; r < _rounds; r++)
						for (int c = 0; c < _cycles; c++)
							for (int m = 0; m < _matchCount; m++)
								matches[r, c, m] = _matches[r, c, m];

					_matches = matches;

					string[] roundNames = new string[value];
					Array.Copy(_roundNames, roundNames, _rounds);
					for (int r = _rounds; r < value; r++)
						roundNames[r] = "סיבוב " + (r + 1).ToString();
					_roundNames = roundNames;
				}
				_rounds = value; 
				OnChange();
			}
		}


		private string[] _cycleNames;
		public string GetCycleName(int cycle)
		{
			return _cycleNames[cycle];
		}
		public void SetCycleName(int cycle, string name)
		{
			name.Replace("|", "");
			_cycleNames[cycle] = name;
			OnChange();
		}
		
		private int _cycles;
		public int Cycles
		{
			get { return _cycles; }
			set 
			{ 
				if (value < 1)
					throw new ArgumentOutOfRangeException("value", "Cycles must be more than 0");

				if (value > _matches.GetLength(1))
				{
					Match[,,] matches = new Match[_rounds, value, _matchCount];
					for (int r = 0; r < _rounds; r++)
						for (int c = 0; c < _cycles; c++)
							for (int m = 0; m < _matchCount; m++)
								matches[r, c, m] = _matches[r, c, m];

					_matches = matches;

					string[] cycleNames = new string[value];
					Array.Copy(_cycleNames, cycleNames, _cycles);
					for (int c = _cycles; c < value; c++)
						cycleNames[c] = "מחזור " + (c + 1).ToString();
					_cycleNames = cycleNames;
				}
				_cycles = value; 
				OnChange();
			}
		}

		private int _matchCount;
		public int Matches
		{
			get { return _matchCount; }
			set 
			{ 
				if (value < 1)
					throw new ArgumentOutOfRangeException("value", "Matches must be more than 0");

				if (value > _matches.GetLength(2))
				{
					Match[,,] matches = new Match[_rounds, _cycles, value];
					for (int r = 0; r < _rounds; r++)
						for (int c = 0; c < _cycles; c++)
							for (int m = 0; m < _matchCount; m++)
								matches[r, c, m] = _matches[r, c, m];

					_matches = matches;
				}
				_matchCount = value; 
				OnChange();
			}
		}

		private int _matchTeams;
		public int MatchTeams
		{
			get { return _matchTeams; }
			set 
			{ 
				if (value < 2)
					throw new ArgumentOutOfRangeException("value", "Match teams must be at least 2");

				if (value < _matchTeams)
				{
					int count = 0;
					for (int n = 0; n < _tournament.Length; n++)
					{
						if (_tournament[n].TeamA > value ||
							_tournament[n].TeamB > value)
						{
							_tournament[n] = null;
							count++;
						}
					}

					if (count > 0)
					{
						if (count == _tournament.Length)
						{
							_tournament = new Game[0];
						}
						else
						{
							Game[] tour = new Game[_tournament.Length - count];
							int i = 0;
							for (int n = 0; n < _tournament.Length; n++)
							{
								if (_tournament[n] != null)
								{
									tour[i] = _tournament[n];
									i++;
								}
							}

							_tournament = tour;
						}
					}
				}

				_matchTeams = value; 
				OnChange();
			}
		}

		private Game[] _tournament;
		public int GameCount
		{
			get { return _tournament.Length; }
		}
		public Game GetGame(int number)
		{
			if (number >= _tournament.Length)
				return null;

			return _tournament[number];
		}
		public void SetGame(int number, Game game)
		{
			if (game == null)
			{
				if (number < _tournament.Length && number > 0)
				{
					Game[] tour = new Game[_tournament.Length - 1];
					Array.Copy(_tournament, tour, number);
					Array.Copy(_tournament, number + 1, tour, number, _tournament.Length - number - 1);
					_tournament = tour;
				}
			}
			else
			{
				if (!CheckGame(game))
					throw new ArgumentException("Game teams not allowed in plan");

				if (number >= _tournament.Length)
				{
					Game[] tour = new Game[_tournament.Length + 1];
					Array.Copy(_tournament, tour, _tournament.Length);
					tour[_tournament.Length] = game;
					_tournament = tour;
				}
				else
				{
					_tournament[number] = game;
				}
			}
			OnChange();
		}

		private Match[,,] _matches;
		public Match GetMatch(int round, int cycle, int matchNumber)
		{
			return _matches[round, cycle, matchNumber];
		}
		public void SetMatch(int round, int cycle, int matchNumber, Match match)
		{
			if (match != null)
				if (!CheckMatch(match))
					throw new ArgumentException("Match teams not allowed in plan");

			_matches[round, cycle, matchNumber] = match;
			OnChange();
		}
		public void SetMatch(int round, int cycle, int matchNumber, int[] teams)
		{
			SetMatch(round, cycle, matchNumber, new Match(teams));
		}

		public Match this[int round, int cycle, int match]
		{
			get
			{
				return GetMatch(round, cycle, match);
			}
			set
			{
				SetMatch(round, cycle, match, value);
			}
		}
		
		public GameBoardItem(int min, int max)
			: base(min, max)
		{
			_rounds = 1;
			_cycles = 1;
			_matchCount = 1;
			_matches = new Match[1,1,1];
			_matchTeams = 2;
			_tournament = new Game[] { new Game(1, 2) };
			_roundNames = new string[1] { "סיבוב 1" };
			_cycleNames = new string[1] { "מחזור 1" };
		}

		// Clone constructor
		public GameBoardItem(GameBoardItem gameBoardItem)
			: base(gameBoardItem.Min, gameBoardItem.Max)
		{
			_rounds = gameBoardItem._rounds;
			_cycles = gameBoardItem._cycles;
			_matchCount = gameBoardItem._matchCount;
			_roundNames = (string[]) gameBoardItem._roundNames.Clone();
			_cycleNames = (string[]) gameBoardItem._cycleNames.Clone();
			_matches = new Match[_rounds, _cycles, _matchCount];
			for (int r = 0; r < _rounds; r++)
			{
				for (int c = 0; c < _cycles; c++)
				{
					for (int m = 0; m < _matchCount; m++)
					{
						if (gameBoardItem._matches[r,c,m] != null)
							_matches[r,c,m] = (Match) gameBoardItem._matches[r,c,m].Clone();
					}
				}
			}
			_tournament = new Game[gameBoardItem.GameCount];
			for (int g = 0; g < gameBoardItem.GameCount; g++)
			{
				_tournament[g] = (Game) gameBoardItem.GetGame(g).Clone();
			}
		}

/*
		GameBoardItem string:
			roundName1|roundName2|roundName2...
			cycleName1|cycleName2|cycleName3...
tournament  teamCount|t-t|t-t|t-t...
rnd1cycl1	t-t-t|t-t-t|t-t-t...
rnd2cycl1	t-t-t|t-t-t|t-t-t...
rnd3cycl1	t-t-t|t-t-t|t-t-t...
rnd1cycl2	t-t-t|t-t-t|t-t-t...
			...
		*/
		public void Read(System.IO.StringReader reader)
		{
			char[] split = new char[] {'|'};
			string line = reader.ReadLine();
			_roundNames = line.Split(split);
			_rounds = _roundNames.Length;

			line = reader.ReadLine();
			_cycleNames = line.Split(split);
			_cycles = _cycleNames.Length;

			string[] s;

			line = reader.ReadLine();
			if (line == null || line.Length == 0)
			{
				_matchTeams = 0;
				_tournament = new Game[0];
			}
			else
			{
				s = line.Split(split);
				_matchTeams = Int32.Parse(s[0]);
				_tournament = new Game[s.Length - 1];
				for (int n = 1; n < s.Length; n++)
				{
					_tournament[n - 1] = Game.Parse(s[n]);
				}
			}

			_matchCount = -1;

			for (int c = 0; c < _cycles; c++)
			{
				for (int r = 0; r < _rounds; r++)
				{
					line = reader.ReadLine();
					s = line.Split(split);
					if (_matchCount == -1)
					{
						_matchCount = s.Length;
						_matches = new Match[_rounds, _cycles, _matchCount];
					}
					else if (_matchCount != s.Length)
						throw new Exception("Failed to load game board, match count inconsistent"); //ExecutionEngineException

					for (int m = 0; m < _matchCount; m++)
					{
						_matches[r,c,m] = Match.Parse(s[m]);
					}
				}
			}
		}

		public void Write(System.IO.StringWriter writer)
		{
			writer.WriteLine(String.Join("|", _roundNames, 0, _rounds));
			writer.WriteLine(String.Join("|", _cycleNames, 0, _cycles));

			string[] s = new string[_tournament.Length + 1];
			s[0] = _matchTeams.ToString();
			for (int n = 0; n < _tournament.Length; n++)
			{
				s[n + 1] = _tournament[n].ToString();
			}

			writer.WriteLine(String.Join("|", s));

			s = new string[_matchCount];

			for (int c = 0; c < _cycles; c++)
			{
				for (int r = 0; r < _rounds; r++)
				{
					for (int m = 0; m < _matchCount; m++)
					{
						Match match = GetMatch(r, c, m);
						if (match != null)
						{
							s[m] = match.ToString();
						}
						else
						{
							s[m] = "";
						}
					}

					writer.WriteLine(String.Join("|", s));
				}
			}
		}

		public override bool Equals(object obj)
		{
			if (obj is GameBoardItem)
			{
				return ((GameBoardItem) obj).GameBoard.Id == GameBoard.Id &&
					Min == ((GameBoardItem) obj).Min && Max == ((GameBoardItem) obj).Max;
			}

			return base.Equals(obj);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}

	public class GameBoard : System.Collections.IEnumerable
	{
		private RangeCollection _items;

		public event EventHandler Changed;
		internal void OnChange()
		{
			if (Changed != null)
				Changed(this, EventArgs.Empty);
		}

		private Sport.Common.RangeArray ranges;
		public Sport.Common.RangeArray Ranges
		{
			get { return ranges; }
		}

		public GameBoard()
		{
			_items = new RangeCollection(this);
			ranges = new Sport.Common.RangeArray();
		}

		public void CalculateRanges()
		{
			ranges.Clear();
			foreach (GameBoardItem item in _items)
			{
				ranges.Add(item.Min, item.Max);
			}
		}

		public GameBoardItem this[int index]
		{
			get { return (GameBoardItem) _items[index]; }
		}

		public GameBoardItem GetGameBoardItem(int teams)
		{
			return (GameBoardItem) _items.GetRangeItem(teams);
		}

		public void Remove(GameBoardItem value)
		{
			_items.Remove(value);
			CalculateRanges();
			OnChange();
		}

		public bool Contains(GameBoardItem value)
		{
			return _items.Contains(value);
		}

		public int IndexOf(GameBoardItem value)
		{
			return _items.IndexOf(value);
		}

		public int Count
		{
			get { return _items.Count; }
		}

		public int Add(GameBoardItem value)
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

		private int _id;
		public int Id
		{
			get { return _id; }
		}

		public bool Load(int gameBoardId)
		{
			SportServices.ProducerService ps = new SportServices.ProducerService();
			ps.CookieContainer = Sport.Core.Session.Cookies;
			string data = ps.GetGameBoard(gameBoardId);

			if (data == null)
				return false;

			System.IO.StringReader reader = new System.IO.StringReader(data);
			/*
				GameBoard string:
					min|max
					GameBoardItem
					min|max
					GameBoardItem
					...
				*/
			Clear();

			char[] split = new char[] {'|'};

			while (reader.Peek() != -1)
			{
				string line = reader.ReadLine();
				string[] s = line.Split(split);
				if (s.Length != 2)
				{
					System.Diagnostics.Debug.WriteLine("Failed to load game board");
					return false;
				}

				int min = Int32.Parse(s[0]);
				int max = Int32.Parse(s[1]);
				GameBoardItem gbi = new GameBoardItem(min, max);
				gbi.Read(reader);
				Add(gbi);
			}

			_id = gameBoardId;
			return true;
		}

		public bool Save(int gameBoardId)
		{
			System.IO.StringWriter writer = new System.IO.StringWriter();

			foreach (GameBoardItem item in _items)
			{
				writer.WriteLine(item.Min.ToString() + "|" + item.Max.ToString());
				item.Write(writer);
			}

			SportServices.ProducerService ps = new SportServices.ProducerService();
			ps.CookieContainer = Sport.Core.Session.Cookies;
			if (!ps.SetGameBoard(gameBoardId, ranges.ToString(), writer.ToString()))
				return false;

			_id = gameBoardId;
			return true;
		}

		public void CreateMatches(Sport.Championships.MatchGroup group)
		{
			GameBoardItem board = GetGameBoardItem(group.Teams.Count);

			if (board != null)
				CreateMatches(group, board.Rounds);
		}


		public void CreateMatches(Sport.Championships.MatchGroup group, int rounds)
		{
			group.Rounds.Clear();

			int number = 1;

			// Getting the maximum match number
			if (group.Phase != null && group.Phase.Championship != null)
			{
				number = ((Sport.Championships.MatchChampionship) group.Phase.Championship).GetMaxMatchNumber() + 1;
			}

			GameBoardItem board = GetGameBoardItem(group.Teams.Count);
            
			if (board != null)
			{
				if (board.Rounds < rounds)
					rounds = board.Rounds;

				int tournamentNumber = 1;
				int tournament;

				for (int r = 0; r < rounds; r++)
				{
					Sport.Championships.Round round = new Sport.Championships.Round(board.GetRoundName(r));

					for (int c = 0; c < board.Cycles; c++)
					{
						Sport.Championships.Cycle cycle = new Sport.Championships.Cycle(board.GetCycleName(c));

						for (int m = 0; m < board.Matches; m++)
						{
							GameBoardItem.Match match = board.GetMatch(r, c, m);
							if (match != null && match.Teams != null)
							{
								if (board.GameCount == 1)
								{
									tournament = -1;
								}
								else
								{
									tournament = cycle.Tournaments.Add(new Sport.Championships.Tournament(tournamentNumber));
									tournamentNumber++;
								}

								for (int g = 0; g < board.GameCount; g++)
								{
									GameBoardItem.Game game = board.GetGame(g);
									if (game.TeamA <= match.Teams.Length && game.TeamB <= match.Teams.Length)
									{
										int teamA=match.Teams[game.TeamA - 1] - 1;
										int teamB=match.Teams[game.TeamB - 1] - 1;
										if (teamA < group.Teams.Count && teamB < group.Teams.Count)
										{
											Sport.Championships.Match champMatch=
												new Sport.Championships.Match(number, teamA, teamB);
											champMatch.Tournament = tournament;
											cycle.Matches.Add(champMatch);
											number++;
										}
									}
								}
							}
						}

						round.Cycles.Add(cycle);
					}

					group.Rounds.Add(round);
				}
			}
		}
	}
}
