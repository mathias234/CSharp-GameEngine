using System;
using System.Drawing;
using NewEngine.Engine.Physics;
using NewEngine.Engine.Rendering;
using NewEngine.Engine.Rendering.GUI;
using NewEngine.Engine.Rendering.ResourceManagament;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace NewEngine.Engine.Core {
    public class CoreEngine : GameWindow, ICoreEngine {
        private static int _width;
        private static int _height;
        private VSyncMode _vSync;

        /// <summary>
        /// Sets up a new CoreEngine, Call Start to start the engine and open the new window
        /// </summary>
        /// <param name="width">The width of the window</param>
        /// <param name="height">The height of the window</param>
        /// <param name="vSync">Decide what VSYNC the engine will use</param>
        /// <param name="game">The start point of the game you are making, you will usually overwrite the Game class with your own</param>
        public CoreEngine(int width, int height, VSyncMode vSync, Game game)
            : base(width, height, new GraphicsMode(32, 24, 0, 0)) {
            GetCoreEngine = this;
            _width = width;
            _height = height;
            Game = game;
            _vSync = vSync;

            game.SetEngine(this);
        }

        public Game Game { get; set; }

        public static ICoreEngine GetCoreEngine { get; private set; }

        public RenderingEngine RenderingEngine { get; set; }
        public GUIRenderer GUIRenderingEngine { get; set; }

        public void CreateWindow(string title) {
            Title = title;

            ClientSize = new Size(_width, _height);
            RenderingEngine = new RenderingEngine(this);
            GUIRenderingEngine = new GUIRenderer(this);
        }

        public void Start() {
            Game.Start();

            UpdateFrame += Run;
            RenderFrame += Render;
            Resize += ResizeWindow;
            VSync = _vSync;

            // Later make this customizable
            Run(200, 200);
        }

        private void Run(object sender, FrameEventArgs e) {
            Game.Update((float) e.Time);

            Input.Update(Mouse);

            PhysicsEngine.Update((float) e.Time);
        }

        private void Render(object sender, FrameEventArgs e) {
            Game.GetRootObject.AddToEngine(this);

            RenderingEngine.Render((float)e.Time); // clear the screen and render all objects
            GUIRenderingEngine.Render((float)e.Time); // when rendering engine is done this will render all ui objects

            SwapBuffers();
        }

        private void ResizeWindow(object sender, EventArgs e) {
            GL.Viewport(ClientRectangle);
            RenderingEngine.ResizeWindow();
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