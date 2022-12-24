using System;

namespace FixMath.NET {

    [Serializable]
    public struct FPQuaternion : IEquatable<FPQuaternion> {

        public FP64 x;
        public FP64 y;
        public FP64 z;
        public FP64 w;

        public FPQuaternion(FP64 x, FP64 y, FP64 z, FP64 w) {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        public static void Add(ref FPQuaternion a, ref FPQuaternion b, out FPQuaternion result) {
            result.x = a.x + b.x;
            result.y = a.y + b.y;
            result.z = a.z + b.z;
            result.w = a.w + b.w;
        }

        public static void Multiply(ref FPQuaternion a, ref FPQuaternion b, out FPQuaternion result) {
            FP64 x = a.x;
            FP64 y = a.y;
            FP64 z = a.z;
            FP64 w = a.w;
            FP64 bX = b.x;
            FP64 bY = b.y;
            FP64 bZ = b.z;
            FP64 bW = b.w;
            result.x = x * bW + bX * w + y * bZ - z * bY;
            result.y = y * bW + bY * w + z * bX - x * bZ;
            result.z = z * bW + bZ * w + x * bY - y * bX;
            result.w = w * bW - x * bX - y * bY - z * bZ;
        }

        public static FPQuaternion Multiply(in FPQuaternion a, in FPQuaternion b) {
            FPQuaternion result;
            FP64 x = a.x;
            FP64 y = a.y;
            FP64 z = a.z;
            FP64 w = a.w;
            FP64 bX = b.x;
            FP64 bY = b.y;
            FP64 bZ = b.z;
            FP64 bW = b.w;
            result.x = x * bW + bX * w + y * bZ - z * bY;
            result.y = y * bW + bY * w + z * bX - x * bZ;
            result.z = z * bW + bZ * w + x * bY - y * bX;
            result.w = w * bW - x * bX - y * bY - z * bZ;
            return result;
        }

        public static void Multiply(ref FPQuaternion q, FP64 scale, out FPQuaternion result) {
            result.x = q.x * scale;
            result.y = q.y * scale;
            result.z = q.z * scale;
            result.w = q.w * scale;
        }

        /// <summary>
        /// Multiplies two quaternions together in opposite order.
        /// </summary>
        /// <param name="a">First quaternion to multiply.</param>
        /// <param name="b">Second quaternion to multiply.</param>
        /// <param name="result">Product of the multiplication.</param>
        public static void Concatenate(ref FPQuaternion a, ref FPQuaternion b, out FPQuaternion result) {
            FP64 aX = a.x;
            FP64 aY = a.y;
            FP64 aZ = a.z;
            FP64 aW = a.w;
            FP64 bX = b.x;
            FP64 bY = b.y;
            FP64 bZ = b.z;
            FP64 bW = b.w;

            result.x = aW * bX + aX * bW + aZ * bY - aY * bZ;
            result.y = aW * bY + aY * bW + aX * bZ - aZ * bX;
            result.z = aW * bZ + aZ * bW + aY * bX - aX * bY;
            result.w = aW * bW - aX * bX - aY * bY - aZ * bZ;


        }

        /// <summary>
        /// Multiplies two quaternions together in opposite order.
        /// </summary>
        /// <param name="a">First quaternion to multiply.</param>
        /// <param name="b">Second quaternion to multiply.</param>
        /// <returns>Product of the multiplication.</returns>
        public static FPQuaternion Concatenate(FPQuaternion a, FPQuaternion b) {
            FPQuaternion result;
            Concatenate(ref a, ref b, out result);
            return result;
        }

        /// <summary>
        /// Quaternion representing the identity transform.
        /// </summary>
        public static FPQuaternion Identity {
            get {
                return new FPQuaternion(F64.C0, F64.C0, F64.C0, FP64.One);
            }
        }

        /// <summary>
        /// Constructs a quaternion from a rotation matrix.
        /// </summary>
        /// <param name="r">Rotation matrix to create the quaternion from.</param>
        /// <param name="q">Quaternion based on the rotation matrix.</param>
        public static void CreateFromRotationMatrix(ref FPMatrix3x3 r, out FPQuaternion q) {
            FP64 trace = r.M11 + r.M22 + r.M33;
            q = new FPQuaternion();
            if (trace >= F64.C0) {
                var S = FP64.Sqrt(trace + FP64.One) * F64.C2; // S=4*qw 
                var inverseS = FP64.One / S;
                q.w = FP64.C0p25 * S;
                q.x = (r.M23 - r.M32) * inverseS;
                q.y = (r.M31 - r.M13) * inverseS;
                q.z = (r.M12 - r.M21) * inverseS;
            } else if ((r.M11 > r.M22) & (r.M11 > r.M33)) {
                var S = FP64.Sqrt(FP64.One + r.M11 - r.M22 - r.M33) * F64.C2; // S=4*qx 
                var inverseS = FP64.One / S;
                q.w = (r.M23 - r.M32) * inverseS;
                q.x = FP64.C0p25 * S;
                q.y = (r.M21 + r.M12) * inverseS;
                q.z = (r.M31 + r.M13) * inverseS;
            } else if (r.M22 > r.M33) {
                var S = FP64.Sqrt(FP64.One + r.M22 - r.M11 - r.M33) * F64.C2; // S=4*qy
                var inverseS = FP64.One / S;
                q.w = (r.M31 - r.M13) * inverseS;
                q.x = (r.M21 + r.M12) * inverseS;
                q.y = FP64.C0p25 * S;
                q.z = (r.M32 + r.M23) * inverseS;
            } else {
                var S = FP64.Sqrt(FP64.One + r.M33 - r.M11 - r.M22) * F64.C2; // S=4*qz
                var inverseS = FP64.One / S;
                q.w = (r.M12 - r.M21) * inverseS;
                q.x = (r.M31 + r.M13) * inverseS;
                q.y = (r.M32 + r.M23) * inverseS;
                q.z = FP64.C0p25 * S;
            }
        }

        /// <summary>
        /// Creates a quaternion from a rotation matrix.
        /// </summary>
        /// <param name="r">Rotation matrix used to create a new quaternion.</param>
        /// <returns>Quaternion representing the same rotation as the matrix.</returns>
        public static FPQuaternion CreateFromRotationMatrix(FPMatrix3x3 r) {
            FPQuaternion toReturn;
            CreateFromRotationMatrix(ref r, out toReturn);
            return toReturn;
        }

        /// <summary>
        /// Constructs a quaternion from a rotation matrix.
        /// </summary>
        /// <param name="r">Rotation matrix to create the quaternion from.</param>
        /// <param name="q">Quaternion based on the rotation matrix.</param>
        public static void CreateFromRotationMatrix(ref FPMatrix4x4 r, out FPQuaternion q) {
            FPMatrix3x3 downsizedMatrix;
            FPMatrix3x3.CreateFromMatrix(ref r, out downsizedMatrix);
            CreateFromRotationMatrix(ref downsizedMatrix, out q);
        }

        /// <summary>
        /// Creates a quaternion from a rotation matrix.
        /// </summary>
        /// <param name="r">Rotation matrix used to create a new quaternion.</param>
        /// <returns>Quaternion representing the same rotation as the matrix.</returns>
        public static FPQuaternion CreateFromRotationMatrix(FPMatrix4x4 r) {
            FPQuaternion toReturn;
            CreateFromRotationMatrix(ref r, out toReturn);
            return toReturn;
        }


        /// <summary>
        /// Ensures the quaternion has unit length.
        /// </summary>
        /// <param name="quaternion">Quaternion to normalize.</param>
        /// <returns>Normalized quaternion.</returns>
        public static FPQuaternion Normalize(FPQuaternion quaternion) {
            FPQuaternion toReturn;
            Normalize(ref quaternion, out toReturn);
            return toReturn;
        }

        /// <summary>
        /// Ensures the quaternion has unit length.
        /// </summary>
        /// <param name="quaternion">Quaternion to normalize.</param>
        /// <param name="toReturn">Normalized quaternion.</param>
        public static void Normalize(ref FPQuaternion quaternion, out FPQuaternion toReturn) {
            FP64 inverse = FP64.One / FP64.Sqrt(quaternion.x * quaternion.x + quaternion.y * quaternion.y + quaternion.z * quaternion.z + quaternion.w * quaternion.w);
            toReturn.x = quaternion.x * inverse;
            toReturn.y = quaternion.y * inverse;
            toReturn.z = quaternion.z * inverse;
            toReturn.w = quaternion.w * inverse;
        }

        /// <summary>
        /// Scales the quaternion such that it has unit length.
        /// </summary>
        public void Normalize() {
            FP64 inverse = FP64.One / FP64.Sqrt(x * x + y * y + z * z + w * w);
            x *= inverse;
            y *= inverse;
            z *= inverse;
            w *= inverse;
        }

        /// <summary>
        /// Computes the squared length of the quaternion.
        /// </summary>
        /// <returns>Squared length of the quaternion.</returns>
        public FP64 LengthSquared() {
            return x * x + y * y + z * z + w * w;
        }

        /// <summary>
        /// Computes the length of the quaternion.
        /// </summary>
        /// <returns>Length of the quaternion.</returns>
        public FP64 Length() {
            return FP64.Sqrt(x * x + y * y + z * z + w * w);
        }


        /// <summary>
        /// Blends two quaternions together to get an intermediate state.
        /// </summary>
        /// <param name="start">Starting point of the interpolation.</param>
        /// <param name="end">Ending point of the interpolation.</param>
        /// <param name="interpolationAmount">Amount of the end point to use.</param>
        /// <param name="result">Interpolated intermediate quaternion.</param>
        public static void Slerp(ref FPQuaternion start, ref FPQuaternion end, FP64 interpolationAmount, out FPQuaternion result) {
            FP64 cosHalfTheta = start.w * end.w + start.x * end.x + start.y * end.y + start.z * end.z;
            if (cosHalfTheta < F64.C0) {
                //Negating a quaternion results in the same orientation, 
                //but we need cosHalfTheta to be positive to get the shortest path.
                end.x = -end.x;
                end.y = -end.y;
                end.z = -end.z;
                end.w = -end.w;
                cosHalfTheta = -cosHalfTheta;
            }
            // If the orientations are similar enough, then just pick one of the inputs.
            if (cosHalfTheta > FP64.C1m1em12) {
                result.w = start.w;
                result.x = start.x;
                result.y = start.y;
                result.z = start.z;
                return;
            }
            // Calculate temporary values.
            FP64 halfTheta = FP64.Acos(cosHalfTheta);
            FP64 sinHalfTheta = FP64.Sqrt(FP64.One - cosHalfTheta * cosHalfTheta);

            FP64 aFraction = FP64.Sin((FP64.One - interpolationAmount) * halfTheta) / sinHalfTheta;
            FP64 bFraction = FP64.Sin(interpolationAmount * halfTheta) / sinHalfTheta;

            //Blend the two quaternions to get the result!
            result.x = (FP64)(start.x * aFraction + end.x * bFraction);
            result.y = (FP64)(start.y * aFraction + end.y * bFraction);
            result.z = (FP64)(start.z * aFraction + end.z * bFraction);
            result.w = (FP64)(start.w * aFraction + end.w * bFraction);




        }

        /// <summary>
        /// Blends two quaternions together to get an intermediate state.
        /// </summary>
        /// <param name="start">Starting point of the interpolation.</param>
        /// <param name="end">Ending point of the interpolation.</param>
        /// <param name="interpolationAmount">Amount of the end point to use.</param>
        /// <returns>Interpolated intermediate quaternion.</returns>
        public static FPQuaternion Slerp(FPQuaternion start, FPQuaternion end, FP64 interpolationAmount) {
            FPQuaternion toReturn;
            Slerp(ref start, ref end, interpolationAmount, out toReturn);
            return toReturn;
        }


        /// <summary>
        /// Computes the conjugate of the quaternion.
        /// </summary>
        /// <param name="quaternion">Quaternion to conjugate.</param>
        /// <param name="result">Conjugated quaternion.</param>
        public static void Conjugate(ref FPQuaternion quaternion, out FPQuaternion result) {
            result.x = -quaternion.x;
            result.y = -quaternion.y;
            result.z = -quaternion.z;
            result.w = quaternion.w;
        }

        /// <summary>
        /// Computes the conjugate of the quaternion.
        /// </summary>
        /// <param name="quaternion">Quaternion to conjugate.</param>
        /// <returns>Conjugated quaternion.</returns>
        public static FPQuaternion Conjugate(FPQuaternion quaternion) {
            FPQuaternion toReturn;
            Conjugate(ref quaternion, out toReturn);
            return toReturn;
        }



        /// <summary>
        /// Computes the inverse of the quaternion.
        /// </summary>
        /// <param name="quaternion">Quaternion to invert.</param>
        /// <param name="result">Result of the inversion.</param>
        public static void Inverse(ref FPQuaternion quaternion, out FPQuaternion result) {
            FP64 inverseSquaredNorm = quaternion.x * quaternion.x + quaternion.y * quaternion.y + quaternion.z * quaternion.z + quaternion.w * quaternion.w;
            result.x = -quaternion.x * inverseSquaredNorm;
            result.y = -quaternion.y * inverseSquaredNorm;
            result.z = -quaternion.z * inverseSquaredNorm;
            result.w = quaternion.w * inverseSquaredNorm;
        }

        /// <summary>
        /// Computes the inverse of the quaternion.
        /// </summary>
        /// <param name="quaternion">Quaternion to invert.</param>
        /// <returns>Result of the inversion.</returns>
        public static FPQuaternion Inverse(in FPQuaternion quaternion) {
            FPQuaternion result;
            FP64 inverseSquaredNorm = quaternion.x * quaternion.x + quaternion.y * quaternion.y + quaternion.z * quaternion.z + quaternion.w * quaternion.w;
            result.x = -quaternion.x * inverseSquaredNorm;
            result.y = -quaternion.y * inverseSquaredNorm;
            result.z = -quaternion.z * inverseSquaredNorm;
            result.w = quaternion.w * inverseSquaredNorm;
            return result;

        }

        /// <summary>
        /// Tests components for equality.
        /// </summary>
        /// <param name="a">First quaternion to test for equivalence.</param>
        /// <param name="b">Second quaternion to test for equivalence.</param>
        /// <returns>Whether or not the quaternions' components were equal.</returns>
        public static bool operator ==(FPQuaternion a, FPQuaternion b) {
            return a.x == b.x && a.y == b.y && a.z == b.z && a.w == b.w;
        }

        /// <summary>
        /// Tests components for inequality.
        /// </summary>
        /// <param name="a">First quaternion to test for equivalence.</param>
        /// <param name="b">Second quaternion to test for equivalence.</param>
        /// <returns>Whether the quaternions' components were not equal.</returns>
        public static bool operator !=(FPQuaternion a, FPQuaternion b) {
            return a.x != b.x || a.y != b.y || a.z != b.z || a.w != b.w;
        }

        /// <summary>
        /// Negates the components of a quaternion.
        /// </summary>
        /// <param name="a">Quaternion to negate.</param>
        /// <param name="b">Negated result.</param>
        public static void Negate(ref FPQuaternion a, out FPQuaternion b) {
            b.x = -a.x;
            b.y = -a.y;
            b.z = -a.z;
            b.w = -a.w;
        }

        /// <summary>
        /// Negates the components of a quaternion.
        /// </summary>
        /// <param name="q">Quaternion to negate.</param>
        /// <returns>Negated result.</returns>
        public static FPQuaternion Negate(FPQuaternion q) {
            Negate(ref q, out var result);
            return result;
        }

        /// <summary>
        /// Negates the components of a quaternion.
        /// </summary>
        /// <param name="q">Quaternion to negate.</param>
        /// <returns>Negated result.</returns>
        public static FPQuaternion operator -(in FPQuaternion q) {
            FPQuaternion res;
            res.x = -q.x;
            res.y = -q.y;
            res.z = -q.z;
            res.w = -q.w;
            return res;
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(FPQuaternion other) {
            return x == other.x && y == other.y && z == other.z && w == other.w;
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <returns>
        /// true if <paramref name="obj"/> and this instance are the same type and represent the same value; otherwise, false.
        /// </returns>
        /// <param name="obj">Another object to compare to. </param><filterpriority>2</filterpriority>
        public override bool Equals(object obj) {
            if (obj is FPQuaternion) {
                return Equals((FPQuaternion)obj);
            }
            return false;
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>
        /// A 32-bit signed integer that is the hash code for this instance.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode() {
            return x.GetHashCode() + y.GetHashCode() + z.GetHashCode() + w.GetHashCode();
        }

        /// <summary>
        /// Transforms the vector using a quaternion.
        /// </summary>
        /// <param name="v">Vector to transform.</param>
        /// <param name="rotation">Rotation to apply to the vector.</param>
        /// <param name="result">Transformed vector.</param>
        public static void Transform(ref FPVector3 v, ref FPQuaternion rotation, out FPVector3 result) {
            //This operation is an optimized-down version of v' = q * v * q^-1.
            //The expanded form would be to treat v as an 'axis only' quaternion
            //and perform standard quaternion multiplication.  Assuming q is normalized,
            //q^-1 can be replaced by a conjugation.
            FP64 x2 = rotation.x + rotation.x;
            FP64 y2 = rotation.y + rotation.y;
            FP64 z2 = rotation.z + rotation.z;
            FP64 xx2 = rotation.x * x2;
            FP64 xy2 = rotation.x * y2;
            FP64 xz2 = rotation.x * z2;
            FP64 yy2 = rotation.y * y2;
            FP64 yz2 = rotation.y * z2;
            FP64 zz2 = rotation.z * z2;
            FP64 wx2 = rotation.w * x2;
            FP64 wy2 = rotation.w * y2;
            FP64 wz2 = rotation.w * z2;
            //Defer the component setting since they're used in computation.
            FP64 transformedX = v.x * (FP64.One - yy2 - zz2) + v.y * (xy2 - wz2) + v.z * (xz2 + wy2);
            FP64 transformedY = v.x * (xy2 + wz2) + v.y * (FP64.One - xx2 - zz2) + v.z * (yz2 - wx2);
            FP64 transformedZ = v.x * (xz2 - wy2) + v.y * (yz2 + wx2) + v.z * (FP64.One - xx2 - yy2);
            result.x = transformedX;
            result.y = transformedY;
            result.z = transformedZ;

        }

        /// <summary>
        /// Transforms the vector using a quaternion.
        /// </summary>
        /// <param name="v">Vector to transform.</param>
        /// <param name="rotation">Rotation to apply to the vector.</param>
        /// <returns>Transformed vector.</returns>
        public static FPVector3 Transform(FPVector3 v, FPQuaternion rotation) {
            FPVector3 toReturn;
            Transform(ref v, ref rotation, out toReturn);
            return toReturn;
        }

        /// <summary>
        /// Transforms a vector using a quaternion. Specialized for x,0,0 vectors.
        /// </summary>
        /// <param name="x">X component of the vector to transform.</param>
        /// <param name="rotation">Rotation to apply to the vector.</param>
        /// <param name="result">Transformed vector.</param>
        public static void TransformX(FP64 x, ref FPQuaternion rotation, out FPVector3 result) {
            //This operation is an optimized-down version of v' = q * v * q^-1.
            //The expanded form would be to treat v as an 'axis only' quaternion
            //and perform standard quaternion multiplication.  Assuming q is normalized,
            //q^-1 can be replaced by a conjugation.
            FP64 y2 = rotation.y + rotation.y;
            FP64 z2 = rotation.z + rotation.z;
            FP64 xy2 = rotation.x * y2;
            FP64 xz2 = rotation.x * z2;
            FP64 yy2 = rotation.y * y2;
            FP64 zz2 = rotation.z * z2;
            FP64 wy2 = rotation.w * y2;
            FP64 wz2 = rotation.w * z2;
            //Defer the component setting since they're used in computation.
            FP64 transformedX = x * (FP64.One - yy2 - zz2);
            FP64 transformedY = x * (xy2 + wz2);
            FP64 transformedZ = x * (xz2 - wy2);
            result.x = transformedX;
            result.y = transformedY;
            result.z = transformedZ;

        }

        /// <summary>
        /// Transforms a vector using a quaternion. Specialized for 0,y,0 vectors.
        /// </summary>
        /// <param name="y">Y component of the vector to transform.</param>
        /// <param name="rotation">Rotation to apply to the vector.</param>
        /// <param name="result">Transformed vector.</param>
        public static void TransformY(FP64 y, ref FPQuaternion rotation, out FPVector3 result) {
            //This operation is an optimized-down version of v' = q * v * q^-1.
            //The expanded form would be to treat v as an 'axis only' quaternion
            //and perform standard quaternion multiplication.  Assuming q is normalized,
            //q^-1 can be replaced by a conjugation.
            FP64 x2 = rotation.x + rotation.x;
            FP64 y2 = rotation.y + rotation.y;
            FP64 z2 = rotation.z + rotation.z;
            FP64 xx2 = rotation.x * x2;
            FP64 xy2 = rotation.x * y2;
            FP64 yz2 = rotation.y * z2;
            FP64 zz2 = rotation.z * z2;
            FP64 wx2 = rotation.w * x2;
            FP64 wz2 = rotation.w * z2;
            //Defer the component setting since they're used in computation.
            FP64 transformedX = y * (xy2 - wz2);
            FP64 transformedY = y * (FP64.One - xx2 - zz2);
            FP64 transformedZ = y * (yz2 + wx2);
            result.x = transformedX;
            result.y = transformedY;
            result.z = transformedZ;

        }

        /// <summary>
        /// Transforms a vector using a quaternion. Specialized for 0,0,z vectors.
        /// </summary>
        /// <param name="z">Z component of the vector to transform.</param>
        /// <param name="rotation">Rotation to apply to the vector.</param>
        /// <param name="result">Transformed vector.</param>
        public static void TransformZ(FP64 z, ref FPQuaternion rotation, out FPVector3 result) {
            //This operation is an optimized-down version of v' = q * v * q^-1.
            //The expanded form would be to treat v as an 'axis only' quaternion
            //and perform standard quaternion multiplication.  Assuming q is normalized,
            //q^-1 can be replaced by a conjugation.
            FP64 x2 = rotation.x + rotation.x;
            FP64 y2 = rotation.y + rotation.y;
            FP64 z2 = rotation.z + rotation.z;
            FP64 xx2 = rotation.x * x2;
            FP64 xz2 = rotation.x * z2;
            FP64 yy2 = rotation.y * y2;
            FP64 yz2 = rotation.y * z2;
            FP64 wx2 = rotation.w * x2;
            FP64 wy2 = rotation.w * y2;
            //Defer the component setting since they're used in computation.
            FP64 transformedX = z * (xz2 + wy2);
            FP64 transformedY = z * (yz2 - wx2);
            FP64 transformedZ = z * (FP64.One - xx2 - yy2);
            result.x = transformedX;
            result.y = transformedY;
            result.z = transformedZ;

        }


        /// <summary>
        /// Multiplies two quaternions.
        /// </summary>
        /// <param name="a">First quaternion to multiply.</param>
        /// <param name="b">Second quaternion to multiply.</param>
        /// <returns>Product of the multiplication.</returns>
        public static FPQuaternion operator *(in FPQuaternion a, in FPQuaternion b) {
            FPQuaternion result;
            FP64 x = a.x;
            FP64 y = a.y;
            FP64 z = a.z;
            FP64 w = a.w;
            FP64 bX = b.x;
            FP64 bY = b.y;
            FP64 bZ = b.z;
            FP64 bW = b.w;
            result.x = x * bW + bX * w + y * bZ - z * bY;
            result.y = y * bW + bY * w + z * bX - x * bZ;
            result.z = z * bW + bZ * w + x * bY - y * bX;
            result.w = w * bW - x * bX - y * bY - z * bZ;
            return result;
        }

        public static FPVector3 operator *(in FPQuaternion quat, in FPVector3 vec) {

            FP64 num = quat.x * 2;
            FP64 num2 = quat.y * 2;
            FP64 num3 = quat.z * 2;
            FP64 num4 = quat.x * num;
            FP64 num5 = quat.y * num2;
            FP64 num6 = quat.z * num3;
            FP64 num7 = quat.x * num2;
            FP64 num8 = quat.x * num3;
            FP64 num9 = quat.y * num3;
            FP64 num10 = quat.w * num;
            FP64 num11 = quat.w * num2;
            FP64 num12 = quat.w * num3;

            FPVector3 result;
            result.x = (1 - (num5 + num6)) * vec.x + (num7 - num12) * vec.y + (num8 + num11) * vec.z;
            result.y = (num7 + num12) * vec.x + (1 - (num4 + num6)) * vec.y + (num9 - num10) * vec.z;
            result.z = (num8 - num11) * vec.x + (num9 + num10) * vec.y + (1 - (num4 + num5)) * vec.z;

            return result;
        }

        /// <summary>
        /// Creates a quaternion from an axis and angle.
        /// </summary>
        /// <param name="axis">Axis of rotation.</param>
        /// <param name="angle">Angle to rotate around the axis.</param>
        /// <returns>Quaternion representing the axis and angle rotation.</returns>
        public static FPQuaternion CreateFromAxisAngle(FPVector3 axis, FP64 angle) {
            FP64 halfAngle = angle * FP64.Half;
            FP64 s = FP64.Sin(halfAngle);
            FPQuaternion q;
            q.x = axis.x * s;
            q.y = axis.y * s;
            q.z = axis.z * s;
            q.w = FP64.Cos(halfAngle);
            return q;
        }

        /// <summary>
        /// Creates a quaternion from an axis and angle.
        /// </summary>
        /// <param name="axis">Axis of rotation.</param>
        /// <param name="angle">Angle to rotate around the axis.</param>
        /// <param name="q">Quaternion representing the axis and angle rotation.</param>
        public static void CreateFromAxisAngle(ref FPVector3 axis, FP64 angle, out FPQuaternion q) {
            FP64 halfAngle = angle * F64.C0p5;
            FP64 s = FP64.Sin(halfAngle);
            q.x = axis.x * s;
            q.y = axis.y * s;
            q.z = axis.z * s;
            q.w = FP64.Cos(halfAngle);
        }

        /// <summary>
        /// Constructs a quaternion from yaw, pitch, and roll.
        /// </summary>
        /// <param name="yaw">Yaw of the rotation.</param>
        /// <param name="pitch">Pitch of the rotation.</param>
        /// <param name="roll">Roll of the rotation.</param>
        /// <returns>Quaternion representing the yaw, pitch, and roll.</returns>
        public static FPQuaternion CreateFromYawPitchRoll(in FP64 yaw, in FP64 pitch, in FP64 roll) {
            FPQuaternion toReturn;
            CreateFromYawPitchRoll(yaw, pitch, roll, out toReturn);
            return toReturn;
        }

        /// <summary>
        /// Constructs a quaternion from yaw, pitch, and roll.
        /// </summary>
        /// <param name="yaw">Yaw of the rotation.</param>
        /// <param name="pitch">Pitch of the rotation.</param>
        /// <param name="roll">Roll of the rotation.</param>
        /// <param name="q">Quaternion representing the yaw, pitch, and roll.</param>
        public static void CreateFromYawPitchRoll(in FP64 yaw, in FP64 pitch, in FP64 roll, out FPQuaternion q) {
            FP64 halfRoll = roll * F64.C0p5;
            FP64 halfPitch = pitch * F64.C0p5;
            FP64 halfYaw = yaw * F64.C0p5;

            FP64 sinRoll = FP64.Sin(halfRoll);
            FP64 sinPitch = FP64.Sin(halfPitch);
            FP64 sinYaw = FP64.Sin(halfYaw);

            FP64 cosRoll = FP64.Cos(halfRoll);
            FP64 cosPitch = FP64.Cos(halfPitch);
            FP64 cosYaw = FP64.Cos(halfYaw);

            FP64 cosYawCosPitch = cosYaw * cosPitch;
            FP64 cosYawSinPitch = cosYaw * sinPitch;
            FP64 sinYawCosPitch = sinYaw * cosPitch;
            FP64 sinYawSinPitch = sinYaw * sinPitch;

            q.x = cosYawSinPitch * cosRoll + sinYawCosPitch * sinRoll;
            q.y = sinYawCosPitch * cosRoll - cosYawSinPitch * sinRoll;
            q.z = cosYawCosPitch * sinRoll - sinYawSinPitch * cosRoll;
            q.w = cosYawCosPitch * cosRoll + sinYawSinPitch * sinRoll;

        }

        /// <summary>
        /// Computes the angle change represented by a normalized quaternion.
        /// </summary>
        /// <param name="q">Quaternion to be converted.</param>
        /// <returns>Angle around the axis represented by the quaternion.</returns>
        public static FP64 GetAngleFromQuaternion(ref FPQuaternion q) {
            FP64 qw = FP64.Abs(q.w);
            if (qw > FP64.One)
                return F64.C0;
            return F64.C2 * FP64.Acos(qw);
        }

        /// <summary>
        /// Computes the axis angle representation of a normalized quaternion.
        /// </summary>
        /// <param name="q">Quaternion to be converted.</param>
        /// <param name="axis">Axis represented by the quaternion.</param>
        /// <param name="angle">Angle around the axis represented by the quaternion.</param>
        public static void GetAxisAngleFromQuaternion(ref FPQuaternion q, out FPVector3 axis, out FP64 angle) {
#if !WINDOWS
            axis = new FPVector3();
#endif
            FP64 qw = q.w;
            if (qw > F64.C0) {
                axis.x = q.x;
                axis.y = q.y;
                axis.z = q.z;
            } else {
                axis.x = -q.x;
                axis.y = -q.y;
                axis.z = -q.z;
                qw = -qw;
            }

            FP64 lengthSquared = axis.LengthSquared();
            if (lengthSquared > F64.C1em14) {
                FPVector3.Divide(ref axis, FP64.Sqrt(lengthSquared), out axis);
                angle = F64.C2 * FP64.Acos(MathHelper.Clamp(qw, -1, FP64.One));
            } else {
                axis = FPVector3.Up;
                angle = F64.C0;
            }
        }

        public static FPQuaternion LookAtHorizontal(FPVector3 dirNormalized) {
            FP64 angle = FPVector3.SignedAngle(FPVector3.Forward, dirNormalized, FPVector3.Up);
            var rot = FPQuaternion.CreateFromAxisAngle(FPVector3.Up, angle * FP64.Deg2Rad);
            return rot;
        }

        /// <summary>
        /// Computes the quaternion rotation between two normalized vectors.
        /// </summary>
        /// <param name="v1">First unit-length vector.</param>
        /// <param name="v2">Second unit-length vector.</param>
        /// <param name="q">Quaternion representing the rotation from v1 to v2.</param>
        public static void GetQuaternionBetweenNormalizedVectors(ref FPVector3 v1, ref FPVector3 v2, out FPQuaternion q) {
            FP64 dot;
            FPVector3.Dot(ref v1, ref v2, out dot);
            //For non-normal vectors, the multiplying the axes length squared would be necessary:
            //FP64 w = dot + (FP64)Math.Sqrt(v1.LengthSquared() * v2.LengthSquared());
            if (dot < F64.Cm0p9999) //parallel, opposing direction
            {
                //If this occurs, the rotation required is ~180 degrees.
                //The problem is that we could choose any perpendicular axis for the rotation. It's not uniquely defined.
                //The solution is to pick an arbitrary perpendicular axis.
                //Project onto the plane which has the lowest component magnitude.
                //On that 2d plane, perform a 90 degree rotation.
                FP64 absX = FP64.Abs(v1.x);
                FP64 absY = FP64.Abs(v1.y);
                FP64 absZ = FP64.Abs(v1.z);
                if (absX < absY && absX < absZ)
                    q = new FPQuaternion(F64.C0, -v1.z, v1.y, F64.C0);
                else if (absY < absZ)
                    q = new FPQuaternion(-v1.z, F64.C0, v1.x, F64.C0);
                else
                    q = new FPQuaternion(-v1.y, v1.x, F64.C0, F64.C0);
            } else {
                FPVector3 axis;
                FPVector3.Cross(ref v1, ref v2, out axis);
                q = new FPQuaternion(axis.x, axis.y, axis.z, dot + FP64.One);
            }
            q.Normalize();
        }

        //The following two functions are highly similar, but it's a bit of a brain teaser to phrase one in terms of the other.
        //Providing both simplifies things.

        /// <summary>
        /// Computes the rotation from the start orientation to the end orientation such that end = Quaternion.Concatenate(start, relative).
        /// </summary>
        /// <param name="start">Starting orientation.</param>
        /// <param name="end">Ending orientation.</param>
        /// <param name="relative">Relative rotation from the start to the end orientation.</param>
        public static void GetRelativeRotation(ref FPQuaternion start, ref FPQuaternion end, out FPQuaternion relative) {
            FPQuaternion startInverse;
            Conjugate(ref start, out startInverse);
            Concatenate(ref startInverse, ref end, out relative);
        }


        /// <summary>
        /// Transforms the rotation into the local space of the target basis such that rotation = Quaternion.Concatenate(localRotation, targetBasis)
        /// </summary>
        /// <param name="rotation">Rotation in the original frame of reference.</param>
        /// <param name="targetBasis">Basis in the original frame of reference to transform the rotation into.</param>
        /// <param name="localRotation">Rotation in the local space of the target basis.</param>
        public static void GetLocalRotation(ref FPQuaternion rotation, ref FPQuaternion targetBasis, out FPQuaternion localRotation) {
            FPQuaternion basisInverse;
            Conjugate(ref targetBasis, out basisInverse);
            Concatenate(ref rotation, ref basisInverse, out localRotation);
        }

        /// <summary>
        /// Gets a string representation of the quaternion.
        /// </summary>
        /// <returns>String representing the quaternion.</returns>
        public override string ToString() {
            return "{ X: " + x + ", Y: " + y + ", Z: " + z + ", W: " + w + "}";
        }
    }
}
