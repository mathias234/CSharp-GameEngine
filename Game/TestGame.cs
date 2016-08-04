using System.Collections.Generic;
using NewEngine.Engine.components;
using NewEngine.Engine.Core;
using NewEngine.Engine.Physics.PhysicsComponents;
using NewEngine.Engine.Rendering;
using OpenTK;
using OpenTK.Input;

namespace Game {
    public class TestGame : NewEngine.Engine.Core.Game {
        private GameObject _camera;
        private GameObject _directionalLightObj;

        private List<GameObject> _spawnedObjects = new List<GameObject>();

        public override void Start() {
            Material brickMaterial = new Material(new Texture("bricks.png"), 0.5f, 32, new Texture("bricks_nrm.png"), new Texture("bricks_disp.jpg"), 0.02f, 0);
            Material rockMaterial = new Material(new Texture("rock.jpg"), 0.5f, 32, new Texture("rock_nrm.jpg"), new Texture("rock_disp.jpg"), 0.02f, 0);

            Mesh planeMeshCollider = new Mesh("plane.obj");

            GameObject floor = new GameObject("floor");
            floor.AddComponent(new MeshRenderer(planeMeshCollider, brickMaterial));
            floor.AddComponent(new MeshCollider(planeMeshCollider));


            CreateCamera();

            _directionalLightObj = new GameObject("Directinal Light");
            var directionalLight = new DirectionalLight(new Vector3(1), 0.2f, 10, 300);
            _directionalLightObj.AddComponent(directionalLight);
            _directionalLightObj.Transform.Rotation *= Quaternion.FromAxisAngle(new Vector3(1, 0, 0), (float)MathHelper.DegreesToRadians(-45.0));

            var spotLightObj = new GameObject("Spot Light");
            var spotLight = new SpotLight(new Vector3(0, 1, 1), 0.2f, new Attenuation(0, 0, 0.02f), MathHelper.DegreesToRadians(91.1f), 9, 1.0f, 0.6f);
            spotLightObj.Transform.Position = new Vector3(3, 1, -5);
            spotLightObj.Transform.Rotate(new Vector3(0, 1, 0), MathHelper.DegreesToRadians(90));

            spotLightObj.AddComponent(spotLight);

            var particleObj = new GameObject("Particle");

            particleObj.AddComponent(new ParticleSystem(20, new Vector3(-20, 0, -20), new Vector3(20, 2, 20), new Vector4(0, 0, 0, 1), new Vector4(1, 1, 1, 1), 2, new Vector3(0, 0 /*-9.825f*/, 0), 
                new Vector3(0, 0.1f, 0), new Vector3(0, 0.5f, 0), 2, 5, 1, 10, 2, true, false, true));

            particleObj.Transform.Position = new Vector3(0, 0, 0);

            //AddObject(spotLightObj);
            AddObject(_directionalLightObj);
            AddObject(floor);
            AddObject(particleObj);


            CoreEngine.GetCoreEngine.RenderingEngine.SetSkybox("skybox/top.jpg", "skybox/bottom.jpg", "skybox/front.jpg",
                "skybox/back.jpg", "skybox/left.jpg", "skybox/right.jpg");
        }

        public override void Update(float deltaTime) {
            //LogManager.Debug(GetRootObject.GetChildren().Count.ToString());

            base.Update(deltaTime);
            var flameColor = new Vector3(226 / 255.0f, 88 / 255.0f, 34 / 255.0f);

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
                    new GameObject("point light").AddComponent(new PointLight(flameColor, 5, new Attenuation(0, 0, 1f)));
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
                    //gObj.AddComponent(new BoxCollider(2,2,2,1));
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

            _camera.Transform.Position = new Vector3(0, 2, 0);

            AddObject(_camera);
        }
    }
}