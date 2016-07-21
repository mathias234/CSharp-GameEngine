using System;
using System.Drawing;
using System.Threading;
using NewEngine;
using NewEngine.Engine.components;
using NewEngine.Engine.Core;
using NewEngine.Engine.Physics.PhysicsComponents;
using NewEngine.Engine.Rendering;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace TestGame {
    public class TestGame : Game {
        private GameObject directionalLightObj;
        private GameObject camera;
        public override void Start() {
            Mesh terrainMesh = new Mesh("shadowTest.obj");

            GameObject shadowTest =
                new GameObject().AddComponent(new MeshRenderer(terrainMesh,
                    new Material(new Texture("bricks.png"), 0.5f, 32f, new Texture("bricks_nrm.png"), new Texture("bricks_disp.jpg"), 0.0134f)));

            shadowTest.AddComponent(new MeshCollider(terrainMesh));
            AddObject(shadowTest);


            camera = new GameObject().AddComponent(new FreeLook()).AddComponent(new FreeMove()).AddComponent(new Camera(Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(70.0f), (float)CoreEngine.GetWidth() / CoreEngine.GetHeight(), 0.1f, 1000)));

            camera.Transform.Position = new Vector3(0, 5, 0);

            AddObject(camera);

            directionalLightObj = new GameObject();
            DirectionalLight directionalLight = new DirectionalLight(new Vector3(1), 0.7f);
            directionalLightObj.AddComponent(directionalLight);
            directionalLightObj.Transform.Rotation *= Quaternion.FromAxisAngle(new Vector3(1, 0, 0), -0.4f);
            AddObject(directionalLightObj);
        }


        public override void Update(float deltaTime) {
            base.Update(deltaTime);
            Vector3 flameColor = new Vector3(226 / 255.0f, 88 / 255.0f, 34 / 255.0f);
            if (Input.GetKeyDown(Key.Q)) {
                StartMassiveSpawn();
            }
            if (Input.GetKeyDown(Key.P)) {
                GameObject newPointLight = new GameObject().AddComponent(new PointLight(flameColor, 5, new Attenuation(0, 0, 1f)));
                newPointLight.Transform.Position = new Vector3(CoreEngine.GetCoreEngine.RenderingEngine.MainCamera.Transform.GetTransformedPosition());
                AddObject(newPointLight);
            }
        }

        public void StartMassiveSpawn() {
            for (int x = -5; x < 5; x++) {
                for (int z = -5; z < 5; z++) {
                    GameObject gObj = new GameObject();
                    gObj.Transform.Position = camera.Transform.Position + new Vector3(5 * x, 0, 5 * z);
                    gObj.AddComponent(new MeshRenderer(new Mesh("sphere.obj"), new Material(new Texture("test.png"))));
                    gObj.AddComponent(new SphereCollider(1, 2));
                    AddObject(gObj);
                }
            }
        }
    }
}
