using System;

namespace FixMath.NET {
    /// <summary>
    /// Provides XNA-like 2D vector math.
    /// </summary>
    [Serializable]
    public struct FPVector2 : IEquatable<FPVector2> {
        /// <summary>
        /// X component of the vector.
        /// </summary>
        public FP64 x;
        /// <summary>
        /// Y component of the vector.
        /// </summary>
        public FP64 y;

        /// <summary>
        /// Constructs a new two dimensional vector.
        /// </summary>
        /// <param name="x">X component of the vector.</param>
        /// <param name="y">Y component of the vector.</param>
        public FPVector2(FP64 x, FP64 y) {
            this.x = x;
            this.y = y;
        }

        /// <summary>
        /// Computes the squared length of the vector.
        /// </summary>
        /// <returns>Squared length of the vector.</returns>
        public FP64 LengthSquared() {
            return x * x + y * y;
        }

        /// <summary>
        /// Computes the length of the vector.
        /// </summary>
        /// <returns>Length of the vector.</returns>
        public FP64 Length() {
            return FP64.Sqrt(x * x + y * y);
        }

        /// <summary>
        /// Gets a string representation of the vector.
        /// </summary>
        /// <returns>String representing the vector.</returns>
        public override string ToString() {
            return "{" + x + ", " + y + "}";
        }

        public void Set(FP64 x, FP64 y) {
            this.x = x;
            this.y = y;
        }

        public void Set(in FPVector2 tar) {
            this.x = tar.x;
            this.y = tar.y;
        }

        /// <summary>
        /// Adds two vectors together.
        /// </summary>
        /// <param name="a">First vector to add.</param>
        /// <param name="b">Second vector to add.</param>
        /// <param name="sum">Sum of the two vectors.</param>
        public static void Add(ref FPVector2 a, ref FPVector2 b, out FPVector2 sum) {
            sum.x = a.x + b.x;
            sum.y = a.y + b.y;
        }

        /// <summary>
        /// Subtracts two vectors.
        /// </summary>
        /// <param name="a">Vector to subtract from.</param>
        /// <param name="b">Vector to subtract from the first vector.</param>
        /// <param name="difference">Result of the subtraction.</param>
        public static void Subtract(ref FPVector2 a, ref FPVector2 b, out FPVector2 difference) {
            difference.x = a.x - b.x;
            difference.y = a.y - b.y;
        }

        /// <summary>
        /// Scales a vector.
        /// </summary>
        /// <param name="v">Vector to scale.</param>
        /// <param name="scale">Amount to scale.</param>
        /// <param name="result">Scaled vector.</param>
        public static void Multiply(ref FPVector2 v, FP64 scale, out FPVector2 result) {
            result.x = v.x * scale;
            result.y = v.y * scale;
        }

        public static FPVector2 Multiply(in FPVector2 v, FP64 scale) {
            return new FPVector2(v.x * scale, v.y * scale);
        }

        /// <summary>
        /// Multiplies two vectors on a per-component basis.
        /// </summary>
        /// <param name="a">First vector to multiply.</param>
        /// <param name="b">Second vector to multiply.</param>
        /// <param name="result">Result of the componentwise multiplication.</param>
        public static void Multiply(ref FPVector2 a, ref FPVector2 b, out FPVector2 result) {
            result.x = a.x * b.x;
            result.y = a.y * b.y;
        }

        /// <summary>
        /// Divides a vector's components by some amount.
        /// </summary>
        /// <param name="v">Vector to divide.</param>
        /// <param name="divisor">Value to divide the vector's components.</param>
        /// <param name="result">Result of the division.</param>
        public static void Divide(ref FPVector2 v, FP64 divisor, out FPVector2 result) {
            FP64 inverse = F64.C1 / divisor;
            result.x = v.x * inverse;
            result.y = v.y * inverse;
        }

        /// <summary>
        /// Computes the dot product of the two vectors.
        /// </summary>
        /// <param name="a">First vector of the dot product.</param>
        /// <param name="b">Second vector of the dot product.</param>
        /// <param name="dot">Dot product of the two vectors.</param>
        public static void Dot(ref FPVector2 a, ref FPVector2 b, out FP64 dot) {
            dot = a.x * b.x + a.y * b.y;
        }

        /// <summary>
        /// Computes the dot product of the two vectors.
        /// </summary>
        /// <param name="a">First vector of the dot product.</param>
        /// <param name="b">Second vector of the dot product.</param>
        /// <returns>Dot product of the two vectors.</returns>
        public static FP64 Dot(FPVector2 a, FPVector2 b) {
            return a.x * b.x + a.y * b.y;
        }

        public static FPVector2 Project(in FPVector2 vector, in FPVector2 onNormal) {
            FP64 num = Dot(onNormal, onNormal);
            bool flag = num < FP64.Epsilon;
            FPVector2 result;
            if (flag) {
                result = FPVector2.Zero;
            } else {
                FP64 num2 = Dot(vector, onNormal);
                result = new FPVector2(onNormal.x * num2 / num, onNormal.y * num2 / num);
            }
            return result;
        }

        public static FPVector2 GetRotated(FPVector2 src, FP64 addDegree) {
            if (addDegree == 0) return src;
            var rad = addDegree * FP64.Deg2Rad;
            var sinVal = FP64.Sin(rad);
            var cosVal = FP64.Cos(rad);
            var x0 = src.x * cosVal - src.y * sinVal;
            var y0 = src.x * sinVal + src.y * cosVal;
            return new FPVector2((FP64)x0, (FP64)y0);
        }

        public static bool IsInVectorRightSide(FPVector2 src, FPVector2 tar) {
            // 前提: 同一个直角坐标系
            //   定义：平面上的三点P1(x1,y1),P2(x2,y2),P3(x3,y3)的面积量：
            // S(P1,P2,P3)=|y1 y2 y3|= (x1-x3)*(y2-y3)-(y1-y3)*(x2-x3)
            // 当P1P2P3逆时针时S为正的，当P1P2P3顺时针时S为负的。
            // 令向量的起点为A，终点为B，判断的点为C
            float x1 = 0;
            float y1 = 0;
            var srcX = src.x;
            var srcY = src.y;
            var tarX = tar.x;
            var tarY = tar.y;
            float value = (x1 - tarX) * (srcY - tarY) - (y1 - tarY) * (srcX - tarX);
            return value < 0;
        }

        /// <summary>
        /// Gets the zero vector.
        /// </summary>
        public static FPVector2 Zero {
            get {
                return new FPVector2();
            }
        }

        /// <summary>
        /// Gets a vector pointing along the X axis.
        /// </summary>
        public static FPVector2 UnitX {
            get { return new FPVector2 { x = F64.C1 }; }
        }

        /// <summary>
        /// Gets a vector pointing along the Y axis.
        /// </summary>
        public static FPVector2 UnitY {
            get { return new FPVector2 { y = F64.C1 }; }
        }


        /// <summary>
        /// Normalizes the vector.
        /// </summary>
        /// <param name="v">Vector to normalize.</param>
        /// <returns>Normalized copy of the vector.</returns>
        public static FPVector2 Normalize(FPVector2 v) {
            FPVector2 toReturn;
            FPVector2.Normalize(ref v, out toReturn);
            return toReturn;
        }

        /// <summary>
        /// Normalizes the vector.
        /// </summary>
        /// <param name="v">Vector to normalize.</param>
        /// <param name="result">Normalized vector.</param>
        public static void Normalize(ref FPVector2 v, out FPVector2 result) {
            FP64 inverse = F64.C1 / FP64.Sqrt(v.x * v.x + v.y * v.y);
            result.x = v.x * inverse;
            result.y = v.y * inverse;
        }

        /// <summary>
        /// Negates the vector.
        /// </summary>
        /// <param name="v">Vector to negate.</param>
        /// <param name="negated">Negated version of the vector.</param>
        public static void Negate(ref FPVector2 v, out FPVector2 negated) {
            negated.x = -v.x;
            negated.y = -v.y;
        }

        /// <summary>
        /// Computes the absolute value of the input vector.
        /// </summary>
        /// <param name="v">Vector to take the absolute value of.</param>
        /// <param name="result">Vector with nonnegative elements.</param>
        public static void Abs(ref FPVector2 v, out FPVector2 result) {
            if (v.x < F64.C0)
                result.x = -v.x;
            else
                result.x = v.x;
            if (v.y < F64.C0)
                result.y = -v.y;
            else
                result.y = v.y;
        }

        /// <summary>
        /// Computes the absolute value of the input vector.
        /// </summary>
        /// <param name="v">Vector to take the absolute value of.</param>
        /// <returns>Vector with nonnegative elements.</returns>
        public static FPVector2 Abs(FPVector2 v) {
            FPVector2 result;
            Abs(ref v, out result);
            return result;
        }

        /// <summary>
        /// Creates a vector from the lesser values in each vector.
        /// </summary>
        /// <param name="a">First input vector to compare values from.</param>
        /// <param name="b">Second input vector to compare values from.</param>
        /// <param name="min">Vector containing the lesser values of each vector.</param>
        public static void Min(ref FPVector2 a, ref FPVector2 b, out FPVector2 min) {
            min.x = a.x < b.x ? a.x : b.x;
            min.y = a.y < b.y ? a.y : b.y;
        }

        /// <summary>
        /// Creates a vector from the lesser values in each vector.
        /// </summary>
        /// <param name="a">First input vector to compare values from.</param>
        /// <param name="b">Second input vector to compare values from.</param>
        /// <returns>Vector containing the lesser values of each vector.</returns>
        public static FPVector2 Min(in FPVector2 a, in FPVector2 b) {
            FPVector2 min;
            min.x = a.x < b.x ? a.x : b.x;
            min.y = a.y < b.y ? a.y : b.y;
            return min;
        }


        /// <summary>
        /// Creates a vector from the greater values in each vector.
        /// </summary>
        /// <param name="a">First input vector to compare values from.</param>
        /// <param name="b">Second input vector to compare values from.</param>
        /// <param name="max">Vector containing the greater values of each vector.</param>
        public static void Max(ref FPVector2 a, ref FPVector2 b, out FPVector2 max) {
            max.x = a.x > b.x ? a.x : b.x;
            max.y = a.y > b.y ? a.y : b.y;
        }

        /// <summary>
        /// Creates a vector from the greater values in each vector.
        /// </summary>
        /// <param name="a">First input vector to compare values from.</param>
        /// <param name="b">Second input vector to compare values from.</param>
        /// <returns>Vector containing the greater values of each vector.</returns>
        public static FPVector2 Max(in FPVector2 a, in FPVector2 b) {
            FPVector2 max;
            max.x = a.x > b.x ? a.x : b.x;
            max.y = a.y > b.y ? a.y : b.y;
            return max;
        }

        public long[] ToRaw() {
            return new long[2] { x.RawValue, y.RawValue };
        }

        public static FPVector2 FromRaw(long[] raw) {
            if (raw == null || raw.Length != 2) {
                throw new Exception("Wrong Length");
            }
            return new FPVector2(FP64.FromRaw(raw[0]), FP64.FromRaw(raw[1]));
        }

        /// <summary>
        /// Normalizes the vector.
        /// </summary>
        public void Normalize() {
            FP64 inverse = F64.C1 / FP64.Sqrt(x * x + y * y);
            x *= inverse;
            y *= inverse;
        }

        /// <summary>
        /// Scales a vector.
        /// </summary>
        /// <param name="v">Vector to scale.</param>
        /// <param name="f">Amount to scale.</param>
        /// <returns>Scaled vector.</returns>
        public static FPVector2 operator *(FPVector2 v, FP64 f) {
            FPVector2 toReturn;
            toReturn.x = v.x * f;
            toReturn.y = v.y * f;
            return toReturn;
        }
        /// <summary>
        /// Scales a vector.
        /// </summary>
        /// <param name="v">Vector to scale.</param>
        /// <param name="f">Amount to scale.</param>
        /// <returns>Scaled vector.</returns>
        public static FPVector2 operator *(FP64 f, FPVector2 v) {
            FPVector2 toReturn;
            toReturn.x = v.x * f;
            toReturn.y = v.y * f;
            return toReturn;
        }

        /// <summary>
        /// Multiplies two vectors on a per-component basis.
        /// </summary>
        /// <param name="a">First vector to multiply.</param>
        /// <param name="b">Second vector to multiply.</param>
        /// <returns>Result of the componentwise multiplication.</returns>
        public static FPVector2 operator *(FPVector2 a, FPVector2 b) {
            FPVector2 result;
            Multiply(ref a, ref b, out result);
            return result;
        }

        /// <summary>
        /// Divides a vector.
        /// </summary>
        /// <param name="v">Vector to divide.</param>
        /// <param name="f">Amount to divide.</param>
        /// <returns>Divided vector.</returns>
        public static FPVector2 operator /(FPVector2 v, FP64 f) {
            FPVector2 toReturn;
            f = F64.C1 / f;
            toReturn.x = v.x * f;
            toReturn.y = v.y * f;
            return toReturn;
        }

        /// <summary>
        /// Subtracts two vectors.
        /// </summary>
        /// <param name="a">Vector to be subtracted from.</param>
        /// <param name="b">Vector to subtract from the first vector.</param>
        /// <returns>Resulting difference.</returns>
        public static FPVector2 operator -(in FPVector2 a, in FPVector2 b) {
            FPVector2 v;
            v.x = a.x - b.x;
            v.y = a.y - b.y;
            return v;
        }

        public static FPVector2 operator *(in FPVector2 a, in FP64 b) {
            FPVector2 v;
            v.x = a.x * b;
            v.y = a.y * b;
            return v;
        }

        /// <summary>
        /// Adds two vectors.
        /// </summary>
        /// <param name="a">First vector to add.</param>
        /// <param name="b">Second vector to add.</param>
        /// <returns>Sum of the addition.</returns>
        public static FPVector2 operator +(FPVector2 a, FPVector2 b) {
            FPVector2 v;
            v.x = a.x + b.x;
            v.y = a.y + b.y;
            return v;
        }

        /// <summary>
        /// Negates the vector.
        /// </summary>
        /// <param name="v">Vector to negate.</param>
        /// <returns>Negated vector.</returns>
        public static FPVector2 operator -(FPVector2 v) {
            v.x = -v.x;
            v.y = -v.y;
            return v;
        }

        /// <summary>
        /// Tests two vectors for componentwise equivalence.
        /// </summary>
        /// <param name="a">First vector to test for equivalence.</param>
        /// <param name="b">Second vector to test for equivalence.</param>
        /// <returns>Whether the vectors were equivalent.</returns>
        public static bool operator ==(FPVector2 a, FPVector2 b) {
            return a.x == b.x && a.y == b.y;
        }
        /// <summary>
        /// Tests two vectors for componentwise inequivalence.
        /// </summary>
        /// <param name="a">First vector to test for inequivalence.</param>
        /// <param name="b">Second vector to test for inequivalence.</param>
        /// <returns>Whether the vectors were inequivalent.</returns>
        public static bool operator !=(FPVector2 a, FPVector2 b) {
            return a.x != b.x || a.y != b.y;
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(FPVector2 other) {
            return x == other.x && y == other.y;
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <returns>
        /// true if <paramref name="obj"/> and this instance are the same type and represent the same value; otherwise, false.
        /// </returns>
        /// <param name="obj">Another object to compare to. </param><filterpriority>2</filterpriority>
        public override bool Equals(object obj) {
            if (obj is FPVector2) {
                return Equals((FPVector2)obj);
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
            return x.GetHashCode() + y.GetHashCode();
        }


    }
}
