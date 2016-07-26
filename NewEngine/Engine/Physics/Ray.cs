using OpenTK;

namespace NewEngine.Engine.Physics {
    public class Ray {
        /// <summary>Direction in which the ray points.</summary>
        public Vector3 Direction;

        public Vector3 Position;

        /// <summary>Constructs a new ray.</summary>
        /// <param name="position">Starting position of the ray.</param>
        /// <param name="direction">Direction in which the ray points.</param>
        public Ray(Vector3 position, Vector3 direction) {
            Position = position;
            Direction = direction;
        }
    }
}