using System.Collections.Generic;
using NewEngine.Engine.components;
using NewEngine.Engine.Rendering;
using NewEngine.Engine.Rendering.Shading;

namespace NewEngine.Engine.Core {
    public class GameObject {
        private List<GameObject> _children;
        private List<GameComponent> _components;
        private Transform _transform;

        public GameObject() {
            _children = new List<GameObject>();
            _components = new List<GameComponent>();
            _transform = new Transform();
        }

        public void AddChild(GameObject child) {
            _children.Add(child);
        }

        public void AddComponent(GameComponent component) {
            _components.Add(component);
            component.Parent = this;
        }

        public void Update() {
            //TODO: find a more elegant solution
            for (int i = 0; i < _components.Count; i++) {
                if (_components[i].IsEnabled == false) {
                    _components[i].OnEnable();
                    _components[i].IsEnabled = true;
                }
            }


            for (int i = 0; i < _components.Count; i++) {
                _components[i].Update();
            }

            for (int i = 0; i < _children.Count; i++) {
                _children[i].Update();
            }
        }

        public void Render(Shader shader) {
            for (int i = 0; i < _components.Count; i++) {
                _components[i].Render(shader);
            }

            for (int i = 0; i < _children.Count; i++) {
                _children[i].Render(shader);
            }
        }

        public void AddToRenderingEngine(RenderingEngine renderingEngine) {
            for (int i = 0; i < _components.Count; i++) {
                _components[i].AddToRenderingEngine(renderingEngine);
            }

            for (int i = 0; i < _children.Count; i++) {
                _children[i].AddToRenderingEngine(renderingEngine);
            }
        }

        public Transform Transform
        {
            get { return _transform; }
            set { _transform = value; }
        }
    }
}
