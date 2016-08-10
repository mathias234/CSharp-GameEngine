﻿using System.Collections.Generic;
using System.IO;
using NewEngine.Engine.components;
using NewEngine.Engine.components.UIComponents;
using NewEngine.Engine.Core;
using NewEngine.Engine.Physics.PhysicsComponents;
using NewEngine.Engine.Rendering;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace Game {
    public class TestGame : NewEngine.Engine.Core.Game {
        private GameObject _camera;
        private GameObject _directionalLightObj;

        private List<GameObject> _spawnedObjects = new List<GameObject>();

        public override void Start() {


            CreateCamera();

            _directionalLightObj = new GameObject("Directinal Light");
            var directionalLight = new DirectionalLight(new Vector3(1), 0.5f, 10, 120 * 2, 1f);
            _directionalLightObj.AddComponent(directionalLight);
            _directionalLightObj.Transform.Rotation *= Quaternion.FromAxisAngle(new Vector3(1, 0, 0), (float)MathHelper.DegreesToRadians(-90.0));

            var spotLightObj = new GameObject("Spot Light");
            var spotLight = new SpotLight(new Vector3(1, 1, 0), 0.2f, new Attenuation(0, 0, 0.02f), MathHelper.DegreesToRadians(40), 9, 1.0f, 0.6f);
            spotLightObj.Transform.Position = new Vector3(5, 1, 0);
            spotLightObj.Transform.Rotate(new Vector3(0, 1, 0), MathHelper.DegreesToRadians(90));

            spotLightObj.AddComponent(spotLight);

            var particleObj = new GameObject("Particle");

            particleObj.AddComponent(new ParticleSystem(20, new Vector3(-20, 0, -20), new Vector3(20, 2, 20), new Vector4(0, 0, 0, 1), new Vector4(1, 1, 1, 1), 2, new Vector3(0, 0 /*-9.825f*/, 0), 
                new Vector3(0, 0.1f, 0), new Vector3(0, 0.5f, 0), 2, 5, 1, 10, 2, true, false, true));

            var particleObj2 = new GameObject("Particle2");

            particleObj2.AddComponent(new ParticleSystem(20, new Vector3(-20, 0, -20), new Vector3(20, 2, 20), new Vector4(0, 0, 0, 1), new Vector4(1, 1, 1, 1), 2, new Vector3(0, 0 /*-9.825f*/, 0),
                new Vector3(0, 0.1f, 0), new Vector3(0, 0.5f, 0), 2, 5, 1, 10, 2, true, false, true));

            particleObj2.Transform.Position = new Vector3(100, 0, 0);

            AddObject(spotLightObj);
            AddObject(_directionalLightObj);
            //AddObject(particleObj2);
            //AddObject(particleObj);

            var plane = new GameObject("plane");
            var cube = new GameObject("cube");

            var cubeMesh = new Mesh("cube.obj");
            var planeMesh = new Mesh("plane.obj");

            var mainMaterial = new Material(new Texture("bricks.png"), 0.5f, 32f, new Texture("bricks_nrm.png"), new Texture("bricks_disp.jpg"), 0.01f);

            plane.AddComponent(new MeshRenderer(planeMesh, mainMaterial));
            plane.AddComponent(new BoxCollider(300, 1, 300, 0));
            cube.AddComponent(new MeshRenderer(cubeMesh, mainMaterial));
            cube.AddComponent(new BoxCollider(1,1,1,0));

            cube.Transform.Position = new Vector3(0, 1, 0);

            //AddObject(plane);
            //AddObject(cube);


            var terrain = new GameObject("terrain");
            var water = new GameObject("water");

            terrain.AddComponent(new TerrainMesh("terrain1/terrain.jpg", 300, 300, 0.1f, "terrain1/tex1.jpg",
                "default_normal.png", "terrain1/tex2.jpg", "terrain1/tex2Nrm.jpg", "terrain1/layer1.jpg",
                "terrain1/tex2.jpg", "terrain1/tex2Nrm.jpg", "terrain1/layer1.jpg"));

            water.AddComponent(new WaterMesh(300, 300, 0.05f, 0.02f, 0.2f, 12));

            terrain.Transform.Position = new Vector3(0, -15, 0);
            water.Transform.Position = new Vector3(0, 0, 0);


            AddObject(terrain);
            AddObject(water);


            //var ui = new GameObject("UI");
            //ui.AddComponent(new Image(new RectTransform(220, 220, 650, 340),
            //    CoreEngine.GetCoreEngine.RenderingEngine.GetTexture("refractionTexture")));

            //ui.AddComponent(new Image(new RectTransform(220, 220, -650, -340),
            //    CoreEngine.GetCoreEngine.RenderingEngine.GetTexture("reflectionTexture")));


            //AddObject(ui);

            CoreEngine.GetCoreEngine.RenderingEngine.SetSkybox("skybox/top.jpg", "skybox/bottom.jpg", "skybox/front.jpg",
                "skybox/back.jpg", "skybox/left.jpg", "skybox/right.jpg");
        }

        public override void Update(float deltaTime) {
            //LogManager.Debug(GetRootObject.GetChildren().Count.ToString());

            base.Update(deltaTime);
            var flameColor = new Vector3(0, 1.0f, 1.0f);

            if (Input.GetKeyDown(Key.E)) {
                StartMassiveSpawn();
            }

            if (Input.GetKeyDown(Key.Q)) {
                foreach (var spawnedObject in _spawnedObjects) {
                    spawnedObject.Destroy();
                }
            }

            if (Input.GetKeyDown(Key.P)) {
                var newPointLight =
                    new GameObject("point light").AddComponent(new PointLight(flameColor, 0.2f, new Attenuation(0, 0, 0.01f)));
                newPointLight.Transform.Position =
                    new Vector3(CoreEngine.GetCoreEngine.RenderingEngine.MainCamera.Transform.GetTransformedPosition());

                AddObject(newPointLight);
            }
        }

        public void StartMassiveSpawn() {
            for (var x = -5; x < 5; x++) {
                for (var z = -5; z < 5; z++) {
                    var gObj = new GameObject("sphere: " + x + ":" + z) {
                        Transform = { Position = _camera.Transform.Position + new Vector3(5 * x, 0, 5 * z) }
                    };
                    gObj.AddComponent(new MeshRenderer(new Mesh("cube.obj"), new Material(new Texture("test.png"))));
                    gObj.AddComponent(new BoxCollider(2, 2, 2, 1));
                    AddObject(gObj);
                    _spawnedObjects.Add(gObj);
                }
            }
        }

        public void CreateCamera() {
            _camera = new GameObject("main camera")
                .AddComponent(new FreeLook())
                .AddComponent(new FreeMove())
                .AddComponent(new Camera(Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(70.0f), CoreEngine.GetWidth() / CoreEngine.GetHeight(), 0.1f, 1000)));

            _camera.Transform.Position = new Vector3(150, 20, 150);

            AddObject(_camera);
        }
    }
}