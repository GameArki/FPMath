using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace FixMath.NET {

    /// <summary>
    /// Represents a Q31.32 fixed-point number.
    /// </summary>
    [Serializable]
    public partial struct FP64 : IEquatable<FP64>, IComparable<FP64> {
        // Field is public and mutable to allow serialization by XNA Content Pipeline
        public long RawValue;

        // Precision of this type is 2^-32, that is 2,3283064365386962890625E-10
        public static readonly decimal Precision = (decimal)(new FP64(1L));//0.00000000023283064365386962890625m;
        public static readonly FP64 MaxValue = new FP64(MAX_VALUE);
        public static readonly FP64 MinValue = new FP64(MIN_VALUE);
        public static readonly FP64 MinusOne = new FP64(-ONE);
        public static readonly FP64 One = new FP64(ONE);
        public static readonly FP64 Two = (FP64)2;
        public static readonly FP64 Three = (FP64)3;
        public static readonly FP64 Zero = new FP64();
        public static readonly FP64 Half = One / Two;
        public static readonly FP64 Quarter = Half * Half;
        public static readonly FP64 C0p25 = (FP64)0.25m;
        public static readonly FP64 C0p28 = (FP64)0.28m;
        public static readonly FP64 EN1 = (FP64)1 / (FP64)10;
        public static readonly FP64 EN2 = (FP64)1 / (FP64)100;
        public static readonly FP64 EN3 = (FP64)1 / (FP64)1000;
        public static readonly FP64 EN4 = (FP64)1 / (FP64)10000;
        public static readonly FP64 C1m1em12 = FP64.One - (FP64)1e-12m;
        public static readonly FP64 Epsilon = F64.C1 / new FP64(10000000);
        /// <summary>
        /// The value of Pi
        /// </summary>
        public static readonly FP64 Pi = new FP64(PI);
        public static readonly FP64 PiOver2 = new FP64(PI_OVER_2);
        public static readonly FP64 PiOver4 = new FP64(PI_OVER_4);
        public static readonly FP64 PiTimes2 = new FP64(PI_TIMES_2);
        public static readonly FP64 PiInv = (FP64)0.3183098861837906715377675267M;
        public static readonly FP64 PiOver2Inv = (FP64)0.6366197723675813430755350535M;
        public static readonly FP64 E = new FP64(E_RAW);
        public static readonly FP64 EPow4 = new FP64(EPOW4);
        public static readonly FP64 Ln2 = new FP64(LN2);
        public static readonly FP64 Log2Max = new FP64(LOG2MAX);
        public static readonly FP64 Log2Min = new FP64(LOG2MIN);
        public static readonly FP64 Deg2Rad = Pi / new FP64(180);
        public static readonly FP64 Rad2Deg = new FP64(180) / Pi;
        static readonly FP64 LutInterval = (FP64)(LUT_SIZE - 1) / PiOver2;
        const long MAX_VALUE = long.MaxValue;
        const long MIN_VALUE = long.MinValue;
        const int NUM_BITS = 64;
        const int FRACTIONAL_PLACES = 32;
        public const long ONE = 1L << FRACTIONAL_PLACES;
        const long PI_TIMES_2 = 0x6487ED511;
        const long PI = 0x3243F6A88;
        const long PI_OVER_2 = 0x1921FB544;
        const long PI_OVER_4 = 0xC90FDAA2;
        const long E_RAW = 0x2B7E15162;
        const long EPOW4 = 0x3699205C4E;
        const long LN2 = 0xB17217F7;
        const long LOG2MAX = 0x1F00000000;
        const long LOG2MIN = -0x2000000000;
        const int LUT_SIZE = (int)(PI_OVER_2 >> 15);

        /// <summary>
        /// Returns a number indicating the sign of a FP64 number.
        /// Returns 1 if the value is positive, 0 if is 0, and -1 if it is negative.
        /// </summary>
        public static int SignI(FP64 value) {
            return
                value.RawValue < 0 ? -1 :
                value.RawValue > 0 ? 1 :
                0;
        }

        public static FP64 Sign(FP64 v) {
            long raw = v.RawValue;
            return
                raw < 0 ? MinusOne :
                raw > 0 ? One :
                FP64.Zero;
        }


        /// <summary>
        /// Returns the absolute value of a FP64 number.
        /// Note: Abs(FP64.MinValue) == FP64.MaxValue.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FP64 Abs(FP64 value) {
            if (value.RawValue == MIN_VALUE) {
                return MaxValue;
            }

            // branchless implementation, see http://www.strchr.com/optimized_abs_function
            var mask = value.RawValue >> 63;
            return new FP64((value.RawValue + mask) ^ mask);
        }

        /// <summary>
        /// Returns the largest integer less than or equal to the specified number.
        /// </summary>
        public static FP64 Floor(FP64 value) {
            // Just zero out the fractional part
            return new FP64((long)((ulong)value.RawValue & 0xFFFFFFFF00000000));
        }

        public static FP64 Log2(FP64 x) {
            if (x.RawValue <= 0)
                throw new ArgumentOutOfRangeException("Non-positive value passed to Ln", "x");

            // This implementation is based on Clay. S. Turner's fast binary logarithm
            // algorithm[1].

            long b = 1U << (FRACTIONAL_PLACES - 1);
            long y = 0;

            long rawX = x.RawValue;
            while (rawX < ONE) {
                rawX <<= 1;
                y -= ONE;
            }

            while (rawX >= (ONE << 1)) {
                rawX >>= 1;
                y += ONE;
            }

            FP64 z = FP64.FromRaw(rawX);

            for (int i = 0; i < FRACTIONAL_PLACES; i++) {
                z = z * z;
                if (z.RawValue >= (ONE << 1)) {
                    z = FP64.FromRaw(z.RawValue >> 1);
                    y += b;
                }
                b >>= 1;
            }

            return FP64.FromRaw(y);
        }

        public static FP64 Ln(FP64 x) {
            return Log2(x) * Ln2;
        }

        public static FP64 Pow2(FP64 x) {
            if (x.RawValue == 0) return One;

            // Avoid negative arguments by exploiting that exp(-x) = 1/exp(x).
            bool neg = (x.RawValue < 0);
            if (neg) x = -x;

            if (x == One)
                return neg ? One / Two : Two;
            if (x >= Log2Max) return neg ? One / MaxValue : MaxValue;
            if (x <= Log2Min) return neg ? MaxValue : Zero;

            /* The algorithm is based on the power series for exp(x):
             * http://en.wikipedia.org/wiki/Exponential_function#Formal_definition
             * 
             * From term n, we get term n+1 by multiplying with x/n.
             * When the sum term drops to zero, we can stop summing.
             */

            int integerPart = (int)Floor(x);
            x = FractionalPart(x);

            FP64 result = One;
            FP64 term = One;
            int i = 1;
            while (term.RawValue != 0) {
                term = x * term * Ln2 / (FP64)i;
                result += term;
                i++;
            }

            result = FromRaw(result.RawValue << integerPart);
            if (neg) result = One / result;

            return result;
        }

        public static FP64 Pow(FP64 b, FP64 exp) {
            if (b == One)
                return One;
            if (exp.RawValue == 0)
                return One;
            if (b.RawValue == 0)
                return Zero;

            FP64 log2 = Log2(b);
            return Pow2(SafeMul(exp, log2));
        }

        /// <summary>
        /// Returns the arccos of of the specified number, calculated using Atan and Sqrt
        /// </summary>
        public static FP64 Acos(FP64 x) {
            if (x.RawValue == 0)
                return FP64.PiOver2;

            FP64 result = FP64.Atan(FP64.Sqrt(One - x * x) / x);
            if (x.RawValue < 0)
                return result + FP64.Pi;
            else
                return result;
        }

        /// <summary>
        /// Returns the smallest integral value that is greater than or equal to the specified number.
        /// </summary>
        public static FP64 Ceiling(FP64 value) {
            var hasFractionalPart = (value.RawValue & 0x00000000FFFFFFFF) != 0;
            return hasFractionalPart ? Floor(value) + One : value;
        }

        /// <summary>
        /// Returns the fractional part of the specified number.
        /// </summary>
        public static FP64 FractionalPart(FP64 value) {
            return FP64.FromRaw(value.RawValue & 0x00000000FFFFFFFF);
        }

        public static void SwapMinorToLeft(ref FP64 a, ref FP64 b) {
            if (a > b) {
                FP64 t = a;
                a = b;
                b = t;
            }
        }

        public static FP64 Min(in FP64 a, in FP64 b) {
            return a > b ? b : a;
        }

        public static FP64 Min(ReadOnlySpan<FP64> nums) {
            FP64 min = nums[0];
            for (int i = 1; i < nums.Length; i++) {
                var n = nums[i];
                if (n < min) {
                    min = n;
                }
            }

            return min;
        }

        public static FP64 Max(in FP64 a, in FP64 b) {
            return a > b ? a : b;
        }

        public static FP64 Max(ReadOnlySpan<FP64> nums) {
            FP64 max = nums[0];
            for (int i = 1; i < nums.Length; i++) {
                var n = nums[i];
                if (n > max) {
                    max = n;
                }
            }

            return max;
        }

        /// <summary>
        /// Rounds a value to the nearest integral value.
        /// If the value is halfway between an even and an uneven value, returns the even value.
        /// </summary>
        public static FP64 Round(FP64 value) {
            var fractionalPart = value.RawValue & 0x00000000FFFFFFFF;
            var integralPart = Floor(value);
            if (fractionalPart < 0x80000000) {
                return integralPart;
            }
            if (fractionalPart > 0x80000000) {
                return integralPart + One;
            }
            // if number is halfway between two values, round to the nearest even number
            // this is the method used by System.Math.Round().
            return (integralPart.RawValue & ONE) == 0
                       ? integralPart
                       : integralPart + One;
        }

        public static FP64 Clamp(FP64 value, in FP64 min, in FP64 max) {
            if (value < min) {
                value = min;
                return value;
            }
            if (value > max) {
                value = max;
            }
            return value;
        }

        /// <summary>
        /// Adds x and y. Performs saturating addition, i.e. in case of overflow, 
        /// rounds to MinValue or MaxValue depending on sign of operands.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FP64 operator +(FP64 x, FP64 y) {
#if CHECKMATH
			var xl = x.RawValue;
            var yl = y.RawValue;
            var sum = xl + yl;
            // if signs of operands are equal and signs of sum and x are different
            if (((~(xl ^ yl) & (xl ^ sum)) & MIN_VALUE) != 0) {
                sum = xl > 0 ? MAX_VALUE : MIN_VALUE;
            }
            return new FP64(sum);
#else
            return new FP64(x.RawValue + y.RawValue);
#endif
        }

        public static FP64 SafeAdd(FP64 x, FP64 y) {
            var xl = x.RawValue;
            var yl = y.RawValue;
            var sum = xl + yl;
            // if signs of operands are equal and signs of sum and x are different
            if (((~(xl ^ yl) & (xl ^ sum)) & MIN_VALUE) != 0) {
                sum = xl > 0 ? MAX_VALUE : MIN_VALUE;
            }
            return new FP64(sum);
        }

        /// <summary>
        /// Subtracts y from x. Performs saturating substraction, i.e. in case of overflow, 
        /// rounds to MinValue or MaxValue depending on sign of operands.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FP64 operator -(FP64 x, FP64 y) {
#if CHECKMATH
			var xl = x.RawValue;
            var yl = y.RawValue;
            var diff = xl - yl;
            // if signs of operands are different and signs of sum and x are different
            if ((((xl ^ yl) & (xl ^ diff)) & MIN_VALUE) != 0) {
                diff = xl < 0 ? MIN_VALUE : MAX_VALUE;
            }
            return new FP64(diff);
#else
            return new FP64(x.RawValue - y.RawValue);
#endif
        }

        public static FP64 SafeSub(FP64 x, FP64 y) {
            var xl = x.RawValue;
            var yl = y.RawValue;
            var diff = xl - yl;
            // if signs of operands are different and signs of sum and x are different
            if ((((xl ^ yl) & (xl ^ diff)) & MIN_VALUE) != 0) {
                diff = xl < 0 ? MIN_VALUE : MAX_VALUE;
            }
            return new FP64(diff);
        }

        static long AddOverflowHelper(long x, long y, ref bool overflow) {
            var sum = x + y;
            // x + y overflows if sign(x) ^ sign(y) != sign(sum)
            overflow |= ((x ^ y ^ sum) & MIN_VALUE) != 0;
            return sum;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FP64 operator *(FP64 x, FP64 y) {
            if (x == Zero || y == Zero) {
                return Zero;
            }
#if CHECKMATH
			var xl = x.RawValue;
            var yl = y.RawValue;

            var xlo = (ulong)(xl & 0x00000000FFFFFFFF);
            var xhi = xl >> FRACTIONAL_PLACES;
            var ylo = (ulong)(yl & 0x00000000FFFFFFFF);
            var yhi = yl >> FRACTIONAL_PLACES;

            var lolo = xlo * ylo;
            var lohi = (long)xlo * yhi;
            var hilo = xhi * (long)ylo;
            var hihi = xhi * yhi;

            var loResult = lolo >> FRACTIONAL_PLACES;
            var midResult1 = lohi;
            var midResult2 = hilo;
            var hiResult = hihi << FRACTIONAL_PLACES;

            bool overflow = false;
            var sum = AddOverflowHelper((long)loResult, midResult1, ref overflow);
            sum = AddOverflowHelper(sum, midResult2, ref overflow);
            sum = AddOverflowHelper(sum, hiResult, ref overflow);

            bool opSignsEqual = ((xl ^ yl) & MIN_VALUE) == 0;

            // if signs of operands are equal and sign of result is negative,
            // then multiplication overflowed positively
            // the reverse is also true
            if (opSignsEqual) {
                if (sum < 0 || (overflow && xl > 0)) {
					throw new OverflowException();
                    return MaxValue;
                }
            }
            else {
                if (sum > 0) {
					throw new OverflowException();
					return MinValue;
                }
            }

            // if the top 32 bits of hihi (unused in the result) are neither all 0s or 1s,
            // then this means the result overflowed.
            var topCarry = hihi >> FRACTIONAL_PLACES;
            if (topCarry != 0 && topCarry != -1 /*&& xl != -17 && yl != -17*/) {
				throw new OverflowException();
				return opSignsEqual ? MaxValue : MinValue; 
            }

            // If signs differ, both operands' magnitudes are greater than 1,
            // and the result is greater than the negative operand, then there was negative overflow.
            if (!opSignsEqual) {
                long posOp, negOp;
                if (xl > yl) {
                    posOp = xl;
                    negOp = yl;
                }
                else {
                    posOp = yl;
                    negOp = xl;
                }
                if (sum > negOp && negOp < -ONE && posOp > ONE) {
					throw new OverflowException();
					return MinValue;
                }
            }

            return new FP64(sum);
#else
            var xl = x.RawValue;
            var yl = y.RawValue;

            var xlo = (ulong)(xl & 0x00000000FFFFFFFF);
            var xhi = xl >> FRACTIONAL_PLACES;
            var ylo = (ulong)(yl & 0x00000000FFFFFFFF);
            var yhi = yl >> FRACTIONAL_PLACES;

            var lolo = xlo * ylo;
            var lohi = (long)xlo * yhi;
            var hilo = xhi * (long)ylo;
            var hihi = xhi * yhi;

            var loResult = lolo >> FRACTIONAL_PLACES;
            var midResult1 = lohi;
            var midResult2 = hilo;
            var hiResult = hihi << FRACTIONAL_PLACES;

            var sum = (long)loResult + midResult1 + midResult2 + hiResult;
            return new FP64(sum);
#endif
        }

        public static FP64 SafeMul(FP64 x, FP64 y) {
            var xl = x.RawValue;
            var yl = y.RawValue;

            var xlo = (ulong)(xl & 0x00000000FFFFFFFF);
            var xhi = xl >> FRACTIONAL_PLACES;
            var ylo = (ulong)(yl & 0x00000000FFFFFFFF);
            var yhi = yl >> FRACTIONAL_PLACES;

            var lolo = xlo * ylo;
            var lohi = (long)xlo * yhi;
            var hilo = xhi * (long)ylo;
            var hihi = xhi * yhi;

            var loResult = lolo >> FRACTIONAL_PLACES;
            var midResult1 = lohi;
            var midResult2 = hilo;
            var hiResult = hihi << FRACTIONAL_PLACES;

            bool overflow = false;
            var sum = AddOverflowHelper((long)loResult, midResult1, ref overflow);
            sum = AddOverflowHelper(sum, midResult2, ref overflow);
            sum = AddOverflowHelper(sum, hiResult, ref overflow);

            bool opSignsEqual = ((xl ^ yl) & MIN_VALUE) == 0;

            // if signs of operands are equal and sign of result is negative,
            // then multiplication overflowed positively
            // the reverse is also true
            if (opSignsEqual) {
                if (sum < 0 || (overflow && xl > 0)) {
                    return MaxValue;
                }
            } else {
                if (sum > 0) {
                    return MinValue;
                }
            }

            // if the top 32 bits of hihi (unused in the result) are neither all 0s or 1s,
            // then this means the result overflowed.
            var topCarry = hihi >> FRACTIONAL_PLACES;
            if (topCarry != 0 && topCarry != -1 /*&& xl != -17 && yl != -17*/) {
                return opSignsEqual ? MaxValue : MinValue;
            }

            // If signs differ, both operands' magnitudes are greater than 1,
            // and the result is greater than the negative operand, then there was negative overflow.
            if (!opSignsEqual) {
                long posOp, negOp;
                if (xl > yl) {
                    posOp = xl;
                    negOp = yl;
                } else {
                    posOp = yl;
                    negOp = xl;
                }
                if (sum > negOp && negOp < -ONE && posOp > ONE) {
                    return MinValue;
                }
            }

            return new FP64(sum);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static int CountLeadingZeroes(ulong x) {
            int result = 0;
            while ((x & 0xF000000000000000) == 0) { result += 4; x <<= 4; }
            while ((x & 0x8000000000000000) == 0) { result += 1; x <<= 1; }
            return result;
        }

        public static FP64 operator /(FP64 x, FP64 y) {
            var xl = x.RawValue;
            var yl = y.RawValue;

            if (yl == 0) {
                return FP64.MaxValue;
                //throw new DivideByZeroException();
            }

            var remainder = (ulong)(xl >= 0 ? xl : -xl);
            var divider = (ulong)(yl >= 0 ? yl : -yl);
            var quotient = 0UL;
            var bitPos = NUM_BITS / 2 + 1;


            // If the divider is divisible by 2^n, take advantage of it.
            while ((divider & 0xF) == 0 && bitPos >= 4) {
                divider >>= 4;
                bitPos -= 4;
            }

            while (remainder != 0 && bitPos >= 0) {
                int shift = CountLeadingZeroes(remainder);
                if (shift > bitPos) {
                    shift = bitPos;
                }
                remainder <<= shift;
                bitPos -= shift;

                var div = remainder / divider;
                remainder = remainder % divider;
                quotient += div << bitPos;

                // Detect overflow
                if ((div & ~(0xFFFFFFFFFFFFFFFF >> bitPos)) != 0) {
                    return ((xl ^ yl) & MIN_VALUE) == 0 ? MaxValue : MinValue;
                }

                remainder <<= 1;
                --bitPos;
            }

            // rounding
            ++quotient;
            var result = (long)(quotient >> 1);
            if (((xl ^ yl) & MIN_VALUE) != 0) {
                result = -result;
            }

            return new FP64(result);
        }

        public static FP64 operator %(FP64 x, FP64 y) {
            return new FP64(
                x.RawValue == MIN_VALUE & y.RawValue == -1 ?
                0 :
                x.RawValue % y.RawValue);
        }

        /// <summary>
        /// Performs modulo as fast as possible; throws if x == MinValue and y == -1.
        /// Use the operator (%) for a more reliable but slower modulo.
        /// </summary>
        public static FP64 FastMod(FP64 x, FP64 y) {
            return new FP64(x.RawValue % y.RawValue);
        }

        public static FP64 operator -(FP64 x) {
            return x.RawValue == MIN_VALUE ? MaxValue : new FP64(-x.RawValue);
        }

        public static bool operator ==(FP64 x, FP64 y) {
            return x.RawValue == y.RawValue;
        }

        public static bool operator !=(FP64 x, FP64 y) {
            return x.RawValue != y.RawValue;
        }

        public static bool operator >(FP64 x, FP64 y) {
            return x.RawValue > y.RawValue;
        }

        public static bool operator <(FP64 x, FP64 y) {
            return x.RawValue < y.RawValue;
        }

        public static bool operator >=(FP64 x, FP64 y) {
            return x.RawValue >= y.RawValue;
        }

        public static bool operator <=(FP64 x, FP64 y) {
            return x.RawValue <= y.RawValue;
        }


        /// <summary>
        /// Returns the square root of a specified number.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The argument was negative.
        /// </exception>
        public static FP64 Sqrt(FP64 x) {
            var xl = x.RawValue;
            if (xl < 0) {
                // We cannot represent infinities like Single and Double, and Sqrt is
                // mathematically undefined for x < 0. So we just throw an exception.
                throw new ArgumentOutOfRangeException("Negative value passed to Sqrt", "x");
            }

            var num = (ulong)xl;
            var result = 0UL;

            // second-to-top bit
            var bit = 1UL << (NUM_BITS - 2);

            while (bit > num) {
                bit >>= 2;
            }

            // The main part is executed twice, in order to avoid
            // using 128 bit values in computations.
            for (var i = 0; i < 2; ++i) {
                // First we get the top 48 bits of the answer.
                while (bit != 0) {
                    if (num >= result + bit) {
                        num -= result + bit;
                        result = (result >> 1) + bit;
                    } else {
                        result = result >> 1;
                    }
                    bit >>= 2;
                }

                if (i == 0) {
                    // Then process it again to get the lowest 16 bits.
                    if (num > (1UL << (NUM_BITS / 2)) - 1) {
                        // The remainder 'num' is too large to be shifted left
                        // by 32, so we have to add 1 to result manually and
                        // adjust 'num' accordingly.
                        // num = a - (result + 0.5)^2
                        //       = num + result^2 - (result + 0.5)^2
                        //       = num - result - 0.5
                        num -= result;
                        num = (num << (NUM_BITS / 2)) - 0x80000000UL;
                        result = (result << (NUM_BITS / 2)) + 0x80000000UL;
                    } else {
                        num <<= (NUM_BITS / 2);
                        result <<= (NUM_BITS / 2);
                    }

                    bit = 1UL << (NUM_BITS / 2 - 2);
                }
            }
            // Finally, if next bit would have been 1, round the result upwards.
            if (num > result) {
                ++result;
            }
            return new FP64((long)result);
        }

        /// <summary>
        /// Returns the Sine of x.
        /// This function has about 9 decimals of accuracy for small values of x.
        /// It may lose accuracy as the value of x grows.
        /// Performance: about 25% slower than Math.Sin() in x64, and 200% slower in x86.
        /// </summary>
        public static FP64 Sin(FP64 x) {
            bool flipHorizontal, flipVertical;
            var clampedL = ClampSinValue(x.RawValue, out flipHorizontal, out flipVertical);
            var clamped = new FP64(clampedL);

            // Find the two closest values in the LUT and perform linear interpolation
            // This is what kills the performance of this function on x86 - x64 is fine though
            var rawIndex = clamped * LutInterval;
            var roundedIndex = Round(rawIndex);
            var indexError = rawIndex - roundedIndex;

            var nearestValue = new FP64(SinLut[flipHorizontal ?
                SinLut.Length - 1 - (int)roundedIndex :
                (int)roundedIndex]);
            var secondNearestValue = new FP64(SinLut[flipHorizontal ?
                SinLut.Length - 1 - (int)roundedIndex - SignI(indexError) :
                (int)roundedIndex + SignI(indexError)]);

            var delta = (indexError * Abs(nearestValue - secondNearestValue)).RawValue;
            var interpolatedValue = nearestValue.RawValue + (flipHorizontal ? -delta : delta);
            var finalValue = flipVertical ? -interpolatedValue : interpolatedValue;
            return new FP64(finalValue);
        }

        /// <summary>
        /// Returns a rough approximation of the Sine of x.
        /// This is at least 3 times faster than Sin() on x86 and slightly faster than Math.Sin(),
        /// however its accuracy is limited to 4-5 decimals, for small enough values of x.
        /// </summary>
        public static FP64 FastSin(FP64 x) {
            bool flipHorizontal, flipVertical;
            var clampedL = ClampSinValue(x.RawValue, out flipHorizontal, out flipVertical);

            // Here we use the fact that the SinLut table has a number of entries
            // equal to (PI_OVER_2 >> 15) to use the angle to index directly into it
            var rawIndex = (uint)(clampedL >> 15);
            if (rawIndex >= LUT_SIZE) {
                rawIndex = LUT_SIZE - 1;
            }
            var nearestValue = SinLut[flipHorizontal ?
                SinLut.Length - 1 - (int)rawIndex :
                (int)rawIndex];
            return new FP64(flipVertical ? -nearestValue : nearestValue);
        }



        [MethodImplAttribute(MethodImplOptions.AggressiveInlining)]
        static long ClampSinValue(long angle, out bool flipHorizontal, out bool flipVertical) {
            // Clamp value to 0 - 2*PI using modulo; this is very slow but there's no better way AFAIK
            var clamped2Pi = angle % PI_TIMES_2;
            if (angle < 0) {
                clamped2Pi += PI_TIMES_2;
            }

            // The LUT contains values for 0 - PiOver2; every other value must be obtained by
            // vertical or horizontal mirroring
            flipVertical = clamped2Pi >= PI;
            // obtain (angle % PI) from (angle % 2PI) - much faster than doing another modulo
            var clampedPi = clamped2Pi;
            while (clampedPi >= PI) {
                clampedPi -= PI;
            }
            flipHorizontal = clampedPi >= PI_OVER_2;
            // obtain (angle % PI_OVER_2) from (angle % PI) - much faster than doing another modulo
            var clampedPiOver2 = clampedPi;
            if (clampedPiOver2 >= PI_OVER_2) {
                clampedPiOver2 -= PI_OVER_2;
            }
            return clampedPiOver2;
        }

        /// <summary>
        /// Returns the cosine of x.
        /// See Sin() for more details.
        /// </summary>
        public static FP64 Cos(FP64 x) {
            var xl = x.RawValue;
            var rawAngle = xl + (xl > 0 ? -PI - PI_OVER_2 : PI_OVER_2);
            return Sin(new FP64(rawAngle));
        }

        /// <summary>
        /// Returns a rough approximation of the cosine of x.
        /// See FastSin for more details.
        /// </summary>
        public static FP64 FastCos(FP64 x) {
            var xl = x.RawValue;
            var rawAngle = xl + (xl > 0 ? -PI - PI_OVER_2 : PI_OVER_2);
            return FastSin(new FP64(rawAngle));
        }

        /// <summary>
        /// Returns the tangent of x.
        /// </summary>
        /// <remarks>
        /// This function is not well-tested. It may be wildly inaccurate.
        /// </remarks>
        public static FP64 Tan(FP64 x) {
            var clampedPi = x.RawValue % PI;
            var flip = false;
            if (clampedPi < 0) {
                clampedPi = -clampedPi;
                flip = true;
            }
            if (clampedPi > PI_OVER_2) {
                flip = !flip;
                clampedPi = PI_OVER_2 - (clampedPi - PI_OVER_2);
            }

            var clamped = new FP64(clampedPi);

            // Find the two closest values in the LUT and perform linear interpolation
            var rawIndex = clamped * LutInterval;
            var roundedIndex = Round(rawIndex);
            var indexError = rawIndex - roundedIndex;

            var nearestValue = new FP64(TanLut[(int)roundedIndex]);
            var secondNearestValue = new FP64(TanLut[(int)roundedIndex + SignI(indexError)]);

            var delta = (indexError * Abs(nearestValue - secondNearestValue)).RawValue;
            var interpolatedValue = nearestValue.RawValue + delta;
            var finalValue = flip ? -interpolatedValue : interpolatedValue;
            return new FP64(finalValue);
        }

        public static FP64 FastAtan2(FP64 y, FP64 x) {
            var yl = y.RawValue;
            var xl = x.RawValue;
            if (xl == 0) {
                if (yl > 0) {
                    return PiOver2;
                }
                if (yl == 0) {
                    return Zero;
                }
                return -PiOver2;
            }
            FP64 atan;
            var z = y / x;

            // Deal with overflow
            if (SafeAdd(One, SafeMul(SafeMul(C0p28, z), z)) == MaxValue) {
                return y.RawValue < 0 ? -PiOver2 : PiOver2;
            }

            if (Abs(z) < One) {
                atan = z / (One + C0p28 * z * z);
                if (xl < 0) {
                    if (yl < 0) {
                        return atan - Pi;
                    }
                    return atan + Pi;
                }
            } else {
                atan = PiOver2 - z / (z * z + C0p28);
                if (yl < 0) {
                    return atan - Pi;
                }
            }
            return atan;
        }

        /// <summary>
        /// Returns the arctan of of the specified number, calculated using Euler series
        /// </summary>
        public static FP64 Atan(FP64 z) {
            if (z.RawValue == 0)
                return Zero;

            // Force positive values for argument
            // Atan(-z) = -Atan(z).
            bool neg = (z.RawValue < 0);
            if (neg) z = -z;

            FP64 result;

            if (z == One)
                result = PiOver4;
            else {
                bool invert = z > One;
                if (invert) z = One / z;

                result = One;
                FP64 term = One;

                FP64 zSq = z * z;
                FP64 zSq2 = zSq * Two;
                FP64 zSqPlusOne = zSq + One;
                FP64 zSq12 = zSqPlusOne * Two;
                FP64 dividend = zSq2;
                FP64 divisor = zSqPlusOne * Three;

                for (int i = 2; i < 30; i++) {
                    term *= dividend / divisor;
                    result += term;

                    dividend += zSq2;
                    divisor += zSq12;

                    if (term.RawValue == 0)
                        break;
                }

                result = result * z / zSqPlusOne;

                if (invert)
                    result = PiOver2 - result;
            }

            if (neg) result = -result;
            return result;
        }

        public static FP64 Atan2(FP64 y, FP64 x) {
            var yl = y.RawValue;
            var xl = x.RawValue;
            if (xl == 0) {
                if (yl > 0)
                    return PiOver2;
                if (yl == 0)
                    return Zero;
                return -PiOver2;
            }

            var z = y / x;

            // Deal with overflow
            if (SafeAdd(One, SafeMul(SafeMul((FP64)0.28M, z), z)) == MaxValue) {
                return y.RawValue < 0 ? -PiOver2 : PiOver2;
            }
            FP64 atan = Atan(z);

            if (xl < 0) {
                if (yl < 0)
                    return atan - Pi;
                return atan + Pi;
            }

            return atan;
        }

        public float AsFloat() {
            return (float)this;
        }

        public int AsInt() {
            return (int)this;
        }

        public static implicit operator FP64(int value) {
            return new FP64(value);
        }
        public static explicit operator FP64(long value) {
            return new FP64(value * ONE);
        }
        public static explicit operator long(FP64 value) {
            return value.RawValue >> FRACTIONAL_PLACES;
        }

        public static explicit operator float(FP64 value) {
            return (float)value.RawValue / ONE;
        }
        public static explicit operator FP64(double value) {
            return new FP64((long)(value * ONE));
        }
        public static explicit operator double(FP64 value) {
            return (double)value.RawValue / ONE;
        }
        public static implicit operator FP64(decimal value) {
            return new FP64((long)(value * ONE));
        }
        public static explicit operator decimal(FP64 value) {
            return (decimal)value.RawValue / ONE;
        }

        public static FP64 ToFP64(float value) {
            return new FP64((long)(value * ONE));
        }

        public override bool Equals(object obj) {
            return obj is FP64 && ((FP64)obj).RawValue == RawValue;
        }

        public override int GetHashCode() {
            return RawValue.GetHashCode();
        }

        public bool Equals(FP64 other) {
            return RawValue == other.RawValue;
        }

        public int CompareTo(FP64 other) {
            return RawValue.CompareTo(other.RawValue);
        }

        public override string ToString() {
            return ((decimal)this).ToString();
        }

        public static FP64 FromRaw(long rawValue) {
            return new FP64(rawValue);
        }

        internal static void GenerateSinLut() {
            using (var writer = new StreamWriter("FP64SinLut.cs")) {
                writer.Write(
@"namespace FixMath.NET {
    partial struct FP64 {
        public static readonly long[] SinLut = new[] {");
                int lineCounter = 0;
                for (int i = 0; i < LUT_SIZE; ++i) {
                    var angle = i * Math.PI * 0.5 / (LUT_SIZE - 1);
                    if (lineCounter++ % 8 == 0) {
                        writer.WriteLine();
                        writer.Write("            ");
                    }
                    var sin = Math.Sin(angle);
                    var rawValue = ((FP64)sin).RawValue;
                    writer.Write(string.Format("0x{0:X}L, ", rawValue));
                }
                writer.Write(
@"
        };
    }
}");
            }
        }

        internal static void GenerateTanLut() {
            using (var writer = new StreamWriter("FP64TanLut.cs")) {
                writer.Write(
@"namespace FixMath.NET {
    partial struct FP64 {
        public static readonly long[] TanLut = new[] {");
                int lineCounter = 0;
                for (int i = 0; i < LUT_SIZE; ++i) {
                    var angle = i * Math.PI * 0.5 / (LUT_SIZE - 1);
                    if (lineCounter++ % 8 == 0) {
                        writer.WriteLine();
                        writer.Write("            ");
                    }
                    var tan = Math.Tan(angle);
                    if (tan > (double)MaxValue || tan < 0.0) {
                        tan = (double)MaxValue;
                    }
                    var rawValue = (((decimal)tan > (decimal)MaxValue || tan < 0.0) ? MaxValue : (FP64)tan).RawValue;
                    writer.Write(string.Format("0x{0:X}L, ", rawValue));
                }
                writer.Write(
@"
        };
    }
}");
            }
        }

        /// <summary>
        /// This is the constructor from raw value; it can only be used interally.
        /// </summary>
        /// <param name="rawValue"></param>
        FP64(long rawValue) {
            RawValue = rawValue;
        }

        public FP64(int value) {
            RawValue = value * ONE;
        }
    }
}
