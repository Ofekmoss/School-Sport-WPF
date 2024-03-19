using System;
using System.Collections;
using System.Windows.Forms;

namespace Sport.UI
{
	public interface ICommandTarget
	{
		bool ExecuteCommand(string command);
	}

	/// <summary>
	/// Contains static methods for handling key press events
	/// </summary>
	public class KeyListener
	{
		#region KeyCommand Class

		private class KeyCommand : IComparable
		{
			private string _command;
			public string Command
			{
				get { return _command; }
			}

			private string _name;
			public string Name
			{
				get { return _name; }
			}

			private Keys _keyData;
			public Keys KeyData
			{
				get { return _keyData; }
				set { _keyData = value; }
			}

			public KeyCommand(string command, string name,
				Keys keyData)
			{
				_command = command;
				_name = name;
				_keyData = keyData;
			}

			public override bool Equals(object obj)
			{
				if (obj is string)
					return _command.Equals((string) obj);

				return base.Equals (obj);
			}

			public override int GetHashCode()
			{
				return base.GetHashCode ();
			}

			#region IComparable Members

			public int CompareTo(object obj)
			{
				if (obj is string)
					return _command.CompareTo((string) obj);
				return _command.CompareTo(((KeyCommand) obj)._command);
			}

			#endregion
		}

		#endregion
		
		public static int Count
		{
			get { return commands.Count; }
		}

		public static Keys GetCommendKey(int index)
		{
			return ((KeyCommand) commands[index]).KeyData;
		}

		public static string GetCommandName(int index)
		{
			return ((KeyCommand) commands[index]).Name;
		}

		public static string GetCommandCommand(int index)
		{
			return ((KeyCommand) commands[index]).Command;
		}

		public static void SetCommandKey(int index, Keys keyData)
		{
			keys.Remove(((KeyCommand) commands[index]).KeyData);
			((KeyCommand) commands[index]).KeyData = keyData;

			if (keys.Contains(keyData))
			{
				string cmd = (string) keys[keyData];
				int i = commands.IndexOf(cmd);
				if (i != index)
					((KeyCommand) commands[i]).KeyData = Keys.None;
			}

			keys[keyData] = ((KeyCommand) commands[index]).Command;
		}

		public static int GetCommandIndex(string command)
		{
			int index = commands.IndexOf(command);
			return index < 0 ? -1 : index;
		}

		public static string GetKeyCommand(Keys keyData)
		{
			return keys[keyData] as string;
		}

		private static Sport.Common.SortedArray commands;
		private static Hashtable				keys;

		static KeyListener()
		{
			commands	= new Sport.Common.SortedArray();
			keys		= new Hashtable();
		}
		
		public static void RegisterCommand(string command, string name,
			Keys keyData)
		{
			if (keyData != Keys.None && keys.Contains(keyData))
			{
				string cmd = (string) keys[keyData];
				int index = commands.IndexOf(cmd);
				((KeyCommand) commands[index]).KeyData = Keys.None;
			}

			commands.Add(new KeyCommand(command, name, keyData));
			if (keyData != Keys.None)
				keys.Add(keyData, command);
		}

		public static Form mainForm = null;
		public static bool HandleEvent(Control control, KeyEventArgs e)
		{
			if ((e.KeyData == Keys.Enter)||(keys.Contains(e.KeyData)))
			{
				string command = (keys.Contains(e.KeyData))?
					((string) keys[e.KeyData]):"ENTER";
				while (control != null)
				{
					if (control is ICommandTarget)
					{
						if (((ICommandTarget) control).ExecuteCommand(command))
							return true;
					}

					control = control.Parent;
				}
			}

			return false;
		}

	}
}
