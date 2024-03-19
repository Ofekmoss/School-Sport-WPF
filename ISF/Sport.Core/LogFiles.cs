using System;
using System.IO;

namespace Sport.Core
{
	/// <summary>
	/// Summary description for LogFiles.
	/// </summary>
	public class LogFiles
	{
		public static void AppendToLogFile(string strFilePath, string strMsg)
		{
			StreamWriter objWriter=null;
			if (!File.Exists(strFilePath))
			{
				objWriter = File.CreateText(strFilePath);
				objWriter.Close();
			}
			FileInfo info=new FileInfo(strFilePath);
			if (info.Length > 102400)
			{
				int x=1;
				string strNewPath=Path.GetDirectoryName(strFilePath)+
					Path.DirectorySeparatorChar+"Archieve_%num_"+Path.GetFileName(strFilePath);
				while (File.Exists(strNewPath.Replace("%num", x.ToString())))
					x++;
				strNewPath = strNewPath.Replace("%num", x.ToString());
				File.Copy(strFilePath, strNewPath, true);
				objWriter = new StreamWriter(strFilePath, false);
				objWriter.Close();
			}
			objWriter = new StreamWriter(strFilePath, true);
			objWriter.Write("["+DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")+"] ");
			objWriter.WriteLine(strMsg.Replace("\n", "<br />"));
			objWriter.Close();
		}
	}
}
