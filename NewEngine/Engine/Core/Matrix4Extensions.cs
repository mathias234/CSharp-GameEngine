using OpenTK;

namespace NewEngine.Engine.Core {
    public static class Matrix4Extensions {
        public static Matrix4 InitRotationFromVectors(this Matrix4 mat4, Vector3 n, Vector3 v, Vector3 u) {
            mat4[0, 0] = u.X; mat4[1, 0] = u.Y; mat4[2, 0] = u.Z; mat4[3, 0] = 0;
            mat4[0, 1] = v.X; mat4[1, 1] = v.Y; mat4[2, 1] = v.Z; mat4[3, 1] = 0;
            mat4[0, 2] = n.X; mat4[1, 2] = n.Y; mat4[2, 2] = n.Z; mat4[3, 2] = 0;
            mat4[0, 3] = 0; mat4[1, 3] = 0; mat4[2, 3] = 0; mat4[3, 3] = 1;

            return mat4;
        }
    }
}
