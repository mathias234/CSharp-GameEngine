using NewEngine.Engine.Core;
using OpenTK;

namespace Game {
    class Program {
        private static void Main(string[] args) {
            using (var engine = new CoreEngine(800, 600, VSyncMode.Off, new TestGame())) {
                engine.CreateWindow("TestGame");
                engine.Start();
            }
        }
    }
}
