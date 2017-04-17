using System.Collections.Generic;
using System.Linq;
using NewEngine.Engine.components;
using NewEngine.Engine.Rendering;

namespace NewEngine.Engine.Core {
    public class GameObject {
        private List<GameObject> _children;
        private List<GameComponent> _components;
        private ICoreEngine _engine;
        private GameObject _parent;
        public bool IsActive; // TODO: ludumdare hack

        public GameObject(string name) {
            _children = new List<GameObject>();
            _components = new List<GameComponent>();
            Transform = new Transform();
            Transform.Name = name;
            Name = name;
            IsActive = true;
            Engine = null;
        }

        public Transform Transform { get; set; }
        public string Name { get; set; }

        public ICoreEngine Engine {
            get { return _engine; }
            set {
                if (_engine == value) return;
                _engine = value;
                for (var i = 0; i < _components.Count; i++) {
                    _components[i].AddToEngine(_engine);
                }

                for (var i = 0; i < _children.Count; i++) {
                    _children[i].Engine = _engine;
                }
            }
        }

        public GameObject Parent {
            get { return _parent; }
            set { _parent = value; }
        }

        public void AddChild(GameObject child) {
            _children.Add(child);
            child._parent = this;
            child.Engine = Engine;
            child.Transform.Parent = Transform;
        }

        public GameObject AddComponent(GameComponent component) {
            _components.Add(component);
            component.gameObject = this;
            return this;
        }

        public T GetComponent<T>() where T : GameComponent {
            return _components.Where(component1 => component1.GetType() == typeof(T)).Cast<T>().FirstOrDefault();
        }

        public void ClearComponent(GameComponent component) {
            component.OnDestroyed(_engine);
            _components.Remove(component);
        }

        public void UpdateAll(float deltaTime) {
            Update(deltaTime);

            //TODO: find a more elegant solution
            for (var i = 0; i < _components.Count; i++) {
                if (_components[i].IsEnabled) continue;
                _components[i].OnEnable();
                _components[i].IsEnabled = true;
            }

            for (var i = 0; i < _children.Count; i++) {
                _children[i].UpdateAll(deltaTime);
            }
        }

        public void RenderAll(string shader, string shaderType, float deltaTime, RenderingEngine renderingEngine,string renderStage) {
            Render(shader, shaderType, deltaTime, renderingEngine, renderStage);
            for (var i = 0; i < _children.Count; i++) {
                _children[i].RenderAll(shader, shaderType, deltaTime, renderingEngine, renderStage);
            }
        }

        public void Update(float delta) {
            for (var i = 0; i < _components.Count; i++) {
                _components[i].Update(delta);
            }
        }

        public void Render(string shader, string shaderType, float deltaTime, RenderingEngine renderingEngine, string renderStage) {
            for (var i = 0; i < _components.Count; i++) {
                _components[i].Render(shader, shaderType, deltaTime, renderingEngine, renderStage);
            }
        }

        // recursivly ask all the components to add itself to the rendering engine for rendering
        public void AddToEngine(ICoreEngine engine) {
            foreach (var gameComponent in _components) {
                gameComponent.AddToEngine(engine);
            }

            foreach (var gameObject in _children) {
                gameObject.AddToEngine(engine);
            }
        }

        public List<GameObject> GetAllAttached() {
            var result = new List<GameObject> {this};
            for (var i = 0; i < _children.Count; i++) {
                var child = _children[i];
                result.AddRange(child.GetAllAttached());
            }
            return result;
        }


        public List<GameObject> GetChildren() {
            return _children;
        }

        public List<GameComponent> GetComponents() {
            return _components;
        }

        public void Destroy() {
            foreach (var gameComponent in _components) {
                gameComponent.OnDestroyed(_engine);
            }
            foreach (var gameObject in _children) {
                gameObject.Destroy();
            }

            _components.Clear();
            _children.Clear();

            _parent._children.Remove(this);

        }
    }
}