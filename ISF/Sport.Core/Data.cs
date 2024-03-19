using System;

using System.Data;
using System.Data.SqlClient;
using System.Data.OleDb;

namespace Sport.Core
{
	/// <summary>
	/// Summary description for Data.
	/// </summary>
	public static class Data
	{
		private static readonly double _currentVersion = 1.432;
		private static readonly bool _betaVersion = false;

		public static readonly string MainCaption = "תוכנת ניהול ספורט התאחדות בתי הספר";
		public static readonly double Field_CurrentVersion = 1.06;
		public static readonly string IniFileName = "Sportsman.ini";
		public static readonly string IconFileName = "mirlogo.ico";
		public static bool TemporaryAuthorization = false;
		public static readonly string ProgramUpdateURL = "https://schoolsport.org.il:8080/ISF/SportServices/Register.aspx"; //?action=DownloadUpdates&fileIndex=1
		public static readonly string Field_ProgramUpdateURL = "https://schoolsport.org.il:8080/ISF/SportServices/Register.aspx?action=DownloadUpdates&fileIndex=0";
		public static readonly double MinimumVersion = 1.11;
		public static readonly double Field_MinimumVersion = 1.0;

		public static readonly char ResultFormatSeperator = '^';
		public static readonly char ResultSeperator = '|';
		public static readonly char ScoreSeperator = '|';
		
		//public static int Logged_User_ID=-1;
		
		public static class KeyCommands
		{
			public static readonly string CloseView = "View.Close";
			public static readonly string NextView = "View.Next";
			public static readonly string PreviousView = "View.Prev";
			public static readonly string FilterTable = "Table.Filter";
			public static readonly string SearchTable = "Table.Search";
			public static readonly string EditItem = "Item.Edit";
			public static readonly string MarkItem = "Item.Mark";
			public static readonly string DeleteItem = "Item.Delete";
			public static readonly string NewItem = "Item.New";
			public static readonly string CustomizeTable = "Table.Customize";
			public static readonly string Print = "Print";
			public static readonly string ItemDetails = "Item.Details";
			public static readonly string SaveItem = "Item.Save";
			public static readonly string Cancel = "Cancel";
		}

		public static double CurrentVersion
		{
			get
			{
				return _currentVersion;
			}
		}

		public static bool BetaVersion
		{
			get
			{
				return _betaVersion;
			}
		}

		public enum ResultValue
		{
			Points,
			Distance,
			Time
		}

		public enum AdministrationReportType
		{
			Undefined,
			Team,
			Personal
		}

		[Flags]
		public enum ResultMeasure
		{
			// Time Measures
			Days = 1,
			Hours = 2,
			Minutes = 4,
			Seconds = 8,
			Miliseconds = 16,
			// Distance Measures
			Kilometers = 1,
			Meters = 2,
			Centimeters = 4,
			Milimeters = 8
		}

		public enum ResultDirection
		{
			Most,
			Least
		}

		public static BankItem[] bankItems = new BankItem[]
		{
			new BankItem(4, "בנק יהב"), 
			new BankItem(7, "בנק לפיתוח התעשיה"), 
			new BankItem(9, "בנק הדואר"), 
			new BankItem(10, "בנק לאומי לישראל"), 
			new BankItem(11, "בנק דיסקונט"), 
			new BankItem(12, "בנק הפועלים"), 
			new BankItem(13, "בנק איגוד"), 
			new BankItem(14, "בנק אוצר החייל"), 
			new BankItem(17, "בנק מרכנתיל דיסקונט"), 
			new BankItem(20, "בנק מזרחי טפחות"), 
			new BankItem(26, "יובנק"), 
			new BankItem(30, "בנק למסחר"), 
			new BankItem(31, "הבנק הבינלאומי הראשון"), 
			new BankItem(34, "בנק ערבי ישראלי"), 
			new BankItem(46, "בנק מסד"), 
			new BankItem(52, "בנק פאגי"),
			new BankItem(54, "בנק ירושלים"), 
			new BankItem(68, "דקסיה ישראל")
		};

		public class BankItem
		{
			private int bankCode = -1;
			private string bankName = null;

			public int BankCode
			{
				get { return bankCode; }
				set { bankCode = value; }
			}

			public string BankName
			{
				get { return bankName; }
				set { bankName = value; }
			}

			public BankItem(int code, string name)
			{
				this.BankCode = code;
				this.BankName = name;
			}

			public override int GetHashCode()
			{
				return bankCode.GetHashCode();
			}

			public override bool Equals(object obj)
			{
				if (obj == null)
					return false;

				if (obj is BankItem)
					return (this.BankCode == (obj as BankItem).BankCode);

				return base.Equals(obj);
			}


			public override string ToString()
			{
				return this.BankName;
			}

		}
	}

	#region User Data
	/// <summary>
	/// UserData: holds user data such as username, user type and user permissions.
	/// </summary>
	public class UserData
	{
		private string _username;
		private string _userFullName;
		private string _userPassword;
		private int _userType;
		private int _userPermissions;
		private int _userRegion;
		private int _userSchool;
		private int _id;
		public static UserData Empty;

		static UserData()
		{
			Empty = new UserData();
			Empty.Username = null;
			Empty.Name = null;
		}

		/// <summary>
		/// create new UserData object with empty values.
		/// </summary>
		public UserData()
		{
			_username = "";
			_userFullName = "-לא מוגדר-";
			_userType = -1;
			_userPermissions = 0;
			_userRegion = 0;
			_userSchool = 0;
			_id = 1;
		}

		/// <summary>
		/// unique database id.
		/// </summary>
		public int Id
		{
			get { return _id; }
			set { _id = value; }
		}

		/// <summary>
		/// unique username used for login. should be in english letters only.
		/// </summary>
		public string Username
		{
			get { return _username; }
			set { _username = value; }
		}

		/// <summary>
		/// name of the person - first and last name.
		/// </summary>
		public string Name
		{
			get { return _userFullName; }
			set { _userFullName = value; }
		}

		public string UserPassword
		{
			get { return _userPassword; }
			set { _userPassword = value; }
		}

		/// <summary>
		/// type of user: internal or external.
		/// </summary>
		public int UserType
		{
			get { return _userType; }
			set { _userType = value; }
		}

		/// <summary>
		/// permissions level of the user.
		/// </summary>
		public int Permissions
		{
			get { return _userPermissions; }
			set { _userPermissions = value; }
		}

		/// <summary>
		/// region to which user belongs.
		/// </summary>
		public int UserRegion
		{
			get { return _userRegion; }
			set { _userRegion = value; }
		}

		/// <summary>
		/// school to which user belongs. can be 0, in case user is external.
		/// </summary>
		public int UserSchool
		{
			get { return _userSchool; }
			set { _userSchool = value; }
		}
	} //end class UserData
	#endregion

	#region Mir Data Reader
	public class MirDataReader
	{
		#region private properties
		private SqlDataReader _sqlReader;
		private OleDbDataReader _oleDbReader;
		private OleDbConnection _oleDbConnection;
		private DataTable _table;
		private int _rowIndex;
		#endregion

		#region constructors
		/// <summary>
		/// Create disconnected MirDataReader, you'll have to populate it later.
		/// </summary>
		public MirDataReader()
		{
			_sqlReader = null;
			_oleDbReader = null;
			_oleDbConnection = null;
			_table = null;
			_rowIndex = -1;
		}

		/// <summary>
		/// create new instance of MirDataReader and associate it with Reader.
		/// </summary>
		/// <param name="reader"></param>
		public MirDataReader(SqlDataReader reader)
			: this()
		{
			_sqlReader = reader;
		}

		/// <summary>
		/// create new instance of MirDataReader and associate it with Reader.
		/// </summary>
		/// <param name="reader"></param>
		public MirDataReader(OleDbDataReader reader, OleDbConnection connection)
			: this()
		{
			_oleDbReader = reader;
			_oleDbConnection = connection;
		}

		/// <summary>
		/// create new instance of MirDataReader and associate it with DataTable.
		/// </summary>
		public MirDataReader(DataTable table)
			: this()
		{
			_table = table;
		}
		#endregion

		#region Public properties
		public int FieldCount
		{
			get
			{
				if (_sqlReader != null)
				{
					return _sqlReader.FieldCount;
				}
				if (_oleDbReader != null)
				{
					return _oleDbReader.FieldCount;
				}
				if (_table != null)
				{
					return _table.Columns.Count;
				}
				throw new Exception("MirDataReader  error #009: can't read: no reader or table.");
			}
		}

		public bool HasRows
		{
			get
			{
				if (_sqlReader != null)
				{
					return _sqlReader.HasRows;
				}
				if (_oleDbReader != null)
				{
					return _oleDbReader.HasRows;
				}
				if (_table != null)
				{
					return (_table.Rows.Count > 0);
				}
				throw new Exception("MirDataReader  error #010: can't read: no reader or table.");
			}
		}

		public bool IsClosed
		{
			get
			{
				if (_sqlReader != null)
					return _sqlReader.IsClosed;
				if (_oleDbReader != null)
					return _oleDbReader.IsClosed;
				return (_table == null);
			}
		}

		public bool FieldExists(string strFieldName)
		{
			if (_sqlReader != null)
			{
				for (int i = 0; i < _sqlReader.FieldCount; i++)
					if (_sqlReader.GetName(i).ToLower() == strFieldName.ToLower())
						return true;
			}
			return false;
		}

		public object this[int columnIndex]
		{
			get
			{
				if (_sqlReader != null)
				{
					return _sqlReader[columnIndex];
				}
				if (_oleDbReader != null)
				{
					return _oleDbReader[columnIndex];
				}
				if (_table != null)
				{
					return ReadCurrentRow(columnIndex);
				}
				throw new Exception("MirDataReader  error #011: can't read: no reader or table.");
			}
		}

		public object this[string columnName]
		{
			get
			{
				if (_sqlReader != null)
				{
					return _sqlReader[columnName];
				}
				if (_oleDbReader != null)
				{
					return _oleDbReader[columnName];
				}
				if (_table != null)
				{
					//get column index:
					int columnIndex = -1;
					for (int i = 0; i < _table.Columns.Count; i++)
					{
						if (_table.Columns[i].ColumnName.ToLower() == columnName.ToLower())
						{
							columnIndex = i;
							break;
						}
					}
					if (columnIndex < 0)
						throw new Exception("MirDataReader: column \"" + columnName + "\" does not exist in the table.");
					return ReadCurrentRow(columnIndex);
				}
				throw new Exception("MirDataReader error #012: can't read: no reader or table.");
			}
		}
		#endregion

		#region overriden methods
		#region DataReader overriden methods
		/// <summary>
		/// Close the MirDataReader
		/// </summary>
		public void Close()
		{
			if (_sqlReader != null)
				_sqlReader.Close();

			if (_oleDbReader != null)
			{
				_oleDbReader.Close();
				if (_oleDbConnection != null)
					_oleDbConnection.Close();
			}

			if (_table != null)
				_table.Dispose();

			_sqlReader = null;
			_oleDbReader = null;
			_table = null;
		}

		/// <summary>
		/// Advance to the next record in the reader. returns whether at EOF or not
		/// </summary>
		public bool Read()
		{
			if (_sqlReader != null)
			{
				return _sqlReader.Read();
			}
			if (_oleDbReader != null)
			{
				return _oleDbReader.Read();
			}
			if (_table != null)
			{
				_rowIndex++;
				return (_rowIndex >= _table.Rows.Count);
			}
			throw new Exception("MirDataReader: can't read: no reader or table.");
		}

		/// <summary>
		/// returns whether column having given index contains Null value or not
		/// </summary>
		public bool IsDBNull(int columnIndex)
		{
			if (_sqlReader != null)
				return _sqlReader.IsDBNull(columnIndex);

			if (_oleDbReader != null)
				return _oleDbReader.IsDBNull(columnIndex);

			if (_table != null)
			{
				ReadCurrentRow(columnIndex);
				return _table.Rows[_rowIndex].IsNull(columnIndex);
			}
			throw new Exception("MirDataReader error #001: can't read: no reader or table.");
		}
		#endregion

		#region overriden object methods
		public override string ToString()
		{
			if (_sqlReader != null)
				return _sqlReader.ToString();
			if (_oleDbReader != null)
				return _oleDbReader.ToString();
			if (_table != null)
				return _table.ToString();
			return base.ToString();
		}

		public override bool Equals(object obj)
		{
			if (_sqlReader != null)
				return _sqlReader.Equals((obj as MirDataReader)._sqlReader);
			if (_oleDbReader != null)
				return _oleDbReader.Equals((obj as MirDataReader)._oleDbReader);
			if (_table != null)
				return _table.Equals((obj as MirDataReader)._table);
			return base.Equals(obj);
		}

		public override int GetHashCode()
		{
			if (_sqlReader != null)
				return _sqlReader.GetHashCode();
			if (_oleDbReader != null)
				return _oleDbReader.GetHashCode();
			if (_table != null)
				return _table.GetHashCode();
			return base.GetHashCode();
		}
		#endregion

		#region Get methods
		public decimal GetDecimal(int columnIndex)
		{
			if (_sqlReader != null)
				return _sqlReader.GetDecimal(columnIndex);

			if (_oleDbReader != null)
				return _oleDbReader.GetDecimal(columnIndex);

			if (_table != null)
			{
				object value = ReadCurrentRow(columnIndex);
				return System.Convert.ToDecimal(value);
			}
			throw new Exception("MirDataReader error #098: can't read: no reader or table.");
		}

		public int GetInt32(int columnIndex)
		{
			if (_sqlReader != null)
				return _sqlReader.GetInt32(columnIndex);

			if (_oleDbReader != null)
				return _oleDbReader.GetInt32(columnIndex);

			if (_table != null)
			{
				object value = ReadCurrentRow(columnIndex);
				return System.Convert.ToInt32(value);
			}
			throw new Exception("MirDataReader error #002: can't read: no reader or table.");
		}

		public long GetInt64(int columnIndex)
		{
			if (_sqlReader != null)
				return _sqlReader.GetInt64(columnIndex);

			if (_oleDbReader != null)
				return _oleDbReader.GetInt64(columnIndex);

			if (_table != null)
			{
				object value = ReadCurrentRow(columnIndex);
				return System.Convert.ToInt64(value);
			}
			throw new Exception("MirDataReader error #003: can't read: no reader or table.");
		}

		public DateTime GetDateTime(int columnIndex)
		{
			if (_sqlReader != null)
				return _sqlReader.GetDateTime(columnIndex);

			if (_oleDbReader != null)
				return _oleDbReader.GetDateTime(columnIndex);

			if (_table != null)
			{
				object value = ReadCurrentRow(columnIndex);
				return System.Convert.ToDateTime(value);
			}
			throw new Exception("MirDataReader error #005: can't read: no reader or table.");
			//_sqlReader.GetDouble
		}

		public double GetDouble(int columnIndex)
		{
			if (_sqlReader != null)
				return _sqlReader.GetDouble(columnIndex);

			if (_oleDbReader != null)
				return _oleDbReader.GetDouble(columnIndex);

			if (_table != null)
			{
				object value = ReadCurrentRow(columnIndex);
				return System.Convert.ToDouble(value);
			}
			throw new Exception("MirDataReader error #006: can't read: no reader or table.");
		}

		public float GetFloat(int columnIndex)
		{
			if (_sqlReader != null)
				return _sqlReader.GetFloat(columnIndex);

			if (_oleDbReader != null)
				return _oleDbReader.GetFloat(columnIndex);

			if (_table != null)
			{
				object value = ReadCurrentRow(columnIndex);
				return float.Parse(value.ToString());
			}
			throw new Exception("MirDataReader error #007: can't read: no reader or table.");
		}

		public string GetString(int columnIndex)
		{
			if (_sqlReader != null)
				return _sqlReader.GetString(columnIndex);

			if (_oleDbReader != null)
				return _oleDbReader.GetString(columnIndex);

			if (_table != null)
			{
				object value = ReadCurrentRow(columnIndex);
				return value.ToString();
			}
			throw new Exception("MirDataReader error #008: can't read: no reader or table.");
			//_sqlReader.GetString
		}

		public object GetValue(int columnIndex)
		{
			if (_sqlReader != null)
				return _sqlReader.GetValue(columnIndex);

			if (_oleDbReader != null)
				return _oleDbReader.GetValue(columnIndex);

			if (_table != null)
				return ReadCurrentRow(columnIndex);

			throw new Exception("MirDataReader error #099: can't read: no reader or table.");
		}
		#endregion
		#endregion

		#region private methods
		/// <summary>
		/// return value of the data in the given column in current row
		/// </summary>
		private object ReadCurrentRow(int columnIndex)
		{
			if (_table == null)
				throw new Exception("MirDataReader: no table to read from.");
			if (_rowIndex >= _table.Rows.Count)
				throw new Exception("MirDataReader: Reader is at EOF");
			if ((columnIndex < 0) || (columnIndex >= _table.Columns.Count))
			{
				throw new Exception("MirDataReader: invalid column index: " + columnIndex +
					", only " + _table.Columns.Count + " columns available.");
			}
			return _table.Rows[_rowIndex].ItemArray[columnIndex];
		}
		#endregion
	}
	#endregion
}

