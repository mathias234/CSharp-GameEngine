using System;
using NewEngine.Engine.Core;
using OpenTK;

namespace Game {
    class Program {
        private static void Main() {
            CoreEngine engine;
            using (engine = new CoreEngine(1920, 1080, VSyncMode.Adaptive, new TestGame())) {
                engine.CreateWindow("TestGame");
                engine.Start();
            }
            engine.ShutdownEngine();
        }
    }
}
