namespace SportSite.Controls
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;

	/// <summary>
	///		Summary description for HorizontalLinkBox.
	/// </summary>
	public class HorizontalLinkBox : LinkBox
	{
		#region LinksContainer Class
		protected class HorizontalLinksContainer : LinksContainer
		{
			protected int _width=0;
			
			public HorizontalLinksContainer(Page page)
				:base(page)
			{
				_control = new System.Web.UI.WebControls.Table();
				(_control as Table).CellSpacing = 1;
				this.ID = "HorizontalLinksContainer";
				this._page = page;
				//add the default and only row for the control:
				TableRow objRow=new TableRow();
				(_control as Table).Rows.Add(objRow);
			}

			#region public properties
			/// <summary>
			/// set or get background color of the container.
			/// </summary>
			public override Color BkColor
			{
				get {return _bkColor;}
				set
				{
					_bkColor = value;
					//(_control as Table).BackColor = value;
					Table objTable=(Table) _control;
					foreach (TableRow objRow in objTable.Rows)
					{
						foreach (TableCell objCell in objRow.Cells)
						{
							objCell.BackColor = value;
						}
					}
				}
			}
			
			/// <summary>
			/// set or get height of the container, in pixels.
			/// </summary>
			public override int Height
			{
				get {return _height;}
				set
				{
					_height = value;
					(_control as Table).Height = new Unit((double) value);
				}
			}

			/// <summary>
			/// total width of the container.
			/// </summary>
			public int Width
			{
				get {return _width;}
				set
				{
					_width = value;
					(_control as Table).Width = new Unit((double) value);
				}
			}
			#endregion

			#region AddLink method
			/// <summary>
			/// override this method to decide how the control would appear.
			/// </summary>
			public override int AddLink(Link link, AddLinkArguments args)
			{
				//////////////////////////////////////////
				//determine appearance of the control.	//
				//currently, each link is one cell in	//
				//table, displayed horizontally.		//
				//////////////////////////////////////////
				
				//add the given link as new cell in the table.
				TableCell objCell=new TableCell();
				if (link.Style.AddLight != 0)
				{
					RegisterJavaScript();
					if (_javascriptRegistered)
					{
						Color bgColor=link.Style.bgColor;
						if (bgColor == Color.Empty)
							bgColor = this.BkColor;
						if (bgColor != Color.Empty)
						{
							LinkBox.LinkStyle tempStyle=link.Style;
							tempStyle.bgColor = bgColor;
							link.Style = tempStyle;
							//add light:
							//SportSite.Common.ClientSide.MakeBrighter(objCell, Sport.Common.Tools.ColorToHex(bgColor), link.Style.AddLight);
						}
					}
				} //end if link has AddLight property
				
				if (args.marginWidth > 0)
				{
					string strHtml="<div style=\"margin-";
					strHtml += (IsHebrew)?"right":"left";
					strHtml += ": "+args.marginWidth.ToString()+"px;\">";
					objCell.Controls.Add(new LiteralControl(strHtml));
				}
				if (this.LinkWidth > 0)
				{
					//objCell.Width = new Unit((double) this.LinkWidth);
				}
				if (link.Style.Height > 0)
				{
					objCell.Height = new Unit((double) link.Style.Height);
				}
				if (link.Style.bgColor != Color.Empty)
				{
					objCell.BackColor = link.Style.bgColor;
				}
				if (link.Visible == false)
				{
					objCell.Visible = false;
				}
				
				if (this.IsHebrew)
				{
					objCell.Attributes.Add("dir", "rtl");
				}
				if (link.Style.hAlign != HorizontalAlign.NotSet)
				{
					objCell.HorizontalAlign = link.Style.hAlign;
				}
				
				System.Web.UI.Control control=null;
				if (link.Text.Length*link.Url.Length > 0)
				{
					HyperLink hyperLink=new HyperLink();
					hyperLink.NavigateUrl = link.Url;
					hyperLink.Text = link.Text;
					if (link.Style.CssClass.Length > 0)
					{
						hyperLink.CssClass = link.Style.CssClass;
					}
					if (link.Style.fontSize > 0)
					{
						hyperLink.Style.Add("font-size", link.Style.fontSize.ToString()+"px");
					}
					control = hyperLink;
				}
				else
				{
					control=new LiteralControl("&nbsp;");
				}
				objCell.Controls.Add(control);
				if (args.marginWidth > 0)
				{
					string strHtml="</div>";
					objCell.Controls.Add(new LiteralControl(strHtml));
				}
				
				//add current cell:
				(_control as Table).Rows[0].Cells.Add(objCell);
				objCell.Visible = true;
				
				//rearrange the cells width:
				int width=(int) ((_control as Table).Width.Value/(_control as Table).Rows[0].Cells.Count);
				for (int i=0; i<(_control as Table).Rows[0].Cells.Count; i++)
				{
					(_control as Table).Rows[0].Cells[i].Width = new Unit((double) width);	
				}
				
				foreach (Link childLink in link.Links)
				{
					AddLinkArguments tempArgs=args;
					tempArgs.marginWidth += this.NestedLinksMargin;
					AddLink(childLink, tempArgs);
				}
				return this.Controls.Count;
			} //end function AddLink
			#endregion
		}
		#endregion

		#region Constructors
		/// <summary>
		/// initialize controls, define the ID.
		/// </summary>
		public HorizontalLinkBox()
		{
			_links = new LinkCollection();
			_container = new HorizontalLinksContainer(Page);
			LinkBox.COUNTER++;
			this.ID = "HorizontalLinkBox"+LinkBox.COUNTER.ToString();
		}

		/// <summary>
		/// initialize controls, define the ID. should get Page control for javascript.
		/// </summary>
		public HorizontalLinkBox(System.Web.UI.Page page)
			:this()
		{
			if (this.Page == null)
			{
				this.Page = page;
				_container = new HorizontalLinksContainer(Page);
			}
		}
		#endregion

		public override int Width
		{
			get
			{
				return (_container as HorizontalLinksContainer).Width;
			}
			set
			{
				(_container as HorizontalLinksContainer).Width = value;
			}
		}


		/// <summary>
		/// add the links to the container, display container on screen.
		/// </summary>
		protected override void Page_Load(object sender, System.EventArgs e)
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
			//Response.Write(((_container.Control as Table).Rows[0].Cells[1].Controls[0] as HyperLink).Visible+"<br />");
			TableRow dummyRow=(_container.Control as Table).Rows[0];
			this.Controls.Add(_container.Control);
			//Response.Write(this.Controls[0].Visible);
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
			//Response.Write("horizontal linkbox: "+this.Parent.GetType().ToString()+"<br />");
			// if (this.Parent.GetType().ToString().Equals("System.Web.UI.HtmlControls.HtmlForm"))
			this.Load += new System.EventHandler(this.Page_Load);
		}
		#endregion
	}
}
