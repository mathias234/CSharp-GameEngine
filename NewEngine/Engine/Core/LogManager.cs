using System;

namespace NewEngine.Engine.Core {
    public class LogManager {
        public static void Error(string error) {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(error);
            Console.ResetColor();
        }

        public static void Debug(string message) {
            Console.WriteLine(message);
        }
    }
}
