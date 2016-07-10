using NewEngine.Engine.Rendering;

namespace NewEngine.Engine.Core {
    public abstract class Game {
        private GameObject _root = new GameObject();

        public virtual void Start() {
        }

        public virtual void Update(float deltaTime) {
            GetRootObject.Update();
        }

        public virtual void Render(RenderingEngine renderingEngine) {
            renderingEngine.Render(GetRootObject);
        }

        public void AddObject(GameObject gObj) {
            _root.AddChild(gObj);
        }

        private GameObject GetRootObject
        {
            get
            {
                if (_root == null)
                    _root = new GameObject();
                return _root;
            }
        }
    }
}
