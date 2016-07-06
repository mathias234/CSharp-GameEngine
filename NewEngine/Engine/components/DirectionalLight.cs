using NewEngine.Engine.Core;
using NewEngine.Engine.Rendering.Shading;
using OpenTK;

namespace NewEngine.Engine.components {
    public class DirectionalLight : BaseLight {
        private Vector3 _direction;

        public DirectionalLight(Vector3 color, float intensity, Vector3 direction) : base(color, intensity) {
            _direction = direction.Normalized();

            Shader = ForwardDirectional.Instance;
        }

        public Vector3 Direction
        {
            get { return _direction; }
            set { _direction = value; }
        }
    }
}
