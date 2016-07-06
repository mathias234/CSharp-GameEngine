using System;
using NewEngine.Engine.Core;
using NewEngine.Engine.Rendering.Shading;
using OpenTK;

namespace NewEngine.Engine.components {
    public class PointLight : BaseLight {
        private const int ColorDepth = 256;

        private float _range;
        private Vector3 _attenuation;

        public PointLight(Vector3 color, float intensity, Vector3 attenuation) : base(color, intensity) {
            _attenuation = attenuation;
            Shader = ForwardPoint.Instance;

            var a = attenuation.Z;
            var b = attenuation.Y;
            var c = attenuation.X - ColorDepth * Intensity * Util.MaxOfVector3(color);

            _range = (float)(-b + Math.Sqrt(b * b - 4 * a * c))/(2*a);
        }

        public float Range {
            get { return _range; }
            set { _range = value; }
        }

        public float Constant {
            get { return _attenuation.X; }
            set { _attenuation.X = value; }
        }

        public float Linear {
            get { return _attenuation.Y; }
            set { _attenuation.Y = value; }
        }

        public float Exponent {
            get { return _attenuation.Z; }
            set { _attenuation.Z = value; }
        }
    }
}
