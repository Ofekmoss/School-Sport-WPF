using System;
using System.Windows.Forms;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;

namespace Sport.UI
{
	/// <summary>
	/// View inherits Control and adds general implementation
	/// needed for the application screens. All application screens
	/// shall inherit View and implement the needed virtual methods.
	/// </summary>
	[DesignerCategory("Form"), 
	DesignTimeVisible(false), 
	Designer("System.Windows.Forms.Design.UserControlDocumentDesigner, System.Design", typeof(IRootDesigner)), 
	DefaultEvent("Load"), 
	ToolboxItem(false)]
	public class View : System.Windows.Forms.Control, Sport.Synchronization.ISynchronizable
	{
		public View()
		{
			// Checking permission
			/*
			if (!IsPermitted)
				throw new Exception("User don't have permission to open the view");
			*/
			
			_viewName = GetType().Name;
			state = new ParameterList();
			Dock = DockStyle.Fill;

			KeyDown += new KeyEventHandler(View_KeyDown);
		}
		
		protected string _editorViewName="";
		public string EditorViewName
		{
			get {return _editorViewName;}
		}
		
		#region Properties

		/// <summary>
		/// The title property fires TitleChanged event on change.
		/// The main application form will catch this event and
		/// will update the screen and caption text.
		/// </summary>
		private string _title;

		public event EventHandler TitleChanged;
		
		public string Title
		{
			get { return _title; }
			set 
			{
				_title = value;

				if (TitleChanged != null)
					TitleChanged(this, EventArgs.Empty);
			}
		}

		private string _viewName;
		public string ViewName
		{
			get { return _viewName; }
			set { _viewName = value; }
		}

		#endregion

		#region View Operations

		public enum ViewStatus
		{
			Closed,
			Opened,
			Activated
		}

		private ViewStatus status = ViewStatus.Closed;
		public ViewStatus Status
		{
			get { return status; }
		}

		// Operations events
		public event EventHandler Opened;
		public event EventHandler Closed;
		public event EventHandler Activated;
		public event EventHandler Deactivated;

		// Open is called by ViewManager once after
		// the View has been created and all its base properties set.
		// A view should implement this method to initialize the view.
		public virtual void Open()
		{
			status = ViewStatus.Opened;
			
			if (Opened != null)
				Opened(this, EventArgs.Empty);
		}

		// Closing is called by ViewManager when a view
		// shoud be closed. The inheriting view should
		// return false if the view should not be closed
		// (e.g. if there are unsaved changes)
		public virtual bool Closing()
		{
			return true;
		}

		// Close is called by ViewManager once before
		// the View is disposed.
		public virtual void Close()
		{
			status = ViewStatus.Closed;
			
			if (Closed != null)
				Closed(this, EventArgs.Empty);
		}

		// Activate is called by ViewManager each time a view is selected
		// after not being selected. A view should implement this method
		// to connect to application states (toolbar state etc.).
		public virtual void Activate()
		{
			status = ViewStatus.Activated;
			
			if (Activated != null)
				Activated(this, EventArgs.Empty);
		}

		// Deactivate is called by ViewManager each time a view is unselected
		// after being selected. A view should implement this method
		// to disconnect itself from application states.
		public virtual void Deactivate()
		{
			status = ViewStatus.Opened;
			
			if (Deactivated != null)
				Deactivated(this, EventArgs.Empty);
		}

		#endregion

		#region View State

		public static readonly string SelectionDialog = "SelectionDialog";

		protected ParameterList state;

		public ParameterList State
		{
			get { return state; }
			set { state = value; }
		}


		#endregion

		private void View_KeyDown(object sender, KeyEventArgs e)
		{
			if (KeyListener.HandleEvent(this, e))
				e.Handled = true;
		}

		public Sport.Core.Configuration GetConfiguration()
		{
			return Sport.Core.Configuration.GetConfiguration().GetConfiguration("Views").GetConfiguration("View", _viewName);
		}

		public static bool IsViewPermitted(Type type)
		{
			//sorry but this just does not work. too complicated to solve.
			/*
			object[] attributes = type.GetCustomAttributes(typeof(ViewPermissionsAttribute), false);
			if (attributes != null && attributes.Length > 0)
			{
				ViewPermissionsAttribute vpa = attributes[0] as ViewPermissionsAttribute;
				int permissions = Sport.Core.Session.User.Permissions;
				if (!Sport.Types.PermissionTypeLookup.Contains(permissions, vpa.Permission) ||
					(vpa.OnlyCentral && Sport.Core.Session.Region != Sport.Entities.Region.CentralRegion))
					return false;
			}
			*/
			return true;
		}

		public bool IsPermitted
		{
			get
			{
				return IsViewPermitted(GetType());
			}
		}

		public virtual bool Synchronize(out object data)
		{
			//nothing here.
			data = null;
			return false;
		}

		public virtual void OnSuccess(object data)
		{
			//nothing here.
		}

		public virtual void OnError(string errorMessage)
		{
			//nothing here.
		}
	}

	[AttributeUsage(AttributeTargets.All, AllowMultiple=false, Inherited=true)]
	public sealed class ViewPermissionsAttribute : Attribute
	{
		private Sport.Types.Permission _permission;
		public Sport.Types.Permission Permission
		{
			get { return _permission; }
		}

		private bool _onlyCentral;
		public bool OnlyCentral
		{
			get { return _onlyCentral; }
		}
	
		public ViewPermissionsAttribute(Sport.Types.Permission permission, bool onlyCentral)
		{
			_permission = permission;
			_onlyCentral = onlyCentral;
		}

		public ViewPermissionsAttribute(Sport.Types.Permission permission)
			: this(permission, false)
		{
		}
	}
}
