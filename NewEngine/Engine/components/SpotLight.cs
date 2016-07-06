using NewEngine.Engine.Rendering.Shading;
using OpenTK;

namespace NewEngine.Engine.components {
    public class SpotLight : PointLight {
        float _cutoff;

        public SpotLight(Vector3 color, float intensity, Vector3 attenuation, float cutoff)
            :base(color, intensity, attenuation)  {
            _cutoff = cutoff;
            Shader = ForwardSpot.Instance;
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
