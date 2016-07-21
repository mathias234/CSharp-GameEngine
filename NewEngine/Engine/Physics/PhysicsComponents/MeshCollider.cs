using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BEPUphysics.BroadPhaseEntries;
using BEPUphysics.Entities;
using BEPUphysics.Entities.Prefabs;
using BEPUutilities;
using NewEngine.Engine.components;
using NewEngine.Engine.Core;
using NewEngine.Engine.Rendering;

namespace NewEngine.Engine.Physics.PhysicsComponents {
    public class MeshCollider : PhysicsComponent {
        private Vector3[] _vertices;
        private int[] _indices;


        public MeshCollider(Vertex[] vertices, int[] indices) {
            _vertices = new Vector3[vertices.Length];

            for (int i = 0; i < vertices.Length; i++) {
                _vertices[i] = ToBepuVector3(vertices[i].Position);
            }
            _indices = indices;
        }

        public MeshCollider(Mesh mesh) {
            _vertices = new Vector3[mesh.Vertices.Length];

            for (int i = 0; i < mesh.Vertices.Length; i++) {
                _vertices[i] = ToBepuVector3(mesh.Vertices[i].Position);
            }
            _indices = mesh.Indices;
        }

        public override void OnEnable() {

            // can have more but to be safe this is the number i went with
            // if you want more split the mesh into more objects
            if (_vertices.Length >= 32296) {
                LogManager.Error("Too many verticies. ABORTING");
                return;
            }

            LogManager.Debug(_vertices.Length.ToString());

            PhysicsObject = new StaticMesh(_vertices, _indices, new AffineTransform(ToBepuVector3(Parent.Transform.GetTransformedPosition())));


            PhysicsEngine.AddToPhysicsEngine(PhysicsObject);
        }
    }
}
