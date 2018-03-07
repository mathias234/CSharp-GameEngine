using NewEngine.Engine;
using NewEngine.Engine.Audio;
using NewEngine.Engine.components;
using NewEngine.Engine.Core;
using NewEngine.Engine.Rendering;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Threading;
using NewEngine.Engine.Rendering.GUI;
using NewEngine.Engine.Physics;
using OpenTK.Input;

namespace Editor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, ICoreEngine
    {
        private int frames;
        private GLControl glControl;
        private DateTime lastMeasureTime;

        NewEngine.Engine.Core.Dispatcher _dispatcher;

        private GameObject _camera;
        private GameObject _cube;
        private GameObject _directionalLightObj;

        public RenderingEngine RenderingEngine { get; set; }
        public GUIRenderer GUIRenderingEngine { get; set; }

        int renderFrameWidth;
        int renderFrameHeight;

        private bool firstUpdate = true;

        public bool Focused => true;

        private DrawableZone _loadedZone;


        // *** CONTROLS ***
        private bool useBrush;
        private float brushSize = 5;

        public MainWindow()
        {
            InitializeComponent();


            _dispatcher = new NewEngine.Engine.Core.Dispatcher();


            this.lastMeasureTime = DateTime.Now;
            this.frames = 0;
            this.glControl = new GLControl();
            this.glControl.Dock = DockStyle.Fill;
            this.Host.Child = this.glControl;

            OpenTK.Toolkit.Init();

            renderFrameWidth = (int)glControl.Width;
            renderFrameHeight = (int)glControl.Height;

            glControl.MakeCurrent();

            RenderingEngine = new RenderingEngine(renderFrameWidth, renderFrameHeight, () =>
            {
                GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, 0);
                GL.Viewport(0, 0, (int)renderFrameWidth, (int)renderFrameHeight);
            });


            // START:

            CreateCamera();


            RenderingEngine.Instance.SetSkybox("skybox/top.jpg", "skybox/bottom.jpg", "skybox/front.jpg",
                                    "skybox/back.jpg", "skybox/left.jpg", "skybox/right.jpg");


            this.glControl.Paint += GlControl_Paint;
        }

        void CreateCamera()
        {
            _cube = new GameObject("CUBE");
            _cube.AddComponent(new MeshRenderer("cube.obj", ""));

            _camera = new GameObject("main camera")
                .AddComponent(new FreeLook(true, true))
                .AddComponent(new FreeMove())
                .AddComponent(new Camera(Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(70.0f), renderFrameWidth / renderFrameHeight, 0.1f, 1000)))
                .AddComponent(new AudioListener());

            _directionalLightObj = new GameObject("Directinal Light");
            var directionalLight = new DirectionalLight(new Vector3(1), 0.5f, 10, 140, 0.9f);
            _directionalLightObj.AddComponent(directionalLight);
            _directionalLightObj.Transform.Rotation *= Quaternion.FromAxisAngle(new Vector3(1, 0, 0),
                (float)MathHelper.DegreesToRadians(-65));
        }

        float map(float x, float in_min, float in_max, float out_min, float out_max)
        {
            return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
        }

        private void GlControl_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            var deltaTime = DateTime.Now.Subtract(this.lastMeasureTime);
            this.lastMeasureTime = DateTime.Now;

            RenderingEngine.Focused = Focused;

            float mouseX = Mouse.GetCursorState().X - (float)Host.PointToScreen(new Point(0, 0)).X;
            float mouseY = Mouse.GetCursorState().Y - (float)Host.PointToScreen(new Point(0, 0)).Y;

            /* Update Loop */

            _camera.UpdateAll(deltaTime.Milliseconds);


            if (useBrush)
            {
                var strength = 0.1f;

                var color = new Vector3(0, 1, 0);
                if (Input.GetKey(Key.ControlLeft))
                {
                    color = new Vector3(1, 0, 0);
                    strength *= -1;
                }

                if (Input.GetKey(Key.Q)) {
                    brushSize += 0.05f;
                }
                if (Input.GetKey(Key.E))
                {
                    brushSize -= 0.05f;
                }

                TerrainMesh.BrushCircleColor = color;
                TerrainMesh.BrushCircleRadius = brushSize;

                var unprojectedMouse = Unproject(new Vector3(mouseX, mouseY, 0));


                RayCastResult result;
                PhysicsEngine.Raycast(new Ray(_camera.Transform.Position, (unprojectedMouse - _camera.Transform.Position).Normalized()), 500000, out result);

                if (Input.GetMouse(MouseButton.Left))
                {
                    _loadedZone.DrawOnTerrain(DrawBrush.Circle, result.HitData.Location.X, result.HitData.Location.Z, brushSize, strength);
                }


                // update circle
                if (result != null && result.HitData.Location != new Vector3(0, 0, 0))
                    TerrainMesh.BrushCirclePosition = new Vector2(result.HitData.Location.X, result.HitData.Location.Z);
                else
                {
                    // Place the brush far away
                    TerrainMesh.BrushCirclePosition = new Vector2(-float.MaxValue, -float.MaxValue);
                }
            }
            else {
                TerrainMesh.BrushCirclePosition = new Vector2(-float.MaxValue, -float.MaxValue);
            }

            /* End Update Loop */

            Input.Update();
            PhysicsEngine.Update(deltaTime.Milliseconds);
            _dispatcher.Update();

            /* DRAW */

            RenderingEngine.Instance.Render(deltaTime.Milliseconds);

            if (_loadedZone != null)
                _loadedZone.Draw(_camera.Transform.Position.X / DrawableChunk.ChunkSizeX, _camera.Transform.Position.Z / DrawableChunk.ChunkSizeY, 2, this);


            _camera.AddToEngine(this);
            _cube.AddToEngine(this);
            _directionalLightObj.AddToEngine(this);

            /* END DRAW */

            GL.Finish();

            SwapBuffers();

            this.glControl.Invalidate();


            this.frames++;

            // Everything is now setup so we need to update size
            if (firstUpdate)
            {
                UpdateSize(glControl.Width, glControl.Height);
                firstUpdate = false;
            }
        }

        public void SwapBuffers()
        {
            this.glControl.SwapBuffers();
        }

        private void UpdateSize(int width, int height)
        {
            renderFrameWidth = (int)glControl.Width;
            renderFrameHeight = (int)glControl.Height;

            GL.Viewport(0, 0, width, height);
            RenderingEngine.ResizeWindow(width, height);
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateSize(glControl.Width, glControl.Height);
        }

        private void Window_GotFocus(object sender, RoutedEventArgs e)
        {
            UpdateSize(glControl.Width, glControl.Height);
        }

        private void SpawnButton_Click(object sender, RoutedEventArgs e)
        {
            GameObject obj = new GameObject("TEST");
            obj.AddComponent(new MeshRenderer("cube.obj", "null"));
            obj.Transform.Position = _camera.Transform.Position;

            if (_loadedZone != null)
                _loadedZone.AddObject(obj);
        }


        private void New_Click(object sender, RoutedEventArgs e)
        {
            DrawableZone.CreateNewZone(0, 20, 20);
            _loadedZone = new DrawableZone(0);
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            _loadedZone.Save();
        }

        private void Load_Click(object sender, RoutedEventArgs e)
        {
            _loadedZone = new DrawableZone(0);
        }

        private void Wireframe_Click(object sender, RoutedEventArgs e)
        {
            RenderingEngine.Wireframe = !RenderingEngine.Wireframe;
        }

        public Vector3 Unproject(Vector3 mouse)
        {

            Vector3 worldCoord = new Vector3();
            int[] viewport = new int[4];
            GL.GetInteger(GetPName.Viewport, viewport);

            Glu.UnProject(mouse, _camera.GetComponent<Camera>().GetView(), _camera.GetComponent<Camera>().Projection, viewport, ref worldCoord);
            return worldCoord;
        }

        private void Brush_Click(object sender, RoutedEventArgs e)
        {
            TerrainMesh.BrushCircleRadius = brushSize;
            useBrush = !useBrush;
        }
    }
}
