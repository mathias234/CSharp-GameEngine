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

        public ForwardSpot() {
            AddVertexShaderFromFile("forward-spot.vs");
            AddFragmentShaderFromFile("forward-spot.fs");

            CompileShader();

            AddUniform("model");
            AddUniform("MVP");

            AddUniform("specularIntensity");
            AddUniform("specularPower");
            AddUniform("eyePos");

            AddUniform("spotLight.pointLight.base.color");
            AddUniform("spotLight.pointLight.base.intensity");
            AddUniform("spotLight.pointLight.atten.constant");
            AddUniform("spotLight.pointLight.atten.linear");
            AddUniform("spotLight.pointLight.atten.exponent");
            AddUniform("spotLight.pointLight.position");
            AddUniform("spotLight.pointLight.range");

            AddUniform("spotLight.direction");
            AddUniform("spotLight.cutoff");

        }

        public override void UpdateUniforms(Transform transform, Material material) {
            Matrix4 worldMatrix = transform.GetTransformation();
            Matrix4 projectedMatrix = worldMatrix * RenderingEngine.MainCamera.GetViewProjection();

            material.MainTexture.Bind();

            SetUniform("model", worldMatrix);
            SetUniform("MVP", projectedMatrix);

            SetUniform("specularIntensity", material.SpecularIntensity);
            SetUniform("specularPower", material.SpecularPower);

            SetUniform("eyePos", RenderingEngine.MainCamera.Position);

            SetUniformSpotLight("spotLight", (SpotLight) RenderingEngine.GetActiveLight);
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
            SetUniform(uniformName + ".position", pointLight.Transform.Position);
            SetUniform(uniformName + ".range", pointLight.Range);
        }

        private void SetUniformSpotLight(string uniformName, SpotLight spotLight) {
            SetUniformPointLight(uniformName + ".pointLight", spotLight);
            SetUniform(uniformName + ".direction", spotLight.Direction);
            SetUniform(uniformName + ".cutoff", spotLight.Cutoff);
        }
    }
}
