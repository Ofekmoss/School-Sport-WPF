using System;
using System.Diagnostics;
using System.Collections;
using Sport.Types;

namespace Sport.Rulesets
{
	/// <summary>
	/// RulesetEventArgs is used when RulesetChanged
	/// event is called
	/// </summary>
	public class RulesetEventArgs : System.EventArgs
	{
		private int		_id;
		private Ruleset _ruleset;

		/// <summary>
		/// Gets the id of the changed ruleset
		/// </summary>
		public int		Id	{ get { return _id; } }
		/// <summary>
		/// Gets the new ruleset
		/// </summary>
		public Ruleset	Ruleset { get { return _ruleset; } }

		public RulesetEventArgs(int id, Ruleset ruleset)
		{
			_id = id;
			_ruleset = ruleset;
		}
	}

	public delegate void RulesetEventHandler(RulesetEventArgs e);

	/// <summary>
	/// Ruleset holds a set of rules.
	/// </summary>
	public class Ruleset : IEnumerable, ICloneable
	{
		#region offline rules
		private static Hashtable _offlineRuleTypes=new Hashtable();
		private static Hashtable _offlineRules=new Hashtable();
		private static int _offlineRulesCount=0;
		public static object LoadOfflineRule(System.Type type, int categoryID, int[] sportFields)
		{
			string strAllSportFields = String.Join("_", Common.Tools.ToStringArray(sportFields));
			string key = type.Name + "_" + categoryID + "_" + strAllSportFields;
			if (_offlineRules[key] == null)
			{
				System.Diagnostics.Debug.WriteLine("(" + _offlineRulesCount + ") reading offline rule " + type.Name + " category " + categoryID + " sport fields " + strAllSportFields);
				DateTime date1 = DateTime.Now;
				Sport.Common.IniFile ini = GetIniFile(categoryID);
				string strMaxValue = "";
				for (int i = 0; i < sportFields.Length; i++)
				{
					int sportFieldID = sportFields[i];
					string strCurValue = ini.ReadValue(type.Name, "sportfield_" + sportFieldID);
					if (strCurValue != null && strCurValue.Length > strMaxValue.Length)
					{
						strMaxValue = strCurValue;
					}
				}
				if (strMaxValue.Length > 0)
				{
					RuleType objRule = GetOfflineRule(type);
					if (objRule != null)
						_offlineRules[key] = objRule.ParseValue(strMaxValue);
				}
				else
				{
					_offlineRules[key] = _offlineRulesCount;
				}
				DateTime date2=DateTime.Now;
				System.Diagnostics.Debug.WriteLine("done, reading took "+((date2-date1).TotalMilliseconds)+" milli seconds. ["+(!(_offlineRules[key] is Int32))+"]");
				_offlineRulesCount++;
			}
			if (_offlineRules[key] is Int32)
				return null;
			return _offlineRules[key];
		}
		
		public static object LoadOfflineRule(System.Type type, int categoryID, int sportFieldsID)
		{
			object oRule = LoadOfflineRule(type, categoryID, new int[] { sportFieldsID });
			if (oRule == null && sportFieldsID >= 0)
			{
				oRule = LoadOfflineRule(type, categoryID, new int[] { -1 });
			}
			return oRule;
		}

		public static void SaveOfflineRule(System.Type type, object value,  
			int categoryID, int sportFieldID)
		{
			RuleType objRule=GetOfflineRule(type);
			if (objRule == null)
				return;
			string strRuleValue="";
			try
			{
				strRuleValue = objRule.ValueToString(value);
			}
			catch (Exception ex)
			{
				throw new Exception("failed to convert value of "+value.GetType().FullName+" to string: "+ex.Message);
			}
			if (strRuleValue.Length == 0)
				return;
			Sport.Common.IniFile ini=GetIniFile(categoryID);
			ini.WriteValue(type.Name, "sportfield_"+sportFieldID.ToString(), 
				strRuleValue);
		}
		
		private static Sport.Common.IniFile GetIniFile(int categoryID)
		{
			Sport.Common.IniFile ini=null;
			try
			{
				ini = new Sport.Common.IniFile(
					Core.Session.GetSeasonCache(false) + System.IO.Path.DirectorySeparatorChar +
					"rules_"+categoryID+".xml");
			}
			catch {}
			return ini;
		}
		
		private static RuleType GetOfflineRule(System.Type type)
		{
			if (_offlineRuleTypes[type] == null)
			{
				RuleType objRule=null;
				try
				{
					objRule = Activator.CreateInstance(type, new object[] { -1 }) as RuleType;
				}
				catch (Exception ex)
				{
					throw new Exception("failed to get offline rule of type "+type.FullName+": "+ex.Message);
				}
				_offlineRuleTypes[type] = objRule;
			}
			return (_offlineRuleTypes[type] as RuleType);
		}
		#endregion
		
		#region Static Rulesets Storage
		public static event RulesetEventHandler RulesetChanged;
		private static Hashtable rulesets;
		
		static Ruleset()
		{
			rulesets = new Hashtable();
		}
		
		// Class holds hashtable of all loaded rulesets
		
		// LoadRuleset search for the ruleset in the memory
		// and loads is if it doesn't
		public static Ruleset LoadRuleset(int id)
		{
			Ruleset ruleset = (Ruleset) rulesets[id];

			if (ruleset == null)
			{
				try
				{
					ruleset = new Ruleset(id);
				}
				catch (RulesetException e)
				{
					Debug.WriteLine(e.Message);
					return null;
				}

				rulesets[id] = ruleset;
			}

			return ruleset;
		}
		#endregion

		#region Properties

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

		private int _sport;
		public int Sport
		{
			get { return _sport; }
		}

		private string _pointsName="";
		public string PointsName
		{
			get {return _pointsName;}
			set {_pointsName = value;}
		}
		
		private SportType _sportType;
		public SportType SportType
		{
			get { return _sportType; }
		}

		private Ruleset _sportRuleset;
		public Ruleset SportRuleset
		{
			get { return _sportRuleset; }
		}

		#endregion

		#region Rule Enumeration

		internal int _version;

		public class RuleEnumerator : IEnumerator
		{
			private bool Next()
			{
				if (valueEnum != null)
				{
					if (valueEnum.MoveNext())
						return true;
					valueEnum = null;
				}

				while (valueEnum == null)
				{
					if (ruleEnum == null)
						ruleEnum = _ruleset.rules.GetEnumerator();

					if (!ruleEnum.MoveNext())
					{
						ruleEnum = null;
						return false;
					}

					valueEnum = Rule._ruleValues.GetEnumerator();
					if (valueEnum.MoveNext())
					{
						return true;
					}
					else
					{
						valueEnum = null;
					}
				}

				return false;
			}

			#region IEnumerator Members

			public void Reset()
			{
				if (version != _ruleset._version)
					throw new InvalidOperationException("InvalidOperation_EnumFailedVersion");

				ruleEnum = null;
				valueEnum = null;
			}

			public object Current
			{
				get
				{
					if (version != _ruleset._version)
						throw new InvalidOperationException("InvalidOperation_EnumFailedVersion");

					return Value;
				}
			}

			public bool MoveNext()
			{
				if (version != _ruleset._version)
					throw new InvalidOperationException("InvalidOperation_EnumFailedVersion");

				bool b = Next();

				while (b)
				{
					if (RuleScope.Contains(_scope))
					{
						return true;
					}
					else
					{
						b = Next();
					}
				}

				return false;
			}

			#endregion

			private Ruleset _ruleset;
			public Ruleset Ruleset
			{
				get { return _ruleset; }
			}
			private RuleScope _scope;
			public RuleScope EnumerationScope
			{
				get { return _scope; }
			}

			private int version;
			private IDictionaryEnumerator ruleEnum;
			private IDictionaryEnumerator valueEnum;

			public Rule Rule
			{
				get
				{
					if (version != _ruleset._version)
						throw new InvalidOperationException("InvalidOperation_EnumFailedVersion");

					if (ruleEnum != null)
						return (Rule) ruleEnum.Value;

					throw new InvalidOperationException("InvalidOperation_EnumEnded");
				}
			}

			public RuleType RuleType
			{
				get 
				{ 
					if (version != _ruleset._version)
						throw new InvalidOperationException("InvalidOperation_EnumFailedVersion");

					if (ruleEnum != null)
						return (RuleType) ruleEnum.Key;

					throw new InvalidOperationException("InvalidOperation_EnumEnded");
				}
			}

			public RuleScope RuleScope
			{
				get 
				{ 
					if (version != _ruleset._version)
						throw new InvalidOperationException("InvalidOperation_EnumFailedVersion");

					if (valueEnum != null)
					{
						return (RuleScope) valueEnum.Key;
					}

					throw new InvalidOperationException("InvalidOperation_EnumEnded");
				}
			}

			public object Value
			{
				get 
				{
					if (version != _ruleset._version)
						throw new InvalidOperationException("InvalidOperation_EnumFailedVersion");

					if (valueEnum != null)
					{
						return valueEnum.Value;
					}

					throw new InvalidOperationException("InvalidOperation_EnumEnded");
				}
			}


			public RuleEnumerator(Ruleset ruleset, RuleScope scope)
			{
				_ruleset = ruleset;
				_scope = scope;
				version = ruleset._version;

				Reset();
			}
		}

		public RuleEnumerator GetRuleEnumerator(RuleScope scope)
		{
			return new RuleEnumerator(this, scope);
		}

		public RuleEnumerator GetRuleEnumerator()
		{
			return GetRuleEnumerator(RuleScope.AnyScope);
		}
		
		#region IEnumerable Members

		public IEnumerator GetEnumerator()
		{
			return GetRuleEnumerator();
		}

		#endregion

		#endregion

		#region Change

		private bool loading = false;

		public event EventHandler Changed;

		internal void OnChange(Rule rule, RuleScope scope)
		{
			if (!loading)
			{
				_version++;

				// Notifing all rules of change
				foreach (DictionaryEntry entry in rules)
				{
					((RuleType) entry.Key).OnValueChange(rule, scope);
				}

				if (Changed != null)
					Changed(this, EventArgs.Empty);
			}
		}

		#endregion

		#region Rules Storage

		private Hashtable rules;
		public Rule this[RuleType ruleType]
		{
			get { return (Rule) rules[ruleType]; }
		}

		public RuleType[] GetRuleTypes()
		{
			ICollection keys = rules.Keys;
			RuleType[] ruleTypes = new RuleType[keys.Count];
			keys.CopyTo(ruleTypes, 0);
			return ruleTypes;
		}

		public Rule Add(RuleType ruleType)
		{
			Rule rule = (Rule) rules[ruleType];

			if (rule == null)
			{
				rule = new Rule(this, ruleType);
				rules[ruleType] = rule;
			}

			return rule;
		}

		public int GetRuleCount(RuleScope scope)
		{
			int count = 0;
			foreach (DictionaryEntry entry in rules)
			{
				Rule rule = entry.Value as Rule;
				count += rule.GetValueCount(scope);
			}

			return count;
		}

		public int GetRuleCount()
		{
			return GetRuleCount(RuleScope.AnyScope);
		}

		#endregion

		#region Constructors

		/// <summary>
		/// Clone constructor, create a clone instance of the given ruleset
		/// </summary>
		public Ruleset(Ruleset ruleset)
		{
			loading = true;
			_version = ruleset._version;
			_id = ruleset._id;
			_name = ruleset._name;
			_sport = ruleset._sport;
			_sportType = ruleset._sportType;

			// If the sport's ruleset is the ruleset it needs
			// to reference the cloned ruleset
			if (ruleset == ruleset._sportRuleset)
			{
				_sportRuleset = this;
			}
			else
			{
				_sportRuleset = ruleset._sportRuleset;
			}
			rules = new Hashtable();

			foreach (DictionaryEntry entry in ruleset.rules)
			{
				rules[entry.Key] = new Rule(this, (Rule) entry.Value);
			}

			loading = false;
		}

		/// <summary>
		/// Ruleset constructor. 
		/// The constructor tries to load the ruleset from the service.
		/// The ruleset is created by the LoadRuleset function only.
		/// </summary>
		protected Ruleset(int rulesetId)
		{
			_version = 0;
			loading = true;

			rules = new Hashtable();

			SportServices.RulesetService rs = new SportServices.RulesetService();
			rs.CookieContainer = Core.Session.Cookies;
			SportServices.Ruleset spRuleset = rs.LoadRuleset(rulesetId);

			if (spRuleset == null)
				throw new RulesetException("Failed to load ruleset " + rulesetId.ToString());

			if (spRuleset.SportRuleset != -1)
			{
				if (spRuleset.SportRuleset == rulesetId)
				{
					_sportRuleset = this;
				}
				else
				{
					_sportRuleset = LoadRuleset(spRuleset.SportRuleset);
					if (_sportRuleset == null)
						throw new RulesetException("Failed to load sport ruleset " + spRuleset.SportRuleset.ToString());
				}
			}

			_id = spRuleset.Id;
			_name = spRuleset.Name;
			_sport = spRuleset.Sport;
			_sportType = (SportType) spRuleset.SportType;
			string pointsName=rs.GetPointsName(this.Sport);
			
			foreach (SportServices.Rule spRule in spRuleset.Rules)
			{
				RuleType ruleType = RuleType.GetRuleType(spRule.RuleType);
				if (ruleType == null)
					throw new RulesetException("Rule type not found");
				
				object value=null;
				if (pointsName.Length > 0)
				{
					value = ruleType.ParseValue(spRule.Value, pointsName); //_pointsName
				}
				else
				{
					value = ruleType.ParseValue(spRule.Value);
				}
				
				Add(ruleType).Set(new RuleScope(spRule.Category, 
					spRule.SportFieldType, spRule.SportField), 
					value == null ? Rule.Empty : value);
			}
			
			loading = false;
		}

		#endregion

		public bool Save()
		{
			SportServices.Ruleset spRuleset = new SportServices.Ruleset();
			spRuleset.Id = _id;
			spRuleset.Name = _name;
			spRuleset.Sport = _sport;
			spRuleset.SportType = (int) _sportType;

			spRuleset.Rules = new SportServices.Rule[GetRuleCount()];
			int i = 0;

			RuleEnumerator e = GetRuleEnumerator();
            
			while (e.MoveNext())
			{
				SportServices.Rule spRule = new SportServices.Rule();
				spRule.RuleType = e.RuleType.Id;
				spRule.Category = e.RuleScope.Category;
				spRule.SportFieldType = e.RuleScope.SportFieldType;
				spRule.SportField = e.RuleScope.SportField;
				spRule.Value = e.RuleType.ValueToString(e.Value);

				spRuleset.Rules[i] = spRule;
				i++;
			}

			SportServices.RulesetService rs = new SportServices.RulesetService();
			rs.CookieContainer = Core.Session.Cookies;

			if (rs.SaveRuleset(spRuleset))
			{
				// Setting memory ruleset to a clone of this
				Ruleset ruleset = LoadRuleset(_id);
				if (ruleset != this)
				{
					ruleset = (Ruleset) Clone();
					rulesets[_id] = ruleset;

					// If the ruleset if the sport's ruleset...
					if (ruleset.SportRuleset == ruleset)
					{
						// ... update sport rulesets of this sport
						foreach (DictionaryEntry entry in rulesets)
						{
							if (((Ruleset) entry.Value).SportRuleset.Id == ruleset.Id)
							{
								((Ruleset) entry.Value)._sportRuleset = ruleset;
							}
						}
					}

					if (RulesetChanged != null)
						RulesetChanged(new RulesetEventArgs(_id, ruleset));
				}

				return true;
			}

			return false;
		}

		public object GetRule(RuleScope scope, Type type, bool exactMatch)
		{
			RuleType ruleType = RuleType.GetRuleType(type);
			if (ruleType == null)
				return null;

			object value = null;
            Rule rule = this[ruleType];
			if (rule != null)
			{
				value = rule.Find(scope, exactMatch);
			}

			if (value == null && _sportRuleset != null)
			{
				rule = _sportRuleset[ruleType];
				if (rule != null)
					value = rule.Find(scope, exactMatch);
			}

			return value;
		}

		public object GetRule(RuleScope scope, Type type)
		{
			return GetRule(scope, type, false);
		}

		#region ICloneable Members

		public object Clone()
		{
			Ruleset result=new Ruleset(this);
			result.PointsName = this._pointsName;
			return result;
		}

		#endregion
	}

	public class RulesetException : Exception
	{
		public RulesetException()
		{
		}

		public RulesetException(string s)
			: base(s)
		{
		}

		public RulesetException(System.Runtime.Serialization.SerializationInfo info, 
			System.Runtime.Serialization.StreamingContext context)
			: base(info, context)
		{
		}
	}
}
