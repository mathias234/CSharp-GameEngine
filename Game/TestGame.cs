using System;
using System.Drawing;
using NewEngine;
using NewEngine.Engine.components;
using NewEngine.Engine.Core;
using NewEngine.Engine.Rendering;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Game {
    public class TestGame : NewEngine.Engine.Core.Game {
        private GameObject PointLight1;
        private GameObject PointLight2;

        public override void Start() {
            Mesh mesh = new Mesh("plane.obj");

            Material material = new Material(new Texture("test.png"));


            MeshRenderer meshRenderer = new MeshRenderer(mesh, material);

            var planeObject = new GameObject();
            planeObject.AddComponent(meshRenderer);
            planeObject.Transform.Position = new Vector3(0, -1, 5);
            AddObject(planeObject);

            AddObject(new GameObject().AddComponent(new FreeLook()).AddComponent(new FreeMove()).AddComponent(new Camera(Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(70.0f), (float)CoreEngine.GetWidth() / CoreEngine.GetHeight(), 0.1f, 1000))));

            Mesh monkey = new Mesh("monkey3.obj");

            GameObject testMonkey = new GameObject().AddComponent(new MeshRenderer(monkey, material));
            AddObject(testMonkey);


            GameObject testMonkey2 = new GameObject().AddComponent(new MeshRenderer(new Mesh("monkey3.obj"), material));
            AddObject(testMonkey2);
            testMonkey2.Transform.Position = new Vector3(0, 5, 0);

            PointLight1 = new GameObject().AddComponent(new PointLight(new Vector3(1, 0, 0), 0.9f, new Attenuation(0, 1, 0.1f)));
            AddObject(PointLight1);


            PointLight2 = new GameObject().AddComponent(new PointLight(new Vector3(0, 1, 0), 0.9f, new Attenuation(0, 1, 0.1f)));
            AddObject(PointLight2);

            GameObject directionalLightObj = new GameObject();
            DirectionalLight directionalLight = new DirectionalLight(new Vector3(1), 0.7f);
            directionalLightObj.AddComponent(directionalLight);
            directionalLightObj.Transform.Rotation = Quaternion.FromAxisAngle(new Vector3(1, 0, 0), MathHelper.DegreesToRadians(-30f));
            AddObject(directionalLightObj);
        }


        private float temp;
        public override void Update(float deltaTime) {
            base.Update(deltaTime);
            temp += deltaTime;

            float sinTemp = (float)Math.Sin(temp) * 5;
            
            PointLight1.Transform.Position = new Vector3(sinTemp, 0,0);
            PointLight2.Transform.Position = new Vector3(sinTemp, 0,10);
        }
    }
}
