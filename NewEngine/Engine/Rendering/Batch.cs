using System;
using OpenTK;

namespace NewEngine.Engine.Rendering {
    public struct Batch : IEquatable<Batch>, IComparable<Batch> {
        public Vector3 Position;
        public Vector3 Speed;

        public float R;
        public float G;
        public float B;
        public float A;
        public float Size;
        public float Angle;
        public float Weight;
        public float Life;
        public float CameraDistance;

        public bool Equals(Batch other) {
            return Math.Abs(other.CameraDistance - CameraDistance) < 0.05f;
        }

        public int CompareTo(Batch other) {
            return CameraDistance < other.CameraDistance ? 1 : 0;
        }
    }
}