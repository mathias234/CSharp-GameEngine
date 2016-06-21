using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameEngine.Engine.UI {
    public class UIElement {
        public Rectangle Rect;
        public Color Color;
        public Action OnClicked;
        public bool DestoryNextUpdate;

        public UIElement() {}

        /// <param name="rect">the position and scale of the uielement</param>
        /// <param name="color"></param>
        /// <param name="onClicked">if null it will not register onClick Events</param>
        public UIElement(Rectangle rect, Color color, Action onClicked = null) {
            Rect = rect;
            Color = color;
            OnClicked = onClicked;
        }

    }
}
