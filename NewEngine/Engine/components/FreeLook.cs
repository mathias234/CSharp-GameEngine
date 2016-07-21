using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            bool rotY = deltaPos.X != 0;
            bool rotX = deltaPos.Y != 0;

            if (rotY)
                Transform.Rotate(-_yAxis, MathHelper.DegreesToRadians(deltaPos.X * 0.5f));
            if (rotX)
                Transform.Rotate(-Transform.Right, MathHelper.DegreesToRadians(deltaPos.Y * 0.5f));

        }
    }
}
