using OpenTK;

namespace NewEngine.Engine.Core {
    public class RectTransform : Transform {
        public Vector2 Size;

        public RectTransform(float width, float height, float posX, float posY) {
            Size = new Vector2(width, height);
            Position = new Vector3(posX, posY, 0);
        }

        public bool Interesects(RectTransform other) {
            var xOverlap = ValueInRange(Position.X, other.Position.X, other.Position.X + other.Size.X) ||
                           ValueInRange(other.Position.X, Position.X, Position.X + Size.X);

            var yOverlap = ValueInRange(Position.Y, other.Position.Y, other.Position.Y + other.Size.Y) ||
                           ValueInRange(other.Position.Y, Position.Y, Position.Y + Size.Y);

            return xOverlap && yOverlap;
        }

        private bool ValueInRange(float value, float min, float max) {
            return (value >= min) && (value <= max);
        }
    }
}