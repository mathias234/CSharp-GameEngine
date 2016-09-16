using NewEngine.Engine.Rendering.ResourceManagament;
using NewEngine.Engine.Rendering.Shading;

namespace NewEngine.Engine.Rendering {
    public class Material : MappedValues {
        public Material(Shader shader) {
            Shader = shader;
        }

        public Shader Shader { get; set; }

        /*
        public Material(Texture diffuse, float specularIntensity = 0.5f, float specularPower = 32.0f,
            Texture normalMap = null, Texture dispMap = null, float dispScale = 0.0f, float dispMapOffset = 0.0f) {
            SetTexture("diffuse", diffuse);

            SetTexture("normalMap", normalMap ?? new Texture("default_normal.png"));

            SetTexture("dispMap", dispMap ?? new Texture("default_disp.jpg"));

            SetFloat("dispMapScale", dispScale);

            var baseBias = GetFloat("dispMapScale") / 2.0f;

            SetFloat("dispMapBias", -baseBias + baseBias * dispMapOffset);

            SetFloat("specularIntensity", specularIntensity);
            SetFloat("specularPower", specularPower);
        }
        */

        /// <summary>
        /// Equivalent to SetTexture("diffuse", texture);
        /// </summary>
        /// <param name="texture"></param>
        public void SetMainTexture(Texture texture) {
            SetTexture("diffuse", texture);
        }
    }
}