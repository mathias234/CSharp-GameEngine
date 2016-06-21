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

namespace MonoGameEngine.Engine.Components {
    public class BoxCollider : Collider {
        public float Width { get; set; }
        public float Height { get; set; }
        public float Length { get; set; }

        public bool IsStatic { get; set; }


        public BoxCollider() { }

        public BoxCollider(float width, float height, float length, float mass) {
            Width = width;
            Height = height;
            Length = length;
            Mass = mass;
        }

        public override void Init() {
            RigidBody = new RigidBody(new BoxShape(Length, Height, Width)) {
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
