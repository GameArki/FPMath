namespace FixMath.NET {

    public struct FPBounds3 {

        public FPVector3 min;
        public FPVector3 max;

        public FPVector3 Center => (min + max) * FP64.Half;
        public FPVector3 Size => max - min;

        public FPBounds3(in FPVector3 center, in FPVector3 size) {
            var half = size * FP64.Half;
            this.min = center - half;
            this.max = center + half;
        }

        public bool IsIntersectOrContains(in FPBounds3 other) {
            return IsIntersect(other) || IsContains(other);
        }

        public bool IsIntersect(in FPBounds3 other) {
            return this.min.x <= other.max.x && this.max.x >= other.min.x &&
                   this.min.y <= other.max.y && this.max.y >= other.min.y &&
                   this.min.z <= other.max.z && this.max.z >= other.min.z;
        }

        public bool IsContains(in FPBounds3 other) {
            return (this.min.x <= other.min.x && this.max.x >= other.max.x &&
                    this.min.y <= other.min.y && this.max.y >= other.max.y &&
                    this.min.z <= other.min.z && this.max.z >= other.max.z) ||
                   (this.min.x >= other.min.x && this.max.x <= other.max.x &&
                    this.min.y >= other.min.y && this.max.y <= other.max.y &&
                    this.min.z >= other.min.z && this.max.z <= other.max.z);
        }

        public string ToFullString() {
            return $"center: {Center.ToString()}, size: {Size.ToString()}, min: {min.ToString()}, max: {max.ToString()}";
        }

    }

}