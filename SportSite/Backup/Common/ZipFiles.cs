using System;
using System.IO;
using System.Diagnostics;

namespace SportSite.Common
{
	/// <summary>
	/// Summary description for ZipFiles.
	/// </summary>
	public class ZipFiles
	{
		private static readonly string m_zipUtilityName="PKZIP25.EXE";
		
		public static int ExtractTo(string strZipFilePath, string strTargetFolder, 
			System.Web.HttpServerUtility Server)
		{
			StreamWriter objFileWrite;
			Process myProcess;
			
			string strUtilityPath=Server.MapPath(Data.AppPath+"/Common/"+m_zipUtilityName);
			
			//check zip existance:
			if (!File.Exists(strUtilityPath))
			{
				string msg=":תכנית דרושה לא נמצאה, לא ניתן להעלות קבצי זיפ<br />";
				msg += strUtilityPath+"<br />";
				throw new Exception(msg);
			}
			if (!File.Exists(strZipFilePath))
			{
				string msg=":קובץ זיפ לא קיים ";
				msg += strZipFilePath+"<br />";
				throw new Exception(msg);
			}

			//check destination folder:
			if (!Directory.Exists(strTargetFolder))
			{
				string msg=":מיקום יעד של קובץ זיפ לא נמצא ";
				msg += strTargetFolder+"<br />";
				throw new Exception(msg);
			}
			
			//create batch file:
			string strTmpBatchPath=Server.MapPath("temp/tmp_"+Tools.CurrentMilliSeconds().ToString()+".bat");
			objFileWrite = new StreamWriter(strTmpBatchPath);

			string strBatchCommand = string.Format("\"{0}\" -extract -overwrite \"{1}\" \"{2}\"",
				strUtilityPath, strZipFilePath, strTargetFolder);
			objFileWrite.WriteLine(strBatchCommand);

			objFileWrite.Close();
			objFileWrite.Dispose();

			//create process:
			myProcess=new Process();
			myProcess.StartInfo.FileName = strTmpBatchPath;
			myProcess.Start();
			myProcess.WaitForExit();
			
			//clean temporary file:
			File.Delete(strTmpBatchPath);
			
			return 0;
		}
	}
}
