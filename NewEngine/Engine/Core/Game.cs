namespace NewEngine.Engine.Core {
    public abstract class Game {
        private GameObject _root;

        public virtual void Start() {
        }

        public virtual void Update(float deltaTime) {
            GetRootObject.Update();
        }

        public GameObject GetRootObject
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
