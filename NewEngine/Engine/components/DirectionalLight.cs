using NewEngine.Engine.Core;
using NewEngine.Engine.Rendering.Shading;
using OpenTK;

namespace NewEngine.Engine.components {
    public class DirectionalLight : BaseLight {

        public DirectionalLight(Vector3 color, float intensity) : base(color, intensity) {

            Shader = new Shader("forward-directional");
        }

        public Vector3 Direction
        {
            get { return Transform.Forward; }
        }
    }
}
