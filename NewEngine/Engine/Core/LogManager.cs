using System;

namespace NewEngine.Engine.Core {
    public class LogManager {
        public static void Error(string error) {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(error);
            Console.ResetColor();
        }

        public static void Debug(string message) {
            System.Diagnostics.Debug.WriteLine(message);
            Console.WriteLine(message);
        }
    }
}
