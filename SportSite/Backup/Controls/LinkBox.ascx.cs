using System.Configuration;
namespace SportSite.Controls
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using Sport.Common;
	

	/// <summary>
	///	Base class for link box - container of text links.
	///	each derived class would have to implement different kind of container
	///	and to override the AddLink method of the Container class.
	/// </summary>
	public class LinkBox : System.Web.UI.UserControl
	{
		public enum LinkIndicator
		{
			None=-1,
			Blue,
			Red,
			Yellow,
			StrongRed,
			Gray
		}
		
		#region Link Class
		/// <summary>
		/// Link represents one instance of link with its URL address, text
		/// to be displayed and the link style which determines its appearance.
		/// Support nesting without limit, each link can contain its own Links.
		/// </summary>
		public class Link
		{
			private string _url;
			private string _text;
			private LinkStyle _style=LinkStyle.Empty;
			private LinkIndicator _indicator;
			private bool _isActive=false;
			private  bool _isHotLink=false;
			private bool _visible=true;

			public int Level { get; set; }

			public static readonly Link Empty;
			
			static Link()
			{
				Empty=new Link("", "", LinkStyle.Empty, LinkIndicator.None);
			}
			
			#region public properties
			/// <summary>
			/// general properties of the link such as Css Class, background color.
			/// </summary>
			public LinkStyle Style
			{
				get {return _style;}
				set {_style = value;}
			}
			
			public string Url
			{
				get	{ return _url; }
				set {_url = value;}
			}
			
			public string Text
			{
				get {return _text;}
				set {_text = value;}
			}
			
			public LinkIndicator Indicator
			{
				get {return _indicator;}
				set {_indicator = value;}
			}
			
			public bool IsActive
			{
				get {return _isActive;}
				set {_isActive = value;}
			}
			
			public bool IsHotLink
			{
				get {return _isHotLink;}
				set {_isHotLink = value;}
			}
			
			public bool Visible
			{
				get {return _visible;}
				set {_visible = value;}
			}
			#endregion
			
			/// <summary>
			/// Links: child links of the current link, i.e. nested links.
			/// </summary>
			private LinkCollection _links;
			public LinkCollection Links
			{
				get { return _links; }
				set {_links = value;}
			}
			
			/// <summary>
			/// creates an empty link.
			/// </summary>
			public Link()
			{
				_links = new LinkCollection();
				_url = "";
				_text = "";
				_indicator = LinkIndicator.None;
				_isActive = false;
			}
			
			/// <summary>
			/// creates new link with the given URL address, text and indicator.
			/// </summary>
			public Link(string url, string text, LinkIndicator indicator)
				:this()
			{
				if (!string.IsNullOrEmpty(url) && url.StartsWith("?"))
					url = HttpContext.Current.Request.FilePath + url;
				_url = url;
				_text = text;
				_indicator = indicator;
			}
			
			/// <summary>
			/// creates new link with the given URL address and text.
			/// </summary>
			public Link(string url, string text)
				:this(url, text, LinkIndicator.None)
			{
			}
			
			/// <summary>
			/// creates new link with given URL, text, link style and indicator.
			/// </summary>
			public Link(string url, string text, LinkStyle style, LinkIndicator indicator)
				:this(url, text, indicator)
			{
				_style = style;
			}
			
			/// <summary>
			/// creates new link with given URL, text and link style.
			/// </summary>
			public Link(string url, string text, LinkStyle style)
				:this(url, text, style, LinkIndicator.None)
			{
			}
			
			public override bool Equals(object obj)
			{
				if (!(obj is Link))
					return false;
				
				Link link=(Link) obj;
				if (link.Text != this.Text)
					return false;
				if (link.Url != this.Url)
					return false;
				if (!link.Style.Equals(this.Style))
					return false;
				return true;
			}
			
			public override int GetHashCode()
			{
				return base.GetHashCode ();
			}
			
			public string GetIndicatorImage()
			{
				switch (this.Indicator)
				{
					case LinkIndicator.Blue:
						return "blue_indicator.gif";
					case LinkIndicator.Red:
						return "red_indicator.gif";
					case LinkIndicator.Yellow:
						return "yellow_indicator.gif";
					case LinkIndicator.StrongRed:
						return "red_strong_indicator.gif";
					case LinkIndicator.Gray:
						return "grey_indicator.gif";
				}
				return "";
			}
		} //end class Link
		#endregion

		#region LinkCollection
		/// <summary>
		/// LinkCollection represents collection of Links and supports 
		/// adding, removing or searching for links within it. Improved 
		/// version of ArrayList, can hold only Links.
		/// </summary>
		public class LinkCollection : GeneralCollection
		{
			private LinkStyle _defaultStyle=LinkStyle.Empty;
			public LinkStyle DefaultSyle
			{
				get {return _defaultStyle;}
				set {_defaultStyle = value;}
			}
			
			/// <summary>
			/// common indexer: returns the Link in given index.
			/// </summary>
			public Link this[int index]
			{
				get { return (Link) GetItem(index); }
			}

			/// <summary>
			/// search for link in the collection based on its url address.
			/// </summary>
			public Link this[string url]
			{
				//read only property.
				get
				{
					//search the collection for the desired Link.
					for (int i=0; i<this.Count; i++)
					{
						Link link=this[i];
						//url address is not case sensitive for Windows based servers.
						if (link.Url.ToLower() == url.ToLower())
						{
							//found matching link, return it:
							return link;
						}
					}
					return null;
				}
			} //end indexer Link[string]

			/// <summary>
			/// insert given link in given index.
			/// </summary>
			public void Insert(int index, Link value)
			{
				//simply call the method from our base class.
				InsertItem(index, value);
			}

			/// <summary>
			/// create new link with given URL and text and insert it in given index.
			/// </summary>
			public void Insert(int index, string url, string text)
			{
				//call base class method with new Link.
				InsertItem(index, new Link(url, text));
			}

			/// <summary>
			/// remove given link from the collection.
			/// </summary>
			public void Remove(Link value)
			{
				//call the method from our base class.
				RemoveItem(value);
			}

			/// <summary>
			/// returns whether the collection contains the given link or not
			/// </summary>
			public bool Contains(Link value)
			{
				//use the method of our base class.
				return base.Contains(value);
			}

			/// <summary>
			/// returns the index of the given link in the collection, if exist or 
			/// -1 if does not exist.
			/// </summary>
			public int IndexOf(Link value)
			{
				//use method from our base class.
				return base.IndexOf(value);
			}
			
			/// <summary>
			/// add the given link to the collection.
			/// </summary>
			public int Add(Link value)
			{
				if ((value.Url.Length == 0)&&(value.Text.Length == 0))
					return -1;
				
				if ((value.Style.AddLight == 0)&&(value.Style.CssClass.Length == 0)&&(value.Style.isBold == false))
					value.Style = _defaultStyle;
				
				return AddItem(value);
			}
			
			/// <summary>
			/// creates new link with the given URL and text and add it into collection.
			/// </summary>
			public int Add(string url, string text)
			{
				return AddItem(new Link(url, text));
			}
			
			/// <summary>
			/// creates new link with given URL address, text and style and add
			/// the link to the collection.
			/// </summary>
			public int Add(string url, string text, LinkStyle style)
			{
				return AddItem(new Link(url, text, style));
			}
			
			/// <summary>
			/// change background color of given link to given color.
			/// </summary>
			public void ChangeBgColor(Link link, Color color)
			{
				LinkBox.LinkStyle tmpStyle=link.Style;
				tmpStyle.originalColor = tmpStyle.bgColor;
				tmpStyle.bgColor = color;
				link.Style = tmpStyle;
			}
			
			/// <summary>
			/// change background color of link with given index to given color.
			/// </summary>
			public void ChangeBgColor(int index, Color color)
			{
				ChangeBgColor(this[index], color);
			}

			public void ChangeMarginBottom(int index, int marginBottom)
			{
				if (this[index] == null)
					return;
				LinkBox.LinkStyle tmpStyle=this[index].Style;
				tmpStyle.marginBottom = marginBottom;
				this[index].Style = tmpStyle;
			}
			
			public void MakeLinkActive(Common.NavBarLink link, string text)
			{
				int index=(int) link;
				
				//restore all previous colors:
				for (int i=0; i<this.Count; i++)
				{
					this[i].IsActive = false;
					if (this[i].Links != null)
					{
						foreach (Link childLink in this[i].Links)
							childLink.IsActive = false;
					}
					//this[i].Style = LinkStyle.ChangeBgColor(
					//	this[i].Style, this[i].Style.originalColor);
				}
				
				//change bg color:
				this[index].IsActive = true;
				//ChangeBgColor(index, Common.Style.ActiveLinkColor);
				if ((text != null)&&(text.Length>0))
				{
					foreach (Link childLink in this[index].Links)
					{
						if (childLink.Text == text)
						{
							childLink.IsActive = true;
							//ChangeBgColor(childLink, Common.Style.ActiveLinkColor);
						}
					}
				}
			}
			
			public void MakeLinkActive(Common.NavBarLink link)
			{
				MakeLinkActive(link, null);
			}
			
			/// <summary>
			/// add bottom margin to the link in given index.
			/// </summary>
			public void AddLinkGap(int index)
			{
				ChangeMarginBottom(index, Common.Style.EmptyLinkGap);
			}

			/// <summary>
			/// add bottom margin to the link in given index.
			/// </summary>
			public void AddLinkGap(Common.NavBarLink link)
			{
				int index=(int) link;
				AddLinkGap(index);
			}

			// Overriding += to use Add
			public static LinkCollection operator + (LinkCollection collection, Link link)
			{
				collection.Add(link);
				return collection;
			}

			// Overriding += to use Add
			public static LinkCollection operator + (LinkCollection collection, Link[] links)
			{
				for (int i=0; i<links.Length; i++)
					collection.Add(links[i]);
				return collection;
			}
		} //end class LinkCollection
		#endregion

		#region LinksContainer Class
		/// <summary>
		/// this class determines how the links will be displayed in the final output.
		/// each derived class of LinkBox should define its own Container.
		/// this default Container 
		/// </summary>
		protected class LinksContainer : System.Web.UI.Control
		{
			#region AddLinkArguments Struct
			/// <summary>
			/// arguments passed to the AddLink method.
			/// </summary>
			public struct AddLinkArguments
			{
				/// <summary>
				/// marginWidth: determine how much margin to have before the link.
				/// </summary>
				public int marginWidth;
				public int NestingLevel;
				public static readonly AddLinkArguments Empty;
				
				public AddLinkArguments(int marginWidth)
				{
					this.marginWidth = marginWidth;
					this.NestingLevel = 0;
				}
				
				static AddLinkArguments()
				{
					Empty = new AddLinkArguments(0);
				}
			} //end struct AddLinkArguments
			#endregion
			
			protected Color _bkColor=Color.Empty;
			protected int _height=0;
			protected int _nestedLinksMargin=30;
			protected int _linkWidth=0;
			protected bool _isHebrew=false;
			protected Page _page=null;
			protected bool _javascriptRegistered=false;
			
			/// <summary>
			/// determines the actual control to be rendered.
			/// </summary>
			protected System.Web.UI.Control _control=null;
			
			/// <summary>
			/// define here the ID of the container and create the proper control.
			/// pass the Page object for registering the javascript.
			/// </summary>
			public LinksContainer(Page page)
			{
				_control = new System.Web.UI.WebControls.Panel();
				this.ID = "LinksContainer";
				this._page = page;
			}
			
			#region public properties
			/// <summary>
			/// returns the actual control.
			/// </summary>
			public System.Web.UI.Control Control
			{
				get {return _control;}
			}
			/// <summary>
			/// set or get background color of the container.
			/// </summary>
			public virtual Color BkColor
			{
				get {return _bkColor;}
				set
				{
					_bkColor = value;
					//(_control as Panel).BackColor = value;
				}
			}
			
			/// <summary>
			/// set or get height of the container, in pixels.
			/// </summary>
			public virtual int Height
			{
				get {return _height;}
				set
				{
					_height = value;
					(_control as Panel).Height = new Unit((double) value);
				}
			}
			
			/// <summary>
			/// set or get the margin of nested links in pixels, i.e. by how 
			/// much to jump for each nested level of links.
			/// </summary>
			public int NestedLinksMargin
			{
				get {return _nestedLinksMargin;}
				set {_nestedLinksMargin = value;}
			}
			
			/// <summary>
			/// set or get the width in pixels of the links.
			/// </summary>
			public int LinkWidth
			{
				get {return _linkWidth;}
				set {_linkWidth = value;}
			}

			/// <summary>
			/// set or get whether the container should display the links in hebrew
			/// style i.e. right-to-left or not
			/// </summary>
			public bool IsHebrew
			{
				get {return _isHebrew;}
				set {_isHebrew = value;}
			}

			/// <summary>
			/// get or set Page for the container.
			/// </summary>
			public override System.Web.UI.Page Page
			{
				get {return _page;}
				set {_page = value;}
			}
			#endregion

			/// <summary>
			/// remember to override this method to decide the final look of the links.
			/// </summary>
			protected override void Render(HtmlTextWriter writer)
			{
				//_control.Render(writer);
			}
			
			#region AddLink method
			private int _linksCount=0;


			/// <summary>
			/// override this method to decide how the control would appear.
			/// </summary>
			public virtual int AddLink(Link link, AddLinkArguments args)
			{
				//////////////////////////////////////////
				//determine appearance of the control.	//
				//currently, each link is one line		//
				//within panel control.					//
				//////////////////////////////////////////
				
				//visible?
				if (!link.Visible)
					return this.Controls.Count;
				
				//add the given link as new line in the panel. use pure html.
				System.Text.StringBuilder builder=new System.Text.StringBuilder();
				
				//being here means we have one more link added to the links container.
				_linksCount++;
				
				//build unique ID for the sub links container, using the links 
				//count and the margin width which represents nesting level:
				string strSubLinksID="subLinksContainer_"+Math.Abs(link.Url.GetHashCode()+link.Text.GetHashCode()); //;_linksCount+"_"+args.marginWidth;
				
				if ((link.Links != null)&&(link.Links.Count > 0))
					link.Url = "javascript:void(0);";
				
				string strClass="ButtonPanel";
				if (args.NestingLevel > 0)
					strClass = "ButtonPanelSub";
				if (link.IsActive)
					strClass = "ButtonPanelActive";
				if (link.IsHotLink)
					strClass = "ButtonPanelMarked";
				if ((link.Url == null)||(link.Url.Length == 0))
					strClass = "ButtonPanelCaption";
				
				if (args.NestingLevel > 0)
					link.Indicator = LinkIndicator.Gray;
				
				builder.Append("<div dir=\"rtl\" class=\""+strClass+"\">");
				if (link.Indicator != LinkIndicator.None)
				{
					string strImageName=link.GetIndicatorImage();
					if (strImageName.Length > 0)
					{
						if (args.NestingLevel > 0)
							builder.Append(Common.Tools.MultiString("&nbsp;", 3*args.NestingLevel));
						builder.Append("<img src=\""+Common.Data.AppPath+"/Images/"+strImageName+"\" class=\"LinkIndicator\" />");
					}
				}
				int multiplier=(args.NestingLevel == 0)?3:2;
				builder.Append(Common.Tools.MultiString("&nbsp;", multiplier*(args.NestingLevel+1)));
				string strLinkHTML="";
				if ((link.Url != null)&&(link.Url.Length > 0))
				{
					strLinkHTML += "<a class=\"ButtonPanelLink\" href=\""+link.Url+"\"";
					if ((link.Links != null)&&(link.Links.Count > 0))
					{
						strLinkHTML += " onclick=\"ToggleVisibility('"+strSubLinksID+"'); "+
							"ResizeContents(); return false;\"";
					}
					strLinkHTML += ">";
				}
				builder.Append(strLinkHTML);
				if (link.Style.isBold)
					builder.Append("<b>");
				if ((link.Text != null)&&(link.Text.Length > 0))
					builder.Append(link.Text);
				else
					builder.Append("&nbsp;");
				if (link.Style.isBold)
					builder.Append("</b>");
				if ((link.Url != null)&&(link.Url.Length > 0))
					builder.Append("</a>");
				if ((link.Links != null)&&(link.Links.Count > 0))
				{
					builder.Append("<span class=\"ButtonPanelGap\"></span>");
					builder.Append(strLinkHTML);
					builder.Append("<img class=\"ButtonPanelMore\" src=\""+
						Common.Data.AppPath+"/Images/combo_arrow_2.gif\" />");
					if ((link.Url != null)&&(link.Url.Length > 0))
						builder.Append("</a>");
				}
				builder.Append("");
				builder.Append("</div>");
				
				string strHtml=builder.ToString();
				(_control as Panel).Controls.Add(new LiteralControl(strHtml));
				
				if ((link.Links != null)&&(link.Links.Count > 0))
				{
					strHtml = "<span id=\""+strSubLinksID+"\"";
					bool active=link.IsActive;
					if (!active)
					{
						for (int i=0; i<link.Links.Count; i++)
						{
							if (link.Links[i].IsActive)
							{
								active = true;
								break;
							}
						}
					}
					if (!active)
						strHtml += " style=\"display: none;\"";
					strHtml += ">";
					(_control as Panel).Controls.Add(new LiteralControl(strHtml));
				}
				
				int oldCount=_linksCount;
				_linksCount = 0;
				foreach (Link childLink in link.Links)
				{
					AddLinkArguments tempArgs=args;
					tempArgs.NestingLevel = args.NestingLevel+1;
					AddLink(childLink, tempArgs);
				}
				_linksCount = oldCount;
				if ((link.Links != null)&&(link.Links.Count > 0))
				{
					strHtml = "</span>";
					(_control as Panel).Controls.Add(new LiteralControl(strHtml));
				}
				
				return this.Controls.Count;
			} //end function AddLink
			#endregion

			#region RegisterJavaScript
			/// <summary>
			/// register the needed javascript for adding more light when 
			/// mouse over link.
			/// </summary>
			protected void RegisterJavaScript()
			{
				//can't register without valid page:
				if (_page == null)
					return;
				
				//exit if already registered:
				if (_javascriptRegistered == true)
					return;
				
				//add the code:
				//SportSite.Common.ClientSide.RegisterAddLight(_page);
				_javascriptRegistered = true;
			} //end function RegisterJavascript
			#endregion

			#region Tools
			/// <summary>
			/// general tools.
			/// </summary>
			protected class Tools
			{
				/// <summary>
				/// return the given string "multiplied" given amount of times.
				/// for example calling it with "hel" and 3 will return "helhelhel"
				/// </summary>
				public string MultiString(string str, int amount)
				{
					string result="";
					for (int i=0; i<amount; i++)
					{
						result += str;
					}
					return result;
				}
			} //end class LinksContainer.Tools
			#endregion
		} //end class LinksContainer
		#endregion

		#region LinkStyle struct
		/// <summary>
		/// represents the style of each link: css class, background color, visiblity
		/// and more properties.
		/// </summary>
		public struct LinkStyle
		{
			public static LinkStyle Empty;
			public string CssClass;
			public Color bgColor;
			public Color originalColor;
			/// <summary>
			/// amount of light to add when mouse is over the link.
			/// </summary>
			public int AddLight;
			public int Height;
			public HorizontalAlign hAlign;
			public int fontSize;
			public int marginBottom;
			public bool isBold;
			
			public LinkStyle(string dummy)
			{
				//default values.
				CssClass = "";
				bgColor = Color.Empty;
				originalColor = Color.Empty;
				AddLight = 0;
				Height = 0;
				hAlign = HorizontalAlign.NotSet;
				fontSize = 0;
				marginBottom = 1;
				isBold = false;
			}

			static LinkStyle()
			{
				Empty = new LinkStyle("");
			}

			public static LinkStyle ChangeFontSize(LinkStyle style, int fontSize)
			{
				LinkStyle result=style;
				result.fontSize = fontSize;
				return result;
			}
			
			public static LinkStyle ChangeBgColor(LinkStyle style, Color color)
			{
				LinkStyle result=style;
				result.bgColor = color;
				return result;
			}

			public static LinkStyle ChangeAlignment(LinkStyle style, HorizontalAlign align)
			{
				LinkStyle result=style;
				result.hAlign = align;
				return result;
			}
			
			public static LinkStyle CopyLinkStyle(LinkStyle style)
			{
				LinkStyle result=new LinkStyle("");
				result.AddLight = style.AddLight;
				result.bgColor = style.bgColor;
				result.CssClass = style.CssClass;
				result.fontSize = style.fontSize;
				result.hAlign = style.hAlign;
				result.Height = style.Height;
				result.marginBottom = style.marginBottom;
				result.isBold = style.isBold;
				result.originalColor = style.originalColor;
				return result;
			}
		}
		#endregion

		protected LinkCollection _links;
		protected LinkStyle _defaultLinkStyle=LinkStyle.Empty;
		protected LinksContainer _container;
		protected int _height=0;
		protected int _width=0;
		protected Color _bkColor=Color.Empty;
		protected int _nestedLinksMargin=5;
		protected bool _isHebrew=false;
		public static int COUNTER=0;
		
		#region public properties
		/// <summary>
		/// total height of links box. set as 0 to have auto size.
		/// </summary>
		public int Height
		{
			get {return _height;}
			set {_height = value;}
		}
		
		/// <summary>
		/// total width of each link. set as 0 to have auto size.
		/// </summary>
		public virtual int Width
		{
			get {return _width;}
			set
			{
				_width = value;
				_container.LinkWidth = value;
			}
		}

		/// <summary>
		/// background color of the box itself.
		/// </summary>
		public Color BkColor
		{
			get {return _bkColor;}
			set
			{
				_bkColor = value;
				_container.BkColor = value;
			}
		}
		
		/// <summary>
		/// collection of all the links that will be displayed.
		/// </summary>
		public LinkCollection Links
		{
			get { return _links; }
			set {_links = value;}
		}
		
		/// <summary>
		/// define the default style for link in the box
		/// </summary>
		public LinkStyle DefaultLinkStyle
		{
			get {return _defaultLinkStyle;}
			set
			{
				_defaultLinkStyle = value;
				_links.DefaultSyle = value;
			}
		}
		
		/// <summary>
		/// define margin width of nested links, in pixels.
		/// </summary>
		public int NestedLinksMargin
		{
			get {return _nestedLinksMargin;}
			set
			{
				_nestedLinksMargin = value;
				_container.NestedLinksMargin = value;
			}
		}
		
		/// <summary>
		/// determines the write order (left-to-right or right-to-left)
		/// </summary>
		public bool IsHebrew
		{
			get {return _isHebrew;}
			set
			{
				_isHebrew = value;
				_container.IsHebrew = value;
			}
		}
		#endregion
		
		/// <summary>
		/// initialize controls, define the ID.
		/// </summary>
		public LinkBox()
		{
			_links = new LinkCollection();
			_container = new LinksContainer(Page);
			LinkBox.COUNTER++;
			this.ID = "LinkBox"+LinkBox.COUNTER.ToString();
		}

		/// <summary>
		/// initialize controls, define the ID. should get Page control for javascript.
		/// </summary>
		public LinkBox(System.Web.UI.Page page)
			:this()
		{
			if (this.Page == null)
			{
				this.Page = page;
				_container = new LinksContainer(Page);
			}
		}
		
		public static Link[] DummyLinks(int amount, LinkBox.LinkStyle style)
		{
			Link[] result=new Link[amount];
			for (int i=0; i<amount; i++)
				result[i] = new Link("", "", style);
			return result;
		}

		/// <summary>
		/// add the links to the container, display container on screen.
		/// </summary>
		protected virtual void Page_Load(object sender, System.EventArgs e)
		{
			//initialize container page:
			_container.Page = this.Page;
			
			//background color:
			_container.BkColor = this.BkColor;
			
			//height:
			if (this.Height > 0)
				_container.Height = this.Height;
			
			//add the links:
			for (int i=0; i<Links.Count; i++)
			{
				Link link=Links[i];
				//apply default style if needed.
				if (link.Style.Equals(LinkStyle.Empty))
				{
					link.Style = this.DefaultLinkStyle;
				}
				_container.AddLink(link, LinkBox.LinksContainer.AddLinkArguments.Empty);
			}
			this.Controls.Add(_container.Control);
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			//Response.Write("linkbox: "+this.Parent.GetType().ToString()+"<br />");
			
			//temporary fix
			if (this.Parent.GetType().ToString().Equals("ASP.Header_ascx"))
				return;
			
			this.Load += new System.EventHandler(this.Page_Load);
		}
		#endregion
	}
}
