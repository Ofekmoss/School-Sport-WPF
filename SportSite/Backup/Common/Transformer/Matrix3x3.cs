using System;

namespace SportSite.Common.Transformer
{
	/// <summary>
	/// A 3x3 Matrix.
	/// </summary>
	public class Matrix3x3
	{
		#region Private

		private double[ , ] m_Matrix;

		#endregion

		#region Indexers

		public double this[int row,int column]
		{
			get { return m_Matrix[row,column]; }
			set { m_Matrix[row,column] = value; }
		}

		#endregion

		#region Constructors

		public Matrix3x3()
		{
			m_Matrix = new double[3,3];
			for (int i = 0;i < 3;i++)
				for (int j = 0;j < 3;j++)
					m_Matrix[i,j] = 0.0;
		}
		
		#endregion
	
		#region Mathematic Operations

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

		#endregion
	
		#region Static Matrices

		#region Transformation Matrices

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

		#endregion

		#region Standard Matrices

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

		#endregion

		#region Misc Math

		/// <summary>
		/// Converts degrees to radians.
		/// </summary>
		/// <param name="degrees"></param>
		/// <returns></returns>
		public static double Deg2Rad(double degrees)
		{
			return (degrees * System.Math.PI) / 180.0;
		}

		#endregion

		#endregion
	}
}
