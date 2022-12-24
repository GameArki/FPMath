namespace FixMath.NET
{
    /// <summary>
    /// 3 row, 2 column matrix.
    /// </summary>
    public struct FPMatrix3x2
    {
        /// <summary>
        /// Value at row 1, column 1 of the matrix.
        /// </summary>
        public FP64 M11;

        /// <summary>
        /// Value at row 1, column 2 of the matrix.
        /// </summary>
        public FP64 M12;

        /// <summary>
        /// Value at row 2, column 1 of the matrix.
        /// </summary>
        public FP64 M21;

        /// <summary>
        /// Value at row 2, column 2 of the matrix.
        /// </summary>
        public FP64 M22;

        /// <summary>
        /// Value at row 3, column 1 of the matrix.
        /// </summary>
        public FP64 M31;

        /// <summary>
        /// Value at row 3, column 2 of the matrix.
        /// </summary>
        public FP64 M32;


        /// <summary>
        /// Constructs a new 3 row, 2 column matrix.
        /// </summary>
        /// <param name="m11">Value at row 1, column 1 of the matrix.</param>
        /// <param name="m12">Value at row 1, column 2 of the matrix.</param>
        /// <param name="m21">Value at row 2, column 1 of the matrix.</param>
        /// <param name="m22">Value at row 2, column 2 of the matrix.</param>
        /// <param name="m31">Value at row 2, column 1 of the matrix.</param>
        /// <param name="m32">Value at row 2, column 2 of the matrix.</param>
        public FPMatrix3x2(FP64 m11, FP64 m12, FP64 m21, FP64 m22, FP64 m31, FP64 m32)
        {
            M11 = m11;
            M12 = m12;
            M21 = m21;
            M22 = m22;
            M31 = m31;
            M32 = m32;
        }


        /// <summary>
        /// Adds the two matrices together on a per-element basis.
        /// </summary>
        /// <param name="a">First matrix to add.</param>
        /// <param name="b">Second matrix to add.</param>
        /// <param name="result">Sum of the two matrices.</param>
        public static void Add(ref FPMatrix3x2 a, ref FPMatrix3x2 b, out FPMatrix3x2 result)
        {
            FP64 m11 = a.M11 + b.M11;
            FP64 m12 = a.M12 + b.M12;

            FP64 m21 = a.M21 + b.M21;
            FP64 m22 = a.M22 + b.M22;

            FP64 m31 = a.M31 + b.M31;
            FP64 m32 = a.M32 + b.M32;

            result.M11 = m11;
            result.M12 = m12;

            result.M21 = m21;
            result.M22 = m22;

            result.M31 = m31;
            result.M32 = m32;
        }

        /// <summary>
        /// Multiplies the two matrices.
        /// </summary>
        /// <param name="a">First matrix to multiply.</param>
        /// <param name="b">Second matrix to multiply.</param>
        /// <param name="result">Product of the multiplication.</param>
        public static void Multiply(ref FPMatrix3x3 a, ref FPMatrix3x2 b, out FPMatrix3x2 result)
        {
            FP64 resultM11 = a.M11 * b.M11 + a.M12 * b.M21 + a.M13 * b.M31;
            FP64 resultM12 = a.M11 * b.M12 + a.M12 * b.M22 + a.M13 * b.M32;

            FP64 resultM21 = a.M21 * b.M11 + a.M22 * b.M21 + a.M23 * b.M31;
            FP64 resultM22 = a.M21 * b.M12 + a.M22 * b.M22 + a.M23 * b.M32;

            FP64 resultM31 = a.M31 * b.M11 + a.M32 * b.M21 + a.M33 * b.M31;
            FP64 resultM32 = a.M31 * b.M12 + a.M32 * b.M22 + a.M33 * b.M32;

            result.M11 = resultM11;
            result.M12 = resultM12;

            result.M21 = resultM21;
            result.M22 = resultM22;

            result.M31 = resultM31;
            result.M32 = resultM32;
        }

        /// <summary>
        /// Multiplies the two matrices.
        /// </summary>
        /// <param name="a">First matrix to multiply.</param>
        /// <param name="b">Second matrix to multiply.</param>
        /// <param name="result">Product of the multiplication.</param>
        public static void Multiply(ref FPMatrix4x4 a, ref FPMatrix3x2 b, out FPMatrix3x2 result)
        {
            FP64 resultM11 = a.M11 * b.M11 + a.M12 * b.M21 + a.M13 * b.M31;
            FP64 resultM12 = a.M11 * b.M12 + a.M12 * b.M22 + a.M13 * b.M32;

            FP64 resultM21 = a.M21 * b.M11 + a.M22 * b.M21 + a.M23 * b.M31;
            FP64 resultM22 = a.M21 * b.M12 + a.M22 * b.M22 + a.M23 * b.M32;

            FP64 resultM31 = a.M31 * b.M11 + a.M32 * b.M21 + a.M33 * b.M31;
            FP64 resultM32 = a.M31 * b.M12 + a.M32 * b.M22 + a.M33 * b.M32;

            result.M11 = resultM11;
            result.M12 = resultM12;

            result.M21 = resultM21;
            result.M22 = resultM22;

            result.M31 = resultM31;
            result.M32 = resultM32;
        }

        /// <summary>
        /// Negates every element in the matrix.
        /// </summary>
        /// <param name="matrix">Matrix to negate.</param>
        /// <param name="result">Negated matrix.</param>
        public static void Negate(ref FPMatrix3x2 matrix, out FPMatrix3x2 result)
        {
            FP64 m11 = -matrix.M11;
            FP64 m12 = -matrix.M12;

            FP64 m21 = -matrix.M21;
            FP64 m22 = -matrix.M22;

            FP64 m31 = -matrix.M31;
            FP64 m32 = -matrix.M32;

            result.M11 = m11;
            result.M12 = m12;

            result.M21 = m21;
            result.M22 = m22;

            result.M31 = m31;
            result.M32 = m32;
        }

        /// <summary>
        /// Subtracts the two matrices from each other on a per-element basis.
        /// </summary>
        /// <param name="a">First matrix to subtract.</param>
        /// <param name="b">Second matrix to subtract.</param>
        /// <param name="result">Difference of the two matrices.</param>
        public static void Subtract(ref FPMatrix3x2 a, ref FPMatrix3x2 b, out FPMatrix3x2 result)
        {
            FP64 m11 = a.M11 - b.M11;
            FP64 m12 = a.M12 - b.M12;

            FP64 m21 = a.M21 - b.M21;
            FP64 m22 = a.M22 - b.M22;

            FP64 m31 = a.M31 - b.M31;
            FP64 m32 = a.M32 - b.M32;

            result.M11 = m11;
            result.M12 = m12;

            result.M21 = m21;
            result.M22 = m22;

            result.M31 = m31;
            result.M32 = m32;
        }

        /// <summary>
        /// Transforms the vector by the matrix.
        /// </summary>
        /// <param name="v">Vector2 to transform.  Considered to be a column vector for purposes of multiplication.</param>
        /// <param name="matrix">Matrix to use as the transformation.</param>
        /// <param name="result">Column vector product of the transformation.</param>
        public static void Transform(ref FPVector2 v, ref FPMatrix3x2 matrix, out FPVector3 result)
        {
#if !WINDOWS
            result = new FPVector3();
#endif
            result.x = matrix.M11 * v.x + matrix.M12 * v.y;
            result.y = matrix.M21 * v.x + matrix.M22 * v.y;
            result.z = matrix.M31 * v.x + matrix.M32 * v.y;
        }

        /// <summary>
        /// Transforms the vector by the matrix.
        /// </summary>
        /// <param name="v">Vector2 to transform.  Considered to be a row vector for purposes of multiplication.</param>
        /// <param name="matrix">Matrix to use as the transformation.</param>
        /// <param name="result">Row vector product of the transformation.</param>
        public static void Transform(ref FPVector3 v, ref FPMatrix3x2 matrix, out FPVector2 result)
        {
#if !WINDOWS
            result = new FPVector2();
#endif
            result.x = v.x * matrix.M11 + v.y * matrix.M21 + v.z * matrix.M31;
            result.y = v.x * matrix.M12 + v.y * matrix.M22 + v.z * matrix.M32;
        }


        /// <summary>
        /// Computes the transposed matrix of a matrix.
        /// </summary>
        /// <param name="matrix">Matrix to transpose.</param>
        /// <param name="result">Transposed matrix.</param>
        public static void Transpose(ref FPMatrix3x2 matrix, out FPMatrix2x3 result)
        {
            result.M11 = matrix.M11;
            result.M12 = matrix.M21;
            result.M13 = matrix.M31;

            result.M21 = matrix.M12;
            result.M22 = matrix.M22;
            result.M23 = matrix.M32;
        }


        /// <summary>
        /// Creates a string representation of the matrix.
        /// </summary>
        /// <returns>A string representation of the matrix.</returns>
        public override string ToString()
        {
            return "{" + M11 + ", " + M12 + "} " +
                   "{" + M21 + ", " + M22 + "} " +
                   "{" + M31 + ", " + M32 + "}";
        }
    }
}