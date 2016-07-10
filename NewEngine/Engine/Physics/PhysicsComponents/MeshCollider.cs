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
        private Vertex[] _vertices;
        private int[] _indices;


        public MeshCollider(Vertex[] vertices, int[] indices) {
            _vertices = vertices;
            _indices = indices;
        }

        public override void OnEnable() {
            AddPhysicsObjectThreaded();
        }

        // since this operation is slow on some big models, do it in a thread, JK!
        private void AddPhysicsObjectThreaded() {
            Vector3[] vertices = new Vector3[_vertices.Length];

            for (int i = 0; i < vertices.Length; i++) {
                vertices[i] = ToBepuVector3(_vertices[i].Position);
            }

            // can have more but to be safe this is the number i went with
            // if you want more split the mesh into more objects
            if (vertices.Length >= 32296) {
                LogManager.Error("Too many verticies. ABORTING");
                return;
            }

            PhysicsObject = new StaticMesh(vertices, _indices, new AffineTransform(ToBepuVector3(Parent.Transform.Position)));

            PhysicsEngine.AddToPhysicsEngine(PhysicsObject);
        }

    }
}
