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
    }
}
