using System;
using System.Data.SqlClient;
using ISF.DataLayer;

namespace SportServices
{
	/// <summary>
	/// StatementBuilder generates SQL statements that selects,
	/// inserts, updates and deletes from tables according
	/// to an entity definition
	/// </summary>
	public class StatementBuilder
	{
		private EntityDefinition _entityDefinition;
		public EntityDefinition EntityDefinition
		{
			get { return _entityDefinition; }
		}

		private object _season;
		public object Season
		{
			get { return _season; }
		}

		private FilterField[] _filter;
		public FilterField[] Filter
		{
			get { return _filter; }
			set
			{
				select = false;
				_filter = value;
			}
		}

		public StatementBuilder(EntityDefinition entityDefinition, object season)
		{
			_entityDefinition = entityDefinition;
			_season = season;

			// select
			select = false;
			_selectFields = null;
			_selectTable = null;
			_selectWhere = null;
			_orderByClause = null;

			// insert
			insert = false;
			_insertTable = null;
			_insertFields = null;
			_insertValues = null;

			// update
			update = false;
			_updateTable = null;
			_setFields = null;
			_updateWhere = null;

			// delete
			delete = false;
			_deleteTable = null;
			_deleteWhere = null;
		}

		#region Parameters

		private int GetNumber(string statement, int position)
		{
			int num = 0;
			char dig = statement[position];
			while (System.Char.IsDigit(dig))
			{
				num *= 10;
				num += dig - '0';
				position++;
				if (position == statement.Length)
					return num;
				dig = statement[position];
			}

			return num;
		}

		private void AddFilterParameters(SimpleCommand command, string statement)
		{
			int p = statement.IndexOf('@');
			while (p >= 0)
			{
				// checking if filter parameter
				if (statement[p + 1] == 'F')
				{
					int num = GetNumber(statement, p + 2);
					if (_filter[num].Value == null)
					{
						command.AddParameter("@F" + num.ToString(), System.DBNull.Value);
					}
					else if (_filter[num].Value is System.Array)
					{
						int vp = statement.IndexOf('V', p + 1);
						int v = GetNumber(statement, vp + 1);
						command.AddParameter("@F" + num.ToString() + "V" + v.ToString(),
							((System.Array) _filter[num].Value).GetValue(v));
					}
					else
					{
						command.AddParameter("@F" + num.ToString(), _filter[num].Value);
					}
				}

				p = statement.IndexOf('@', p + 1);
			}
		}

		private void AddEntityParameters(SimpleCommand command, string statement, object[] entity)
		{
			int p = statement.IndexOf('@');
			while (p >= 0)
			{
				// checking if entity parameter
				if (statement[p + 1] == 'E')
				{
					int num = GetNumber(statement, p + 2);
					command.AddParameter("@E" + num.ToString(), 
						entity[num] == null ? DBNull.Value : entity[num]);
				}

				p = statement.IndexOf('@', p + 1);
			}
		}

		#endregion
		
		#region Select

		private bool select;
		private void BuildSelect()
		{
			select = true;

			_selectFields = String.Join(", ", _entityDefinition.Fields);
			_selectTable = _entityDefinition.TableName;

			if (_filter != null && _filter.Length > 0)
			{
				string[] wheres = new string[_filter.Length];
				for (int n = 0; n < _filter.Length; n++)
				{
					if (_filter[n].Value == null)
					{
						wheres[n] = (_filter[n].Not ? " NOT (" : "(") + _entityDefinition.Fields[(int) _filter[n].Field] + " IS NULL OR " +
							_entityDefinition.Fields[(int) _filter[n].Field] + " = @F" + n.ToString() + ")";
					}
					else if (_filter[n].Value is System.Array)
					{
						string vs = "";
						System.Array values = (System.Array) _filter[n].Value;
						for (int v = 0; v < values.Length; v++)
						{
							vs += _entityDefinition.Fields[(int) _filter[n].Field] + " = @F" + n.ToString() + "V" + v.ToString();
							if (v < values.Length - 1)
								vs += " OR ";
						}

						wheres[n] = (_filter[n].Not ? " NOT (" : "(") + vs + ")";
					}
					else
					{
						wheres[n] = (_filter[n].Not ? " NOT (" : "(") + _entityDefinition.Fields[(int) _filter[n].Field] + " = @F" + n.ToString() + ")";
					}
				}
				_selectWhere = String.Join(" AND ", wheres);

				if (!this.IgnoreSeason && _entityDefinition.SeasonClause != null && _season != null)
				{
					_selectWhere += " AND (" + _entityDefinition.SeasonClause + ")";
				}
			}
			else
			{
				if (_entityDefinition.SeasonClause != null && _season != null)
				{
					_selectWhere = _entityDefinition.SeasonClause;
				}
				else
				{
					_selectWhere = "";
				}
			}

			if (_entityDefinition.IdField != -1)
			{
				_orderByClause = _entityDefinition.Fields[_entityDefinition.IdField];
			}
			else
			{
				_orderByClause = "";
			}

			//Common.Debug("season: " + _season + ", clause: " + _entityDefinition.SeasonClause + ", definition: " + _entityDefinition.TableName);
		}

		private string _selectFields;
		public string SelectFields
		{
			get
			{
				if (!select)
					BuildSelect();
				return _selectFields;
			}
			set
			{
				if (!select)
					BuildSelect();
				_selectFields = value;
			}
		}

		private string _selectTable;
		public string SelectTable
		{
			get
			{
				if (!select)
					BuildSelect();
				return _selectTable;
			}
			set
			{
				if (!select)
					BuildSelect();
				_selectTable = value;
			}
		}

		private string _selectWhere;
		public string SelectWhere
		{
			get
			{
				if (!select)
					BuildSelect();
				return _selectWhere;
			}
			set
			{
				if (!select)
					BuildSelect();
				_selectWhere = value;
			}
		}

		private string _orderByClause;
		public string OrderByClause
		{
			get
			{
				if (!select)
					BuildSelect();
				return _orderByClause;
			}
			set
			{
				if (!select)
					BuildSelect();
				_orderByClause = value;
			}
		}

		public SimpleCommand GetSelect(decimal timestamp)
		{
			if (!select)
				BuildSelect();

			string statement = "SELECT " + _selectFields + ", DATE_DELETED FROM " + _selectTable + " WHERE timestamp > @TS ";
			if (_selectWhere.Length > 0)
				statement += " AND " + _selectWhere;
			if (_orderByClause.Length > 0)
				statement += " ORDER BY " + _orderByClause;

			SimpleCommand command = new SimpleCommand(statement);
			AddFilterParameters(command, statement);

			if (!this.IgnoreSeason && _entityDefinition.SeasonClause != null && _season != null)
				command.AddParameter("@SEASON", _season);

			command.AddParameter("@TS", Common.TimestampToArray(timestamp));

			return command;
		}

		public SimpleCommand GetTimestampSelect()
		{
			string statement = "SELECT MAX(timestamp) FROM " + SelectTable +
				(SelectWhere.Length > 0 ? " WHERE " + SelectWhere : " ");
			SimpleCommand command = new SimpleCommand(statement);
			AddFilterParameters(command, statement);

			if (_entityDefinition.SeasonClause != null && _season != null)
				command.AddParameter("@SEASON", _season);

			return command;
		}

		public bool IgnoreSeason
		{
			get
			{
				return (_entityDefinition.IdField >= 0) &&
					(_selectWhere.IndexOf(_entityDefinition.Fields[_entityDefinition.IdField] + " =", 
					StringComparison.CurrentCultureIgnoreCase) >= 0);
			}
		}
		#endregion

		#region Insert

		private bool insert;
		private void BuildInsert()
		{
			insert = true;

			_insertTable = _entityDefinition.TableName;
			_insertFields = "";
			_insertValues = "";

			bool first = true;
			for (int f = 0; f < _entityDefinition.Fields.Length; f++)
			{
				if (f != _entityDefinition.IdField)
				{
					if (first)
					{
						_insertValues = "@E" + f.ToString();
						_insertFields = _entityDefinition.Fields[f];
						first = false;
					}
					else
					{
						_insertValues += ", @E" + f.ToString();
						_insertFields += ", " + _entityDefinition.Fields[f];
					}
				}
			}
		}

		private string _insertTable;
		public string InsertTable
		{
			get
			{
				if (!insert)
					BuildInsert();
				return _insertTable;
			}
			set
			{
				if (!insert)
					BuildInsert();
				_insertTable = value;
			}
		}

		private string _insertFields;
		public string InsertFields
		{
			get
			{
				if (!insert)
					BuildInsert();
				return _insertFields;
			}
			set
			{
				if (!insert)
					BuildInsert();
				_insertFields = value;
			}
		}

		private string _insertValues;
		public string InsertValues
		{
			get
			{
				if (!insert)
					BuildInsert();
				return _insertValues;
			}
			set
			{
				if (!insert)
					BuildInsert();
				_insertValues = value;
			}
		}

		public string Insert
		{
			get
			{
				if (!insert)
					BuildInsert();
				return "INSERT INTO " + _insertTable + " (" + _insertFields + ") VALUES (" +
					_insertValues + ")";
			}
		}

		public SimpleCommand GetInsert(object[] entity)
		{
			string statement = Insert;
			SimpleCommand command = new SimpleCommand(statement);
			AddEntityParameters(command, statement, entity);
			return command;
		}

		#endregion

		#region Update

		private bool update;
		private void BuildUpdate()
		{
			update = true;

			_updateTable = _entityDefinition.TableName;
			_setFields = "";
			_updateWhere = "";

			int n = 0;
			for (int f = 0; f < _entityDefinition.Fields.Length; f++)
			{
				if (f == _entityDefinition.IdField)
				{
					_updateWhere = _entityDefinition.Fields[f] + " = @E" + f.ToString();
					n++;
				}
				else
				{
					if (_setFields.Length == 0)
					{
						_setFields = _entityDefinition.Fields[f] + " = @E" + f.ToString();
					}
					else
					{
						_setFields += ", " + _entityDefinition.Fields[f] + " = @E" + f.ToString();
					}
				}
			}
		}

		// update
		private string _updateTable;
		public string UpdateTable
		{
			get
			{
				if (!update)
					BuildUpdate();
				return _updateTable;
			}
			set
			{
				if (!update)
					BuildUpdate();
				_updateTable = value;
			}
		}
		private string _setFields;
		public string SetFields
		{
			get
			{
				if (!update)
					BuildUpdate();
				return _setFields;
			}
			set
			{
				if (!update)
					BuildUpdate();
				_setFields = value;
			}
		}

		private string _updateWhere;
		public string UpdateWhere
		{
			get
			{
				if (!update)
					BuildUpdate();
				return _updateWhere;
			}
			set
			{
				if (!update)
					BuildUpdate();
				_updateWhere = value;
			}
		}

		public string Update
		{
			get
			{
				if (!update)
					BuildUpdate();
				return "UPDATE " + _updateTable + " SET " + _setFields + " WHERE " + _updateWhere;
			}
		}

		public SimpleCommand GetUpdate(object[] entity)
		{
			string statement = Update;
			SimpleCommand command = new SimpleCommand(statement);
			AddEntityParameters(command, statement, entity);
			return command;
		}

		#endregion

		#region Delete

		private bool delete;
		private void BuildDelete()
		{
			delete = true;

			_deleteTable = _entityDefinition.TableName;

			_deleteWhere = _entityDefinition.Fields[_entityDefinition.IdField] + " = @E" + _entityDefinition.IdField.ToString();
		}

		private string _deleteTable;
		public string DeleteTable
		{
			get
			{
				if (!delete)
					BuildDelete();
				return _deleteTable;
			}
			set
			{
				if (!delete)
					BuildDelete();
				_deleteTable = value;
			}
		}

		private string _deleteWhere;
		public string DeleteWhere
		{
			get
			{
				if (!delete)
					BuildDelete();
				return _deleteWhere;
			}
			set
			{
				if (!delete)
					BuildDelete();
				_deleteWhere = value;
			}
		}

		public string Delete
		{
			get
			{
				if (!delete)
					BuildDelete();
				//return "DELETE FROM " + _deleteTable + " WHERE " + _deleteWhere;
				string strResult = "UPDATE " + _deleteTable + " SET DATE_DELETED = ";
				if (_deleteTable.ToUpper() == "ACCOUNT_ENTRIES")
					strResult += "'1/1/2005 00:00:00'";
				else
					strResult += "GetDate()";
				strResult += " WHERE " + _deleteWhere;
				return strResult;
			}
		}

		public SimpleCommand GetDelete(object[] entity)
		{
			string statement = Delete;
			SimpleCommand command = new SimpleCommand(statement);
			AddEntityParameters(command, statement, entity);
			return command;
		}

		#endregion
	}
}
