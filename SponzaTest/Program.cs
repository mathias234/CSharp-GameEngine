using NewEngine.Engine.Core;
using OpenTK;

namespace SponzaTest {
    class Program {
        private static void Main() {
            using (var engine = new CoreEngine(1920, 1080, VSyncMode.Off, new Sponza())) {
                engine.CreateWindow("Sponza Scene");
                engine.Start();
            }
        }
    }
}
