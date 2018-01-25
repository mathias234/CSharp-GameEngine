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

        private GameObject _root;

        private GameObject _camera;

        public RenderingEngine RenderingEngine { get; set; }
        public GUIRenderer GUIRenderingEngine { get; set; }

        int renderFrameWidth;
        int renderFrameHeight;

        private bool firstUpdate = true;

        public bool Focused => true;

        private void AddObject(GameObject gobj)
        {
            gobj.Parent = _root;
            _root.AddChild(gobj);
        }

        public MainWindow()
        {
            InitializeComponent();


            _dispatcher = new NewEngine.Engine.Core.Dispatcher();
            _root = new GameObject("ROOT");

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


            GameObject obj = new GameObject("TEST");
            obj.AddComponent(new MeshRenderer("cube.obj", "null"));

            AddObject(obj);

            RenderingEngine.Instance.SetSkybox("skybox/top.jpg", "skybox/bottom.jpg", "skybox/front.jpg",
                                    "skybox/back.jpg", "skybox/left.jpg", "skybox/right.jpg");


            this.glControl.Paint += GlControl_Paint;
        }

        void CreateCamera()
        {
            _camera = new GameObject("main camera")
                .AddComponent(new FreeLook(true, true))
                .AddComponent(new FreeMove())
                .AddComponent(new Camera(Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(70.0f), renderFrameWidth / renderFrameHeight, 0.1f, 1000)))
                .AddComponent(new AudioListener());

            //_camera.Transform.Rotate(new Vector3(1, 0, 0), -0.4f);

            AddObject(_camera);
        }

        private void GlControl_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            var deltaTime = DateTime.Now.Subtract(this.lastMeasureTime);
            this.lastMeasureTime = DateTime.Now;

            RenderingEngine.Focused = Focused;

            Input.Update();

            _root.UpdateAll(deltaTime.Milliseconds);


            _dispatcher.Update();

            _root.AddToEngine(this);

            this.glControl.Invalidate();



            /* DRAW */

            RenderingEngine.Instance.Render(deltaTime.Milliseconds);

            /* END DRAW */

            SwapBuffers();

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
            AddObject(obj);
        }
    }
}
