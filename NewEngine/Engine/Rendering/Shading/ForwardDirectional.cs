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

        public ForwardDirectional() {
            AddVertexShaderFromFile("forward-directional.vs");
            AddFragmentShaderFromFile("forward-directional.fs");

            CompileShader();


            AddUniform("model");
            AddUniform("MVP");

            AddUniform("specularIntensity");
            AddUniform("specularPower");
            AddUniform("eyePos");

            AddUniform("directionalLight.base.color");
            AddUniform("directionalLight.base.intensity");
            AddUniform("directionalLight.direction");

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
            SetUniformDirectionalLight("directionalLight", (DirectionalLight)RenderingEngine.GetActiveLight);
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
