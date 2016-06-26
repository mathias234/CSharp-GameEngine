using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameEngine;
using MonoGameEngine.Engine;
using MonoGameEngine.Engine.Components;
using MonoGameEngine.Engine.Physics;

namespace MinecraftClone {
    public class GameCode : BaseGameCode {
        private Vector2 _lastMousePos;
        private Vector2 _currMousePos;
        private const int InitialChunkAmount = 30;

        public Vector3 ChunkSize = new Vector3(16, 256, 16);

        private const int Height = 30;
        private const int DigDepth = 100;
        public const float NoiseScale = 1.5f;
        public const int Seed = 12040;

        public List<RenderChunk> renderChunks = new List<RenderChunk>();

        public bool hasGeneratedChunks = false;

        public Texture2D Texture;

        public override void Initialize() {
            Texture = Texture2D.FromStream(CoreEngine.instance.GraphicsDevice, File.Open(@"c:/master.png", FileMode.Open));

            var camera = new GameObject(new Vector3(0, 20, -100));
            camera.AddComponent<Camera>();
            camera.name = "Camera";
            camera.Instantiate();

            Thread thread = new Thread(StartWorldGen);
            thread.Start();

            Debug.WriteLine("init done");
        }

        public void StartWorldGen() {
            for (int x = 0; x < InitialChunkAmount; x++) {
                for (int z = 0; z < InitialChunkAmount; z++) {
                    GameObject chunk1 = new GameObject("chunk { X:" + x + " Z:" + z + " }");
                    chunk1.Transform.Position = new Vector3(x * ChunkSize.X, 0, z * ChunkSize.Z);
                    chunk1.Instantiate();
                    RenderChunk rChunk1 = chunk1.AddComponent<RenderChunk>();
                    rChunk1.mainTexture = Texture;
                    rChunk1.ChunkPostion = new Vector3(x, 0, z);
                    rChunk1.scale = ChunkSize;
                    rChunk1.heightFactor = Height;
                    rChunk1.digDepth = DigDepth;
                    rChunk1.noiseScale = NoiseScale;
                    rChunk1.seed = Seed;
                    rChunk1.StartChunkGeneration();
                    renderChunks.Add(rChunk1);
                }
            }

        }

        public override void Update(float deltaTime) {
            UpdateEditorCam(deltaTime);

            for (int i = 0; i < renderChunks.Count; i++) {
                renderChunks[i].CheckIfMeshUpdatedIsRequired();
            }
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

            UpdateCameraRotation(camera);
        }

        private void UpdateCameraRotation(GameObject camera) {
            _lastMousePos = _currMousePos;
            _currMousePos = Mouse.GetState().Position.ToVector2();
            var cameraTargetRot = camera.Transform.Rotation;
            var rotation = new Vector2(_currMousePos.X - _lastMousePos.X, _currMousePos.Y - _lastMousePos.Y) * 0.01f;

            if (Mouse.GetState().RightButton == ButtonState.Pressed) {
                cameraTargetRot += Quaternion.CreateFromYawPitchRoll(-rotation.X, rotation.Y, 0);
                camera.Transform.Rotation = cameraTargetRot;
            }
        }
    }
}
