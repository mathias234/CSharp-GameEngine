using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SwiftEngine.Engine.UI {
    public class UISystem {
        public List<UIElement> UIElements = new List<UIElement>();
        private SpriteBatch _spriteBatch;
        private SpriteFont font;
        private Texture2D whiteSquare;


        private MouseState lastMouseState;
        private MouseState currentMouseState;

        public UISystem() {
            _spriteBatch = CoreEngine.instance.GetSpriteBatch;
            lastMouseState = Mouse.GetState();
            currentMouseState = Mouse.GetState();
            font = CoreEngine.instance.Content.Load<SpriteFont>("DefaultFont");
            whiteSquare = CoreEngine.instance.Content.Load<Texture2D>("Whitebg");
        }

        public void Update() {
            lastMouseState = currentMouseState;
            currentMouseState = Mouse.GetState();

            if (lastMouseState.LeftButton == ButtonState.Released && currentMouseState.LeftButton == ButtonState.Pressed) {
                // reverse the list so we go from bottom to top
                foreach (var uiElement in UIElements.Reverse<UIElement>()) {
                    if (uiElement.OnClicked == null)
                        continue;

                    if (uiElement.Rect.Contains(currentMouseState.X, currentMouseState.Y)) {
                        uiElement.OnClicked();
                        // dont trigger the ui element beneth
                        return;
                    }
                }
            }
        }
        public void Draw() {
            for (int i = 0; i < UIElements.Count; i++) {
                var uiElement = UIElements[i];

                if (uiElement is UITextureElement) {
                    var uiElementTexture = (UITextureElement)uiElement;
                    _spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
                    _spriteBatch.Draw(uiElementTexture.Texture2D ?? whiteSquare, uiElement.Rect, uiElement.Color);
                    _spriteBatch.End();
                }
                if (uiElement is UITextElement) {
                    var uiElementText = (UITextElement)uiElement;
                    _spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
                    _spriteBatch.Draw(whiteSquare, new Rectangle(uiElement.Rect.X, uiElement.Rect.Y, 50, 20), uiElementText.BackGroundColor);
                    _spriteBatch.DrawString(font, uiElementText.Text, new Vector2(uiElement.Rect.X, uiElement.Rect.Y), uiElement.Color);
                    _spriteBatch.End();
                }

                if (uiElement.DestoryNextUpdate)
                    UIElements.Remove(uiElement);
                CoreEngine.instance.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            }
        }

        public static void CreateUI(UITextureElement uiElement) {
            CoreEngine.instance.GetUISystem.UIElements.Add(uiElement);
        }
        public static void CreateUI(UITextElement uiElement) {
            CoreEngine.instance.GetUISystem.UIElements.Add(uiElement);
        }
    }
}
