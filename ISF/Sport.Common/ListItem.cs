using System;

namespace Sport.Common
{
	/// <summary>
	/// Summary description for ListItem.
	/// </summary>
	public class ListItem
	{
		private string _text="";
		private object _value=null;
		
		public string Text
		{
			get { return _text; }
			set { _text = value; }
		}

		public object Value
		{
			get { return _value; }
			set { _value = value; }
		}

		public ListItem()
		{
			_text = "";
			_value = null;
		}

		public ListItem(string text)
			: this()
		{
			_text = text;
		}

		public ListItem(string text, object value)
			: this()
		{
			_text = text;
			_value = value;
		}

		public override bool Equals(object obj)
		{
			if (obj is ListItem)
			{
				ListItem item=(ListItem) obj;
				if ((item._value == null)&&(this._value == null))
					return true;
				if (item._value == null)
					return false;
				return item._value.Equals(this._value);
			}
			return false;
		}
		
		public override int GetHashCode()
		{
			return base.GetHashCode ();
		}

		public override string ToString()
		{
			return (_text == null)?"":_text;
		}


	}
}
