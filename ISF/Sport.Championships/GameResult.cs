using System;

namespace Sport.Championships
{
	public class GameResult : ICloneable
	{

		#region PartResultCollection

		public class PartResultCollection : Sport.Common.GeneralCollection
		{
			public int[] this[int part]
			{
				get 
				{ 
					if (part < 0 || part >= Count)
						throw new ArgumentOutOfRangeException("part", "Part value out of range");

					return (int[]) GetItem(part); 
				}

				set 
				{ 
					if (part < 0 || part > Count)
						throw new ArgumentOutOfRangeException("part", "Part value out of range");

					if (part == Count)
					{
						Add(value);
					}
					else
					{
						SetItem(part, value); 
					}
				}
			}

			public void Insert(int part, int[] value)
			{
				InsertItem(part, value);
			}

			public void Remove(int[] value)
			{
				RemoveItem(value);
			}

			public bool Contains(int[] value)
			{
				return base.Contains(value);
			}

			public int IndexOf(int[] value)
			{
				return base.IndexOf(value);
			}

			public int Add(int[] value)
			{
				return AddItem(value);
			}
		}

		#endregion

		#region GameResultCollection

		public class GameResultCollection : Sport.Common.GeneralCollection
		{
			public PartResultCollection this[int index]
			{
				get { return (PartResultCollection) GetItem(index); }
				set { SetItem(index, value); }
			}

			public void Insert(int index, PartResultCollection value)
			{
				InsertItem(index, value);
			}

			public void Remove(PartResultCollection value)
			{
				RemoveItem(value);
			}

			public bool Contains(PartResultCollection value)
			{
				return base.Contains(value);
			}

			public int IndexOf(PartResultCollection value)
			{
				return base.IndexOf(value);
			}

			public int Add(PartResultCollection value)
			{
				return AddItem(value);
			}
		}

		#endregion

		private GameResultCollection gamesResults;
		public GameResultCollection GamesResults
		{
			get { return gamesResults; }
		}

		public int Games
		{
			get { return gamesResults.Count; }
		}

		public void AddGame()
		{
			gamesResults.Add(new PartResultCollection());
		}

		public PartResultCollection this[int game]
		{
			get 
			{ 
				if (game < 0 || game >= gamesResults.Count)
					throw new ArgumentOutOfRangeException("game", "Game value out of range");
				return gamesResults[game]; 
			}
			set
			{
				if (game < 0 || game > gamesResults.Count)
					throw new ArgumentOutOfRangeException("game", "Game value out of range");

				if (game == gamesResults.Count)
				{
					gamesResults.Add(value);
				}
				else
				{
					gamesResults[game] = value;
				}
			}
		}

		public GameResult()
		{
			gamesResults = new GameResultCollection();
			gamesResults.Add(new PartResultCollection());
		}

		public GameResult(string gameResult)
		{
			if ((gameResult == null)||(gameResult.Length == 0))
				throw new ArgumentNullException("gameResult", "Game result cannot be null or empty");

			gamesResults = new GameResultCollection();

			string[] games = gameResult.Split(new char[] { '\n' });

			foreach (string game in games)
			{
				PartResultCollection prc = new PartResultCollection();
				gamesResults.Add(prc);

				string[] parts = game.Split(new char[] { Sport.Core.Data.ScoreSeperator });

				foreach (string part in parts)
				{
					int p = part.IndexOf('-');
					if (p < 0)
						throw new ArgumentException("Incorrect result string: "+gameResult);
					prc.Add(new int[] {
										  Int32.Parse(part.Substring(0, p)),
										  Int32.Parse(part.Substring(p + 1))
									  });
				}
			}

			if (gamesResults.Count == 0)
				gamesResults.Add(new PartResultCollection());
		}

		public override string ToString()
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder();

			for (int game = 0; game < gamesResults.Count; game++)
			{
				PartResultCollection gameResult = gamesResults[game];
				if (game != 0)
				{
					sb.Append('\n');
				}

				for (int part = 0; part < gameResult.Count; part++)
				{
					if (part != 0)
					{
						sb.Append(Sport.Core.Data.ScoreSeperator);
					}

					int[] result = gameResult[part];
					sb.Append(result[0]);
					sb.Append('-');
					sb.Append(result[1]);
				}
			}

			return sb.ToString();
		}

		#region ICloneable Members

		public object Clone()
		{
			GameResult gr = new GameResult();

			while (gr.gamesResults.Count < gamesResults.Count)
				gr.gamesResults.Add(new PartResultCollection());

			for (int game = 0; game < gamesResults.Count; game++)
			{
				PartResultCollection prc = gamesResults[game];

				for (int part = 0; part < prc.Count; part++)
				{
					gr.gamesResults[game].Add((int[]) prc[part].Clone());
				}
			}

			return gr;
		}

		#endregion
	}
}
