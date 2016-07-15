using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using Data.Voxel;
using Data.Voxel.Map;
using NewEngine.Engine.components;
using NewEngine.Engine.Core;
using NewEngine.Engine.Physics;
using NewEngine.Engine.Physics.PhysicsComponents;
using NewEngine.Engine.Rendering;
using OpenTK;
using OpenTK.Input;
using Image = NewEngine.Engine.components.UIComponents.Image;

namespace MinecraftClone {
    public class GameCode : Game {
        public const int InitialChunkAmount = 2;

        public static Vector3 _chunkSize = new Vector3(16, 50, 16);

        private const int Height = 12;
        public const int DigDepth = 2;
        public const float NoiseScale = 5;
        public const int Seed = 2132;

        public static RenderChunk[,,] renderChunks;

        public bool hasGeneratedChunks = false;

        private Material terrainMaterial;

        private GameObject camera;

        private WaterSimulation _waterSimulation;

        public override void Start() {
            var crossHairY = new GameObject().AddComponent(new Image(new RectTransform(2.5f, 20, 1.25f, 10), new Texture((Bitmap)null)));
            var crossHairX = new GameObject().AddComponent(new Image(new RectTransform(20, 2.5f, 10, 1.25f), new Texture((Bitmap)null)));
            AddObject(crossHairY);
            AddObject(crossHairX);

            camera = new GameObject().AddComponent(new Camera(MathHelper.DegreesToRadians(70.0f),
                (float)CoreEngine.GetWidth() / CoreEngine.GetHeight(), 0.1f, 1000));
            AddObject(camera);


            terrainMaterial = new Material(new Texture("terrain.png", TextureFilter.Point), 1, 16);


            GameObject directionalLightObj = new GameObject();
            DirectionalLight directionalLight = new DirectionalLight(new Vector3(1), 0.7f);
            directionalLightObj.AddComponent(directionalLight);
            directionalLightObj.Transform.Rotation = Quaternion.FromAxisAngle(new Vector3(1, 0, 0), MathHelper.DegreesToRadians(-45));
            AddObject(directionalLightObj);

            StartWorldGen();

            Debug.WriteLine("init done");
            _waterSimulation = new WaterSimulation();
            Thread t = new Thread(_waterSimulation.Simulate);
            t.Start();
        }

        public void StartWorldGen() {
            renderChunks = new RenderChunk[3000, 1, 3000];

            for (int x = 0; x < InitialChunkAmount; x++) {
                for (int z = 0; z < InitialChunkAmount; z++) {
                    GameObject chunk1 = new GameObject();
                    chunk1.Transform.Position = new Vector3(x * _chunkSize.X - 0.5f, 0.5f, z * _chunkSize.Z - 0.5f);

                    RenderChunk rChunk1 = new RenderChunk();

                    chunk1.AddComponent(rChunk1);

                    rChunk1.Material = terrainMaterial;
                    rChunk1.ChunkPostion = new Vector3(x, 0, z);
                    rChunk1.Scale = _chunkSize;
                    rChunk1.HeightFactor = Height;
                    rChunk1.DigDepth = DigDepth;
                    rChunk1.NoiseScale = NoiseScale;
                    rChunk1.Seed = Seed;
                    rChunk1.CreateNewChunk();
                    renderChunks[x, 0, z] = rChunk1;

                    AddObject(chunk1);
                }
            }


        }

        public static Voxel GetBlockAt(int x, int y, int z) {
            return GetBlockAt(new Vector3(x, y, z));
        }

        public static Voxel GetBlockAt(Vector3 worldCoord) {
            var size = new Vector3(_chunkSize.X * (float)InitialChunkAmount,
                                _chunkSize.Y * InitialChunkAmount,
                                _chunkSize.Z * InitialChunkAmount);

            int chunkX = FloorToInt(worldCoord.X / _chunkSize.X);
            int chunkY = 0;
            int chunkZ = FloorToInt(worldCoord.Z / _chunkSize.Z);

            RenderChunk chunk = null;

            if (worldCoord.X >= size.X || worldCoord.X < 0 || worldCoord.Y >= size.Y || worldCoord.Y < 0 || worldCoord.Z >= size.Z || worldCoord.Z < 0) {
                // to optimize the mesh more we dont render all the chunk edges, just the once on the top( to avoid any wierd bugs )
                return new Voxel(BlockTypes.Stone);
            }


            chunk = renderChunks[chunkX, chunkY, chunkZ];


            if (chunk != null) {
                var blockX = RoundToInt(worldCoord.X - (chunk.ChunkPostion.X * _chunkSize.X));

                int blockY = RoundToInt(worldCoord.Y);

                var blockZ = RoundToInt(worldCoord.Z - (chunk.ChunkPostion.Z * _chunkSize.Z));

                if (chunk.Map != null) {
                    return chunk.Map.GetVoxel(blockX, blockY, blockZ);
                }
            }
            return null;
        }

        public static void SetBlockAt(int x, int y, int z, BlockTypes type) {
            SetBlockAt(new Vector3(x, y, z), type);
        }

        public static void SetBlockAt(Vector3 worldCoord, BlockTypes type) {
            int chunkX = FloorToInt(worldCoord.X / _chunkSize.X);
            int chunkY = 0;
            int chunkZ = FloorToInt(worldCoord.Z / _chunkSize.Z);

            RenderChunk chunk = null;

            try {
                chunk = renderChunks[chunkX, chunkY, chunkZ];
            }
            catch (IndexOutOfRangeException e) {
                LogManager.Debug(e.Message);
            }


            if (chunk != null) {
                var blockX = RoundToInt(worldCoord.X - (chunk.ChunkPostion.X * _chunkSize.X));

                int blockY = RoundToInt(worldCoord.Y);

                var blockZ = RoundToInt(worldCoord.Z - (chunk.ChunkPostion.Z * _chunkSize.Z));

                if (chunk.Map != null) {
                    chunk.Map.SetVoxel(blockX, blockY, blockZ, type);
                    if (type == BlockTypes.Water)
                        chunk.Map.GetVoxel(blockX, blockY, blockZ).Mass = 1;

                    chunk.HasGeneratedMesh = false;
                }
            }
        }

        private int _updatesSinceLastSimulation = 0;

        public override void Update(float deltaTime) {
            if (_updatesSinceLastSimulation == 50) {
                _waterSimulation.simulate = true;
                _updatesSinceLastSimulation = 0;
            }

            _updatesSinceLastSimulation ++;

            base.Update(deltaTime);
            if (Input.GetMouseDown(MouseButton.Left)) {
                Ray ray = new Ray(camera.Transform.Position, camera.Transform.Forward);
                RayCastResult hitResult = null;
                PhysicsEngine.Raycast(ray, 5000, out hitResult);


                Vector3 position = hitResult.HitData.Location;
                position += (hitResult.HitData.Normal * -0.5f);

                Debug.WriteLine(hitResult.HitData.Normal);
                SetBlockAt(position, BlockTypes.Air);
            }
            if (Input.GetMouseDown(MouseButton.Right)) {
                Ray ray = new Ray(camera.Transform.Position, camera.Transform.Forward);
                RayCastResult hitResult = null;
                PhysicsEngine.Raycast(ray, 5000, out hitResult);


                Vector3 position = hitResult.HitData.Location;
                position += (hitResult.HitData.Normal * 0.5f);

                Debug.WriteLine(hitResult.HitData.Normal);
                SetBlockAt(position, BlockTypes.Sand);
            }
            if (Input.GetKeyDown(Key.B)) {
                GameObject gObj = new GameObject();
                gObj.Transform.Position = camera.Transform.Position;
                gObj.AddComponent(new MeshRenderer(new Mesh("monkey3.obj"), new Material(new Texture("test.png"))));
                gObj.AddComponent(new BoxCollider(2, 2, 2, 2));
                AddObject(gObj);
            }
        }


        public static int RoundToInt(float f) {
            return (int)Math.Round((double)f);
        }

        public static int FloorToInt(float f) {
            return (int)Math.Floor((double)f);
        }
    }
}
