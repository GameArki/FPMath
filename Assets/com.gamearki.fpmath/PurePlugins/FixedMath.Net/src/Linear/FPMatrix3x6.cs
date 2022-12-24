using System;
using System.Threading;

namespace FixMath.NET
{
	static class FPMatrix3x6
	{
		[ThreadStatic] private static FP64[,] Matrix;

		public static bool Gauss(FP64[,] M, int m, int n)
		{
			// Perform Gauss-Jordan elimination
			for (int k = 0; k < m; k++)
			{
				FP64 maxValue = FP64.Abs(M[k, k]);
				int iMax = k;
				for (int i = k+1; i < m; i++)
				{
					FP64 value = FP64.Abs(M[i, k]);
					if (value >= maxValue)
					{
						maxValue = value;
						iMax = i;
					}
				}
				if (maxValue == FP64.Zero)
					return false;
				// Swap rows k, iMax
				if (k != iMax)
				{
					for (int j = 0; j < n; j++)
					{
						FP64 temp = M[k, j];
						M[k, j] = M[iMax, j];
						M[iMax, j] = temp;
					}
				}

				// Divide row by pivot
				FP64 pivotInverse = FP64.One / M[k, k];

				M[k, k] = FP64.One;
				for (int j = k + 1; j < n; j++)
				{
					M[k, j] *= pivotInverse;
				}

				// Subtract row k from other rows
				for (int i = 0; i < m; i++)
				{
					if (i == k)
						continue;
					FP64 f = M[i, k];					
					for (int j = k + 1; j < n; j++)
					{
						M[i, j] = M[i, j] - M[k, j] * f;
					}
					M[i, k] = FP64.Zero;
				}
			}
			return true;
		}
		
		public static bool Invert(ref FPMatrix3x3 m, out FPMatrix3x3 r)
		{
			if (Matrix == null)
				 Matrix = new FP64[3, 6];
			FP64[,] M = Matrix;

			// Initialize temporary matrix
			M[0, 0] = m.M11;
			M[0, 1] = m.M12;
			M[0, 2] = m.M13;
			M[1, 0] = m.M21;
			M[1, 1] = m.M22;
			M[1, 2] = m.M23;
			M[2, 0] = m.M31;
			M[2, 1] = m.M32;
			M[2, 2] = m.M33;

			M[0, 3] = FP64.One;
			M[0, 4] = FP64.Zero;
			M[0, 5] = FP64.Zero;
			M[1, 3] = FP64.Zero;
			M[1, 4] = FP64.One;
			M[1, 5] = FP64.Zero;
			M[2, 3] = FP64.Zero;
			M[2, 4] = FP64.Zero;
			M[2, 5] = FP64.One;

			if (!Gauss(M, 3, 6))
			{
				r = new FPMatrix3x3();
				return false;
			}
			r = new FPMatrix3x3(
				// m11...m13
				M[0, 3],
				M[0, 4],
				M[0, 5],

				// m21...m23
				M[1, 3],
				M[1, 4],
				M[1, 5],

				// m31...m33
				M[2, 3],
				M[2, 4],
				M[2, 5]
				);
			return true;
		}
	}
}
