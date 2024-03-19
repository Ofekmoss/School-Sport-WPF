using System;

namespace Sport.Common
{
	public class LetterNumberFormat
	{
		public struct Letter
		{
			public int		value;
			public string	letter;
			public Letter(int val, string let)
			{
				value = val;
				letter = let;
			}
		}

		protected virtual Letter[] Letters
		{
			get { return null; }
		}

		protected virtual Letter[] FixedLetters
		{
			get { return null; }
		}

		public virtual int Max
		{
			get { return Int32.MaxValue; }
		}

		public virtual int Min
		{
			get { return 0; }
		}

		public virtual string ToString(int number)
		{
			Letter[] letters = Letters;
			Letter[] fixedLetters = FixedLetters;
			if (letters == null)
				return number.ToString();
			if (fixedLetters == null)
				fixedLetters = new Letter[0];

			string result = "";
			int n = number;
			int l = 0;
			int fl = 0;
			while (n > 0)
			{
				while (fl < fixedLetters.Length && n < fixedLetters[fl].value)
					fl++;
				if (fl < fixedLetters.Length && n == fixedLetters[fl].value)
				{
					result += fixedLetters[fl].letter;
					n = 0;
				}
				else
				{
					bool b = false;
					while (!b)
					{
						if (l < letters.Length)
						{
							while (n >= letters[l].value)
							{
								n -= letters[l].value;
								result += letters[l].letter;
								b = true;
							}
							l++;
						}
						else
							return null;
					}
				}
			}

			return result;
		}

		public virtual int Parse(string s)
		{
			Letter[] letters = Letters;
			Letter[] fixedLetters = FixedLetters;
			if (letters == null)
				return Int32.Parse(s);
			if (fixedLetters == null)
				fixedLetters = new Letter[0];


			string text = s;
			int n = 0;
			int l = 0;
			while (text.Length > 0)
			{
				for (int fl = 0; fl < fixedLetters.Length; fl++)
				{
					if (text == fixedLetters[fl].letter)
					{
						n += fixedLetters[fl].value;
						return n;
					}
				}

				bool b = false;
				while (!b)
				{
					if (l < letters.Length)
					{
						if (text.Substring(0, letters[l].letter.Length) == 
							letters[l].letter)
						{
							n += letters[l].value;
							text = text.Substring(letters[l].letter.Length);
							b = true;
						}
						else
							l++;
					}
					else
					{
						return 0;
					}
				}
			}
			return n;
		}
	}

	public class HebrewNumberFormat : LetterNumberFormat
	{
		private static Letter[] letters = 
			new Letter[] { 
							 new Letter(400, "ъ"),
							 new Letter(300, "щ"),
							 new Letter(200, "ш"),
							 new Letter(100, "ч"),
							 new Letter(90, "ц"),
							 new Letter(80, "ф"),
							 new Letter(70, "т"),
							 new Letter(60, "с"),
							 new Letter(50, "р"),
							 new Letter(40, "о"),
							 new Letter(30, "м"),
							 new Letter(20, "л"),
							 new Letter(10, "й"),
							 new Letter(9, "и"),
							 new Letter(8, "з"),
							 new Letter(7, "ж"),
							 new Letter(6, "е"),
							 new Letter(5, "д"),
							 new Letter(4, "г"),
							 new Letter(3, "в"),
							 new Letter(2, "б"),
							 new Letter(1, "а")
						 };
		private static Letter[] fixedLetters =
			new Letter[] {
							 new Letter(16, "иж"),
							 new Letter(15, "ие")
						 };

		protected override Sport.Common.LetterNumberFormat.Letter[] Letters
		{
			get { return letters; }
		}

		protected override Sport.Common.LetterNumberFormat.Letter[] FixedLetters
		{
			get { return fixedLetters; }
		}

		public override int Max
		{
			get { return 99999; }
		}
	}

	public class RomanNumberFormat : LetterNumberFormat
	{
		private static Letter[] letters = 
			new Letter[] { 
							 new Letter(1000, "M"),
							 new Letter(900, "CM"),
							 new Letter(100, "C"),
							 new Letter(90, "XC"),
							 new Letter(50, "L"),
							 new Letter(40, "XL"),
							 new Letter(10, "X"),
							 new Letter(9, "IX"),
							 new Letter(8, "VIII"),
							 new Letter(7, "VII"),
							 new Letter(6, "VI"),
							 new Letter(5, "V"),
							 new Letter(4, "IV"),
							 new Letter(3, "III"),
							 new Letter(2, "II"),
							 new Letter(1, "I")
						 };

		protected override Sport.Common.LetterNumberFormat.Letter[] Letters
		{
			get { return letters; }
		}

		public override int Max
		{
			get { return 99999; }
		}

	}

	public class EnglishNumberFormat : LetterNumberFormat
	{
		private static Letter[] letters = 
			new Letter[] { 
							 new Letter(26, "Z"),
							 new Letter(25, "Y"),
							 new Letter(24, "X"),
							 new Letter(23, "W"),
							 new Letter(22, "V"),
							 new Letter(21, "U"),
							 new Letter(20, "T"),
							 new Letter(19, "S"),
							 new Letter(18, "R"),
							 new Letter(17, "Q"),
							 new Letter(16, "P"),
							 new Letter(15, "O"),
							 new Letter(14, "N"),
							 new Letter(13, "M"),
							 new Letter(12, "L"),
							 new Letter(11, "K"),
							 new Letter(10, "J"),
							 new Letter(9, "I"),
							 new Letter(8, "H"),
							 new Letter(7, "G"),
							 new Letter(6, "F"),
							 new Letter(5, "E"),
							 new Letter(4, "D"),
							 new Letter(3, "C"),
							 new Letter(2, "B"),
							 new Letter(1, "A")
						 };

		protected override Sport.Common.LetterNumberFormat.Letter[] Letters
		{
			get { return letters; }
		}

		public override int Max
		{
			get { return 26; }
		}
	}

	public class LetterNumber
	{
		private LetterNumberFormat _format;
		public LetterNumberFormat Format
		{
			get { return _format; }
			set { _format = value; }
		}

		private int _number;
		public int Number
		{
			get { return _number; }
			set
			{
				if (value != _number)
				{
					if (_format != null)
					{
						if (value <= _format.Min || value > _format.Max)
							throw new ArgumentOutOfRangeException("Number", "Number out of format range");
					}

					_number = value;
				}
			}
		}

		public LetterNumber(int number, LetterNumberFormat format)
		{
			_format = format;
			Number = number;
		}

		public LetterNumber(int number)
			: this(number, new HebrewNumberFormat())
		{
		}

		public override string ToString()
		{
			if (_format == null)
				return null;
			return _format.ToString(_number);
		}

		public static string ToString(int number)
		{
			return new HebrewNumberFormat().ToString(number);
		}

		public static int Parse(string s)
		{
			return new HebrewNumberFormat().Parse(s);
		}
	}
}
