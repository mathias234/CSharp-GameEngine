using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NewEngine.Engine.components;
using NewEngine.Engine.Core;
using NewEngine.Engine.Rendering.ResourceManagament;

namespace NewEngine.Engine.Rendering.GUI {
    public class GUIRenderer : MappedValues {
        private readonly ICoreEngine _coreEngine;

        private List<GameComponent> gameComponents;

        public GUIRenderer(ICoreEngine coreEngine) {
            _coreEngine = coreEngine;
            gameComponents = new List<GameComponent>();
        }

        public void Render(float time) {
            var core = (CoreEngine)_coreEngine;
            core.Game.GetRootObject.AddToEngine(CoreEngine.GetCoreEngine);

            foreach (var gameComponent in gameComponents) {
                // render the UI object here    
            }

            gameComponents = new List<GameComponent>();
        }

        public void AddToEngine(GameComponent gc) {
            gameComponents.Add(gc);
        }
    }
}
