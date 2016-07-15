using System.Collections.Generic;
using NewEngine.Engine.Core;
using OpenTK;

namespace NewEngine.Engine.Rendering.MeshLoading {
    public class IndexedModel {
        private List<Vector3> _positions;
        private List<Vector2> _texCoords;
        private List<Vector3> _normals;
        private List<Vector3> _tangents;
        private List<int?> _indices;

        public IndexedModel() {
            _positions = new List<Vector3>();
            _texCoords = new List<Vector2>();
            _normals = new List<Vector3>();
            _tangents = new List<Vector3>();
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

        public void CalculateTangents() {
            for (var i = 0; i < _indices.Count; i += 3) {
                int i0 = _indices[i].Value;
                int i1 = _indices[i + 1].Value;
                int i2 = _indices[i + 2].Value;


                Vector3 edge1 = _positions[i1] - _positions[i0];
                Vector3 edge2 = _positions[i2] - _positions[i0];


                float deltaU1 = _texCoords[i1].X - _texCoords[i0].X;
                float deltaV1 = _texCoords[i1].Y - _texCoords[i0].Y;
                float deltaU2 = _texCoords[i2].X - _texCoords[i0].X;
                float deltaV2 = _texCoords[i2].Y - _texCoords[i0].Y;

                float dividend = (deltaU1 * deltaV2 - deltaU2 * deltaV1);
                //TODO: The first 0.0f may need to be changed to 1.0f here.
                float f = dividend == 0 ? 0.0f : 1.0f / dividend;

                Vector3 tangent = new Vector3(0, 0, 0);
                tangent.X = (f * (deltaV2 * edge1.X - deltaV1 * edge2.X));
                tangent.Y = (f * (deltaV2 * edge1.Y - deltaV1 * edge2.Y));
                tangent.Z = (f * (deltaV2 * edge1.Z - deltaV1 * edge2.Z));

                _tangents[i0] =  (_tangents[i0] + tangent);
                _tangents[i1] =  (_tangents[i1] + tangent);
                _tangents[i2] =  (_tangents[i2] + tangent);
            }

            for (int i = 0; i < _tangents.Count; i++) {
                _tangents[i] = _tangents[i].Normalized();
            }
        }

        public List<Vector3> Normals
        {
            get { return _normals; }
        }

        public List<Vector2> TexCoords
        {
            get { return _texCoords; }
        }

        public List<Vector3> Positions
        {
            get { return _positions; }
        }

        public List<int?> Indices
        {
            get { return _indices; }
        }

        public List<Vector3> Tangents
        {
            get { return _tangents; }
        }
    }
}
