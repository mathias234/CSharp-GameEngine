using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace MonoGameEngine.Components {
    public class Camera : Component {
        public static Camera Main => GameObject.FindGameObjectOfType<Camera>();

        private Vector3 _camTarget;

        public Matrix ProjectionMatrix { get; set; }
        public Matrix ViewMatrix { get; set; }
        public Matrix WorldMatrix { get; set; }

        public Camera() {

        }

        public override void Init() {
            base.Init();
            _camTarget = new Vector3(0f, 0f, 0f);

            ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(
                               MathHelper.ToRadians(45f),
                               CoreEngine.instance.GraphicsDevice.DisplayMode.AspectRatio, 1f, 1000f);

            WorldMatrix = Matrix.CreateWorld(_camTarget, Vector3.
                          Forward, Vector3.Up);


            var rotationMatrix =
                Matrix.CreateRotationX(GameObject.Transform.Rotation.X) *
                Matrix.CreateRotationY(GameObject.Transform.Rotation.Y) *
                Matrix.CreateRotationZ(GameObject.Transform.Rotation.Z);

            // Create a vector pointing the direction the camera is facing.
            var transformedReference = Vector3.Transform(Vector3.Forward, rotationMatrix);

            // Calculate the position the camera is looking at.
            var cameraLookat = transformedReference;

            ViewMatrix = Matrix.CreateLookAt(GameObject.Transform.Position, cameraLookat, Vector3.Up);
        }


        public override void Update(float deltaTime) {
            base.Update(deltaTime);

            Matrix rotationMatrix =
                Matrix.CreateRotationX(GameObject.Transform.Rotation.X) *
                Matrix.CreateRotationY(GameObject.Transform.Rotation.Y) *
                Matrix.CreateRotationZ(GameObject.Transform.Rotation.Z);

            // Create a vector pointing the direction the camera is facing.
            Vector3 transformedReference = Vector3.Transform(new Vector3(0,0,1), rotationMatrix);

            // Calculate the position the camera is looking at.
            Vector3 cameraLookat = GameObject.Transform.Position + transformedReference;

            ViewMatrix = Matrix.CreateLookAt(GameObject.Transform.Position, cameraLookat, Vector3.Up);
        }

        public Vector2 MouseToWorldPos() {
            Vector2 mousePos = Mouse.GetState().Position.ToVector2();

            return Vector2.Transform(mousePos, Matrix.Invert(ViewMatrix));
        }
    }
}
