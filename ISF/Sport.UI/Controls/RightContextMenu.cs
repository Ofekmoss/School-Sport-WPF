using System;
using System.Windows.Forms;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Sport.UI.Controls
{
	/// <summary>
	/// RightContextMenu inherits ContextMenu and makes
	/// the RightToLeft property control in which side (Left or Right) of
	/// the given point in Show will the menu be tracked.
	/// </summary>
	public class RightContextMenu : ContextMenu
	{
		public RightContextMenu()
		{
		}

		public RightContextMenu(MenuItem[] menuItems)
			: base(menuItems)
		{
		}

		[StructLayout(LayoutKind.Sequential)]
		public class TPMPARAMS
		{
			public int cbSize;
			public int rcExclude_left;
			public int rcExclude_top;
			public int rcExclude_right;
			public int rcExclude_bottom;
			public TPMPARAMS()
			{
				this.cbSize = Marshal.SizeOf(typeof(TPMPARAMS));
			}

		}

		[DllImport("user32.dll", CharSet=CharSet.Auto, ExactSpelling=true)]
		public static extern bool TrackPopupMenuEx(HandleRef hmenu, int fuFlags, int x, int y, HandleRef hwnd, TPMPARAMS tpm);


		private Control sourceControl;

		public new void Show(Control control, Point pos)
		{
			if (control == null)
			{
				throw new ArgumentException("InvalidArgument");
			}
			if (!control.IsHandleCreated || !control.Visible)
			{
				throw new ArgumentException("ContextMenuInvalidParent");
			}
			this.sourceControl = control;
			this.OnPopup(EventArgs.Empty);
			pos = control.PointToScreen(pos);
			TrackPopupMenuEx(new HandleRef(this, base.Handle), 
				RightToLeft == RightToLeft.No ? 0x40 : 0x48,
				pos.X, pos.Y, new HandleRef(control, control.Handle), null);
		}
 


	}
}
