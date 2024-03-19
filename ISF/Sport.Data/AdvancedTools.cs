using System;

namespace Sport.Data
{
	/// <summary>
	/// Summary description for AdvancedTools.
	/// </summary>
	public class AdvancedTools
	{
		public static void ReportExcpetion(Exception ex, string message)
		{
			try
			{
				AdvancedServices.AdvancedService service=new AdvancedServices.AdvancedService();
				service.ReportUnhandledError(message + ex.Message, ex.StackTrace, Sport.Core.Session.User.Id, 
					DateTime.Now, System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
			}
			catch
			{
			}
		}

		public static void ReportExcpetion(Exception ex)
		{
			ReportExcpetion(ex, "");
		}
	}
}
