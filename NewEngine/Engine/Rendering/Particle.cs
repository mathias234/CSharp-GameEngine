using System;
using OpenTK;

namespace NewEngine.Engine.Rendering {
    public struct Particle : IEquatable<Particle>, IComparable<Particle> {
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

        public bool Equals(Particle other) {
            return Math.Abs(other.CameraDistance - CameraDistance) < 0.05f;
        }

        public int CompareTo(Particle other) {
            return CameraDistance < other.CameraDistance ? 1 : 0;
        }
    }
}