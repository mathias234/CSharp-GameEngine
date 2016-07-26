using NewEngine.Engine.Rendering.Shading;
using OpenTK;

namespace NewEngine.Engine.components {
    public class DirectionalLight : BaseLight {

        public DirectionalLight(Vector3 color, float intensity) : base(color, intensity) {

            Shader = new Shader("forward-DirectionalLight");
            ShadowInfo = new ShadowInfo(Matrix4.CreateOrthographicOffCenter(-100, 100, -100, 100, -100, 1000), 1.5f, true);
        }

        public Vector3 Direction => Transform.Forward;
    }
}
