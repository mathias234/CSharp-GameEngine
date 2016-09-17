using System.Collections.Generic;
using System.Drawing;
using NewEngine.Engine.components;
using NewEngine.Engine.Core;
using NewEngine.Engine.Physics;
using NewEngine.Engine.Physics.PhysicsComponents;
using NewEngine.Engine.Rendering;
using NewEngine.Engine.Rendering.Shading;
using OpenTK;
using OpenTK.Input;
using Image = NewEngine.Engine.components.UIComponents.Image;

namespace Game {
    public class TestGame : NewEngine.Engine.Core.Game {
        private GameObject _camera;
        private GameObject _directionalLightObj;

        private List<GameObject> _spawnedObjects = new List<GameObject>();

        private Material _mainMaterial;
        private Mesh _mesh;
        private GameObject _cube;
        private Mesh _treeBranch;
        private Mesh _treeLeaf;

        public override void Start() {
            CreateCamera();

            RenderingEngine.AmbientLight = new Vector3(0.3f);

            _directionalLightObj = new GameObject("Directinal Light");
            var directionalLight = new DirectionalLight(new Vector3(1), 0.5f, 10, 140, 0.9f);
            _directionalLightObj.AddComponent(directionalLight);
            _directionalLightObj.Transform.Rotation *= Quaternion.FromAxisAngle(new Vector3(1, 0, 0), (float)MathHelper.DegreesToRadians(-80));

            var spotLightObj = new GameObject("Spot Light");
            var spotLight = new SpotLight(new Vector3(1, 1, 0), 0.7f, new Attenuation(0, 0, 0.2f), MathHelper.DegreesToRadians(70), 9, 0.5f, 0.6f);
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

            //AddObject(spotLightObj);
            AddObject(_directionalLightObj);
            //AddObject(particleObj2);
            AddObject(particleObj);

            var plane = new GameObject("plane");
            _cube = new GameObject("cubebase");

            var cubeMesh = new Mesh("plane.obj");
            var planeMesh = new Mesh("plane.obj");

            _mainMaterial = new Material(new Shader("batchedShader.shader"));

            _mainMaterial.SetTexture("diffuse", new Texture("bricks.png"));

            _mainMaterial.SetTexture("normalMap", new Texture("bricks_nrm.png"));

            _mainMaterial.SetTexture("dispMap", new Texture("bricks_disp.jpg"));

            _mainMaterial.SetFloat("dispMapScale", 0.01f);

            var baseBias = _mainMaterial.GetFloat("dispMapScale") / 2.0f;

            _mainMaterial.SetFloat("dispMapBias", -baseBias + baseBias * 0);

            _mainMaterial.SetFloat("specularIntensity", 0.5f);
            _mainMaterial.SetFloat("specularPower", 32);


            plane.AddComponent(new BoxCollider(300, 0.1f, 300, 0));
            _cube.AddComponent(new MeshRenderer(cubeMesh, _mainMaterial));
            _cube.AddComponent(new BoxCollider(1, 1, 1, 0));


            _cube.Transform.Position = new Vector3(-100, 10, 0);
            plane.Transform.Position = new Vector3(-100, 0, 0);
            _cube.Transform.Scale = new Vector3(0.2f);

            _cube.Transform.Rotate(new Vector3(1, 1, 0), MathHelper.DegreesToRadians(132));


            AddObject(plane);
            AddObject(_cube);


            var terrain = new GameObject("terrain");
            var water = new GameObject("water");
            var water2 = new GameObject("water2");

            terrain.AddComponent(new TerrainMesh("terrain1/terrain.jpg", 300, 300, 0.1f, "terrain1/tex1.jpg",
                "default_normal.png", "terrain1/tex2.jpg", "terrain1/tex2Nrm.jpg", "terrain1/layer1.jpg",
                "terrain1/tex2.jpg", "terrain1/tex2Nrm.jpg", "terrain1/layer1.jpg"));

            water.AddComponent(new WaterMesh(300, 300, 0.05f, 0.02f, 0.2f, 12));

            water.Transform.Position = new Vector3(0, 15, 0);

            AddObject(terrain);
            AddObject(water);

            CoreEngine.GetCoreEngine.RenderingEngine.SetSkybox("skybox/top.jpg", "skybox/bottom.jpg", "skybox/front.jpg",
                "skybox/back.jpg", "skybox/left.jpg", "skybox/right.jpg");

            _mesh = new Mesh("sphere.obj");
            _treeBranch = new Mesh("tree/branch.obj");
            _treeLeaf = new Mesh("tree/leaf.obj");
        }

        public override void Update(float deltaTime) {
            _cube.Transform.Position += new Vector3(42, 10, 0);

            //LogManager.Debug(GetRootObject.GetChildren().Count.ToString());

            base.Update(deltaTime);
            var flameColor = new Vector3(0, 1.0f, 1.0f);

            if (Input.GetKeyDown(Key.E)) {
               StartMassiveSpawn();
            }


            if (Input.GetKeyDown(Key.R)) {

                RayCastResult result;

                PhysicsEngine.Raycast(new Ray(_camera.Transform.Position, -Vector3.UnitY), 10000, out result);

                var branch = new GameObject("branch") {
                    Transform = { Position = new Vector3(_camera.Transform.Position.X, result.HitData.Location.Y, _camera.Transform.Position.Z)  }
                };
                branch.AddComponent(new MeshRenderer(_treeBranch, _mainMaterial));
                AddObject(branch);

                var leaf = new GameObject("leaf") {
                    Transform = { Position = new Vector3(_camera.Transform.Position.X, result.HitData.Location.Y, _camera.Transform.Position.Z) }
                };
                leaf.AddComponent(new MeshRenderer(_treeLeaf, _mainMaterial));
                AddObject(leaf);

                _spawnedObjects.Add(branch);
                _spawnedObjects.Add(leaf);
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
                    gObj.AddComponent(new MeshRenderer(_mesh, _mainMaterial));
                    //gObj.AddComponent(new SphereCollider(2, 1));
                    AddObject(gObj);
                    _spawnedObjects.Add(gObj);
                }
            }
        }

        public void CreateCamera() {
            _camera = new GameObject("main camera")
                .AddComponent(new FreeLook(true, true))
                .AddComponent(new FreeMove())
                .AddComponent(new Camera(Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(70.0f), CoreEngine.GetWidth() / CoreEngine.GetHeight(), 0.1f, 1000)));

            _camera.Transform.Position = new Vector3(50, 50, 190);
            _camera.Transform.Rotate(new Vector3(1, 0, 0), -0.4f );

            AddObject(_camera);
        }
    }
}