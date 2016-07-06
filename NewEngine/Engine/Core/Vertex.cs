using OpenTK;

namespace NewEngine.Engine.Core {
    public struct Vertex {
        public const int Size = 8;

        private Vector3 _position;
        private Vector2 _uv;
        private Vector3 _normal;

        public Vertex(Vector3 pos, Vector2 uv) {
            _position = pos;
            _uv = uv;
            _normal = new Vector3(0);
        }


        public Vertex(Vector3 pos, Vector2 uv, Vector3 normal) {
            _position = pos;
            _uv = uv;
            _normal = normal;
        }


        public Vertex(Vector3 pos) {
            _position = pos;
            _uv = new Vector2(0);
            _normal = new Vector3(0);
        }

        public Vector3 Position {
            get { return _position; }
            set { _position = value; }
        }

        public Vector2 Uv {
            get { return _uv; }
            set { _uv = value; }
        }

        public Vector3 Normal {
            get { return _normal; }
            set { _normal = value; }
        }
    }
}
