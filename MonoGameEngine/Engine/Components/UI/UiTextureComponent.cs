using System;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameEngine.Engine.Components.UI;

namespace MonoGameEngine.Engine.UI {
    public class UiTextureComponent : UIComponent {
        [XmlIgnore]
        public Texture2D Texture2D;

        public override void Draw(GraphicsDevice graphicsDevice) {

            if (GameObject.GetComponent<UIMask>() != null) {
                CoreEngine.instance.GetSpriteBatch.GraphicsDevice.ScissorRectangle =
                    GameObject.GetComponent<UIMask>().Rect;
                var rasterizerState = new RasterizerState { ScissorTestEnable = true };
                CoreEngine.instance.GetSpriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, null, null,
                    rasterizerState);
            }
            else {
                CoreEngine.instance.GetSpriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
            }

            CoreEngine.instance.GetSpriteBatch.Draw(Texture2D ?? UISystem.GetDefaultBackground, Rect, Color);
            CoreEngine.instance.GetSpriteBatch.End();

            CoreEngine.instance.GetSpriteBatch.GraphicsDevice.RasterizerState = new RasterizerState {
                ScissorTestEnable = false
            };
            CoreEngine.instance.GetSpriteBatch.GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            base.Draw(graphicsDevice);
        }
    }
}