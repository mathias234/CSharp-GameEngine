using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SwiftEngine.Engine.UI {
    public class UITextElement : UIElement {
        public string Text;
        public Color BackGroundColor;

        /// <param name="rect">the position and scale of the uielement</param>
        /// <param name="texture2D">the texture you want to draw</param>
        /// <param name="color"></param>
        /// <param name="backGroundColor"></param>
        /// <param name="destoryNextUpdate"></param>
        /// <param name="onClicked">if null it will not register onClick Events</param>
        public UITextElement(Rectangle rect, string text, Color color, Color backGroundColor, bool destoryNextUpdate = false, Action onClicked = null) {
            Rect = rect;
            Text = text;
            Color = color;
            BackGroundColor = backGroundColor;
            OnClicked = onClicked;
            DestoryNextUpdate = destoryNextUpdate;
            Rect = new Rectangle(rect.X, rect.Y, 50, 20);
        }
    }
}