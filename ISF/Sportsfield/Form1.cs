using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;

namespace Sportsfield
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class Form1 : System.Windows.Forms.Form
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public Form1()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			// 
			// Form1
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(440, 302);
			this.Name = "Form1";
			this.Text = "Form1";
			this.Load += new System.EventHandler(this.Form1_Load);

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Sport.Core.Session.Connected = false;
			Application.Run(new Form1());
		}

		private void Form1_Load(object sender, System.EventArgs e)
		{
			Sport.Data.EntityFilterField seasonStatus = new Sport.Data.EntityFilterField(
				(int) Sport.Entities.Season.Fields.Status, Sport.Types.SeasonStatus.Closed, true);

			Sport.Data.Entity[] seasons = Sport.Entities.Season.Type.GetEntities(
				null);//new Sport.Data.EntityFilter(seasonStatus));

			string s="";

			foreach (Sport.Data.Entity season in seasons)
			{
				s += season.Name;
			}

			//THE BELOW LINE IS CAUSING COMPILE ERROR. PLEASE DO NOT UN-COMMENT. Yahav.
			//Sport.UI.MessageBox.Show(s);
		}
	}
}
