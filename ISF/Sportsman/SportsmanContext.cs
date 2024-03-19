using System;
using System.Windows.Forms;

namespace Sportsman
{
	public class SportsmanContext : ApplicationContext
	{
		System.Collections.ArrayList mainForms;

		public SportsmanContext(Forms.MainForm mainForm)
		{
			mainForms = new System.Collections.ArrayList();
			Add(mainForm);
		}

		public void Add(Forms.MainForm mainForm)
		{
			mainForm.Closed += new EventHandler(FormClosed);
			mainForms.Add(mainForm);
		}

		public void CloseAll()
		{
			foreach (Forms.MainForm mainForm in mainForms)
			{
				mainForm.Close();
			}

			mainForms.Clear();
			ExitThread();
		}

		private void FormClosed(object sender, EventArgs e)
		{
			mainForms.Remove(sender);
			if (mainForms.Count == 0)
				ExitThread();
		}

		public void SetStatusBar(Forms.MainForm.StatusBarPanels panel, string text)
		{
			int index=(int) panel;
			if (index < 0)
			{
				System.Diagnostics.Debug.WriteLine("Set Status Bar: invalid panel "+panel);
				return;
			}
			try
			{
				foreach (Forms.MainForm mainForm in mainForms)
				{
					if ((mainForm.isfStatusBar != null)&&(mainForm.isfStatusBar.Panels[index] != null))
					{
						mainForm.isfStatusBar.Panels[index].Text = text;
					}
				}
			}
			catch
			{
			}
		}
		
		public string GetStatusText(Forms.MainForm.StatusBarPanels panel)
		{
			int index=(int) panel;
			if (index < 0)
			{
				System.Diagnostics.Debug.WriteLine("Set Status Bar: invalid panel "+panel);
				return null;
			}
			foreach (Forms.MainForm mainForm in mainForms)
				if ((mainForm.isfStatusBar != null)&&(mainForm.isfStatusBar.Panels[index] != null))
					return mainForm.isfStatusBar.Panels[index].Text;
			return null;
		}
	}
}
