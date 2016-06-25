using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGameEngine;
using MonoGameEngine.Engine;
using MonoGameEngine.Engine.Components;
using MonoGameEngine.Engine.Physics;

namespace MinecraftClone {
    // here you can place code that you do not want on a gameobject (sorta disconnected from the whole gameobject stuff)
    // or you can build your scene from here if you dont want to use the editor( or if i have not implemented and editor yet)
    public class GameCode : BaseGameCode {
        private Vector2 _lastMousePos;
        private Vector2 _currMousePos;
        private int _initialChunkAmount = 30;

        public Vector3 chunkSize = new Vector3(32, 200, 32);

        public int height = 10;
        public int digDepth = 5;
        public float noiseScale = 0.62f;
        public int seed = 12040;

        public override void Initialize() {
            var camera = new GameObject(new Vector3(0, 20, -100));
            camera.AddComponent<Camera>();
            camera.name = "Camera";
            camera.Instantiate();

            var sampleCube = new GameObject(new Vector3(0, 30, 0));
            sampleCube.AddComponent<MeshRenderer>();
            sampleCube.GetComponent<MeshRenderer>().Mesh = Primitives.CreateCube();
            sampleCube.GetComponent<MeshRenderer>().Color = Color.LightGray;
            var sphereCollider = sampleCube.AddComponent<SphereCollider>();
            sphereCollider.Radius = 2;
            sphereCollider.Mass = 10;
            sphereCollider.IsStatic = false;
            sampleCube.name = "cube";
            sampleCube.Instantiate();

            var ground = new GameObject(new Vector3(0, 0, 0));
            ground.AddComponent<MeshRenderer>();
            ground.GetComponent<MeshRenderer>().Mesh = Primitives.CreateCube();
            ground.GetComponent<MeshRenderer>().Color = Color.LightGray;
            ground.Transform.Scale = new Vector3(200, 1, 200);
            var boxCollider = ground.AddComponent<BoxCollider>();
            boxCollider.Height = 2f;
            boxCollider.Width = 200 * 2f;
            boxCollider.Length = 200 * 2f;
            boxCollider.Mass = 0;
            boxCollider.IsStatic = true;
            ground.name = "ground";
            ground.Instantiate();

            for (int x = 0; x < _initialChunkAmount; x++) {
                for (int z = 0; z < _initialChunkAmount; z++) {
                    GameObject chunk1 = new GameObject("chunk { X:" + x + " Z:" + z + " }");
                    chunk1.Transform.Position = new Vector3(x * chunkSize.X, 0, z * chunkSize.Z);
                    RenderChunk rChunk1 = chunk1.AddComponent<RenderChunk>();
                    rChunk1.ChunkPostion = new Vector3(x, 0, z);
                    rChunk1.scale = chunkSize;
                    rChunk1.heightFactor = height;
                    rChunk1.digDepth = digDepth;
                    rChunk1.noiseScale = noiseScale;
                    rChunk1.seed = seed;
                    rChunk1.StartChunkGeneration();
                    chunk1.Instantiate();
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
