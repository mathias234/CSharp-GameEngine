using NewEngine.Engine.Rendering.ResourceManagament;
using NewEngine.Engine.Rendering.Shading;

namespace NewEngine.Engine.Rendering {
    public class Material : MappedValues {
        public Material(Shader shader) {
            Shader = shader;
        }

        public Shader Shader { get; set; }

        /// <summary>
        /// Equivalent to SetTexture("diffuse", texture);
        /// </summary>
        /// <param name="texture"></param>
        public void SetMainTexture(Texture texture) {
            SetTexture("diffuse", texture);
        }
    }
}