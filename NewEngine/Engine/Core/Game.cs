using NewEngine.Engine.Rendering;

namespace NewEngine.Engine.Core {
    public abstract class Game {
        private GameObject _root = new GameObject("root");

        public virtual void Start() {
        }

        public virtual void Update(float deltaTime) {
            GetRootObject.UpdateAll(deltaTime);
        }

        public void AddObject(GameObject gObj) {
            gObj.Parent = _root;
            _root.AddChild(gObj);
        }

        public GameObject GetRootObject => _root ?? (_root = new GameObject("root"));

        public void SetEngine(CoreEngine coreEngine) {
            GetRootObject.Engine = coreEngine;
        }

        protected void Destory(GameObject gameObject) {
            gameObject.Destroy();
        }
    }
}
