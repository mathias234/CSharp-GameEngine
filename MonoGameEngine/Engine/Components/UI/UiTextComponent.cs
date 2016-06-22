using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameEngine.Engine.UI {
    public class UiTextComponent : UIComponent {
        public string Text;
        public Color BackGroundColor;

        public UiTextComponent() { }

        public override void Init() {
            base.Init();
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