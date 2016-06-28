using System.Diagnostics;
using System.Xml.Serialization;
using Microsoft.Xna.Framework.Graphics;
using MonoGameEngine.Engine.Physics;
using MonoGameEngine.Engine.UI;

namespace MonoGameEngine.Engine.Components {
    [XmlInclude(typeof(Camera))]
    [XmlInclude(typeof(Transform))]
    [XmlInclude(typeof(MeshRenderer))]
    [XmlInclude(typeof(BoxCollider))]
    [XmlInclude(typeof(SphereCollider))]
    [XmlInclude(typeof(UIComponent))]
    [XmlInclude(typeof(UiTextComponent))]
    [XmlInclude(typeof(UiTextureComponent))]
    [XmlInclude(typeof(UIComponent))]
    public class Component {
        private GameObject _gameObject;
        [XmlIgnore]
        public GameObject GameObject
        {
            get
            {
                // if i have not been assigned a parent then ill have to find it myself.
                if (_gameObject == null) {
                    foreach (GameObject t in CoreEngine.instance.GameObjects) {
                        foreach (Component t1 in t._components) {
                            Debug.WriteLine("found my parent");
                            if (t1 == this) {
                                _gameObject = t;
                                return _gameObject;
                            }
                        }
                    }
                }
                return _gameObject;
            }
            set { _gameObject = value; }
        }

        [XmlIgnore]
        public Transform Transform => GameObject.Transform;

        public bool IsInitialized;

        public virtual void Update(float deltaTime) { }
        public virtual void Draw(GraphicsDevice graphicsDevice) { }

        public virtual void Init() {
            IsInitialized = true;
        }
    }
}
