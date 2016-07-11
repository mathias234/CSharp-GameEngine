using System.Drawing;
using NewEngine.Engine.components;
using NewEngine.Engine.components.UIComponents;
using NewEngine.Engine.Core;
using NewEngine.Engine.Rendering;
using NewEngine.Engine.Rendering.Shading;
using OpenTK;
using OpenTK.Input;
using Image = NewEngine.Engine.components.UIComponents.Image;

namespace Game {
    public class TestGame : NewEngine.Engine.Core.Game {
        private GameObject planeObject2;
        public override void Start() {


            const float fieldDepth = 100.0f;
            const float fieldWidth = 100.0f;


            Vertex[] vertices = new Vertex[] {  new Vertex( new Vector3(-fieldWidth, 0.0f, -fieldDepth), new Vector2(0.0f, 0.0f)),
                                            new Vertex( new Vector3(-fieldWidth, 0.0f, fieldDepth * 3), new Vector2(0.0f, 1.0f)),
                                            new Vertex( new Vector3(fieldWidth * 3, 0.0f, -fieldDepth), new Vector2(1.0f, 0.0f)),
                                            new Vertex( new Vector3(fieldWidth * 3, 0.0f, fieldDepth * 3), new Vector2(1.0f, 1.0f))};

            const float fieldDepth2 = 1;
            const float fieldWidth2 = 1;

            Vertex[] vertices2 = new Vertex[] {  new Vertex( new Vector3(-fieldWidth2, 0.0f, -fieldDepth2), new Vector2(0.0f, 0.0f)),
                                            new Vertex( new Vector3(-fieldWidth2, 0.0f, fieldDepth2 * 3), new Vector2(0.0f, 1.0f)),
                                            new Vertex( new Vector3(fieldWidth2 * 3, 0.0f, -fieldDepth2), new Vector2(1.0f, 0.0f)),
                                            new Vertex( new Vector3(fieldWidth2 * 3, 0.0f, fieldDepth2 * 3), new Vector2(1.0f, 1.0f))};

            int[] indices = { 0, 1, 2,
                          2, 1, 3};


            Mesh mesh = new Mesh(vertices, indices, true);
            Mesh mesh2 = new Mesh(vertices2, indices, true);

            Material material = new Material();
            material.AddTexture("diffuse", new Texture("test.png"));
            material.AddFloat("specularIntensity", 1);
            material.AddFloat("specularPower", 8);

            MeshRenderer meshRenderer = new MeshRenderer(mesh, material);
            MeshRenderer meshRenderer2 = new MeshRenderer(mesh2, material);
            MeshRenderer meshRenderer3 = new MeshRenderer(mesh2, material);



            var planeObject = new GameObject();
            planeObject.AddComponent(meshRenderer);
            planeObject.Transform.Position = new Vector3(0, -1, 5);
            AddObject(planeObject);


            planeObject2 = new GameObject();
            planeObject2.AddComponent(meshRenderer2);
            planeObject2.Transform.Position = new Vector3(0, 2, 0);

            AddObject(planeObject2);

            var planeObject3 = new GameObject();
            planeObject3.AddComponent(meshRenderer3);
            planeObject3.Transform.Position = new Vector3(0, 0, 5);
            planeObject2.AddChild(planeObject3);

            planeObject2.Transform.Rotation = Quaternion.FromAxisAngle(new Vector3(0, 1, 0), 0.4f);



            GameObject directionalLightObj = new GameObject();
            DirectionalLight directionalLight = new DirectionalLight(new Vector3(1), 0.7f);
            directionalLightObj.AddComponent(directionalLight);
            AddObject(directionalLightObj);

            GameObject pointLightObj = new GameObject();
            pointLightObj.Transform.Position = new Vector3(5, 0, 0);
            pointLightObj.AddComponent(new PointLight(new Vector3(0, 1, 0), 0.7f,
                new Vector3(0, 0, 0.5f)));
            AddObject(pointLightObj);

            GameObject spotLightObj = new GameObject();
            spotLightObj.Transform.Position = new Vector3(10, 0, 0);
            spotLightObj.AddComponent(new SpotLight(new Vector3(0, 1, 0), 0.7f,
                new Vector3(0, 0, 0.5f), 0.7f));

            spotLightObj.Transform.Rotation = Quaternion.FromAxisAngle(new Vector3(0, 1, 0), MathHelper.DegreesToRadians(90.0f));

            AddObject(spotLightObj);


            AddObject(new GameObject().AddComponent(new Camera(MathHelper.DegreesToRadians(70.0f), (float)CoreEngine.GetWidth() / CoreEngine.GetHeight(), 0.1f, 1000)));

            directionalLightObj.Transform.Rotation = Quaternion.FromAxisAngle(new Vector3(1, 0, 0), MathHelper.DegreesToRadians(-45));

            Mesh monkey = new Mesh("monkey3.obj");

            GameObject testMonkey = new GameObject().AddComponent(new MeshRenderer(monkey, material));
            AddObject(testMonkey);


            GameObject testMonkey2 = new GameObject().AddComponent(new MeshRenderer(new Mesh("monkey3.obj"), material));
            AddObject(testMonkey2);
            testMonkey2.Transform.Position = new Vector3(0, 5, 0);


            Image t = new Image(new RectTransform(200, 20, 200, 100), new Texture((Bitmap) null));
            GameObject testObject = new GameObject().AddComponent(t);
            AddObject(testObject);
        }

        private bool mouseLocked = false;

        public override void Update(float deltaTime) {
            base.Update(deltaTime);
        }
    }
}
