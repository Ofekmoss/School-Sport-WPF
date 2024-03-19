using System;

namespace Sport.Rulesets.Rules
{
	#region Functionary Value Class
	public class FunctionaryValue : ICloneable
	{
		private int _id;
		public int Id
		{
			get {return _id;}
			set {_id = value;}
		}
		
		private string _description;
		public string Description
		{
			get {return _description;}
			set {_description = value;}
		}
		
		private Sport.Types.FunctionaryType _type;
		private string _typeName;
		public Sport.Types.FunctionaryType Type
		{
			get {return _type;}
			set
			{
				_type = value;
				_typeName = (new Sport.Types.FunctionaryTypeLookup()).Lookup((int) _type);
			}
		}
		
		public FunctionaryValue(string description, Sport.Types.FunctionaryType type)
		{
			Description = description;
			Type = type;
			_id = -1;
		}

		public override string ToString()
		{
			return this.Description+" - "+_typeName;
		}
		
		#region ICloneable Members
		public object Clone()
		{
			return new FunctionaryValue(this.Description, this.Type);
		}
		#endregion
	}
	#endregion

	#region Functionaries Class
	public class Functionaries : ICloneable
	{
		#region FunctionaryCollection
		public class FunctionaryCollection : Sport.Common.GeneralCollection
		{
			public FunctionaryValue this[int index]
			{
				get { return (FunctionaryValue) GetItem(index); }
				set { SetItem(index, value); }
			}

			public void Insert(int index, FunctionaryValue value)
			{
				InsertItem(index, value);
			}

			public void Remove(FunctionaryValue value)
			{
				RemoveItem(value);
			}

			public bool Contains(FunctionaryValue value)
			{
				return base.Contains(value);
			}

			public int IndexOf(FunctionaryValue value)
			{
				return base.IndexOf(value);
			}

			public int Add(FunctionaryValue value)
			{
				return AddItem(value);
			}
		}
		#endregion

		private FunctionaryCollection fields;
		public FunctionaryCollection Fields
		{
			get { return fields; }
		}

		public Functionaries()
		{
			fields = new FunctionaryCollection();
		}

		public override string ToString()
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder();

			foreach (FunctionaryValue field in fields)
			{
				sb.Append(field.ToString());
				sb.Append('\n');
			}

			return sb.ToString();
		}
		
		#region ICloneable Members
		public object Clone()
		{
			Functionaries func = new Functionaries();

			foreach (FunctionaryValue field in fields)
			{
				func.Fields.Add((FunctionaryValue) field.Clone());
			}
			
			return func;
		}
		#endregion
	}
	#endregion
	
	[RuleEditor("Sport.Producer.UI.Rules.FunctionariesRuleEditor, Sport.Producer.UI")]
	public class FunctionariesRule : Sport.Rulesets.RuleType
	{
		public FunctionariesRule(int id)
			: base(id, "בעלי תפקידים", Sport.Types.SportType.Both)
		{
		}
		
		public override Type GetDataType()
		{
			return typeof(Functionaries);
		}
		
		public override string ValueToString(object value)
		{
			if (value is Functionaries)
			{
				Functionaries func=(value as Functionaries);
				string result="";
				for (int i=0; i<func.Fields.Count; i++)
				{
					FunctionaryValue val=func.Fields[i];
					result += val.Description+"-"+((int) val.Type).ToString();
					if (i < (func.Fields.Count-1))
					{
						result += "\n";
					}
				}
				return result;
				//return (value as Functionaries).ToString();
			}
			return null;
		}
		
		public override object ParseValue(string value)
		{
			if (value == null)
				return null;
			
			string[] lines = value.Split(new char[] { '\n' });
			Functionaries func=new Functionaries();
			for (int i=0; i<lines.Length; i++)
			{
				string[] arrTemp = lines[i].Split(new char[] { '-' });
				if (arrTemp.Length >= 2)
				{
					string description="";
					for (int j=0; j<arrTemp.Length-1; j++)
					{
						description += arrTemp[j];
						if (j < (arrTemp.Length-1-1))
							description += "-";
					}
					Sport.Types.FunctionaryType type=(Sport.Types.FunctionaryType) 
						Int32.Parse(arrTemp[arrTemp.Length-1]);
					FunctionaryValue val=new FunctionaryValue(description, type);
					func.Fields.Add(val);
				}
			}
			return func;
		}
		
		public override void OnValueChange(Sport.Rulesets.Rule rule, 
			Sport.Rulesets.RuleScope scope)
		{
		}
	}
}
