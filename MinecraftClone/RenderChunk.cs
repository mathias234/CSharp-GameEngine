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
using MonoGameEngine.Engine.Physics;

namespace MinecraftClone {
    public class RenderChunk : Component {
        private List<Vector3> _vertices = new List<Vector3>();
        private List<Vector3> _normals = new List<Vector3>();
        private List<short> _indices = new List<short>();
        private List<Vector2> _uvs = new List<Vector2>();

        public Map Map;

        private short _faceCount;

        private Mesh _mesh;


        public Vector2 GrassTop = new Vector2(0, 0);
        public Vector2 Dirt = new Vector2(2, 0);
        public Vector2 GrassSide = new Vector2(3, 0);
        public Vector2 Lava = new Vector2(15, 15);
        public Vector2 Water = new Vector2(15, 13);
        public Vector2 Stone = new Vector2(1, 0);
        public Vector2 Sand = new Vector2(2, 1);
        public float TextureUnit = 0.0625f;
        public int TextureCountX = 16;

        public float HeightFactor;

        public Vector3 Scale;

        public Vector3 ChunkPostion = new Vector3(0, 0, 0);

        public int DigDepth;

        public float NoiseScale;
        public int Seed;

        public Texture2D MainTexture;

        public bool HasGeneratedMesh = false;
        private bool _worldReady = false;
        private bool _meshGenerationStarted = false;


        public void CreateNewChunk() {
            Map = new Map(Seed, ChunkPostion, (int)Scale.X, (int)Scale.Y, (int)Scale.Z, HeightFactor, DigDepth, NoiseScale);
            _worldReady = true;
        }

        private void StartChunkGeneration() {
            _vertices = new List<Vector3>();
            _normals = new List<Vector3>();
            _indices = new List<short>();
            _uvs = new List<Vector2>();
            _meshGenerationStarted = true;

            for (var x = 0; x < (int)Scale.X; x++) {
                for (var y = 0; y < (int)Scale.Y; y++) {
                    for (var z = 0; z < (int)Scale.Z; z++) {
                        if (Map.GetVoxel(x, y, z) != 0) {
                            if (Map.GetVoxel(x, y + 1, z) == 0) {
                                SetTop(x, y, z, Map.GetVoxel(x, y, z));
                            }
                            if (Map.GetVoxel(x, y - 1, z) == 0) {
                                if (Map.GetVoxel(x, y + 1, z) == 0)
                                    SetBottom(x, y, z, Map.GetVoxel(x, y, z), false);
                                else
                                    SetBottom(x, y, z, Map.GetVoxel(x, y, z), true);
                            }
                            if (Map.GetVoxel(x + 1, y, z) == 0) {
                                if (Map.GetVoxel(x, y + 1, z) == 0)
                                    SetEast(x, y, z, Map.GetVoxel(x, y, z), false);
                                else
                                    SetEast(x, y, z, Map.GetVoxel(x, y, z), true);
                            }
                            if (Map.GetVoxel(x - 1, y, z) == 0) {
                                if (Map.GetVoxel(x, y + 1, z) == 0)
                                    SetWest(x, y, z, Map.GetVoxel(x, y, z), false);
                                else
                                    SetWest(x, y, z, Map.GetVoxel(x, y, z), true);
                            }
                            if (Map.GetVoxel(x, y, z + 1) == 0) {
                                if (Map.GetVoxel(x, y + 1, z) == 0)
                                    SetNorth(x, y, z, Map.GetVoxel(x, y, z), false);
                                else
                                    SetNorth(x, y, z, Map.GetVoxel(x, y, z), true);
                            }
                            if (Map.GetVoxel(x, y, z - 1) == 0) {
                                if (Map.GetVoxel(x, y + 1, z) == 0)
                                    SetSouth(x, y, z, Map.GetVoxel(x, y, z), false);
                                else
                                    SetSouth(x, y, z, Map.GetVoxel(x, y, z), true);
                            }
                        }
                    }
                }
            }
            UpdateMesh();
        }

        void SetTop(int x, int y, int z, BlockTypes type) {
            _vertices.Add(new Vector3(x, y, z + 1));
            _vertices.Add(new Vector3(x + 1, y, z + 1));
            _vertices.Add(new Vector3(x + 1, y, z));
            _vertices.Add(new Vector3(x, y, z));

            _normals.Add(Vector3.Up);
            _normals.Add(Vector3.Up);
            _normals.Add(Vector3.Up);
            _normals.Add(Vector3.Up);

            Vector2 texturePos = GetTexturePosForType(type, false, false, true);

            Cube(texturePos);
        }

        void SetNorth(int x, int y, int z, BlockTypes type, bool hasBlockAbove) {
            _vertices.Add(new Vector3(x + 1, y - 1, z + 1));
            _vertices.Add(new Vector3(x + 1, y, z + 1));
            _vertices.Add(new Vector3(x, y, z + 1));
            _vertices.Add(new Vector3(x, y - 1, z + 1));

            _normals.Add(Vector3.UnitX);
            _normals.Add(Vector3.UnitX);
            _normals.Add(Vector3.UnitX);
            _normals.Add(Vector3.UnitX);


            Vector2 texturePos = GetTexturePosForType(type, hasBlockAbove);

            Cube(texturePos);
        }

        void SetEast(int x, int y, int z, BlockTypes type, bool hasBlockAbove) {
            _vertices.Add(new Vector3(x + 1, y - 1, z));
            _vertices.Add(new Vector3(x + 1, y, z));
            _vertices.Add(new Vector3(x + 1, y, z + 1));
            _vertices.Add(new Vector3(x + 1, y - 1, z + 1));

            _normals.Add(Vector3.Right);
            _normals.Add(Vector3.Right);
            _normals.Add(Vector3.Right);
            _normals.Add(Vector3.Right);


            Vector2 texturePos = GetTexturePosForType(type, hasBlockAbove);

            Cube(texturePos);
        }

        void SetSouth(int x, int y, int z, BlockTypes type, bool hasBlockAbove) {
            _vertices.Add(new Vector3(x, y - 1, z));
            _vertices.Add(new Vector3(x, y, z));
            _vertices.Add(new Vector3(x + 1, y, z));
            _vertices.Add(new Vector3(x + 1, y - 1, z));

            _normals.Add(-Vector3.UnitX);
            _normals.Add(-Vector3.UnitX);
            _normals.Add(-Vector3.UnitX);
            _normals.Add(-Vector3.UnitX);


            Vector2 texturePos = GetTexturePosForType(type, hasBlockAbove);


            Cube(texturePos);
        }

        void SetWest(int x, int y, int z, BlockTypes type, bool hasBlockAbove) {
            _vertices.Add(new Vector3(x, y - 1, z + 1));
            _vertices.Add(new Vector3(x, y, z + 1));
            _vertices.Add(new Vector3(x, y, z));
            _vertices.Add(new Vector3(x, y - 1, z));

            _normals.Add(-Vector3.Right);
            _normals.Add(-Vector3.Right);
            _normals.Add(-Vector3.Right);
            _normals.Add(-Vector3.Right);


            Vector2 texturePos = GetTexturePosForType(type, hasBlockAbove);


            Cube(texturePos);
        }

        void SetBottom(int x, int y, int z, BlockTypes type, bool hasBlockAbove) {
            _vertices.Add(new Vector3(x, y - 1, z));
            _vertices.Add(new Vector3(x + 1, y - 1, z));
            _vertices.Add(new Vector3(x + 1, y - 1, z + 1));
            _vertices.Add(new Vector3(x, y - 1, z + 1));

            _normals.Add(Vector3.Down);
            _normals.Add(Vector3.Down);
            _normals.Add(Vector3.Down);
            _normals.Add(Vector3.Down);


            Vector2 texturePos = GetTexturePosForType(type, hasBlockAbove, true);
            Cube(texturePos);
        }

        Vector2 GetTexturePosForType(BlockTypes type, bool hasBlockAbove = true, bool isBottom = false, bool isTop = false) {
            // 1 = grass
            // 2 = stone
            // 3 = lava

            if (type == BlockTypes.Grass) {
                if (isTop)
                    return GrassTop;
                if (isBottom)
                    return Dirt;
                if (hasBlockAbove)
                    return Dirt;

                return GrassSide;
            }
            if (type == BlockTypes.Stone) {
                return Stone;
            }
            if (type == BlockTypes.Water)
                return Water;
            if (type == BlockTypes.Sand) {
                return Sand;
            }


            return new Vector2(0, 0);
        }

        void Cube(Vector2 texturePos) {
            _indices.Add((short)(_faceCount * 4)); //1
            _indices.Add((short)(_faceCount * 4 + 2)); //3
            _indices.Add((short)(_faceCount * 4 + 1)); //2
            _indices.Add((short)(_faceCount * 4)); //1
            _indices.Add((short)(_faceCount * 4 + 3)); //4
            _indices.Add((short)(_faceCount * 4 + 2)); //3


            _uvs.Add(new Vector2(TextureUnit * texturePos.X, (TextureUnit * texturePos.Y + TextureUnit)));
            _uvs.Add(new Vector2(TextureUnit * texturePos.X, (TextureUnit * texturePos.Y)));
            _uvs.Add(new Vector2(TextureUnit * texturePos.X + TextureUnit, (TextureUnit * texturePos.Y)));
            _uvs.Add(new Vector2(TextureUnit * texturePos.X + TextureUnit, (TextureUnit * texturePos.Y + TextureUnit)));



            _faceCount++; // Add this line
        }

        void UpdateMesh() {
            MeshRenderer mr = GameObject.AddComponent<MeshRenderer>();
            MeshCollider mc = GameObject.AddComponent<MeshCollider>();
            _mesh = new Mesh();
            _mesh.Vertices = _vertices.ToArray();
            _mesh.Indices = _indices.ToArray();
            _mesh.Uvs = _uvs.ToArray();
            _mesh.Normals = _normals.ToArray();

            mr.Mesh = _mesh;
            mr.Color = Color.LightGray;
            mr.Texture = MainTexture;
            mr.Init();

            mc .Init();
            _faceCount = 0;
            HasGeneratedMesh = true;
        }

        public override void Update(float deltaTime) {
            if (_worldReady && HasGeneratedMesh == false) {
                StartChunkGeneration();
            }
        }
    }
}
