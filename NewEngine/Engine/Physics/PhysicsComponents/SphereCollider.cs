using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BEPUphysics.Entities;
using BEPUphysics.Entities.Prefabs;
using NewEngine.Engine.components;
using NewEngine.Engine.Core;
using NewEngine.Engine.Rendering;

namespace NewEngine.Engine.Physics.PhysicsComponents {
    public class SphereCollider : PhysicsComponent {
        private float _radius;

        private float _mass;

        public SphereCollider(float radius, float mass) {
            _radius = radius;
            _mass = mass;
        }

        public override void OnEnable() {
            PhysicsObject = new Sphere(ToBepuVector3(Parent.Transform.Position), _radius, _mass);

            PhysicsEngine.AddToPhysicsEngine(PhysicsObject);
        }

        public override void Update(float deltaTime) {
            var obj = (Sphere)PhysicsObject;
            if (obj != null) {
                Parent.Transform.Position = FromBepuVector3(obj.Position);
                Parent.Transform.Rotation = FromBepuQuaternion(obj.Orientation);
            }
        }
    }
}
