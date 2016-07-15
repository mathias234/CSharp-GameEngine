using System.Runtime.InteropServices;
using OpenTK;

namespace NewEngine.Engine.Rendering {
    public struct Vertex {
        private Vector3 _position;
        private Vector2 _texCoord;
        private Vector3 _normal;
        private Vector3 _tangent;

        public static readonly int SizeInBytes = Marshal.SizeOf((object) new Vertex());

        public Vertex(Vector3 pos, Vector2 texCoord, Vector3 normal, Vector3 tangent) {
            _position = pos;
            _texCoord = texCoord;
            _normal = normal;
            _tangent = tangent;
        }

        public Vertex(Vector3 pos, Vector2 texCoord, Vector3 normal) {
            _position = pos;
            _texCoord = texCoord;
            _normal = normal;
            _tangent = new Vector3(1);
        }

        public Vertex(Vector3 pos, Vector2 texCoord) {
            _position = pos;
            _texCoord = texCoord;
            _normal = new Vector3(0);
            _tangent = new Vector3(1);
        }


        public Vertex(Vector3 pos) {
            _position = pos;
            _texCoord = new Vector2(0);
            _normal = new Vector3(0);
            _tangent = new Vector3(1);
        }

        public Vector3 Position {
            get { return _position; }
            set { _position = value; }
        }

        public Vector2 TexCoord {
            get { return _texCoord; }
            set { _texCoord = value; }
        }

        public Vector3 Normal {
            get { return _normal; }
            set { _normal = value; }
        }

        public Vector3 Tangent {
            get { return _tangent; }
            set { _tangent = value; }
        }
    }
}
