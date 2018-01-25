using System.Collections.Generic;
using NewEngine.Engine.Core;
using NewEngine.Engine.Rendering;
using NewEngine.Engine.Rendering.ResourceManagament;
using NewEngine.Engine.Rendering.Shading;

namespace NewEngine.Engine.components
{
    public class MeshRenderer : GameComponent
    {
        public MeshRenderer(string mesh, string material)
        {
            Mesh = NewEngine.Engine.Rendering.Mesh.GetMesh(mesh);
            Material = new Material(Shader.GetShader("batchedShader"));
        }

        public MeshRenderer(Mesh mesh, Material material = null)
        {
            Mesh = mesh;
            Material = material;
        }

        public Material Material { get; set; }

        public Mesh Mesh { get; set; }

        public override void Render(string shader, string shaderType, float deltaTime, BaseRenderingEngine renderingEngine, string renderStage)
        {
            if (!Material.Shader.GetShaderTypes.Contains(shaderType))
                return;

            Material.Shader.Bind(shaderType);
            Material.Shader.UpdateUniforms(Transform, Material, renderingEngine, shaderType);
            Mesh.Draw();
        }

        public override void AddToEngine(ICoreEngine engine) {
            engine.RenderingEngine.AddToEngine(this);
        }
    }
}