using NewEngine.Engine.Core;
using NewEngine.Engine.Rendering;
using OpenTK;
using OpenTK.Input;

namespace NewEngine.Engine.components {
    public class Camera : GameComponent {
        private Matrix4 _projection;

        public Camera(Matrix4 projection) {
            _projection = projection; //Matrix4.CreatePerspectiveFieldOfView(fov, aspect, zNear, zFar);

        }

        public Matrix4 GetViewProjection() {
            Vector3 cameraLookAt = Transform.GetTransformedPosition() + Transform.Forward;

            Matrix4 cameraMatrix = Matrix4.LookAt(Transform.GetTransformedPosition(), cameraLookAt, Vector3.UnitY);

            return cameraMatrix * _projection;
        }

        public Matrix4 GetOrtographicProjection() {
            return Matrix4.CreateOrthographic(800, 600, 0.1f, 1000.0f);
        }


        public override void AddToEngine(CoreEngine engine) {
            engine.RenderingEngine.AddCamera(this);
        }
    }
}
