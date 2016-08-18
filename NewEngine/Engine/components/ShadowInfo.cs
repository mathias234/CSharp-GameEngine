using NewEngine.Engine.Rendering;
using OpenTK;

namespace NewEngine.Engine.components {
    public class ShadowInfo {
        public ShadowInfo(Matrix4 projection, bool flipFaces, int shadowMapSizeAsPowerOf2, float shadowSoftness = 1.0f, float lightBleedReductionAmount = 0.2f, float minVariance = 0.00002f) {
            Projection = projection;
            FlipFaces = flipFaces;
            ShadowMapSizeAsPowerOf2 = shadowMapSizeAsPowerOf2;
            LightBleedReductionAmount = lightBleedReductionAmount;
            ShadowSoftness = shadowSoftness;
            MinVariance = minVariance;
        }

        public Matrix4 Projection { get; set; }

        public bool FlipFaces { get; }

        public float ShadowSoftness { get; set; }
        public float LightBleedReductionAmount { get; set; }
        public float MinVariance { get; set; }
        public Texture ShadowMap { get; set; }
        public Texture TempShadowMap { get; set; }
        public int ShadowMapSizeAsPowerOf2;
    }
}