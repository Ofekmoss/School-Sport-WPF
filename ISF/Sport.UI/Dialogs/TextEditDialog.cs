using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Sport.UI.Controls;

namespace Sport.UI.Dialogs
{
	public class TextEditDialog : GenericEditDialog
	{
		public string Value
		{
			get { return (string) Items[0].Value; }
			set { Items[0].Value = value; }
		}
	
		public TextEditDialog(string title, string text)
			: base(title)
		{
			Items.Add(new GenericItem(GenericItemType.Text, text));
		}


		protected override void OnActivated(EventArgs e)
		{
			base.OnActivated (e);

			Items[0].Focus();
		}


		public static bool EditText(string title, ref string text)
		{
			TextEditDialog ted = new TextEditDialog(title, text);

			if (ted.ShowDialog() == DialogResult.OK)
			{
				text = ted.Value;

				return true;
			}

			return false;
		}
	}
}
