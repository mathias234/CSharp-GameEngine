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
using MonoGameEngine.Engine.Components;
using MonoGameEngine.Engine.Physics;
using MonoGameEngine.Engine.UI;

namespace MonoGameEngine.Engine {
    public class EditorCode : BaseGameCode {
        private Texture2D checkerboard;
        private Vector2 _lastMousePos;
        private Vector2 _currMousePos;

        private GameObject _editorUi = new GameObject(Vector3.Zero);
        private GameObject _gameObjectHierarchyParent;

        public override void Initialize() {
            _gameObjectHierarchyParent = new GameObject(new Vector3(0));
            _gameObjectHierarchyParent.Instantiate();
            SceneManager.CreateNewScene("scene1.scene");
            checkerboard = CoreEngine.instance.Content.Load<Texture2D>("checkerboard");
            SetupEditorView();
        }

        public override void Update(float deltaTime) {
            UpdateEditorCam(deltaTime);
            UpdateGameObjectHierarchy();
        }

        public void UpdateGameObjectHierarchy() {
            // TODO: i need to be able to add / remove components after instantiate
            _gameObjectHierarchyParent.ClearAllComponents();
            CoreEngine.instance.DestoryGameObject(_gameObjectHierarchyParent);
            _gameObjectHierarchyParent = new GameObject(new Vector3(0));

            var x = 0;
            foreach (var gameObject in CoreEngine.instance.GameObjects) {
                var text =
                    new UiTextComponent(
                        new Rectangle(CoreEngine.instance.GraphicsDevice.Viewport.Width - 65, 30 * x, 0, 0),
                        gameObject.name, Color.LightGray, new Color(0, 0, 0, 0), true,
                        () => {
                            Debug.WriteLine("Selected an object");
                        });

                _gameObjectHierarchyParent.AddComponent(text);
                x++;
            }

            _gameObjectHierarchyParent.Instantiate();
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
            GameObject EditorUI = new GameObject(Vector3.Zero);


            var leftBox = new UiTextureComponent(
                new Rectangle(0, 0, 70, CoreEngine.instance.GraphicsDevice.Viewport.Height), null, Color.DimGray);

            EditorUI.AddComponent(leftBox);

            var createStandardCube = new UiTextureComponent(new Rectangle(10, 60, 50, 50), checkerboard, Color.White, false,
                () => {
                    var camPos = Camera.Main.GameObject.Transform.Position;
                    // place the cube infront of the player
                    var sampleCube =
                        new GameObject(new Vector3(camPos.X, camPos.Y, camPos.Z) +
                                       (-Camera.Main.GameObject.Transform.Forward() * 10));
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
                });

            EditorUI.AddComponent(createStandardCube);

            var save = new UiTextComponent(new Rectangle(0, 0, 30, 30), "Save", Color.LightGray, Color.Gray, false,
                () => {
                    SceneManager.SaveScene("scene1.scene");
                });

            EditorUI.AddComponent(save);


            var newScene = new UiTextComponent(new Rectangle(0, 33, 30, 30), "New", Color.LightGray, Color.Gray, false,
                () => {
                    SceneManager.CreateNewScene("scene1.scene");
                });
            EditorUI.AddComponent(newScene);


            var gameObjectHierarchyBackground =
                new UiTextureComponent(
                    new Rectangle(CoreEngine.instance.GraphicsDevice.Viewport.Width - 70, 0, 70, 200), null,
                    Color.DimGray);

            EditorUI.AddComponent(gameObjectHierarchyBackground);



            EditorUI.Instantiate();
        }
    }
}
