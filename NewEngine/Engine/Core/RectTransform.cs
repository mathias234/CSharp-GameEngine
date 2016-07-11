using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace NewEngine.Engine.Core {
    public class RectTransform : Transform{
        public Vector2 size;

        public RectTransform(float width, float height, float posX, float posY) {
            size = new Vector2(width, height);
            Position = new Vector3(posX, posY,0);
        }

        public bool Interesects(RectTransform other) {
            var xOverlap = ValueInRange(Position.X, other.Position.X, other.Position.X + other.size.X) ||
                  ValueInRange(other.Position.X, Position.X, Position.X + size.X);

            var yOverlap = ValueInRange(Position.Y, other.Position.Y, other.Position.Y + other.size.Y) ||
                  ValueInRange(other.Position.Y, Position.Y, Position.Y + size.Y);

            return xOverlap && yOverlap;
        }

        private bool ValueInRange(float value, float min, float max) {
            return (value >= min) && (value <= max);
        }
    }
}
