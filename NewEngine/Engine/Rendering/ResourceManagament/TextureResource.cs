using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
using PixelFormat = OpenTK.Graphics.OpenGL.PixelFormat;

namespace NewEngine.Engine.Rendering.ResourceManagament {
    public class TextureResource {
        private int[] _id;
        private int _refCount;
        private int _numTextures;
        private int _frameBuffer;

        public TextureResource(int numTexture, BitmapData[] data, TextureFilter[] filters) {
            _id = new int[numTexture];
            _numTextures = numTexture;
            _frameBuffer = 0;

            InitTextures(data, filters);
        }
        //Destructor
        ~TextureResource() {
            GL.DeleteBuffers(_numTextures, _id);
        }

        public void AddReference() {
            _refCount++;
        }

        public bool RemoveReference() {
            _refCount--;
            return _refCount == 0;
        }


        private void InitTextures(BitmapData[] data, TextureFilter[] filters) {
            GL.GenTextures(_numTextures, _id);

            for (int i = 0; i < _numTextures; i++) {
                GL.BindTexture(TextureTarget.Texture2D, _id[i]);

                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

                switch (filters[i]) {
                    case TextureFilter.Linear:
                        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (float)TextureMinFilter.Linear);
                        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (float)TextureMagFilter.Linear);
                        break;
                    case TextureFilter.Point:
                        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (float)TextureMinFilter.Nearest);
                        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (float)TextureMagFilter.Nearest);
                        break;
                }

                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb8, data[i].Width, data[i].Height, 0, PixelFormat.Bgra, PixelType.UnsignedByte, data[i].Scan0);
            }
        }

        public void Bind(int textureNum) {
            GL.BindTexture(TextureTarget.Texture2D, _id[textureNum]);
        }
    }
}
