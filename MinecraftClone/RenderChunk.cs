using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Voxel.Map;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameEngine.Engine;
using MonoGameEngine.Engine.Components;

namespace MinecraftClone {
    class RenderChunk : Component {
        private List<VertexPositionNormalTexture> vertices = new List<VertexPositionNormalTexture>();
        private List<short> indices = new List<short>();
        private List<Vector2> uvs = new List<Vector2>();

        private Map map;

        private short faceCount;

        private Mesh mesh;

        public float TextureUnit = 0;

        public Vector2 grassTop = new Vector2(0, 0);
        public Vector2 dirt = new Vector2(2, 0);
        public Vector2 grassSide = new Vector2(3, 0);
        public Vector2 lava = new Vector2(31, 31 - 4);
        public Vector2 stonePosition = new Vector2(1, 0);


        public int TextureCountX;

        public float heightFactor;

        public Vector3 scale;

        public Vector3 ChunkPostion = new Vector3(0, 0, 0);

        public int digDepth;

        public float noiseScale;
        public int seed;


        public void StartChunkGeneration() {
            map = new Map(seed, ChunkPostion, (int)scale.X, (int)scale.Y, (int)scale.Z, heightFactor, digDepth, noiseScale);


            for (var x = 0; x < (int)scale.X; x++) {
                for (var y = 0; y < (int)scale.Y; y++) {
                    for (var z = 0; z < (int)scale.Z; z++) {
                        if (map.GetVoxel(x, y, z) != 0) {
                            if (map.GetVoxel(x, y + 1, z) == 0) {
                                SetTop(x, y, z, map.GetVoxel(x, y, z));
                            }
                            if (map.GetVoxel(x, y - 1, z) == 0) {
                                if (map.GetVoxel(x, y + 1, z) == 0)
                                    SetBottom(x, y, z, map.GetVoxel(x, y, z), false);
                                else
                                    SetBottom(x, y, z, map.GetVoxel(x, y, z), true);
                            }
                            if (map.GetVoxel(x + 1, y, z) == 0) {
                                if (map.GetVoxel(x, y + 1, z) == 0)
                                    SetEast(x, y, z, map.GetVoxel(x, y, z), false);
                                else
                                    SetEast(x, y, z, map.GetVoxel(x, y, z), true);
                            }
                            if (map.GetVoxel(x - 1, y, z) == 0) {
                                if (map.GetVoxel(x, y + 1, z) == 0)
                                    SetWest(x, y, z, map.GetVoxel(x, y, z), false);
                                else
                                    SetWest(x, y, z, map.GetVoxel(x, y, z), true);
                            }
                            if (map.GetVoxel(x, y, z + 1) == 0) {
                                if (map.GetVoxel(x, y + 1, z) == 0)
                                    SetNorth(x, y, z, map.GetVoxel(x, y, z), false);
                                else
                                    SetNorth(x, y, z, map.GetVoxel(x, y, z), true);
                            }
                            if (map.GetVoxel(x, y, z - 1) == 0) {
                                if (map.GetVoxel(x, y + 1, z) == 0)
                                    SetSouth(x, y, z, map.GetVoxel(x, y, z), false);
                                else
                                    SetSouth(x, y, z, map.GetVoxel(x, y, z), true);
                            }
                        }
                    }
                }
            }
            UpdateMesh();
        }

        void SetTop(int x, int y, int z, byte type) {
            vertices.Add(new VertexPositionNormalTexture(new Vector3(x,y,z+1), Vector3.Up, new Vector2(0,0)));
            vertices.Add(new VertexPositionNormalTexture(new Vector3(x + 1, y, z + 1), Vector3.Up, new Vector2(1, 0)));
            vertices.Add(new VertexPositionNormalTexture(new Vector3(x + 1, y, z), Vector3.Up, new Vector2(1, 1)));
            vertices.Add(new VertexPositionNormalTexture(new Vector3(x, y, z), Vector3.Up, new Vector2(0, 1)));

            Vector2 texturePos = GetTexturePosForType(type, false, false, true);

            Cube(texturePos);
        }

        void SetNorth(int x, int y, int z, byte type, bool hasBlockAbove) {
            vertices.Add(new VertexPositionNormalTexture(new Vector3(x + 1, y - 1, z + 1), Vector3.UnitY, new Vector2(0, 0)));
            vertices.Add(new VertexPositionNormalTexture(new Vector3(x + 1, y, z + 1), Vector3.UnitY, new Vector2(1, 0)));
            vertices.Add(new VertexPositionNormalTexture(new Vector3(x, y, z + 1), Vector3.UnitY, new Vector2(1, 1)));
            vertices.Add(new VertexPositionNormalTexture(new Vector3(x, y - 1, z + 1), Vector3.UnitY, new Vector2(0, 1)));

            Vector2 texturePos = GetTexturePosForType(type, hasBlockAbove);

            Cube(texturePos);
        }

        void SetEast(int x, int y, int z, byte type, bool hasBlockAbove) {
            vertices.Add(new VertexPositionNormalTexture(new Vector3(x + 1, y - 1, z), Vector3.Left, new Vector2(0, 1)));
            vertices.Add(new VertexPositionNormalTexture(new Vector3(x + 1, y, z), Vector3.Left, new Vector2(0, 1)));
            vertices.Add(new VertexPositionNormalTexture(new Vector3(x + 1, y, z + 1), Vector3.Left, new Vector2(0, 1)));
            vertices.Add(new VertexPositionNormalTexture(new Vector3(x + 1, y - 1, z + 1), Vector3.Left, new Vector2(0, 1)));


            Vector2 texturePos = GetTexturePosForType(type, hasBlockAbove);

            Cube(texturePos);
        }

        void SetSouth(int x, int y, int z, byte type, bool hasBlockAbove) {
            vertices.Add(new VertexPositionNormalTexture(new Vector3(x, y - 1, z), Vector3.Left, new Vector2(0, 1)));
            vertices.Add(new VertexPositionNormalTexture(new Vector3(x, y, z), Vector3.Left, new Vector2(0, 1)));
            vertices.Add(new VertexPositionNormalTexture(new Vector3(x + 1, y, z), Vector3.Left, new Vector2(0, 1)));
            vertices.Add(new VertexPositionNormalTexture(new Vector3(x + 1, y - 1, z), Vector3.Left, new Vector2(0, 1)));

            Vector2 texturePos = GetTexturePosForType(type, hasBlockAbove);


            Cube(texturePos);
        }

        void SetWest(int x, int y, int z, byte type, bool hasBlockAbove) {
            vertices.Add(new VertexPositionNormalTexture(new Vector3(x, y - 1, z + 1), Vector3.Left, new Vector2(0, 1)));
            vertices.Add(new VertexPositionNormalTexture(new Vector3(x, y, z + 1), Vector3.Left, new Vector2(0, 1)));
            vertices.Add(new VertexPositionNormalTexture(new Vector3(x, y, z), Vector3.Left, new Vector2(0, 1)));
            vertices.Add(new VertexPositionNormalTexture(new Vector3(x, y - 1, z), Vector3.Left, new Vector2(0, 1)));


            Vector2 texturePos = GetTexturePosForType(type, hasBlockAbove);


            Cube(texturePos);
        }

        void SetBottom(int x, int y, int z, byte type, bool hasBlockAbove) {
            vertices.Add(new VertexPositionNormalTexture(new Vector3(x, y - 1, z), Vector3.Left, new Vector2(0, 1)));
            vertices.Add(new VertexPositionNormalTexture(new Vector3(x + 1, y - 1, z), Vector3.Left, new Vector2(0, 1)));
            vertices.Add(new VertexPositionNormalTexture(new Vector3(x + 1, y - 1, z + 1), Vector3.Left, new Vector2(0, 1)));
            vertices.Add(new VertexPositionNormalTexture(new Vector3(x, y - 1, z + 1), Vector3.Left, new Vector2(0, 1)));

            Vector2 texturePos = GetTexturePosForType(type, hasBlockAbove, true);
            Cube(texturePos);
        }

        Vector2 GetTexturePosForType(byte type, bool hasBlockAbove = true, bool isBottom = false, bool isTop = false) {
            // 1 = grass
            // 2 = stone
            // 3 = lava

            if (type == 1) {
                if (isTop)
                    return grassTop;
                if (isBottom)
                    return dirt;
                if (hasBlockAbove)
                    return dirt;

                return grassSide;
            }
            if (type == 2) {
                return stonePosition;
            }
            if (type == 3)
                return lava;


            return new Vector2(0, 0);
        }

        void Cube(Vector2 texturePos) {
            indices.Add((short) (faceCount * 4)); //1
            indices.Add((short) (faceCount * 4 + 2)); //3
            indices.Add((short) (faceCount * 4 + 1)); //2
            indices.Add((short) (faceCount * 4)); //1
            indices.Add((short) (faceCount * 4 + 3)); //4
            indices.Add((short) (faceCount * 4 + 2)); //3


            uvs.Add(new Vector2(TextureUnit * texturePos.X, TextureCountX - (TextureUnit * texturePos.Y + TextureUnit)));
            uvs.Add(new Vector2(TextureUnit * texturePos.X, TextureCountX - (TextureUnit * texturePos.Y)));
            uvs.Add(new Vector2(TextureUnit * texturePos.X + TextureUnit, TextureCountX - (TextureUnit * texturePos.Y)));
            uvs.Add(new Vector2(TextureUnit * texturePos.X + TextureUnit, TextureCountX - (TextureUnit * texturePos.Y + TextureUnit)));



            faceCount++; // Add this line
        }

        void OnDrawGizmosSelected() {

        }

        void UpdateMesh() {
           MeshRenderer mr = GameObject.AddComponent<MeshRenderer>();
            mesh = new Mesh();
            mesh.Vertices = vertices.ToArray();
            mesh.Indices = indices.ToArray();
            mr.Mesh = mesh;

            vertices.Clear();
            indices.Clear();
            faceCount = 0;
        }
    }
}
