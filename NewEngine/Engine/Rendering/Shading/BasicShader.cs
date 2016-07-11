using NewEngine.Engine.Core;
using OpenTK;

namespace NewEngine.Engine.Rendering.Shading {
    public class BasicShader : Shader {


        private static BasicShader _instance;

        public static BasicShader Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new BasicShader();

                return _instance;
            }
        }

        public BasicShader() {
            AddVertexShaderFromFile("basicVertex.vs");
            AddFragmentShaderFromFile("basicFragment.fs");
            CompileShader();



            AddUniform("transform");
            AddUniform("color");
        }

        public override void UpdateUniforms(Transform transform, Material material) {
            Matrix4 worldMatrix = transform.GetTransformation();
            Matrix4 projectedMatrix = worldMatrix * RenderingEngine.MainCamera.GetViewProjection();

            material.MainTexture.Bind();

            SetUniform("transform", projectedMatrix);
            SetUniform("color", material.Color);
        }
    }
}
