using System;

namespace FixMath.NET
{
	static class FPMatrix4x8
	{
		[ThreadStatic] private static FP64[,] Matrix;

		public static bool Invert(ref FPMatrix4x4 m, out FPMatrix4x4 r)
		{
			if (Matrix == null)
				Matrix = new FP64[4, 8];
			FP64[,] M = Matrix;

			M[0, 0] = m.M11;
			M[0, 1] = m.M12;
			M[0, 2] = m.M13;
			M[0, 3] = m.M14;
			M[1, 0] = m.M21;
			M[1, 1] = m.M22;
			M[1, 2] = m.M23;
			M[1, 3] = m.M24;
			M[2, 0] = m.M31;
			M[2, 1] = m.M32;
			M[2, 2] = m.M33;
			M[2, 3] = m.M34;
			M[3, 0] = m.M41;
			M[3, 1] = m.M42;
			M[3, 2] = m.M43;
			M[3, 3] = m.M44;

			M[0, 4] = FP64.One;
			M[0, 5] = FP64.Zero;
			M[0, 6] = FP64.Zero;
			M[0, 7] = FP64.Zero;
			M[1, 4] = FP64.Zero;
			M[1, 5] = FP64.One;
			M[1, 6] = FP64.Zero;
			M[1, 7] = FP64.Zero;
			M[2, 4] = FP64.Zero;
			M[2, 5] = FP64.Zero;
			M[2, 6] = FP64.One;
			M[2, 7] = FP64.Zero;
			M[3, 4] = FP64.Zero;
			M[3, 5] = FP64.Zero;
			M[3, 6] = FP64.Zero;
			M[3, 7] = FP64.One;


			if (!FPMatrix3x6.Gauss(M, 4, 8))
			{
				r = new FPMatrix4x4();
				return false;
			}
			r = new FPMatrix4x4(
				// m11...m14
				M[0, 4],
				M[0, 5],
				M[0, 6],
				M[0, 7],

				// m21...m24				
				M[1, 4],
				M[1, 5],
				M[1, 6],
				M[1, 7],

				// m31...m34
				M[2, 4],
				M[2, 5],
				M[2, 6],
				M[2, 7],

				// m41...m44
				M[3, 4],
				M[3, 5],
				M[3, 6],
				M[3, 7]
				);
			return true;
		}
	}
}
