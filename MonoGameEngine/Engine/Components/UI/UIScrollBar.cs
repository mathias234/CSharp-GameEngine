using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameEngine.Engine.UI;

namespace MonoGameEngine.Engine.Components.UI {
    public class UIScrollBar :UIComponent{
        // a number between 0 - 1
        public float value;

        private Vector2 _currMousePos;

        private bool _hasClicked;

        private void OnScrollBarClicked() {
            Debug.WriteLine("hey");
            _hasClicked = true;

        }

        public override void Update(float deltaTime) {
            OnClicked = OnScrollBarClicked;

            if (Mouse.GetState().LeftButton == ButtonState.Released)
                _hasClicked = false;

            if(_hasClicked == false)
                return;

            _currMousePos = Mouse.GetState().Position.ToVector2();
            var newValue = (_currMousePos.Y/
                           Rect.Height) - (float)Rect.Y / Rect.Height;
            value = newValue;
            value = MathHelper.Clamp(value, 0f, 1f);
        }

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

#region draw

            CoreEngine.instance.GetSpriteBatch.Draw(UISystem.GetDefaultBackground, new Rectangle(Rect.X, Rect.Y -18, Rect.Width, Rect.Height + 35), Color);

            CoreEngine.instance.GetSpriteBatch.Draw(UISystem.GetDefaultBackground,
                new Rectangle(Rect.X + 3, Rect.Y + (int)( value * Rect.Height) - 30/2, Rect.Width - 6, 30), Color.LightGray);
            ClickZone = new Rectangle(Rect.X + 3, Rect.Y + (int) (value*Rect.Height) - 30/2, Rect.Width - 6, 30);

#endregion

            CoreEngine.instance.GetSpriteBatch.End();

            CoreEngine.instance.GetSpriteBatch.GraphicsDevice.RasterizerState = new RasterizerState {
                ScissorTestEnable = false
            };
            CoreEngine.instance.GetSpriteBatch.GraphicsDevice.DepthStencilState = DepthStencilState.Default;


        }
    }
}