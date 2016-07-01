using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameEngine.Engine {
    public class Mesh {
        public Vector3[] Vertices;
        public short[] Indices;
        public BoundingBox BoundingBox;
        public Vector2[] Uvs { get; set; }
        public Vector3[] Normals { get; set; }

        public void CalculateBoundingBox() {
            // not beautiful but might work, slow on really big objects so maybe not run this runtime
            BoundingBox bb = new BoundingBox();

            float minPosX = 0;
            float maxPosX = 0;

            float minPosY = 0;
            float maxPosY = 0;

            float minPosZ = 0;
            float maxPosZ = 0;

            BoundingBox = BoundingBox.CreateFromPoints(Vertices);
            /*
                foreach (var vertexPositionNormalTexture in Vertices) {
                    if (vertexPositionNormalTexture.Position.X < minPosX)
                        minPosX = vertexPositionNormalTexture.Position.X;

                    if (vertexPositionNormalTexture.Position.X > maxPosX)
                        maxPosX = vertexPositionNormalTexture.Position.X;

                    if (vertexPositionNormalTexture.Position.Y < minPosY)
                        minPosY = vertexPositionNormalTexture.Position.Y;

                    if (vertexPositionNormalTexture.Position.Y > maxPosY)
                        maxPosY = vertexPositionNormalTexture.Position.Y;

                    if (vertexPositionNormalTexture.Position.Z < minPosZ)
                        minPosZ = vertexPositionNormalTexture.Position.Z;

                    if (vertexPositionNormalTexture.Position.Z > maxPosZ)
                        maxPosZ = vertexPositionNormalTexture.Position.Z;
                }
                */
            bb.Min = new Vector3(minPosX, minPosY, minPosZ);
            bb.Max = new Vector3(maxPosX, maxPosY, maxPosZ);

        //    BoundingBox = bb;
        }

        /// <summary>
        /// BROKEN!
        /// </summary>
        public void CalculateNormals() {
            Normals = new Vector3[Indices.Length];
            for (var i = 0; i < Indices.Length; i += 3) {
                Vector3 v0 = Vertices[Indices[i]];
                Vector3 v1 = Vertices[Indices[i + 1]];
                Vector3 v2 = Vertices[Indices[i + 2]];

                Vector3 normal = Vector3.Normalize(Vector3.Cross(v2 - v0, v1 - v0)); //This is the normal of the triangle if that's all you're interested in.

                Normals[i] = normal;
                Normals[i + 1] = normal;
                Normals[i + 2] = normal;
            }

            for (var i = 0; i < Vertices.Length; i++) {
                Normals[i] = Vector3.Normalize(Normals[i]);
            }
        }
    }
}
