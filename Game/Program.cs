using System;
using NewEngine.Engine.Core;
using OpenTK;

namespace Game {
    class Program {
        private static void Main() {
            using (var engine = new CoreEngine(1920, 1080, VSyncMode.Adaptive, new TestGame())) {
                engine.CreateWindow("TestGame");
                engine.Start();
            }
        }
    }
}
