//////////////////////////////////////////////////////////////////////////
///	Sport.Core.Configuration reads and stores configuration variables
///	in a local configuration file

using System;
using System.Xml;

namespace Sport.Core
{
	public class Configuration
	{
		private static readonly string configFileName = "Sportsman.cfg";

		private static XmlDocument configDocument = null;

		static Configuration()
		{
			string filePath = Mir.Common.Utils.Instance.MapPath(configFileName);
			configDocument = null;
			if (System.IO.File.Exists(filePath))
			{
				XmlTextReader reader = null;
				try
				{
					reader = new XmlTextReader(filePath);
				}
				catch (Exception e)
				{
					//throw new Exception("Failed to read configuration: " + e.Message);
					System.Diagnostics.Debug.WriteLine("Failed to read configuration: " + e.Message);
					System.Diagnostics.Debug.WriteLine(e.StackTrace);
					reader = null;
				}

				if (reader != null)
				{
					configDocument = new XmlDocument();
					try
					{
						configDocument.Load(reader);
					}
					catch (Exception e)
					{
						//throw new Exception("Failed to load configuration: " + e.Message);
						System.Diagnostics.Debug.WriteLine("Failed to load configuration: " + e.Message);
						System.Diagnostics.Debug.WriteLine(e.StackTrace);
						configDocument = null;
					}
					reader.Close();
				}
			}

			if (configDocument == null)
			{
				configDocument = new XmlDocument();
				configDocument.LoadXml("<Configuration></Configuration>");
				configDocument.InsertBefore(
					configDocument.CreateXmlDeclaration("1.0", null, null),
					configDocument.DocumentElement);
			}
		}

		public static void Save()
		{
			string filePath = Mir.Common.Utils.Instance.MapPath(configFileName);
			try
			{
				XmlTextWriter writer = new XmlTextWriter(filePath, null);
				configDocument.Save(writer);
				writer.Close();
			}
			catch (Exception e)
			{
				//throw new Exception("Failed to save configuration: " + e.Message);
				System.Diagnostics.Debug.WriteLine("Failed to save configuration to '" + filePath + "': " + e.Message);
				System.Diagnostics.Debug.WriteLine(e.StackTrace);
			}
		}

		public static string ReadString(string section, string key)
		{
			XmlNode node = null;
			try
			{
				node = configDocument.DocumentElement.SelectSingleNode(section + "/" + key);
			}
			catch (Exception e)
			{
				//throw new Exception("Failed to save configuration: " + e.Message);
				System.Diagnostics.Debug.WriteLine("Failed to read key '" + key + "' in section '" + section + "': " + e.Message);
				System.Diagnostics.Debug.WriteLine(e.StackTrace);
			}
			return node == null ? null : node.InnerText;
		}

		public static int ReadInt(string section, string key, int defValue)
		{
			string rawValue = ReadString(section, key);
			int value = defValue;
			if (rawValue != null && rawValue.Length > 0)
			{
				if (!Int32.TryParse(rawValue, out value))
				{
					value = defValue;
				}
			}
			return value;
		}

		public static void WriteString(string section, string key, string value)
		{
			XmlNode k = null;
			XmlNode s = configDocument.DocumentElement.SelectSingleNode(section);
			if (s == null)
			{
				s = configDocument.CreateElement(section);
				k = configDocument.CreateElement(key);

				s.AppendChild(k);
				configDocument.DocumentElement.AppendChild(s);
			}
			else
			{
				k = s.SelectSingleNode(key);

				if (k == null)
				{
					k = configDocument.CreateElement(key);
					s.AppendChild(k);
				}
			}

			k.InnerText = value;
			Save();
		}

		private static XmlNode GetCreateNode(XmlNode node, string element)
		{
			if (node == null)
				node = configDocument.DocumentElement;

			XmlNode elementNode = node.SelectSingleNode(element);
			if (elementNode == null)
			{
				elementNode = configDocument.CreateElement(element);
				node.AppendChild(elementNode);
			}

			return elementNode;
		}

		private static XmlNode GetCreateNode(XmlNode node, string element, string name)
		{
			if (node == null)
				node = configDocument.DocumentElement;

			XmlNode elementNode = node.SelectSingleNode(element + "[@name=\"" + name + "\"]");
			if (elementNode == null)
			{
				elementNode = configDocument.CreateElement(element);
				XmlAttribute nameAttribute = configDocument.CreateAttribute("name");
				nameAttribute.Value = name;
				elementNode.Attributes.Append(nameAttribute);
				node.AppendChild(elementNode);
			}

			return elementNode;
		}

		public static Configuration GetConfiguration()
		{
			return new Configuration(configDocument.DocumentElement);
		}

		private XmlNode _node;
		private Configuration(XmlNode node)
		{
			_node = node;
		}

		public string Value
		{
			get { return _node.InnerText; }
			set { _node.InnerText = value; }
		}

		public Configuration GetConfiguration(string configuration)
		{
			XmlNode node = GetCreateNode(_node, configuration);
			return new Configuration(node);
		}

		public Configuration GetConfiguration(string configuration, string name)
		{
			XmlNode node = GetCreateNode(_node, configuration, name);
			return new Configuration(node);
		}

	}
}
