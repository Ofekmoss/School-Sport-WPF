using System;
using System.Drawing;
//using Word=Microsoft.Office.Interop.Word;
using System.IO;

namespace Sport.Documents
{
	public enum ImagePosition
	{
		Normal,
		Stretch,
		Center,
		Tile
	}

	public class ImageItem : PageItem
	{
		public ImageItem(Rectangle bounds, Image image, ImagePosition imagePosition)
			: base(bounds)
		{
			_image = image;
			_imagePosition = imagePosition;
		}

		public ImageItem(Rectangle bounds, Image image)
			: this(bounds, image, ImagePosition.Stretch)
		{
		}

		public ImageItem(Rectangle bounds)
			: this(bounds, null, ImagePosition.Stretch)
		{
		}

		public ImageItem()
			: this(new Rectangle(0, 0, 100, 100), null, ImagePosition.Stretch)
		{
		}

		#region Cloning

		public ImageItem(ImageItem item)
			: base(item)
		{
			_image = item._image;
			_imagePosition = item._imagePosition;
		}

		public override DocumentItem Clone()
		{
			return new ImageItem(this);
		}

		#endregion

		private Brush _imageBrush;
		private Image _image;
		public Image Image
		{
			get { return _image; }
			set 
			{ 
				_image = value; 
				_imageBrush = null;
			}
		}

		private ImagePosition _imagePosition;
		public ImagePosition ImagePosition
		{
			get { return _imagePosition; }
			set 
			{ 
				_imagePosition = value; 
				_imageBrush = null;
			}
		}

		public override void PaintItem(Graphics g)
		{
			base.PaintItem (g);

			if (_image != null)
			{
				switch (_imagePosition)
				{
					case (ImagePosition.Normal):
						g.DrawImage(_image, new Rectangle(Bounds.Left, Bounds.Top, _image.Width, _image.Height));
						break;
					case (ImagePosition.Stretch):
						g.DrawImage(_image, Bounds);
						break;
					case (ImagePosition.Center):
						g.DrawImage(_image, new Rectangle(Bounds.Left + (Bounds.Width - _image.Width) / 2, 
							Bounds.Top + (Bounds.Height - _image.Height) / 2, _image.Width, _image.Height));
						break;
					case (ImagePosition.Tile):
						if (_imageBrush == null)
							_imageBrush = new TextureBrush(_image);
						g.FillRectangle(_imageBrush, Bounds);
						break;
				}
			}
		}

		/*
		public override void Save(Word.Document document)
		{
			//string tempFile = document.Application.Path + "\\temp";

			string tempFile = Path.Combine(Path.GetTempPath(), "sportsman_picture");

			_image.Save(tempFile);

			object missing = Type.Missing;
			object width = (float) Bounds.Width * 72 / 100;
			object height = (float) Bounds.Height * 72 / 100;

			Word.Shape shape = document.Shapes.AddCanvas(
				(float) Bounds.Left * 72 / 100,
				(float) Bounds.Top * 72 / 100,
				(float) Bounds.Width * 72 / 100,
				(float) Bounds.Height * 72 / 100, ref missing);

			object i = 0;

			object a = false, b = true;
			shape.CanvasItems.AddPicture(tempFile, ref a, ref b, 
				ref i, ref i, ref width, ref height);
		}
		*/
	}
}
