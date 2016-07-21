using NewEngine.Engine.Core;
using NewEngine.Engine.Rendering;
using NewEngine.Engine.Rendering.Shading;

namespace NewEngine.Engine.components {
    public class MeshRenderer : GameComponent {
        private Mesh _mesh;
        private Material _material;

        public MeshRenderer(Mesh mesh, Material material = null) {
            _mesh = mesh;
            _material = material;
        }

        public Material Material
        {
            get { return _material; }
            set { _material = value; }
        }

        public Mesh Mesh
        {
            get { return _mesh; }
            set { _mesh = value; }
        }

        public override void Render(Shader shader, RenderingEngine renderingEngine) {
            shader.Bind();
            shader.UpdateUniforms(new Transform(), _material, renderingEngine);
            _mesh.Draw();
        }
    }
}
