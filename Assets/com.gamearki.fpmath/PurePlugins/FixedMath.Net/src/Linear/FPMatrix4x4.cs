using System;

namespace FixMath.NET {
    /// <summary>
    /// Provides XNA-like 4x4 matrix math.
    /// </summary>
    [Serializable]
    public struct FPMatrix4x4 {
        /// <summary>
        /// Value at row 1, column 1 of the matrix.
        /// </summary>
        public FP64 M11;

        /// <summary>
        /// Value at row 1, column 2 of the matrix.
        /// </summary>
        public FP64 M12;

        /// <summary>
        /// Value at row 1, column 3 of the matrix.
        /// </summary>
        public FP64 M13;

        /// <summary>
        /// Value at row 1, column 4 of the matrix.
        /// </summary>
        public FP64 M14;

        /// <summary>
        /// Value at row 2, column 1 of the matrix.
        /// </summary>
        public FP64 M21;

        /// <summary>
        /// Value at row 2, column 2 of the matrix.
        /// </summary>
        public FP64 M22;

        /// <summary>
        /// Value at row 2, column 3 of the matrix.
        /// </summary>
        public FP64 M23;

        /// <summary>
        /// Value at row 2, column 4 of the matrix.
        /// </summary>
        public FP64 M24;

        /// <summary>
        /// Value at row 3, column 1 of the matrix.
        /// </summary>
        public FP64 M31;

        /// <summary>
        /// Value at row 3, column 2 of the matrix.
        /// </summary>
        public FP64 M32;

        /// <summary>
        /// Value at row 3, column 3 of the matrix.
        /// </summary>
        public FP64 M33;

        /// <summary>
        /// Value at row 3, column 4 of the matrix.
        /// </summary>
        public FP64 M34;

        /// <summary>
        /// Value at row 4, column 1 of the matrix.
        /// </summary>
        public FP64 M41;

        /// <summary>
        /// Value at row 4, column 2 of the matrix.
        /// </summary>
        public FP64 M42;

        /// <summary>
        /// Value at row 4, column 3 of the matrix.
        /// </summary>
        public FP64 M43;

        /// <summary>
        /// Value at row 4, column 4 of the matrix.
        /// </summary>
        public FP64 M44;

        /// <summary>
        /// Constructs a new 4 row, 4 column matrix.
        /// </summary>
        /// <param name="m11">Value at row 1, column 1 of the matrix.</param>
        /// <param name="m12">Value at row 1, column 2 of the matrix.</param>
        /// <param name="m13">Value at row 1, column 3 of the matrix.</param>
        /// <param name="m14">Value at row 1, column 4 of the matrix.</param>
        /// <param name="m21">Value at row 2, column 1 of the matrix.</param>
        /// <param name="m22">Value at row 2, column 2 of the matrix.</param>
        /// <param name="m23">Value at row 2, column 3 of the matrix.</param>
        /// <param name="m24">Value at row 2, column 4 of the matrix.</param>
        /// <param name="m31">Value at row 3, column 1 of the matrix.</param>
        /// <param name="m32">Value at row 3, column 2 of the matrix.</param>
        /// <param name="m33">Value at row 3, column 3 of the matrix.</param>
        /// <param name="m34">Value at row 3, column 4 of the matrix.</param>
        /// <param name="m41">Value at row 4, column 1 of the matrix.</param>
        /// <param name="m42">Value at row 4, column 2 of the matrix.</param>
        /// <param name="m43">Value at row 4, column 3 of the matrix.</param>
        /// <param name="m44">Value at row 4, column 4 of the matrix.</param>
        public FPMatrix4x4(FP64 m11, FP64 m12, FP64 m13, FP64 m14,
                      FP64 m21, FP64 m22, FP64 m23, FP64 m24,
                      FP64 m31, FP64 m32, FP64 m33, FP64 m34,
                      FP64 m41, FP64 m42, FP64 m43, FP64 m44) {
            this.M11 = m11;
            this.M12 = m12;
            this.M13 = m13;
            this.M14 = m14;

            this.M21 = m21;
            this.M22 = m22;
            this.M23 = m23;
            this.M24 = m24;

            this.M31 = m31;
            this.M32 = m32;
            this.M33 = m33;
            this.M34 = m34;

            this.M41 = m41;
            this.M42 = m42;
            this.M43 = m43;
            this.M44 = m44;
        }

        /// <summary>
        /// Gets or sets the translation component of the transform.
        /// </summary>
        public FPVector3 Translation {
            get {
                return new FPVector3() {
                    x = M41,
                    y = M42,
                    z = M43
                };
            }
            set {
                M41 = value.x;
                M42 = value.y;
                M43 = value.z;
            }
        }

        /// <summary>
        /// Gets or sets the backward vector of the matrix.
        /// </summary>
        public FPVector3 Backward {
            get {
                FPVector3 vector = new FPVector3();
                vector.x = -M31;
                vector.y = -M32;
                vector.z = -M33;
                return vector;
            }
            set {
                M31 = -value.x;
                M32 = -value.y;
                M33 = -value.z;
            }
        }

        /// <summary>
        /// Gets or sets the down vector of the matrix.
        /// </summary>
        public FPVector3 Down {
            get {
                FPVector3 vector = new FPVector3();
                vector.x = -M21;
                vector.y = -M22;
                vector.z = -M23;
                return vector;
            }
            set {
                M21 = -value.x;
                M22 = -value.y;
                M23 = -value.z;
            }
        }

        /// <summary>
        /// Gets or sets the forward vector of the matrix.
        /// </summary>
        public FPVector3 Forward {
            get {
                FPVector3 vector = new FPVector3();
                vector.x = M31;
                vector.y = M32;
                vector.z = M33;
                return vector;
            }
            set {
                M31 = value.x;
                M32 = value.y;
                M33 = value.z;
            }
        }

        /// <summary>
        /// Gets or sets the left vector of the matrix.
        /// </summary>
        public FPVector3 Left {
            get {
                FPVector3 vector = new FPVector3();
                vector.x = -M11;
                vector.y = -M12;
                vector.z = -M13;
                return vector;
            }
            set {
                M11 = -value.x;
                M12 = -value.y;
                M13 = -value.z;
            }
        }

        /// <summary>
        /// Gets or sets the right vector of the matrix.
        /// </summary>
        public FPVector3 Right {
            get {
                FPVector3 vector = new FPVector3();
                vector.x = M11;
                vector.y = M12;
                vector.z = M13;
                return vector;
            }
            set {
                M11 = value.x;
                M12 = value.y;
                M13 = value.z;
            }
        }

        /// <summary>
        /// Gets or sets the up vector of the matrix.
        /// </summary>
        public FPVector3 Up {
            get {
                FPVector3 vector = new FPVector3();
                vector.x = M21;
                vector.y = M22;
                vector.z = M23;
                return vector;
            }
            set {
                M21 = value.x;
                M22 = value.y;
                M23 = value.z;
            }
        }


        /// <summary>
        /// Computes the determinant of the matrix.
        /// </summary>
        /// <returns></returns>
        public FP64 Determinant() {
            //Compute the re-used 2x2 determinants.
            FP64 det1 = M33 * M44 - M34 * M43;
            FP64 det2 = M32 * M44 - M34 * M42;
            FP64 det3 = M32 * M43 - M33 * M42;
            FP64 det4 = M31 * M44 - M34 * M41;
            FP64 det5 = M31 * M43 - M33 * M41;
            FP64 det6 = M31 * M42 - M32 * M41;
            return
                (M11 * ((M22 * det1 - M23 * det2) + M24 * det3)) -
                (M12 * ((M21 * det1 - M23 * det4) + M24 * det5)) +
                (M13 * ((M21 * det2 - M22 * det4) + M24 * det6)) -
                (M14 * ((M21 * det3 - M22 * det5) + M23 * det6));
        }

        /// <summary>
        /// Transposes the matrix in-place.
        /// </summary>
        public void Transpose() {
            FP64 intermediate = M12;
            M12 = M21;
            M21 = intermediate;

            intermediate = M13;
            M13 = M31;
            M31 = intermediate;

            intermediate = M14;
            M14 = M41;
            M41 = intermediate;

            intermediate = M23;
            M23 = M32;
            M32 = intermediate;

            intermediate = M24;
            M24 = M42;
            M42 = intermediate;

            intermediate = M34;
            M34 = M43;
            M43 = intermediate;
        }

        /// <summary>
        /// Creates a matrix representing the given axis and angle rotation.
        /// </summary>
        /// <param name="axis">Axis around which to rotate.</param>
        /// <param name="angle">Angle to rotate around the axis.</param>
        /// <returns>Matrix created from the axis and angle.</returns>
        public static FPMatrix4x4 CreateFromAxisAngle(FPVector3 axis, FP64 angle) {
            FPMatrix4x4 toReturn;
            CreateFromAxisAngle(ref axis, angle, out toReturn);
            return toReturn;
        }

        /// <summary>
        /// Creates a matrix representing the given axis and angle rotation.
        /// </summary>
        /// <param name="axis">Axis around which to rotate.</param>
        /// <param name="angle">Angle to rotate around the axis.</param>
        /// <param name="result">Matrix created from the axis and angle.</param>
        public static void CreateFromAxisAngle(ref FPVector3 axis, FP64 angle, out FPMatrix4x4 result) {
            FP64 xx = axis.x * axis.x;
            FP64 yy = axis.y * axis.y;
            FP64 zz = axis.z * axis.z;
            FP64 xy = axis.x * axis.y;
            FP64 xz = axis.x * axis.z;
            FP64 yz = axis.y * axis.z;

            FP64 sinAngle = FP64.Sin(angle);
            FP64 oneMinusCosAngle = FP64.One - FP64.Cos(angle);

            result.M11 = FP64.One + oneMinusCosAngle * (xx - FP64.One);
            result.M21 = -axis.z * sinAngle + oneMinusCosAngle * xy;
            result.M31 = axis.y * sinAngle + oneMinusCosAngle * xz;
            result.M41 = FP64.Zero;

            result.M12 = axis.z * sinAngle + oneMinusCosAngle * xy;
            result.M22 = FP64.One + oneMinusCosAngle * (yy - FP64.One);
            result.M32 = -axis.x * sinAngle + oneMinusCosAngle * yz;
            result.M42 = FP64.Zero;

            result.M13 = -axis.y * sinAngle + oneMinusCosAngle * xz;
            result.M23 = axis.x * sinAngle + oneMinusCosAngle * yz;
            result.M33 = FP64.One + oneMinusCosAngle * (zz - FP64.One);
            result.M43 = FP64.Zero;

            result.M14 = FP64.Zero;
            result.M24 = FP64.Zero;
            result.M34 = FP64.Zero;
            result.M44 = FP64.One;
        }

        /// <summary>
        /// Creates a rotation matrix from a quaternion.
        /// </summary>
        /// <param name="quaternion">Quaternion to convert.</param>
        /// <param name="result">Rotation matrix created from the quaternion.</param>
        public static void CreateFromQuaternion(ref FPQuaternion quaternion, out FPMatrix4x4 result) {
            FP64 qX2 = quaternion.x + quaternion.x;
            FP64 qY2 = quaternion.y + quaternion.y;
            FP64 qZ2 = quaternion.z + quaternion.z;
            FP64 XX = qX2 * quaternion.x;
            FP64 YY = qY2 * quaternion.y;
            FP64 ZZ = qZ2 * quaternion.z;
            FP64 XY = qX2 * quaternion.y;
            FP64 XZ = qX2 * quaternion.z;
            FP64 XW = qX2 * quaternion.w;
            FP64 YZ = qY2 * quaternion.z;
            FP64 YW = qY2 * quaternion.w;
            FP64 ZW = qZ2 * quaternion.w;

            result.M11 = FP64.One - YY - ZZ;
            result.M21 = XY - ZW;
            result.M31 = XZ + YW;
            result.M41 = FP64.Zero;

            result.M12 = XY + ZW;
            result.M22 = FP64.One - XX - ZZ;
            result.M32 = YZ - XW;
            result.M42 = FP64.Zero;

            result.M13 = XZ - YW;
            result.M23 = YZ + XW;
            result.M33 = FP64.One - XX - YY;
            result.M43 = FP64.Zero;

            result.M14 = FP64.Zero;
            result.M24 = FP64.Zero;
            result.M34 = FP64.Zero;
            result.M44 = FP64.One;
        }

        /// <summary>
        /// Creates a rotation matrix from a quaternion.
        /// </summary>
        /// <param name="quaternion">Quaternion to convert.</param>
        /// <returns>Rotation matrix created from the quaternion.</returns>
        public static FPMatrix4x4 CreateFromQuaternion(FPQuaternion quaternion) {
            FPMatrix4x4 toReturn;
            CreateFromQuaternion(ref quaternion, out toReturn);
            return toReturn;
        }

        /// <summary>
        /// Multiplies two matrices together.
        /// </summary>
        /// <param name="a">First matrix to multiply.</param>
        /// <param name="b">Second matrix to multiply.</param>
        /// <param name="result">Combined transformation.</param>
        public static void Multiply(ref FPMatrix4x4 a, ref FPMatrix4x4 b, out FPMatrix4x4 result) {
            FP64 resultM11 = a.M11 * b.M11 + a.M12 * b.M21 + a.M13 * b.M31 + a.M14 * b.M41;
            FP64 resultM12 = a.M11 * b.M12 + a.M12 * b.M22 + a.M13 * b.M32 + a.M14 * b.M42;
            FP64 resultM13 = a.M11 * b.M13 + a.M12 * b.M23 + a.M13 * b.M33 + a.M14 * b.M43;
            FP64 resultM14 = a.M11 * b.M14 + a.M12 * b.M24 + a.M13 * b.M34 + a.M14 * b.M44;

            FP64 resultM21 = a.M21 * b.M11 + a.M22 * b.M21 + a.M23 * b.M31 + a.M24 * b.M41;
            FP64 resultM22 = a.M21 * b.M12 + a.M22 * b.M22 + a.M23 * b.M32 + a.M24 * b.M42;
            FP64 resultM23 = a.M21 * b.M13 + a.M22 * b.M23 + a.M23 * b.M33 + a.M24 * b.M43;
            FP64 resultM24 = a.M21 * b.M14 + a.M22 * b.M24 + a.M23 * b.M34 + a.M24 * b.M44;

            FP64 resultM31 = a.M31 * b.M11 + a.M32 * b.M21 + a.M33 * b.M31 + a.M34 * b.M41;
            FP64 resultM32 = a.M31 * b.M12 + a.M32 * b.M22 + a.M33 * b.M32 + a.M34 * b.M42;
            FP64 resultM33 = a.M31 * b.M13 + a.M32 * b.M23 + a.M33 * b.M33 + a.M34 * b.M43;
            FP64 resultM34 = a.M31 * b.M14 + a.M32 * b.M24 + a.M33 * b.M34 + a.M34 * b.M44;

            FP64 resultM41 = a.M41 * b.M11 + a.M42 * b.M21 + a.M43 * b.M31 + a.M44 * b.M41;
            FP64 resultM42 = a.M41 * b.M12 + a.M42 * b.M22 + a.M43 * b.M32 + a.M44 * b.M42;
            FP64 resultM43 = a.M41 * b.M13 + a.M42 * b.M23 + a.M43 * b.M33 + a.M44 * b.M43;
            FP64 resultM44 = a.M41 * b.M14 + a.M42 * b.M24 + a.M43 * b.M34 + a.M44 * b.M44;

            result.M11 = resultM11;
            result.M12 = resultM12;
            result.M13 = resultM13;
            result.M14 = resultM14;

            result.M21 = resultM21;
            result.M22 = resultM22;
            result.M23 = resultM23;
            result.M24 = resultM24;

            result.M31 = resultM31;
            result.M32 = resultM32;
            result.M33 = resultM33;
            result.M34 = resultM34;

            result.M41 = resultM41;
            result.M42 = resultM42;
            result.M43 = resultM43;
            result.M44 = resultM44;
        }


        /// <summary>
        /// Multiplies two matrices together.
        /// </summary>
        /// <param name="a">First matrix to multiply.</param>
        /// <param name="b">Second matrix to multiply.</param>
        /// <returns>Combined transformation.</returns>
        public static FPMatrix4x4 Multiply(FPMatrix4x4 a, FPMatrix4x4 b) {
            FPMatrix4x4 result;
            Multiply(ref a, ref b, out result);
            return result;
        }


        /// <summary>
        /// Scales all components of the matrix.
        /// </summary>
        /// <param name="matrix">Matrix to scale.</param>
        /// <param name="scale">Amount to scale.</param>
        /// <param name="result">Scaled matrix.</param>
        public static void Multiply(ref FPMatrix4x4 matrix, FP64 scale, out FPMatrix4x4 result) {
            result.M11 = matrix.M11 * scale;
            result.M12 = matrix.M12 * scale;
            result.M13 = matrix.M13 * scale;
            result.M14 = matrix.M14 * scale;

            result.M21 = matrix.M21 * scale;
            result.M22 = matrix.M22 * scale;
            result.M23 = matrix.M23 * scale;
            result.M24 = matrix.M24 * scale;

            result.M31 = matrix.M31 * scale;
            result.M32 = matrix.M32 * scale;
            result.M33 = matrix.M33 * scale;
            result.M34 = matrix.M34 * scale;

            result.M41 = matrix.M41 * scale;
            result.M42 = matrix.M42 * scale;
            result.M43 = matrix.M43 * scale;
            result.M44 = matrix.M44 * scale;
        }

        /// <summary>
        /// Multiplies two matrices together.
        /// </summary>
        /// <param name="a">First matrix to multiply.</param>
        /// <param name="b">Second matrix to multiply.</param>
        /// <returns>Combined transformation.</returns>
        public static FPMatrix4x4 operator *(FPMatrix4x4 a, FPMatrix4x4 b) {
            FPMatrix4x4 toReturn;
            Multiply(ref a, ref b, out toReturn);
            return toReturn;
        }

        /// <summary>
        /// Scales all components of the matrix by the given value.
        /// </summary>
        /// <param name="m">First matrix to multiply.</param>
        /// <param name="f">Scaling value to apply to all components of the matrix.</param>
        /// <returns>Product of the multiplication.</returns>
        public static FPMatrix4x4 operator *(FPMatrix4x4 m, FP64 f) {
            FPMatrix4x4 result;
            Multiply(ref m, f, out result);
            return result;
        }

        /// <summary>
        /// Scales all components of the matrix by the given value.
        /// </summary>
        /// <param name="m">First matrix to multiply.</param>
        /// <param name="f">Scaling value to apply to all components of the matrix.</param>
        /// <returns>Product of the multiplication.</returns>
        public static FPMatrix4x4 operator *(FP64 f, FPMatrix4x4 m) {
            FPMatrix4x4 result;
            Multiply(ref m, f, out result);
            return result;
        }

        /// <summary>
        /// Transforms a vector using a matrix.
        /// </summary>
        /// <param name="v">Vector to transform.</param>
        /// <param name="matrix">Transform to apply to the vector.</param>
        /// <param name="result">Transformed vector.</param>
        public static void Transform(ref FPVector4 v, ref FPMatrix4x4 matrix, out FPVector4 result) {
            FP64 vX = v.x;
            FP64 vY = v.y;
            FP64 vZ = v.z;
            FP64 vW = v.w;
            result.x = vX * matrix.M11 + vY * matrix.M21 + vZ * matrix.M31 + vW * matrix.M41;
            result.y = vX * matrix.M12 + vY * matrix.M22 + vZ * matrix.M32 + vW * matrix.M42;
            result.z = vX * matrix.M13 + vY * matrix.M23 + vZ * matrix.M33 + vW * matrix.M43;
            result.w = vX * matrix.M14 + vY * matrix.M24 + vZ * matrix.M34 + vW * matrix.M44;
        }

        /// <summary>
        /// Transforms a vector using a matrix.
        /// </summary>
        /// <param name="v">Vector to transform.</param>
        /// <param name="matrix">Transform to apply to the vector.</param>
        /// <returns>Transformed vector.</returns>
        public static FPVector4 Transform(FPVector4 v, FPMatrix4x4 matrix) {
            FPVector4 toReturn;
            Transform(ref v, ref matrix, out toReturn);
            return toReturn;
        }

        /// <summary>
        /// Transforms a vector using the transpose of a matrix.
        /// </summary>
        /// <param name="v">Vector to transform.</param>
        /// <param name="matrix">Transform to tranpose and apply to the vector.</param>
        /// <param name="result">Transformed vector.</param>
        public static void TransformTranspose(ref FPVector4 v, ref FPMatrix4x4 matrix, out FPVector4 result) {
            FP64 vX = v.x;
            FP64 vY = v.y;
            FP64 vZ = v.z;
            FP64 vW = v.w;
            result.x = vX * matrix.M11 + vY * matrix.M12 + vZ * matrix.M13 + vW * matrix.M14;
            result.y = vX * matrix.M21 + vY * matrix.M22 + vZ * matrix.M23 + vW * matrix.M24;
            result.z = vX * matrix.M31 + vY * matrix.M32 + vZ * matrix.M33 + vW * matrix.M34;
            result.w = vX * matrix.M41 + vY * matrix.M42 + vZ * matrix.M43 + vW * matrix.M44;
        }

        /// <summary>
        /// Transforms a vector using the transpose of a matrix.
        /// </summary>
        /// <param name="v">Vector to transform.</param>
        /// <param name="matrix">Transform to tranpose and apply to the vector.</param>
        /// <returns>Transformed vector.</returns>
        public static FPVector4 TransformTranspose(FPVector4 v, FPMatrix4x4 matrix) {
            FPVector4 toReturn;
            TransformTranspose(ref v, ref matrix, out toReturn);
            return toReturn;
        }

        /// <summary>
        /// Transforms a vector using a matrix.
        /// </summary>
        /// <param name="v">Vector to transform.</param>
        /// <param name="matrix">Transform to apply to the vector.</param>
        /// <param name="result">Transformed vector.</param>
        public static void Transform(ref FPVector3 v, ref FPMatrix4x4 matrix, out FPVector4 result) {
            result.x = v.x * matrix.M11 + v.y * matrix.M21 + v.z * matrix.M31 + matrix.M41;
            result.y = v.x * matrix.M12 + v.y * matrix.M22 + v.z * matrix.M32 + matrix.M42;
            result.z = v.x * matrix.M13 + v.y * matrix.M23 + v.z * matrix.M33 + matrix.M43;
            result.w = v.x * matrix.M14 + v.y * matrix.M24 + v.z * matrix.M34 + matrix.M44;
        }

        /// <summary>
        /// Transforms a vector using a matrix.
        /// </summary>
        /// <param name="v">Vector to transform.</param>
        /// <param name="matrix">Transform to apply to the vector.</param>
        /// <returns>Transformed vector.</returns>
        public static FPVector4 Transform(FPVector3 v, FPMatrix4x4 matrix) {
            FPVector4 toReturn;
            Transform(ref v, ref matrix, out toReturn);
            return toReturn;
        }

        /// <summary>
        /// Transforms a vector using the transpose of a matrix.
        /// </summary>
        /// <param name="v">Vector to transform.</param>
        /// <param name="matrix">Transform to tranpose and apply to the vector.</param>
        /// <param name="result">Transformed vector.</param>
        public static void TransformTranspose(ref FPVector3 v, ref FPMatrix4x4 matrix, out FPVector4 result) {
            result.x = v.x * matrix.M11 + v.y * matrix.M12 + v.z * matrix.M13 + matrix.M14;
            result.y = v.x * matrix.M21 + v.y * matrix.M22 + v.z * matrix.M23 + matrix.M24;
            result.z = v.x * matrix.M31 + v.y * matrix.M32 + v.z * matrix.M33 + matrix.M34;
            result.w = v.x * matrix.M41 + v.y * matrix.M42 + v.z * matrix.M43 + matrix.M44;
        }

        /// <summary>
        /// Transforms a vector using the transpose of a matrix.
        /// </summary>
        /// <param name="v">Vector to transform.</param>
        /// <param name="matrix">Transform to tranpose and apply to the vector.</param>
        /// <returns>Transformed vector.</returns>
        public static FPVector4 TransformTranspose(FPVector3 v, FPMatrix4x4 matrix) {
            FPVector4 toReturn;
            TransformTranspose(ref v, ref matrix, out toReturn);
            return toReturn;
        }

        /// <summary>
        /// Transforms a vector using a matrix.
        /// </summary>
        /// <param name="v">Vector to transform.</param>
        /// <param name="matrix">Transform to apply to the vector.</param>
        /// <param name="result">Transformed vector.</param>
        public static void Transform(ref FPVector3 v, ref FPMatrix4x4 matrix, out FPVector3 result) {
            FP64 vX = v.x;
            FP64 vY = v.y;
            FP64 vZ = v.z;
            result.x = vX * matrix.M11 + vY * matrix.M21 + vZ * matrix.M31 + matrix.M41;
            result.y = vX * matrix.M12 + vY * matrix.M22 + vZ * matrix.M32 + matrix.M42;
            result.z = vX * matrix.M13 + vY * matrix.M23 + vZ * matrix.M33 + matrix.M43;
        }

        /// <summary>
        /// Transforms a vector using the transpose of a matrix.
        /// </summary>
        /// <param name="v">Vector to transform.</param>
        /// <param name="matrix">Transform to tranpose and apply to the vector.</param>
        /// <param name="result">Transformed vector.</param>
        public static void TransformTranspose(ref FPVector3 v, ref FPMatrix4x4 matrix, out FPVector3 result) {
            FP64 vX = v.x;
            FP64 vY = v.y;
            FP64 vZ = v.z;
            result.x = vX * matrix.M11 + vY * matrix.M12 + vZ * matrix.M13 + matrix.M14;
            result.y = vX * matrix.M21 + vY * matrix.M22 + vZ * matrix.M23 + matrix.M24;
            result.z = vX * matrix.M31 + vY * matrix.M32 + vZ * matrix.M33 + matrix.M34;
        }

        /// <summary>
        /// Transforms a vector using a matrix.
        /// </summary>
        /// <param name="v">Vector to transform.</param>
        /// <param name="matrix">Transform to apply to the vector.</param>
        /// <param name="result">Transformed vector.</param>
        public static void TransformNormal(ref FPVector3 v, ref FPMatrix4x4 matrix, out FPVector3 result) {
            FP64 vX = v.x;
            FP64 vY = v.y;
            FP64 vZ = v.z;
            result.x = vX * matrix.M11 + vY * matrix.M21 + vZ * matrix.M31;
            result.y = vX * matrix.M12 + vY * matrix.M22 + vZ * matrix.M32;
            result.z = vX * matrix.M13 + vY * matrix.M23 + vZ * matrix.M33;
        }

        /// <summary>
        /// Transforms a vector using a matrix.
        /// </summary>
        /// <param name="v">Vector to transform.</param>
        /// <param name="matrix">Transform to apply to the vector.</param>
        /// <returns>Transformed vector.</returns>
        public static FPVector3 TransformNormal(FPVector3 v, FPMatrix4x4 matrix) {
            FPVector3 toReturn;
            TransformNormal(ref v, ref matrix, out toReturn);
            return toReturn;
        }

        /// <summary>
        /// Transforms a vector using the transpose of a matrix.
        /// </summary>
        /// <param name="v">Vector to transform.</param>
        /// <param name="matrix">Transform to tranpose and apply to the vector.</param>
        /// <param name="result">Transformed vector.</param>
        public static void TransformNormalTranspose(ref FPVector3 v, ref FPMatrix4x4 matrix, out FPVector3 result) {
            FP64 vX = v.x;
            FP64 vY = v.y;
            FP64 vZ = v.z;
            result.x = vX * matrix.M11 + vY * matrix.M12 + vZ * matrix.M13;
            result.y = vX * matrix.M21 + vY * matrix.M22 + vZ * matrix.M23;
            result.z = vX * matrix.M31 + vY * matrix.M32 + vZ * matrix.M33;
        }

        /// <summary>
        /// Transforms a vector using the transpose of a matrix.
        /// </summary>
        /// <param name="v">Vector to transform.</param>
        /// <param name="matrix">Transform to tranpose and apply to the vector.</param>
        /// <returns>Transformed vector.</returns>
        public static FPVector3 TransformNormalTranspose(FPVector3 v, FPMatrix4x4 matrix) {
            FPVector3 toReturn;
            TransformNormalTranspose(ref v, ref matrix, out toReturn);
            return toReturn;
        }


        /// <summary>
        /// Transposes the matrix.
        /// </summary>
        /// <param name="m">Matrix to transpose.</param>
        /// <param name="transposed">Matrix to transpose.</param>
        public static void Transpose(ref FPMatrix4x4 m, out FPMatrix4x4 transposed) {
            FP64 intermediate = m.M12;
            transposed.M12 = m.M21;
            transposed.M21 = intermediate;

            intermediate = m.M13;
            transposed.M13 = m.M31;
            transposed.M31 = intermediate;

            intermediate = m.M14;
            transposed.M14 = m.M41;
            transposed.M41 = intermediate;

            intermediate = m.M23;
            transposed.M23 = m.M32;
            transposed.M32 = intermediate;

            intermediate = m.M24;
            transposed.M24 = m.M42;
            transposed.M42 = intermediate;

            intermediate = m.M34;
            transposed.M34 = m.M43;
            transposed.M43 = intermediate;

            transposed.M11 = m.M11;
            transposed.M22 = m.M22;
            transposed.M33 = m.M33;
            transposed.M44 = m.M44;
        }

        /// <summary>
        /// Inverts the matrix.
        /// </summary>
        /// <param name="m">Matrix to invert.</param>
        /// <param name="inverted">Inverted version of the matrix.</param>
        public static void Invert(ref FPMatrix4x4 m, out FPMatrix4x4 inverted) {
            FPMatrix4x8.Invert(ref m, out inverted);
        }

        /// <summary>
        /// Inverts the matrix.
        /// </summary>
        /// <param name="m">Matrix to invert.</param>
        /// <returns>Inverted version of the matrix.</returns>
        public static FPMatrix4x4 Invert(FPMatrix4x4 m) {
            FPMatrix4x4 inverted;
            Invert(ref m, out inverted);
            return inverted;
        }

        /// <summary>
        /// Inverts the matrix using a process that only works for rigid transforms.
        /// </summary>
        /// <param name="m">Matrix to invert.</param>
        /// <param name="inverted">Inverted version of the matrix.</param>
        public static void InvertRigid(ref FPMatrix4x4 m, out FPMatrix4x4 inverted) {
            //Invert (transpose) the upper left 3x3 rotation.
            FP64 intermediate = m.M12;
            inverted.M12 = m.M21;
            inverted.M21 = intermediate;

            intermediate = m.M13;
            inverted.M13 = m.M31;
            inverted.M31 = intermediate;

            intermediate = m.M23;
            inverted.M23 = m.M32;
            inverted.M32 = intermediate;

            inverted.M11 = m.M11;
            inverted.M22 = m.M22;
            inverted.M33 = m.M33;

            //Translation component
            var vX = m.M41;
            var vY = m.M42;
            var vZ = m.M43;
            inverted.M41 = -(vX * inverted.M11 + vY * inverted.M21 + vZ * inverted.M31);
            inverted.M42 = -(vX * inverted.M12 + vY * inverted.M22 + vZ * inverted.M32);
            inverted.M43 = -(vX * inverted.M13 + vY * inverted.M23 + vZ * inverted.M33);

            //Last chunk.
            inverted.M14 = FP64.Zero;
            inverted.M24 = FP64.Zero;
            inverted.M34 = FP64.Zero;
            inverted.M44 = FP64.One;
        }

        /// <summary>
        /// Inverts the matrix using a process that only works for rigid transforms.
        /// </summary>
        /// <param name="m">Matrix to invert.</param>
        /// <returns>Inverted version of the matrix.</returns>
        public static FPMatrix4x4 InvertRigid(FPMatrix4x4 m) {
            FPMatrix4x4 inverse;
            InvertRigid(ref m, out inverse);
            return inverse;
        }

        /// <summary>
        /// Gets the 4x4 identity matrix.
        /// </summary>
        public static FPMatrix4x4 Identity {
            get {
                FPMatrix4x4 toReturn;
                toReturn.M11 = FP64.One;
                toReturn.M12 = FP64.Zero;
                toReturn.M13 = FP64.Zero;
                toReturn.M14 = FP64.Zero;

                toReturn.M21 = FP64.Zero;
                toReturn.M22 = FP64.One;
                toReturn.M23 = FP64.Zero;
                toReturn.M24 = FP64.Zero;

                toReturn.M31 = FP64.Zero;
                toReturn.M32 = FP64.Zero;
                toReturn.M33 = FP64.One;
                toReturn.M34 = FP64.Zero;

                toReturn.M41 = FP64.Zero;
                toReturn.M42 = FP64.Zero;
                toReturn.M43 = FP64.Zero;
                toReturn.M44 = FP64.One;
                return toReturn;
            }
        }

        /// <summary>
        /// Creates a right handed orthographic projection.
        /// </summary>
        /// <param name="left">Leftmost coordinate of the projected area.</param>
        /// <param name="right">Rightmost coordinate of the projected area.</param>
        /// <param name="bottom">Bottom coordinate of the projected area.</param>
        /// <param name="top">Top coordinate of the projected area.</param>
        /// <param name="zNear">Near plane of the projection.</param>
        /// <param name="zFar">Far plane of the projection.</param>
        /// <param name="projection">The resulting orthographic projection matrix.</param>
        public static void CreateOrthographicRH(FP64 left, FP64 right, FP64 bottom, FP64 top, FP64 zNear, FP64 zFar, out FPMatrix4x4 projection) {
            FP64 width = right - left;
            FP64 height = top - bottom;
            FP64 depth = zFar - zNear;
            projection.M11 = FP64.Two / width;
            projection.M12 = FP64.Zero;
            projection.M13 = FP64.Zero;
            projection.M14 = FP64.Zero;

            projection.M21 = FP64.Zero;
            projection.M22 = FP64.Two / height;
            projection.M23 = FP64.Zero;
            projection.M24 = FP64.Zero;

            projection.M31 = FP64.Zero;
            projection.M32 = FP64.Zero;
            projection.M33 = -1 / depth;
            projection.M34 = FP64.Zero;

            projection.M41 = (left + right) / -width;
            projection.M42 = (top + bottom) / -height;
            projection.M43 = zNear / -depth;
            projection.M44 = FP64.One;

        }

        /// <summary>
        /// Creates a right-handed perspective matrix.
        /// </summary>
        /// <param name="fieldOfView">Field of view of the perspective in radians.</param>
        /// <param name="aspectRatio">Width of the viewport over the height of the viewport.</param>
        /// <param name="nearClip">Near clip plane of the perspective.</param>
        /// <param name="farClip">Far clip plane of the perspective.</param>
        /// <param name="perspective">Resulting perspective matrix.</param>
        public static void CreatePerspectiveFieldOfViewRH(FP64 fieldOfView, FP64 aspectRatio, FP64 nearClip, FP64 farClip, out FPMatrix4x4 perspective) {
            FP64 h = FP64.One / FP64.Tan(fieldOfView / FP64.Two);
            FP64 w = h / aspectRatio;
            perspective.M11 = w;
            perspective.M12 = FP64.Zero;
            perspective.M13 = FP64.Zero;
            perspective.M14 = FP64.Zero;

            perspective.M21 = FP64.Zero;
            perspective.M22 = h;
            perspective.M23 = FP64.Zero;
            perspective.M24 = FP64.Zero;

            perspective.M31 = FP64.Zero;
            perspective.M32 = FP64.Zero;
            perspective.M33 = farClip / (nearClip - farClip);
            perspective.M34 = -1;

            perspective.M41 = FP64.Zero;
            perspective.M42 = FP64.Zero;
            perspective.M44 = FP64.Zero;
            perspective.M43 = nearClip * perspective.M33;

        }

        /// <summary>
        /// Creates a right-handed perspective matrix.
        /// </summary>
        /// <param name="fieldOfView">Field of view of the perspective in radians.</param>
        /// <param name="aspectRatio">Width of the viewport over the height of the viewport.</param>
        /// <param name="nearClip">Near clip plane of the perspective.</param>
        /// <param name="farClip">Far clip plane of the perspective.</param>
        /// <returns>Resulting perspective matrix.</returns>
        public static FPMatrix4x4 CreatePerspectiveFieldOfViewRH(FP64 fieldOfView, FP64 aspectRatio, FP64 nearClip, FP64 farClip) {
            FPMatrix4x4 perspective;
            CreatePerspectiveFieldOfViewRH(fieldOfView, aspectRatio, nearClip, farClip, out perspective);
            return perspective;
        }

        /// <summary>
        /// Creates a view matrix pointing from a position to a target with the given up vector.
        /// </summary>
        /// <param name="position">Position of the camera.</param>
        /// <param name="target">Target of the camera.</param>
        /// <param name="upVector">Up vector of the camera.</param>
        /// <param name="viewMatrix">Look at matrix.</param>
        public static void CreateLookAtRH(ref FPVector3 position, ref FPVector3 target, ref FPVector3 upVector, out FPMatrix4x4 viewMatrix) {
            FPVector3 forward;
            FPVector3.Subtract(ref target, ref position, out forward);
            CreateViewRH(ref position, ref forward, ref upVector, out viewMatrix);
        }

        /// <summary>
        /// Creates a view matrix pointing from a position to a target with the given up vector.
        /// </summary>
        /// <param name="position">Position of the camera.</param>
        /// <param name="target">Target of the camera.</param>
        /// <param name="upVector">Up vector of the camera.</param>
        /// <returns>Look at matrix.</returns>
        public static FPMatrix4x4 CreateLookAtRH(FPVector3 position, FPVector3 target, FPVector3 upVector) {
            FPMatrix4x4 lookAt;
            FPVector3 forward;
            FPVector3.Subtract(ref target, ref position, out forward);
            CreateViewRH(ref position, ref forward, ref upVector, out lookAt);
            return lookAt;
        }


        /// <summary>
        /// Creates a view matrix pointing in a direction with a given up vector.
        /// </summary>
        /// <param name="position">Position of the camera.</param>
        /// <param name="forward">Forward direction of the camera.</param>
        /// <param name="upVector">Up vector of the camera.</param>
        /// <param name="viewMatrix">Look at matrix.</param>
        public static void CreateViewRH(ref FPVector3 position, ref FPVector3 forward, ref FPVector3 upVector, out FPMatrix4x4 viewMatrix) {
            FPVector3 z;
            FP64 length = forward.Length();
            FPVector3.Divide(ref forward, -length, out z);
            FPVector3 x;
            FPVector3.Cross(ref upVector, ref z, out x);
            x.Normalize();
            FPVector3 y;
            FPVector3.Cross(ref z, ref x, out y);

            viewMatrix.M11 = x.x;
            viewMatrix.M12 = y.x;
            viewMatrix.M13 = z.x;
            viewMatrix.M14 = FP64.Zero;
            viewMatrix.M21 = x.y;
            viewMatrix.M22 = y.y;
            viewMatrix.M23 = z.y;
            viewMatrix.M24 = FP64.Zero;
            viewMatrix.M31 = x.z;
            viewMatrix.M32 = y.z;
            viewMatrix.M33 = z.z;
            viewMatrix.M34 = FP64.Zero;
            FPVector3.Dot(ref x, ref position, out viewMatrix.M41);
            FPVector3.Dot(ref y, ref position, out viewMatrix.M42);
            FPVector3.Dot(ref z, ref position, out viewMatrix.M43);
            viewMatrix.M41 = -viewMatrix.M41;
            viewMatrix.M42 = -viewMatrix.M42;
            viewMatrix.M43 = -viewMatrix.M43;
            viewMatrix.M44 = FP64.One;

        }

        /// <summary>
        /// Creates a view matrix pointing looking in a direction with a given up vector.
        /// </summary>
        /// <param name="position">Position of the camera.</param>
        /// <param name="forward">Forward direction of the camera.</param>
        /// <param name="upVector">Up vector of the camera.</param>
        /// <returns>Look at matrix.</returns>
        public static FPMatrix4x4 CreateViewRH(FPVector3 position, FPVector3 forward, FPVector3 upVector) {
            FPMatrix4x4 lookat;
            CreateViewRH(ref position, ref forward, ref upVector, out lookat);
            return lookat;
        }



        /// <summary>
        /// Creates a world matrix pointing from a position to a target with the given up vector.
        /// </summary>
        /// <param name="position">Position of the transform.</param>
        /// <param name="forward">Forward direction of the transformation.</param>
        /// <param name="upVector">Up vector which is crossed against the forward vector to compute the transform's basis.</param>
        /// <param name="worldMatrix">World matrix.</param>
        public static void CreateWorldRH(ref FPVector3 position, ref FPVector3 forward, ref FPVector3 upVector, out FPMatrix4x4 worldMatrix) {
            FPVector3 z;
            FP64 length = forward.Length();
            FPVector3.Divide(ref forward, -length, out z);
            FPVector3 x;
            FPVector3.Cross(ref upVector, ref z, out x);
            x.Normalize();
            FPVector3 y;
            FPVector3.Cross(ref z, ref x, out y);

            worldMatrix.M11 = x.x;
            worldMatrix.M12 = x.y;
            worldMatrix.M13 = x.z;
            worldMatrix.M14 = FP64.Zero;
            worldMatrix.M21 = y.x;
            worldMatrix.M22 = y.y;
            worldMatrix.M23 = y.z;
            worldMatrix.M24 = FP64.Zero;
            worldMatrix.M31 = z.x;
            worldMatrix.M32 = z.y;
            worldMatrix.M33 = z.z;
            worldMatrix.M34 = FP64.Zero;

            worldMatrix.M41 = position.x;
            worldMatrix.M42 = position.y;
            worldMatrix.M43 = position.z;
            worldMatrix.M44 = FP64.One;

        }


        /// <summary>
        /// Creates a world matrix pointing from a position to a target with the given up vector.
        /// </summary>
        /// <param name="position">Position of the transform.</param>
        /// <param name="forward">Forward direction of the transformation.</param>
        /// <param name="upVector">Up vector which is crossed against the forward vector to compute the transform's basis.</param>
        /// <returns>World matrix.</returns>
        public static FPMatrix4x4 CreateWorldRH(FPVector3 position, FPVector3 forward, FPVector3 upVector) {
            FPMatrix4x4 lookat;
            CreateWorldRH(ref position, ref forward, ref upVector, out lookat);
            return lookat;
        }



        /// <summary>
        /// Creates a matrix representing a translation.
        /// </summary>
        /// <param name="translation">Translation to be represented by the matrix.</param>
        /// <param name="translationMatrix">Matrix representing the given translation.</param>
        public static void CreateTranslation(ref FPVector3 translation, out FPMatrix4x4 translationMatrix) {
            translationMatrix = new FPMatrix4x4 {
                M11 = FP64.One,
                M22 = FP64.One,
                M33 = FP64.One,
                M44 = FP64.One,
                M41 = translation.x,
                M42 = translation.y,
                M43 = translation.z
            };
        }

        /// <summary>
        /// Creates a matrix representing a translation.
        /// </summary>
        /// <param name="translation">Translation to be represented by the matrix.</param>
        /// <returns>Matrix representing the given translation.</returns>
        public static FPMatrix4x4 CreateTranslation(FPVector3 translation) {
            FPMatrix4x4 translationMatrix;
            CreateTranslation(ref translation, out translationMatrix);
            return translationMatrix;
        }

        /// <summary>
        /// Creates a matrix representing the given axis aligned scale.
        /// </summary>
        /// <param name="scale">Scale to be represented by the matrix.</param>
        /// <param name="scaleMatrix">Matrix representing the given scale.</param>
        public static void CreateScale(ref FPVector3 scale, out FPMatrix4x4 scaleMatrix) {
            scaleMatrix = new FPMatrix4x4 {
                M11 = scale.x,
                M22 = scale.y,
                M33 = scale.z,
                M44 = FP64.One
            };
        }

        /// <summary>
        /// Creates a matrix representing the given axis aligned scale.
        /// </summary>
        /// <param name="scale">Scale to be represented by the matrix.</param>
        /// <returns>Matrix representing the given scale.</returns>
        public static FPMatrix4x4 CreateScale(FPVector3 scale) {
            FPMatrix4x4 scaleMatrix;
            CreateScale(ref scale, out scaleMatrix);
            return scaleMatrix;
        }

        /// <summary>
        /// Creates a matrix representing the given axis aligned scale.
        /// </summary>
        /// <param name="x">Scale along the x axis.</param>
        /// <param name="y">Scale along the y axis.</param>
        /// <param name="z">Scale along the z axis.</param>
        /// <param name="scaleMatrix">Matrix representing the given scale.</param>
        public static void CreateScale(FP64 x, FP64 y, FP64 z, out FPMatrix4x4 scaleMatrix) {
            scaleMatrix = new FPMatrix4x4 {
                M11 = x,
                M22 = y,
                M33 = z,
                M44 = FP64.One
            };
        }

        /// <summary>
        /// Creates a matrix representing the given axis aligned scale.
        /// </summary>
        /// <param name="x">Scale along the x axis.</param>
        /// <param name="y">Scale along the y axis.</param>
        /// <param name="z">Scale along the z axis.</param>
        /// <returns>Matrix representing the given scale.</returns>
        public static FPMatrix4x4 CreateScale(FP64 x, FP64 y, FP64 z) {
            FPMatrix4x4 scaleMatrix;
            CreateScale(x, y, z, out scaleMatrix);
            return scaleMatrix;
        }

        /// <summary>
        /// Creates a string representation of the matrix.
        /// </summary>
        /// <returns>A string representation of the matrix.</returns>
        public override string ToString() {
            return "{" + M11 + ", " + M12 + ", " + M13 + ", " + M14 + "} " +
                   "{" + M21 + ", " + M22 + ", " + M23 + ", " + M24 + "} " +
                   "{" + M31 + ", " + M32 + ", " + M33 + ", " + M34 + "} " +
                   "{" + M41 + ", " + M42 + ", " + M43 + ", " + M44 + "}";
        }
    }
}
