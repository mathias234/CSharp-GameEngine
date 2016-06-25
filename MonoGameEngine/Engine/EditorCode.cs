using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameEngine.Engine.Components;
using MonoGameEngine.Engine.Components.UI;
using MonoGameEngine.Engine.Physics;
using MonoGameEngine.Engine.UI;

namespace MonoGameEngine.Engine {
    public class EditorCode : BaseGameCode {
        private Texture2D checkerboard;
        private Vector2 _lastMousePos;
        private Vector2 _currMousePos;

        private GameObject _gameObjectHierarchyParent;
        private GameObject _gameObjectInspectorParent;
        private GameObject _editorUi;

        // can be an int as it should never change type
        private int _selectedGameObject;

        public override void Initialize() {
            SceneManager.CreateNewScene("scene1.scene");

            GameObject.SubscribeToOnGameObjectInstantiated(OnGameObjectInstantiated);

            _gameObjectHierarchyParent = new GameObject(new Vector3(0));
            _gameObjectInspectorParent = new GameObject(new Vector3(0));
            _editorUi = new GameObject(new Vector3(0));

            _gameObjectHierarchyParent.Instantiate();
            _gameObjectInspectorParent.Instantiate();
            _editorUi.Instantiate();


            checkerboard = CoreEngine.instance.Content.Load<Texture2D>("checkerboard");
            SetupEditorView();
        }

        public override void Update(float deltaTime) {
            UpdateEditorCam(deltaTime);
        }

        public void OnGameObjectInstantiated(GameObject gObj) {
          // UpdateGameObjectHierarchy();
          //  UpdateGameObjectInspector();
        }

        public void UpdateGameObjectHierarchy() {
            // VERY NOT OPTIMIZED: TODO: NEEDS OPTIMIZATION SOON!
            _gameObjectHierarchyParent.ClearAllComponents();

            var gameObjectHierarchyBackground = _gameObjectHierarchyParent.AddComponent<UiTextureComponent>();
            gameObjectHierarchyBackground.Rect = new Rectangle(CoreEngine.instance.GraphicsDevice.Viewport.Width - 200, 0, 200, CoreEngine.instance.GraphicsDevice.Viewport.Height / 2);
            gameObjectHierarchyBackground.Color = Color.DimGray;

            var gameObjectHierarchyMask = _gameObjectHierarchyParent.AddComponent<UIMask>();
            gameObjectHierarchyMask.Rect = new Rectangle(CoreEngine.instance.GraphicsDevice.Viewport.Width - 200, 0, 200, CoreEngine.instance.GraphicsDevice.Viewport.Height / 2);
            gameObjectHierarchyMask.Color = Color.DimGray;

            var gameObjectInspectorHierarchySplitter = _editorUi.AddComponent<UiTextureComponent>();
            gameObjectInspectorHierarchySplitter.Rect = new Rectangle(CoreEngine.instance.GraphicsDevice.Viewport.Width - 200, CoreEngine.instance.GraphicsDevice.Viewport.Height / 2 - 30, 200, 12);
            gameObjectInspectorHierarchySplitter.Color = Color.DarkGray;



            var x = 0;
            foreach (var gameObject in CoreEngine.instance.GameObjects) {
                var text = _gameObjectHierarchyParent.AddComponent<UiTextComponent>();
                text.Rect = new Rectangle(CoreEngine.instance.GraphicsDevice.Viewport.Width - 195, 30 * x, 30, 30);
                text.Text = gameObject.name ?? "No Name";
                text.Color = Color.LightGray;
                text.BackGroundColor = new Color(0, 0, 0, 0);
                var x1 = x;
                text.OnClicked = () => {
                    Debug.WriteLine("Selected an object");
                    _selectedGameObject = x1;
                };

                x++;
            }
        }

        public void UpdateGameObjectInspector() {
            _gameObjectInspectorParent.ClearAllComponents();

            var gameObjectInspectorBackground = _gameObjectInspectorParent.AddComponent<UiTextureComponent>();
            gameObjectInspectorBackground.Rect = new Rectangle(CoreEngine.instance.GraphicsDevice.Viewport.Width - 200,
                CoreEngine.instance.GraphicsDevice.Viewport.Height/2, 200,
                CoreEngine.instance.GraphicsDevice.Viewport.Height/2);
            gameObjectInspectorBackground.Color = Color.DimGray;

            var gameObjectInspectorMask = _gameObjectInspectorParent.AddComponent<UIMask>();
            gameObjectInspectorMask.Rect = new Rectangle(CoreEngine.instance.GraphicsDevice.Viewport.Width - 200,
                CoreEngine.instance.GraphicsDevice.Viewport.Height/2, 200,
                CoreEngine.instance.GraphicsDevice.Viewport.Height/2);
            gameObjectInspectorMask.Color = Color.DimGray;

            var selectedObjectTemp = CoreEngine.instance.GameObjects[_selectedGameObject];

            if (selectedObjectTemp == null)
                return;

            int x = 0;
            foreach (var component in selectedObjectTemp._components) {
                var componentTitle = _gameObjectInspectorParent.AddComponent<UiTextComponent>();
                componentTitle.Rect = new Rectangle(CoreEngine.instance.GraphicsDevice.Viewport.Width - 200,
                                        CoreEngine.instance.GraphicsDevice.Viewport.Height / 2 + (x*30), 200,
                                        20 );


                componentTitle.Text = component.GetType().Name;
                componentTitle.Color = Color.LightGray;
                componentTitle.BackGroundColor = Color.Gray;

                x++;

                foreach (var fieldInfo in component.GetType().GetFields()) {
                    var field = _gameObjectInspectorParent.AddComponent<UiTextComponent>();
                    field.Rect = new Rectangle(CoreEngine.instance.GraphicsDevice.Viewport.Width - 200 + 5,
                                            CoreEngine.instance.GraphicsDevice.Viewport.Height / 2 + (x * 30), 200,
                                            20);


                    field.Text = fieldInfo.Name + " (type: " + fieldInfo.FieldType.Name + ")";
                    field.Color = Color.LightGray;
                    field.BackGroundColor = Color.Gray;
                    x++;
                    var fieldData = _gameObjectInspectorParent.AddComponent<UiTextComponent>();
                    fieldData.Rect = new Rectangle(CoreEngine.instance.GraphicsDevice.Viewport.Width - 200 + 5,
                                            CoreEngine.instance.GraphicsDevice.Viewport.Height / 2 + (x * 30), 200,
                                            20);


                    fieldData.Text = fieldInfo.GetValue(component).ToString();
                    fieldData.Color = Color.LightGray;
                    fieldData.BackGroundColor = new Color(0,0,0,0);
                    x++;
                }
            }

            Debug.WriteLine(selectedObjectTemp.name);

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
          /*  var leftBox = _editorUi.AddComponent<UiTextureComponent>();
            leftBox.Rect = new Rectangle(0, 0, 70, CoreEngine.instance.GraphicsDevice.Viewport.Height);
            leftBox.Color = Color.DimGray;

            var createStandardCube = _editorUi.AddComponent<UiTextureComponent>();
            createStandardCube.Rect = new Rectangle(10, 60, 50, 50);
            createStandardCube.Texture2D = checkerboard;
            createStandardCube.Color = Color.White;
            createStandardCube.OnClicked = () => {
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
            };


            var save = _editorUi.AddComponent<UiTextComponent>();
            save.Rect = new Rectangle(0, 0, 30, 30);
            save.Text = "Save";
            save.Color = Color.LightGray;
            save.BackGroundColor = Color.Gray;
            save.OnClicked = () => {
                SceneManager.SaveScene("scene1.scene");
            };

            var newScene = _editorUi.AddComponent<UiTextComponent>();
            newScene.Rect = new Rectangle(0, 33, 30, 30);
            newScene.Text = "New";
            newScene.Color = Color.LightGray;
            newScene.BackGroundColor = Color.Gray;
            newScene.OnClicked = () => {
                SceneManager.CreateNewScene("scene1.scene");
            };
            */

            var scrollBar = _editorUi.AddComponent<UIScrollBar>();
            var screenHeight = CoreEngine.instance.GraphicsDevice.Viewport.Height;
            var screenWidth = CoreEngine.instance.GraphicsDevice.Viewport.Width;
            scrollBar.Rect = new Rectangle(screenWidth / 2 - 30/2, 90, 30, 300);
            scrollBar.value = 0.5f;
            scrollBar.Color = Color.DimGray;

            var contentPanel = new GameObject("Content Panel");
            var contentPanelUI = contentPanel.AddComponent<UiTextureComponent>();
            contentPanelUI.Rect = new Rectangle(screenWidth/2 - 330, 90, 300, 300);
            contentPanelUI.Color = Color.Green;

                var scrollRect = contentPanel.AddComponent<UIScrollRect>();
            scrollRect.Scrollbar = scrollBar;
            scrollRect.ScrollRect = contentPanelUI;
            scrollRect.ScrollAmount = 20;

            contentPanel.Instantiate();
        }
    }
}
