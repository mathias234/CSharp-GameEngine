using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BEPUphysics.Entities.Prefabs;
using Microsoft.Xna.Framework;

namespace MonoGameEngine.Engine.Physics {
    public class BoxCollider : Collider {
        public float Width { get; set; }
        public float Height { get; set; }
        public float Length { get; set; }

        public bool IsStatic { get; set; }


        public BoxCollider() {

        }

        public BoxCollider(float width, float height, float length, float mass) {
            Width = width;
            Height = height;
            Length = length;
            Mass = mass;
        }

        public override void Init() {
            base.Init();

            var position = new BEPUutilities.Vector3(GameObject.Transform.Position.X, GameObject.Transform.Position.Y,
    GameObject.Transform.Position.Z);

            RigidBody = new Box(position, Width, Height, Length, Mass);

            PhysicsEngine.AddPhysicsObject(RigidBody);
        }

        public override void Update(float deltaTime) {
            base.Update(deltaTime);
            var rBody = (Box)RigidBody;

            if (rBody.IsDynamic)
                GameObject.Transform.Position = new Vector3(rBody.Position.X, rBody.Position.Y, rBody.Position.Z);
        }
    }
}
