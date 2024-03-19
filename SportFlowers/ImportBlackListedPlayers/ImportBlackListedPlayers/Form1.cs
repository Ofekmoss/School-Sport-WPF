using Sport.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace ImportBlackListedPlayers
{
	public partial class Form1 : Form
	{
		private bool _stopImport = false;
		private List<PlayerRowItem> _allRows = new List<PlayerRowItem>();

		public Form1()
		{
			Indices.parent = this;
			InitializeComponent();
		}

		private void btnBrowse_Click(object sender, EventArgs e)
		{
			if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				tbFilePath.Text = openFileDialog1.FileName;
			}
		}

		private void btnAnalyzeFile_Click(object sender, EventArgs e)
		{
			_stopImport = false;
			lbRowProgress.Visible = false;
			nudImportFirstIndex.Maximum = 0;
			nudImportFirstIndex.Value = 0;
			nudImportLastIndex.Maximum = 0;
			nudImportLastIndex.Value = 0;
			string filePath = tbFilePath.Text;
			tbOutput.Lines = new string[] { };
			if (filePath.Length == 0)
			{
				MessageBox.Show("Must provide file path", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			if (!File.Exists(filePath))
			{
				MessageBox.Show("File '" + filePath + "' does not exist", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			string[] sameIndexElements;
			if (!ValidateIndices(out sameIndexElements))
			{
				MessageBox.Show(string.Join(" and ", sameIndexElements) + " can't have the same index", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			try
			{
				AnalyzeFile(filePath);
			}
			catch (Exception ex)
			{
				Output("General error while analyzing file: " + ex.ToString());
			}

			nudImportFirstIndex.Maximum = _allRows.Count;
			if (_allRows.Count > 0)
				nudImportFirstIndex.Value = 1;
			nudImportLastIndex.Maximum = _allRows.Count;
			nudImportLastIndex.Value = _allRows.Count;
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			string errorMsg;
			if (!DatabaseLayer.VerifyDatabaseConnection(out errorMsg))
			{
				MessageBox.Show(errorMsg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				Application.Exit();
			}
			
			nudLastNameIndex.Value = -1;
			nudIdNumberIndex.Value = 0;
			nudFullNameIndex.Value = 1;
			nudSportNameIndex.Value = 2;
			nudLastNameIndex.Tag = lbLastNameIndexTitle;
			nudIdNumberIndex.Tag = lbIdNumberIndexTitle;
			nudFullNameIndex.Tag = lbFullNameIndexTitle;
			nudSportNameIndex.Tag = lbSportNamendexTitle;
		}

		private bool ValidateIndices(out string[] sameIndexElements)
		{
			sameIndexElements = null;
			Dictionary<int, string> mapping = new Dictionary<int, string>();
			List<NumericUpDown> elements = pnlIndices.Controls.OfType<NumericUpDown>().ToList();
			for (int i = 0; i < elements.Count; i++)
			{
				NumericUpDown currentElement = elements[i];
				int currentIndex = (int)currentElement.Value;
				string currentTitle = (currentElement.Tag as Label).Text;
				if (mapping.ContainsKey(currentIndex))
				{
					sameIndexElements = new string[] { mapping[currentIndex],  currentTitle };
					return false;
				}
				mapping.Add(currentIndex, currentTitle);
			}
			return true;
		}

		private void AnalyzeFile(string filePath)
		{
			Output(string.Format("Analyzing file '{0}'...", filePath));
			List<List<string>> excelRows;
			int sheetCount;
			string errorMsg;
			btnAnalyzeFile.Enabled = false;
			btnStartImport.Enabled = false;
			Application.DoEvents();
			if (!Tools.TryReadExcelFile(filePath, out excelRows, out sheetCount, out errorMsg))
			{
				btnAnalyzeFile.Enabled = true;
				btnStartImport.Enabled = true;
				Output("Error parsing as Excel file: " + errorMsg);
				return;
			}

			if (excelRows == null || excelRows.Count == 0)
			{
				btnAnalyzeFile.Enabled = true;
				btnStartImport.Enabled = true;
				Output("File is empty");
				return;
			}

			_allRows.Clear();
			int totalRowCount = excelRows.Count;
			Output("Total of " + totalRowCount + " rows in Excel file");
			PlayerRowItem currentPlayerItem;
			lbRowProgress.Visible = true;
			lbRowProgress.Text = "0/0";
			Dictionary<int, bool> existingPlayers = new Dictionary<int, bool>();
			List<string> arrOutputLines = new List<string>();
			bool silent = chkSilent.Checked;
			for (int i = 1; i < totalRowCount; i++)
			{
				if (_stopImport)
				{
					Output("Stopped by user");
					break;
				}

				lbRowProgress.Text = string.Format("{0}/{1}", i + 1, totalRowCount);
				ConditionalOutput(silent, "Analyzing line #" + (i + 1) + "...", arrOutputLines);
				if (PlayerRowItem.TryParseRow(excelRows[i], out currentPlayerItem, out errorMsg))
				{
					int idNumber = Int32.Parse(currentPlayerItem.IdNumber);
					if (existingPlayers.ContainsKey(idNumber))
					{
						ConditionalOutput(silent, string.Format("Player with id number {0} already added to list, ignoring", 
							currentPlayerItem.IdNumber), arrOutputLines);
					}
					else
					{
						_allRows.Add(currentPlayerItem);
						existingPlayers.Add(idNumber, true);
						ConditionalOutput(silent, string.Format("Player \"{0}\" (id number {1}) is valid and has been added to the list.", 
							currentPlayerItem.FullName, currentPlayerItem.IdNumber), arrOutputLines);
					}
				}
				else
				{
					ConditionalOutput(silent, errorMsg, arrOutputLines);
				}
			}

			if (arrOutputLines.Count > 0)
				Output(arrOutputLines);

			if (_allRows.Count == 0)
				Output("No valid lines have been found");
			else
				Output("Found total of " + _allRows.Count + " valid lines");
			btnAnalyzeFile.Enabled = true;
			btnStartImport.Enabled = true;
		}

		private void PerformImport(int firstIndex, int lastIndex)
		{
			Output(string.Format("Importing lines #{0} to #{1}...", firstIndex + 1, lastIndex + 1));
			btnAnalyzeFile.Enabled = false;
			btnStartImport.Enabled = false;
			QueryExecutionResult currentResult;
			int totalUpdated = 0, totalInserted = 0;
			lbRowProgress.Visible = true;
			lbRowProgress.Text = "0/0";
			int count = (lastIndex - firstIndex) + 1;
			PlayerRowItem currentPlayerItem;
			List<string> arrOutputLines = new List<string>();
			bool silent = chkSilent.Checked;
			for (int i = firstIndex; i <= lastIndex; i++)
			{
				if (_stopImport)
				{
					Output("Stopped by user");
					break;
				}

				lbRowProgress.Text = string.Format("{0}/{1}", (i - firstIndex) + 1, count);
				Application.DoEvents();
				ConditionalOutput(silent, "Importing line #" + (i + 1) + "...", arrOutputLines);
				currentPlayerItem = _allRows[i];
				currentResult = QueryExecutionResult.None;
				try
				{
					DatabaseLayer.ImportSinglePlayer(currentPlayerItem, out currentResult);
				}
				catch (Exception ex)
				{
					ConditionalOutput(silent, string.Format("Error importing player with id number '{0}' to database: {1}", 
						currentPlayerItem.IdNumber,  ex.ToString()), arrOutputLines);
				}
				string currentMessage = "";
				switch (currentResult)
				{
					case QueryExecutionResult.Updated:
						currentMessage = "Player with id number {0} already exists, details updated successfully";
						totalUpdated++;
						break;
					case QueryExecutionResult.Inserted:
						currentMessage = "Player with id number {0} inserted successfully to database";
						totalInserted++;
						break;
				}
				if (currentMessage.Length > 0)
				{
					ConditionalOutput(silent, string.Format(currentMessage, currentPlayerItem.IdNumber), arrOutputLines);
					Thread.Sleep(5);
				}
			}
			btnStartImport.Enabled = true;
			btnAnalyzeFile.Enabled = true;

			if (arrOutputLines.Count > 0)
				Output(arrOutputLines);

			if (totalUpdated > 0)
				Output("Updated total of " + totalUpdated + " existing players details in database");

			if (totalInserted > 0)
				Output("Inserted total of " + totalInserted + " new players to database");
		}

		private void Output(List<string> messages)
		{
			List<string> lines = tbOutput.Lines.ToList();
			lines.AddRange(messages);
			tbOutput.Lines = lines.ToArray();
			if (messages.Count == 1)
			{
				tbOutput.SelectionStart = tbOutput.TextLength;
				tbOutput.ScrollToCaret();
			}
			Application.DoEvents();
		}

		private void ConditionalOutput(bool silent, string message, List<string> buffer)
		{
			if (silent)
				buffer.Add(message);
			else
				Output(message);
		}

		private void Output(string message)
		{
			Output(new List<string>(new string[] { message }));
		}

		private void btnStopImport_Click(object sender, EventArgs e)
		{
			_stopImport = true;
		}

		private void btnStartImport_Click(object sender, EventArgs e)
		{
			if (_allRows.Count == 0)
			{
				MessageBox.Show("No valid lines were found", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			int firstIndex = (int)nudImportFirstIndex.Value - 1;
			int lastIndex = (int)nudImportLastIndex.Value - 1;
			if (firstIndex < 0)
				firstIndex = 0;
			if (lastIndex >= _allRows.Count)
				lastIndex = _allRows.Count - 1;
			if ((lastIndex - firstIndex) < 0)
			{
				MessageBox.Show("No lines were selected for import", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			_stopImport = false;
			try
			{
				PerformImport(firstIndex, lastIndex);
			}
			catch (Exception ex)
			{
				Output("General error while importing: " + ex.ToString());
			}
		}
	}

	public enum QueryExecutionResult
	{
		None = 0,
		Updated,
		Inserted
	}

	public class Indices
	{
		public static Form1 parent = null;
		public static int LastName { get { return (int)parent.nudLastNameIndex.Value; } }
		public static int IdNumber { get { return (int)parent.nudIdNumberIndex.Value; } }
		public static int FullName { get { return (int)parent.nudFullNameIndex.Value; } }
		public static int SportName { get { return (int)parent.nudSportNameIndex.Value; } }
	}

	public class PlayerRowItem
	{
		public string IdNumber { get; set; }
		public string FullName { get; set; }
		public string SportName { get; set; }

		public static bool TryParseRow(List<string> cells, out PlayerRowItem rowItem, out string errorMsg)
		{
			rowItem = null;
			errorMsg = "";
			int maxIndex = new List<int>(new int[] { Indices.IdNumber, Indices.FullName, Indices.SportName, Indices.LastName }).Max();
			if (cells.Count < maxIndex)
			{
				errorMsg = "Row contains only " + cells.Count + " cells, expected at least " + (maxIndex + 1);
				return false;
			}
			string rawNumber = cells[Indices.IdNumber].Replace("-", "").Replace(".", "").Replace(" ", "").Trim();
			long idNumber;
			if (!Int64.TryParse(rawNumber, out idNumber))
			{
				errorMsg = "Invalid Id Number: " + rawNumber;
				return false;
			}

			string fullName = cells[Indices.FullName] + "";
			if (fullName.Length == 0)
			{
				errorMsg = "Full name is empty";
				return false;
			}

			int lastNameIndex = Indices.LastName;
			if (lastNameIndex >= 0)
			{
				string lastName = (cells[lastNameIndex] + "").Trim();
				if (lastName.Length > 0 && !lastName.Equals(fullName, StringComparison.CurrentCultureIgnoreCase))
					fullName += " " + lastName;
			}

			string sportName = cells[Indices.SportName] + "";
			rowItem = new PlayerRowItem
			{
				IdNumber = idNumber.ToString().PadLeft(9, '0'),
				FullName = fullName,
				SportName = sportName
			};
			return true;
		}
	}
}
