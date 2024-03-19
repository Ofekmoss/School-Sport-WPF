using System;
using System.Drawing;

namespace Sport.Documents
{
	public class Page : PageItem
	{
		public Page(Size size)
			: base(new Rectangle(0, 0, size.Width, size.Height))
		{
			_newSection = false;
		}

		public Page()
			: this(Size.Empty)
		{
		}

		#region Cloning

		public Page(Page page)
			: base(page)
		{
			_newSection = false;
		}

		public override DocumentItem Clone()
		{
			return new Page(this);
		}

		#endregion

		public Document Document
		{
			get
			{
				return Parent as Document;
			}
		}

		private bool _newSection;
		public bool NewSection
		{
			get { return _newSection; }
			set { _newSection = value; }
		}

		public int PageNumber
		{
			get 
			{ 
				Document document = Document;
				if (document == null)
					return 0;

				return document.GetPageNumber(this);
			}
		}

		public int SectionPageNumber
		{
			get
			{ 
				Document document = Document;
				if (document == null)
					return 0;

				return document.GetSectionPageNumber(this);
			}
		}

		public int Section
		{
			get
			{
				Document document = Document;
				if (document == null)
					return 0;

				return document.GetPageSection(this);
			}
		}

		public Size Size
		{
			get 
			{ 
				if (Bounds.Size.IsEmpty)
				{
					Document document = Parent as Document;
					if (document == null)
						return Size.Empty;

					return document.DefaultPageSize;
				}

				return Bounds.Size;
			}
			set 
			{ 
				base.Bounds = new Rectangle(0, 0, value.Width, value.Height); 
			}
		}

		[System.ComponentModel.Browsable(false), System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
		public new Rectangle Bounds
		{
			get { return base.Bounds; }
			set {
				base.Bounds = value;
				Size = value.Size;
			}
		}
	}
}
