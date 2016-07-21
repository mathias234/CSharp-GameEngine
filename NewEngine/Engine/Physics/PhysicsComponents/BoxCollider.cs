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
    public class BoxCollider : PhysicsComponent {
        private float _width;
        private float _height;
        private float _length;

        private float _mass;

        public BoxCollider(float width, float height, float length, float mass) {
            _width = width;
            _height = height;
            _length = length;
            _mass = mass;
        }

        public override void OnEnable() {
            PhysicsObject = new Box(ToBepuVector3(Parent.Transform.Position), _width, _height, _length, _mass);

            PhysicsEngine.AddToPhysicsEngine(PhysicsObject);
        }

        public override void Update(float deltaTime) {
            var obj = (Box)PhysicsObject;
            if (obj != null) {
                Parent.Transform.Position = FromBepuVector3(obj.Position);
                Parent.Transform.Rotation = FromBepuQuaternion(obj.Orientation);
            }
        }
    }
}
