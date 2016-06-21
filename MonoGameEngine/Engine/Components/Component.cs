using System;
using System.Diagnostics;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using MonoGameEngine.Engine.Components;

namespace MonoGameEngine.Components {
    [XmlInclude(typeof(Camera))]
    [XmlInclude(typeof(Transform))]
    [XmlInclude(typeof(MeshRenderer))]
    [XmlInclude(typeof(BoxCollider))]
    [XmlInclude(typeof(SphereCollider))]
    public class Component  {
        [XmlIgnore]
        public GameObject GameObject;

        [XmlIgnore]
        public Transform Transform => GameObject.Transform;

        public virtual void Update(float deltaTime) { }

        public virtual void Init() { }
    }
}
