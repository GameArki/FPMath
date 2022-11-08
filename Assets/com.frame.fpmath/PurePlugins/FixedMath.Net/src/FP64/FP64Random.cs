using System;

namespace FixMath.NET
{
	public class Fix64Random
    {
        private Random random;

        public Fix64Random(int seed)
        {
            random = new Random(seed);
        }

        public FP64 Next()
        {
            FP64 result = new FP64();
            result.RawValue = (uint)random.Next(int.MinValue, int.MaxValue);
            return result;
        }

        public FP64 NextInt(int maxValue)
        {
            return random.Next(maxValue);
        }
    }
}
