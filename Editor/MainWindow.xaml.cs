using System;
using System.Drawing;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Threading;
using Game;
using NewEngine.Engine;
using NewEngine.Engine.Core;
using NewEngine.Engine.Physics;
using NewEngine.Engine.Rendering;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using Size = System.Drawing.Size;

namespace Editor {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, ICoreEngine {
        private int frames;

        private GLControl glcontrol;

        private DateTime lastMeasureTime;
        private TestGame game;


        private float angle;

        private int displayList;

        private Size size;
        private bool firstRender = true;


        public MainWindow() {
            InitializeComponent();
            Focused = true;


        }

        private void Host_Initialized(object sender, EventArgs e) {
            lastMeasureTime = DateTime.Now;
            frames = 0;

            glcontrol = new GLControl(new GraphicsMode(32, 24, 0, 0));
            glcontrol.Paint += GlcontrolOnPaint;
            glcontrol.Dock = DockStyle.Fill;
            Host.Child = glcontrol;


            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(1);
            timer.Tick += this.TimerOnTick;
            timer.Start();
        }

        private void GlcontrolOnPaint(object sender, PaintEventArgs e) {
            glcontrol.MakeCurrent();

            if (firstRender) {
                RenderingEngine = new RenderingEngine(this);

                game = new TestGame();

                game.SetEngine(this);

                game.Start();

                firstRender = false;
            }

            RenderingEngine.RenderBatches(lastMeasureTime.Millisecond);


            frames++;
        }

        private void TimerOnTick(object sender, EventArgs e) {
            if (DateTime.Now.Subtract(this.lastMeasureTime) > TimeSpan.FromSeconds(1)) {
                Title = this.frames + "fps";
                frames = 0;
                lastMeasureTime = DateTime.Now;
            }

            glcontrol.Invalidate();
        }

        public void SwapBuffers() {
            glcontrol.SwapBuffers();
        }

        public RenderingEngine RenderingEngine { get; set; }

        public bool Focused { get; }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e) {
            CoreEngine.SetWidth(glcontrol.Size.Width);
            CoreEngine.SetHeight(glcontrol.Size.Height);
            GL.Viewport(glcontrol.Size);

            if (RenderingEngine != null)
                RenderingEngine.ResizeWindow();
        }
    }
}
