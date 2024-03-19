using System;
using System.Windows.Forms;

namespace Sport.UI.Controls
{
    /// <summary>
    /// Inheriting StatusBar to enable RightToLeft
    /// </summary>
	public class RightStatusBar : StatusBar
	{
		protected override CreateParams CreateParams
		{
			get
			{
				CreateParams p = base.CreateParams;

				if (RightToLeft == RightToLeft.Yes)
				{
					p.ExStyle |= 0x400000;
					p.ExStyle &= -12289;
				}

				return p;
			}
		}
	}

	/// <summary>
	/// Inheriting ToolBar to enable RightToLeft
	/// </summary>
	public class RightToolBar : ToolBar
	{
		protected override CreateParams CreateParams
		{
			get
			{
				CreateParams p = base.CreateParams;

				if (RightToLeft == RightToLeft.Yes)
				{
					p.ExStyle |= 0x400000;
					p.ExStyle &= -12289;
				}

				return p;
			}
		}
	}

	/// <summary>
	/// Inheriting TabControl to enable RightToLeft
	/// </summary>
	public class RightTabControl : TabControl
	{
		protected override CreateParams CreateParams
		{
			get
			{
				CreateParams p = base.CreateParams;

				if (RightToLeft == RightToLeft.Yes)
					p.ExStyle = p.ExStyle | 0x400000 | 0x100000;
				return p;
			}
		}
	}
}
