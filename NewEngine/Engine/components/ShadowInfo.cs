using OpenTK;

namespace NewEngine.Engine.components {
    public class ShadowInfo {
        public ShadowInfo(Matrix4 projection, float bias, bool flipFaces) {
            Projection = projection;
            Bias = bias;
            FlipFaces = flipFaces;
        }

        public Matrix4 Projection { get; set; }

        public bool FlipFaces { get; }

        public float Bias { get; }
    }
}