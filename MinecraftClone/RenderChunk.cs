using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Data.Voxel.Map;
using NewEngine.Engine.components;
using NewEngine.Engine.Core;
using NewEngine.Engine.Physics.PhysicsComponents;
using NewEngine.Engine.Rendering;
using OpenTK;

namespace MinecraftClone {
    public class RenderChunk : GameComponent {
        private List<Vertex> _vertices = new List<Vertex>();
        private List<Vector3> _normals = new List<Vector3>();
        private List<int> _indices = new List<int>();
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
        public Vector2 Diamond = new Vector2(2, 3);
        public float TextureUnit = 0.0625f;
        public int TextureCountX = 16;

        public float HeightFactor;

        public Vector3 Scale;

        public Vector3 ChunkPostion = new Vector3(0, 0, 0);

        public int DigDepth;

        public float NoiseScale;
        public int Seed;

        public Material Material;

        public bool HasGeneratedMesh = false;
        private bool _worldReady = false;
        private bool _meshGenerationStarted = false;


        public void CreateNewChunk() {
            Map = new Map(Seed, ChunkPostion, (int)Scale.X, (int)Scale.Y, (int)Scale.Z, HeightFactor, DigDepth, NoiseScale);
            _worldReady = true;
        }

        private void StartChunkGeneration() {
            _vertices = new List<Vertex>();
            _normals = new List<Vector3>();
            _indices = new List<int>();
            _uvs = new List<Vector2>();
            _meshGenerationStarted = true;

            for (var x = 0; x < (int)Scale.X; x++) {
                for (var y = 0; y < (int)Scale.Y; y++) {
                    for (var z = 0; z < (int)Scale.Z; z++) {
                        if (Map.GetVoxel(x, y, z) != BlockTypes.Air) {
                            if (Map.GetVoxel(x, y + 1, z) == BlockTypes.Air) {
                                SetTop(x, y, z, Map.GetVoxel(x, y, z));
                            }
                            if (Map.GetVoxel(x, y - 1, z) == BlockTypes.Air) {
                                if (Map.GetVoxel(x, y + 1, z) == BlockTypes.Air)
                                    SetBottom(x, y, z, Map.GetVoxel(x, y, z), false);
                                else
                                    SetBottom(x, y, z, Map.GetVoxel(x, y, z), true);
                            }
                            if (Map.GetVoxel(x + 1, y, z) == BlockTypes.Air) {
                                if (Map.GetVoxel(x, y + 1, z) == BlockTypes.Air)
                                    SetEast(x, y, z, Map.GetVoxel(x, y, z), false);
                                else
                                    SetEast(x, y, z, Map.GetVoxel(x, y, z), true);
                            }
                            if (Map.GetVoxel(x - 1, y, z) == BlockTypes.Air) {
                                if (Map.GetVoxel(x, y + 1, z) == BlockTypes.Air)
                                    SetWest(x, y, z, Map.GetVoxel(x, y, z), false);
                                else
                                    SetWest(x, y, z, Map.GetVoxel(x, y, z), true);
                            }
                            if (Map.GetVoxel(x, y, z + 1) == BlockTypes.Air) {
                                if (Map.GetVoxel(x, y + 1, z) == BlockTypes.Air)
                                    SetNorth(x, y, z, Map.GetVoxel(x, y, z), false);
                                else
                                    SetNorth(x, y, z, Map.GetVoxel(x, y, z), true);
                            }
                            if (Map.GetVoxel(x, y, z - 1) == BlockTypes.Air) {
                                if (Map.GetVoxel(x, y + 1, z) == BlockTypes.Air)
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

            Vector2 texturePos = GetTexturePosForType(type, false, false, true);

            _vertices.Add(new Vertex(new Vector3(x, y, z + 1), new Vector2(TextureUnit * texturePos.X, (TextureUnit * texturePos.Y + TextureUnit)), Vector3.UnitY));
            _vertices.Add(new Vertex(new Vector3(x + 1, y, z + 1), new Vector2(TextureUnit * texturePos.X, (TextureUnit * texturePos.Y)), Vector3.UnitY));
            _vertices.Add(new Vertex(new Vector3(x + 1, y, z), new Vector2(TextureUnit * texturePos.X + TextureUnit, (TextureUnit * texturePos.Y)), Vector3.UnitY));
            _vertices.Add(new Vertex(new Vector3(x, y, z), new Vector2(TextureUnit * texturePos.X + TextureUnit, (TextureUnit * texturePos.Y + TextureUnit)), Vector3.UnitY));
            Cube();
        }

        void SetNorth(int x, int y, int z, BlockTypes type, bool hasBlockAbove) {
            Vector2 texturePos = GetTexturePosForType(type, hasBlockAbove);
            _vertices.Add(new Vertex(new Vector3(x + 1, y - 1, z + 1), new Vector2(TextureUnit * texturePos.X, (TextureUnit * texturePos.Y + TextureUnit)), Vector3.UnitX));
            _vertices.Add(new Vertex(new Vector3(x + 1, y, z + 1), new Vector2(TextureUnit * texturePos.X, (TextureUnit * texturePos.Y)), Vector3.UnitX));
            _vertices.Add(new Vertex(new Vector3(x, y, z + 1), new Vector2(TextureUnit * texturePos.X + TextureUnit, (TextureUnit * texturePos.Y)), Vector3.UnitX));
            _vertices.Add(new Vertex(new Vector3(x, y - 1, z + 1), new Vector2(TextureUnit * texturePos.X + TextureUnit, (TextureUnit * texturePos.Y + TextureUnit)), Vector3.UnitX));

            Cube();
        }

        void SetEast(int x, int y, int z, BlockTypes type, bool hasBlockAbove) {
            Vector2 texturePos = GetTexturePosForType(type, hasBlockAbove);
            _vertices.Add(new Vertex(new Vector3(x + 1, y - 1, z), new Vector2(TextureUnit * texturePos.X, (TextureUnit * texturePos.Y + TextureUnit)), Vector3.UnitZ));
            _vertices.Add(new Vertex(new Vector3(x + 1, y, z), new Vector2(TextureUnit * texturePos.X, (TextureUnit * texturePos.Y)), Vector3.UnitZ));
            _vertices.Add(new Vertex(new Vector3(x + 1, y, z + 1), new Vector2(TextureUnit * texturePos.X + TextureUnit, (TextureUnit * texturePos.Y)), Vector3.UnitZ));
            _vertices.Add(new Vertex(new Vector3(x + 1, y - 1, z + 1), new Vector2(TextureUnit * texturePos.X + TextureUnit, (TextureUnit * texturePos.Y + TextureUnit)), Vector3.UnitZ));
            Cube();
        }

        void SetSouth(int x, int y, int z, BlockTypes type, bool hasBlockAbove) {
            Vector2 texturePos = GetTexturePosForType(type, hasBlockAbove);

            _vertices.Add(new Vertex(new Vector3(x, y - 1, z), new Vector2(TextureUnit * texturePos.X, (TextureUnit * texturePos.Y + TextureUnit)), -Vector3.UnitX));
            _vertices.Add(new Vertex(new Vector3(x, y, z), new Vector2(TextureUnit * texturePos.X, (TextureUnit * texturePos.Y)), -Vector3.UnitX));
            _vertices.Add(new Vertex(new Vector3(x + 1, y, z), new Vector2(TextureUnit * texturePos.X + TextureUnit, (TextureUnit * texturePos.Y)), -Vector3.UnitX));
            _vertices.Add(new Vertex(new Vector3(x + 1, y - 1, z), new Vector2(TextureUnit * texturePos.X + TextureUnit, (TextureUnit * texturePos.Y + TextureUnit)), -Vector3.UnitX));
            Cube();
        }

        void SetWest(int x, int y, int z, BlockTypes type, bool hasBlockAbove) {
            Vector2 texturePos = GetTexturePosForType(type, hasBlockAbove);

            _vertices.Add(new Vertex(new Vector3(x, y - 1, z + 1), new Vector2(TextureUnit * texturePos.X, (TextureUnit * texturePos.Y + TextureUnit)), -Vector3.UnitZ));
            _vertices.Add(new Vertex(new Vector3(x, y, z + 1), new Vector2(TextureUnit * texturePos.X, (TextureUnit * texturePos.Y)), -Vector3.UnitZ));
            _vertices.Add(new Vertex(new Vector3(x, y, z), new Vector2(TextureUnit * texturePos.X + TextureUnit, (TextureUnit * texturePos.Y)), -Vector3.UnitZ));
            _vertices.Add(new Vertex(new Vector3(x, y - 1, z), new Vector2(TextureUnit * texturePos.X + TextureUnit, (TextureUnit * texturePos.Y + TextureUnit)), -Vector3.UnitZ));
            Cube();
        }

        void SetBottom(int x, int y, int z, BlockTypes type, bool hasBlockAbove) {
            Vector2 texturePos = GetTexturePosForType(type, hasBlockAbove, true);

            _vertices.Add(new Vertex(new Vector3(x, y - 1, z), new Vector2(TextureUnit * texturePos.X, (TextureUnit * texturePos.Y + TextureUnit)), -Vector3.UnitY));
            _vertices.Add(new Vertex(new Vector3(x + 1, y - 1, z), new Vector2(TextureUnit * texturePos.X, (TextureUnit * texturePos.Y)), -Vector3.UnitY));
            _vertices.Add(new Vertex(new Vector3(x + 1, y - 1, z + 1), new Vector2(TextureUnit * texturePos.X + TextureUnit, (TextureUnit * texturePos.Y)), -Vector3.UnitY));
            _vertices.Add(new Vertex(new Vector3(x, y - 1, z + 1), new Vector2(TextureUnit * texturePos.X + TextureUnit, (TextureUnit * texturePos.Y + TextureUnit)), -Vector3.UnitY));
            Cube();
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
            if (type == BlockTypes.Diamond) {
                return Diamond;
            }


            return new Vector2(0, 0);
        }

        void Cube() {
            _indices.Add((short)(_faceCount * 4 + 2)); //3
            _indices.Add((short)(_faceCount * 4)); //1
            _indices.Add((short)(_faceCount * 4 + 1)); //2
            _indices.Add((short)(_faceCount * 4 + 3)); //4
            _indices.Add((short)(_faceCount * 4)); //1
            _indices.Add((short)(_faceCount * 4 + 2)); //3

            _faceCount++; // Add this line
        }

        void UpdateMesh() {

            _mesh = new Mesh(_vertices.ToArray(), _indices.ToArray(), false);

            Parent.AddComponent(new MeshRenderer(_mesh, Material));
            Parent.AddComponent(new MeshCollider(_vertices.ToArray(), _indices.ToArray()));

            _faceCount = 0;
            HasGeneratedMesh = true;
        }

        public override void Update() {
            if (_worldReady && HasGeneratedMesh == false) {
                StartChunkGeneration();
            }
        }
    }
}
