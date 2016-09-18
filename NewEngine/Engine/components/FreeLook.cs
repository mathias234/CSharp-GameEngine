using System;
using NewEngine.Engine.Core;
using OpenTK;
using OpenTK.Input;

namespace NewEngine.Engine.components {
    public class FreeLook : GameComponent {
        private readonly bool _useX;
        private readonly bool _useY;
        private static Vector3 _yAxis = new Vector3(0, 1, 0);
        private bool _rotateCamera;
        private Vector2 _lastMousePos;

        public FreeLook(bool useX, bool useY) {
            _useX = useX;
            _useY = useY;
        }

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

            if (rotY && _useY)
                Transform.Rotate(-_yAxis, MathHelper.DegreesToRadians(deltaPos.X * 0.5f));
            if (rotX && _useX)
                Transform.Rotate(-Transform.Right, MathHelper.DegreesToRadians(deltaPos.Y * 0.5f));

        }
    }
}
