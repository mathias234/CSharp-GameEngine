using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BEPUphysics.BroadPhaseEntries;
using BEPUphysics.Entities.Prefabs;
using BEPUutilities;
using MonoGameEngine.Engine.Components;
using Vector3 = Microsoft.Xna.Framework.Vector3;

namespace MonoGameEngine.Engine.Physics {
    public class MeshCollider : Collider {
        private Mesh _mesh;

        public Mesh Mesh {
            get { return _mesh; }
            set {
                PhysicsEngine.RemovePhysicsObject(RigidBody);
                _mesh = value;
                List<BEPUutilities.Vector3> vertices = new List<BEPUutilities.Vector3>();
                foreach (var vector3 in _mesh.Vertices) {
                    vertices.Add(new BEPUutilities.Vector3(vector3.X, vector3.Y, vector3.Z));
                }

                List<int> indices = new List<int>();
                foreach (var index in _mesh.Indices) {
                    indices.Add(index);
                }

                var rbody2 = new StaticMesh(vertices.ToArray(), indices.ToArray(), new AffineTransform(new BEPUutilities.Vector3(GameObject.Transform.Position.X, GameObject.Transform.Position.Y,
                    GameObject.Transform.Position.Z)));

                RigidBody = rbody2;
                PhysicsEngine.AddPhysicsObject(RigidBody);
            }
        }

        public MeshCollider() {

        }

        public override void Init() {
            base.Init();
 
        }
    }
}
