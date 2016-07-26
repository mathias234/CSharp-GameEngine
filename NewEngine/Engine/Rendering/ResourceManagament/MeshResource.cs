﻿using OpenTK.Graphics.OpenGL;

namespace NewEngine.Engine.Rendering.ResourceManagament {
    public class MeshResource {
        private int _ibo;
        private int _refCount;
        private int _vbo;

        public MeshResource() {
            GL.GenBuffers(1, out _vbo);
            GL.GenBuffers(1, out _ibo);
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

        public void AddReference() {
            _refCount++;
        }

        public bool RemoveReference() {
            _refCount--;
            return _refCount == 0;
        }
    }
}