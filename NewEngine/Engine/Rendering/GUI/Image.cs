using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NewEngine.Engine.components;

namespace NewEngine.Engine.Rendering.GUI {
    public class Image : GameComponent {
        public Image(Texture texture) {
            Texture = texture;
        }

        public Texture Texture { get; set; }

        public override void Render(string shader, string shaderType, float deltaTime, RenderingEngine renderingEngine, string renderStage) {

        }

        public override void AddToEngine(ICoreEngine engine) {
        }

        public override void OnDestroyed(ICoreEngine engine) {
        }
    }
}
