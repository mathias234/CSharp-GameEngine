using System;
using MonoGameEngine.Engine;

namespace MonoGameEngine.GameCode {
#if Editor || WINDOWS || LINUX
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program {

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main() {
#if Editor
            var baseGameCode = new EditorCode();
#else
            var baseGameCode = new GameCode();
#endif

            using (var game = new CoreEngine("MonoGameEngine", baseGameCode))
                game.Run();
        }
    }
#endif
        }
