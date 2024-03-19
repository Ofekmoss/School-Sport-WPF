<%@ Import namespace="System.Drawing" %>
<script language="C#" runat="server">
private void Page_Load(object sender, System.EventArgs e)
{
	string strImagePath=ToStringDef(Request.QueryString["image"], "");
	string strThumbnailPath=ToStringDef(Request.QueryString["thumb"], "");
	int desiredWidth=ToIntDef(Request.QueryString["width"], -1);
	int desiredHeight=ToIntDef(Request.QueryString["height"], -1);
	if (strImagePath.Length == 0)
	{
		Response.Write("missing image path");
		Response.End();
	}
	if (strThumbnailPath.Length == 0)
	{
		Response.Write("missing thumbnail path");
		Response.End();
	}
	if ((desiredWidth < 0)&&(desiredHeight < 0))
	{
		Response.Write("missing or invalid image dimensions");
		Response.End();
	}
	if (!System.IO.File.Exists(strImagePath))
	{
		Response.Write("image file does not exist (check ASPNET permissions)");
		Response.End();
	}
	Bitmap bitmap=null;
	BitmapWrapper wrapper=null;
	try
	{
		bitmap = new Bitmap(strImagePath);
		wrapper = new BitmapWrapper();
		wrapper.Load(strImagePath);
	}
	catch (Exception ex)
	{
		Response.Write("failed to load bitmap: "+strImagePath+": "+ex.Message);
		Response.End();
	}
	double scaleFactorX=0;
	double scaleFactorY=0;
	if ((desiredWidth > 0)&&(desiredHeight > 0))
	{
		scaleFactorX = ((double) desiredWidth)/((double) wrapper.Bitmap.Width);
		scaleFactorY = ((double) desiredHeight)/((double) wrapper.Bitmap.Height);
	}
	else
	{
		if (desiredWidth > 0)
		{
			scaleFactorX = ((double) desiredWidth)/((double) wrapper.Bitmap.Width);
			scaleFactorY = scaleFactorX;
		}
		else
		{
			scaleFactorY = ((double) desiredHeight)/((double) wrapper.Bitmap.Height);
			scaleFactorX = scaleFactorY;
		}
	}
	wrapper.Scale(scaleFactorX, scaleFactorY, BitmapWrapper.InterpolationType.LinearInterpolation);
	wrapper.Save(strThumbnailPath);
	Response.Write("OK");
}

public class BitmapWrapper
{
	public enum FlipType
	{
		None = 0,
		Horizontal = 1,
		Vertical = 2,
		HorizontalAndVertical = Horizontal | Vertical
	}
	
	public enum InterpolationType
	{
		NearestPixel = 0,
		LinearInterpolation,
		CubicInterpolation
	}
	
	/// <summary>
	/// Used to descibe an offset of an image after transformation
	/// </summary>
	public struct ImageOffset
	{
		public int minX,maxX;
		public int minY,maxY;
		public static ImageOffset Empty,Zero;
            
		public ImageOffset(int minX,int maxX,int minY,int maxY)
		{
			this.minX = minX;
			this.maxX = maxX;
			this.minY = minY;
			this.maxY = maxY;
		}

		static ImageOffset()
		{
			Empty = new ImageOffset(-1,-1,-1,-1);
			Zero = new ImageOffset(0,0,0,0);
		}
	}

	private string m_loadedFile;

	/// <summary>
	/// Gets or sets the color at a specific pixel location
	/// </summary>
	private Color this[int column,int row]
	{
		get 
		{ 
			if (column >= 0 && column < Size.Width && row >= 0 && row < Size.Height)
				return m_Bitmap.GetPixel(column,row); 
			else
				return Color.Black;
		}
		set { m_Bitmap.SetPixel(column,row,value);	}
	}

	public Size Size
	{
		get { return m_Bitmap.Size; }
	}

	public Point Center
	{
		get 
		{ 
			return new Point(
				(int)(Math.Floor(Size.Width / 2.0)),
				(int)(Math.Floor(Size.Height / 2.0))); 
		}
	}
	
	private bool m_isLoaded;
	/// <summary>
	/// Returns TRUE if an image is loaded.
	/// </summary>
	public bool isLoaded
	{
		get { return m_isLoaded; }
	}

	private Bitmap m_Bitmap;
	/// <summary>
	/// Gets the Bitmap object.
	/// </summary>
	public Bitmap Bitmap
	{
		get { return m_Bitmap; }
	}
		
	/// <summary>
	/// Gets or sets the pixel matrix for the bitmap.
	/// </summary>
	private Color[ , ] PixelMatrix
	{
		get
		{
			if (!isLoaded)
				return null;
				Color[ , ] resultMatrix = new Color[Size.Width,Size.Height];
		
			for (int i = 0;i < Size.Width;i++)
				for (int j = 0;j < Size.Height;j++)
					resultMatrix[i,j] = m_Bitmap.GetPixel(i,j);
				return resultMatrix;
			}

			set
			{
				if (!isLoaded)
					return;

				Color[ , ] incomingMatrix = value;

				for (int i = 0;i < Size.Width;i++)
					for (int j = 0;j < Size.Height;j++)
						m_Bitmap.SetPixel(i,j,incomingMatrix[i,j]);
			}
		}

		public BitmapWrapper()
		{
			m_isLoaded = false;
		}


		/// <summary>
		/// Checks the dirty bit, and acts according to the user.
		/// </summary>
		/// <returns>True if the user still wishes to load (or if the dirty bit is off
		/// False if the user canceled</returns>
		public bool checkDirty()
		{
			return true;
		}

		public void Load(string filename)
		{
			if (!checkDirty())
				return;
			
			try
			{
				m_Bitmap = new Bitmap(filename);
				m_isLoaded = true;
				m_loadedFile = filename;
				releaseImageFile();
			}
			catch (Exception exp)
			{
				m_isLoaded = false;
				throw exp;
			}
			
		}

		public void Save(string filename)
		{
			Save(filename,m_Bitmap.RawFormat);
		}

		public void Save(string filename,System.Drawing.Imaging.ImageFormat fileFormat)
		{
			try
			{
				m_Bitmap.Save(filename,fileFormat);
			}	
			catch (Exception exp)
			{
				throw new Exception("Error saving file: "+exp.Message);
			}
		}

		/// <summary>
		/// Releases the Bitmap member.
		/// This is addressing a bug in .NET that the GDI+ locks the file.
		/// </summary>
		private void releaseImageFile()
		{
			Bitmap tempBitmap = new Bitmap(m_Bitmap.Width,m_Bitmap.Height,m_Bitmap.PixelFormat);
			Graphics tempGraphics = Graphics.FromImage(tempBitmap);
			tempGraphics.DrawImage(m_Bitmap,0,0,m_Bitmap.PhysicalDimension.Width,m_Bitmap.PhysicalDimension.Height);
			tempGraphics.Dispose();
			m_Bitmap.Dispose();
			m_Bitmap = tempBitmap;
		}

		/// <summary>
		/// Rotates the image.
		/// </summary>
		/// <param name="rotateAngle">Rotation angle</param>
		public void Rotate(double rotateAngle,InterpolationType interpolationType)
		{
			Matrix3x3 inverseRotate = Matrix3x3.InverseRotateMatrix(rotateAngle);

			BitmapWrapper.ImageOffset newImageOffset = getOffsetAfterRotation(rotateAngle);
			Size newImageSize = new Size(newImageOffset.maxX - newImageOffset.minX + 1,newImageOffset.maxY - newImageOffset.minY + 1);
		
			ExecTransformation(inverseRotate,interpolationType,newImageSize,newImageOffset);
		}

		/// <summary>
		/// Scales the image.
		/// </summary>
		/// <param name="scaleFactor">Scale factor</param>
		public void Scale(double scaleFactorX, double scaleFactorY,InterpolationType interpolationType)
		{
			int newImageSizeWidth = (int)(Math.Floor(Size.Width * scaleFactorX));
			int newImageSizeHeight = (int)(Math.Floor(Size.Height * scaleFactorY));
			Size newImageSize = new Size(newImageSizeWidth,newImageSizeHeight);

			Matrix3x3 inverseScale = Matrix3x3.InverseScaleMatrix(scaleFactorX,scaleFactorY);
			
			ExecTransformation(inverseScale,interpolationType,newImageSize,BitmapWrapper.ImageOffset.Zero);
		}

		/// <summary>
		/// Flips the image.
		/// </summary>
		/// <param name="flipType">Bitwise enumeration for selecting flip type</param>
		public void Flip(FlipType flipType)
		{
			ExecFlip(flipType);
		}

		/// <summary>
		/// Gets the new offset of the image after rotation.
		/// </summary>
		/// <param name="angle">Angle of rotation</param>
		/// <returns>The new offset of the image</returns>
		private BitmapWrapper.ImageOffset getOffsetAfterRotation(double angle)
		{
			Matrix3x3 rotateMatrix = Matrix3x3.RotateMatrix(angle);
			int maxX = -1,minX = -1;
			int maxY = -1,minY = -1;

			Vector3[] Corners = new Vector3[4];

			Corners[0] = new Vector3(0.0,0.0,1.0);
			Corners[1] = new Vector3(Size.Width - 1,0.0,1.0);
			Corners[2] = new Vector3(Size.Width - 1,Size.Height - 1,1.0);
			Corners[3] = new Vector3(0.0,Size.Height - 1,1.0);

			Vector3 tempVector = rotateMatrix.multiplyRight(Corners[0]);
			maxX = (int)(Math.Ceiling(tempVector[0])); minX = (int)(Math.Ceiling(tempVector[0]));
			maxY = (int)(Math.Ceiling(tempVector[1])); minY = (int)(Math.Ceiling(tempVector[1]));

			for (int i = 1;i < Corners.Length;i++)
			{
				tempVector = rotateMatrix.multiplyRight(Corners[i]);
				maxX = (tempVector[0] > maxX) ? (int)(Math.Ceiling(tempVector[0])) : maxX;
				minX = (tempVector[0] < minX) ? (int)(Math.Ceiling(tempVector[0])) : minX;
				maxY = (tempVector[1] > maxY) ? (int)(Math.Ceiling(tempVector[1])) : maxY;
				minY = (tempVector[1] < minY) ? (int)(Math.Ceiling(tempVector[1])) : minY;
			}
			BitmapWrapper.ImageOffset result = new ImageOffset(minX,maxX,minY,maxY);
			return result;
//			return new Size(maxX - minX + 1,maxY - minY + 1);
		}

		/// <summary>
		/// Executes the transformation and changes the loaded image.
		/// Note: Because this program uses the backward method, one must
		/// send the inverse transformation matrix for the transformation
		/// to work.
		/// </summary>
		/// <param name="transformationMatrix">The transformation matrix</param>
		/// <param name="interpolationType">Type of interpolation</param>
		/// <param name="newImageSize">The size of the new image</param>
		
		private void ExecTransformation(Matrix3x3 transformationMatrix,InterpolationType interpolationType,Size newImageSize,BitmapWrapper.ImageOffset newImageOffset)
		{
			Color[ , ] tempPixelMatrix = new Color[newImageSize.Width,newImageSize.Height];
			for (int i = 0;i < newImageSize.Width;i++)
				for (int j = 0;j < newImageSize.Height;j++)
					tempPixelMatrix[i,j] = Color.Black;

			Vector3 currentPixel = new Vector3();
			Vector3 originPixel;
			
			double numOfNewPixels = newImageSize.Width * newImageSize.Height;
			double currentProgress = 0.0;

			for (int column = 0;column < newImageSize.Width;column++)
				for (int row = 0;row < newImageSize.Height;row++)
				{
					currentPixel.setVector(column + newImageOffset.minX,row + newImageOffset.minY,1.0);
					originPixel = transformationMatrix.multiplyRight(currentPixel);
					tempPixelMatrix[column,row] = getInterpolatedPixel(originPixel,interpolationType);
					
					currentProgress ++;
				}
			System.Drawing.Imaging.PixelFormat oldPixelFormat = m_Bitmap.PixelFormat;
			m_Bitmap.Dispose();
			m_Bitmap = new Bitmap(newImageSize.Width,newImageSize.Height,oldPixelFormat);
			PixelMatrix = tempPixelMatrix;
		}

		/// <summary>
		/// Executes the flip transformation.
		/// </summary>
		/// <param name="flipType">The type of flip (Horizontal, Vetrical or both)</param>
		private void ExecFlip(FlipType flipType)
		{
			Color[ , ] tempPixelMatrix = new Color[Size.Width,Size.Height];

			double numOfNewPixels = Size.Width * Size.Height;
			double currentProgress = 0.0;

			int originColumn;
			int originRow;

			for (int column = 0;column < Size.Width;column++)
				for (int row = 0;row < Size.Height;row++)
				{

					if ((flipType & FlipType.Horizontal) != 0)
						originColumn = (Size.Width - 1) - column;
					else
						originColumn = column;
									
					if ((flipType & FlipType.Vertical) != 0)
						originRow = (Size.Height - 1) - row;
					else
						originRow = row;
					
					tempPixelMatrix[column,row] = this[originColumn,originRow];
						
					currentProgress ++;
				}
				System.Drawing.Imaging.PixelFormat oldPixelFormat = m_Bitmap.PixelFormat;
				Size oldSize = Size;
				m_Bitmap.Dispose();
				m_Bitmap = new Bitmap(oldSize.Width,oldSize.Height,oldPixelFormat);
				PixelMatrix = tempPixelMatrix;
		}

		/// <summary>
		/// Calculates the color of a pixel by interpolation method.
		/// </summary>
		/// <param name="originPixel">The origin pixel</param>
		/// <param name="interpolationType">The interpolation type</param>
		/// <returns>The color of the pixel after interpolation</returns>
		private Color getInterpolatedPixel(Vector3 originPixel,InterpolationType interpolationType)
		{
			Color result = Color.Black;
			switch (interpolationType)
			{
				case InterpolationType.NearestPixel: // Nearest Pixel
					result = NearestPixelInterpolation(originPixel);
					break;

				case InterpolationType.LinearInterpolation: // Linear Interpolation
					result = LinearInterpolation(originPixel);
					break;

				case InterpolationType.CubicInterpolation: // Cubic Interpolation
					result = CubicInterpolation(originPixel);
					break;
			}
			return result;
		}

		/// <summary>
		/// Gets the color of a pixel by Nearest Pixel Interpolation
		/// </summary>
		/// <param name="originPixel">Origin pixel</param>
		/// <returns>Color of the nearest pixel</returns>
		private Color NearestPixelInterpolation(Vector3 originPixel)
		{
			int originX = (int)(Math.Round(originPixel[0]));
			int originY = (int)(Math.Round(originPixel[1]));

			return this[originX,originY];
		}

		private Color LinearInterpolation(Vector3 originPixel)
		{
			int floorX = (int)(Math.Floor(originPixel[0]));
			int floorY = (int)(Math.Floor(originPixel[1]));

			byte resultRed = this[floorX,floorY].R;
			byte resultGreen = this[floorX,floorY].G;
			byte resultBlue = this[floorX,floorY].B;

			double dx = originPixel[0] - floorX;
			double dy = originPixel[1] - floorY;

			Color Cell00,Cell10,Cell01,Cell11;

			if (floorX < Size.Width - 1)
			{
				Cell00 = this[floorX,floorY];
				Cell10 = this[floorX + 1,floorY];

				resultRed = (byte)( ((1 - dx) * resultRed) + (dx * Cell10.R));
				resultGreen = (byte)( ((1 - dx) * resultGreen) + (dx * Cell10.G));
				resultBlue = (byte)( ((1 - dx) * resultBlue) + (dx * Cell10.B));

				if (floorY < Size.Height - 1)
				{
					Cell01 = this[floorX,floorY + 1];
					Cell11 = this[floorX + 1,floorY + 1];

					resultRed = (byte)( ((1 - dy) * resultRed) + (dy * 
						(((1 - dx) * Cell01.R) + (dx * Cell11.R))));
					resultGreen = (byte)( ((1 - dy) * resultGreen) + (dy * 
						(((1 - dx) * Cell01.G) + (dx * Cell11.G))));
					resultBlue = (byte)( ((1 - dy) * resultBlue) + (dy * 
						(((1 - dx) * Cell01.B) + (dx * Cell11.B))));
				}
			}
			else if (floorY < Size.Height - 1)
			{
				Cell01 = this[floorX,floorY + 1];

				resultRed = (byte)( ((1 - dy) * resultRed) + (dy * Cell01.R));
				resultGreen = (byte)( ((1 - dy) * resultGreen) + (dy * Cell01.G));
				resultBlue = (byte)( ((1 - dy) * resultBlue) + (dy * Cell01.B));
			}

			return Color.FromArgb(resultRed,resultGreen,resultBlue);
		}

		private Color CubicInterpolation(Vector3 originPixel)
		{
			int floorX = (int)(Math.Floor(originPixel[0]));
			int floorY = (int)(Math.Floor(originPixel[1]));

			byte resultRed = 0;
			byte resultGreen = 0;
			byte resultBlue = 0;

			double dx = originPixel[0] - floorX;
			double dy = originPixel[1] - floorY;

//			if (floorX < 1 || floorX > Size.Width - 3 || floorY < 1 || floorY > Size.Height - 3)
//				return this[floorX,floorY];

			for (int m = -1;m < 3;m++)
				for (int n = -1;n < 3;n++)
				{
					resultRed += (byte)(this[floorX + m,floorY + n].R * cubicWeight(m - dx) * cubicWeight(dy - n));
					resultGreen += (byte)(this[floorX + m,floorY + n].G * cubicWeight(m - dx) * cubicWeight(dy - n));
					resultBlue += (byte)(this[floorX + m,floorY + n].B * cubicWeight(m - dx) * cubicWeight(dy - n));
				}

			return Color.FromArgb(resultRed,resultGreen,resultBlue);
		}

		private double cubicWeight(double x)
		{
			return (Math.Pow(PositiveFunc(x + 2),3) - (4 * Math.Pow(PositiveFunc(x + 1),3)) + 
				(6 * Math.Pow(PositiveFunc(x),3)) - (4 * Math.Pow(PositiveFunc(x - 1),3))) / 6.0;
		}

		private double PositiveFunc(double x)
		{
			if (x <= 0)
				return 0;
			else
				return x;
		}
	}
	
	public class Matrix3x3
	{
		private double[ , ] m_Matrix;

		public double this[int row,int column]
		{
			get { return m_Matrix[row,column]; }
			set { m_Matrix[row,column] = value; }
		}

		public Matrix3x3()
		{
			m_Matrix = new double[3,3];
			for (int i = 0;i < 3;i++)
				for (int j = 0;j < 3;j++)
					m_Matrix[i,j] = 0.0;
		}

		/// <summary>
		/// Multiplies a vector from the right.
		/// </summary>
		/// <param name="vector">The given vector</param>
		/// <returns>Result vector</returns>
		public Vector3 multiplyRight(Vector3 vector)
		{
			Vector3 resultVector = new Vector3();

			resultVector[0] = 
				(m_Matrix[0,0] * vector[0]) +
				(m_Matrix[1,0] * vector[1]) +
				(m_Matrix[2,0] * vector[2]);

			resultVector[1] = 
				(m_Matrix[0,1] * vector[0]) +
				(m_Matrix[1,1] * vector[1]) +
				(m_Matrix[2,1] * vector[2]);

			resultVector[2] = 
				(m_Matrix[0,2] * vector[0]) +
				(m_Matrix[1,2] * vector[1]) +
				(m_Matrix[2,2] * vector[2]);

			return resultVector;
		}

		/// <summary>
		/// Gets a rotation matrix around the given angle.
		/// </summary>
		/// <param name="angle">the angle</param>
		/// <returns></returns>
		public static Matrix3x3 RotateMatrix(double angle)
		{
			Matrix3x3 matrix = new Matrix3x3();
			
			double radians = Deg2Rad(angle);
			
			matrix[0,0] = Math.Cos(radians);
			matrix[0,1] = Math.Sin(radians);
			matrix[1,0] = - Math.Sin(radians);
			matrix[1,1] = Math.Cos(radians);
			matrix[2,2] = 1.0;

			return matrix;
		}

		/// <summary>
		/// Gets an inverse rotation matrix around the given angle.
		/// </summary>
		/// <param name="angle">the angle</param>
		/// <returns></returns>
		public static Matrix3x3 InverseRotateMatrix(double angle)
		{
			Matrix3x3 matrix = new Matrix3x3();
			
			double radians = Deg2Rad(angle);
			
			matrix[0,0] = Math.Cos(radians);
			matrix[0,1] = - Math.Sin(radians);
			matrix[1,0] = Math.Sin(radians);
			matrix[1,1] = Math.Cos(radians);
			matrix[2,2] = 1.0;

			return matrix;
		}

		/// <summary>
		/// Gets a scale matrix by x and y factors
		/// </summary>
		/// <param name="scaleFactorX">the X factor</param>
		/// <param name="scaleFactorY">the Y factor</param>
		/// <returns>The scale matrix</returns>
		public static Matrix3x3 ScaleMatrix(double scaleFactorX, double scaleFactorY)
		{
			if ((scaleFactorX == 0.0) || (scaleFactorY == 0.0))
				return null;

			Matrix3x3 matrix = new Matrix3x3();
			
			matrix[0,0] = scaleFactorX;
			matrix[1,1] = scaleFactorY;
			matrix[2,2] = 1.0;

			return matrix;
		}

		/// <summary>
		/// Gets an inverse scale matrix by x and y factors
		/// </summary>
		/// <param name="scaleFactorX">the X factor</param>
		/// <param name="scaleFactorY">the Y factor</param>
		/// <returns>The scale matrix</returns>
		public static Matrix3x3 InverseScaleMatrix(double scaleFactorX, double scaleFactorY)
		{
			if ((scaleFactorX == 0.0) || (scaleFactorY == 0.0))
				return null;

			Matrix3x3 matrix = new Matrix3x3();
			
			matrix[0,0] = 1 / scaleFactorX;
			matrix[1,1] = 1 / scaleFactorY;
			matrix[2,2] = 1.0;

			return matrix;
		}

		/// <summary>
		/// Gets an identity matrix
		/// </summary>
		/// <returns>The identity matrix</returns>
		public static Matrix3x3 identityMatrix()
		{
			Matrix3x3 matrix = new Matrix3x3();

			matrix[0,0] = 1.0;
			matrix[1,1] = 1.0;
			matrix[2,2] = 1.0;

			return matrix;
		}

		/// <summary>
		/// Converts degrees to radians.
		/// </summary>
		/// <param name="degrees"></param>
		/// <returns></returns>
		public static double Deg2Rad(double degrees)
		{
			return (degrees * System.Math.PI) / 180.0;
		}
	}
	
	public class Vector3
	{
		double[] m_Vector;

		public double this[int index]
		{
			get { return m_Vector[index]; }
			set { m_Vector[index] = value; }
		}

		public Vector3()
		{
			m_Vector = new double[3];
			for (int i = 0;i < 3;i++)
				m_Vector[i] = 0.0;
		}

		public Vector3(double v1,double v2,double v3)
		{
			m_Vector = new double[3];
			m_Vector[0] = v1;
			m_Vector[1] = v2;
			m_Vector[2] = v3;
		}

		public void setVector(double x,double y,double z)
		{
			this[0] = x;
			this[1] = y;
			this[2] = z;
		}

		public void setVector(Vector3 other)
		{
			this[0] = other[0];
			this[1] = other[1];
			this[2] = other[2];
		}
	}	
	
	public string ToStringDef(object obj, string def)
	{
		if ((obj == null)||(obj == DBNull.Value))
			return def;
		return obj.ToString();
	}
	
	public int ToIntDef(object obj, int def)
	{
		if (obj == null)
			return def;
		try
		{
			return System.Convert.ToInt32(obj);
		}
		catch
		{
			return def;
		}
	}
</script>

