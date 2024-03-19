using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Web;
using System.Web.Services;
using System.Xml.Serialization;
using System.IO;

namespace SportServices
{
	/// <summary>
	/// This service is used for auto updating sportsman to the latest version
	/// </summary>
	public class AutoUpdateService : System.Web.Services.WebService
	{
		public struct FileData
		{
			public string filename;
			public string displayname;
			public int index;
			public long size;
			public byte[] file;
		}

		public AutoUpdateService()
		{
			//CODEGEN: This call is required by the ASP.NET Web Services Designer
			InitializeComponent();
		}

		#region Component Designer generated code
		
		//Required by the Web Services Designer 
		private IContainer components = null;
				
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if(disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);		
		}
		
		#endregion
		
		/// <summary>
		/// Gets the list of files in the latest version directory and put them in Application["Files"] 
		/// As an ArrayList. Must run before any other method
		/// </summary>
		private void initFileList()
		{
			Application["Folder Name"] = System.Configuration.ConfigurationSettings.AppSettings["LatestFolder"];
			ArrayList filesArray = new ArrayList();
			FileData currentFile;
			FileInfo currentFileInfo;
			int currentFileIndex = 0;
			string[] currentFilePath;
			char[] seperator = new char[1];
			long overallSize = 0;
			string[] filesList = System.IO.Directory.GetFiles(Server.MapPath((string)Application["Folder Name"]));
			foreach (string file in filesList)
			{
				currentFile = new FileData();
				seperator[0] = '\\';
				
				currentFilePath = file.Split(seperator[0]);
                currentFile.filename = currentFilePath[currentFilePath.Length - 1];
				currentFile.displayname = currentFilePath[currentFilePath.Length - 1];
				
				currentFile.index = currentFileIndex++;
				
				currentFileInfo = new FileInfo(file);

				currentFile.size = currentFileInfo.Length;
				overallSize += currentFileInfo.Length;
				filesArray.Add(currentFile);
			}
			Application["Latest Files"] = filesArray;
			Application["Number Of Files"] = currentFileIndex;
			Application["Overall Size"] = overallSize;
		}

		private void checkInit()
		{
			if (Application["Number Of Files"] == null)
				initFileList();
		}
		
		/// <summary>
		/// reset files list and returns amount of the files.
		/// </summary>
		/// <returns></returns>
		[WebMethod]
		public int InitFilesList()
		{
			initFileList();
			return (int) Application["Number Of Files"];
		}

		/// <summary>
		/// Gets the overall size of all files in Latest
		/// </summary>
		/// <returns></returns>
		[WebMethod]
		public long getOverallSize()
		{
			checkInit();
			return (long)Application["Overall Size"];
		}

		/// <summary>
		/// Returns the number of files in the latest version directory
		/// </summary>
		/// <returns></returns>
		[WebMethod]
		public int getNumOfFiles()
		{
			checkInit();
			return (int)Application["Number Of Files"];
		}
		
		/// <summary>
		/// Returns a FileData object corresponding to the correct file
		/// </summary>
		/// <param name="fileNum"></param>
		/// <returns></returns>
		[WebMethod]
		public FileData getFile(int fileNum)
		{
			checkInit();
			FileData returnFile = new FileData();
			if (fileNum < 0||fileNum >= (int)Application["Number Of Files"])
				return returnFile;
			else
			{
				ArrayList files = (ArrayList)Application["Latest Files"];
				returnFile = (FileData)files[fileNum];
				returnFile.file = getFile(returnFile.filename);
				return returnFile;
			}
		}


		/// <summary>
		/// Converts the file into a byte array
		/// </summary>
		/// <param name="filename"></param>
		/// <returns></returns>
		private byte[] getFile(string filename)
		{
			BinaryReader binReader = new BinaryReader(File.Open(Server.MapPath((string)Application["Folder Name"] + "/" + filename),FileMode.Open,FileAccess.Read));
			binReader.BaseStream.Position = 0;
			byte[] binFile = binReader.ReadBytes(Convert.ToInt32(binReader.BaseStream.Length));
			binReader.Close();
			return binFile;
		}
	}
}
