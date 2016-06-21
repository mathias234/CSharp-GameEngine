using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameEngine.Components;

namespace MonoGameEngine.Engine.UI {
    public class UIComponent : Component {
        public Rectangle Rect;
        public Color Color;
        [XmlIgnore]
        public Action OnClicked;
        public bool DestoryNextUpdate;

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
