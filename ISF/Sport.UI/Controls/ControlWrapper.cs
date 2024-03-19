using System;
using System.Windows.Forms;

namespace Sport.UI.Controls
{
	/// <summary>
	/// A wrapper for the Control class, for automatic setting of hot keys
	/// </summary>
	public class ControlWrapper : Control
	{
		public ControlWrapper() : base()
		{
			KeyDown += new KeyEventHandler(ControlWrapper_KeyDown);
		}

		private void ControlWrapper_KeyDown(object sender, KeyEventArgs e)
		{
			if (KeyListener.HandleEvent(this, e))
				e.Handled = true;
		}
	}
}
