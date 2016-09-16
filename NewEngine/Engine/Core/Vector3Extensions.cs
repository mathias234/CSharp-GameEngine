using System;
using OpenTK;

namespace NewEngine.Engine.Core {
    public static class Vector3Extensions {
        public static Vector3 Rotate(this Vector3 vector3, float angle, Vector3 axis) {
            var sinAngle = (float)Math.Sin(-angle);
            var cosAngle = (float)Math.Cos(-angle);

            return vector3.Cross(axis * sinAngle) + (vector3 * cosAngle) + axis * Vector3.Dot(vector3, axis * (1 - cosAngle));
        }

        public static Vector3 Cross(this Vector3 vector3, Vector3 r) {
            float x = vector3[1] * r[2] - vector3[2] * r[1];
            float y = vector3[2] * r[0] - vector3[0] * r[2];
            float z = vector3[0] * r[1] - vector3[1] * r[0];

            return new Vector3(x, y, z);
        }

        public static Vector3 Rotate(this Vector3 vector3, Quaternion rotation) {
            var conjugate = rotation;
            conjugate.Conjugate();

            var w = Util.Mul(rotation, vector3) * conjugate;

            return new Vector3(w.X, w.Y, w.Z);
        }

        public static double Distance(this Vector3 a, Vector3 b) {
            var vector3 = new Vector3(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
            return Math.Sqrt(vector3.X * vector3.X + vector3.Y * vector3.Y + vector3.Z * vector3.Z);
        }
    }
}
