using NewEngine.Engine.Rendering;
using OpenTK;
using NewEngine.Engine.Core;

namespace NewEngine.Engine.components {
    /// Get Look At Direction is not working atm
    public class LookAtComponent : GameComponent {
        private RenderingEngine _renderingEngine;

        public override void Update(float deltaTime) {
            if (_renderingEngine == null) return;
            var newRot = Transform.GetLookAtDirection(_renderingEngine.MainCamera.Transform.GetTransformedPosition(), Vector3.UnitY);

            Transform.Rotation = Transform.Rotation.Nlerp(newRot, deltaTime * 0.01f, false);
            LogManager.Debug(Transform.Rotation.ToString());
        }

        public override void Render(string shader, string shaderType, float deltaTime, RenderingEngine renderingEngine, string renderStage) {
            _renderingEngine = renderingEngine;
        }
    }
}
