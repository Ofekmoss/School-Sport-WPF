using System;
using System.IO;
using System.Xml;

namespace Sport.Common
{
	/// <summary>
	/// Author: Gilad Bauman
	/// Description: Used to create, read and write into a configuration file in XML format.
	/// </summary>
	public class IniFile
	{
		
		private XmlDocument m_doc;
		private String m_filename;
		
		public IniFile()
		{
			m_doc = null;
			m_filename = null;
		}
		
		public IniFile(string filename)
			: this()
		{
			LoadIniFile(filename);
			m_filename = filename;
		}
		
		public string FileName
		{
			get { return m_filename; }
			set { m_filename = value; }
		}
		
		/// <summary>
		/// Create a new configuration file.
		/// </summary>
		public void CreateIniFile(String filename)
		{
			//can't overwrite!
			if (System.IO.File.Exists(filename))
				throw new Exception("can't create ini file: file already exist: "+filename);
			
			m_doc = new XmlDocument();
			m_doc.LoadXml("<Configuration></Configuration>");
			XmlDeclaration dec = m_doc.CreateXmlDeclaration("1.0",null,null);
			XmlElement root = m_doc.DocumentElement;
			m_doc.InsertBefore(dec, root);
			m_filename = filename;
			this.SaveIniFile();
		}
		
		/// <summary>
		/// Load a configuration file into the memory.
		/// </summary>
		public void LoadIniFile(String filename)
		{
			//create if needed:
			if (!System.IO.File.Exists(filename))
			{
				CreateIniFile(filename);
				m_filename = null;
			}
			
			if ((m_filename == null)||(m_filename.ToLower() != filename.ToLower()))
			{
				m_doc = new XmlDocument();
				m_filename = filename;
				XmlTextReader reader = new XmlTextReader(m_filename);
				m_doc.Load(reader);
				reader.Close();
			}
		}
		
		/// <summary>
		/// Save the configuration into the file from which it was opened.
		/// </summary>
		public void SaveIniFile()
		{
			if (!IsFileOpen())
			{
				//Console.WriteLine("There is no open file!");
				throw new Exception("can't save ini file: no path specified.");
			}
			else
			{
				try
				{
					XmlTextWriter writer = new XmlTextWriter(m_filename,null);
					m_doc.Save(writer);
					writer.Close();
				}
				catch (Exception e)
				{
					throw new Exception("failed to save ini file "+this.m_filename+": "+e.Message);
				}
			}
		}

		/// <summary>
		/// Save the configuration into a file of choice.
		/// </summary>
		public void SaveAsIniFile(String filename)
		{
			m_filename = filename;
			XmlTextWriter writer = new XmlTextWriter(m_filename,null);
			m_doc.Save(writer);
			writer.Close();
		}

		/// <summary>
		/// Add a configuration setting into the section of choice.
		/// If section does not exist, create section.
		/// </summary>
		public void WriteValue(String section, String key, String value)
		{
			//load the document:
			this.LoadIniFile(this.m_filename);
			
			if (!IsFileOpen())
			{
				//Console.WriteLine("There is no open file!");
				throw new Exception("can't add ini file value: no path specified.");
			}
			else
			{
				// Find the section into which we want to insert the new value. If it does not exist, create it.
				XmlNode current_section = FindSectionElement(section);
				if (current_section == null)
				{
					current_section = m_doc.CreateElement(section);
					m_doc.DocumentElement.AppendChild(current_section);
				}

				// Append the new data into the section. If the key already exists, the value is replaced with the new value.
				XmlNode current_input = FindSectionElement(key,current_section);
				if (current_input == null)
				{
					current_input = m_doc.CreateElement(key);
					current_section.AppendChild(current_input);
				}
				current_input.InnerText = value;

				//save xml document:
				this.SaveIniFile();
			}
		}
		
		/// <summary>
		/// Returns a value by a given section and key.
		/// </summary>
		public String ReadValue(String section, String key)
		{
			//load the document:
			this.LoadIniFile(this.m_filename);
			
			XmlNode current_section = FindSectionElement(section);
			if (current_section == null)
				return null;
			
			XmlNode current_output = FindSectionElement(key,current_section);
			if (current_output == null)
				return null;
			
			return current_output.InnerText;
		}

		/// <summary>
		/// Check if a file is open for editing.
		/// </summary>
		private bool IsFileOpen()
		{
			return (m_doc != null);
		}

		/// <summary>
		/// Find section element. return null if section does not exist.
		/// </summary>
		private XmlNode FindSectionElement(String section)
		{
			XmlNode current_node = m_doc.DocumentElement.FirstChild;
			
			while (current_node != null)
			{
				if (section.ToLower().Equals(current_node.Name.ToLower()))
					return current_node;
				else
					current_node = current_node.NextSibling;
			}
			return null;
		}
			
		/// <summary>
		/// Find key element inside a given section.
		/// return null if section does not exist.
		/// </summary>
		private XmlNode FindSectionElement(String key,XmlNode section)
		{
			XmlNode current_node = section.FirstChild;
			
			while (current_node != null)
			{
				if (key.ToLower().Equals(current_node.Name.ToLower()))
					return current_node;
				else
					current_node = current_node.NextSibling;
			}
			return null;
		}
	} //end class IniFile
}
