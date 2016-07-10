using NewEngine.Engine.Core;
using OpenTK;

namespace NewEngine.Engine.Rendering.Shading {
    public class ForwardAmbient : Shader {


        private static ForwardAmbient _instance;

        public static ForwardAmbient Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new ForwardAmbient();

                return _instance;
            }
        }

        public ForwardAmbient() : base("forward-ambient") {

        }

        public override void UpdateUniforms(Transform transform, Material material, RenderingEngine renderingEngine) {
            if(renderingEngine.MainCamera == null)
                return;

            Matrix4 worldMatrix = transform.GetTransformation();
            Matrix4 projectedMatrix = worldMatrix * renderingEngine.MainCamera.GetViewProjection();

            material.GetTexture("diffuse").Bind();

            SetUniform("MVP", projectedMatrix);
            SetUniform("ambientIntensity", renderingEngine.GetAmbientLight);
        }
    }
}
