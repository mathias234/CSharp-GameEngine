using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;

namespace NewEngine.Engine.Rendering.ResourceManagament {
   public class TextureResource : IDisposable {
        private int _id;
        private int _refCount;

        public TextureResource() {
            GL.GenTextures(1, out _id);
        }

        public void AddReference() {
            _refCount++;
        }

        public bool RemoveReference() {
            _refCount--;
            return _refCount == 0;
        }

        public int Id {
            get { return _id; }
        }

        public void Dispose() {
            GL.DeleteBuffers(1, ref _id);
        }
    }
}
