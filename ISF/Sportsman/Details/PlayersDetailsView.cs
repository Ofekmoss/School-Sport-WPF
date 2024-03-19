using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Sport.UI;

namespace Sportsman.Details
{
	/// <summary>
	/// Inherits DetailView for students details
	/// </summary>
	public class PlayersDetailsView : DetailsView
	{
		private System.ComponentModel.Container components = null;

		#region Initialization

		#region Constructor

		public PlayersDetailsView()
		{
			EntityType = Sport.Entities.Player.Type;
		}

		#endregion

		#endregion

		#region View Operations

		public override void Open()
		{
			base.Open();
		}

		#endregion

		#region Dispose

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#endregion
	}
}
