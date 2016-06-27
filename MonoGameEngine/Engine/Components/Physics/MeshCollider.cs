using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BEPUphysics.Entities.Prefabs;
using Microsoft.Xna.Framework;
using MonoGameEngine.Engine.Components;

namespace MonoGameEngine.Engine.Physics {
    public class MeshCollider : Collider {
        public bool IsStatic { get; set; }

        public MeshCollider() { }

        public override void Init() {
            base.Init();

            if (GameObject.GetComponent<MeshRenderer>() == null)
                GameObject._components.Remove(this);

            List<BEPUutilities.Vector3> vectors = new List<BEPUutilities.Vector3>();
            foreach (var vector3 in GameObject.GetComponent<MeshRenderer>().Mesh.Vertices) {
                vectors.Add(new BEPUutilities.Vector3(vector3.X, vector3.Y, vector3.Z));
            }

            var rBody = new ConvexHull(new BEPUutilities.Vector3(GameObject.Transform.Position.X, GameObject.Transform.Position.Y,
                GameObject.Transform.Position.Z), vectors);

            RigidBody = rBody;

            PhysicsEngine.AddPhysicsObject(RigidBody);

        }

        public override void Update(float deltaTime) {
            base.Update(deltaTime);
            var rBody = (ConvexHull)RigidBody;

            if (rBody.IsDynamic)
                GameObject.Transform.Position = new Vector3(rBody.Position.X, rBody.Position.Y, rBody.Position.Z);
        }
    }
}
