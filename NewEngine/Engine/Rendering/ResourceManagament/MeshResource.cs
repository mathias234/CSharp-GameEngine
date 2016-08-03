using System;
using OpenTK.Graphics.OpenGL;

namespace NewEngine.Engine.Rendering.ResourceManagament {
    public class MeshResource : IDisposable {
        private int _ibo;
        private int _refCount;
        private int _vbo;
        private int _matrixBuffer;

        public MeshResource() {
            GL.GenBuffers(1, out _vbo);
            GL.GenBuffers(1, out _ibo);
            GL.GenBuffers(1, out _matrixBuffer);
            Size = 0;
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

        public int Size { get; set; }

        public int MatrixBuffer {
            get { return _matrixBuffer; }
            set { _matrixBuffer = value; }
        }


        public void AddReference() {
            _refCount++;
        }

        public bool RemoveReference() {
            _refCount--;
            return _refCount == 0;
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) {
            if (disposing) {
                GL.DeleteBuffers(1, ref _vbo);
                GL.DeleteBuffers(1, ref _ibo);
                GL.DeleteVertexArray(_vbo);
                GL.DeleteVertexArray(_ibo);
            }
        }
    }
}