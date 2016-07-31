using System;
using NewEngine.Engine.Core;
using NewEngine.Engine.Rendering;
using NewEngine.Engine.Rendering.Shading;
using OpenTK;

namespace NewEngine.Engine.components {
    public class PointLight : BaseLight {
        private const int ColorDepth = 256;

        public PointLight(Vector3 color, float intensity, Attenuation attenuation) : base(color, intensity) {
            Attenuation = attenuation;
            Shader = new Shader("forward-PointLight");

            var a = attenuation.Exponent;
            var b = attenuation.Linear;
            var c = attenuation.Constant - ColorDepth * Intensity * Util.MaxOfVector3(color);

            Range = (float)(-b + Math.Sqrt(b * b - 4 * a * c)) / (2 * a);
        }

        public float Range { get; set; }

        public Attenuation Attenuation { get; set; }
    }
}