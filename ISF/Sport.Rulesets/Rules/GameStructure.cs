using System;
using Sport.Common;

namespace Sport.Rulesets.Rules
{
	#region GameStructure Value Class

	public enum GameType
	{
		Duration,
		Points
	}

	public class GameStructure : ICloneable
	{
		private GameType _gameType;
		public GameType GameType
		{
			get { return _gameType; }
			set { _gameType = value; }
		}

		public static readonly string SetsName = "מערכות";
		public static readonly string PartsName = "חלקים";
		public static readonly string GamesName = "משחקונים";
		public static readonly string ExtensionsName = "הארכות";

		private int _setPart;
		public int SetPart
		{
			get { return _setPart; }
			set { _setPart = value; }
		}

		private int _gameExtesion;
		public int GameExtension
		{
			get { return _gameExtesion; }
			set { _gameExtesion = value; }
		}

		#region Points Properties

		public int Sets
		{
			get 
			{ 
				if (_gameType != GameType.Points)
					throw new Exception("Must be 'points' game structure to set sets");
				return _setPart; 
			}
			set 
			{ 
				if (_gameType != GameType.Points)
					throw new Exception("Must be 'points' game structure to set sets");
				_setPart = value; 
			}
		}

		public int Games
		{
			get 
			{ 
				if (_gameType != GameType.Points)
					throw new Exception("Must be 'points' game structure to set games");
				return _gameExtesion; 
			}
			set 
			{ 
				if (_gameType != GameType.Points)
					throw new Exception("Must be 'points' game structure to set games");
				_gameExtesion = value; 
			}
		}

		#endregion

		#region Duration Properties

		public int Parts
		{
			get 
			{ 
				if (_gameType != GameType.Points)
					throw new Exception("Must be 'duration' game structure to set parts");
				return _setPart; 
			}
			set 
			{ 
				if (_gameType != GameType.Points)
					throw new Exception("Must be 'duration' game structure to set parts");
				_setPart = value; 
			}
		}

		public int Extensions
		{
			get 
			{ 
				if (_gameType != GameType.Points)
					throw new Exception("Must be 'duration' game structure to set extensions");
				return _gameExtesion; 
			}
			set 
			{ 
				if (_gameType != GameType.Points)
					throw new Exception("Must be 'duration' game structure to set extensions");
				_gameExtesion = value; 
			}
		}

		#endregion

		public GameStructure(GameType gameType)
		{
			_gameType = gameType;

			if (_gameType == GameType.Points)
			{
				_setPart = 3;
				_gameExtesion = 1;
			}
			else
			{
				_setPart = 2;
				_gameExtesion = 5;
			}
		}

		public override string ToString()
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			if (_gameType == GameType.Points)
			{
				sb.Append(SetsName);
				sb.Append(": ");
				sb.Append(_setPart);
				if (_gameExtesion > 1)
				{
					sb.Append('\n');
					sb.Append(GamesName);
					sb.Append(": ");
					sb.Append(_gameExtesion);
				}
			}
			else
			{
				sb.Append(PartsName);
				sb.Append(": ");
				sb.Append(_setPart);
				if (_gameExtesion > 0)
				{
					sb.Append('\n');
					sb.Append(ExtensionsName);
					sb.Append(": ");
					sb.Append(_gameExtesion);
				}
			}

			return sb.ToString();
		}

		#region ICloneable Members

		public object Clone()
		{
			GameStructure gs = new GameStructure(_gameType);

			gs._setPart = _setPart;
			gs._gameExtesion = _gameExtesion;

			return gs;
		}

		#endregion
	}

	#endregion

	[RuleEditor("Sport.Producer.UI.Rules.GameStructureRuleEditor, Sport.Producer.UI")]
	public class GameStructureRule : Sport.Rulesets.RuleType
	{
		public GameStructureRule(int id)
			: base(id, "מבנה משחק", Sport.Types.SportType.Match)
		{
		}

		public override Type GetDataType()
		{
			return typeof(GameStructure);
		}


		public override string ValueToString(object value)
		{
			GameStructure gs = value as GameStructure;
			if (gs == null)
				return null;

			return String.Join(".", 
				new string[] {
								 ((int) gs.GameType).ToString(),
								 gs.SetPart.ToString(),
								 gs.GameExtension.ToString()
							 });
		}

		public override object ParseValue(string value)
		{
			if (value == null)
				return null;

			string[] s = value.Split(new char[] { '.' });

			if (s.Length == 3)
			{
				try
				{
					GameStructure gs = new GameStructure((GameType) Int32.Parse(s[0]));
					gs.SetPart = Int32.Parse(s[1]);
					gs.GameExtension = Int32.Parse(s[2]);

					return gs;
				}
				catch
				{
				}
			}

			return null;
		}

		public override void OnValueChange(Sport.Rulesets.Rule rule, 
			Sport.Rulesets.RuleScope scope)
		{
		}
	}
}
