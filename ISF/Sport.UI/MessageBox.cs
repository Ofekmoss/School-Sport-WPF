using System;

namespace Sport.UI
{
	public enum MessageBoxReturnType 
	{
		Yes = 0,
		No,
		Cancel
	}

	public class MessageBox
	{
		public static string DefaultTitle = "сфешисоп";

		public static System.Windows.Forms.DialogResult Show(string text, string caption, 
			System.Windows.Forms.MessageBoxButtons buttons, 
			System.Windows.Forms.MessageBoxIcon icon, 
			System.Windows.Forms.MessageBoxDefaultButton defaultButton)
		{
			return System.Windows.Forms.MessageBox.Show(text, caption, buttons, icon, defaultButton, 
				System.Windows.Forms.MessageBoxOptions.RtlReading | System.Windows.Forms.MessageBoxOptions.RightAlign);
		}

		public static System.Windows.Forms.DialogResult Show(string text, string caption, 
			System.Windows.Forms.MessageBoxButtons buttons, 
			System.Windows.Forms.MessageBoxIcon icon)
		{
			return Show(text, caption, buttons, icon,
				System.Windows.Forms.MessageBoxDefaultButton.Button1);
		}
			
		public static System.Windows.Forms.DialogResult Show(string text, string caption, 
			System.Windows.Forms.MessageBoxButtons buttons)
		{
			return Show(text, caption, buttons, 
				System.Windows.Forms.MessageBoxIcon.None);
		}

		public static System.Windows.Forms.DialogResult Show(string text,
			System.Windows.Forms.MessageBoxButtons buttons, 
			System.Windows.Forms.MessageBoxIcon icon, 
			System.Windows.Forms.MessageBoxDefaultButton defaultButton)
		{
			return Show(text, DefaultTitle, buttons, icon, defaultButton);
		}

		public static System.Windows.Forms.DialogResult Show(string text,
			System.Windows.Forms.MessageBoxButtons buttons, 
			System.Windows.Forms.MessageBoxIcon icon)
		{
			return Show(text, DefaultTitle, buttons, icon);
		}
			
		public static System.Windows.Forms.DialogResult Show(string text,
			System.Windows.Forms.MessageBoxButtons buttons)
		{
			return Show(text, DefaultTitle, buttons);
		}
			
		public static void Show(string text, string caption,
			System.Windows.Forms.MessageBoxIcon icon)
		{
			Show(text, caption, System.Windows.Forms.MessageBoxButtons.OK,
				icon, System.Windows.Forms.MessageBoxDefaultButton.Button1);
		}

		public static void Show(string text, System.Windows.Forms.MessageBoxIcon icon)
		{
			Show(text, DefaultTitle, System.Windows.Forms.MessageBoxButtons.OK,
				icon, System.Windows.Forms.MessageBoxDefaultButton.Button1);
		}

		public static void Show(string text, string caption)
		{
			Show(text, caption, System.Windows.Forms.MessageBoxButtons.OK,
				System.Windows.Forms.MessageBoxIcon.None, 
				System.Windows.Forms.MessageBoxDefaultButton.Button1);
		}

		public static void Show(string text)
		{
			Show(text, DefaultTitle);
		}

		public static bool Ask(string text, string caption,
			System.Windows.Forms.MessageBoxIcon icon, bool def)
		{
			return Show(text, caption, System.Windows.Forms.MessageBoxButtons.YesNo,
				icon, 
				def ? System.Windows.Forms.MessageBoxDefaultButton.Button1 : System.Windows.Forms.MessageBoxDefaultButton.Button2) ==
				System.Windows.Forms.DialogResult.Yes;
		}

		public static bool Ask(string text, 
			System.Windows.Forms.MessageBoxIcon icon, bool def)
		{
			return Ask(text, DefaultTitle, icon, def);
		}

		public static bool Ask(string text, string caption, bool def)
		{
			return Ask(text, caption, System.Windows.Forms.MessageBoxIcon.Question, def);
		}

		public static bool Ask(string text, bool def)
		{
			return Ask(text, System.Windows.Forms.MessageBoxIcon.Question, def);
		}

		public static MessageBoxReturnType AskYesNoCancel(string text,MessageBoxReturnType def)
		{
			System.Windows.Forms.DialogResult result = System.Windows.Forms.DialogResult.Cancel;
			switch(def)
			{
				case MessageBoxReturnType.Yes:
					result = Show(text,System.Windows.Forms.MessageBoxButtons.YesNoCancel,System.Windows.Forms.MessageBoxIcon.Question,System.Windows.Forms.MessageBoxDefaultButton.Button1);
					break;
				case MessageBoxReturnType.No:
					result = Show(text,System.Windows.Forms.MessageBoxButtons.YesNoCancel,System.Windows.Forms.MessageBoxIcon.Question,System.Windows.Forms.MessageBoxDefaultButton.Button2);
					break;
				case MessageBoxReturnType.Cancel:
					result = Show(text,System.Windows.Forms.MessageBoxButtons.YesNoCancel,System.Windows.Forms.MessageBoxIcon.Question,System.Windows.Forms.MessageBoxDefaultButton.Button3);
					break;
			}
			switch(result)
			{
				case System.Windows.Forms.DialogResult.Yes:
					return MessageBoxReturnType.Yes;
					
				case System.Windows.Forms.DialogResult.No:
					return MessageBoxReturnType.No;
					
				case System.Windows.Forms.DialogResult.Cancel:
					return MessageBoxReturnType.Cancel;
				default:
					return MessageBoxReturnType.Cancel;
					
			}
				
		}

		public static void Warn(string text, string caption)
		{
			Show(text, caption, System.Windows.Forms.MessageBoxIcon.Warning);
		}

		public static void Error(string text, string caption)
		{
			Show(text, caption, System.Windows.Forms.MessageBoxIcon.Error);
		}

		public static void Success(string text, string caption)
		{
			Show(text, caption, System.Windows.Forms.MessageBoxIcon.Information);
		}
	}
}
