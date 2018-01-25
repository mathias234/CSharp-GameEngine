using System;
using System.Drawing;
using NewEngine.Engine.Physics;
using NewEngine.Engine.Rendering;
using NewEngine.Engine.Rendering.GUI;
using NewEngine.Engine.Rendering.ResourceManagament;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using FileSystem;
using NewEngine.Engine;
using NewEngine.Engine.Core;

namespace Player {
    public class Player : GameWindow, ICoreEngine {
        private static int _width;
        private static int _height;
        private VSyncMode _vSync;

        private Action<float> _start;

        private NewEngine.Engine.Core.GameObject _rootObject = new NewEngine.Engine.Core.GameObject("root");

        /// <summary>
        /// Sets up a new CoreEngine, Call Start to start the engine and open the new window
        /// </summary>
        /// <param name="width">The width of the window</param>
        /// <param name="height">The height of the window</param>
        /// <param name="vSync">Decide what VSYNC the engine will use</param>
        /// <param name="game">The start point of the game you are making, you will usually overwrite the Game class with your own</param>
        public Player(int width, int height, VSyncMode vSync, Action<float> start)
            : base(width, height, new GraphicsMode(32, 24, 0, 0)) {
            GetCoreEngine = this;
            _width = width;
            _height = height;
            _vSync = vSync;

            _start += start;

        }

        public static ICoreEngine GetCoreEngine { get; private set; }

        public RenderingEngine RenderingEngine { get; set; }
        public GUIRenderer GUIRenderingEngine { get; set; }
        private Dispatcher _dispatcher;

        public void CreateWindow(string title) {
            _dispatcher = new Dispatcher();

            Title = title;
            RenderingEngine = new RenderingEngine(_width, _height, BindAsRenderTarget);


            ClientSize = new Size(_width, _height);
            GUIRenderingEngine = new GUIRenderer(this);
        }

        public void Start() {
            _start?.Invoke(5);

            UpdateFrame += Run;
            RenderFrame += Render;
            Resize += ResizeWindow;
            VSync = _vSync;

            // Later make this customizable
            Run(200, 200);
        }

        public void AddObject(NewEngine.Engine.Core.GameObject gObj) {
            gObj.Parent = _rootObject;
            _rootObject.AddChild(gObj);
        }

        private void Run(object sender, FrameEventArgs e) {
            _rootObject.UpdateAll((float)e.Time);

            Input.Update();

            PhysicsEngine.Update((float) e.Time);

            _dispatcher.Update();
        }

        private void Render(object sender, FrameEventArgs e) {
            _rootObject.AddToEngine(this);

            RenderingEngine.Focused = Focused;
            RenderingEngine.Render((float)e.Time); // clear the screen and render all objects
            GUIRenderingEngine.Render((float)e.Time); // when rendering engine is done this will render all ui objects

            SwapBuffers();
        }

        private void ResizeWindow(object sender, EventArgs e) {
            GL.Viewport(ClientRectangle);
            RenderingEngine.ResizeWindow(Width, Height);
            _width = Width;
            _height = Height;
        }

        public void ShutdownEngine() {
            ResourceManager.CleanupResources();
        }

        public static void BindAsRenderTarget() {
            GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, 0);
            GL.Viewport(0, 0, (int) GetWidth(), (int) GetHeight());
        }

        public static float GetWidth() {
            return _width;
        }

        public static void SetWidth(int width)
        {
            _width = width;
        }

        public static float GetHeight() {
            return _height;
        }

        public static void SetHeight(int height) {
            _height = height;
        }
    }
}