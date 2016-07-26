using NewEngine.Engine.Rendering;
using NewEngine.Engine.Rendering.Shading;
using OpenTK;

namespace NewEngine.Engine.components {
    public class SpotLight : PointLight {
        public SpotLight(Vector3 color, float intensity, Attenuation attenuation, float cutoff)
            : base(color, intensity, attenuation) {
            Cutoff = cutoff;
            Shader = new Shader("forward-SpotLight");
        }

        public float Cutoff { get; set; }

        public Vector3 Direction => Transform.Forward;
    }
}