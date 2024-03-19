using System;

namespace Sport.Documents
{
	public interface ISectionObject
	{
		void InitializeSave(DocumentBuilder builder);
		bool SavePage(DocumentBuilder builder, Page page);
		void FinalizeSave(DocumentBuilder builder);
	}

	#region Section Class

	public class Section : Page
	{
		private System.Drawing.Pen _border;
		public System.Drawing.Pen SectionBorder
		{
			get { return _border; }
			set { _border = value; }
		}
		
		private Borders _borders;
		public Borders SectionBorders
		{
			get { return _borders; }
			set { _borders = value; }
		}
		
		public Section()
		{
		}

		#region Cloning
		
		public Section(Section section)
			: base(section)
		{
		}

		public override DocumentItem Clone()
		{
			return new Section(this);
		}

		#endregion

		public DocumentBuilder DocumentBuilder
		{
			get { return Parent as DocumentBuilder; }
		}

		public void Save(Document document)
		{
			foreach (PageItem pi in Items)
			{
				if (pi is ISectionObject)
					((ISectionObject) pi).InitializeSave(DocumentBuilder);
			}

			bool more = true;
			while (more)
			{
				//sanity check
				if (document.Pages.Count >= 200)
					break;

				Page page = new Page(Bounds.Size);
				document.Pages.Add(page);

				more = false;

				foreach (PageItem pi in Items)
				{
					if (pi is ISectionObject)
					{
						if (((ISectionObject) pi).SavePage(DocumentBuilder, page))
							more = true;
					}
					else
					{
						page.Items.Add(pi.Clone());
					}
				}
			}

			foreach (PageItem pi in Items)
			{
				if (pi is ISectionObject)
					((ISectionObject) pi).FinalizeSave(DocumentBuilder);
			}
		}
	}

	#endregion

	public class DocumentBuilder : Document
	{
		public DocumentBuilder()
		{
		}

		public DocumentBuilder(string title, string author)
			: base(title, author)
		{
		}

		public DocumentBuilder(string title)
			: base(title)
		{
		}

		#region SectionCollection Class

		public class SectionCollection : DocumentItemCollection
		{
			public SectionCollection(DocumentBuilder builder)
				: base(builder)
			{
			}

			protected override void SetItem(int index, DocumentItem value)
			{
				if (value != null && !(value is Section))
				{
					throw new DocumentException("Child item in document builder must inherit Section");
				}

				base.SetItem (index, value);
			}

			public Document Document
			{
				get { return ((Document) DocumentItem); }
			}

			public new Section this[int index]
			{
				get { return (Section) GetItem(index); }
				set { SetItem(index, value); }
			}

			public void Insert(int index, Section value)
			{
				InsertItem(index, value);
			}

			public void Remove(Section value)
			{
				RemoveItem(value);
			}

			public bool Contains(Section value)
			{
				return base.Contains(value);
			}

			public int IndexOf(Section value)
			{
				return base.IndexOf(value);
			}

			public int Add(Section value)
			{
				return AddItem(value);
			}
		}

		#endregion

		protected override DocumentItemCollection CreateItemCollection()
		{
			return new SectionCollection(this);
		}

		[System.ComponentModel.Browsable(false), System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Content)]
		public SectionCollection Sections
		{
			get { return (SectionCollection) Items; }
		}

		protected override void Dispose(bool disposing)
		{
			if (measureGraphics != null)
				measureGraphics.Dispose();

			base.Dispose (disposing);
		}


		#region Measure

		private System.Drawing.Graphics measureGraphics;
		public System.Drawing.Graphics MeasureGraphics
		{
			get { return measureGraphics; }
		}
		private void CreateMeasureGraphics()
		{
			System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(DefaultPageSize.Width, DefaultPageSize.Height);
			measureGraphics = System.Drawing.Graphics.FromImage(bmp);
		}

		public System.Drawing.SizeF MeasureString(string text, System.Drawing.Font font, 
			int width, System.Drawing.StringFormat format)
		{
			return measureGraphics.MeasureString(text, font, width, format);
		}

		public System.Drawing.SizeF MeasureString(string text, System.Drawing.Font font, 
			System.Drawing.SizeF layoutArea, System.Drawing.StringFormat format)
		{
			return measureGraphics.MeasureString(text, font, layoutArea, format);
		}

		public System.Drawing.SizeF MeasureString(string text, System.Drawing.Font font, 
			int width)
		{
			return measureGraphics.MeasureString(text, font, width);
		}

		public System.Drawing.SizeF MeasureString(string text, System.Drawing.Font font, 
			System.Drawing.SizeF layoutArea)
		{
			return measureGraphics.MeasureString(text, font, layoutArea);
		}

		#endregion

		public virtual Document CreateDocument()
		{
			CreateMeasureGraphics();

			Document document = new Document(Title, Author);

			document.DefaultPageSize = DefaultPageSize;
			document.DefaultMargins = DefaultMargins;
			document.Direction = Direction;
			document.Font = Font;
			document.ForeBrush = ForeBrush;

			int sectionPage = 0;

			foreach (Section section in Sections)
			{
				section.Save(document);

				if (document.Pages.Count > sectionPage)
				{
					document.Pages[sectionPage].NewSection = true;
					sectionPage = document.Pages.Count;
				}
			}

			return document;
		}
	}
}
