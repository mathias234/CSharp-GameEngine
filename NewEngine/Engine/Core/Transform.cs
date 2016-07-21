using OpenTK;

namespace NewEngine.Engine.Core {
    public class Transform {
        private Transform parent;
        private Matrix4 parentMatrix;

        private Vector3 _position;
        private Quaternion _rotation;
        private Vector3 _scale;

        private bool hasChanged;

        public Transform() {
            _position = new Vector3(0, 0, 0);
            _rotation = Quaternion.Identity;
            _scale = new Vector3(1, 1, 1);

            parentMatrix = Matrix4.Identity;
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
            var translationMatrix = Matrix4.CreateTranslation(Position);
            Matrix4 rotationMatrix = Matrix4.CreateFromQuaternion(Rotation);
            var scaleMatrix = Matrix4.CreateScale(_scale);



            // ORDER IS IMPORTANT
            return scaleMatrix * rotationMatrix * translationMatrix * GetParentMatrix();
        }


        public Matrix4 GetTransformationNoRot() {
            var translationMatrix = Matrix4.CreateTranslation(Position);
            var scaleMatrix = Matrix4.CreateScale(_scale);

            // ORDER IS IMPORTANT
            return scaleMatrix * translationMatrix * GetParentMatrix();
        }


        private Matrix4 GetParentMatrix() {
            if (parent != null && parent.hasChanged) {
                parentMatrix = parent.GetTransformation();
            }

            return parentMatrix;

        }

        public Vector3 GetTransformedPosition() {
            return Vector3.Transform(Position, GetParentMatrix());
        }

        public Quaternion GetTransformedRotation() {
            Quaternion parentRotation = new Quaternion(0, 0, 0, 1);
            if (parent != null) {
                parentRotation = parent.GetTransformedRotation();
            }

            return parentRotation * Rotation;
        }

        public Vector3 Forward
        {
            get
            {
                //var forward = new Vector3(2.0f * (Rotation.X * Rotation.Z - Rotation.W * Rotation.Y),
                //                    2.0f * (Rotation.Y * Rotation.Z + Rotation.W * Rotation.X),
                //                    1.0f - 2.0f * (Rotation.X * Rotation.Z - Rotation.W * Rotation.X));

                return Rotate(GetTransformedRotation(), new Vector3(0, 0, 1));
            }
        }

        public Vector3 Up
        {
            get
            {
                //var up = new Vector3(2.0f * (Rotation.X * Rotation.Y + Rotation.W * Rotation.Z),
                //    1.0f - 2.0f * (Rotation.X * Rotation.X + Rotation.Z * Rotation.Z),
                //    2.0f - (Rotation.Y * Rotation.Z - Rotation.W * Rotation.X));

                return Rotate(GetTransformedRotation(), new Vector3(0, 1, 0));

            }
        }


        public Vector3 Right
        {
            get
            {
                //var right = new Vector3(1.0f - 2.0f * (Rotation.Y * Rotation.Y + Rotation.Z * Rotation.Z),
                //                   2.0f * (Rotation.X * Rotation.Y - Rotation.W * Rotation.Z),
                //                   2.0f * (Rotation.X * Rotation.Z + Rotation.W * Rotation.Y));

                return Rotate(GetTransformedRotation(), new Vector3(1, 0, 0));
            }
        }

        public Vector3 Rotate(Quaternion rotation, Vector3 vecToRotate) {
            Quaternion conjugate = rotation;
            conjugate.Conjugate();

            Quaternion w = Util.Mul(rotation, vecToRotate)*conjugate;

            return new Vector3(w.X, w.Y, w.Z);
        }

        public Vector3 Position
        {
            get { return _position; }
            set
            {
                _position = value;
                hasChanged = true;
            }
        }

        public Quaternion Rotation
        {
            get { return _rotation; }
            set
            {
                _rotation = value;
                hasChanged = true;
            }
        }

        public Vector3 Scale
        {
            get { return _scale; }
            set
            {
                _scale = value;
                hasChanged = true;
            }
        }

        public Transform Parent
        {
            get { return parent; }
            set
            {
                parent = value;
                hasChanged = true;
            }
        }
    }
}
