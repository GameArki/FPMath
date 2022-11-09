namespace FixMath.NET {

    public struct FPBounds2 {

        public FPVector2 center;
        public FPVector2 size;

        public FPVector2 min;
        public FPVector2 max;

        public FPBounds2(in FPVector2 center, in FPVector2 size) {

            this.center = center;
            this.size = size;

            var half = size * FP64.Half;
            this.min = center - half;
            this.max = center + half;

        }

        public bool IsIntersect(in FPBounds2 other) {
            return this.min.x <= other.max.x && this.max.x >= other.min.x &&
                   this.min.y <= other.max.y && this.max.y >= other.min.y;
        }

        public bool IsContains(in FPBounds2 other) {
            return (this.min.x <= other.min.x && this.max.x >= other.max.x &&
                   this.min.y <= other.min.y && this.max.y >= other.max.y) ||
                   (this.min.x >= other.min.x && this.max.x <= other.max.x &&
                   this.min.y >= other.min.y && this.max.y <= other.max.y);
        }

        public string ToFullString() {
            return $"center: {center.ToString()}, size: {size.ToString()}, min: {min.ToString()}, max: {max.ToString()}";
        }

    }

}