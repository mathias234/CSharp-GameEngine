using NewEngine.Engine.Core;
using NewEngine.Engine.Rendering;

namespace NewEngine.Engine.components {
    public abstract class GameComponent {
        public bool IsEnabled;

        public Transform Transform => Parent.Transform;

        public GameObject Parent { get; set; }

        public virtual void Update(float deltaTime) {}

        public virtual void Render(string shader, RenderingEngine renderingEngine, bool baseShader) {}

        public virtual void OnEnable() {}

        public virtual void OnDestroyed() {}

        public virtual void AddToEngine(CoreEngine engine) {}
    }
}