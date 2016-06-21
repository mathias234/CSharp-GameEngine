using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGameEngine.Engine;

namespace MonoGameEngine {
    public static class Primitives {
        public static Mesh CreateIcosahedron() {
            // A temporary array, with 12 items in it, because
            // the icosahedron has 12 distinct vertices
            var vertices = new VertexPositionNormalTexture[12];

            // vertex position and color information for icosahedron
            vertices[0] = new VertexPositionNormalTexture(new Vector3(-0.26286500f, 0.0000000f, 0.42532500f), Vector3.Up, Vector2.One);
            vertices[1] = new VertexPositionNormalTexture(new Vector3(0.26286500f, 0.0000000f, 0.42532500f), Vector3.Up, Vector2.One);
            vertices[2] = new VertexPositionNormalTexture(new Vector3(-0.26286500f, 0.0000000f, -0.42532500f), Vector3.Up, Vector2.One);
            vertices[3] = new VertexPositionNormalTexture(new Vector3(0.26286500f, 0.0000000f, -0.42532500f), Vector3.Up, Vector2.One);
            vertices[4] = new VertexPositionNormalTexture(new Vector3(0.0000000f, 0.42532500f, 0.26286500f), Vector3.Up, Vector2.One);
            vertices[5] = new VertexPositionNormalTexture(new Vector3(0.0000000f, 0.42532500f, -0.26286500f), Vector3.Up, Vector2.One);
            vertices[6] = new VertexPositionNormalTexture(new Vector3(0.0000000f, -0.42532500f, 0.26286500f), Vector3.Up, Vector2.One);
            vertices[7] = new VertexPositionNormalTexture(new Vector3(0.0000000f, -0.42532500f, -0.26286500f), Vector3.Up, Vector2.One);
            vertices[8] = new VertexPositionNormalTexture(new Vector3(0.42532500f, 0.26286500f, 0.0000000f), Vector3.Up, Vector2.One);
            vertices[9] = new VertexPositionNormalTexture(new Vector3(-0.42532500f, 0.26286500f, 0.0000000f), Vector3.Up, Vector2.One);
            vertices[10] = new VertexPositionNormalTexture(new Vector3(0.42532500f, -0.26286500f, 0.0000000f), Vector3.Up, Vector2.One);
            vertices[11] = new VertexPositionNormalTexture(new Vector3(-0.42532500f, -0.26286500f, 0.0000000f), Vector3.Up, Vector2.One);

            var indices = new short[60];
            indices[0] = 0; indices[1] = 6; indices[2] = 1;
            indices[3] = 0; indices[4] = 11; indices[5] = 6;
            indices[6] = 1; indices[7] = 4; indices[8] = 0;
            indices[9] = 1; indices[10] = 8; indices[11] = 4;
            indices[12] = 1; indices[13] = 10; indices[14] = 8;
            indices[15] = 2; indices[16] = 5; indices[17] = 3;
            indices[18] = 2; indices[19] = 9; indices[20] = 5;
            indices[21] = 2; indices[22] = 11; indices[23] = 9;
            indices[24] = 3; indices[25] = 7; indices[26] = 2;
            indices[27] = 3; indices[28] = 10; indices[29] = 7;
            indices[30] = 4; indices[31] = 8; indices[32] = 5;
            indices[33] = 4; indices[34] = 9; indices[35] = 0;
            indices[36] = 5; indices[37] = 8; indices[38] = 3;
            indices[39] = 5; indices[40] = 9; indices[41] = 4;
            indices[42] = 6; indices[43] = 10; indices[44] = 1;
            indices[45] = 6; indices[46] = 11; indices[47] = 7;
            indices[48] = 7; indices[49] = 10; indices[50] = 6;
            indices[51] = 7; indices[52] = 11; indices[53] = 2;
            indices[54] = 8; indices[55] = 10; indices[56] = 3;
            indices[57] = 9; indices[58] = 11; indices[59] = 0;
            var m = new Mesh {
                Indices = indices,
                Vertices = vertices
            };
            m.CalculateBoundingBox();
            return m;
        }

        public static Mesh CreatePlane() {
            var vertices = new VertexPositionNormalTexture[4];

            vertices[0] = new VertexPositionNormalTexture(new Vector3(-1.0f, 0, -1.0f), Vector3.Up, Vector2.Zero);
            vertices[1] = new VertexPositionNormalTexture(new Vector3(1.0f, 0, -1.0f), Vector3.Up, new Vector2(1,0));
            vertices[2] = new VertexPositionNormalTexture(new Vector3(1.0f, 0, 1.0f), Vector3.Up, new Vector2(1, 1));
            vertices[3] = new VertexPositionNormalTexture(new Vector3(-1.0f, 0, 1.0f), Vector3.Up, new Vector2(0, 1));

            var indices = new short[] {
                0, 1, 2, 2, 3, 0,
            };


            var m = new Mesh {
                Indices = indices,
                Vertices = vertices
            };
            m.CalculateBoundingBox();

            return m;
        }

        public static Mesh CreateCube() {
            var vertices = new VertexPositionNormalTexture[8];
            var indices = new short[18];

            vertices[0] = new VertexPositionNormalTexture(new Vector3(-1.0f, -1.0f, 1.0f), new Vector3(-1.0f, -1.0f, -1.0f), Vector2.Zero);
            vertices[3] = new VertexPositionNormalTexture(new Vector3(-1.0f, 1.0f, 1.0f), new Vector3(-1.0f, 1.0f, 1.0f), Vector2.Zero);
            vertices[4] = new VertexPositionNormalTexture(new Vector3(-1.0f, -1.0f, -1.0f), new Vector3(-1.0f, -1.0f, -1.0f), Vector2.Zero);
            vertices[7] = new VertexPositionNormalTexture(new Vector3(-1.0f, 1.0f, -1.0f), new Vector3(-1.0f, 1.0f, -1.0f), Vector2.Zero);

            vertices[1] = new VertexPositionNormalTexture(new Vector3(1.0f, -1.0f, 1.0f), new Vector3(1.0f, -1.0f, 1.0f), Vector2.Zero);
            vertices[6] = new VertexPositionNormalTexture(new Vector3(1.0f, 1.0f, -1.0f), new Vector3(1.0f, 1.0f, -1.0f), Vector2.Zero);
            vertices[2] = new VertexPositionNormalTexture(new Vector3(1.0f, 1.0f, 1.0f), new Vector3(1.0f, 1.0f, 1.0f), Vector2.Zero);
            vertices[5] = new VertexPositionNormalTexture(new Vector3(1.0f, -1.0f, -1.0f), new Vector3(1.0f, -1.0f, -1.0f), Vector2.Zero);


            indices = new short[] {
                0, 2, 1,
                2, 0, 3,
                3, 6, 2,
                6, 3, 7,
                7, 5, 6,
                5, 7, 4,
                4, 3, 0,
                3, 4, 7,
                0, 1, 5,
                5, 4, 0,
                1, 6, 5,
                6, 1, 2
            };


            var m = new Mesh {
                Indices = indices,
                Vertices = vertices
            };
            m.CalculateBoundingBox();

            return m;
        }
    }
}