using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonoGameEngine.Engine.UI {
    // This will be rewritten to use the GameObject Component system
    public class UISystem {
        private SpriteBatch _spriteBatch;

        private SpriteFont _defaultFont;
        public static SpriteFont GetDefaultFont => CoreEngine.instance.GetUISystem._defaultFont;

        private Texture2D _defaultBackground;
        public static Texture2D GetDefaultBackground => CoreEngine.instance.GetUISystem._defaultBackground;

        private MouseState _lastMouseState;
        private MouseState _currentMouseState;

        public UISystem() {
            _spriteBatch = CoreEngine.instance.GetSpriteBatch;
            _lastMouseState = Mouse.GetState();
            _currentMouseState = Mouse.GetState();
        //    _defaultFont = CoreEngine.instance.Content.Load<SpriteFont>("DefaultFont");
          //  _defaultBackground = CoreEngine.instance.Content.Load<Texture2D>("Whitebg");
        }

        public void Update() {
            _lastMouseState = _currentMouseState;
            _currentMouseState = Mouse.GetState();

            if (_lastMouseState.LeftButton == ButtonState.Released && _currentMouseState.LeftButton == ButtonState.Pressed) {
                var uiElements = new List<UIComponent>();
                // reverse the list so we go from bottom to top
                foreach (var gameObject in CoreEngine.instance.GameObjects) {
                    foreach (var component in gameObject._components) {
                        if (component is UIComponent)
                            uiElements.Add((UIComponent)component);
                    }
                }


                foreach (var uiElement in uiElements.Reverse<UIComponent>()) {
                    if (uiElement.OnClicked == null)
                        continue;

                    if (uiElement.ClickZone.Contains(_currentMouseState.X, _currentMouseState.Y)) {
                        uiElement.OnClicked();
                        // dont trigger the ui element beneth
                        return;
                    }
                }
            }
        }
    }
}
