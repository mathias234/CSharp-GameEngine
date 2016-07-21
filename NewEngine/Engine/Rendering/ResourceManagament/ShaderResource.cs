using System;
using System.Collections.Generic;
using NewEngine.Engine.Core;
using OpenTK.Graphics.OpenGL;

namespace NewEngine.Engine.Rendering.ResourceManagament {
    public class ShaderResource {
        private int _program;
        private Dictionary<string, int> _uniforms;
        private List<string> _uniformNames;
        private List<string> _uniformTypes;
        private int _refCount;


        public ShaderResource() {
            _uniforms = new Dictionary<string, int>();
            _uniformNames = new List<string>();
            _uniformTypes = new List<string>();
            _program = GL.CreateProgram();

            if (_program == 0) {
                LogManager.Error("Shader creation failed: could not find valid memory location in constructor");
            }

        }

        public void AddReference() {
            _refCount++;
        }

        public bool RemoveReference() {
            _refCount--;
            return _refCount == 0;
        }

        public int Program
        {
            get { return _program; }
        }

        public Dictionary<string, int> Uniforms
        {
            get { return _uniforms; }
            set { _uniforms = value; }
        }

        public List<string> UniformNames
        {
            get { return _uniformNames; }
            set { _uniformNames = value; }
        }

        public List<string> UniformTypes
        {
            get { return _uniformTypes; }
            set { _uniformTypes = value; }
        }
    }
}
