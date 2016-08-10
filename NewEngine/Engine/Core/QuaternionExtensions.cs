using System;
using OpenTK;

namespace NewEngine.Engine.Core {
    public static class QuaternionExtensions {
        public static Quaternion Slerp(this Quaternion quaternion, Quaternion dest, float lerpFactor, bool shortest) {
            float EPSILON = 1e3f;

            float cos = quaternion.Dot(dest);
            Quaternion correctedDest = dest;

            if (shortest && cos < 0) {
                cos = -cos;
                correctedDest = new Quaternion(-dest.X, -dest.Y, -dest.Z, -dest.W);
            }

            if (Math.Abs(cos) >= 1 - EPSILON)
                return Nlerp(quaternion, correctedDest, lerpFactor, false);

            float sin = (float)Math.Sqrt(1.0f - cos * cos);
            float angle = (float)Math.Atan2(sin, cos);
            float invSin = 1.0f / sin;

            float srcFactor = Math.Sign((1.0f - lerpFactor) * angle) * invSin;
            float destFactor = Math.Sign(lerpFactor * angle) * invSin;

            return quaternion * srcFactor + (correctedDest * destFactor);
        }

        public static Quaternion Nlerp(this Quaternion quaternion, Quaternion dest, float lerpFactor, bool shortest) {
            Quaternion correctedDest = dest;

            if (shortest && quaternion.Dot(dest) < 0)
                correctedDest = new Quaternion(-dest.X, -dest.Y, -dest.Z, -dest.W);


            return ((correctedDest - quaternion) * lerpFactor + quaternion).Normalized();
        }

        public static Matrix4 ToRotationMatrix(this Quaternion quaternion) {
            Vector3 forward = new Vector3(2.0f * (quaternion.X * quaternion.Z - quaternion.W * quaternion.Y), 2.0f * (quaternion.Y * quaternion.Z + quaternion.W * quaternion.X), 1.0f - 2.0f * (quaternion.X * quaternion.X + quaternion.Y * quaternion.Y));
            Vector3 up = new Vector3(2.0f * (quaternion.X * quaternion.Y + quaternion.W * quaternion.Z), 1.0f - 2.0f * (quaternion.X * quaternion.X + quaternion.Z * quaternion.Z), 2.0f * (quaternion.Y * quaternion.Z - quaternion.W * quaternion.X));
            Vector3 right = new Vector3(1.0f - 2.0f * (quaternion.Y * quaternion.Y + quaternion.Z * quaternion.Z), 2.0f * (quaternion.X * quaternion.Y - quaternion.W * quaternion.Z), 2.0f * (quaternion.X * quaternion.Z + quaternion.W * quaternion.Y));

            return new Matrix4().InitRotationFromVectors(forward, up, right);
        }

        public static Vector3 GetForward(this Quaternion quaternion) {
            return new Vector3(0, 0, 1).Rotate(quaternion);
        }

        public static Quaternion ConjugateExt(this Quaternion quaternion) {
            return new Quaternion(-quaternion.X, -quaternion.Y, -quaternion.Z, quaternion.W);
        }

        public static float Dot(this Quaternion quaternion, Quaternion r) {
            return quaternion.X * r.X + quaternion.Y * r.Y + quaternion.Z * r.Z + quaternion.W * r.W;
        }

        public static Vector3 ToEuler(this Quaternion quaternion) {
            var eX = Math.Atan2(-2 * (quaternion.Y * quaternion.Z - quaternion.W * quaternion.X), quaternion.W * quaternion.W - quaternion.X * quaternion.X - quaternion.Y * quaternion.Y + quaternion.Z * quaternion.Z);
            var eY = Math.Asin(2 * (quaternion.X * quaternion.Z + quaternion.W * quaternion.Y));
            var eZ = Math.Atan2(-2 * (quaternion.X * quaternion.Y - quaternion.W * quaternion.Z), quaternion.W * quaternion.W + quaternion.X * quaternion.X - quaternion.Y * quaternion.Y - quaternion.Z * quaternion.Z);
            return new Vector3((float)eX, (float)eY, (float)eZ);
        }

        public static Quaternion FromEuler(this Quaternion quaternion, Vector3 euler) {
            var eX = euler.X;
            var eY = euler.Y;
            var eZ = euler.Z;

            var c1 = Math.Cos(eX / 2);
            var s1 = Math.Sin(eX / 2);
            var c2 = Math.Cos(eY / 2);
            var s2 = Math.Sin(eY / 2);
            var c3 = Math.Cos(eZ / 2);
            var s3 = Math.Sin(eZ / 2);


            var qw = c1 * c2 * c3 - s1 * s2 * s3;
            var qx = s1 * c2 * c3 + c1 * s2 * s3;
            var qy = c1 * s2 * c3 - s1 * c2 * s3;
            var qz = c1 * c2 * s3 + s1 * s2 * c3;

            return new Quaternion((float)qx, (float)qy, (float)qz, (float)qw);
        }

        public static Quaternion InvertPitch(this Quaternion quaternion) {
            Vector3 euler = quaternion.ToEuler();

            Vector3 invertedEuler = new Vector3(-euler.X, euler.Y, -euler.Z);

            return new Quaternion().FromEuler(invertedEuler);
        }
    }
}
