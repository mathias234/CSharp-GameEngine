using NewEngine.Engine.components;
using NewEngine.Engine.Core;
using OpenTK;

namespace NewEngine.Engine.Rendering.Shading {
    public class ForwardPoint : Shader {


        private static ForwardPoint _instance;

        public static ForwardPoint Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new ForwardPoint();

                return _instance;
            }
        }

        public ForwardPoint() : base("forward-point") {
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

            SetUniformPointLight("pointLight", (PointLight)renderingEngine.GetActiveLight);
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
    }
}
