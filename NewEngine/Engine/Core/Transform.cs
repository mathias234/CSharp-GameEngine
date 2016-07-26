using OpenTK;

namespace NewEngine.Engine.Core {
    public class Transform {
        private Vector3 _position;
        private Quaternion _rotation;
        private Vector3 _scale;

        private bool _hasChanged;
        private Transform _parent;
        private Matrix4 _parentMatrix;

        public Transform() {
            _position = new Vector3(0, 0, 0);
            _rotation = Quaternion.Identity;
            _scale = new Vector3(1, 1, 1);

            _parentMatrix = Matrix4.Identity;
        }

        public Vector3 Forward => Rotate(GetTransformedRotation(), new Vector3(0, 0, 1));

        public Vector3 Up => Rotate(GetTransformedRotation(), new Vector3(0, 1, 0));


        public Vector3 Right => Rotate(GetTransformedRotation(), new Vector3(1, 0, 0));

        public Vector3 Position {
            get { return _position; }
            set {
                _position = value;
                _hasChanged = true;
            }
        }

        public Quaternion Rotation {
            get { return _rotation; }
            set {
                _rotation = value;
                _hasChanged = true;
            }
        }

        public Vector3 Scale {
            get { return _scale; }
            set {
                _scale = value;
                _hasChanged = true;
            }
        }

        public Transform Parent {
            get { return _parent; }
            set {
                _parent = value;
                _hasChanged = true;
            }
        }

        public void Rotate(Vector3 axis, float angle) {
            _rotation = (Quaternion.FromAxisAngle(axis, angle)*_rotation).Normalized();
        }

        public void LookAt(Vector3 point, Vector3 up) {
            _rotation = GetLookAtDirection(point, up);
        }

        public Quaternion GetLookAtDirection(Vector3 point, Vector3 up) {
            return Matrix4.LookAt(Position, point, up).ExtractRotation();
        }


        public Matrix4 GetTransformation() {
            var translationMatrix = Matrix4.CreateTranslation(Position);
            var rotationMatrix = Matrix4.CreateFromQuaternion(Rotation);
            var scaleMatrix = Matrix4.CreateScale(_scale);


            // ORDER IS IMPORTANT
            return scaleMatrix*rotationMatrix*translationMatrix*GetParentMatrix();
        }


        public Matrix4 GetTransformationNoRot() {
            var translationMatrix = Matrix4.CreateTranslation(Position);
            var scaleMatrix = Matrix4.CreateScale(_scale);

            // ORDER IS IMPORTANT
            return scaleMatrix*translationMatrix*GetParentMatrix();
        }


        private Matrix4 GetParentMatrix() {
            if (_parent != null && _parent._hasChanged) {
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

            return parentRotation*Rotation;
        }

        public Vector3 Rotate(Quaternion rotation, Vector3 vecToRotate) {
            var conjugate = rotation;
            conjugate.Conjugate();

            var w = Util.Mul(rotation, vecToRotate)*conjugate;

            return new Vector3(w.X, w.Y, w.Z);
        }
    }
}