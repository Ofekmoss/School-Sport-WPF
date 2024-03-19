using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing.Printing;
using Sport.UI.Dialogs;

namespace Sport.UI
{
	public static class Helpers
	{
		public static bool GetPrinterSettings(out PrinterSettings ps, out PrintSettingsForm settingsForm, bool checkInstalledPrinters, double? overrideWidth_cm, double? overrideHeight_cm)
		{
			//settings:
			ps = new PrinterSettings();
			settingsForm = null;

			//check if there are any printers installed?
			if (checkInstalledPrinters && Sport.Core.Utils.GetInstalledPrinters().Count == 0)
			{
				Sport.UI.MessageBox.Show("לא נמצאה מדפסת ברירת מחדל - אנא בדוק הגדרות הדפסה",
					"שגיאה", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
				return false;
			}

			settingsForm = new Sport.UI.Dialogs.PrintSettingsForm(ps, overrideWidth_cm, overrideHeight_cm);
			//ps.DefaultPageSettings.Margins = new Margins(10, 10, 10, 10);
			return true;
		}

		public static bool GetPrinterSettings(out PrinterSettings ps, out PrintSettingsForm settingsForm, bool checkInstalledPrinters)
		{
			return GetPrinterSettings(out ps, out settingsForm, checkInstalledPrinters, null, null);
		}

		public static bool GetPrinterSettings(out PrinterSettings ps, out PrintSettingsForm settingsForm, double? overrideWidth_cm, double? overrideHeight_cm)
		{
			return GetPrinterSettings(out ps, out settingsForm, true, overrideWidth_cm, overrideHeight_cm);
		}

		public static bool GetPrinterSettings(out PrinterSettings ps, out PrintSettingsForm settingsForm)
		{
			return GetPrinterSettings(out ps, out settingsForm, null, null);
		}
	}
}
