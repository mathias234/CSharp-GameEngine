using NewEngine.Engine.components;
using NewEngine.Engine.Core;
using OpenTK;

namespace NewEngine.Engine.Rendering.Shading {
    public class ForwardDirectional : Shader {


        private static ForwardDirectional _instance;

        public static ForwardDirectional Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new ForwardDirectional();

                return _instance;
            }
        }

        public ForwardDirectional() : base("forward-directional") {
        }

        public override void UpdateUniforms(Transform transform, Material material, RenderingEngine renderingEngine) {
            Matrix4 worldMatrix = transform.GetTransformation();
            Matrix4 projectedMatrix = worldMatrix * renderingEngine.MainCamera.GetViewProjection();

            material.GetTexture("diffuse").Bind();

            SetUniform("model", worldMatrix);
            SetUniform("MVP", projectedMatrix);

            SetUniform("specularIntensity", material.GetFloat("specularIntensity"));
            SetUniform("specularPower", material.GetFloat("specularPower"));

            SetUniform("eyePos", renderingEngine.MainCamera.Transform.GetTransformedPosition());
            SetUniformDirectionalLight("directionalLight", (DirectionalLight)renderingEngine.GetActiveLight);
        }


        public void SetUniformBaseLight(string uniformName, BaseLight baseLight) {
            SetUniform(uniformName + ".color", baseLight.Color);
            SetUniform(uniformName + ".intensity", baseLight.Intensity);
        }

        public void SetUniformDirectionalLight(string uniformName, DirectionalLight directionalLight) {
            SetUniformBaseLight(uniformName + ".base", directionalLight);
            SetUniform(uniformName + ".direction", directionalLight.Direction);
        }

    }
}
