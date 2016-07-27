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

        private TerrainMesh _tMesh;

        public override void Start() {
            //Mesh terrainMesh = new Mesh("ShadowTest.obj");

            //GameObject shadowTest =
            //    new GameObject().AddComponent(new MeshRenderer(terrainMesh,
            //        new Material(new Texture("bricks.png"), 0.5f, 32, new Texture("bricks_nrm.png"), new Texture("bricks_disp.jpg"), 0.02f, 0)));

            //shadowTest.AddComponent(new MeshCollider(terrainMesh));
            //AddObject(shadowTest);


            _camera =
                new GameObject().AddComponent(new FreeLook())
                    .AddComponent(new FreeMove())
                    .AddComponent(
                        new Camera(Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(70.0f),
                            CoreEngine.GetWidth()/CoreEngine.GetHeight(), 0.1f, 1000)));

            _camera.Transform.Position = new Vector3(0, 5, 0);

            AddObject(_camera);

            _directionalLightObj = new GameObject();
            var directionalLight = new DirectionalLight(new Vector3(1), 0.5f);
            _directionalLightObj.AddComponent(directionalLight);
            _directionalLightObj.Transform.Rotation *= Quaternion.FromAxisAngle(new Vector3(1, 0, 0), -0.7f);
            AddObject(_directionalLightObj);
            CoreEngine.GetCoreEngine.RenderingEngine.SetSkybox("skybox1/top.jpg", "skybox1/bottom.jpg",
                "skybox1/front.jpg", "skybox1/back.jpg", "skybox1/left.jpg", "skybox1/right.jpg");
            CoreEngine.GetCoreEngine.RenderingEngine.SetSkybox("skybox/top.jpg", "skybox/bottom.jpg", "skybox/front.jpg",
                "skybox/back.jpg", "skybox/left.jpg", "skybox/right.jpg");


            var terrain = new GameObject();
            _tMesh = new TerrainMesh("terrain1/terrain.jpg", 500, 500, 0.3f, "terrain1/tex1.jpg", "default_normal.png",
                "terrain1/tex2.jpg", "terrain1/tex2Nrm.jpg",
                "terrain1/layer1.jpg", "terrain1/tex3.jpg", "terrain1/tex3Nrm.jpg", "terrain1/layer2.jpg");
            terrain.AddComponent(_tMesh);
            AddObject(terrain);

            //GameObject uiTest = new GameObject();
            //_fps = new NewEngine.Engine.components.UIComponents.Text(new RectTransform(300, 300, 0, 0), "This is Text", 30);
            //uiTest.AddComponent(_fps);

            //AddObject(uiTest);
        }

        public override void Update(float deltaTime) {
            base.Update(deltaTime);
            var flameColor = new Vector3(226/255.0f, 88/255.0f, 34/255.0f);
            if (Input.GetKeyDown(Key.Q)) {
                CoreEngine.GetCoreEngine.RenderingEngine.SetSkybox("skybox1/top.jpg", "skybox1/bottom.jpg",
                    "skybox1/front.jpg", "skybox1/back.jpg", "skybox1/left.jpg", "skybox1/right.jpg");
            }
            if (Input.GetKeyDown(Key.E)) {
                CoreEngine.GetCoreEngine.RenderingEngine.SetSkybox("skybox/top.jpg", "skybox/bottom.jpg",
                    "skybox/front.jpg", "skybox/back.jpg", "skybox/left.jpg", "skybox/right.jpg");
            }
            if (!Input.GetKeyDown(Key.P)) return;
            var newPointLight =
                new GameObject().AddComponent(new PointLight(flameColor, 5, new Attenuation(0, 0, 1f)));
            newPointLight.Transform.Position =
                new Vector3(CoreEngine.GetCoreEngine.RenderingEngine.MainCamera.Transform.GetTransformedPosition());
            AddObject(newPointLight);
        }

        public void StartMassiveSpawn() {
            for (var x = -5; x < 5; x++) {
                for (var z = -5; z < 5; z++) {
                    var gObj = new GameObject {
                        Transform = {Position = _camera.Transform.Position + new Vector3(5*x, 0, 5*z)}
                    };
                    gObj.AddComponent(new MeshRenderer(new Mesh("sphere.obj"), new Material(new Texture("test.png"))));
                    gObj.AddComponent(new SphereCollider(1, 2));
                    AddObject(gObj);
                }
            }
        }
    }
}