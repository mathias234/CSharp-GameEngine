using OpenTK;

namespace NewEngine.Engine.Core {
    public class Transform {
        private Vector3 _position;
        private Quaternion _rotation;
        private Vector3 _scale;

        public Transform() {
            _position = new Vector3(0, 0, 0);
            _rotation = Quaternion.Identity;
            _scale = new Vector3(1, 1, 1);
        }

        public Matrix4 GetTransformation() {
            var translationMatrix = Matrix4.CreateTranslation(Position);
            Matrix4 rotationMatrix = Matrix4.CreateFromQuaternion(Rotation);

            var scaleMatrix = Matrix4.CreateScale(_scale);

            return translationMatrix * rotationMatrix * scaleMatrix;
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

        public Vector3 Scale
        {
            get { return _scale; }
            set { _scale = value; }
        }
    }
}
