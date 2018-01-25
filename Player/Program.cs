using NewEngine.Engine.Audio;
using NewEngine.Engine.components;
using NewEngine.Engine.Core;
using NewEngine.Engine.Rendering;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Player
{
    class Program
    {
        static Player player;

        static GameObject _camera;

        static void Main(string[] args)
        {
            using (player = new Player(800, 600, OpenTK.VSyncMode.On, Start)) {
                player.CreateWindow("Test");
                player.Start();
            }
        }

        static void Start(float deltaTime) {
            CreateCamera();

            GameObject obj = new GameObject("TEST");
            obj.AddComponent(new MeshRenderer("cube.obj", "null"));

            player.AddObject(obj);

            RenderingEngine.Instance.SetSkybox("skybox/top.jpg", "skybox/bottom.jpg", "skybox/front.jpg",
                                                "skybox/back.jpg", "skybox/left.jpg", "skybox/right.jpg");
        }

        static void CreateCamera()
        {
            _camera = new GameObject("main camera")
                .AddComponent(new FreeLook(true, true))
                .AddComponent(new FreeMove())
                .AddComponent(new Camera(Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(70.0f), RenderingEngine.GetWidth() / RenderingEngine.GetHeight(), 0.1f, 1000)))
                .AddComponent(new AudioListener());

            _camera.Transform.Rotate(new Vector3(1, 0, 0), -0.4f);

            player.AddObject(_camera);

        }
    }
}
