using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameEngine.Engine.UI {
    public class UiTextComponent : UIComponent {
        public string Text;
        public Color BackGroundColor;

        public UiTextComponent() { }

        /// <param name="rect">the position and scale of the uielement</param>
        /// <param name="texture2D">the texture you want to draw</param>
        /// <param name="color"></param>
        /// <param name="backGroundColor"></param>
        /// <param name="destoryNextUpdate"></param>
        /// <param name="onClicked">if null it will not register onClick Events</param>
        public UiTextComponent(Rectangle rect, string text, Color color, Color backGroundColor, bool destoryNextUpdate = false, Action onClicked = null) {
            Rect = rect;
            Text = text;
            Color = color;
            BackGroundColor = backGroundColor;
            OnClicked = onClicked;
            DestoryNextUpdate = destoryNextUpdate;
            Rect = new Rectangle(rect.X, rect.Y, 50, 20);
        }

        public override void Draw(GraphicsDevice graphicsDevice) {
            if (Text == null)
                Text = "Unnamed GameOject";
            CoreEngine.instance.GetSpriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
            CoreEngine.instance.GetSpriteBatch.Draw(UISystem.GetDefaultBackground, new Rectangle(Rect.X, Rect.Y, 50, 20), BackGroundColor);
            CoreEngine.instance.GetSpriteBatch.DrawString(UISystem.GetDefaultFont, Text, new Vector2(Rect.X, Rect.Y), Color);
            CoreEngine.instance.GetSpriteBatch.End();

            base.Draw(graphicsDevice);
        }
    }
}