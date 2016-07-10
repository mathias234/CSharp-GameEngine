using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;

namespace NewEngine.Engine.Rendering.ResourceManagament {
    public class MeshResource : IDisposable {
        private int _vbo;
        private int _ibo;
        private int _size;
        private int _refCount;

        public MeshResource() {
            GL.GenBuffers(1, out _vbo);
            GL.GenBuffers(1, out _ibo);
            _size = 0;
            _refCount = 1;
        }

        public int Vbo {
            get { return _vbo; }
            set { _vbo = value; }
        }

        public int Ibo {
            get { return _ibo; }
            set { _ibo = value; }
        }

        public void AddReference() {
            _refCount++;
        }

        public bool RemoveReference() {
            _refCount--;
            return _refCount == 0;
        }

        public int Size {
            get { return _size; }
            set { _size = value; }
        }

        public void Dispose() {
            GL.DeleteBuffers(1, ref _vbo);
            GL.DeleteBuffers(1, ref _ibo);
        }
    }
}
