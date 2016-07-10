using System;
using System.Diagnostics;
using System.Threading;
using Data.Voxel.Map;
using NewEngine.Engine.components;
using NewEngine.Engine.Core;
using NewEngine.Engine.Physics;
using NewEngine.Engine.Physics.PhysicsComponents;
using NewEngine.Engine.Rendering;
using OpenTK;
using OpenTK.Input;

namespace MinecraftClone {
    public class GameCode : Game {
        public const int InitialChunkAmount = 10;

        private  static Vector3 _chunkSize = new Vector3(16, 50, 16);

        private const int Height = 12;
        public const int DigDepth = 2;
        public const float NoiseScale = 5;
        public const int Seed = 2132;

        public static RenderChunk[,,] renderChunks;

        public bool hasGeneratedChunks = false;

        private Material terrainMaterial;

        private GameObject camera;

        public override void Start() {
            camera = new GameObject().AddComponent(new Camera(MathHelper.DegreesToRadians(70.0f),
                (float)CoreEngine.GetWidth() / CoreEngine.GetHeight(), 0.1f, 1000));
            AddObject(camera);


            terrainMaterial = new Material();
            terrainMaterial.AddTexture("diffuse", new Texture("terrain.png", TextureType.Point));
            terrainMaterial.AddFloat("specularIntensity", 1);
            terrainMaterial.AddFloat("specularPower", 16);


            GameObject directionalLightObj = new GameObject();
            DirectionalLight directionalLight = new DirectionalLight(new Vector3(1), 0.7f);
            directionalLightObj.AddComponent(directionalLight);
            directionalLightObj.Transform.Rotation = Quaternion.FromAxisAngle(new Vector3(1, 0, 0), MathHelper.DegreesToRadians(-45));
            AddObject(directionalLightObj);

            Thread thread = new Thread(StartWorldGen);
            thread.Start();

            Debug.WriteLine("init done");
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

        public static void SetBlockAt(Vector3 worldCoord, BlockTypes type) {
            Debug.WriteLine("hit at worldcoord " + worldCoord);


            int chunkX = FloorToInt(worldCoord.X / _chunkSize.X);
            int chunkY = 0;
            int chunkZ = FloorToInt(worldCoord.Z / _chunkSize.Z);
            Debug.WriteLine("updating " + chunkX + " " + chunkY + " " + chunkZ);

            RenderChunk chunk = null;

            try {
                chunk = renderChunks[chunkX, chunkY, chunkZ];
            }
            catch (IndexOutOfRangeException e) {
                Debug.WriteLine(e.Message);
            }


            if (chunk != null) {
                var blockX = RoundToInt(worldCoord.X - (chunk.ChunkPostion.X * _chunkSize.X));

                int blockY = RoundToInt(worldCoord.Y);

                var blockZ = RoundToInt(worldCoord.Z - (chunk.ChunkPostion.Z * _chunkSize.Z));

                if (chunk.Map != null) {
                    Debug.WriteLine("updating block" + blockX + " " + blockY + " " + blockZ);

                    chunk.Map.SetVoxel(blockX, blockY, blockZ, type);
                    if (type == BlockTypes.Water)
                        chunk.Map.GetVoxel(blockX, blockY, blockZ).Mass = 1;

                    chunk.HasGeneratedMesh = false;
                }
            }
        }

        public override void Update(float deltaTime) {
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
                gObj.AddComponent(new MeshRenderer(new Mesh("monkey3.obj"), new Material().AddTexture("diffuse", new Texture("test.png"))));
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
