using System;
using System.Collections;

namespace Sport.UI
{
	/// <summary>
	/// ParameterList stores a list of parameters and their value.
	/// </summary>
	public class ParameterList
	{
		private Hashtable parameters;

		public ParameterList()
		{
			parameters = new Hashtable();
		}

		// This constructor parse a parameter list in
		// the form of 'parameter name=value&parameter name=value...'
		// to a list of parameters.
		public ParameterList(string paramlist)
		{
			parameters = new Hashtable();

			string[] paramarr = paramlist.Split(new char[]{'&'});
			string[] paramvalue;

			foreach (string param in paramarr)
			{
				paramvalue = param.Split(new char[]{'='});
				this[paramvalue[0]] = paramvalue[1];
			}
		}

		// Returning the value of the given parameter
		public string this[string param]
		{
			get
			{
				return (string) parameters[param];
			}
			set
			{
				parameters[param] = value;
			}
		}

		// Returning the int value of the given state
		public int GetInt(string param)
		{
			string state = this[param];
			return state == null ? -1 : Int32.Parse(state);
		}

		// Returning the boolean value of the given state
		public bool IsSet(string param)
		{
			return this[param] == "1";
		}

		public override string ToString()
		{
			if ((parameters == null)||(parameters.Count == 0))
				return "";
			string result="?";
			foreach (string key in parameters.Keys)
			{
				if (key.Length > 0)
				{
					object value=parameters[key];
					if ((value != null)&&(value.ToString().Length > 0))
						result += key+"="+value.ToString()+"&";
				}
			}
			return result.Substring(0, result.Length-1);
		}


		public override int GetHashCode()
		{
			return base.GetHashCode ();
		}

		// Compares two parameter lists
		public override bool Equals(object obj)
		{
			if (obj is ParameterList)
			{
				Hashtable ps = (Hashtable) ((ParameterList) obj).parameters.Clone();
				IDictionaryEnumerator e = parameters.GetEnumerator();
				while (e.MoveNext())
				{
					string value = (string) ps[e.Key];
					if (value == null)
					{
						if (e.Value != null)
							return false;
					}
					else if (value.CompareTo(e.Value) != 0)
						return false; // If the other don't have this parameter or
					// the parameter value is different then
					// the states are not equal
					ps.Remove(e.Key);
				}

				// If parameters are left the states are not equal
				return ps.Count == 0;
			}

			return false;
		}

	}
}
