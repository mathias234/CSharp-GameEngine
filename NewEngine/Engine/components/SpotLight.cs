using NewEngine.Engine.Rendering;
using NewEngine.Engine.Rendering.Shading;
using OpenTK;

namespace NewEngine.Engine.components {
    public class SpotLight : PointLight {
        float _cutoff;

        public SpotLight(Vector3 color, float intensity, Attenuation attenuation, float cutoff)
            :base(color, intensity, attenuation)  {
            _cutoff = cutoff;
            Shader = new Shader("forward-spot");
        }

        public float Cutoff {
            get { return _cutoff; }
            set { _cutoff = value; }
        }

        public Vector3 Direction {
            get { return Transform.Forward; }
        }
    }
}
