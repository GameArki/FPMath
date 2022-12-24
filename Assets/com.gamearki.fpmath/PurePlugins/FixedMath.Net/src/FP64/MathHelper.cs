namespace FixMath.NET {
    /// <summary>
    /// Contains helper math methods.
    /// </summary>
    public static class MathHelper {
        /// <summary>
        /// Approximate value of Pi.
        /// </summary>
        public static readonly FP64 Pi = FP64.Pi;

        /// <summary>
        /// Approximate value of Pi multiplied by two.
        /// </summary>
        public static readonly FP64 TwoPi = FP64.PiTimes2;

        /// <summary>
        /// Approximate value of Pi divided by two.
        /// </summary>
        public static readonly FP64 PiOver2 = FP64.PiOver2;

        /// <summary>
        /// Approximate value of Pi divided by four.
        /// </summary>
        public static readonly FP64 PiOver4 = FP64.Pi / new FP64(4);

        /// <summary>
        /// Calculate remainder of of FP64 division using same algorithm
        /// as Math.IEEERemainder
        /// </summary>
        /// <param name="dividend">Dividend</param>
        /// <param name="divisor">Divisor</param>
        /// <returns>Remainder</returns>
        public static FP64 IEEERemainder(FP64 dividend, FP64 divisor) {
            return dividend - (divisor * FP64.Round(dividend / divisor));
        }

        /// <summary>
        /// Reduces the angle into a range from -Pi to Pi.
        /// </summary>
        /// <param name="angle">Angle to wrap.</param>
        /// <returns>Wrapped angle.</returns>
        public static FP64 WrapAngle(FP64 angle) {
            angle = IEEERemainder(angle, TwoPi);
            if (angle < -Pi) {
                angle += TwoPi;
                return angle;
            }
            if (angle >= Pi) {
                angle -= TwoPi;
            }
            return angle;

        }

        /// <summary>
        /// Clamps a value between a minimum and maximum value.
        /// </summary>
        /// <param name="value">Value to clamp.</param>
        /// <param name="min">Minimum value.  If the value is less than this, the minimum is returned instead.</param>
        /// <param name="max">Maximum value.  If the value is more than this, the maximum is returned instead.</param>
        /// <returns>Clamped value.</returns>
        public static FP64 Clamp(FP64 value, FP64 min, FP64 max) {
            if (value < min)
                return min;
            else if (value > max)
                return max;
            return value;
        }


        /// <summary>
        /// Returns the higher value of the two parameters.
        /// </summary>
        /// <param name="a">First value.</param>
        /// <param name="b">Second value.</param>
        /// <returns>Higher value of the two parameters.</returns>
        public static FP64 Max(in FP64 a, in FP64 b) {
            return a > b ? a : b;
        }

        public static FP64 Max(in FP64 a, in FP64 b, in FP64 c) {
            FP64 res = Max(a, b);
            res = Max(res, c);
            return res;
        }

        /// <summary>
        /// Returns the lower value of the two parameters.
        /// </summary>
        /// <param name="a">First value.</param>
        /// <param name="b">Second value.</param>
        /// <returns>Lower value of the two parameters.</returns>
        public static FP64 Min(in FP64 a, in FP64 b) {
            return a < b ? a : b;
        }

        public static FP64 Min(in FP64 a, in FP64 b, in FP64 c) {
            FP64 res = Min(a, b);
            return Min(res, c);
        }

        /// <summary>
        /// Converts degrees to radians.
        /// </summary>
        /// <param name="degrees">Degrees to convert.</param>
        /// <returns>Radians equivalent to the input degrees.</returns>
        public static FP64 ToRadians(FP64 degrees) {
            return degrees * (Pi / F64.C180);
        }

        /// <summary>
        /// Converts radians to degrees.
        /// </summary>
        /// <param name="radians">Radians to convert.</param>
        /// <returns>Degrees equivalent to the input radians.</returns>
        public static FP64 ToDegrees(FP64 radians) {
            return radians * (F64.C180 / Pi);
        }
    }
}
