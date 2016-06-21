using System;
using SwiftEngine.Engine;

namespace SwiftEngine.GameCode {
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

            using (var game = new CoreEngine("SwiftEngine", baseGameCode))
                game.Run();
        }
    }
#endif
        }
