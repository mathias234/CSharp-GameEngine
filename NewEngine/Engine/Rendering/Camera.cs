using System.Diagnostics;
using NewEngine.Engine.Core;
using OpenTK;
using OpenTK.Input;

namespace NewEngine.Engine.Rendering {
    public class Camera {
        private Vector3 _position;
        private Quaternion _rotation;

        private Matrix4 _projection;

        public Camera(float fov, float aspect, float zNear, float zFar) {
            _position = new Vector3(0, 0, 0);
            _rotation = Quaternion.Identity;
            _projection = Matrix4.CreatePerspectiveFieldOfView(fov, aspect, zNear, zFar);

            m_CameraTargetRot = Rotation;
        }

        public Matrix4 GetViewProjection() {
            Vector3 cameraLookAt = _position + Forward;
            Matrix4 cameraMatrix = Matrix4.LookAt(_position, cameraLookAt, Vector3.UnitY);

            return cameraMatrix * _projection;
        }


        public Vector3 Forward
        {
            get
            {
                return Util.VecMultiplyQuat(Rotation, Vector3.UnitZ);
            }
        }

        public Vector3 Left
        {
            get
            {
                return Util.VecMultiplyQuat(new Quaternion(0, Rotation.Y, 0, Rotation.W), Vector3.UnitX);
            }
        }

        public Vector3 Position
        {
            get { return _position; }
            set { _position = value; }
        }

        public Quaternion Rotation
        {
            get { return _rotation; }
            set { _rotation = value; }
        }

        private Vector2 _lastMousePos;
        private Vector2 _currMousePos;
        private bool _rotateCamera;

        public void Update(float deltaTime) {
            float movAmt = 0.2f;
            float rotAmt = 100;


            Vector3 input = new Vector3(0);

            if (Input.GetKey(Key.W))
                input += new Vector3(0,0,1);
            if (Input.GetKey(Key.S))
                input += new Vector3(0, 0, -1);

            if (Input.GetKey(Key.A))
                input += new Vector3(1, 0, 0);
            if (Input.GetKey(Key.D))
                input += new Vector3(-1, 0, 0);

            Vector3 move = (Forward * input.Z + Left * input.X) * movAmt;

            Position += move;


            if (Input.GetKeyDown(Key.Tab))
                _rotateCamera = !_rotateCamera;

            UpdateCameraRotation(deltaTime);
        }

        private Quaternion m_CameraTargetRot;

        private void UpdateCameraRotation(float deltaTime) {
            // NEEDS IMPROVEMENTS! kinda bad
            _lastMousePos = _currMousePos;
            _currMousePos = Input.GetMousePosition();

            var rotation = new Vector2(_currMousePos.X - _lastMousePos.X, _currMousePos.Y - _lastMousePos.Y) * 0.005f;

            m_CameraTargetRot *= Util.FromEulerAngles(-rotation.X, rotation.Y, 0f);

            Rotation = m_CameraTargetRot;
        }

    }
}
