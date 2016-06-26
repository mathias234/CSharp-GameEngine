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
            var vertices = new Vector3[12];

            // vertex position and color information for icosahedron
            vertices[0] = new Vector3(-0.26286500f, 0.0000000f, 0.42532500f);
            vertices[1] = new Vector3(0.26286500f, 0.0000000f, 0.42532500f);
            vertices[2] = new Vector3(-0.26286500f, 0.0000000f, -0.42532500f);
            vertices[3] = new Vector3(0.26286500f, 0.0000000f, -0.42532500f);
            vertices[4] = new Vector3(0.0000000f, 0.42532500f, 0.26286500f);
            vertices[5] = new Vector3(0.0000000f, 0.42532500f, -0.26286500f);
            vertices[6] = new Vector3(0.0000000f, -0.42532500f, 0.26286500f);
            vertices[7] = new Vector3(0.0000000f, -0.42532500f, -0.26286500f);
            vertices[8] = new Vector3(0.42532500f, 0.26286500f, 0.0000000f);
            vertices[9] = new Vector3(-0.42532500f, 0.26286500f, 0.0000000f);
            vertices[10] = new Vector3(0.42532500f, -0.26286500f, 0.0000000f);
            vertices[11] = new Vector3(-0.42532500f, -0.26286500f, 0.0000000f);

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
            var vertices = new Vector3[4];

            vertices[0] = new Vector3(-1.0f, 0, -1.0f);
            vertices[1] = new Vector3(1.0f, 0, -1.0f);
            vertices[2] = new Vector3(1.0f, 0, 1.0f);
            vertices[3] = new Vector3(-1.0f, 0, 1.0f);

            var normals = new Vector3[4];
            normals[0] = Vector3.Up;
            normals[1] = Vector3.Up;
            normals[2] = Vector3.Up;
            normals[3] = Vector3.Up;

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
            var vertices = new Vector3[8];
            var indices = new short[18];

            vertices[0] = new Vector3(-1.0f, -1.0f, 1.0f);
            vertices[3] = new Vector3(-1.0f, 1.0f, 1.0f);
            vertices[4] = new Vector3(-1.0f, -1.0f, -1.0f);
            vertices[7] = new Vector3(-1.0f, 1.0f, -1.0f);

            vertices[1] = new Vector3(1.0f, -1.0f, 1.0f);
            vertices[6] = new Vector3(1.0f, 1.0f, -1.0f);
            vertices[2] = new Vector3(1.0f, 1.0f, 1.0f);
            vertices[5] = new Vector3(1.0f, -1.0f, -1.0f);

            var normals = new Vector3[8];
            normals[0] = Vector3.Up;
            normals[1] = Vector3.Up;
            normals[2] = Vector3.Up;
            normals[3] = Vector3.Up;
            normals[4] = Vector3.Up;
            normals[5] = Vector3.Up;
            normals[6] = Vector3.Up;
            normals[7] = Vector3.Up;


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
                Vertices = vertices,
                Normals = normals,
            };
            m.CalculateBoundingBox();

            return m;
        }
    }
}