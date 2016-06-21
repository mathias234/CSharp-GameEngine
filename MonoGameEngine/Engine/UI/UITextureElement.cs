using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SwiftEngine.Engine.UI {
    public class UITextureElement : UIElement {
        public Texture2D Texture2D;


        /// <param name="rect">the position and scale of the uielement</param>
        /// <param name="texture2D">the texture you want to draw</param>
        /// <param name="color"></param>
        /// <param name="onClicked">if null it will not register onClick Events</param>
        public UITextureElement(Rectangle rect, Texture2D texture2D, Color color, bool destoryNextUpdate = false, Action onClicked = null) {
            Rect = rect;
            Texture2D = texture2D;
            Color = color;
            OnClicked = onClicked;
            DestoryNextUpdate = destoryNextUpdate;
        }
    }
}