using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using MinecraftClone;
using NewEngine.Engine.Core;
using OpenTK;

namespace WindowsFormsApplication1 {
    static class Program {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main() {
            using (var engine = new CoreEngine(1920, 1080, VSyncMode.Off, new GameCode())) {
                engine.CreateWindow("TestGame");
                engine.Start();
            }
        }
    }
}
