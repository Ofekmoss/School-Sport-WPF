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
	public class ChampionshipSelectForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label labelChampionship;
		private System.Windows.Forms.ComboBox cbChampionship;
		private System.Windows.Forms.Button btnOk;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button btnImport;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public class Championship
		{
			[System.Xml.Serialization.XmlAttribute(AttributeName = "id")]
			public int	_id;

			public int Id
			{
				get { return _id; }
			}

			[System.Xml.Serialization.XmlElement(ElementName = "name")]
			public string _name;

			public string Name
			{
				get { return _name; }
			}

			public override string ToString()
			{
				return _name;
			}

			public Championship()
			{
			}

			public Championship(int id, string name)
			{
				_id = id;
				_name = name;
			}
		}

		public class Sportsfield
		{
			public Championship[] Championships;
		}

		public ChampionshipSelectForm()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			btnOk.Enabled = false;

			ReadLocalChampionships();
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
			this.labelChampionship = new System.Windows.Forms.Label();
			this.cbChampionship = new System.Windows.Forms.ComboBox();
			this.btnOk = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.btnImport = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// labelChampionship
			// 
			this.labelChampionship.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelChampionship.Location = new System.Drawing.Point(296, 11);
			this.labelChampionship.Name = "labelChampionship";
			this.labelChampionship.Size = new System.Drawing.Size(48, 18);
			this.labelChampionship.TabIndex = 0;
			this.labelChampionship.Text = "אליפות:";
			// 
			// cbChampionship
			// 
			this.cbChampionship.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.cbChampionship.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbChampionship.Location = new System.Drawing.Point(8, 8);
			this.cbChampionship.Name = "cbChampionship";
			this.cbChampionship.Size = new System.Drawing.Size(288, 21);
			this.cbChampionship.TabIndex = 1;
			this.cbChampionship.SelectedIndexChanged += new System.EventHandler(this.cbChampionship_SelectedIndexChanged);
			// 
			// btnOk
			// 
			this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOk.Location = new System.Drawing.Point(8, 40);
			this.btnOk.Name = "btnOk";
			this.btnOk.TabIndex = 2;
			this.btnOk.Text = "אישור";
			// 
			// btnCancel
			// 
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(88, 40);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.TabIndex = 3;
			this.btnCancel.Text = "ביטול";
			// 
			// btnImport
			// 
			this.btnImport.Location = new System.Drawing.Point(272, 40);
			this.btnImport.Name = "btnImport";
			this.btnImport.TabIndex = 4;
			this.btnImport.Text = "יבא";
			this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
			// 
			// ChampionshipSelectForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 14);
			this.ClientSize = new System.Drawing.Size(352, 70);
			this.Controls.Add(this.btnImport);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOk);
			this.Controls.Add(this.cbChampionship);
			this.Controls.Add(this.labelChampionship);
			this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.Name = "ChampionshipSelectForm";
			this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "בחירת אליפות";
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			ChampionshipSelectForm csf = new ChampionshipSelectForm();

			if (csf.ShowDialog() == DialogResult.OK)
			{
				Sport.Core.Session.Connected = false;
				Application.Run(/*new Form1()*/);
			}
		}

		private void cbChampionship_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			btnOk.Enabled = cbChampionship.SelectedItem != null;
		}

		private void btnImport_Click(object sender, System.EventArgs e)
		{
			if (UserLogin.PerformLogin())
			{
				ChampionshipImportForm cif = new ChampionshipImportForm();

				if (cif.ShowDialog() == DialogResult.OK)
				{
					Sport.Championships.Championship championship = Sport.Championships.Championship.GetChampionship(cif.ChampionshipCategory.Id);
					if (championship != null)
					{
						Sport.Core.Session.Connected = false;
						championship.Edit();
						championship.Save();
						Sport.Core.Session.Connected = true;

						cbChampionship.Items.Add(new Championship(championship.ChampionshipCategory.Id, championship.Name));

						WriteLocalChampionships();
					}
				}
			}
		}

		private void ReadLocalChampionships()
		{
			try
			{
				System.IO.FileStream stream = new System.IO.FileStream("championships.xml", System.IO.FileMode.Open);
				System.Xml.Serialization.XmlSerializer xml = new System.Xml.Serialization.XmlSerializer(typeof(Championship));
				Championship[] championships = (Championship[]) xml.Deserialize(stream);
				stream.Close();

				cbChampionship.Items.AddRange(championships);
			}
			catch (Exception e)
			{
				System.Diagnostics.Debug.WriteLine("Failed to read championship list: " + e.Message);
			}
		}

		private void WriteLocalChampionships()
		{
			try
			{
				Championship[] championships = new Championship[cbChampionship.Items.Count];
				cbChampionship.Items.CopyTo(championships, 0);

				System.IO.FileStream stream = new System.IO.FileStream("championships.xml", System.IO.FileMode.Create);
				System.Xml.Serialization.XmlSerializer xml = new System.Xml.Serialization.XmlSerializer(typeof(Sportsfield));

				Sportsfield sportsfield = new Sportsfield();
				sportsfield.Championships = championships;


				xml.Serialize(stream, sportsfield);
				stream.Close();
			}
			catch (Exception e)
			{
				System.Diagnostics.Debug.WriteLine("Failed to save championship list: " + e.Message);
			}
		}
	}
}
