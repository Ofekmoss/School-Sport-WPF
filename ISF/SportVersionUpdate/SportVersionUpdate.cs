using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Xml.Serialization;
using System.IO;
using Microsoft.Win32;

namespace SportVersionUpdate
{
	/// <summary>
	/// Summary description for SportVersionUpdate.
	/// </summary>
	public class SportVersionUpdate : System.Windows.Forms.Form
	{
		private double latestVersion;
		private System.Windows.Forms.ProgressBar progressBar1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label currentFileName;
		private System.ComponentModel.IContainer components=null;
		private System.Timers.Timer execUpdateTimer;

		private bool ERROR = false;
		public SportVersionUpdate()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			System.Windows.Forms.Form.CheckForIllegalCrossThreadCalls = false;

			try
			{
				SessionService.SessionService session = new SessionService.SessionService();
				latestVersion = session.GetLatestVersion().VersionNumber;
			}
			catch
			{
				System.Windows.Forms.MessageBox.Show("כשלון בהתחברות לשרת",
					"שגיאה",System.Windows.Forms.MessageBoxButtons.OK,System.Windows.Forms.MessageBoxIcon.Error);
				ERROR = true;
				Application.Exit();
			}
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
			this.progressBar1 = new System.Windows.Forms.ProgressBar();
			this.label1 = new System.Windows.Forms.Label();
			this.currentFileName = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// progressBar1
			// 
			this.progressBar1.Location = new System.Drawing.Point(80, 56);
			this.progressBar1.Name = "progressBar1";
			this.progressBar1.Size = new System.Drawing.Size(256, 23);
			this.progressBar1.TabIndex = 0;
			// 
			// label1
			// 
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.label1.Location = new System.Drawing.Point(8, 8);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(392, 40);
			this.label1.TabIndex = 1;
			this.label1.Text = "מעדכן את התוכנה, אנא המתן...";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// currentFileName
			// 
			this.currentFileName.Location = new System.Drawing.Point(80, 88);
			this.currentFileName.Name = "currentFileName";
			this.currentFileName.Size = new System.Drawing.Size(256, 24);
			this.currentFileName.TabIndex = 2;
			this.currentFileName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// SportVersionUpdate
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(408, 118);
			this.ControlBox = false;
			this.Controls.Add(this.currentFileName);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.progressBar1);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "SportVersionUpdate";
			this.Opacity = 0;
			this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "נמצאה גרסה חדשה של התוכנה";
			this.Load += new System.EventHandler(this.SportVersionUpdate_Load);
			this.ResumeLayout(false);

		}
		#endregion

		private void execUpdate()
		{
			//Delete all .dat files
			string[] localFiles = System.IO.Directory.GetFiles(System.IO.Directory.GetCurrentDirectory());
			foreach (string localFile in localFiles)
			{
				char[] seperator = new char[1];
				seperator[0] = '.';
				string[] fileComponents = localFile.Split(seperator);
				if (fileComponents.Length > 1)
				{
					string fileExtention = fileComponents[1];
					if (fileExtention == "dat")
						System.IO.File.Delete(localFile);
				}
			}
			
			//delete cache
			string strFolderPath=Path.GetDirectoryName(Application.ExecutablePath)+
				Path.DirectorySeparatorChar+"Cache";
			if (System.IO.Directory.Exists(strFolderPath))
				System.IO.Directory.Delete(strFolderPath, true);
			
			AutoUpdate.AutoUpdateService autoUpdate = new AutoUpdate.AutoUpdateService();
			int numOfFiles = autoUpdate.getNumOfFiles();
			AutoUpdate.FileData currentFile;
			long overallSize = autoUpdate.getOverallSize();
			int currentPercentage = 0;
			for (int i = 0;i < numOfFiles;i++)
			{
				currentFile = autoUpdate.getFile(i);
				currentFileName.Text = currentFile.filename;
				writeFile(currentFile);
				currentPercentage = (int)(((double)currentFile.size/(double)overallSize)*100);
				progressBar1.Value += currentPercentage;
				
			}
	
			progressBar1.Value = 100;
		}
		
		private void deleteFile(string filename)
		{
			if (System.IO.File.Exists(filename))
			{
				try
				{
					System.IO.File.Delete(filename);
				}
				catch
				{
					System.Windows.Forms.MessageBox.Show("לא ניתן לכתוב את הקובץ: \n"+filename+"\nייתכן כי הקובץ מוגן. אנא צאו מכל התוכנות ונסו שנית",
						filename,System.Windows.Forms.MessageBoxButtons.OK,System.Windows.Forms.MessageBoxIcon.Error);
					Application.Exit();
				}
			}
		}
		
		/// <summary>
		/// Writes the actual file to the hard drive
		/// </summary>
		/// <param name="file"></param>
		private void writeFile(AutoUpdate.FileData file)
		{
			if (System.IO.File.Exists(file.filename))
			{
				try
				{
					System.IO.File.Delete(file.filename);
				}
				catch
				{
					System.Windows.Forms.MessageBox.Show("לא ניתן לכתוב את הקובץ,ייתכן כי הקובץ מוגן. אנא צאו מכל התוכנות ונסו שנית",
						file.filename,System.Windows.Forms.MessageBoxButtons.OK,System.Windows.Forms.MessageBoxIcon.Error);
					Application.Exit();
				}
			}
			BinaryWriter binWriter = new BinaryWriter(File.Open(file.filename,FileMode.Create,FileAccess.ReadWrite));
			binWriter.Write(file.file);
			binWriter.Close();
			
		}

		/// <summary>
		/// Checks if the current version matches the latest version
		/// if not, update is run.
		/// </summary>
		/// <returns></returns>
		private bool checkCurrentVersion()
		{
			RegistryParser regParser = new RegistryParser();
			string currentVersionText = regParser.getValue(RegistryParser.REGUTIL_KEYS.HKLM,"SOFTWARE\\MIR\\SchoolSport\\Ver","Current Version","-1");
			double currentVersion = Double.Parse(currentVersionText);
			if (currentVersion < latestVersion)
				return false;
			else
				return true;
		}

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			
			Application.Run(new SportVersionUpdate());
		}

		private void SportVersionUpdate_Load(object sender, System.EventArgs e)
		{
			//this.Check
			execUpdateTimer = new System.Timers.Timer(100);
			execUpdateTimer.Elapsed += new System.Timers.ElapsedEventHandler(execTimerTick);
			execUpdateTimer.AutoReset = false;
			execUpdateTimer.Start();
		}

		private void execTimerTick(object sender, System.Timers.ElapsedEventArgs e)
		{
			if (ERROR)
			{
				this.Close();
				return;
			}

			if (!checkCurrentVersion())
			{
				this.Opacity = 100;
				execUpdate();

				RegistryParser regParser = new RegistryParser();
				int writeError = 0;
				writeError = regParser.setValue(RegistryParser.REGUTIL_KEYS.HKLM,"SOFTWARE\\MIR\\SchoolSport\\Ver","Current Version",latestVersion.ToString());
				switch(writeError)
				{
					case RegistryParser.REG_ERR_NULL_REFERENCE:
						System.Windows.Forms.MessageBox.Show("תקלה קרתה","תקלה",System.Windows.Forms.MessageBoxButtons.OK,System.Windows.Forms.MessageBoxIcon.Error);
						Application.Exit();
						break;
					case RegistryParser.REG_ERR_SECURITY:
						System.Windows.Forms.MessageBox.Show("עליך להפעיל את התוכנה עם הרשאות אדמיניסטרטיביות לפחות פעם אחת!","בעיית הרשאות",System.Windows.Forms.MessageBoxButtons.OK,System.Windows.Forms.MessageBoxIcon.Error);
						Application.Exit();
						break;
					case RegistryParser.REG_ERR_UNKNOWN:
						System.Windows.Forms.MessageBox.Show("תקלה בלתי מוכרת קרתה","תקלה",System.Windows.Forms.MessageBoxButtons.OK,System.Windows.Forms.MessageBoxIcon.Error);
						Application.Exit();
						break;
					
				}
			}
			else
				System.Windows.Forms.MessageBox.Show("אין עדכונים חדשים");
			System.Diagnostics.Process.Start("Sportsman.exe");
			Application.Exit();
		}
	}
}
