using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK;

namespace NewEngine.Engine.Core {
    public class Util {
        public static string[] RemoveEmptyStrings(string[] array) {
            return array.Where(t => t != string.Empty).ToArray();
        }

        public static int[] FromNullableIntArray(int?[] nullableArray) {
            var array = new int[nullableArray.Length];
            for (var i = 0; i < array.Length; i++) {
                var nullable = nullableArray[i];

                if (nullable != null) array[i] = nullable.Value;
            }

            return array;
        }

        public static Quaternion FromEulerAngles(float yaw, float pitch, float roll) {
            var num1 = roll*0.5f;
            var num2 = pitch*0.5f;
            var num3 = yaw*0.5f;
            var num4 = (float) Math.Sin(num1);
            var num5 = (float) Math.Cos(num1);
            var num6 = (float) Math.Sin(num2);
            var num7 = (float) Math.Cos(num2);
            var num8 = (float) Math.Sin(num3);
            var num9 = (float) Math.Cos(num3);

            return new Quaternion((float) (num9*(double) num6*num5 + num8*(double) num7*num4),
                (float) (num8*(double) num7*num5 - num9*(double) num6*num4),
                (float) (num9*(double) num7*num4 - num8*(double) num6*num5),
                (float) (num9*(double) num7*num5 + num8*(double) num6*num4));
        }

        public static Vector3 ToEulerAngles(Quaternion q) {
            double yaw;
            double pitch;
            double roll;

            double w2 = q.W*q.W;
            double x2 = q.X*q.X;
            double y2 = q.Y*q.Y;
            double z2 = q.Z*q.Z;

            var unitLength = w2 + x2 + y2 + z2; // Normalised == 1, otherwise correction divisor.
            double abcd = q.W*q.X + q.Y*q.Z;
            var eps = 1e-7; // TODO: pick from your math lib instead of hardcoding.
            var pi = Math.PI; // TODO: pick from your math lib instead of hardcoding.

            if (abcd > (0.5 - eps)*unitLength) {
                yaw = 2*Math.Atan2(q.Y, q.W);
                pitch = pi;
                roll = 0;
            }
            else if (abcd < (-0.5 + eps)*unitLength) {
                yaw = -2*Math.Atan2(q.Y, q.W);
                pitch = -pi;
                roll = 0;
            }
            else {
                double adbc = q.W*q.Z - q.X*q.Y;
                double acbd = q.W*q.Y - q.X*q.Z;
                yaw = Math.Atan2(2*adbc, 1 - 2*(z2 + x2));
                pitch = Math.Asin(2*abcd/unitLength);
                roll = Math.Atan2(2*acbd, 1 - 2*(y2 + x2));
            }

            return new Vector3((float) yaw, (float) pitch, (float) roll);
        }

        public static float MaxOfVector3(Vector3 vector3) {
            return Math.Max(vector3.X, Math.Max(vector3.Y, vector3.Z));
        }


        public static Vector3 VecMultiplyQuat(Quaternion rotation, Vector3 point) {
            var num1 = rotation.X*2f;
            var num2 = rotation.Y*2f;
            var num3 = rotation.Z*2f;
            var num4 = rotation.X*num1;
            var num5 = rotation.Y*num2;
            var num6 = rotation.Z*num3;
            var num7 = rotation.X*num2;
            var num8 = rotation.X*num3;
            var num9 = rotation.Y*num3;
            var num10 = rotation.W*num1;
            var num11 = rotation.W*num2;
            var num12 = rotation.W*num3;
            Vector3 vector3;
            vector3.X =
                (float)
                    ((1.0 - (num5 + (double) num6))*point.X + (num7 - (double) num12)*point.Y +
                     (num8 + (double) num11)*point.Z);
            vector3.Y =
                (float)
                    ((num7 + (double) num12)*point.X + (1.0 - (num4 + (double) num6))*point.Y +
                     (num9 - (double) num10)*point.Z);
            vector3.Z =
                (float)
                    ((num8 - (double) num11)*point.X + (num9 + (double) num10)*point.Y +
                     (1.0 - (num4 + (double) num5))*point.Z);
            return vector3;
        }

        public static Quaternion Mul(Quaternion baseQuat, Vector3 r) {
            var w = -baseQuat.X*r.X - baseQuat.Y*r.Y - baseQuat.Z*r.Z;
            var x = baseQuat.W*r.X + baseQuat.Y*r.Z - baseQuat.Z*r.Y;
            var y = baseQuat.W*r.Y + baseQuat.Z*r.X - baseQuat.X*r.Z;
            var z = baseQuat.W*r.Z + baseQuat.X*r.Y - baseQuat.Y*r.X;

            return new Quaternion(x, y, z, w);
        }
    }
}