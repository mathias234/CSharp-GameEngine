using System;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameEngine.Engine.UI {
    public class UiTextureComponent : UIComponent {
        [XmlIgnore]
        public Texture2D Texture2D;

        public UiTextureComponent() {}

        /// <param name="rect">the position and scale of the uielement</param>
        /// <param name="texture2D">the texture you want to draw</param>
        /// <param name="color"></param>
        /// <param name="onClicked">if null it will not register onClick Events</param>
        public UiTextureComponent(Rectangle rect, Texture2D texture2D, Color color, bool destoryNextUpdate = false, Action onClicked = null) {
            Rect = rect;
            Texture2D = texture2D;
            Color = color;
            OnClicked = onClicked;
            DestoryNextUpdate = destoryNextUpdate;
        }

        public override void Draw(GraphicsDevice graphicsDevice) {
            CoreEngine.instance.GetSpriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
            CoreEngine.instance.GetSpriteBatch.Draw(Texture2D ?? UISystem.GetDefaultBackground, Rect, Color);
            CoreEngine.instance.GetSpriteBatch.End();
            base.Draw(graphicsDevice);
        }
    }
}