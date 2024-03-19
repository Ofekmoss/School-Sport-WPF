using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Sport.Rulesets
{
	/// <summary>
	/// RuleScope defines a scope for which a rule is applied.
	/// The scope contains either a category definition, a
	/// sport field and field type definition, both or none.
	/// </summary>
	public struct RuleScope
	{
		public static readonly int Empty = -1;
		public static readonly int Any = -2;

		public static readonly RuleScope EmptyScope = new RuleScope(Empty);
		public static readonly RuleScope AnyScope = new RuleScope(Any, Any, Any);

		private int _sportFieldType;
		public int SportFieldType
		{
			get { return _sportFieldType; }
			set { _sportFieldType = value; }
		}

		private int _sportField;
		public int SportField
		{
			get { return _sportField; }
			set { _sportField = value; }
		}

		private int _category;
		public int Category
		{
			get { return _category; }
			set { _category = value; }
		}

		public RuleScope(int category, int sportFieldType, int sportField)
		{
			_category = category;
			_sportFieldType = sportFieldType;
			_sportField = sportField;
		}

		public RuleScope(int category, int sportFieldType)
			: this(category, sportFieldType, Empty)
		{
		}

		public RuleScope(int category)
			: this(category, Empty, Empty)
		{
		}

		public override int GetHashCode()
		{
			return _category ^ ((_sportFieldType << 16) | _sportField);
		}

		public override bool Equals(object obj)
		{
			if (!(obj is RuleScope))
				return false;
			RuleScope s = (RuleScope)obj;

			return s._category == _category && s._sportField == _sportField &&
				s._sportFieldType == _sportFieldType;
		}

		public static bool operator ==(RuleScope a, RuleScope b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(RuleScope a, RuleScope b)
		{
			return !a.Equals(b);
		}

		public override string ToString()
		{
			return String.Format("{0},{1},{2}", new object[] { 
																 _category, 
																 _sportFieldType,
																 _sportField
															 });
		}

		public bool Contains(RuleScope scope)
		{
			if (scope._sportFieldType != Any && _sportFieldType != Empty && scope._sportFieldType != _sportFieldType)
				return false;
			if (scope._sportField != Any && _sportField != Empty && scope._sportField != _sportField)
				return false;
			if (scope._category != Any && _category != Empty &&
				(_category | scope._category) != _category)
				return false;
			return true;
		}
	}

	/// <summary>
	/// Rule holds the data of all rules of a single
	/// RuleType in the Ruleset.
	/// </summary>
	public class Rule
	{
		private class EmptyRule
		{
			public override string ToString()
			{
				return "ריק";
			}
		}

		public static readonly object Empty = new EmptyRule();

		#region Properties

		private Ruleset _ruleset;
		/// <summary>
		/// Gets the ruleset the rule belong to
		/// </summary>
		public Ruleset Ruleset
		{
			get { return _ruleset; }
		}

		private RuleType _ruleType;
		/// <summary>
		/// Gets the type of rule
		/// </summary>
		public RuleType RuleType
		{
			get { return _ruleType; }
		}

		/// <summary>
		/// Gets the value of the rule for a specific scope
		/// </summary>
		public object this[RuleScope scope]
		{
			get { return Get(scope); }
			set { Set(scope, value); }
		}

		#endregion

		#region Constructors

		/// <summary>
		/// Rule clone constructor, creates a clone of the given rule
		/// </summary>
		public Rule(Ruleset ruleset, Rule rule)
		{
			_ruleset = ruleset;
			_ruleType = rule._ruleType;
			_ruleValues = (Hashtable)rule._ruleValues.Clone();
			_version = rule._version;
		}

		/// <summary>
		/// Rule constructor that is called when a rule is loaded
		/// from the service
		/// </summary>
		public Rule(Ruleset ruleset, RuleType ruleType)
		{
			if (ruleset == null)
				throw new ArgumentNullException("ruleset", "Ruleset cannot be null");

			_ruleset = ruleset;
			_ruleType = ruleType;
			_ruleValues = new Hashtable();
			_version = 0;
		}

		#endregion

		#region Value Enumeration

		internal int _version;

		public class ValueEnumerator : IDictionaryEnumerator
		{
			private bool Next()
			{
				if (valueEnum == null)
					valueEnum = _rule._ruleValues.GetEnumerator();
				return valueEnum.MoveNext();
			}

			#region IEnumerator Members

			public void Reset()
			{
				if (version != _rule._version)
					throw new InvalidOperationException("InvalidOperation_EnumFailedVersion");

				valueEnum = null;
			}

			public object Current
			{
				get
				{
					if (version != _rule._version)
						throw new InvalidOperationException("InvalidOperation_EnumFailedVersion");

					return Value;
				}
			}

			public bool MoveNext()
			{
				if (version != _rule._version)
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

			private Rule _rule;
			public Rule Rule
			{
				get { return _rule; }
			}
			private RuleScope _scope;
			public RuleScope EnumerationScope
			{
				get { return _scope; }
			}

			private int version;
			private IDictionaryEnumerator valueEnum;

			public RuleScope RuleScope
			{
				get
				{
					if (version != _rule._version)
						throw new InvalidOperationException("InvalidOperation_EnumFailedVersion");

					if (valueEnum != null)
					{
						return (RuleScope)valueEnum.Key;
					}

					throw new InvalidOperationException("InvalidOperation_EnumEnded");
				}
			}

			public ValueEnumerator(Rule rule, RuleScope scope)
			{
				_rule = rule;
				_scope = scope;
				version = _rule._version;

				Reset();
			}

			#region IDictionaryEnumerator Members

			public object Value
			{
				get
				{
					if (version != _rule._version)
						throw new InvalidOperationException("InvalidOperation_EnumFailedVersion");

					if (valueEnum != null)
					{
						return valueEnum.Value;
					}

					throw new InvalidOperationException("InvalidOperation_EnumEnded");
				}
			}

			public object Key
			{
				get
				{
					if (version != _rule._version)
						throw new InvalidOperationException("InvalidOperation_EnumFailedVersion");

					if (valueEnum != null)
					{
						return valueEnum.Key;
					}

					throw new InvalidOperationException("InvalidOperation_EnumEnded");
				}
			}

			public DictionaryEntry Entry
			{
				get
				{
					if (version != _rule._version)
						throw new InvalidOperationException("InvalidOperation_EnumFailedVersion");

					if (valueEnum != null)
					{
						return valueEnum.Entry;
					}

					throw new InvalidOperationException("InvalidOperation_EnumEnded");
				}
			}

			#endregion
		}

		public ValueEnumerator GetValueEnumerator(RuleScope scope)
		{
			return new ValueEnumerator(this, scope);
		}

		public ValueEnumerator GetValueEnumerator()
		{
			return GetValueEnumerator(RuleScope.AnyScope);
		}

		#region IEnumerable Members

		public IEnumerator GetEnumerator()
		{
			return GetValueEnumerator();
		}

		#endregion

		#endregion

		#region Value Operations

		private void OnChange(RuleScope scope)
		{
			_version++;
			_ruleset.OnChange(this, scope);
		}

		internal Hashtable _ruleValues;

		public object Get(RuleScope scope)
		{
			return _ruleValues[scope];
		}

		public object Find(RuleScope scope, bool exactMatch)
		{
			object value = null;
			RuleScope last = RuleScope.EmptyScope;

			//make sure to place the global scope last:
			List<DictionaryEntry> entries = new List<DictionaryEntry>();
			foreach (DictionaryEntry entry in _ruleValues)
				entries.Add(entry);
			if (entries.Count > 1)
			{
				int globalIndex = -1;
				DictionaryEntry globalEntry = default(DictionaryEntry);
				for (int i = 0; i < entries.Count - 1; i++)
				{
					DictionaryEntry curEntry = entries[i];
					RuleScope current = (RuleScope)curEntry.Key;
					if (current.Category < 0)
					{
						globalIndex = i;
						globalEntry = curEntry;
						break;
					}
				}
				if (globalIndex >= 0)
				{
					DictionaryEntry temp = entries[entries.Count - 1];
					entries[entries.Count - 1] = globalEntry;
					entries[globalIndex] = temp;
				}
			}


			foreach (DictionaryEntry entry in entries)
			{
				RuleScope current = (RuleScope)entry.Key;
				if (current.Category == scope.Category && current.SportFieldType == scope.SportFieldType && current.SportField == scope.SportField)
				{
					value = entry.Value;
					return value;
				}
			}

			foreach (DictionaryEntry entry in entries)
			{
				RuleScope current = (RuleScope)entry.Key;
				bool match1 = ((exactMatch == true) && (current.Equals(scope)));
				bool match2 = ((exactMatch == false) && (current.Contains(scope)));
				if (match1 || match2)
				{
					bool match1a = ((exactMatch == true) && (last.Equals(scope)));
					bool match2a = ((exactMatch == false) && (last.Contains(scope)));
					if (match1a || match2a)
					{
						value = entry.Value;
						last = current;
						break;
					}
				}
			}

			return value;
		}

		public object Find(RuleScope scope)
		{
			return Find(scope, false);
		}

		public void Set(RuleScope scope, object value)
		{
			if (value == null)
				_ruleValues.Remove(scope);
			else
				_ruleValues[scope] = value;
			OnChange(scope);
		}

		public object Get()
		{
			return Get(RuleScope.EmptyScope);
		}

		public void Set(object value)
		{
			Set(RuleScope.EmptyScope, value);
		}

		public int GetValueCount(RuleScope scope)
		{
			int count = 0;
			foreach (DictionaryEntry entry in _ruleValues)
			{
				if (((RuleScope)entry.Key).Contains(scope))
					count++;
			}

			return count;
		}

		public int GetValueCount()
		{
			return GetValueCount(RuleScope.AnyScope);
		}

		#endregion
	}
}
