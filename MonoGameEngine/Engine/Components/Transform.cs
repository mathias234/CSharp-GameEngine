using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace MonoGameEngine.Engine.Components {
    public class Transform : Component {
        public Vector3 Position { get; set; }

        public Quaternion Rotation { get; set; }

        public Vector3 Scale = new Vector3(1, 1, 1);

        public Matrix WorldMatrix;

        public Transform() {
            WorldMatrix = new Matrix();
        }

        public Vector3 Forward() {
            Matrix rotationMatrix =
                Matrix.CreateRotationX(Rotation.X) *
                Matrix.CreateRotationY(Rotation.Y) *
                Matrix.CreateRotationZ(Rotation.Z);
            
            // Create a vector pointing the direction the camera is facing.
            Vector3 transformedReference = Vector3.Transform(Vector3.Forward, rotationMatrix);
            return transformedReference;
        }

        public Vector3 Left() {
            Matrix rotationMatrix =
                Matrix.CreateRotationX(Rotation.X) *
                Matrix.CreateRotationY(Rotation.Y) *
                Matrix.CreateRotationZ(Rotation.Z);

            // Create a vector pointing the direction the camera is facing.
            Vector3 transformedReference = Vector3.Transform(Vector3.Left, rotationMatrix);
            return transformedReference;
        }


        public override void Update(float deltaTime) {
            base.Update(deltaTime);

            if (GameObject.FindGameObjectOfType<Camera>() == null) {
                Debug.WriteLine("No Camera");
                return;
            }

            var rotationMatrix = Matrix.CreateFromQuaternion(Rotation);

            var scaleMatrix = Matrix.CreateScale(Scale);

            var translationMatrix = Matrix.CreateTranslation(Position);

            if (GameObject.parent == null)
                WorldMatrix = GameObject.FindGameObjectOfType<Camera>().WorldMatrix * translationMatrix * rotationMatrix * scaleMatrix;
            else
                WorldMatrix = GameObject.parent.Transform.WorldMatrix * GameObject.FindGameObjectOfType<Camera>().WorldMatrix * translationMatrix * rotationMatrix * scaleMatrix;
        }
    }
}
