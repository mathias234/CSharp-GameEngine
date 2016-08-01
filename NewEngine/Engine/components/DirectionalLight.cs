using System;
using NewEngine.Engine.Core;
using NewEngine.Engine.Rendering.Shading;
using OpenTK;

namespace NewEngine.Engine.components {
    public class DirectionalLight : BaseLight {
        private float _halfShadowArea;

        public DirectionalLight(Vector3 color, float intensity, int shadowMapSizeAsPowerOf2 = 0, float shadowArea = 140,
            float shadowSoftness = 1.0f, float lightBleedReductionAmount = 0.2f, float minVariance = 0.00002f) : base(color, intensity) {

            Shader = new Shader("forward-DirectionalLight");

            _halfShadowArea = shadowArea / 2.0f;

            if (shadowMapSizeAsPowerOf2 != 0)
                ShadowInfo = new ShadowInfo(Matrix4.CreateOrthographicOffCenter(-_halfShadowArea, _halfShadowArea, -_halfShadowArea, _halfShadowArea, -_halfShadowArea, _halfShadowArea),
                    false, shadowMapSizeAsPowerOf2, shadowSoftness, lightBleedReductionAmount, minVariance);
        }

        //public override ShadowCameraTransform CalcShadowCameraTransform(Transform transform) {
        //    ShadowCameraTransform result;
        //    result.pos = transform.Position + transform.Forward * _halfShadowArea;
        //    result.rot = Transform.GetTransformedRotation();

        //    var worldTexelSize = (_halfShadowArea * 2) / (1 << ShadowInfo.ShadowMapSizeAsPowerOf2);

        //    Vector3 lightSpaceCameraPos = result.pos.Rotate(result.rot.ConjugateExt());

        //    lightSpaceCameraPos.X = worldTexelSize * (float)Math.Floor(lightSpaceCameraPos.X / worldTexelSize);
        //    lightSpaceCameraPos.Y = worldTexelSize * (float)Math.Floor(lightSpaceCameraPos.Y / worldTexelSize);

        //    result.pos = lightSpaceCameraPos.Rotate(result.rot);



        //    return result;
        //}

        public Vector3 Direction => Transform.Forward;
    }
}
