using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGameEngine.Engine;
using MonoGameEngine.Engine.Components;

namespace MonoGameEngine.GameCode {
    // here you can place code that you do not want on a gameobject (sorta disconnected from the whole gameobject stuff)
    // or you can build your scene from here if you dont want to use the editor( or if i have not implemented and editor yet)
    public class GameCode : BaseGameCode {
        public override void Initialize() {
            // Scene should not be loaded from here i guess
            SceneManager.CreateNewScene("scene1.xml");
        }

        public override void Update(float deltaTime) {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back ==
                ButtonState.Pressed || Keyboard.GetState().IsKeyDown(
                Keys.Escape))
                CoreEngine.Quit();

            Matrix rotationMatrix =
                Matrix.CreateRotationX(GameObject.FindGameObjectOfType<Camera>().GameObject.Transform.Rotation.X) *
                Matrix.CreateRotationY(GameObject.FindGameObjectOfType<Camera>().GameObject.Transform.Rotation.Y) *
                Matrix.CreateRotationZ(GameObject.FindGameObjectOfType<Camera>().GameObject.Transform.Rotation.Z);

            // Create a vector pointing the direction the camera is facing.
            Vector3 transformedReference = Vector3.Transform(Vector3.Forward, rotationMatrix);

            if (Keyboard.GetState().IsKeyDown(Keys.Left)) {
                GameObject.FindGameObjectOfType<Camera>().Transform.Rotation += new Quaternion(0, 0.05f, 0, 0);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Right)) {
                GameObject.FindGameObjectOfType<Camera>().GameObject.Transform.Rotation -= new Quaternion(0, 0.05f, 0, 0);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Up)) {
                GameObject.FindGameObjectOfType<Camera>().GameObject.Transform.Position -= transformedReference;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Down)) {
                GameObject.FindGameObjectOfType<Camera>().GameObject.Transform.Position += transformedReference;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.LeftControl)) {
                GameObject.FindGameObjectOfType<Camera>().GameObject.Transform.Position -= new Vector3(0, 0.2f, 0);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.LeftShift)) {
                GameObject.FindGameObjectOfType<Camera>().GameObject.Transform.Position += new Vector3(0, 0.2f, 0);
            }
        }
    }
}
