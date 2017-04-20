using System;
using System.Collections.Generic;
using NewEngine.Engine.components;
using NewEngine.Engine.Core;
using NewEngine.Engine.Physics;
using NewEngine.Engine.Physics.PhysicsComponents;
using NewEngine.Engine.Rendering;
using NewEngine.Engine.Rendering.Shading;
using OpenTK;
using OpenTK.Input;
using NewEngine.Engine.Audio;
using NewEngine.Engine.Rendering.GUI;

namespace Game {
    public class TestGame : NewEngine.Engine.Core.Game {
        private GameObject _camera;
        private GameObject _directionalLightObj;

        private List<GameObject> _spawnedObjects = new List<GameObject>();

        private Material _mainMaterial;
        private GameObject _cube;

        public override void Start() {
            CreateCamera();

            var UIImage = new GameObject("UI image");
            UIImage.AddComponent(new Image(Texture.GetTexture("bricks.png")));
            UIImage.Transform.Scale = new Vector3(100, 30, 1);

            AddObject(UIImage);

            //AudioMaster.Initialize();

            //RenderingEngine.AmbientLight = new Vector3(0.3f);

            //_directionalLightObj = new GameObject("Directinal Light");
            //var directionalLight = new DirectionalLight(new Vector3(1), 0.5f, 10, 140, 0.9f);
            //_directionalLightObj.AddComponent(directionalLight);
            //_directionalLightObj.Transform.Rotation *= Quaternion.FromAxisAngle(new Vector3(1, 0, 0),
            //    (float)MathHelper.DegreesToRadians(-80));

            //var spotLightObj = new GameObject("Spot Light");
            //var spotLight = new SpotLight(new Vector3(1, 1, 0), 5f, new Attenuation(0, 0, 0.01f),
            //    MathHelper.DegreesToRadians(70), 0, 0.5f, 0.6f);
            //spotLightObj.Transform.Position = new Vector3(30, 0, 30);
            //spotLightObj.Transform.Rotate(new Vector3(0, 1, 0), MathHelper.DegreesToRadians(0));

            //spotLightObj.AddComponent(spotLight);

            //var particleObj = new GameObject("Particle");

            //particleObj.AddComponent(new ParticleSystem(200000, Texture.GetTexture("test2_cutout.png"),  new Vector3(-200, 90, -200), new Vector3(200, 90, 200),
            //    //new Vector4(1, 0.5f, 0, 1), new Vector4(1f, 0.7f, 0, 1), 2, new Vector3(0, 0 -9.825f, 0),
            //    new Vector4(1, 1, 1, 1), new Vector4(1, 1, 1, 1), 2, new Vector3(0, 0 -9.825f, 0),
            //    new Vector3(0, 0.1f, 0), new Vector3(0, 0.5f, 0), 1, 2, 100, 100, 20, false, true, true));


            //_cube = new GameObject("Cube");

            //_mainMaterial = new Material(Shader.GetShader("batchedShader"));

            //_mainMaterial.SetTexture("diffuse", Texture.GetTexture("bricks.png"));

            //_mainMaterial.SetTexture("normalMap", Texture.GetTexture("bricks_nrm.png"));

            //_mainMaterial.SetTexture("dispMap", Texture.GetTexture("bricks_disp.jpg"));

            //_mainMaterial.SetFloat("dispMapScale", 0.01f);

            //var baseBias = _mainMaterial.GetFloat("dispMapScale") / 2.0f;

            //_mainMaterial.SetFloat("dispMapBias", -baseBias + baseBias * 0);

            //_mainMaterial.SetFloat("specularIntensity", 0.5f);
            //_mainMaterial.SetFloat("specularPower", 32);


            //_cube.AddComponent(new BoxCollider(1, 0.1f, 1, 0));

            //_cube.AddComponent(new MeshRenderer(Mesh.GetMesh("cube.obj"), _mainMaterial));


            //_cube.Transform.Position = new Vector3(0, -2, 0);
            //_cube.Transform.Scale = new Vector3(200, 1, 200);

            //var terrain = new GameObject("terrain");
            //var water = new GameObject("water");

            //terrain.AddComponent(new TerrainMesh("terrain1/terrain.jpg", 300, 300, 0.1f, "terrain1/tex1.jpg",
            //    "default_normal.png", "terrain1/tex2.jpg", "terrain1/tex2Nrm.jpg", "terrain1/layer1.jpg",
            //    "terrain1/tex2.jpg", "terrain1/tex2Nrm.jpg", "terrain1/layer1.jpg", 0.1f, 64));

            //water.AddComponent(new WaterMesh(300, 300, new Vector4(0.7f, 1, 0.9f, 1), 0.05f, 0.02f, 0.2f, 12));

            //water.Transform.Position = new Vector3(0, 20, 0);

            //AddObject(terrain);
            //AddObject(water);
            ////AddObject(_cube);
            //AddObject(_directionalLightObj);
            ////AddObject(particleObj);


            GetRootObject.Engine.RenderingEngine.SetSkybox("skybox/top.jpg", "skybox/bottom.jpg", "skybox/front.jpg",
                "skybox/back.jpg", "skybox/left.jpg", "skybox/right.jpg");
        }

        public override void Update(float deltaTime) {
            base.Update(deltaTime);

            LogManager.Debug(Fps.GetFps(deltaTime).ToString());

            //_cube.Transform.Position += new Vector3(0.1f, 0,0);

            var flameColor = new Vector3(0, 1.0f, 1.0f);

            if (Input.GetKeyDown(Key.E)) {
                StartMassiveSpawn();
            }

            if (Input.GetKeyDown(Key.R)) {
                SingleSpawn();
            }

            if (Input.GetKeyDown(Key.F)) {
                var audioSourceObject = new GameObject("audio source");
                var audioSource = new AudioSource();
                audioSourceObject.Transform.Position = new Vector3(CoreEngine.GetCoreEngine.RenderingEngine.MainCamera.Transform.GetTransformedPosition());
                audioSourceObject.Transform.Rotate(new Vector3(0, 1, 0), MathHelper.DegreesToRadians(0));

                audioSourceObject.AddComponent(audioSource);

                AddObject(audioSourceObject);

                audioSource.Play("./res/bounce.wav");
                audioSource.SetLooping(true);
                audioSource.Is3D(true);
                audioSource.SetVolume(20);
            }

            if (Input.GetKeyDown(Key.Q)) {
                foreach (var spawnedObject in _spawnedObjects) {
                    spawnedObject.Destroy();
                }
                _spawnedObjects.Clear();
            }

            if (Input.GetKeyDown(Key.P)) {

                var pointLight = new GameObject("Point Light");
                var spotLight = new PointLight(flameColor, 5f, new Attenuation(0, 0, 0.1f));
                pointLight.Transform.Position = new Vector3(CoreEngine.GetCoreEngine.RenderingEngine.MainCamera.Transform.GetTransformedPosition());
                pointLight.Transform.Rotate(new Vector3(0, 1, 0), MathHelper.DegreesToRadians(0));

                pointLight.AddComponent(spotLight);

                AddObject(pointLight);
            }

            double i = 0;

            foreach (var spawnedObject in _spawnedObjects) {
                Random r = new Random(DateTime.Now.Millisecond);

                double speed = 2;

                double x = r.NextDouble() / speed + (i + 10); 
                double y = r.NextDouble() / speed + (i + 10);
                double z = r.NextDouble() / speed + (i + 10);

                spawnedObject.Transform.Position = new Vector3((float)x, (float)y, (float)z);

                i++;
            }
        }

        public void StartMassiveSpawn() {
            for (int i = 0; i < 5; i++) {
                for (int j = 0; j < 5; j++) {
                    for (int k = 0; k < 5; k++) {
                        var gObj = new GameObject("sphere:") {
                            Transform = { Position = _camera.Transform.Position + (new Vector3(i, j, k) * 2.5f) }
                        };

                        var _mesh = Mesh.GetMesh("cube.obj");

                        gObj.AddComponent(new MeshRenderer(_mesh, _mainMaterial));
                        //gObj.AddComponent(new SphereCollider(2, 1));
                        AddObject(gObj);
                        _spawnedObjects.Add(gObj);
                    }
                }
            }
        }


        public void SingleSpawn() {

            var gObj = new GameObject("sphere:") {
                Transform = { Position = _camera.Transform.Position }
            };

            var _mesh = Mesh.GetMesh("cube.obj");

            gObj.AddComponent(new MeshRenderer(_mesh, _mainMaterial));
            //gObj.AddComponent(new SphereCollider(2, 1));
            AddObject(gObj);
            _spawnedObjects.Add(gObj);
        }


        public void CreateCamera() {
            _camera = new GameObject("main camera")
                .AddComponent(new FreeLook(true, true))
                .AddComponent(new FreeMove())
                .AddComponent(new Camera(Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(70.0f), CoreEngine.GetWidth() / CoreEngine.GetHeight(), 0.1f, 1000)))
                .AddComponent(new AudioListener());

            _camera.Transform.Rotate(new Vector3(1, 0, 0), -0.4f);

            AddObject(_camera);
        }
    }
}