using OpenTK;
using NewEngine.Engine.Core;

namespace NewEngine.Engine.Core {
    public class Transform {
        private Vector3 _position;
        private Quaternion _rotation;
        private Vector3 _scale;

        private Transform _parent;
        private Matrix4 _parentMatrix;

        private string _name;

        public Transform() {
            _position = new Vector3(0, 0, 0);
            _rotation = Quaternion.Identity;
            _scale = new Vector3(1, 1, 1);

            _parentMatrix = Matrix4.Identity;
        }

        public Vector3 Forward => new Vector3(0,0,1).Rotate(GetTransformedRotation()); 

        public Vector3 Up => new Vector3(0, 1, 0).Rotate(GetTransformedRotation());

        public Vector3 Right => new Vector3(1, 0, 0).Rotate(GetTransformedRotation());

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
            }
        }

        public Vector3 Position
        {
            get { return _position; }
            set
            {
                _position = value;
            }
        }

        public Quaternion Rotation
        {
            get { return _rotation; }
            set
            {
                _rotation = value;
            }
        }

        public Vector3 Scale
        {
            get { return _scale; }
            set
            {
                _scale = value;
            }
        }

        public Transform Parent
        {
            get { return _parent; }
            set
            {
                _parent = value;
            }
        }

        public void Rotate(Vector3 axis, float angle) {
            _rotation = (Quaternion.FromAxisAngle(axis, angle) * _rotation).Normalized();
        }

        public void LookAt(Vector3 point, Vector3 up) {
            _rotation = GetLookAtDirection(point, up);
        }

        public Quaternion GetLookAtDirection(Vector3 point, Vector3 up) {
            return Matrix4.LookAt(Position, point, up).ExtractRotation();
        }


        public Matrix4 GetTransformation() {
            var translationMatrix = Matrix4.CreateTranslation(_position);
            var rotationMatrix = Matrix4.CreateFromQuaternion(_rotation);
            var scaleMatrix = Matrix4.CreateScale(_scale);

            // ORDER IS IMPORTANT
            var result = rotationMatrix * scaleMatrix * translationMatrix;

            return GetParentMatrix() * result;
        }


        public Matrix4 GetTransformationNoRot() {
            var translationMatrix = Matrix4.CreateTranslation(Position);
            var scaleMatrix = Matrix4.CreateScale(_scale);

            var result = translationMatrix * scaleMatrix;

            // ORDER IS IMPORTANT
            return GetParentMatrix() * result;
        }


        private Matrix4 GetParentMatrix() {
            if (_parent != null) {
                _parentMatrix = _parent.GetTransformation();
            }

            return _parentMatrix;
        }

        public Vector3 GetTransformedPosition() {
            return Vector3.Transform(Position, GetParentMatrix());
        }

        public Quaternion GetTransformedRotation() {
            var parentRotation = new Quaternion(0, 0, 0, 1);
            if (_parent != null) {
                parentRotation = _parent.GetTransformedRotation();
            }

            return parentRotation * Rotation;
        }
    }
}