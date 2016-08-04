using System.Collections.Generic;
using NewEngine.Engine.Core;
using NewEngine.Engine.Rendering;
using NewEngine.Engine.Rendering.ResourceManagament;
using NewEngine.Engine.Rendering.Shading;

namespace NewEngine.Engine.components {
    public class MeshRenderer : GameComponent {
        private Dictionary<string, Shader> _loadedShaders = new Dictionary<string, Shader>();
        private Shader _baseShader;

        public MeshRenderer(Mesh mesh, Material material = null) {
            Mesh = mesh;
            Material = material;
            _baseShader = new Shader("forward-ambient");
        }

        public Material Material { get; set; }

        public Mesh Mesh { get; set; }

        public override void Render(string shader, string shaderType, float deltaTime, RenderingEngine renderingEngine) {
            Shader shaderToUse;
            
            if (shaderType == "base") {
                shaderToUse = _baseShader;
            }
            else if (shaderType != "light") {
                return;
            }
            else if (_loadedShaders.ContainsKey(shader)) {
                shaderToUse = _loadedShaders[shader];
            }
            else {
                shaderToUse = new Shader("forward-" + shader);
                _loadedShaders.Add(shader, shaderToUse);
            }

            shaderToUse.Bind();
            shaderToUse.UpdateUniforms(Parent.Transform, Material, renderingEngine);
            Mesh.Draw();
        }

        public override void AddToEngine(CoreEngine engine) {
            base.AddToEngine(engine);
            engine.RenderingEngine.AddBatch(Mesh, Parent);
        }

        public override void OnDestroyed(CoreEngine engine) {
            base.OnDestroyed(engine);
            engine.RenderingEngine.RemoveBatch(Mesh, Parent);
        }
    }
}