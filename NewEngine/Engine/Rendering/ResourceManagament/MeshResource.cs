using System;
using NewEngine.Engine.Core;
using OpenTK.Graphics.OpenGL;

namespace NewEngine.Engine.Rendering.ResourceManagament {
    public class MeshResource : IResourceManaged {
        private int _ibo;
        private int _refCount;
        private int _vbo;
        private int _matrixBuffer;

        public MeshResource() {
            Dispatcher.Current.BeginInvoke(() => {
                GL.GenBuffers(1, out _vbo);
                GL.GenBuffers(1, out _ibo);
                GL.GenBuffers(1, out _matrixBuffer);
            });
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

        public void Cleanup() {
            GL.DeleteBuffers(1, ref _vbo);
            GL.DeleteBuffers(1, ref _ibo);
            GL.DeleteBuffers(1, ref _matrixBuffer);
            GL.DeleteVertexArray(_vbo);
            GL.DeleteVertexArray(_ibo);
        }
    }
}