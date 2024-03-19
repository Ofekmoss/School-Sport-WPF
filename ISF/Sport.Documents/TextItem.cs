using System;
using System.Drawing;
//using Word=Microsoft.Office.Interop.Word;
using Core=Microsoft.Office.Core;

namespace Sport.Documents
{
	#region ITextItemStyle Interface

	public enum TextAlignment
	{
		Default = -1,
		Near = StringAlignment.Near,
		Far = StringAlignment.Far,
		Center = StringAlignment.Center
	}

	public interface ITextItemStyle
	{
		TextAlignment Alignment { get; }
		TextAlignment LineAlignment { get; }
	}

	#endregion

	#region TextItem Class

	public class TextItem : PageItem, ITextItemStyle
	{
		public TextItem(Rectangle bounds, string text)
			: base(bounds)
		{
			_text = text;
			_alignment = TextAlignment.Default;
			_lineAlignment = TextAlignment.Default;
		}

		public TextItem(string text)
			: this(new Rectangle(0, 0, 50, 20), text)
		{
		}

		public TextItem()
			: this(new Rectangle(0, 0, 50, 20), null)
		{
		}

		#region Cloning

		public TextItem(TextItem item)
			: base(item)
		{
			_text = item._text;
			_alignment = item._alignment;
			_lineAlignment = item._lineAlignment;
		}

		public override DocumentItem Clone()
		{
			return new TextItem(this);
		}


		#endregion

		private string _text;
		public string Text
		{
			get { return _text; }
			set { _text = value; }
		}

		private TextAlignment _alignment;
		public TextAlignment Alignment
		{
			get 
			{ 
				if (_alignment == TextAlignment.Default)
					return TextAlignment.Near;
				return _alignment; 
			}
			set { _alignment = value; }
		}

		private TextAlignment _lineAlignment;
		public TextAlignment LineAlignment
		{
			get 
			{ 
				if (_lineAlignment == TextAlignment.Default)
					return TextAlignment.Near;
				return _lineAlignment; 
			}
			set { _lineAlignment = value; }
		}

		protected virtual string GetText()
		{
			return _text;
		}

		public override void PaintItem(Graphics g)
		{
			base.PaintItem (g);

			StringFormat sf = new StringFormat();
			if (Direction == Direction.Right)
				sf.FormatFlags = StringFormatFlags.DirectionRightToLeft;
			sf.Alignment = (StringAlignment) Alignment;
			sf.LineAlignment = (StringAlignment) LineAlignment;

			string text = GetText();
			if (text != null)
			{
				g.DrawString(text, Font, ForeBrush, Bounds, sf);
			}
		}

		/*
		public override void Save(Word.Document document)
		{
			object missing = Type.Missing;
			Word.Shape shape = document.Shapes.AddTextbox(Microsoft.Office.Core.MsoTextOrientation.msoTextOrientationHorizontal,
				(float) Bounds.Left * 72 / 100, (float) Bounds.Top * 72 / 100, 
				(float) Bounds.Width * 72 / 100, (float) Bounds.Height * 72 / 100, 
				ref missing);

			shape.TextFrame.TextRange.ParagraphFormat.Alignment = 
				WordHelper.GetAlignment(Direction, Alignment);

			WordHelper.SetFill(shape.Fill, BackBrush);
			WordHelper.SetLine(shape.Line, Border);

			shape.TextFrame.MarginLeft = 0;
			shape.TextFrame.MarginTop = 0;
			shape.TextFrame.MarginRight = 0;
			shape.TextFrame.MarginBottom = 0;

			WordHelper.SetFont(shape.TextFrame.TextRange.Font, Font);

			shape.TextFrame.TextRange.Text = GetText();
		}
		*/
	}

	#endregion

	#region FieldTextItem Class

	public enum TextField
	{
		Page,
		PageCount,
		SectionPage,
		SectionPageCount,
		Title,
		Author
	}

	public class FieldTextItem : TextItem
	{
		protected override string GetText()
		{
			Page page = Page;
			Document document = page.Document;
			object[] fields = new object[] {
											   page.PageNumber,
											   document.Pages.Count,
											   page.SectionPageNumber,
											   document.GetSectionPageCount(page.Section),
											   document.Title,
											   document.Author
										   };

			return String.Format(Text, fields);
		}

		public FieldTextItem(Rectangle bounds, string text)
			: base(bounds, text)
		{
		}

		public FieldTextItem(string text)
			: base(text)
		{
		}

		public FieldTextItem()
			: base()
		{
		}

		#region Cloning

		public FieldTextItem(FieldTextItem item)
			: base(item)
		{
		}

		public override DocumentItem Clone()
		{
			return new FieldTextItem(this);
		}

		#endregion
	}

	#endregion
}
