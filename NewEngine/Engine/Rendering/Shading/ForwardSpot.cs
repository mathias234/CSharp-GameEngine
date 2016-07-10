using NewEngine.Engine.components;
using NewEngine.Engine.Core;
using OpenTK;

namespace NewEngine.Engine.Rendering.Shading {
    public class ForwardSpot : Shader {
        private static ForwardSpot _instance;

        public static ForwardSpot Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new ForwardSpot();

                return _instance;
            }
        }

        public ForwardSpot() : base("forward-spot") {
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

            SetUniformSpotLight("spotLight", (SpotLight)renderingEngine.GetActiveLight);
        }

        public void SetUniformBaseLight(string uniformName, BaseLight baseLight) {
            SetUniform(uniformName + ".color", baseLight.Color);
            SetUniform(uniformName + ".intensity", baseLight.Intensity);
        }

        private void SetUniformPointLight(string uniformName, PointLight pointLight) {
            SetUniformBaseLight(uniformName + ".base", pointLight);
            SetUniform(uniformName + ".atten.constant", pointLight.Constant);
            SetUniform(uniformName + ".atten.linear", pointLight.Linear);
            SetUniform(uniformName + ".atten.exponent", pointLight.Exponent);
            SetUniform(uniformName + ".position", pointLight.Transform.GetTransformedPosition());
            SetUniform(uniformName + ".range", pointLight.Range);
        }

        private void SetUniformSpotLight(string uniformName, SpotLight spotLight) {
            SetUniformPointLight(uniformName + ".pointLight", spotLight);
            SetUniform(uniformName + ".direction", spotLight.Direction);
            SetUniform(uniformName + ".cutoff", spotLight.Cutoff);
        }
    }
}
