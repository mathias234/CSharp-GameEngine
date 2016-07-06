using System.Drawing;

namespace NewEngine.Engine.Rendering {
    public class Material {
        private Texture _texture;
        private Color _color;
        private float _specularIntensity;
        private float _specularPower;

        public Material(Texture texture, Color color, float specularIntensity = 2, float specularPower = 32) {
            MainTexture = texture;
            Color = color;
            _specularIntensity = specularIntensity;
            _specularPower = specularPower;
        }

        public Texture MainTexture {
            get { return _texture; }
            set { _texture = value; }
        }

        public Color Color {
            get { return _color; }
            set { _color = value; }
        }

        public float SpecularIntensity {
            get { return _specularIntensity; }
            set { _specularIntensity = value; }
        }

        public float SpecularPower {
            get { return _specularPower; }
            set { _specularPower = value; }
        }
    }
}
