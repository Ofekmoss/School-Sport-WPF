using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sport.Common;
using System.Windows.Forms;
using Mir.Common;

namespace Sport.Synchronization
{
	public interface ISynchronizable
	{
		/// <summary>
		/// Perform synchronization, setting data for the OnSuccess handler
		/// </summary>
		/// <returns>true if anything has been done, false otherwise</returns>
		bool Synchronize(out object data);
		void OnError(string errorMessage);
		void OnSuccess(object data);
	}

	public static class SynchronizeManager
	{
		private static System.Threading.Timer _synchronizationTimer = null;
		private static System.Threading.ThreadState _synchronizationState = new System.Threading.ThreadState();
		private static System.Threading.TimerCallback _synchronizationCallback = new System.Threading.TimerCallback(SynchronizationTick);
		private static List<ISynchronizable> _instancesToSynchronize = new List<ISynchronizable>();
		
		private static void SynchronizationTick(Object state)
		{
			foreach (ISynchronizable instance in _instancesToSynchronize)
			{
				bool success;
				object data = null;
				try
				{
					success = instance.Synchronize(out data);
				}
				catch (Exception ex)
				{
					success = false;
					Logger.Instance.WriteLog(LogType.Error, "Error synchronizing instance " + instance.GetType().ToString() + ": " + ex.ToString());
					instance.OnError(ex.Message);
				}

				if (success)
				{
					try
					{
						instance.OnSuccess(data);
					}
					catch (Exception ex)
					{
						Logger.Instance.WriteLog(LogType.Error, "Error in OnSuccess event of " + instance.GetType().ToString() + ": " + ex.ToString());
					}
				}
			}
			//Sportsman.Context.SetStatusBar(Forms.MainForm.StatusBarPanels.Error, "synch " + DateTime.Now.Second);
		}

		public static void Initialize()
		{
			if (_synchronizationTimer == null)
			{
				_synchronizationTimer = new System.Threading.Timer(_synchronizationCallback, _synchronizationState, 10000, 20000);
			}
		}

		public static bool Add(ISynchronizable instance)
		{
			if (_instancesToSynchronize.Contains(instance))
				return false;
			_instancesToSynchronize.Add(instance);
			return true;
		}

		public static void Remove(ISynchronizable instance)
		{
			_instancesToSynchronize.Remove(instance);
		}

		public static int Count()
		{
			return _instancesToSynchronize.Count;
		}
	}
}
