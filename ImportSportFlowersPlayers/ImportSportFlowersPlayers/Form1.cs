using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Configuration;
using System.IO;
using Sport.Common;
using System.Threading;

namespace ImportSportFlowersPlayers
{
	public partial class Form1 : Form
	{
		private int Year = 0;
		public static string DateFormat = "dd/MM/yyyy";

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

		private void Form1_Load(object sender, EventArgs e)
		{
			string errorMsg;
			if (!DatabaseLayer.VerifyDatabaseConnection(out errorMsg))
			{
				MessageBox.Show(errorMsg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				Application.Exit();
			}

			if (!Int32.TryParse(ConfigurationManager.AppSettings["Year"], out Year) || Year <= 0)
			{
				Output("Missing or invalid Year setting");
				return;
			}

			string dateFormat = ConfigurationManager.AppSettings["DateFormat"] + "";
			if (dateFormat.Length > 0)
				DateFormat = dateFormat;

			this.Text += " (Year: " + Year + ", Date format: " + DateFormat + ")";
			cbMunicipalities.Items.Clear();
			cbMunicipalities.Items.Add(new ListBoxItem("Choose municipality...", 0));
			Dictionary<string, int> allMunicipalities = DatabaseLayer.GetAllMunicipalities();
			List<string> arrSortedMunicipalities = allMunicipalities.Keys.ToList();
			arrSortedMunicipalities.Sort();
			arrSortedMunicipalities.ForEach(muniName => cbMunicipalities.Items.Add(new ListBoxItem(muniName, allMunicipalities[muniName])));
			cbMunicipalities.SelectedIndex = 0;
			nudIdNumberIndex.Value = 1;
			nudLastNameIndex.Value = 2;
			nudFirstNameIndex.Value = 3;
			nudGenderIndex.Value = 4;
			nudBirthdateIndex.Value = 5;
			nudIdNumberIndex.Tag = lbIdNumberIndexTitle;
			nudLastNameIndex.Tag = lbLastNameIndexTitle;
			nudFirstNameIndex.Tag = lbFirstNameIndexTitle;
			nudGenderIndex.Tag = lbGenderIndexTitle;
			nudBirthdateIndex.Tag = lbBirthdateIndexTitle;
		}

		private void btnAnalyzeFile_Click(object sender, EventArgs e)
		{
			lbRowProgress.Visible = false;
			string filePath = tbFilePath.Text;
			tbOutput.Lines = new string[] {};
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

			int muniSeq = (int)(cbMunicipalities.Items[cbMunicipalities.SelectedIndex] as ListBoxItem).Value;
			if (muniSeq <= 0)
			{
				MessageBox.Show("Please select municipality", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
				PerformImport(filePath);
			}
			catch (Exception ex)
			{
				Output("General error while importing: " + ex.ToString());	
			}
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

		private void PerformImport(string filePath)
		{
			ListBoxItem selectedMuni = cbMunicipalities.Items[cbMunicipalities.SelectedIndex] as ListBoxItem;
			int muniSeq = (int)selectedMuni.Value;
			Output(string.Format("Importing file '{0}' to municipality '{1}' (seq {2})...", filePath, selectedMuni.Text, muniSeq));
			List<List<string>> excelRows;
			int sheetCount;
			string errorMsg;
			btnAnalyzeFile.Enabled = false;
			Application.DoEvents();
			if (!Tools.TryReadExcelFile(filePath, out excelRows, out sheetCount, out errorMsg))
			{
				btnAnalyzeFile.Enabled = true;
				Output("Error parsing as Excel file: " + errorMsg);
				return;
			}

			if (excelRows == null || excelRows.Count == 0)
			{
				btnAnalyzeFile.Enabled = true;
				Output("File is empty");
				return;
			}

			int totalRowCount = excelRows.Count;
			Output("Total of " + totalRowCount + " rows in Excel file");
			PlayerRowItem currentPlayerItem;
			QueryExecutionResult currentResult;
			int totalUpdated = 0, totalInserted = 0;
			lbRowProgress.Visible = true;
			lbRowProgress.Text = "0/0";
			for (int i = 0; i < totalRowCount; i++)
			{
				lbRowProgress.Text = string.Format("{0}/{1}", i + 1, totalRowCount);
				Output("Importing line #" + (i + 1) + "...");
				if (PlayerRowItem.TryParseRow(excelRows[i], out currentPlayerItem, out errorMsg))
				{
					currentResult = QueryExecutionResult.None;
					try
					{
						DatabaseLayer.ImportSinglePlayer(Year, muniSeq, currentPlayerItem, out currentResult);
					}
					catch (Exception ex)
					{
						Output("Error importing player with id number '" + currentPlayerItem.IdNumber + "' to database: " + ex.ToString());
					}
					switch (currentResult)
					{
						case QueryExecutionResult.Updated:
							Output("Player with id number " + currentPlayerItem.IdNumber + " already exists, details updated successfully");
							totalUpdated++;
							break;
						case QueryExecutionResult.Inserted:
							Output("Player with id number " + currentPlayerItem.IdNumber + " inserted successfully to database");
							totalInserted++;
							break;
					}
					Thread.Sleep(5);
				}
				else
				{
					Output(errorMsg);
				}
			}

			btnAnalyzeFile.Enabled = true;

			if (totalUpdated > 0)
				Output("Updated total of " + totalUpdated + " existing players details in database");

			if (totalInserted > 0)
				Output("Inserted total of " + totalInserted + " new players to database");
		}

		private void Output(string message)
		{
			List<string> lines = tbOutput.Lines.ToList();
			lines.Add(message);
			tbOutput.Lines = lines.ToArray();
			tbOutput.SelectionStart = tbOutput.TextLength;
			tbOutput.ScrollToCaret();
			Application.DoEvents();
		}
	}

	public enum QueryExecutionResult
	{
		None = 0,
		Updated,
		Inserted
	}

	public enum GenderType
	{
		Boys = 1,
		Girls = 2
	}

	public class PlayerRowItem
	{
		public string IdNumber { get; set; }
		public string LastName { get; set; }
		public string FirstName { get; set; }
		public GenderType Gender { get; set; }
		public DateTime Birthdate { get; set; }

		public static bool TryParseRow(List<string> cells, out PlayerRowItem rowItem, out string errorMsg)
		{
			rowItem = null;
			errorMsg = "";
			int maxIndex = new List<int>(new int[] { Indices.IdNumber, Indices.LastName, Indices.FirstName, Indices.Gender, Indices.BirthDate }).Max();
			if (cells.Count < maxIndex)
			{
				errorMsg = "Row contains only " + cells.Count + " cells, expected at least " + (maxIndex + 1);
				return false;
			}
			string rawNumber = cells[Indices.IdNumber].Trim();
			long idNumber;
			if (!Int64.TryParse(rawNumber, out idNumber))
			{
				errorMsg = "Invalid Id Number: " + rawNumber;
				return false;
			}

			GenderType gender;
			string rawGender = cells[Indices.Gender] + "";
			if (!TryParseGender(rawGender, out gender))
			{
				errorMsg = "Unknown gender: " + rawGender;
				return false;
			}

			DateTime birthDate;
			string rawBirthdate = cells[Indices.BirthDate] + "";
			if (!DateTime.TryParseExact(rawBirthdate, Form1.DateFormat, null, System.Globalization.DateTimeStyles.None, out birthDate))
			{
				errorMsg = "Invalid birthdate value: " + rawBirthdate;
				return false;
			}

			string lastName = cells[Indices.LastName] + "", firstName = cells[Indices.FirstName] + "";
			if (lastName.Length == 0 && firstName.Length == 0)
			{
				errorMsg = "Both first name and last name are empty";
				return false;
			}

			rowItem = new PlayerRowItem
			{
				IdNumber = idNumber.ToString().PadLeft(9, '0'), 
				LastName = lastName, 
				FirstName = firstName, 
				Gender = gender, 
				Birthdate = birthDate
			};
			return true;
		}

		private static bool TryParseGender(string rawValue, out GenderType gender)
		{
			gender = GenderType.Boys;
			if (rawValue.Length == 0)
				return false;
			if (rawValue.StartsWith("ז") || rawValue.Equals("בן") || rawValue.Equals("בנים"))
			{
				gender = GenderType.Boys;
				return true;
			}
			if (rawValue.StartsWith("נ") || rawValue.Equals("בת") || rawValue.Equals("בנות"))
			{
				gender = GenderType.Girls;
				return true;
			}
			return false;
		}
	}

	public class Indices
	{
		public static Form1 parent = null;
		public static int IdNumber { get { return (int)parent.nudIdNumberIndex.Value; } }
		public static int LastName { get { return (int)parent.nudLastNameIndex.Value; } }
		public static int FirstName { get { return (int)parent.nudFirstNameIndex.Value; } }
		public static int Gender { get { return (int)parent.nudGenderIndex.Value; } }
		public static int BirthDate { get { return (int)parent.nudBirthdateIndex.Value; } }
	}

	public class ListBoxItem
	{
		public string Text { get; set; }
		public object Value { get; set; }

		public ListBoxItem(string text, object value)
		{
			this.Text = text;
			this.Value = value;
		}

		public ListBoxItem()
			: this("", null)
		{
		}

		public override string ToString()
		{
			return this.Text + "";
		}

		public override int GetHashCode()
		{
			return (this.Value == null) ? 0 : this.Value.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (obj == null || obj is DBNull)
				return false;
			return (obj as ListBoxItem).Value.Equals(this.Value);
		}
	}
}
