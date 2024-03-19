using System;

namespace Sport.Data
{
	public class ExpandString
	{
		private string _text;
		public string Text
		{
			get { return _text; }
			set 
			{ 
				_text = value;
				ExpandText(_text, out _expanded, out _variables); 
			}
		}

		private string _expanded;
		public string Expanded
		{
			get { return _expanded; }
		}

		private object[] _variables;
		public object[] Variables
		{
			get { return _variables; }
		}

		public ExpandString(string text)
		{
			_text = text;
			ExpandText(_text, out _expanded, out _variables);
		}

		/// <summary>
		/// Expand the {?type=id} variables in the string to {n} and adds
		/// the entity of type 'type' and id 'id' to the variables array
		/// </summary>
		private static void ExpandText(string text, out string expanded, out object[] variables)
		{
			if (text == null)
			{
				expanded = null;
				variables = null;
				return ;
			}

			System.Collections.ArrayList vars = new System.Collections.ArrayList();
			System.Text.StringBuilder sb = new System.Text.StringBuilder(text.Length);

			int start = 0;
			int pos = text.IndexOf("{?", start);
			int end, eq;
			bool isValid=true;
			while (pos >= 0)
			{
				end = -1;
				eq = text.IndexOf('=', pos + 2);

				if (eq > pos)
					end = text.IndexOf('}', eq + 1);

				if (end > pos)
				{
					if (pos > start)
						sb.Append(text.Substring(start, pos - start));
					string typeName = text.Substring(pos + 2, eq - pos - 2);
					string idText = text.Substring(eq + 1, end - eq - 1);
					EntityType entityType = EntityType.GetEntityType(typeName);
					if (entityType == null)
						throw new EntityException("Failed to find entity type");
					int id = Int32.Parse(idText);
					try
					{
						Entity entity = entityType.Lookup(id);
						int index = vars.Add(entity);
						sb.Append("{" + index + "}");
					}
					catch
					{
						//sb.Append("???");
						isValid = false;
						sb.Remove(0, sb.Length);
					}
					start = end + 1;
				}
				else
				{
					sb.Append(text.Substring(start, pos - start + 2));
					start = pos + 2;
				}

				pos = text.IndexOf("{?", start);
			}

			if (isValid)
				sb.Append(text.Substring(start));

			expanded = sb.ToString();
			variables = vars.ToArray();
		}

		public override string ToString()
		{
			return String.Format(_expanded, _variables);
		}

		public static string Expand(string text)
		{
			string expanded;
			object[] variables;
			ExpandText(text, out expanded, out variables);

			return String.Format(expanded, variables);
		}
	}
}
