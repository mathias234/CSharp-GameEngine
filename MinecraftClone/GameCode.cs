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

namespace MinecraftClone {
    public class GameCode : BaseGameCode {
        private Vector2 _lastMousePos;
        private Vector2 _currMousePos;
        private bool _rotateCamera;
        private const int InitialChunkAmount = 10;

        public Vector3 ChunkSize = new Vector3(16, 256, 16);

        private const int Height = 12;
        private const int DigDepth = 5;
        public const float NoiseScale = 5;
        public const int Seed = 12040;

        public RenderChunk[,,] renderChunks;

        public bool hasGeneratedChunks = false;

        public Texture2D Texture;


        public override void Initialize() {
            Texture = CoreEngine.instance.Content.Load<Texture2D>("terrain");

            var camera = new GameObject(new Vector3(0, 20, -100));
            camera.AddComponent<Camera>();
            camera.name = "Camera";
            camera.Instantiate();

            Thread thread = new Thread(StartWorldGen);
            thread.Start();

            Debug.WriteLine("init done");
        }

        public void StartWorldGen() {
            renderChunks = new RenderChunk[3000, 1, 3000];

            for (int x = 0; x < InitialChunkAmount; x++) {
                for (int z = 0; z < InitialChunkAmount; z++) {
                    GameObject chunk1 = new GameObject("chunk { X:" + x + " Z:" + z + " }");
                    chunk1.Transform.Position = new Vector3(x * ChunkSize.X, 0, z * ChunkSize.Z);
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

        public void GetBlockAtWorldCoord(Vector3 worldCoord) {
            int chunkX = (int)Math.Floor(worldCoord.X / ChunkSize.X);
            int chunkY = 0;
            int chunkZ = (int)Math.Floor(worldCoord.Z / ChunkSize.Z);
            Debug.WriteLine("updating " + chunkX + " " + chunkY + " " + chunkZ);


            int blockX = (int)Math.Floor(chunkX / worldCoord.X);
            int blockY = (int)Math.Floor(worldCoord.Y);
            int blockZ = (int)Math.Floor(chunkY / worldCoord.Y);

            if (renderChunks[chunkX, chunkY, chunkZ] != null) {
                var map = renderChunks[chunkX, chunkY, chunkZ].Map;
                if (map != null) {
                    renderChunks[chunkX, chunkY, chunkZ].Map.SetVoxel(blockX, blockY, blockZ, BlockTypes.Stone);
                    renderChunks[chunkX, chunkY, chunkZ].HasGeneratedMesh = false;
                }
            }
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
                _rotateCamera = !_rotateCamera;
            }
            if (Mouse.GetState().LeftButton == ButtonState.Pressed) {
                var sampleCube = new GameObject(camera.Transform.Position);
                sampleCube.AddComponent<MeshRenderer>();
                sampleCube.GetComponent<MeshRenderer>().Mesh = Primitives.CreateCube();
                sampleCube.GetComponent<MeshRenderer>().Color = Color.LightGray;
                var sphereCollider = sampleCube.AddComponent<SphereCollider>();
                sphereCollider.Radius = 1;
                sphereCollider.Mass = 1;
                sphereCollider.IsStatic = false;
                sampleCube.name = "cube";
                sampleCube.Instantiate();

            }


            UpdateCameraRotation(camera);
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
