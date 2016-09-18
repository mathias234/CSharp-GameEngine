using System;
using NewEngine.Engine.Core;
using NewEngine.Engine.Rendering;
using NewEngine.Engine.Rendering.Shading;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace NewEngine.Engine.components {
    public class SpotLight : PointLight {
        public SpotLight(Vector3 color, float intensity, Attenuation attenuation, float viewAngle, int shadowMapSizeAsPowerOf2 = 0,
            float shadowSoftness = 1.0f, float lightBleedReductionAmount = 0.2f, float minVariance = 0.00002f)
            : base(color, intensity, attenuation) {

            Cutoff = (float)Math.Cos(viewAngle / 2);

            if (shadowMapSizeAsPowerOf2 != 0) {
                ShadowInfo = new ShadowInfo(Matrix4.CreatePerspectiveFieldOfView(viewAngle, 1.0f, 0.1f, Range), false, shadowMapSizeAsPowerOf2, shadowSoftness, lightBleedReductionAmount, minVariance);
                int shadowMapSize = 1 << (shadowMapSizeAsPowerOf2 + 1);
                ShadowInfo.ShadowMap = new Texture(IntPtr.Zero, shadowMapSize, shadowMapSize, TextureMinFilter.Linear, PixelInternalFormat.Rg32f, PixelFormat.Rgba, true);
                ShadowInfo.TempShadowMap = new Texture(IntPtr.Zero, shadowMapSize, shadowMapSize, TextureMinFilter.Linear, PixelInternalFormat.Rg32f, PixelFormat.Rgba, true);
            }
        }

        public float Cutoff { get; set; }

        public Vector3 Direction => Transform.Forward;
    }
}