using System;
using System.Diagnostics;
using System.Linq;
using Jitter;
using Jitter.Collision;
using Jitter.Collision.Shapes;
using Jitter.Dynamics;
using Jitter.LinearMath;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using SwiftEngine.Components;
using SwiftEngine.Engine.Components;

namespace SwiftEngine.Engine {
    public class PhysicsEngine {
        private World world;

        public PhysicsEngine() {
            Debug.WriteLine("Physics Engine initialized");

            CollisionSystem collision = new CollisionSystemPersistentSAP();

            world = new World(collision); world.AllowDeactivation = true;
        }

        public void Update(GameTime gameTime) {
            float step = (float)gameTime.ElapsedGameTime.TotalSeconds;
            world.Step(step, true);
        }

        public static void AddPhysicsObject(RigidBody rigidBody) {
            CoreEngine.instance.GetPhysicsEngine.world.AddBody(rigidBody);
        }

        public void Reset() {
            CollisionSystem collision = new CollisionSystemPersistentSAP();
            world = new World(collision);
        }
    }
}
