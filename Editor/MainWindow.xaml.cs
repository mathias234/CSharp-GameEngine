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

        public bool Focused => throw new NotImplementedException();

        private void AddObject(GameObject gobj)
        {
            gobj.Parent = _root;
            _root.AddChild(gobj);
        }

        public MainWindow()
        {
            InitializeComponent();

            renderFrameWidth = (int)Host.Width;
            renderFrameHeight = (int)Host.Height;

            _dispatcher = new NewEngine.Engine.Core.Dispatcher();
            _root = new GameObject("ROOT");

            this.lastMeasureTime = DateTime.Now;
            this.frames = 0;
            this.glControl = new GLControl();
            this.glControl.Dock = DockStyle.Fill;
            this.Host.Child = this.glControl;

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
            renderFrameWidth = (int)Host.Width;
            renderFrameHeight = (int)Host.Height;

            _camera.Transform.Position += new Vector3(0, 0, 0.05f);

            _dispatcher.Update();

            _root.AddToEngine(this);

            this.glControl.Invalidate();

            if (DateTime.Now.Subtract(this.lastMeasureTime) > TimeSpan.FromSeconds(1))
            {
                this.Title = this.frames + "fps";
                this.frames = 0;
                this.lastMeasureTime = DateTime.Now;
            }

            this.glControl.MakeCurrent();

            /* DRAW */

            RenderingEngine.Instance.Render(DateTime.Now.Subtract(this.lastMeasureTime).Milliseconds);

            /* END DRAW */

            SwapBuffers();

            this.frames++;
        }

        public void SwapBuffers()
        {
            this.glControl.SwapBuffers();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            GL.Viewport(0, 0, renderFrameWidth, renderFrameHeight);
            RenderingEngine.ResizeWindow(renderFrameWidth, renderFrameHeight);
        }
    }
}
