using System;
using System.Collections;

namespace Sport.UI.Display
{
	public class StateItem
	{
		protected bool _checked;
		protected bool _enabled;
		protected bool _visible;

		public bool Checked
		{
			get { return _checked; }
			set 
			{ 
				if (value != _checked) 
				{
					_checked = value;

					if (StateChanged != null)
						StateChanged(this, EventArgs.Empty);
				}
			}
		}

		public bool Enabled
		{
			get { return _enabled; }
			set 
			{ 
				if (value != _enabled) 
				{
					_enabled = value;

					if (StateChanged != null)
						StateChanged(this, EventArgs.Empty);
				}
			}
		}

		public bool Visible
		{
			get { return _visible; }
			set 
			{ 
				if (value != _visible) 
				{
					_visible = value;

					if (StateChanged != null)
						StateChanged(this, EventArgs.Empty);
				}
			}
		}

		public StateItem()
		{
			_checked = false;
			_enabled = true;
			_visible = true;
		}

		public event EventHandler StateChanged;
	}

	/// <summary>
	/// Summary description for StateManager.
	/// </summary>
	public class StateManager
	{
		public sealed class StatesTable
		{
			private Hashtable states;

			internal StatesTable()
			{
				states = new Hashtable();
			}

			public StateItem this[string name]
			{
				get 
				{ 
					StateItem si = (StateItem) states[name]; 
					if (si == null)
					{
						si = new StateItem();
						states[name] = si;
					}
					return si;
				}
			}
		}

		private static StatesTable states;
			
		static StateManager()
		{
			states = new StatesTable();
		}

		public static StatesTable States
		{
			get { return states; }
		}
	}
}
