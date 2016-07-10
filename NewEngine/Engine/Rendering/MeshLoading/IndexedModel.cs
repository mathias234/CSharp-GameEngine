using System.Collections.Generic;
using NewEngine.Engine.Core;
using OpenTK;

namespace NewEngine.Engine.Rendering.MeshLoading {
   public  class IndexedModel {
        private List<Vector3> _positions;
        private List<Vector2> _texCoords;
        private List<Vector3> _normals;
        private List<int?> _indices;

        public IndexedModel() {
            _positions = new List<Vector3>();
            _texCoords = new List<Vector2>();
            _normals = new List<Vector3>();
            _indices = new List<int?>();
        }

        public void CalculateNormals() {
            for (var i = 0; i < _indices.Count; i += 3) {
                int i0 = _indices[i].Value;
                int i1 = _indices[i + 1].Value;
                int i2 = _indices[i + 2].Value;

                Vector3 v1 = _positions[i1] - _positions[i0];
                Vector3 v2 = _positions[i2] - _positions[i0];

                Vector3 normal = Vector3.Cross(v1, v2);
                normal.Normalize();

                _normals[i0] = _normals[i0] + normal;
                _normals[i1] = _normals[i1] + normal;
                _normals[i2] = _normals[i2] + normal;
            }

            for (int i = 0; i < _normals.Count; i++) {
                _normals[i] = _normals[i].Normalized();
            }
        }

        public List<Vector3> Normals {
           get { return _normals; }
       }

       public List<Vector2> TexCoords {
           get { return _texCoords; }
       }

       public List<Vector3> Positions {
           get { return _positions; }
       }

       public List<int?> Indices {
           get { return _indices; }
       }
   }
}
