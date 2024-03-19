using System;
using System.Windows.Forms;
using System.Xml;
using Sport.UI;
using Sport.UI.Display;
using System.Collections;
//using Sportsman.PermissionServices;

namespace Sportsman.Config
{
	/// <summary>
	/// MenuSectionHandler implements IConfigurationSectionHandler
	/// in order to read the menu structure from the configuration file.
	/// </summary>
	public class MenuSectionHandler : System.Configuration.IConfigurationSectionHandler
	{
		//private PermissionService _permissionService=null;
		//private PermissionData[] _arrUserPermissions=null;

		/// <summary>
		/// MenuCommandHandler acts as an event target
		/// for the menu Click event, containing the 
		/// Command object to execute and the param string to pass.
		/// </summary>
		public class MenuCommandHandler
		{
			string		_param;
			Command		_command;
			MenuItem	_item;

			public MenuCommandHandler(MenuItem item, Command cmd, string param)
			{
				_command = cmd;
				_param = param;
				_item = item;
			}

			public void MenuItemClicked(object sender, System.EventArgs e)
			{
				_command.Execute(_param);
			}

			public void MenuItemStateChanged(object sender, System.EventArgs e)
			{
				if (sender is StateItem)
				{
					_item.Checked = ((StateItem) sender).Checked;
					_item.Enabled = ((StateItem) sender).Enabled;
					_item.Visible = ((StateItem) sender).Visible;
				}
			}
		}

		/// <summary>
		/// CreateMenuItem recursively create a MenuItem
		/// and its sub items while reading the information
		/// needed from an XmlNode
		/// </summary>
		public MenuItem CreateMenuItem(XmlNode node)
		{
			// The caption attribute will be the menu item text
			XmlNode caption = node.Attributes.GetNamedItem("caption");
			MenuItem mi = new MenuItem(caption == null ? "ללא שם" : caption.Value);
			// The command attribute will be the class name
			// of the Command object
			XmlNode command = node.Attributes.GetNamedItem("command");
			// The param string will be pass to the command
			// on execute
			XmlNode param = node.Attributes.GetNamedItem("param");
			XmlNode state = node.Attributes.GetNamedItem("state");
			XmlNode shortcut = node.Attributes.GetNamedItem("shortcut");
			XmlNode myNode=param;
			if ((myNode == null)||(myNode.Value == null)||(myNode.Value.Length == 0))
				myNode = command;
			
			if (shortcut != null && shortcut.Value != null && shortcut.Value.Length > 0)
			{
				try
				{
					mi.Shortcut = (System.Windows.Forms.Shortcut)
						Enum.Parse(typeof(System.Windows.Forms.Shortcut), shortcut.Value);
				}
				catch
				{
				}
			}
			
			if (command != null)
			{
				Type type = Type.GetType(command.Value);
				if (type != null)
				{
					object o;
					try
					{
						o = Activator.CreateInstance(type);
					}
					catch (Exception e)
					{
						o = null;
						System.Diagnostics.Debug.WriteLine("Failed to create type: " + type.AssemblyQualifiedName);
						System.Diagnostics.Debug.WriteLine(e.Message);
					}

					if (o is Command)
					{
						Command cmd = (Command) o;
						if (!cmd.IsPermitted(param == null ? null : param.Value))
							return null;
						MenuCommandHandler mch = new MenuCommandHandler(
							mi, cmd, 
							param == null ? null : param.Value);
						mi.Click += new System.EventHandler(mch.MenuItemClicked);
						if (state != null)
						{
							StateItem si = StateManager.States[state.Value];
							if (si != null)
							{
								si.StateChanged += new System.EventHandler(mch.MenuItemStateChanged);
							}
						}
					}
				}
			}

			// Recursing to sub MenuItem nodes
			XmlNodeList nodeList;
			nodeList = node.SelectNodes("MenuItem");
			string[] arrOfflineCommands=new string[] {"Sportsman.Commands.CloseCommand", 
						"Sportsman.Producer.OpenChampionshipCommand", 
						"Sportsman.Forms.KeyboardForm", 
						"Sport.UI.Display.RestateCommand", 
						"Sportsman.Commands.AboutCommand"};
			foreach (XmlNode n in nodeList)
			{
				MenuItem subMenu = CreateMenuItem(n);
				if (subMenu != null)
				{
					if (!Sport.Core.Session.Connected)
					{
						string strCommand = "";
						string strParam   = "";
						XmlNode nodeCommand=n.Attributes.GetNamedItem("command");
						XmlNode nodeParam=n.Attributes.GetNamedItem("param");
						bool blnValid=false;
						if ((nodeCommand != null)&&(nodeCommand.Value != null))
							strCommand = nodeCommand.Value.ToLower();
						if ((nodeParam != null)&&(nodeParam.Value != null))
							strParam = nodeParam.Value.ToLower();
						if ((strCommand.Length == 0)&&(strParam.Length == 0))
						{
							blnValid = true;
						}
						else
						{
							for (int i=0; i<arrOfflineCommands.Length; i++)
							{
								string strOfflineCommand=arrOfflineCommands[i].ToLower();
								if ((strCommand.IndexOf(strOfflineCommand) >= 0)||
									(strParam.IndexOf(strOfflineCommand) >= 0))
								{
									blnValid = true;
									break;
								}
							}
						}
						subMenu.Enabled = blnValid;
					}
					mi.MenuItems.Add(subMenu);
				}
			}

			// If menu item have no command and no children - don't add it
			if (command == null && mi.MenuItems.Count == 0)
				return null;
			
			return mi;
		}

		// Implements IConfigurationSectionHandler.Create
		public object Create(object parent, object configContext, System.Xml.XmlNode section)
		{
			MainMenu mm = new MainMenu();
			mm.MergeMenu(CreateMenuItem(section));
			
			return mm;
		}
	}
}
