using System.Collections.Generic;
using System.Linq;
using NewEngine.Engine.components;
using NewEngine.Engine.Rendering;

namespace NewEngine.Engine.Core {
    public class GameObject {
        private List<GameObject> _children;
        private List<GameComponent> _components;
        private CoreEngine _engine;

        public GameObject() {
            _children = new List<GameObject>();
            _components = new List<GameComponent>();
            Transform = new Transform();
            Engine = null;
        }

        public Transform Transform { get; set; }

        public CoreEngine Engine {
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

        public void AddChild(GameObject child) {
            _children.Add(child);
            child.Engine = Engine;
            child.Transform.Parent = Transform;
        }

        public GameObject AddComponent(GameComponent component) {
            _components.Add(component);
            component.Parent = this;
            return this;
        }

        public T GetComponent<T>() where T : GameComponent {
            return _components.Where(component1 => component1.GetType() == typeof(T)).Cast<T>().FirstOrDefault();
        }

        public void ClearComponent(GameComponent component) {
            component.OnDestroyed();
            _components.Remove(component);
        }

        public void UpdateAll(float deltaTime) {
            Update(deltaTime);

            //TODO: find a more elegant solution
            for (var i = 0; i < _components.Count; i++) {
                if (_components[i].IsEnabled != false) continue;
                _components[i].OnEnable();
                _components[i].IsEnabled = true;
            }

            for (var i = 0; i < _children.Count; i++) {
                _children[i].UpdateAll(deltaTime);
            }
        }

        public void RenderAll(string shader, RenderingEngine renderingEngine, bool baseShader) {
            Render(shader, renderingEngine, baseShader);
            for (var i = 0; i < _children.Count; i++) {
                _children[i].RenderAll(shader, renderingEngine, baseShader);
            }
        }

        public void Update(float delta) {
            for (var i = 0; i < _components.Count; i++) {
                _components[i].Update(delta);
            }
        }

        public void Render(string shader, RenderingEngine renderingEngine, bool baseShader) {
            for (var i = 0; i < _components.Count; i++) {
                _components[i].Render(shader, renderingEngine, baseShader);
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
    }
}