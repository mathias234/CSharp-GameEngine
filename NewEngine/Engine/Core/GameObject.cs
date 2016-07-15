using System.Collections.Generic;
using System.Linq;
using NewEngine.Engine.components;
using NewEngine.Engine.Physics.PhysicsComponents;
using NewEngine.Engine.Rendering;
using NewEngine.Engine.Rendering.Shading;

namespace NewEngine.Engine.Core {
    public class GameObject {
        private List<GameObject> _children;
        private List<GameComponent> _components;
        private Transform _transform;
        private CoreEngine _engine;

        public GameObject() {
            _children = new List<GameObject>();
            _components = new List<GameComponent>();
            _transform = new Transform();
            Engine = null;
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
            for (int i = 0; i < _components.Count; i++) {
                if (_components[i].IsEnabled == false) {
                    _components[i].OnEnable();
                    _components[i].IsEnabled = true;
                }
            }

            for (int i = 0; i < _children.Count; i++) {
                _children[i].UpdateAll(deltaTime);
            }
        }

        public void RenderAll(Shader shader, RenderingEngine renderingEngine) {
            Render(shader, renderingEngine);
            for (int i = 0; i < _children.Count; i++) {
                _children[i].RenderAll(shader, renderingEngine);
            }
        }

        public void Update(float delta) {
            for (int i = 0; i < _components.Count; i++) {
                _components[i].Update(delta);
            }
        }

        public void Render(Shader shader, RenderingEngine renderingEngine) {
            for (int i = 0; i < _components.Count; i++) {
                _components[i].Render(shader, renderingEngine);
            }
        }


        public List<GameObject> GetAllAttached() {
            List<GameObject> result = new List<GameObject> {this};
            for (int i = 0; i < _children.Count; i++) {
                GameObject child = _children[i];
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

        public Transform Transform
        {
            get { return _transform; }
            set { _transform = value; }
        }

        public CoreEngine Engine
        {
            get { return _engine; }
            set
            {
                if (this._engine != value) {

                    _engine = value;
                    for (int i = 0; i < _components.Count; i++) {
                        _components[i].AddToEngine(_engine);
                    }

                    for (int i = 0; i < _children.Count; i++) {
                        _children[i].Engine = _engine;
                    }
                }
            }
        }
    }
}
