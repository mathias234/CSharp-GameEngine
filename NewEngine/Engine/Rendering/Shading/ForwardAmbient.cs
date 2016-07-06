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

        public ForwardAmbient() {
            AddVertexShaderFromFile("forward-ambient.vs");
            AddFragmentShaderFromFile("forward-ambient.fs");

            CompileShader();

            AddUniform("MVP");
            AddUniform("ambientIntensity");
        }

        public override void UpdateUniforms(Transform transform, Material material) {
            Matrix4 worldMatrix = transform.GetTransformation();
            Matrix4 projectedMatrix = worldMatrix * RenderingEngine.MainCamera.GetViewProjection();

            material.MainTexture.Bind();

            SetUniform("MVP", projectedMatrix);
            SetUniform("ambientIntensity", RenderingEngine.GetAmbientLight);
        }
    }
}
