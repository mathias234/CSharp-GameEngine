using System;
using NewEngine.Engine.Core;
using OpenTK;
using OpenTK.Input;

namespace NewEngine.Engine.components {
    public class FreeLook : GameComponent {
        private static Vector3 _yAxis = new Vector3(0, 1, 0);
        private bool _rotateCamera;
        private Vector2 _lastMousePos;

        public override void Update(float deltaTime) {
            if (Input.GetKeyDown(Key.Tab)) {
                _rotateCamera = !_rotateCamera;
                Input.LockMouse();
            }


            UpdateCameraRotation();
        }

        private void UpdateCameraRotation() {
            Vector2 deltaPos = Input.GetMousePosition() - _lastMousePos;

            _lastMousePos = Input.GetMousePosition();


            if (!_rotateCamera)
                return;
            bool rotY = Math.Abs(deltaPos.X) > 0.02f;
            bool rotX = Math.Abs(deltaPos.Y) > 0.02f;

            if (rotY)
                Transform.Rotate(-_yAxis, MathHelper.DegreesToRadians(deltaPos.X * 0.5f));
            if (rotX)
                Transform.Rotate(-Transform.Right, MathHelper.DegreesToRadians(deltaPos.Y * 0.5f));

        }
    }
}
