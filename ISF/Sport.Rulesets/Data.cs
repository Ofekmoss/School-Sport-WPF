using System;

namespace Sport.Rulesets
{
	/// <summary>
	/// Summary description for Data.
	/// </summary>
	public class Data
	{
		public static readonly string[] ScoreNames = new string[]
			{
				"������",
				"����",
				"����",
				"���� ����",
				"������ ����"
			};

		public static readonly string[] ValueNames = new string[]
			{ "������", "����", "���" };
		public static readonly string[] DirectionNames = new string[]
			{ "����", "����" };
		public static readonly string[] DistanceNames = new string[]
			{ "�\"�", "���", "�\"�", "�\"�" };
		public static readonly string[] TimeNames = new string[]
			{ "����", "����", "����", "�����", "������" };
		
		public static readonly string[] MethodValueDescription = new string[]
			{
				"�����",
				"��� [%pn]",
				"���� [%pn]",
				"[%pn] ����", 
				"������ ����� ����"
			};
		public static readonly string MatchedTeamsDescription = "������ ������";
	}
}
