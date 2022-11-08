namespace FixMath.NET {

    public struct FPBounds3 {

        public FPVector3 center;
        public FPVector3 size;

        public FPVector3 min;
        public FPVector3 max;

        public FPBounds3(in FPVector3 center, in FPVector3 size) {

            this.center = center;
            this.size = size;

            var half = size * FP64.Half;
            this.min = center - half;
            this.max = center + half;

        }

        public bool IsIntersect(in FPBounds3 other) {
            return this.min.x <= other.max.x && this.max.x >= other.min.x &&
                   this.min.y <= other.max.y && this.max.y >= other.min.y &&
                   this.min.z <= other.max.z && this.max.z >= other.min.z;
        }

    }

}