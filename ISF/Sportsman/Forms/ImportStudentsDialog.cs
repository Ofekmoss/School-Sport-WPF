using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace Sportsman.Forms
{
	public partial class ImportStudentsDialog : Form
	{
		List<List<string>> excelRows = null;
		private static string Version = "1.21";
		int idNumberColumn = 0, firstNameColumn = 0, schoolSymbolColumn = 0;
		int lastNameColumn = 0, birthdayColumn = 0, genderColumn = 0;

		public ImportStudentsDialog()
		{
			InitializeComponent();
		}

		private void ImportStudentsDialog_Load(object sender, EventArgs e)
		{
			lbVersion.Text = Version;
		}

		private void btnBrowse_Click(object sender, EventArgs e)
		{
			if (openFileDialog1.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
			{
				txtFilePath.Text = openFileDialog1.FileName;
			}
		}

		private void txtFilePath_TextChanged(object sender, EventArgs e)
		{
			string filePath = txtFilePath.Text.Trim();
			if (!filePath.EndsWith(".xls", StringComparison.CurrentCultureIgnoreCase) && !filePath.EndsWith(".xlsx", StringComparison.CurrentCultureIgnoreCase))
				return;
			
			excelRows = null;

			btnBrowse.Tag = btnBrowse.Text;
			btnBrowse.Text = "Please wait...";
			btnBrowse.Enabled = false;
			ShowProgress("");
			Application.DoEvents();
			try
			{
				LoadExcelFile(filePath);
			}
			catch (Exception ex)
			{
				ShowProgress("General error while loading file: " + ex.ToString());
			}
			btnBrowse.Enabled = true;
			btnBrowse.Text = btnBrowse.Tag.ToString();
		}

		private void LoadExcelFile(string filePath)
		{
			if (!File.Exists(filePath))
			{
				ShowProgress("File '" + filePath + "' does not exist");
				return;
			}

			string error;
			int sheetCount;
			if (!Sport.Common.Tools.TryReadExcelFile(filePath, out excelRows, out sheetCount, out error))
			{
				excelRows = null;
				ShowProgress("Error reading file: " + error);
				return;
			}

			int cellCount = 0;
			if (excelRows != null && excelRows.Count > 0)
			{
				cellCount = excelRows.Max(cells => cells.Count);
				excelRows.RemoveAll(cells => (cells.Count < 6 && cells.Count < cellCount));

				//look for ID number and school symbol columns
				idNumberColumn = -1;
				schoolSymbolColumn = -1;
				for (int i = 0; i < excelRows.Count; i++)
				{
					if (idNumberColumn >= 0 && schoolSymbolColumn >= 0)
						break;
					List<string> cells = excelRows[i];
					int numericCellsCount = 0;
					for (int j = 0; j < cells.Count; j++)
					{
						if (numericCellsCount >= 2)
							break;
						string currentCellValue = cells[j];
						if (Sport.Common.Tools.IsInteger(currentCellValue))
						{
							if (currentCellValue.Length > 6)
							{
								if (idNumberColumn < 0)
								{
									idNumberColumn = j;
								}
							}
							else if (schoolSymbolColumn < 0)
								schoolSymbolColumn = j;
							numericCellsCount++;
						}
					}
				}
				if (idNumberColumn >= 0 && schoolSymbolColumn >= 0)
				{
					excelRows.RemoveAll(cells => !Sport.Common.Tools.IsInteger(cells[idNumberColumn]) || !Sport.Common.Tools.IsInteger(cells[schoolSymbolColumn]));
					nudIdColumn.Value = idNumberColumn;
					nudLastNameColumn.Value = idNumberColumn + 1;
					nudFirstNameColumn.Value = idNumberColumn + 2;
					nudBirthdayColumn.Value = idNumberColumn + 3;
					nudSchoolColumn.Value = idNumberColumn + 4;
					nudGenderColumn.Value = idNumberColumn + 5;
				}
			}

			if (excelRows == null || excelRows.Count == 0)
			{
				excelRows = null;
				ShowProgress("File '" + filePath + "' contains no valid rows");
				return;
			}

			string message = string.Format("File contains {0} valid rows with {1} columns",
				excelRows.Count, cellCount);
			if (sheetCount > 1)
				message += ", in " + sheetCount + " sheets";
			ShowProgress(message);
		}

		private void ShowProgress(string message)
		{
			txtProgress.Text = message;
		}

		private void ShowProgress(string[] lines)
		{
			txtProgress.Lines = lines;
		}

		private bool GetAndValidateColumn(NumericUpDown nudValue, bool required, out int column)
		{
			column = (int)nudValue.Value;
			string title = GetProperCaption(nudValue);
			if (required && column < 0)
			{
				MessageBox.Show(this, title + " is mandatory", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return false;
			}

			int cellCount = excelRows.Max(cells => cells.Count);
			if (column >= 0 && column >= cellCount)
			{
				MessageBox.Show(this, title + " value must be between 0 and " + (cellCount - 1), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return false;
			}

			return true;
		}

		private string GetProperCaption(NumericUpDown nudControl)
		{
			if ((nudControl.Tag + "").Length > 0)
			{
				Control[] matchingLabels = gbColumns.Controls.Find(nudControl.Tag.ToString(), true);
				if (matchingLabels != null && matchingLabels.Length > 0)
					return ((Label)matchingLabels[0]).Text;
			}
			return nudControl.Name;
		}

		private void btnStartImport_Click(object sender, EventArgs e)
		{
			if (excelRows == null || excelRows.Count == 0)
			{
				MessageBox.Show(this, "No valid Excel file has been selected", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			btnStartImport.Tag = btnStartImport.Text;
			btnStartImport.Text = "Please wait...";
			btnStartImport.Enabled = false;
			ShowProgress("");
			Application.DoEvents();
			try
			{
				PerformImport();
			}
			catch (Exception ex)
			{
				ShowProgress("General error while importing: " + ex.ToString());
			}
			btnStartImport.Enabled = true;
			btnStartImport.Text = btnStartImport.Tag.ToString();
		}

		private void PerformImport()
		{
			//mandatory: school, grade, first name, id number

			if (!GetAndValidateColumn(nudIdColumn, true, out idNumberColumn) ||
				!GetAndValidateColumn(nudFirstNameColumn, true, out firstNameColumn) ||
				!GetAndValidateColumn(nudSchoolColumn, true, out schoolSymbolColumn) ||
				!GetAndValidateColumn(nudLastNameColumn, false, out lastNameColumn) ||
				!GetAndValidateColumn(nudBirthdayColumn, false, out birthdayColumn) ||
				!GetAndValidateColumn(nudGenderColumn, false, out genderColumn))
				return;

			Dictionary<int, List<string>> columnMapping = new Dictionary<int, List<string>>();
			gbColumns.Controls.OfType<NumericUpDown>().ToList().FindAll(n => (n.Tag + "").Length > 0).ForEach(n =>
			{
				Control[] matchingLabels = gbColumns.Controls.Find(n.Tag.ToString(), true);
				if (matchingLabels != null && matchingLabels.Length > 0)
				{
					Label label = (Label)matchingLabels[0];
					int currentIndex = (int)n.Value;
					if (!columnMapping.ContainsKey(currentIndex))
						columnMapping.Add(currentIndex, new List<string>());
					columnMapping[currentIndex].Add(label.Text);
				}
			});

			List<string> invalidValues = columnMapping.Keys.ToList().FindAll(index => index >= 0 && columnMapping[index].Count > 1).ConvertAll(index => string.Join(", ", columnMapping[index]));
			if (invalidValues.Count > 0)
			{
				MessageBox.Show(this, "The following have the same index which is not allowed: " + string.Join("\n", invalidValues), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			string separatorLine = "-----------------------------";
			Dictionary<string, string> alreadyExists = new Dictionary<string, string>();
			List<string> generalErrors = new List<string>();
			Dictionary<string, Sport.Entities.School> schoolMapping = new Dictionary<string, Sport.Entities.School>();
			List<Sport.Common.StudentData> students = TranslateRawData(excelRows, alreadyExists, generalErrors, schoolMapping);
			List<string> outputLines = new List<string>();
			if (alreadyExists.Count > 0)
			{
				outputLines.Add(alreadyExists.Count + " student(s) already exist:");
				outputLines.AddRange(alreadyExists.Keys.ToList().ConvertAll(idNumber => string.Format("Student {0} - exists in '{1}'", idNumber, alreadyExists[idNumber])));
				outputLines.Add(separatorLine);
			}
			if (generalErrors.Count > 0)
			{
				outputLines.Add(generalErrors.Count + " invalid row(s) found:");
				outputLines.AddRange(generalErrors);
				outputLines.Add(separatorLine);
			}
			if (students.Count > 0)
			{
				outputLines.Add("Importing " + students.Count + " student(s)...");
				string invalidIdReason;
				students.ForEach(currentStudentData =>
				{
					string currentIdNumber = currentStudentData.IdNumber;
					if (!Sport.Common.Tools.IsValidIdNumber(currentIdNumber, out invalidIdReason))
					{
						outputLines.Add("Id number '" + currentIdNumber + "' is not valid (" + invalidIdReason + ")");
					}
					else
					{
						Sport.Entities.School currentSchool = schoolMapping[currentStudentData.School.Symbol];
						DateTime currentBirthday = currentStudentData.Birthdate;
						Sport.Data.EntityEdit entityEdit = new Sport.Data.EntityEdit(Sport.Entities.Student.Type);
						Sport.Entities.Student studentEntity = new Sport.Entities.Student(entityEdit);
						studentEntity.Entity.Fields[(int)Sport.Entities.Student.Fields.School] = currentSchool.Id;
						studentEntity.Entity.Fields[(int)Sport.Entities.Student.Fields.IdNumber] = currentIdNumber;
						studentEntity.Entity.Fields[(int)Sport.Entities.Student.Fields.FirstName] = currentStudentData.FirstName;
						studentEntity.Entity.Fields[(int)Sport.Entities.Student.Fields.LastName] = currentStudentData.LastName;
						studentEntity.Entity.Fields[(int)Sport.Entities.Student.Fields.Grade] = currentStudentData.Grade;
						studentEntity.Entity.Fields[(int)Sport.Entities.Student.Fields.SexType] = currentStudentData.SexType;
						if (currentBirthday.Year >= 1900)
							studentEntity.Entity.Fields[(int)Sport.Entities.Student.Fields.BirthDate] = currentBirthday;
						studentEntity.Entity.Fields[(int)Sport.Entities.Student.Fields.IdNumberType] = 0;
						studentEntity.Entity.Fields[(int)Sport.Entities.Student.Fields.LastModified] = DateTime.Now;
						Sport.Data.EntityResult result = null;
						bool success = false;
						string error = "";
						try
						{
							result = studentEntity.Entity.EntityType.Save(studentEntity.Entity as Sport.Data.EntityEdit);
							success = true;
						}
						catch (Exception ex)
						{
							error = ex.Message;
						}

						if (success && result.Succeeded)
							outputLines.Add("Student " + currentIdNumber + " has been added successfully to '" + currentSchool.Name + "'");
						else if (error.Length > 0)
							outputLines.Add("Error saving new student " + currentIdNumber + ": " + error);
						else if (result.Succeeded == false)
							outputLines.Add("Failed saving new student " + currentIdNumber + ": " + result.GetMessage());
						else
							outputLines.Add("Unspecified error saving student " + currentIdNumber);
					}
				});
			}
			else
			{
				outputLines.Add("No students to import");
			}
			ShowProgress(outputLines.ToArray());
		}

		private List<Sport.Common.StudentData> TranslateRawData(List<List<string>> excelRows,
			Dictionary<string, string> alreadyExists, List<string> generalErrors, 
			Dictionary<string, Sport.Entities.School> schoolMapping)
		{
			alreadyExists.Clear();
			generalErrors.Clear();
			schoolMapping.Clear();
			
			List<Sport.Common.StudentData> students = new List<Sport.Common.StudentData>();
			DataServices.DataService service = new DataServices.DataService();
			string currentStudentIdNumber;
			List<long> handledIdNumbers = new List<long>();
			string dateFormat = "dd/MM/yyyy HH:mm:ss";
			Sport.Types.SexTypeLookup genderLookup = new Sport.Types.SexTypeLookup();
			excelRows.ForEach(row =>
			{
				if (TryGetNumber(row[idNumberColumn], out currentStudentIdNumber))
				{
					if (handledIdNumbers.Contains(Int64.Parse(currentStudentIdNumber)))
					{
						//silently ignore
					}
					else
					{
						DataServices.StudentData existingStudent = service.GetStudentByNumber(currentStudentIdNumber);
						if (existingStudent == null || existingStudent.ID <= 0 || string.IsNullOrEmpty(existingStudent.IdNumber))
						{
							string currentSchoolSymbol = row[schoolSymbolColumn];
							if (currentSchoolSymbol.Length > 0)
							{
								if (ValidateSchool(currentSchoolSymbol, schoolMapping))
								{
									DateTime currentBirthday = GetStudentBirthday(row, dateFormat);
									Sport.Types.Sex currentGender = GetStudentGender(row);
									students.Add(new Sport.Common.StudentData
									{
										IdNumber = currentStudentIdNumber,
										Birthdate = currentBirthday,
										FirstName = row[firstNameColumn],
										LastName = row[lastNameColumn] + "",
										Grade = 0,
										School = new Sport.Common.SchoolData
										{
											Symbol = currentSchoolSymbol
										}, 
										SexType = (int)currentGender
									});
								}
								else
								{
									generalErrors.Add("School '" + currentSchoolSymbol + "' for student " + currentStudentIdNumber + " does not exist");
								}
							}
							else
							{
								generalErrors.Add("Missing school symbol for student ID " + currentStudentIdNumber);
							}
						}
						else
						{
							string schoolName = "";
							if (existingStudent.School != null)
							{
								string existingSchoolSymbol = existingStudent.School.Symbol;
								if (!schoolMapping.ContainsKey(existingSchoolSymbol))
									schoolMapping.Add(existingSchoolSymbol, Sport.Entities.School.FromSymbol(existingSchoolSymbol));
								schoolName = schoolMapping[existingSchoolSymbol].Name;
							}
							alreadyExists.Add(currentStudentIdNumber, schoolName);
						}
						handledIdNumbers.Add(Int64.Parse(currentStudentIdNumber));
					}
				}
				else
				{
					generalErrors.Add("Invalid id number: " + row[idNumberColumn]);
				}
			});
			
			return students;
		}

		private Sport.Types.Sex GetStudentGender(List<string> excelRow)
		{
			Sport.Types.Sex studentGender = Sport.Types.Sex.None;
			if (genderColumn >= 0)
			{
				string rawGender = excelRow[genderColumn];
				if (rawGender.Contains("ז"))
					studentGender = Sport.Types.Sex.Boys;
				else if (rawGender.Contains("נ"))
					studentGender = Sport.Types.Sex.Girls;
			}
			
			return studentGender;
		}

		private DateTime GetStudentBirthday(List<string> excelRow, string dateFormat)
		{
			if (birthdayColumn >= 0)
			{
				string rawBirthday = excelRow[birthdayColumn].Trim();
				if (rawBirthday.Length > 0 && rawBirthday.IndexOf(".") > 0)
				{
					string[] temp = rawBirthday.Split('.');
					if (temp.Length == 3)
					{
						int day, month, year;
						if (Int32.TryParse(temp[0], out day) && day > 0 && day <= 31 &&
							Int32.TryParse(temp[1], out month) && month > 0 && month <= 12 &&
							Int32.TryParse(temp[2], out year) && year > 1900)
							return new DateTime(year, month, day);
					}
				}
				DateTime dtBirthday;
				if (DateTime.TryParseExact(rawBirthday, dateFormat, null, System.Globalization.DateTimeStyles.None, out dtBirthday))
					return dtBirthday.Date;
			}
			
			return DateTime.MinValue;
		}

		private bool ValidateSchool(string symbol, Dictionary<string, Sport.Entities.School> mapping)
		{
			if (!mapping.ContainsKey(symbol))
			{
				Sport.Entities.School school = null;
				try
				{
					school = Sport.Entities.School.FromSymbol(symbol);
				}
				catch
				{
					school = null;
				}
				mapping.Add(symbol, school);
			}

			return mapping[symbol] != null;
		}

		private bool TryGetNumber(string rawValue, out string number)
		{
			number = "";
			if (rawValue.Length == 0)
				return false;
			long temp;
			if (!Int64.TryParse(rawValue, out temp))
				return false;
			number = temp.ToString();
			return true;
		}

		private void btnOpenInExcel_Click(object sender, EventArgs e)
		{
			string filePath = txtFilePath.Text;
			if (filePath.Length == 0)
			{
				MessageBox.Show(this, "No file selected", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			if (!File.Exists(filePath))
			{
				MessageBox.Show(this, "File '" + filePath + "' does not exist", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			Process.Start(filePath);
		}
	}
}
