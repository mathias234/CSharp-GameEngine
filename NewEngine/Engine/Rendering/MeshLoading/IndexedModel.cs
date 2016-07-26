using System;
using System.Collections.Generic;
using OpenTK;

namespace NewEngine.Engine.Rendering.MeshLoading {
    public class IndexedModel {
        public IndexedModel() {
            Positions = new List<Vector3>();
            TexCoords = new List<Vector2>();
            Normals = new List<Vector3>();
            Tangents = new List<Vector3>();
            Indices = new List<int?>();
        }

        public List<Vector3> Normals { get; }

        public List<Vector2> TexCoords { get; }

        public List<Vector3> Positions { get; }

        public List<int?> Indices { get; }

        public List<Vector3> Tangents { get; }

        public void CalculateNormals() {
            for (var i = 0; i < Indices.Count; i += 3) {
                var i0 = Indices[i].Value;
                var i1 = Indices[i + 1].Value;
                var i2 = Indices[i + 2].Value;

                var v1 = Positions[i1] - Positions[i0];
                var v2 = Positions[i2] - Positions[i0];

                var normal = Vector3.Cross(v1, v2);
                normal.Normalize();

                Normals[i0] = Normals[i0] + normal;
                Normals[i1] = Normals[i1] + normal;
                Normals[i2] = Normals[i2] + normal;
            }

            for (var i = 0; i < Normals.Count; i++) {
                Normals[i] = Normals[i].Normalized();
            }
        }

        public void CalculateTangents() {
            for (var i = 0; i < Indices.Count; i += 3) {
                var i0 = Indices[i].Value;
                var i1 = Indices[i + 1].Value;
                var i2 = Indices[i + 2].Value;


                var edge1 = Positions[i1] - Positions[i0];
                var edge2 = Positions[i2] - Positions[i0];


                var deltaU1 = TexCoords[i1].X - TexCoords[i0].X;
                var deltaV1 = TexCoords[i1].Y - TexCoords[i0].Y;
                var deltaU2 = TexCoords[i2].X - TexCoords[i0].X;
                var deltaV2 = TexCoords[i2].Y - TexCoords[i0].Y;

                var dividend = deltaU1*deltaV2 - deltaU2*deltaV1;
                //TODO: The first 0.0f may need to be changed to 1.0f here.
                var f = Math.Abs(dividend) < 0.001f ? 0.0f : 1.0f/dividend;

                var tangent = new Vector3(0, 0, 0) {
                    X = f*(deltaV2*edge1.X - deltaV1*edge2.X),
                    Y = f*(deltaV2*edge1.Y - deltaV1*edge2.Y),
                    Z = f*(deltaV2*edge1.Z - deltaV1*edge2.Z)
                };

                Tangents[i0] = Tangents[i0] + tangent;
                Tangents[i1] = Tangents[i1] + tangent;
                Tangents[i2] = Tangents[i2] + tangent;
            }

            for (var i = 0; i < Tangents.Count; i++) {
                Tangents[i] = Tangents[i].Normalized();
            }
        }
    }
}