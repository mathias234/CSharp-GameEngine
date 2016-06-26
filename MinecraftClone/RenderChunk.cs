using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Data.Voxel.Map;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameEngine.Engine;
using MonoGameEngine.Engine.Components;

namespace MinecraftClone {
    public class RenderChunk : Component {
        private List<Vector3> vertices = new List<Vector3>();
        private List<Vector3> normals = new List<Vector3>();
        private List<short> indices = new List<short>();
        private List<Vector2> uvs = new List<Vector2>();

        private Map map;

        private short faceCount;

        private Mesh mesh;


        public Vector2 grassTop = new Vector2(0, 31);
        public Vector2 dirt = new Vector2(2, 31);
        public Vector2 grassSide = new Vector2(3, 31);
        public Vector2 lava = new Vector2(31, 4);
        public Vector2 stonePosition = new Vector2(1, 31);

        public float TextureUnit = 0.03125f;
        public int TextureCountX = 31;

        public float heightFactor;

        public Vector3 scale;

        public Vector3 ChunkPostion = new Vector3(0, 0, 0);

        public int digDepth;

        public float noiseScale;
        public int seed;

        public Texture2D mainTexture;

        private bool hasGeneratedMesh = false;
        private bool meshReady = false;

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
            meshReady = true;
        }

        void SetTop(int x, int y, int z, byte type) {
            vertices.Add(new Vector3(x, y, z + 1));
            vertices.Add(new Vector3(x + 1, y, z + 1));
            vertices.Add(new Vector3(x + 1, y, z));
            vertices.Add(new Vector3(x, y, z));

            normals.Add(Vector3.Up);
            normals.Add(Vector3.Up);
            normals.Add(Vector3.Up);
            normals.Add(Vector3.Up);

            Vector2 texturePos = GetTexturePosForType(type, false, false, true);

            Cube(texturePos);
        }

        void SetNorth(int x, int y, int z, byte type, bool hasBlockAbove) {
            vertices.Add(new Vector3(x + 1, y - 1, z + 1));
            vertices.Add(new Vector3(x + 1, y, z + 1));
            vertices.Add(new Vector3(x, y, z + 1));
            vertices.Add(new Vector3(x, y - 1, z + 1));

            normals.Add(Vector3.UnitX);
            normals.Add(Vector3.UnitX);
            normals.Add(Vector3.UnitX);
            normals.Add(Vector3.UnitX);


            Vector2 texturePos = GetTexturePosForType(type, hasBlockAbove);

            Cube(texturePos);
        }

        void SetEast(int x, int y, int z, byte type, bool hasBlockAbove) {
            vertices.Add(new Vector3(x + 1, y - 1, z));
            vertices.Add(new Vector3(x + 1, y, z));
            vertices.Add(new Vector3(x + 1, y, z + 1));
            vertices.Add(new Vector3(x + 1, y - 1, z + 1));

            normals.Add(Vector3.Right);
            normals.Add(Vector3.Right);
            normals.Add(Vector3.Right);
            normals.Add(Vector3.Right);


            Vector2 texturePos = GetTexturePosForType(type, hasBlockAbove);

            Cube(texturePos);
        }

        void SetSouth(int x, int y, int z, byte type, bool hasBlockAbove) {
            vertices.Add(new Vector3(x, y - 1, z));
            vertices.Add(new Vector3(x, y, z));
            vertices.Add(new Vector3(x + 1, y, z));
            vertices.Add(new Vector3(x + 1, y - 1, z));

            normals.Add(-Vector3.UnitX);
            normals.Add(-Vector3.UnitX);
            normals.Add(-Vector3.UnitX);
            normals.Add(-Vector3.UnitX);


            Vector2 texturePos = GetTexturePosForType(type, hasBlockAbove);


            Cube(texturePos);
        }

        void SetWest(int x, int y, int z, byte type, bool hasBlockAbove) {
            vertices.Add(new Vector3(x, y - 1, z + 1));
            vertices.Add(new Vector3(x, y, z + 1));
            vertices.Add(new Vector3(x, y, z));
            vertices.Add(new Vector3(x, y - 1, z));

            normals.Add(-Vector3.Right);
            normals.Add(-Vector3.Right);
            normals.Add(-Vector3.Right);
            normals.Add(-Vector3.Right);


            Vector2 texturePos = GetTexturePosForType(type, hasBlockAbove);


            Cube(texturePos);
        }

        void SetBottom(int x, int y, int z, byte type, bool hasBlockAbove) {
            vertices.Add(new Vector3(x, y - 1, z));
            vertices.Add(new Vector3(x + 1, y - 1, z));
            vertices.Add(new Vector3(x + 1, y - 1, z + 1));
            vertices.Add(new Vector3(x, y - 1, z + 1));

            normals.Add(Vector3.Down);
            normals.Add(Vector3.Down);
            normals.Add(Vector3.Down);
            normals.Add(Vector3.Down);


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
            indices.Add((short)(faceCount * 4)); //1
            indices.Add((short)(faceCount * 4 + 2)); //3
            indices.Add((short)(faceCount * 4 + 1)); //2
            indices.Add((short)(faceCount * 4)); //1
            indices.Add((short)(faceCount * 4 + 3)); //4
            indices.Add((short)(faceCount * 4 + 2)); //3


            uvs.Add(new Vector2(TextureUnit * texturePos.X, TextureCountX - (TextureUnit * texturePos.Y)));
            uvs.Add(new Vector2(TextureUnit * texturePos.X, TextureCountX - (TextureUnit * texturePos.Y + TextureUnit)));
            uvs.Add(new Vector2(TextureUnit * texturePos.X + TextureUnit, TextureCountX - (TextureUnit * texturePos.Y + TextureUnit)));
            uvs.Add(new Vector2(TextureUnit * texturePos.X + TextureUnit, TextureCountX - (TextureUnit * texturePos.Y)));



            faceCount++; // Add this line
        }

        void UpdateMesh() {
            MeshRenderer mr = GameObject.AddComponent<MeshRenderer>();
            mesh = new Mesh();
            mesh.Vertices = vertices.ToArray();
            mesh.Indices = indices.ToArray();
            mesh.Uvs = uvs.ToArray();
            mesh.Normals = normals.ToArray();
            mr.Mesh = mesh;
            mr.Color = Color.LightGray;
            mr.Texture = mainTexture;

            vertices.Clear();
            indices.Clear();
            faceCount = 0;
            hasGeneratedMesh = true;
        }

        public void CheckIfMeshUpdatedIsRequired() {
            if (hasGeneratedMesh == false && meshReady) {
                UpdateMesh();
                MeshRenderer mr = GameObject.GetComponent<MeshRenderer>();
                mr.Init();
            }
        }
    }
}
