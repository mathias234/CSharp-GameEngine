using System.Runtime.InteropServices;
using OpenTK;

namespace NewEngine.Engine.Rendering {
    public struct Vertex {
        public static readonly int SizeInBytes = Marshal.SizeOf(new Vertex());

        public Vertex(Vector3 pos, Vector2 texCoord, Vector3 normal, Vector3 tangent) {
            Position = pos;
            TexCoord = texCoord;
            Normal = normal;
            Tangent = tangent;
        }

        public Vertex(Vector3 pos, Vector2 texCoord, Vector3 normal) {
            Position = pos;
            TexCoord = texCoord;
            Normal = normal;
            Tangent = new Vector3(1);
        }

        public Vertex(Vector3 pos, Vector2 texCoord) {
            Position = pos;
            TexCoord = texCoord;
            Normal = new Vector3(0);
            Tangent = new Vector3(1);
        }


        public Vertex(Vector3 pos) {
            Position = pos;
            TexCoord = new Vector2(0);
            Normal = new Vector3(0);
            Tangent = new Vector3(1);
        }

        public Vector3 Position { get; set; }

        public Vector2 TexCoord { get; set; }

        public Vector3 Normal { get; set; }

        public Vector3 Tangent { get; set; }
    }
}