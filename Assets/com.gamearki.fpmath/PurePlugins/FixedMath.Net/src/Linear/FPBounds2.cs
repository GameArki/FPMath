namespace FixMath.NET {

    public struct FPBounds2 {

        public FPVector2 min;
        public FPVector2 max;

        public FPVector2 Center => (min + max) * FP64.Half;
        public FPVector2 Size => max - min;

        public FPBounds2(in FPVector2 center, in FPVector2 size) {
            var half = size * FP64.Half;
            this.min = center - half;
            this.max = center + half;
        }

        public bool IsIntersectOrContains(in FPBounds2 other) {
            return IsIntersect(other) || IsContains(other);
        }

        public bool IsIntersect(in FPBounds2 other) {
            return (this.min.x <= other.max.x && this.max.x >= other.min.x
                && this.min.y <= other.max.y && this.max.y >= other.min.y);
        }

        public bool IsContains(in FPBounds2 other) {
            return (this.min.x <= other.min.x && this.max.x >= other.max.x &&
                   this.min.y <= other.min.y && this.max.y >= other.max.y) ||
                   (this.min.x >= other.min.x && this.max.x <= other.max.x &&
                   this.min.y >= other.min.y && this.max.y <= other.max.y);
        }

        public string ToFullString() {
            return $"center: {Center.ToString()}, size: {Size.ToString()}, min: {min.ToString()}, max: {max.ToString()}";
        }

    }

}