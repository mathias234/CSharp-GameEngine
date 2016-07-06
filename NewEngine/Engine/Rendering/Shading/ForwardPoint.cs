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

        public ForwardPoint() {
            AddVertexShaderFromFile("forward-point.vs");
            AddFragmentShaderFromFile("forward-point.fs");

            CompileShader();

            AddUniform("model");
            AddUniform("MVP");

            AddUniform("specularIntensity");
            AddUniform("specularPower");
            AddUniform("eyePos");

            AddUniform("pointLight.base.color");
            AddUniform("pointLight.base.intensity");
            AddUniform("pointLight.atten.constant");
            AddUniform("pointLight.atten.linear");
            AddUniform("pointLight.atten.exponent");
            AddUniform("pointLight.position");
            AddUniform("pointLight.range");
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

            SetUniformPointLight("pointLight", (PointLight)RenderingEngine.GetActiveLight);
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
    }
}
