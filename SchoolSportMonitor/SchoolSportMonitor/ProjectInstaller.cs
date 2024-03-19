using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration.Install;
using System.ComponentModel;
using System.ServiceProcess;

namespace SchoolSportMonitor
{
	// Provide the ProjectInstaller class which allows 
	// the service to be installed by the Installutil.exe tool
	[RunInstaller(true)]
	public class ProjectInstaller : Installer
	{
		//"C:\Windows\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe" "D:\SVN\Branches\SchoolSport2010\SchoolSportMonitor\SchoolSportMonitor\bin\Debug\SchoolSportMonitor.exe"
		//"C:\Windows\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe" /u "D:\Projects\BioDaqReader\BioDaqReader\bin\Debug\BioDaqReader.exe"

		private ServiceProcessInstaller process;
		private ServiceInstaller service;

		public ProjectInstaller()
		{
			process = new ServiceProcessInstaller();
			process.Account = ServiceAccount.LocalSystem;
			service = new ServiceInstaller();
			service.ServiceName = MonitorServiceHost.Service_Name;
			service.Description = MonitorServiceHost.Service_Description;
			Installers.Add(process);
			Installers.Add(service);
		}
	}
}
