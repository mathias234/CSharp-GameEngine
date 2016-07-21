using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NewEngine.Engine.Rendering;
using NewEngine.Engine.Rendering.Shading;
using OpenTK;
using NewEngine.Engine.Core;

namespace NewEngine.Engine.components {
    /// Get Look At Direction is not working atm
    public class LookAtComponent : GameComponent {
        private RenderingEngine _renderingEngine;

        public override void Update(float deltaTime) {
            if (_renderingEngine != null) {
                Quaternion newRot = Transform.GetLookAtDirection(_renderingEngine.MainCamera.Transform.GetTransformedPosition(), Vector3.UnitY);

                Transform.Rotation = Transform.Rotation.Nlerp(newRot, deltaTime * 0.01f, false);
                LogManager.Debug(Transform.Rotation.ToString());
            }
        }

        public override void Render(Shader shader, RenderingEngine renderingEngine) {
            _renderingEngine = renderingEngine;
        }
    }
}
