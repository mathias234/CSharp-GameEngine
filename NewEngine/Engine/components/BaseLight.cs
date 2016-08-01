using NewEngine.Engine.Core;
using NewEngine.Engine.Rendering.Shading;
using OpenTK;

namespace NewEngine.Engine.components {
    public struct ShadowCameraTransform {
        public Vector3 pos;
        public Quaternion rot;
    }


    public class BaseLight : GameComponent {
        public BaseLight(Vector3 color, float intensity) {
            Intensity = intensity;
            Color = color;
            ShadowInfo = null;
        }

        public float Intensity { get; set; }

        public Vector3 Color { get; set; }

        public Shader Shader { get; set; }

        public ShadowInfo ShadowInfo { get; set; }

        public virtual ShadowCameraTransform CalcShadowCameraTransform(Transform transform) {
            ShadowCameraTransform result;
            result.pos = Transform.GetTransformedPosition();
            result.rot = Transform.GetTransformedRotation();
            return result;
        }

        public override void AddToEngine(CoreEngine engine) {
            engine.RenderingEngine.AddLight(this);
        }

        public override void OnDestroyed(CoreEngine engine) {
            engine.RenderingEngine.RemoveLight(this);
        }
    }
}