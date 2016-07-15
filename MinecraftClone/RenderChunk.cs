using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Data.Voxel;
using Data.Voxel.Map;
using NewEngine;
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

        public bool HasGeneratedMesh;
        private bool _worldReady;


        public void CreateNewChunk() {
            Map = new Map(Seed, ChunkPostion, (int)Scale.X, (int)Scale.Y, (int)Scale.Z, HeightFactor, DigDepth,
                NoiseScale);

            _worldReady = true;
            //Thread thread = new Thread(ThreadedUpdate);
            //thread.Start();
        }

        private void StartChunkGeneration() {
            _vertices = new List<Vertex>();
            _normals = new List<Vector3>();
            _indices = new List<int>();
            _uvs = new List<Vector2>();

            for (var x = 0; x < (int)Scale.X; x++) {
                for (var y = 0; y < (int)Scale.Y; y++) {
                    for (var z = 0; z < (int)Scale.Z; z++) {
                        if (Map.GetVoxel(x,y,z).Type != BlockTypes.Air) {
                            if (CanSeeThrough(x, y + 1, z)) {
                                SetTop(x, y, z, Map.GetVoxel(x, y, z));
                            }
                            if (CanSeeThrough(x, y - 1, z)) {
                                if (CanSeeThrough(x, y + 1, z))
                                    SetBottom(x, y, z, Map.GetVoxel(x, y, z), false);
                                else
                                    SetBottom(x, y, z, Map.GetVoxel(x, y, z), true);
                            }
                            if (CanSeeThrough(x + 1, y, z)) {
                                if (CanSeeThrough(x, y + 1, z))
                                    SetEast(x, y, z, Map.GetVoxel(x, y, z), false);
                                else
                                    SetEast(x, y, z, Map.GetVoxel(x, y, z), true);
                            }
                            if (CanSeeThrough(x - 1, y, z)) {
                                if (CanSeeThrough(x, y + 1, z))
                                    SetWest(x, y, z, Map.GetVoxel(x, y, z), false);
                                else
                                    SetWest(x, y, z, Map.GetVoxel(x, y, z), true);
                            }
                            if (CanSeeThrough(x, y, z + 1)) {
                                if (CanSeeThrough(x, y + 1, z))
                                    SetNorth(x, y, z, Map.GetVoxel(x, y, z), false);
                                else
                                    SetNorth(x, y, z, Map.GetVoxel(x, y, z), true);
                            }
                            if (CanSeeThrough(x, y, z - 1)) {
                                if (CanSeeThrough(x, y + 1, z))
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

        public bool CanSeeThrough(int x, int y, int z) {
            if (Map.GetVoxel(x, y, z).Type == BlockTypes.Air || (Map.GetVoxel(x,y,z) .Type== BlockTypes.Water))
                return true;

            return false;
        }

        public float InvertedMass(int x, int y, int z) {
            return 1 - Map.GetVoxel(x, y, z).Mass;
        }

        void SetTop(int x, int y, int z, Voxel voxel) {
            float waterHeight = (voxel.Type != BlockTypes.Water) ? 0.0f : InvertedMass(x, y, z);

            Vector2 texturePos = GetTexturePosForType(voxel.Type, false, false, true);

            _vertices.Add(new Vertex(new Vector3(x, y - waterHeight, z + 1),
                new Vector2(TextureUnit * texturePos.X, (TextureUnit * texturePos.Y + TextureUnit)), Vector3.UnitY));
            _vertices.Add(new Vertex(new Vector3(x + 1, y - waterHeight, z + 1),
                new Vector2(TextureUnit * texturePos.X, (TextureUnit * texturePos.Y)), Vector3.UnitY));
            _vertices.Add(new Vertex(new Vector3(x + 1, y - waterHeight, z),
                new Vector2(TextureUnit * texturePos.X + TextureUnit, (TextureUnit * texturePos.Y)), Vector3.UnitY));
            _vertices.Add(new Vertex(new Vector3(x, y - waterHeight, z),
                new Vector2(TextureUnit * texturePos.X + TextureUnit, (TextureUnit * texturePos.Y + TextureUnit)),
                Vector3.UnitY));


            Cube();
        }

        void SetNorth(int x, int y, int z, Voxel voxel, bool hasBlockAbove) {
            float waterHeight = (voxel.Type != BlockTypes.Water) ? 0.0f : InvertedMass(x, y, z);

            Vector2 texturePos = GetTexturePosForType(voxel.Type, hasBlockAbove);
            _vertices.Add(new Vertex(new Vector3(x + 1, y - 1, z + 1),
                new Vector2(TextureUnit * texturePos.X, (TextureUnit * texturePos.Y + TextureUnit)), Vector3.UnitX));
            _vertices.Add(new Vertex(new Vector3(x + 1, y - waterHeight, z + 1),
                new Vector2(TextureUnit * texturePos.X, (TextureUnit * texturePos.Y)), Vector3.UnitX));
            _vertices.Add(new Vertex(new Vector3(x, y - waterHeight, z + 1),
                new Vector2(TextureUnit * texturePos.X + TextureUnit, (TextureUnit * texturePos.Y)), Vector3.UnitX));
            _vertices.Add(new Vertex(new Vector3(x, y - 1, z + 1),
                new Vector2(TextureUnit * texturePos.X + TextureUnit, (TextureUnit * texturePos.Y + TextureUnit)),
                Vector3.UnitX));

            Cube();
        }

        void SetEast(int x, int y, int z, Voxel voxel, bool hasBlockAbove) {
            float waterHeight = (voxel.Type != BlockTypes.Water) ? 0.0f : InvertedMass(x, y, z);

            Vector2 texturePos = GetTexturePosForType(voxel.Type, hasBlockAbove);


            _vertices.Add(new Vertex(new Vector3(x + 1, y - 1, z),
                new Vector2(TextureUnit * texturePos.X, (TextureUnit * texturePos.Y + TextureUnit)), Vector3.UnitZ));
            _vertices.Add(new Vertex(new Vector3(x + 1, y - waterHeight, z),
                new Vector2(TextureUnit * texturePos.X, (TextureUnit * texturePos.Y)), Vector3.UnitZ));
            _vertices.Add(new Vertex(new Vector3(x + 1, y - waterHeight, z + 1),
                new Vector2(TextureUnit * texturePos.X + TextureUnit, (TextureUnit * texturePos.Y)), Vector3.UnitZ));
            _vertices.Add(new Vertex(new Vector3(x + 1, y - 1, z + 1),
                new Vector2(TextureUnit * texturePos.X + TextureUnit, (TextureUnit * texturePos.Y + TextureUnit)),
                Vector3.UnitZ));


            Cube();
        }

        void SetSouth(int x, int y, int z, Voxel voxel, bool hasBlockAbove) {
            float waterHeight = (voxel.Type != BlockTypes.Water) ? 0.0f : InvertedMass(x, y, z);

            Vector2 texturePos = GetTexturePosForType(voxel.Type, hasBlockAbove);

            _vertices.Add(new Vertex(new Vector3(x, y - 1, z),
                new Vector2(TextureUnit * texturePos.X, (TextureUnit * texturePos.Y + TextureUnit)), -Vector3.UnitX));
            _vertices.Add(new Vertex(new Vector3(x, y- waterHeight, z),
                new Vector2(TextureUnit * texturePos.X, (TextureUnit * texturePos.Y)), -Vector3.UnitX));
            _vertices.Add(new Vertex(new Vector3(x + 1, y - waterHeight, z),
                new Vector2(TextureUnit * texturePos.X + TextureUnit, (TextureUnit * texturePos.Y)), -Vector3.UnitX));
            _vertices.Add(new Vertex(new Vector3(x + 1, y - 1, z),
                new Vector2(TextureUnit * texturePos.X + TextureUnit, (TextureUnit * texturePos.Y + TextureUnit)),
                -Vector3.UnitX));
            Cube();
        }

        void SetWest(int x, int y, int z, Voxel voxel, bool hasBlockAbove) {
            float waterHeight = (voxel.Type != BlockTypes.Water) ? 0.0f : InvertedMass(x, y, z);


            Vector2 texturePos = GetTexturePosForType(voxel.Type, hasBlockAbove);

            _vertices.Add(new Vertex(new Vector3(x, y - 1, z + 1),
                new Vector2(TextureUnit * texturePos.X, (TextureUnit * texturePos.Y + TextureUnit)), -Vector3.UnitZ));
            _vertices.Add(new Vertex(new Vector3(x, y - waterHeight, z + 1),
                new Vector2(TextureUnit * texturePos.X, (TextureUnit * texturePos.Y)), -Vector3.UnitZ));
            _vertices.Add(new Vertex(new Vector3(x, y - waterHeight, z),
                new Vector2(TextureUnit * texturePos.X + TextureUnit, (TextureUnit * texturePos.Y)), -Vector3.UnitZ));
            _vertices.Add(new Vertex(new Vector3(x, y - 1, z),
                new Vector2(TextureUnit * texturePos.X + TextureUnit, (TextureUnit * texturePos.Y + TextureUnit)),
                -Vector3.UnitZ));
            Cube();
        }

        void SetBottom(int x, int y, int z, Voxel voxel, bool hasBlockAbove) {
            Vector2 texturePos = GetTexturePosForType(voxel.Type, hasBlockAbove, true);

            _vertices.Add(new Vertex(new Vector3(x, y - 1, z),
                new Vector2(TextureUnit * texturePos.X, (TextureUnit * texturePos.Y + TextureUnit)), -Vector3.UnitY));
            _vertices.Add(new Vertex(new Vector3(x + 1, y - 1, z),
                new Vector2(TextureUnit * texturePos.X, (TextureUnit * texturePos.Y)), -Vector3.UnitY));
            _vertices.Add(new Vertex(new Vector3(x + 1, y - 1, z + 1),
                new Vector2(TextureUnit * texturePos.X + TextureUnit, (TextureUnit * texturePos.Y)), -Vector3.UnitY));
            _vertices.Add(new Vertex(new Vector3(x, y - 1, z + 1),
                new Vector2(TextureUnit * texturePos.X + TextureUnit, (TextureUnit * texturePos.Y + TextureUnit)),
                -Vector3.UnitY));
            Cube();
        }

        Vector2 GetTexturePosForType(BlockTypes type, bool hasBlockAbove = true, bool isBottom = false,
            bool isTop = false) {
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

            if (Parent.GetComponent<MeshRenderer>() != null)
                Parent.ClearComponent(Parent.GetComponent<MeshRenderer>());

            Parent.AddComponent(new MeshRenderer(_mesh, Material));

            if (Parent.GetComponent<MeshCollider>() != null)
                Parent.ClearComponent(Parent.GetComponent<MeshCollider>());

            Parent.AddComponent(new MeshCollider(_vertices.ToArray(), _indices.ToArray()));

            _faceCount = 0;
            HasGeneratedMesh = true;
        }

        public int updatesSinceLastSimulation;

        public override void Update(float deltaTime) {
            if (_worldReady && HasGeneratedMesh == false) {
                StartChunkGeneration();
            }
            if (updatesSinceLastSimulation == 30) {
                updatesSinceLastSimulation = 0;
            }
            updatesSinceLastSimulation++;

        }
        // WATER SIMULATION

        public void ThreadedUpdate() {
            //while (true) {
            //    // since this loop is insainly fast it needs a huge number so we dont update tooo often

            //}
        }

    }
}
