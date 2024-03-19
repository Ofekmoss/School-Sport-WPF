using System;

namespace SportSite.Common.Transformer
{
	/// <summary>
	/// 3D vector
	/// </summary>
	public class Vector3
	{
		#region Private

		double[] m_Vector;

		#endregion

		#region Indexers

		public double this[int index]
		{
			get { return m_Vector[index]; }
			set { m_Vector[index] = value; }
		}

		#endregion

		#region Constructors

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

		#endregion

		#region Set

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

		#endregion
	}
}
