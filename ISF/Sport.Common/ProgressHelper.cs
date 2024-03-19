using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sport.Common
{
	public class ProgressHelper
	{
		public event System.EventHandler ProgressChanged;
		private int progess = 0;

		public int CurrentProgress
		{
			get
			{
				return progess;
			}
		}

		public bool IsCancelled { get; set; }

		public void AddProgress(int value)
		{
			progess += value;
			if (progess > 100)
				progess = 100;

			if (this.ProgressChanged != null)
				this.ProgressChanged(this, EventArgs.Empty);
		}
	}
}
