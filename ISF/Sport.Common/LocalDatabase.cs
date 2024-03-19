using System;
using System.Data;
using System.IO;
using System.Xml;

namespace Sport.Common
{
	/// <summary>
	/// Used as a local database for field work.
	/// </summary>
	public class LocalDatabase
	{
		
		private DataSet mTables;
		string mSchemaFileName;
		string mDataFileName;

		public LocalDatabase()
		{
			mTables = new DataSet("Local Database");
			mTables.CaseSensitive = false;
			mSchemaFileName = "scheme.xml";
			mDataFileName = "data.xml";
			
			if (!File.Exists(mSchemaFileName))
				SaveDatabaseSchema(mSchemaFileName);
			else
				LoadDatabaseSchema(mSchemaFileName);

			if (!File.Exists(mDataFileName))
				SaveDatabase(mDataFileName);
			else
				LoadDatabase(mDataFileName);
		}

		/// <summary>
		/// Creates a new table with columns specified.
		/// </summary>
		/// <param name="tableName"></param>
		/// <param name="columns"></param>
		public void CreateTable(string tableName, string [] columns)
		{
			if (mTables.Tables.Contains(tableName))
				throw new Exception("Table " + tableName + " already exists!!");
			DataTable insertTable = new DataTable(tableName);
			for (int i = 0;i < columns.Length;i++)
			{
				DataColumn insertColumn = new DataColumn(columns[i],System.Type.GetType("System.String"));
				insertTable.Columns.Add(insertColumn);
			}
			mTables.Tables.Add(insertTable);

			mTables.AcceptChanges();
			SaveDatabaseSchema(mSchemaFileName);
		}

		/// <summary>
		/// Creates a new row and inserts it into the table
		/// </summary>
		/// <param name="tableName"></param>
		/// <param name="data"></param>
		public void CreateRowInTable(string tableName, string [] data)
		{
			if (!mTables.Tables.Contains(tableName))
				throw new Exception ("Table " + tableName + " does not exist!!");
			DataTable insertTable = mTables.Tables[tableName];
			if (data.Length != insertTable.Columns.Count)
				throw new Exception ("Too many or not enough rows!!");
			DataRow insertRow = insertTable.NewRow();
			for (int i = 0;i < data.Length;i++)
				insertRow[i] = data[i];
			insertTable.Rows.Add(insertRow);
			
			mTables.AcceptChanges();
			SaveDatabase(mDataFileName);
		}


		/// <summary>
		/// Loads the database schema from filename.
		/// </summary>
		public void LoadDatabaseSchema(string filename)
		{
			FileStream loadStream = new FileStream(filename,FileMode.Open);
			XmlTextReader loadReader = new XmlTextReader(loadStream);
			mTables.ReadXmlSchema(loadReader);
			loadReader.Close();
		}

		/// <summary>
		/// Saves the database schema into filename. If exists, overwrites.
		/// </summary>
		public void SaveDatabaseSchema(string filename)
		{
			FileStream saveStream = new FileStream(filename,FileMode.Create);
			XmlTextWriter saveWriter = new XmlTextWriter(saveStream,System.Text.Encoding.Unicode);
			mTables.WriteXmlSchema(saveWriter);
			saveWriter.Close();
		}

		/// <summary>
		/// Loads the database from filename.
		/// </summary>
		public void LoadDatabase(string filename)
		{
			FileStream loadStream = new FileStream(filename,FileMode.Open);
			XmlTextReader loadReader = new XmlTextReader(loadStream);
			mTables.ReadXml(loadReader);
			loadReader.Close();
		}

		/// <summary>
		/// Saves the database into filename. If exists, overwrites.
		/// </summary>
		public void SaveDatabase(string filename)
		{
			FileStream saveStream = new FileStream(filename,FileMode.Create);
			XmlTextWriter saveWriter = new XmlTextWriter(saveStream,System.Text.Encoding.Unicode);
			mTables.WriteXml(saveWriter);
			saveWriter.Close();
		}

		public void printDatabase()
		{
			PrintValues(mTables,mTables.DataSetName);
		}

		private void PrintValues(DataSet ds, string label)
		{
			Console.WriteLine("\n" + label);
			foreach(DataTable t in ds.Tables)
			{
				Console.WriteLine("TableName: " + t.TableName);
				foreach(DataColumn c in t.Columns)
					Console.Write("\t " + c.ColumnName );
				Console.Write("\n");
				foreach(DataRow r in t.Rows)
				{
					foreach(DataColumn c in t.Columns)
						Console.Write("\t " + r[c] );
					Console.WriteLine();
				}
			}
		}


	}
}
