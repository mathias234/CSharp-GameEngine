using System;
using System.Diagnostics;
using System.Drawing;
using NewEngine.Engine.Physics;
using NewEngine.Engine.Rendering;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace NewEngine.Engine.Core {
    public class CoreEngine : GameWindow {
        private static int _width;
        private static int _height;
        private VSyncMode _vSync;
        private string _title;

        private Game _game;
        private RenderingEngine _renderingEngine;

        private static CoreEngine _getCoreEngine;


        public CoreEngine(int width, int height, VSyncMode vSync, Game game) : base(width, height, new GraphicsMode(32,24,0,16)) {
            _getCoreEngine = this;
            _width = width;
            _height = height;
            _game = game;
            _vSync = vSync;
            game.SetEngine(this);
        }

        public Game Game {
            get { return _game; }
            set { _game = value; }
        }

        public static CoreEngine GetCoreEngine {
            get { return _getCoreEngine; }
        }

        public RenderingEngine RenderingEngine {
            get { return _renderingEngine; }
            set { _renderingEngine = value; }
        }

        public void CreateWindow(string title) {
            _title = title;
            Title = title;

            ClientSize = new Size(_width, _height);
            _renderingEngine = new RenderingEngine();
        }

        public void Start() {
            _game.Start();

            UpdateFrame += Run;
            RenderFrame += Render;
            Resize += ResizeWindow;
            VSync = _vSync;
            Run(70, 100);
        }

        private void Run(object sender, FrameEventArgs e) {
            LogManager.Debug(Fps.GetFps(e.Time).ToString());
            _game.Update((float)e.Time);

            PhysicsEngine.Update((float)e.Time);

            Input.Update(Mouse);
        }

        private void Render(object sender, FrameEventArgs e) {
            _game.Render(_renderingEngine);
            SwapBuffers();
        }

        private void ResizeWindow(object sender, EventArgs e) {
            GL.Viewport(0, 0, _width, _height);
        }

        public static int GetWidth() {
            return _width;
        }

        public static int GetHeight() {
            return _height;
        }

        public override void Dispose() {
            base.Dispose();
        }
    }
}
