using NewEngine.Engine.Core;
using NewEngine.Engine.Rendering;
using OpenTK;

namespace NewEngine.Engine.components {
    public class Camera : GameComponent {
        public Matrix4 Projection { get; set; }

        public Camera(Matrix4 projection) {
            Projection = projection;

        }


        public Matrix4 GetView()
        {
            Matrix4 cameraRotation = Transform.GetTransformedRotation().ConjugateExt().ToRotationMatrix();
            Matrix4 cameraTranslation = Matrix4.CreateTranslation(Transform.GetTransformedPosition() * -1);

            return cameraTranslation * cameraRotation;
        }


        public Matrix4 GetViewProjection() {
            Matrix4 cameraRotation = Transform.GetTransformedRotation().ConjugateExt().ToRotationMatrix();
            Matrix4 cameraTranslation = Matrix4.CreateTranslation(Transform.GetTransformedPosition() * -1);

            return cameraTranslation * cameraRotation *  Projection;
        }

        public Matrix4 GetOrtographicProjection() {
            return Matrix4.CreateOrthographic(RenderingEngine.GetWidth(), RenderingEngine.GetHeight(), 0.1f, 1000.0f);
        }

        public override void AddToEngine(ICoreEngine engine) {
            engine.RenderingEngine.AddCamera(this);
        }
    }
}
