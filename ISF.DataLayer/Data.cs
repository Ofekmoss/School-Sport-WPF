using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISF.DataLayer
{
	public class SimpleCommand
	{
		public string CommandText { get; set; }
		public List<SimpleParameter> Parameters { get; private set; }

		private static string getDataLogPath = "";
		private static string getDataLogPath_prev = "";
		static SimpleCommand()
		{
			//getDataLogPath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName), "");
			System.Web.HttpContext current = System.Web.HttpContext.Current;
			if (current != null && current.Server != null)
			{
				getDataLogPath = current.Server.MapPath("GetDataLog.txt");
				getDataLogPath_prev = current.Server.MapPath("GetDataLog_prev.txt");
			}
		}

		public SimpleCommand()
		{
			this.CommandText = "";
			this.Parameters = new List<SimpleParameter>();
			
		}

		public SimpleCommand(string commandText)
			: this()
		{
			this.CommandText = commandText;
		}

		public void AddParameter(string name, object value)
		{
			if (value == null)
				value = DBNull.Value;
			this.Parameters.Add(new SimpleParameter(name, value));
		}

		public void AddParameter(SimpleParameter parameter)
		{
			this.Parameters.Add(parameter);
		}

		public bool UpdateParameter(string name, object newValue)
		{
			int index = -1;
			for (int i = 0; i < this.Parameters.Count; i++)
			{
				if (this.Parameters[i].Name.Equals(name, StringComparison.CurrentCultureIgnoreCase))
				{
					index = i;
					break;
				}
			}
			if (index >= 0)
			{
				this.Parameters.RemoveAt(index);
				this.AddParameter(name, newValue);
				return true;
			}
			return false;
		}

		public void ClearAllParameters()
		{
			this.Parameters.Clear();
		}

		public SimpleTable GetData()
		{
			if (getDataLogPath.Length > 0)
			{
				try
				{
					string lineToAdd = string.Format("[{0}] {1} [{2}]", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"),
						this.CommandText.Replace("\n", " "),
						string.Join(", ", this.Parameters.ConvertAll(p => string.Format("{0}={1}", p.Name, p.Value))));
					List<string> allLines = new List<string>();
					if (System.IO.File.Exists(getDataLogPath))
						allLines.AddRange(System.IO.File.ReadAllLines(getDataLogPath));
					if (allLines.Count >= 1000)
					{
						System.IO.File.WriteAllLines(getDataLogPath_prev, allLines.ToArray());
						allLines.Clear();
					}
					allLines.Add(lineToAdd);
					System.IO.File.WriteAllLines(getDataLogPath, allLines.ToArray());
				}
				catch
				{
				}
			}
			return DB.Instance.GetDataBySQL(this.CommandText, this.Parameters.ToArray());
		}

		public int Execute()
		{
			return DB.Instance.Execute(this.CommandText, this.Parameters.ToArray());
		}
	}

	public class SimpleTable
	{
		public string[] Columns { get; private set; }

		private List<SimpleRow> rows = new List<SimpleRow>();
		public List<SimpleRow> Rows { get { return rows; } }

		private static SimpleTable empty = null;
		public static SimpleTable Empty { get { return empty; } }
		static SimpleTable()
		{
			empty = new SimpleTable(new string[] { });
		}

		public bool IsEmpty()
		{
			return (this.Columns.Length == 0);
		}

		public bool HasRows()
		{
			return (this.Rows.Count > 0);
		}

		public SimpleTable(string[] columns)
		{
			this.Columns = columns;
		}

		public int AddRow(object[] values)
		{
			this.Rows.Add(new SimpleRow { Values = values, Parent = this });
			return this.Rows.Count;
		}

		public List<Sport.Common.SimpleData> ToSimpleData()
		{
			List<Sport.Common.SimpleData> data = new List<Sport.Common.SimpleData>();
			this.Rows.ForEach(row =>
			{
				int id = (int)row[0];
				string name = row[1].ToString();
				if (name.Length > 0 && id >= 0)
					data.Add(new Sport.Common.SimpleData(id, name));
			});
			return data;
		}
	}

	public struct SimpleRow
	{
		private static SimpleRow empty;
		static SimpleRow()
		{
			empty.Values = new object[] { };
			empty.Parent = SimpleTable.Empty;
		}
		public static SimpleRow Empty { get { return empty; } }

		public object[] Values;
		public SimpleTable Parent;

		public Sport.Common.SimpleData ToSimpleData(string idField, string nameField, string nameOverride)
		{
			return new Sport.Common.SimpleData
			{
				ID = (int)this[idField],
				Name = (nameOverride.Length > 0) ? (nameOverride) : (this[nameField] + "")
			};
		}

		public Sport.Common.SimpleData ToSimpleData(string idField, string nameField)
		{
			return ToSimpleData(idField, nameField, "");
		}

		public object this[int index]
		{
			get
			{
				if (Values != null && index >= 0 && index < Values.Length)
					return Values[index];
				return null;
			}
		}

		public object this[string column]
		{
			get
			{
				int index = GetColIndex(column);
				return this[index];
			}
		}

		public int GetSafe(string column, int defValue)
		{
			object value = this[column];
			if (value is DBNull)
				return defValue;
			return (int)value;
		}

		public int GetIntOrDefault(int index, int defValue)
		{
			int value = (int)this[index];
			return value == -1 ? defValue : value;
		}

		public int GetIntOrDefault(string column, int defValue)
		{
			int value = (int)this[column];
			return value == -1 ? defValue : value;
		}

		public bool ContainsField(string name)
		{
			return (GetColIndex(name) >= 0);
		}

		private int GetColIndex(string column)
		{
			return (Parent == null) ? -1 : Array.FindIndex(Parent.Columns, c => c.Equals(column, StringComparison.CurrentCultureIgnoreCase));
		}
	}

	public struct SimpleParameter
	{
		public string Name;
		public object Value;

		public SimpleParameter(string name, object value)
		{
			this.Name = name;
			this.Value = value;
		}

		public SimpleParameter(string name, bool cond, object valTrue, object valFalse)
		{
			this.Name = name;
			if (cond == true)
				this.Value = valTrue;
			else
				this.Value = valFalse;
		}
	}

	public struct FunctionaryData
	{
		public int Type;
		public string Name;
		public string Phone;

		public override string ToString()
		{
			string func = this.Name + "";
			if (!string.IsNullOrEmpty(this.Phone))
			{
				if (func.Length > 0)
					func += " ";
				func += string.Format("({0})", this.Phone);
			}
			return func;
		}
	}
}
