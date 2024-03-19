using System;
//using Word=Microsoft.Office.Interop.Word;
using Core=Microsoft.Office.Core;

namespace Sport.Documents
{
	/*
	public class WordHelper
	{
		private WordHelper()
		{
		}

		public static void SetFont(Word.Font wordFont, System.Drawing.Font font)
		{
			wordFont.Name = font.Name;
			wordFont.NameBi = font.Name;
			wordFont.Size = font.Size * 0.72f;
			wordFont.SizeBi = font.Size * 0.72f;
			wordFont.Bold = font.Bold ? 1 : 0;
			wordFont.BoldBi = font.Bold ? 1 : 0;
			wordFont.Italic = font.Italic ? 1 : 0;
			wordFont.ItalicBi = font.Italic ? 1 : 0;
		}

		public static void SetFill(Word.FillFormat wordFill, System.Drawing.Brush brush)
		{
			if (brush == null)
			{
				wordFill.Visible = Core.MsoTriState.msoFalse;
			}
			else
			{
				System.Drawing.SolidBrush solidBrush = brush as System.Drawing.SolidBrush;
				if (solidBrush != null)
				{
					wordFill.BackColor.RGB = solidBrush.Color.ToArgb();
				}
			}
		}

		public static void SetLine(Word.LineFormat wordLine, System.Drawing.Pen border)
		{
			if (border == null)
			{
				wordLine.Visible = Core.MsoTriState.msoFalse;
			}
			else
			{
				wordLine.Weight = border.Width;
				try
				{
					wordLine.ForeColor.RGB = border.Color.ToArgb();
				}
				catch
				{
					try
					{
						wordLine.ForeColor.RGB = System.Drawing.Color.Black.ToArgb();
					}
					catch
					{}
				}
			}
		}

		public static Word.WdParagraphAlignment GetAlignment(Direction direction, TextAlignment alignment)
		{
			if (alignment == TextAlignment.Center)
				return Word.WdParagraphAlignment.wdAlignParagraphCenter;
			if ((alignment == TextAlignment.Near && direction == Direction.Right) ||
				(alignment == TextAlignment.Far && direction == Direction.Left))
				return Word.WdParagraphAlignment.wdAlignParagraphRight;

			return Word.WdParagraphAlignment.wdAlignParagraphLeft;
		}

		public static Word.WdColor GetColor(System.Drawing.Color color)
		{
			Word.WdColor wordColor = (Word.WdColor) (color.R | (color.G << 8) | (color.B << 16));
			return wordColor;
		}

		public static void SetShading(Word.Shading wordShading, System.Drawing.Brush brush)
		{
			System.Drawing.SolidBrush solidBrush = brush as System.Drawing.SolidBrush;
			if (solidBrush != null)
			{
				wordShading.BackgroundPatternColor = GetColor(solidBrush.Color);
			}
		}

		public static void SetBorder(Word.Border wordBorder, System.Drawing.Pen border)
		{
			if (border != null)
			{
				try
				{
					wordBorder.Visible = true;
					wordBorder.Color = GetColor(border.Color);
					//wordBorder.LineWidth = (Word.WdLineWidth) (int) (border.Width * 8);
				}
				catch
				{
				}
			}
		}
	}
	*/
}
