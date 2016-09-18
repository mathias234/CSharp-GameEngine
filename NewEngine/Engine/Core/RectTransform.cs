using OpenTK;

namespace NewEngine.Engine.Core {
    public class RectTransform : Transform {

        public RectTransform(float width, float height, float posX, float posY) {
            Scale = new Vector3(width, height, 1);
            Position = new Vector3(posX, posY, 0);
        }

        public bool Interesects(RectTransform other) {
            var xOverlap = ValueInRange(Position.X, other.Position.X, other.Position.X + other.Scale.X) ||
                           ValueInRange(other.Position.X, Position.X, Position.X + Scale.X);

            var yOverlap = ValueInRange(Position.Y, other.Position.Y, other.Position.Y + other.Scale.Y) ||
                           ValueInRange(other.Position.Y, Position.Y, Position.Y + Scale.Y);

            return xOverlap && yOverlap;
        }

        private bool ValueInRange(float value, float min, float max) {
            return (value >= min) && (value <= max);
        }
    }
}