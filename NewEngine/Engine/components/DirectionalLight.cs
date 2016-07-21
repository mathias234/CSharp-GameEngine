using NewEngine.Engine.Core;
using NewEngine.Engine.Rendering.Shading;
using OpenTK;
using OpenTK.Graphics.ES10;

namespace NewEngine.Engine.components {
    public class DirectionalLight : BaseLight {

        public DirectionalLight(Vector3 color, float intensity) : base(color, intensity) {

            Shader = new Shader("forward-directional");
            ShadowInfo = new ShadowInfo(Matrix4.CreateOrthographicOffCenter(-100, 100, -100, 100, -100, 1000), 1.5f, false);
        }

        public Vector3 Direction
        {
            get { return Transform.Forward; }
        }
    }
}
