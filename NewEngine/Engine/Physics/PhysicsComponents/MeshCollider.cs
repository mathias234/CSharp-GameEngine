using BEPUphysics.BroadPhaseEntries;
using BEPUutilities;
using NewEngine.Engine.Rendering;

namespace NewEngine.Engine.Physics.PhysicsComponents {
    public class MeshCollider : PhysicsComponent {
        private int[] _indices;
        private Vector3[] _vertices;


        public MeshCollider(Vertex[] vertices, int[] indices) {
            _vertices = new Vector3[vertices.Length];

            for (var i = 0; i < vertices.Length; i++) {
                _vertices[i] = ToBepuVector3(vertices[i].Position);
            }
            _indices = indices;
        }

        public MeshCollider(Mesh mesh) {
            _vertices = new Vector3[mesh.Vertices.Length];

            for (var i = 0; i < mesh.Vertices.Length; i++) {
                _vertices[i] = ToBepuVector3(mesh.Vertices[i].Position);
            }
            _indices = mesh.Indices;
        }

        public override void OnEnable() {
            PhysicsObject = new StaticMesh(_vertices, _indices,
                new AffineTransform(ToBepuVector3(gameObject.Transform.GetTransformedPosition())));


            PhysicsEngine.AddToPhysicsEngine(PhysicsObject);
        }
    }
}