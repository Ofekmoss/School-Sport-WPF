using System;

namespace Sport.Common
{
	/// <summary>
	/// Summary description for Crypto.
	/// </summary>
	public class Crypto
	{
		private static int ENCODING_BITS=16;
		private static char CAESAR_ZERO_COUNT='a';
		private static int CAESAR_DIVERSION=3;
		
		public static string Encode(string str)
		{
			string result="";
			string[] arrBinChars=new string[str.Length+1];
			string curPart;
			int minZeroCount=ENCODING_BITS;
			int i;
		
			for (i=0; i<str.Length; i++)
				arrBinChars[i+1] = CharToBinary(str[i], ENCODING_BITS);
		
			for (i=1; i<arrBinChars.Length; i++)
			{
				curPart = arrBinChars[i];
				int curZeroCount=0, j=0;
				while ((j < curPart.Length)&&(curPart[j] == '0'))
				{
					curZeroCount++;
					j += 1;
				}
				if (curZeroCount < minZeroCount)
					minZeroCount = curZeroCount;
			}
		
			arrBinChars[0] = "";
			if (minZeroCount < ENCODING_BITS)
			{
				arrBinChars[0] = ((char)(((int)CAESAR_ZERO_COUNT)+minZeroCount)).ToString();
				for (i=1; i<arrBinChars.Length; i++)
				{
					arrBinChars[i] = arrBinChars[i].Substring(minZeroCount);
					arrBinChars[i] = Scramble(arrBinChars[i]);
				}
			}
		
			result = String.Join("", arrBinChars);
			return result;
		}
	
		public static string Decode(string str)
		{
			string result="";
			string curBinary;
			char curChar;
			int minZeroCount=0;
			int i;
		
			if ((str.Length > 0)&&((int)str[0] > (int)CAESAR_ZERO_COUNT))
				minZeroCount = ((int)str[0])-((int)CAESAR_ZERO_COUNT);
		
			if (minZeroCount >= ENCODING_BITS)
				throw new Exception("decoding failed: invalid string: "+str);
		
			for (i=1; i<str.Length; i+=(ENCODING_BITS-minZeroCount))
			{
				curBinary = str.Substring(i, ENCODING_BITS-minZeroCount);
				curBinary = UnScramble(curBinary);
				curChar = BinaryToChar(curBinary);
				result += curChar.ToString();
			}
		
			return result;
		}
	
		private static string CharToBinary(char c, int digitsCount)
		{
			int decValue=(int)c;
			return IntToBinary(decValue, digitsCount);
		}
	
		private static char BinaryToChar(string strBinary)
		{
			int decValue=BinaryToInt(strBinary);
			return (char)decValue;
		}
	
		private static string Scramble(string str)
		{
			string result="";
			for (int i=0; i<str.Length; i++)
			{
				result += str[(i+CAESAR_DIVERSION)%str.Length].ToString();
			}
			return result;
		}
	
		private static string UnScramble(string str)
		{
			string result="";
			for (int i=0; i<str.Length; i++)
			{
				result += str[(i+str.Length-CAESAR_DIVERSION)%str.Length].ToString();
			}
			return result;
		}
	
		private static int MyPower(int num, int power)
		{
			if (power <= 0)
				return 1;
			return num*MyPower(num, power-1);
		}
	
		private static string IntToBinary(int num, int digits)
		{
			string result="";
			int tmp=num;
			if (num <= 0)
				return "0";
			while(tmp > 0)
			{
				result += ((tmp % 2)==0)?"0":"1";
				tmp /= 2;
			}
			return ReverseString(result).PadLeft(digits, '0');
		}
	
		private static int BinaryToInt(string strBinary)
		{
			int result=0;
			for (int i=0; i<strBinary.Length; i++)
			{
				if (strBinary[i] == '1')
					result += MyPower(2, (strBinary.Length-i-1));
			}
			return result;
		}
	
		private static string ReverseString(string str)
		{
			string result="";
			for (int i=str.Length-1; i>=0; i--)
			{
				result += str[i].ToString();
			}
			return result;
		}
	}
}
