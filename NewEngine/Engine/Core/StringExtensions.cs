using System.Globalization;

namespace NewEngine.Engine.Core {
    public static class StringExtensions {
        public static float ParseInvariantFloat(this string floatString) {
            return float.Parse(floatString, CultureInfo.InvariantCulture.NumberFormat);
        }
    }
}
