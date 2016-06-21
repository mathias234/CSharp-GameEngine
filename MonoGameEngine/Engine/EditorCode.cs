using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameEngine.Components;
using MonoGameEngine.Engine.Components;
using MonoGameEngine.Engine.UI;

namespace MonoGameEngine.Engine {
    public class EditorCode : BaseGameCode {
        private Texture2D checkerboard;
        private Vector2 _lastMousePos;
        private Vector2 _currMousePos;

        // the position of the selected gameobject in the GameObjects List
        private int _selectedGameObject;

        public override void Initialize() {
            SceneManager.LoadScene("scene1.scene");
            checkerboard = CoreEngine.instance.Content.Load<Texture2D>("checkerboard");
            SetupEditorView();
        }

        public override void Update(float deltaTime) {
            UpdateEditorCam(deltaTime);

            int x = 0;
            foreach (var gameObject in CoreEngine.instance.GameObjects) {
                var x1 = x;
                UISystem.CreateUI(new UITextElement(new Rectangle(CoreEngine.instance.GraphicsDevice.Viewport.Width - 65, 30 * x, 0, 0), gameObject.name, Color.LightGray, new Color(0, 0, 0, 0), true,
                    () => {
                        _selectedGameObject = x1;
                    }));
                x++;
            }

            UpdateSelectedGameObject();
        }


        private int lastSelectedObject;

        private void UpdateSelectedGameObject() {
            if (lastSelectedObject != _selectedGameObject) {
                if (CoreEngine.instance.GameObjects[lastSelectedObject] != null && CoreEngine.instance.GameObjects[lastSelectedObject].GetComponent<MeshRenderer>() != null)
                    CoreEngine.instance.GameObjects[lastSelectedObject].GetComponent<MeshRenderer>().Color = Color.White;
            }

            lastSelectedObject = _selectedGameObject;
        }

        private void UpdateEditorCam(float deltaTime) {
            if (Camera.Main == null)
                return;
            GameObject camera = Camera.Main.GameObject;
            

            if (Keyboard.GetState().IsKeyDown(Keys.W)) {
                camera.Transform.Position -= camera.Transform.Forward();
            }
            if (Keyboard.GetState().IsKeyDown(Keys.S)) {
                camera.Transform.Position += camera.Transform.Forward();
            }
            if (Keyboard.GetState().IsKeyDown(Keys.A)) {
                camera.Transform.Position -= camera.Transform.Left();
            }
            if (Keyboard.GetState().IsKeyDown(Keys.D)) {
                camera.Transform.Position += camera.Transform.Left();
            }

            UpdateCameraRotation(camera);
        }

        private void UpdateCameraRotation(GameObject camera) {
            _lastMousePos = _currMousePos;
            _currMousePos = Mouse.GetState().Position.ToVector2();
            var cameraTargetRot = camera.Transform.Rotation;
            var rotation = new Vector2(_currMousePos.X - _lastMousePos.X, _currMousePos.Y - _lastMousePos.Y) * 0.01f;

            if (Mouse.GetState().RightButton == ButtonState.Pressed) {
                cameraTargetRot += Quaternion.CreateFromYawPitchRoll(-rotation.X, rotation.Y, 0);
                camera.Transform.Rotation = cameraTargetRot;
            }
        }

        private void SetupEditorView() {
            UISystem.CreateUI(new UITextureElement(new Rectangle(0, 0, 70, CoreEngine.instance.GraphicsDevice.Viewport.Height), null, Color.DimGray));
            UISystem.CreateUI(new UITextureElement(new Rectangle(10, 60, 50, 50), checkerboard, Color.White, false, () => {
                var camPos = Camera.Main.GameObject.Transform.Position;
                    // place the cube infront of the player
                    var sampleCube = new GameObject(new Vector3(camPos.X, camPos.Y, camPos.Z) + (-Camera.Main.GameObject.Transform.Forward() * 10));
                    sampleCube.AddComponent<MeshRenderer>();
                    sampleCube.GetComponent<MeshRenderer>().Mesh = Primitives.CreateCube();
                    sampleCube.GetComponent<MeshRenderer>().Color = Color.LightGray;
                    sampleCube.AddComponent<BoxCollider>();
                    sampleCube.GetComponent<BoxCollider>().Height = 2;
                    sampleCube.GetComponent<BoxCollider>().Width = 2;
                    sampleCube.GetComponent<BoxCollider>().Length = 2;
                    sampleCube.GetComponent<BoxCollider>().IsStatic = false;
                    sampleCube.name = "cube";
                    sampleCube.Instantiate();
            }));

            UISystem.CreateUI(new UITextElement(new Rectangle(0, 0, 30, 30), "Save", Color.LightGray, Color.Gray, false,
                () => {
                    SceneManager.SaveScene("scene1.scene");
                }));


            UISystem.CreateUI(new UITextElement(new Rectangle(0, 33, 30, 30), "New", Color.LightGray, Color.Gray, false,
                () => {
                    SceneManager.CreateNewScene("scene1.scene");
                }));


            UISystem.CreateUI(new UITextureElement(new Rectangle(CoreEngine.instance.GraphicsDevice.Viewport.Width - 70, 0, 70, 200), null, Color.DimGray));
        }
    }
}
