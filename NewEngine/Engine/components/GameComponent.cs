using NewEngine.Engine.Core;
using NewEngine.Engine.Rendering;

namespace NewEngine.Engine.components {
    public abstract class GameComponent {
        public bool IsEnabled;

        public Transform Transform => gameObject.Transform;

        public GameObject gameObject;

        public virtual void Update(float deltaTime) {}

        public virtual void Render(string shader, string shaderType, float deltaTime, RenderingEngine renderingEngine, string renderStage) {}

        public virtual void OnEnable() {}

        public virtual void AddToEngine(CoreEngine engine) {}

        public virtual void OnDestroyed(CoreEngine engine) {

        }
    }
}