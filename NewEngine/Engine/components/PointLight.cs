using System;
using NewEngine.Engine.Core;
using NewEngine.Engine.Rendering;
using NewEngine.Engine.Rendering.Shading;
using OpenTK;

namespace NewEngine.Engine.components {
    public class PointLight : BaseLight {
        private const int ColorDepth = 256;

        private float _range;
        private Attenuation _attenuation;

        public PointLight(Vector3 color, float intensity, Attenuation attenuation) : base(color, intensity) {
            _attenuation = attenuation;
            Shader = new Shader("forward-point");

            var a = attenuation.Constant;
            var b = attenuation.Linear;
            var c = attenuation.Exponent - ColorDepth * Intensity * Util.MaxOfVector3(color);

            _range = (float)(-b + Math.Sqrt(b * b - 4 * a * c))/(2*a);
        }

        public float Range {
            get { return _range; }
            set { _range = value; }
        }

        public Attenuation Attenuation {
            get { return _attenuation; }
            set { _attenuation = value; }
        }
    }
}
