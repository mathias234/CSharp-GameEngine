using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BEPUphysics.Entities.Prefabs;
using Microsoft.Xna.Framework;

namespace MonoGameEngine.Engine.Physics {
    public class SphereCollider : Collider {
        public float Radius { get; set; }
        public bool IsStatic { get; set; }

        public override void Init() {
            RigidBody = new Sphere(new BEPUutilities.Vector3(GameObject.Transform.Position.X, GameObject.Transform.Position.Y,
GameObject.Transform.Position.Z), Radius, Mass);


            PhysicsEngine.AddPhysicsObject(RigidBody);
        }

        public SphereCollider(float radius, float mass) {
            this.Radius = radius;
            this.Mass = mass;
        }

        public SphereCollider() {
        }

        public override void Update(float deltaTime) {
            base.Update(deltaTime);

            var rBody = (Sphere)RigidBody;

            if (rBody.IsDynamic)
                GameObject.Transform.Position = new Vector3(rBody.Position.X, rBody.Position.Y, rBody.Position.Z);
        }
    }
}
