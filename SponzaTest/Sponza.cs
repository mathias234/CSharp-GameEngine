using System;
using NewEngine.Engine.components;
using NewEngine.Engine.Core;
using NewEngine.Engine.Rendering;
using OpenTK;

namespace Game {
    public class Sponza : NewEngine.Engine.Core.Game {
        private GameObject PointLight1;
        private GameObject PointLight2;

        public override void Start() {
            AddObject(new GameObject().AddComponent(new FreeLook()).AddComponent(new FreeMove()).AddComponent(new Camera(MathHelper.DegreesToRadians(70.0f), (float)CoreEngine.GetWidth() / CoreEngine.GetHeight(), 0.1f, 1000)));

            GameObject directionalLightObj = new GameObject();
            DirectionalLight directionalLight = new DirectionalLight(new Vector3(1), 0.7f);
            directionalLightObj.AddComponent(directionalLight);
            directionalLightObj.Transform.Rotation = Quaternion.FromAxisAngle(new Vector3(1, 0, 0), MathHelper.DegreesToRadians(-30f));
            AddObject(directionalLightObj);


            string[] sponzaModels = new[] { "blue_flags", "red_flags", "green_flags", "floor_plants", "columns", "arch", "leaves", "lion", "ceiling", "bricks", "floor",
                "blue_fabric", "green_fabric", "red_fabric", "vase_hanging","chains", "columns1", "roof", "flagpole", "details", "columns2", "background", "floor_plants_plant" };

            foreach (var sponzaModel in sponzaModels) {
                Material material = new Material();
                material.AddTexture("diffuse", new Texture("sponza/" + sponzaModel + ".png"));
                material.AddTexture("normalMap", new Texture("sponza/" + sponzaModel + "_nrm.png"));
                material.AddFloat("specularIntensity", 0.5f);
                material.AddFloat("specularPower", 32);
                GameObject sponza = new GameObject().AddComponent(new MeshRenderer(new Mesh("sponza/" + sponzaModel + "/model.obj"), material));
                AddObject(sponza);
            }


        }


        public override void Update(float deltaTime) {
            base.Update(deltaTime);
        }
    }
}
