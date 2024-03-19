using System;
using System.Collections;
using System.Diagnostics;
using Sport.Types;

namespace Sport.Rulesets
{
	/// <summary>
	/// Base class for all rule types
	/// </summary>
	public abstract class RuleType
	{
		// Class holds hashtable for all rule types
		private static Hashtable idRuleTypes;
		private static Hashtable typeRuleTypes;
		public static RuleType GetRuleType(int id)
		{
			return (RuleType) idRuleTypes[id];
		}

		public static RuleType GetRuleType(Type type)
		{
			if (Sport.Core.Session.Connected)
			{
				return (RuleType) typeRuleTypes[type];
			}
			else
			{
				Sport.Common.IniFile ini = GetIniFile();
				string strSection = type.Name;
				string strClass = ini.ReadValue(strSection, "class");
				string strId = ini.ReadValue(strSection, "id");
				if (strSection != null && strId != null)
				{
					return GetRuleType(strClass, Int32.Parse(strId));
				}
			}
			return null;
		}

		public static RuleType[] GetRuleTypes(SportType sportType)
		{
			ArrayList rt = new ArrayList();
			foreach (DictionaryEntry entry in idRuleTypes)
			{
				RuleType ruleType = entry.Value as RuleType;
				if ((ruleType.SportType & sportType) != 0)
					rt.Add(ruleType);
			}

			return (RuleType[]) rt.ToArray(typeof(RuleType));
		}

		public static RuleType GetRuleType(string strTypeClass, int id)
		{
			// Finding rule type class
			Type type = Type.GetType(strTypeClass);
			if (type == null)
			{
				throw new RulesetException("Rule type class not found: " + strTypeClass);
			}

			// Trying to create RuleType instance
			RuleType ruleType = Activator.CreateInstance(type, new object[] { id }) as RuleType;
			// If created instance is not RuleType...
			if (ruleType == null)
			{
				//... throwing exception
				throw new RulesetException("Not of type RuleType");
			}

			return ruleType;
		}

		public static SportServices.RuleType[] GetRuleTypes()
		{
			SportServices.RulesetService rs = new SportServices.RulesetService();
			rs.CookieContainer = Sport.Core.Session.Cookies;
			return rs.GetRuleTypes();
		}

		public static void Export()
		{
			Sport.Common.IniFile ini = GetIniFile();
			SportServices.RuleType[] spRuleTypes = GetRuleTypes();
			foreach (Type type in typeRuleTypes.Keys)
			{
				string strSection = type.Name;
				RuleType ruleType = (RuleType) typeRuleTypes[type];
				SportServices.RuleType serviceType = null;
				foreach (SportServices.RuleType spType in spRuleTypes)
				{
					if (spType.Id == ruleType.Id)
					{
						serviceType = spType;
						break;
					}
				}
				if (serviceType != null)
				{
					ini.WriteValue(strSection, "class", serviceType.Class);
					ini.WriteValue(strSection, "id", serviceType.Id.ToString());
				}
			}
		}

		private static Sport.Common.IniFile GetIniFile()
		{
			return new Sport.Common.IniFile(
				Core.Session.GetSeasonCache(false) + System.IO.Path.DirectorySeparatorChar +
				"ruletypes.xml");
		}

		static RuleType()
		{
			// The rule types are read from the service
			idRuleTypes = new Hashtable();
			typeRuleTypes = new Hashtable();
			
			if (Sport.Core.Session.Connected)
			{
				SportServices.RuleType[] spRuleTypes = GetRuleTypes();

				foreach (SportServices.RuleType spType in spRuleTypes)
				{
					try
					{
						RuleType ruleType = GetRuleType(spType.Class, spType.Id);

						// Adding RuleType to hashtable
						idRuleTypes[spType.Id] = ruleType; 
						typeRuleTypes[ruleType.GetDataType()] = ruleType;
					}
					catch (Exception e)
					{
						Debug.WriteLine("Failed to create rule type instance: " + spType.Class);
						Debug.WriteLine(e.Message);
					}
				}
			}
		}

		private int _id;
		public int Id
		{
			get { return _id; }
		}
		private string _name;
		public string Name
		{
			get { return _name; }
		}
		private SportType _sportType;
		public SportType SportType
		{
			get { return _sportType; }
		}
		private CompetitionType _competitionType;
		public CompetitionType CompetitionType
		{
			get { return _competitionType; }
		}

		public override string ToString()
		{
			return _name;
		}


		public RuleType(int id, string name, SportType sportType)
		{
			_id = id;
			_name = name;
			_sportType = sportType;
			_competitionType = (_sportType & SportType.Competition) == 0 ?
				CompetitionType.None : CompetitionType.Both;
		}

		public RuleType(int id, string name, CompetitionType competitionType)
		{
			_id = id;
			_name = name;
			_sportType = SportType.Competition;
			_competitionType = competitionType;
		}

		public override bool Equals(object obj)
		{
			RuleType rt = obj as RuleType;
			if (rt == null)
				return false;
			return _id == rt._id;
		}

		public override int GetHashCode()
		{
			return _id;
		}

		public abstract Type	GetDataType();
		public abstract string	ValueToString(object value);
		public abstract object	ParseValue(string value);
		public virtual object ParseValue(string value, string pointsName)
		{
			return ParseValue(value);
		}
		public abstract void	OnValueChange(Rule rule, RuleScope scope);
		public virtual string[] GetDefinitions()
		{
			return null;
		}
		public virtual string GetDefinitionDefaultValue(string definition, object ruleValue)
		{
			return null;
		}
		public virtual string[] GetDefinitionValues(string definition, object ruleValue)
		{
			return null;
		}
	}
   
}
