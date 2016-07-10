using NewEngine.Engine.Core;
using NewEngine.Engine.Rendering;
using OpenTK;
using OpenTK.Input;

namespace NewEngine.Engine.components {
    public class Camera : GameComponent {
        private Matrix4 _projection;

        public Camera(float fov, float aspect, float zNear, float zFar) {
            _projection = Matrix4.CreatePerspectiveFieldOfView(fov, aspect, zNear, zFar);

        }

        public Matrix4 GetViewProjection() {
            Vector3 cameraLookAt = Transform.GetTransformedPosition() + Transform.Forward;

            Matrix4 cameraMatrix = Matrix4.LookAt(Transform.GetTransformedPosition(), cameraLookAt, Vector3.UnitY);

            return cameraMatrix * _projection;
        }


        public override void AddToRenderingEngine(RenderingEngine renderingEngine) {
            renderingEngine.AddCamera(this);
        }

        public override void Update() {
            float movAmt = 0.2f;

            Vector3 input = new Vector3(0);

            if (Input.GetKey(Key.W))
                input += new Vector3(0, 0, 1);
            if (Input.GetKey(Key.S))
                input += new Vector3(0, 0, -1);

            if (Input.GetKey(Key.A))
                input += new Vector3(1, 0, 0);
            if (Input.GetKey(Key.D))
                input += new Vector3(-1, 0, 0);

            Vector3 move = (Transform.Forward * input.Z + Transform.Right * input.X) * movAmt;

            Transform.Position += move;

            if (Input.GetKeyDown(Key.Tab)) {
                rotateCamera = !rotateCamera;
                Input.LockMouse();
            }


            UpdateCameraRotation();
        }

        private static Vector3 Y_AXIS = new Vector3(0, 1, 0);
        private bool rotateCamera;
        private Vector2 lastMousePos;


        private void UpdateCameraRotation() {
            Vector2 deltaPos = Input.GetMousePosition() - lastMousePos;

            lastMousePos = Input.GetMousePosition();


            if (!rotateCamera)
                return;
            bool rotY = deltaPos.X != 0;
            bool rotX = deltaPos.Y != 0;

            if (rotY)
                Transform.Rotate(Y_AXIS, MathHelper.DegreesToRadians(-deltaPos.X * 0.5f));
            if (rotX)
                Transform.Rotate(Transform.Right, MathHelper.DegreesToRadians(deltaPos.Y * 0.5f));

        }

    }
}
