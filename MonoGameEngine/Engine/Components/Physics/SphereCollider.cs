using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jitter.Collision.Shapes;
using Jitter.Dynamics;
using Jitter.LinearMath;
using Microsoft.Xna.Framework;

namespace MonoGameEngine.Engine.Physics {
    public class SphereCollider : Collider {
        public float Radius { get; set; }
        public bool IsStatic { get; set; }

        public SphereCollider() { }

        public SphereCollider(float radius, float mass) {
            this.Radius = radius;
            this.Mass = mass;
        }

        public override void Init() {
            base.Init();

            RigidBody = new RigidBody(new SphereShape(Radius)) {
                Tag = GameObject.name,
                Position =
                    new JVector(GameObject.Transform.Position.X, GameObject.Transform.Position.Y,
                        GameObject.Transform.Position.Z),
                IsStatic = IsStatic,
                Mass = Mass <= 0 ? 0.1f : Mass
            };

            PhysicsEngine.AddPhysicsObject(RigidBody);
        }

        public override void Update(float deltaTime) {
            base.Update(deltaTime);

            if (!RigidBody.IsStatic)
                GameObject.Transform.Position = new Vector3(RigidBody.Position.X, RigidBody.Position.Y, RigidBody.Position.Z);
        }
    }
}
