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


        public CoreEngine(int width, int height, VSyncMode vSync, Game game)
            : base(width, height, new GraphicsMode(32, 24, 0, 16)) {
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
            Run(70, 100);
        }


        private void Run(object sender, FrameEventArgs e) {
            Game.Update((float) e.Time);

            PhysicsEngine.Update((float) e.Time);

            Input.Update(Mouse);
        }

        private void Render(object sender, FrameEventArgs e) {
            Game.Render(RenderingEngine);
        }

        private void ResizeWindow(object sender, EventArgs e) {
            GL.Viewport(0, 0, _width, _height);
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