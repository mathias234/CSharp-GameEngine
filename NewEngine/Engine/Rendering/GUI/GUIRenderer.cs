using System.Collections.Generic;
using NewEngine.Engine.components;
using NewEngine.Engine.Core;
using NewEngine.Engine.Rendering.ResourceManagament;

namespace NewEngine.Engine.Rendering.GUI {
    public class GUIRenderer : BaseRenderingEngine {

        private List<GameComponent> gameComponents;

        public GUIRenderer(ICoreEngine coreEngine) {
            MainEngine = coreEngine;
            gameComponents = new List<GameComponent>();
        }

        public void Render(float time) {
            //foreach (var gameComponent in gameComponents) {
            //    // render the UI object here   
            //    //gameComponent.Render("UIShader", "", time, null, "ui");
            //}

            //gameComponents = new List<GameComponent>();
        }

        public void AddToEngine(GameComponent gc) {
            gameComponents.Add(gc);
        }
    }
}
