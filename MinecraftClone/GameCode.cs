using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Data.Voxel.Map;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameEngine;
using MonoGameEngine.Engine;
using MonoGameEngine.Engine.Components;
using MonoGameEngine.Engine.Physics;
using MonoGameEngine.Engine.Threading;
using MonoGameEngine.Engine.UI;
using MonoGameEngine.Engine.Utilites.Physics;

namespace MinecraftClone {
    public class GameCode : BaseGameCode {
        private Vector2 _lastMousePos;
        private Vector2 _currMousePos;
        private bool _rotateCamera;
        private const int InitialChunkAmount = 1;

        public Vector3 ChunkSize = new Vector3(16, 256, 16);

        private const int Height = 12;
        private const int DigDepth = 5;
        public const float NoiseScale = 5;
        public const int Seed = 12040;

        public RenderChunk[,,] renderChunks;

        public bool hasGeneratedChunks = false;

        public Texture2D Texture;

        KeyboardState keyboardPreviousState = new KeyboardState();
        MouseState mousePreviousState = new MouseState();


        public override void Initialize() {
            Texture = CoreEngine.instance.Content.Load<Texture2D>("terrain");

            var camera = new GameObject(new Vector3(0, 20, -100));
            camera.AddComponent<Camera>();
            camera.name = "Camera";
            camera.Instantiate();

            CreateGUI();

            ThreadManager.NewThread(StartWorldGen);


            Debug.WriteLine("init done");
        }

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

        }

        public void StartWorldGen() {
            renderChunks = new RenderChunk[3000, 1, 3000];

            for (int x = 0; x < InitialChunkAmount; x++) {
                for (int z = 0; z < InitialChunkAmount; z++) {
                    GameObject chunk1 = new GameObject("chunk { X:" + x + " Z:" + z + " }");
                    chunk1.Transform.Position = new Vector3(x * ChunkSize.X - 0.5f, 0.5f, z * ChunkSize.Z - 0.5f);
                    chunk1.Instantiate();
                    RenderChunk rChunk1 = chunk1.AddComponent<RenderChunk>();
                    rChunk1.MainTexture = Texture;
                    rChunk1.ChunkPostion = new Vector3(x, 0, z);
                    rChunk1.Scale = ChunkSize;
                    rChunk1.HeightFactor = Height;
                    rChunk1.DigDepth = DigDepth;
                    rChunk1.NoiseScale = NoiseScale;
                    rChunk1.Seed = Seed;
                    rChunk1.CreateNewChunk();
                    renderChunks[x, 0, z] = rChunk1;
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


        public static int RoundToInt(float f) {
            return (int)Math.Round((double)f);
        }

        public static int FloorToInt(float f) {
            return (int)Math.Floor((double)f);
        }


        public override void Update(float deltaTime) {
            UpdateEditorCam(deltaTime);
        }

        private void UpdateEditorCam(float deltaTime) {
            if (Camera.Main == null)
                return;
            GameObject camera = Camera.Main.GameObject;


            if (Keyboard.GetState().IsKeyDown(Keys.W)) {
                camera.Transform.Position -= camera.Transform.Forward();
            }
            if (Keyboard.GetState().IsKeyDown(Keys.S)) {
                camera.Transform.Position += camera.Transform.Forward();
            }
            if (Keyboard.GetState().IsKeyDown(Keys.A)) {
                camera.Transform.Position -= camera.Transform.Left();
            }

            if (Keyboard.GetState().IsKeyDown(Keys.D)) {
                camera.Transform.Position += camera.Transform.Left();
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Escape)) {
                CoreEngine.Quit();
            }

            if (PressedOnceKeyboard(Keys.Tab)) {
                _rotateCamera = !_rotateCamera;
            }
            if (PressedOnceLeftMouse()) {
                Ray ray = new Ray(camera.Transform.Position, -camera.Transform.Forward());
                RayCastResult hitResult = null;
                PhysicsEngine.Raycast(ray, 5000, out hitResult);

                if (hitResult.HitObject != null)
                    Debug.WriteLine(hitResult.HitObject.name);


                Vector3 position = hitResult.HitData.Location;
                position += (hitResult.HitData.Normal * 0.5f);

                Debug.WriteLine(hitResult.HitData.Normal);

                ChangeBlockAtWorldCoord(position, BlockTypes.Stone);

            }

            if (PressedOnceRightMouse()) {
                Ray ray = new Ray(camera.Transform.Position, -camera.Transform.Forward());
                RayCastResult hitResult = null;
                PhysicsEngine.Raycast(ray, 5000, out hitResult);

                if (hitResult.HitObject != null)
                    Debug.WriteLine(hitResult.HitObject.name);


                Vector3 position = hitResult.HitData.Location;
                position += (hitResult.HitData.Normal * -0.5f);

                Debug.WriteLine(hitResult.HitData.Normal);

                ChangeBlockAtWorldCoord(position, BlockTypes.Air);

            }


            UpdateCameraRotation(camera);

            keyboardPreviousState = Keyboard.GetState();
            mousePreviousState = Mouse.GetState();
        }



        private bool PressedOnceKeyboard(Keys key) {
            bool keyboard = Keyboard.GetState().IsKeyDown(key) && !keyboardPreviousState.IsKeyDown(key);

            if (key == Keys.Add) key = Keys.OemPlus;
            keyboard |= Keyboard.GetState().IsKeyDown(key) && !keyboardPreviousState.IsKeyDown(key);

            if (key == Keys.Subtract) key = Keys.OemMinus;
            keyboard |= Keyboard.GetState().IsKeyDown(key) && !keyboardPreviousState.IsKeyDown(key);

            return keyboard;
        }

        private bool PressedOnceLeftMouse() {
            bool keyboard = Mouse.GetState().LeftButton == ButtonState.Pressed && mousePreviousState.LeftButton != ButtonState.Pressed;

            return keyboard;
        }
        private bool PressedOnceMiddleMouse() {
            bool keyboard = Mouse.GetState().MiddleButton == ButtonState.Pressed && mousePreviousState.MiddleButton != ButtonState.Pressed;

            return keyboard;
        }
        private bool PressedOnceRightMouse() {
            bool keyboard = Mouse.GetState().RightButton == ButtonState.Pressed && mousePreviousState.RightButton != ButtonState.Pressed;

            return keyboard;
        }



        private void UpdateCameraRotation(GameObject camera) {

            _lastMousePos = _currMousePos;
            _currMousePos = Mouse.GetState().Position.ToVector2();
            var cameraTargetRot = camera.Transform.Rotation;


            var rotation = new Vector2(_currMousePos.X - _lastMousePos.X, _currMousePos.Y - _lastMousePos.Y) * 0.01f;

            if (!_rotateCamera)
                return;
            cameraTargetRot += Quaternion.CreateFromYawPitchRoll(-rotation.X, rotation.Y, 0);
            camera.Transform.Rotation = cameraTargetRot;
        }
    }
}
