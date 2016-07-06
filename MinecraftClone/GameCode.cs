using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using Data.Voxel.Map;
using NewEngine.Engine.components;
using NewEngine.Engine.Core;
using NewEngine.Engine.Physics.PhysicsComponents;
using NewEngine.Engine.Rendering;
using OpenTK;
using OpenTK.Input;
using ButtonState = OpenTK.Input.ButtonState;

namespace MinecraftClone {
    public class GameCode : Game {
        private Vector2 _lastMousePos;
        private Vector2 _currMousePos;
        private bool _rotateCamera;
        private const int InitialChunkAmount = 32;

        public Vector3 ChunkSize = new Vector3(16, 256, 16);

        private const int Height = 12;
        private const int DigDepth = 42;
        public const float NoiseScale = 5;
        public const int Seed = 52393;

        public RenderChunk[,,] renderChunks;

        public bool hasGeneratedChunks = false;

        KeyboardState keyboardPreviousState = new KeyboardState();
        MouseState mousePreviousState = new MouseState();


        private Material terrainMaterial;

        public override void Start() {
            terrainMaterial = new Material(new Texture("terrain.png"), Color.White, 1, 16);

            GameObject directionalLightObj = new GameObject();
            DirectionalLight directionalLight = new DirectionalLight(new Vector3(1), 0.7f, new Vector3(1));
            directionalLightObj.AddComponent(directionalLight);
            GetRootObject.AddChild(directionalLightObj);

            Thread thread = new Thread(StartWorldGen);
            thread.Start();

            Debug.WriteLine("init done");
        }
        /*
        private void CreateGUI() {
            GameObject uiObject = new GameObject("UISystem");
            uiObject.Instantiate();

            var crosshair1 = uiObject.AddComponent<UiTextureComponent>();
            crosshair1.Rect = new Rectangle(CoreEngine.instance.GraphicsDevice.Viewport.Width / 2 - 10, CoreEngine.instance.GraphicsDevice.Viewport.Height / 2 - 2, 20, 4);
            crosshair1.Texture2D = UISystem.GetDefaultBackground;
            crosshair1.Color = Color.DarkGray;

            var crosshair2 = uiObject.AddComponent<UiTextureComponent>();
            crosshair2.Rect = new Rectangle(CoreEngine.instance.GraphicsDevice.Viewport.Width / 2 - 2, CoreEngine.instance.GraphicsDevice.Viewport.Height / 2 - 10, 4, 20);
            crosshair2.Texture2D = UISystem.GetDefaultBackground;
            crosshair2.Color = Color.DarkGray;

        }*/

        public void StartWorldGen() {
            renderChunks = new RenderChunk[3000, 1, 3000];

            for (int x = 0; x < InitialChunkAmount; x++) {
                for (int z = 0; z < InitialChunkAmount; z++) {
                    GameObject chunk1 = new GameObject();
                    chunk1.Transform.Position = new Vector3(x * ChunkSize.X - 0.5f, 0.5f, z * ChunkSize.Z - 0.5f);

                    RenderChunk rChunk1 = new RenderChunk();

                    chunk1.AddComponent(rChunk1);

                    rChunk1.Material = terrainMaterial;
                    rChunk1.ChunkPostion = new Vector3(x, 0, z);
                    rChunk1.Scale = ChunkSize;
                    rChunk1.HeightFactor = Height;
                    rChunk1.DigDepth = DigDepth;
                    rChunk1.NoiseScale = NoiseScale;
                    rChunk1.Seed = Seed;
                    rChunk1.CreateNewChunk();
                    renderChunks[x, 0, z] = rChunk1;

                    GetRootObject.AddChild(chunk1);
                }
            }

        }

        public void ChangeBlockAtWorldCoord(Vector3 worldCoord, BlockTypes type) {
            Debug.WriteLine("hit at worldcoord " + worldCoord);


            int chunkX = FloorToInt(worldCoord.X / ChunkSize.X);
            int chunkY = 0;
            int chunkZ = FloorToInt(worldCoord.Z / ChunkSize.Z);
            Debug.WriteLine("updating " + chunkX + " " + chunkY + " " + chunkZ);

            RenderChunk chunk = null;

            try {
                chunk = renderChunks[chunkX, chunkY, chunkZ];
            }
            catch (IndexOutOfRangeException e) {
                Debug.WriteLine(e.Message);
            }


            if (chunk != null) {
                var blockX = RoundToInt(worldCoord.X - (chunk.ChunkPostion.X * ChunkSize.X));

                int blockY = RoundToInt(worldCoord.Y);

                var blockZ = RoundToInt(worldCoord.Z - (chunk.ChunkPostion.Z * ChunkSize.Z));

                if (chunk.Map != null) {
                    Debug.WriteLine("updating block" + blockX + " " + blockY + " " + blockZ);

                    chunk.Map.SetVoxel(blockX, blockY, blockZ, type);
                    chunk.HasGeneratedMesh = false;
                }
            }
        }

        public override void Update(float deltaTime) {
            base.Update(deltaTime);
            if (Input.GetKey(Key.B)) {
                var obj = new GameObject();
                obj.Transform.Position = RenderingEngine.Instance.MainCamera.Position;
                obj.AddComponent(new BoxCollider(2,2,2, 30));
                obj.AddComponent(new MeshRenderer(new Mesh("test.obj"), new Material(new Texture("test.png"), Color.White)));
                GetRootObject.AddChild(obj);
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
