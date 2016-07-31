using NewEngine.Engine.Core;
using NewEngine.Engine.Rendering;
using NewEngine.Engine.Rendering.Shading;

namespace NewEngine.Engine.components {
    public class MeshRenderer : GameComponent {
        public MeshRenderer(Mesh mesh, Material material = null) {
            Mesh = mesh;
            Material = material;
        }

        public Material Material { get; set; }

        public Mesh Mesh { get; set; }

        public override void Render(string shader, RenderingEngine renderingEngine, bool baseShader) {
            var shaderToUse = baseShader ? new Shader("forward-ambient") : new Shader("forward-" + shader);

            shaderToUse.Bind();
            shaderToUse.UpdateUniforms(Parent.Transform, Material, renderingEngine);
            Mesh.Draw();
        }
    }
}