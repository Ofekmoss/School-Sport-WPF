using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using LocalDatabaseManager;

namespace Sportsman_Field
{
	/// <summary>
	/// Summary description for Sportsman_Field.
	/// </summary>
	public class Sportsman_Field
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			//check local connection:
			Sport.UI.Dialogs.WaitForm.ShowWait("בודק חיבור עם מסד נתונים מקומי...");
			string strErrorMessage=LocalDatabase.TestConnection();
			Sport.UI.Dialogs.WaitForm.HideWait();
			if (strErrorMessage != null)
			{
				Sport.UI.MessageBox.Error("שגיאה בעת התחברות אל מסד נתונים מקומי:\n"+
					strErrorMessage+"\nלא ניתן להשתמש בתוכנת השטח", "שגיאת מערכת");
				return;
			}
			
			Application.Run(new MainForm());
		}
	}

	#region general stuff
	/*
		private void Form1_Load(object sender, System.EventArgs e)
		{
			//MessageBox.Show(LocalDatabase.LocalDatabase.ConnectionString());
		}

		private void button1_Click(object sender, System.EventArgs e)
		{
			string strConnection=LocalDatabase.LocalDatabase.ConnectionString();
			System.Data.OleDb.OleDbConnection connection=
				new System.Data.OleDb.OleDbConnection(strConnection);
			connection.Open();
			//INT, nvarchar(n)
			string strSQL=textBox1.Text; //"CREATE TABLE [TEST] ([FIELD1] INT)";
			System.Data.OleDb.OleDbCommand command=
				new System.Data.OleDb.OleDbCommand(strSQL, connection);
			try
			{
				command.ExecuteNonQuery();
				MessageBox.Show("success");
			}
			catch (Exception err)
			{
				MessageBox.Show("error:\n"+err.Message);
			}
			connection.Close();
		}
	*/		
	#endregion
}
