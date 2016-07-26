using System.Collections.Generic;
using NewEngine.Engine.Core;
using OpenTK.Graphics.OpenGL;

namespace NewEngine.Engine.Rendering.ResourceManagament {
    public class ShaderResource {
        private int _refCount;


        public ShaderResource() {
            Uniforms = new Dictionary<string, int>();
            UniformNames = new List<string>();
            UniformTypes = new List<string>();
            Program = GL.CreateProgram();

            if (Program == 0) {
                LogManager.Error("Shader creation failed: could not find valid memory location in constructor");
            }
        }

        public int Program { get; }

        public Dictionary<string, int> Uniforms { get; set; }

        public List<string> UniformNames { get; set; }

        public List<string> UniformTypes { get; set; }

        public void AddReference() {
            _refCount++;
        }

        public bool RemoveReference() {
            _refCount--;
            return _refCount == 0;
        }
    }
}