using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace NewEngine.Engine.Physics {
    public class Ray {
        public Vector3 Position;
        /// <summary>Direction in which the ray points.</summary>
        public Vector3 Direction;

        /// <summary>Constructs a new ray.</summary>
        /// <param name="position">Starting position of the ray.</param>
        /// <param name="direction">Direction in which the ray points.</param>
        public Ray(Vector3 position, Vector3 direction) {
            this.Position = position;
            this.Direction = direction;
        }

    }
}
