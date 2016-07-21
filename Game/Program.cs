using NewEngine.Engine.Core;
using OpenTK;

namespace Game {
    class Program {
        private static void Main(string[] args) {
            using (var engine = new CoreEngine(1920, 1080, VSyncMode.Off, new TestGame())) {
                engine.CreateWindow("TestGame");
                engine.Start();
            }
        }
    }
}
