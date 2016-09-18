using System;
using System.Drawing;
using NewEngine.Engine.Physics;
using NewEngine.Engine.Rendering;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace NewEngine.Engine.Core {
    public class CoreEngine : GameWindow {
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

        public static CoreEngine GetCoreEngine { get; private set; }

        public RenderingEngine RenderingEngine { get; set; }

        public void CreateWindow(string title) {
            Title = title;

            ClientSize = new Size(_width, _height);
            RenderingEngine = new RenderingEngine();
        }

        public void Start() {
            Game.Start();

            UpdateFrame += Run;
            RenderFrame += Render;
            Resize += ResizeWindow;
            VSync = _vSync;
            Run(70);
        }

        private void Run(object sender, FrameEventArgs e) {
            Game.Update((float) e.Time);

            Input.Update(Mouse);

            PhysicsEngine.Update((float) e.Time);
        }

        private void Render(object sender, FrameEventArgs e) {
            RenderingEngine.RenderBatches((float)e.Time);
        }

        private void ResizeWindow(object sender, EventArgs e) {
            GL.Viewport(ClientRectangle);
            RenderingEngine.ResizeWindow();
            _width = Width;
            _height = Height;
        }

        public static void BindAsRenderTarget() {
            GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, 0);
            GL.Viewport(0, 0, (int) GetWidth(), (int) GetHeight());
        }

        public static float GetWidth() {
            return _width;
        }

        public static float GetHeight() {
            return _height;
        }
    }
}