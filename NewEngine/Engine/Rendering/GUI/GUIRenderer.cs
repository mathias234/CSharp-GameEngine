using System.Collections.Generic;
using NewEngine.Engine.components;
using NewEngine.Engine.Core;
using NewEngine.Engine.Rendering.ResourceManagament;
using OpenTK.Graphics.OpenGL;

namespace NewEngine.Engine.Rendering.GUI {
    public class GUIRenderer : BaseRenderingEngine {

        private List<GameComponent> gameComponents;

        public GUIRenderer(ICoreEngine coreEngine) {
            SamplerMap = new Dictionary<string, int> {
                {"diffuse", 0}
            };

            gameComponents = new List<GameComponent>();
        }

        public override void Render(float time) {
            foreach (var gameComponent in gameComponents) {
                GL.Enable(EnableCap.Blend);
                GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
                GL.Disable(EnableCap.DepthTest);
                // render the UI object here   
                gameComponent.Render("UIShader", "UIShader", time, this, "ui");
                GL.Disable(EnableCap.Blend);
                GL.Enable(EnableCap.DepthTest);
            }

            gameComponents = new List<GameComponent>();
        }

        public override void AddToEngine(GameComponent gc) {
            gameComponents.Add(gc);
        }
    }
}
