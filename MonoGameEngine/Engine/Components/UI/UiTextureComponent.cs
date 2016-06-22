using System;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameEngine.Engine.UI {
    public class UiTextureComponent : UIComponent {
        [XmlIgnore]
        public Texture2D Texture2D;

        public override void Draw(GraphicsDevice graphicsDevice) {
            CoreEngine.instance.GetSpriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
            CoreEngine.instance.GetSpriteBatch.Draw(Texture2D ?? UISystem.GetDefaultBackground, Rect, Color);
            CoreEngine.instance.GetSpriteBatch.End();
            base.Draw(graphicsDevice);
        }
    }
}