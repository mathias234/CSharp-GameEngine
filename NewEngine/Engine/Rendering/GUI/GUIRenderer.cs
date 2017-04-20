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

            MainEngine = coreEngine;
            gameComponents = new List<GameComponent>();
        }

        public void Render(float time) {
            foreach (var gameComponent in gameComponents) {
                GL.Viewport(0, 0, (int)CoreEngine.GetWidth(), (int)CoreEngine.GetHeight());
                GL.Disable(EnableCap.Lighting);
                GL.Disable(EnableCap.CullFace);
                GL.Enable(EnableCap.Blend);
                GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
                // render the UI object here   
                gameComponent.Render("UIShader", "UIShader", time, this, "ui");
                GL.Enable(EnableCap.Lighting);
                GL.Enable(EnableCap.CullFace);
                GL.Disable(EnableCap.Blend);
                GL.BlendFunc(BlendingFactorSrc.One, BlendingFactorDest.One);
            }

            gameComponents = new List<GameComponent>();
        }

        public void AddToEngine(GameComponent gc) {
            gameComponents.Add(gc);
        }
    }
}
