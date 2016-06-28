using System;
using MonoGameEngine.Engine;

namespace MonoGameEngine.GameCode {
#if Editor || WINDOWS || LINUX
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Engine {
        public static void EngineStart(BaseGameCode gameCode) {
#if Editor
            var editorCode = new EditorCode();
            
            using (var editor = new CoreEngine("MonoGameEngine", editorCode))
                editor.Run();
#else

            using (var editor = new CoreEngine("MonoGameEngine", gameCode))
                editor.Run();
#endif
        }
    }
#endif
}
