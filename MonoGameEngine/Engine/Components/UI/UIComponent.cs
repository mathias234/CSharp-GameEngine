using System;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using MonoGameEngine.Engine.Components;

namespace MonoGameEngine.Engine.UI {
    public class UIComponent : Component {
        public Rectangle Rect;
        public Rectangle ClickZone;
        public Color Color;
        [XmlIgnore]
        public Action OnClicked;
        public bool DestoryNextUpdate;

        public override void Init() {
            base.Init();
        }

        public UIComponent() {}

        /// <param name="rect">the position and scale of the uielement</param>
        /// <param name="color"></param>
        /// <param name="onClicked">if null it will not register onClick Events</param>
        public UIComponent(Rectangle rect, Color color, Action onClicked = null) {
            Rect = rect;
            Color = color;
            OnClicked = onClicked;
        }
    }
}
