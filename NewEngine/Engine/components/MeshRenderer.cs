using System.Collections.Generic;
using NewEngine.Engine.Core;
using NewEngine.Engine.Rendering;
using NewEngine.Engine.Rendering.ResourceManagament;
using NewEngine.Engine.Rendering.Shading;

namespace NewEngine.Engine.components {
    public class MeshRenderer : GameComponent {
        public MeshRenderer(Mesh mesh, Material material = null) {
            Mesh = mesh;
            Material = material;
        }

        public Material Material { get; set; }

        public Mesh Mesh { get; set; }

        public override void AddToEngine(CoreEngine engine) {
            base.AddToEngine(engine);

            engine.RenderingEngine.AddObjectToBatch(Material, Mesh, gameObject);
        }

        public override void OnDestroyed(CoreEngine engine) {
            base.OnDestroyed(engine);

            engine.RenderingEngine.RemoveFromBatch(Material, Mesh, gameObject);
        }
    }
}