using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using MinecraftClone;
using MonoGameEngine.GameCode;

namespace WindowsFormsApplication1 {
    static class Program {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main() {
            var GameCode = new GameCode();
            Engine.EngineStart(GameCode);
        }
    }
}
