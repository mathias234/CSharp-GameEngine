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
        private string _title;

        private Game _game;
        private RenderingEngine _renderingEngine;


        public CoreEngine(int width, int height, VSyncMode vSync, Game game) : base(width, height, new GraphicsMode(32,24,0,32)) {
            _width = width;
            _height = height;
            _game = game;
            _vSync = vSync;
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
            Run(60);
        }

        private void Run(object sender, FrameEventArgs e) {
            _game.Update((float)e.Time);

            _renderingEngine.MainCamera.Update((float)e.Time);

            PhysicsEngine.Update((float)e.Time);

            Input.Update(Mouse);
        }

        private void Render(object sender, FrameEventArgs e) {
           // LogManager.Debug(Fps.GetFps(e.Time).ToString());
            _renderingEngine.Render(_game.GetRootObject);
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

        public Vector2 GetWindowCenter() {
            return new Vector2((float)GetWidth() /2, (float)GetWidth() /2);
        }
    }
}
