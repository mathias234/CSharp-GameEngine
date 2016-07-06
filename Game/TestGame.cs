using System.Drawing;
using NewEngine.Engine.components;
using NewEngine.Engine.Core;
using NewEngine.Engine.Rendering;
using NewEngine.Engine.Rendering.Shading;
using OpenTK;

namespace Game {
    public class TestGame : NewEngine.Engine.Core.Game {
        public override void Start() {

            const float fieldDepth = 100.0f;
            const float fieldWidth = 100.0f;


            Vertex[] vertices = new Vertex[] {  new Vertex( new Vector3(-fieldWidth, 0.0f, -fieldDepth), new Vector2(0.0f, 0.0f)),
                                            new Vertex( new Vector3(-fieldWidth, 0.0f, fieldDepth * 3), new Vector2(0.0f, 1.0f)),
                                            new Vertex( new Vector3(fieldWidth * 3, 0.0f, -fieldDepth), new Vector2(1.0f, 0.0f)),
                                            new Vertex( new Vector3(fieldWidth * 3, 0.0f, fieldDepth * 3), new Vector2(1.0f, 1.0f))};

            int[] indices = { 0, 1, 2,
                          2, 1, 3};


            Mesh mesh = new Mesh(vertices, indices, true);
            Material material = new Material(new Texture("nrm1.png"), Color.White, 1, 8);

            MeshRenderer meshRenderer = new MeshRenderer(mesh, material);

            var planeObject = new GameObject();
            planeObject.AddComponent(meshRenderer);
            planeObject.Transform.Position = new Vector3(0, -1, 5);

            GetRootObject.AddChild(planeObject);

            GameObject directionalLightObj = new GameObject();
            DirectionalLight directionalLight = new DirectionalLight(new Vector3(1), 0.7f, new Vector3(1));
            directionalLightObj.AddComponent(directionalLight);
            GetRootObject.AddChild(directionalLightObj);

            GameObject pointLightObj = new GameObject();
            pointLightObj.Transform.Position = new Vector3(5, 0, 0);
            pointLightObj.AddComponent(new PointLight(new Vector3(0, 1, 0), 0.7f,
                new Vector3(0, 0, 0.5f)));
            GetRootObject.AddChild(pointLightObj);

            GameObject spotLightObj = new GameObject();
            spotLightObj.Transform.Position = new Vector3(10, 0, 0);
            spotLightObj.AddComponent(new SpotLight(new Vector3(0, 1, 0), 0.7f,
                new Vector3(0, 0, 0.5f), 0.7f));

            spotLightObj.Transform.Rotation = Quaternion.FromAxisAngle(new Vector3(0,1,0), MathHelper.DegreesToRadians(90.0f));

            GetRootObject.AddChild(spotLightObj);
        }
    }
}
