using NewEngine.Engine.Core;
using OpenTK;

namespace NewEngine.Engine.components {
    public class Camera : GameComponent {
        private Matrix4 _projection;

        public Camera(Matrix4 projection) {
            _projection = projection;

        }

        public Matrix4 SetProjection {
            set { _projection = value; } 
        }

        public Matrix4 GetViewProjection() {
            Matrix4 cameraRotation = Transform.GetTransformedRotation().ConjugateExt().ToRotationMatrix();
            Matrix4 cameraTranslation = Matrix4.CreateTranslation(Transform.GetTransformedPosition() * -1);

            return cameraTranslation * cameraRotation *  _projection;
        }

        public Matrix4 GetOrtographicProjection() {
            return Matrix4.CreateOrthographic(CoreEngine.GetWidth(), CoreEngine.GetHeight(), 0.1f, 1000.0f);
        }


        public override void AddToEngine(CoreEngine engine) {
            engine.RenderingEngine.AddCamera(this);
        }
    }
}
