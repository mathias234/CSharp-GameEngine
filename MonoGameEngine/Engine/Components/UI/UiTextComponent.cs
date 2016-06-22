using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameEngine.Engine.Components.UI;

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
                return;


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

            CoreEngine.instance.GetSpriteBatch.Draw(UISystem.GetDefaultBackground, Rect, BackGroundColor);
            CoreEngine.instance.GetSpriteBatch.DrawString(UISystem.GetDefaultFont, Text, new Vector2(Rect.X, Rect.Y), Color);
            CoreEngine.instance.GetSpriteBatch.End();


            RasterizerState defaultRasterizerState = new RasterizerState { ScissorTestEnable = false };

            CoreEngine.instance.GetSpriteBatch.GraphicsDevice.RasterizerState = defaultRasterizerState;

            CoreEngine.instance.GetSpriteBatch.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            base.Draw(graphicsDevice);
        }
    }
}