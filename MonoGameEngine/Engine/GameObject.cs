using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameEngine.Components;
using MonoGameEngine.Engine.Components;

namespace MonoGameEngine {
    public class GameObject {
        public List<Component> _components = new List<Component>();
        public GameObject parent;

        public string name;

        public GameObject() {
        }

        public GameObject(Vector3 position) {
            AddComponent<Transform>();
            Transform.Position = position;
        }

        public void Instantiate() {
            foreach (var component in _components) {
                component.GameObject = this;
                component.Init();
            }
            CoreEngine.instance.AddGameObject(this);

        }

        public Transform Transform
        {
            get { return (Transform)_components[0]; }
            set { _components[0] = value; }

        }

        public Component AddComponent<T>() where T : Component {
            Component component = Activator.CreateInstance<T>();
            _components.Add(component);
            _components[_components.Count - 1].GameObject = this;
            return _components[_components.Count - 1];
        }

        public Component AddComponent(Component component) {
            _components.Add(component);
            _components[_components.Count - 1].GameObject = this;
            return _components[_components.Count - 1];
        }


        public T GetComponent<T>() where T : Component {
            return _components.Where(component1 => component1.GetType() == typeof(T)).Cast<T>().FirstOrDefault();
        }

        public static T FindGameObjectOfType<T>() where T : Component {
            foreach (var gameObject in CoreEngine.instance.GameObjects) {
                if (gameObject.GetComponent<T>() != null)
                    return gameObject.GetComponent<T>();
            }

            return null;
        }

        /// DO NOT USE... ONLY FOR XML LOAD
        public void Initialize() {
            foreach (var component in _components) {
                component.GameObject = this;
                component.Init();
            }
        }

        public void Update(float deltaTime) {
            foreach (var component in _components) {
                component.Update(deltaTime);
            }
        }

        public void Draw(GraphicsDevice graphicsDevice) {
            foreach (var component in _components) {
                component.Draw(graphicsDevice);
                CoreEngine.instance.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            }
        }

        public override string ToString() {
            string components = "";
            foreach (var component in _components) {
                components = components + " : " + component.ToString();
            }
            return " Postion : " + Transform.Position + " Components " + components;

        }


    }
}
