using UnityEngine;
using FixMath.NET;

namespace JackFrame.FPMath.Sample {

    public static class Extentions {

        public static FP64 ToFP64(this float v) {
            return FP64.ToFP64(v);
        }

        public static FPVector3 ToFPVector3(this Vector3 v) {
            return new FPVector3(v.x.ToFP64(), v.y.ToFP64(), v.z.ToFP64());
        }

        public static FPVector2 ToFPVector2(this Vector2 v) {
            return new FPVector2(v.x.ToFP64(), v.y.ToFP64());
        }

        public static FPVector2 ToFPVector2(this Vector3 v) {
            return new FPVector2(v.x.ToFP64(), v.y.ToFP64());
        }

        public static Vector2 ToVector2(this FPVector2 v) {
            return new Vector2(v.x.AsFloat(), v.y.AsFloat());
        }

        public static Vector3 ToVector3(this FPVector3 v) {
            return new Vector3(v.x.AsFloat(), v.y.AsFloat(), v.z.AsFloat());
        }
    }
}