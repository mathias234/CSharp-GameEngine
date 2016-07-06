using NewEngine.Engine.Core;
using NewEngine.Engine.Rendering;
using NewEngine.Engine.Rendering.Shading;

namespace NewEngine.Engine.components {
    public abstract class GameComponent {
        private GameObject _parent;
        public bool IsEnabled;

        public virtual void Update() {

        }

        public virtual void Render(Shader shader) {

        }

        public virtual void OnEnable() {
            
        }

        public Transform Transform {
            get { return Parent.Transform; }
        }

        public GameObject Parent {
            get { return _parent; }
            set { _parent = value; }
        }

        public virtual void AddToRenderingEngine(RenderingEngine renderingEngine) {

        }
    }
}
