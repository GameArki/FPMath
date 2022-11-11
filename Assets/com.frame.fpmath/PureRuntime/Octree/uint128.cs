using System;

namespace JackFrame.FPMath {

    public struct uint128 {

        public static readonly uint128 MaxValue = new uint128(ulong.MaxValue, ulong.MaxValue);
        public static readonly uint128 MinValue = new uint128(0, 0);

        public ulong low;
        public ulong high;

        public uint128(ulong low, ulong high) {
            this.low = low;
            this.high = high;
        }

        public static implicit operator uint128(ulong value) {
            return new uint128(value, 0);
        }

        public static implicit operator uint128(uint value) {
            return new uint128(value, 0);
        }

        public static implicit operator uint128(ushort value) {
            return new uint128(value, 0);
        }

        public static implicit operator uint128(byte value) {
            return new uint128(value, 0);
        }

        public static explicit operator ulong(uint128 value) {
            return value.low;
        }

        public static explicit operator uint(uint128 value) {
            return (uint)value.low;
        }

        public static explicit operator ushort(uint128 value) {
            return (ushort)value.low;
        }

        public static explicit operator byte(uint128 value) {
            return (byte)value.low;
        }

        public static uint128 operator |(uint128 a, uint128 b) {
            return new uint128(a.low | b.low, a.high | b.high);
        }

        public static uint128 operator |(uint128 a, ulong b) {
            return new uint128(a.low | b, a.high);
        }

        public static uint128 operator |(uint128 a, uint b) {
            return new uint128(a.low | b, a.high);
        }

        public static uint128 operator |(uint128 a, ushort b) {
            return new uint128(a.low | b, a.high);
        }

        public static uint128 operator |(uint128 a, byte b) {
            return new uint128(a.low | b, a.high);
        }

        public static uint128 operator &(uint128 a, uint128 b) {
            return new uint128(a.low & b.low, a.high & b.high);
        }

        public static uint128 operator &(uint128 a, ulong b) {
            return new uint128(a.low & b, a.high);
        }

        public static uint128 operator &(uint128 a, uint b) {
            return new uint128(a.low & b, a.high);
        }

        public static uint128 operator &(uint128 a, ushort b) {
            return new uint128(a.low & b, a.high);
        }

        public static uint128 operator &(uint128 a, byte b) {
            return new uint128(a.low & b, a.high);
        }

        public static uint128 operator <<(uint128 a, int b) {
            if (b < 64) {
                return new uint128(a.low << b, (a.high << b) | (a.low >> (64 - b)));
            } else {
                return new uint128(0, a.low << (b - 64));
            }
        }

        public static uint128 operator >>(uint128 a, int b) {
            if (b < 64) {
                return new uint128((a.low >> b) | (a.high << (64 - b)), a.high >> b);
            } else {
                return new uint128(a.high >> (b - 64), 0);
            }
        }

        public static bool operator ==(uint128 a, uint128 b) {
            return a.low == b.low && a.high == b.high;
        }

        public static bool operator !=(uint128 a, uint128 b) {
            return a.low != b.low || a.high != b.high;
        }

        public override bool Equals(object obj) {
            if (obj is uint128) {
                return this == (uint128)obj;
            } else {
                return false;
            }
        }

        public override int GetHashCode() {
            return HashCode.Combine(low, high);
        }

    }

}