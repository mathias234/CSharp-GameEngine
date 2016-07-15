using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NewEngine.Engine.Core;
using OpenTK.Graphics.OpenGL;
using PixelFormat = OpenTK.Graphics.OpenGL.PixelFormat;

namespace NewEngine.Engine.Rendering.ResourceManagament {
    public class TextureResource {
        private int[] _id;
        private int _refCount;
        private int _numTextures;
        private int _frameBuffer;
        private int _renderBuffer;
        private int _width;
        private int _height;

        public TextureResource(int numTexture, int width, int height, List<char[]> data, TextureFilter[] filters, FramebufferAttachment[] attachments) {
            _id = new int[numTexture];
            _numTextures = numTexture;
            _frameBuffer = 0;
            _renderBuffer = 0;
            _width = width;
            _height = height;

            InitTextures(data, filters);
            InitRenderTargets(attachments);
        }

        public TextureResource(int numTexture, int width, int height, IntPtr[] data, TextureFilter[] filters, FramebufferAttachment[] attachments) {
            _id = new int[numTexture];
            _numTextures = numTexture;
            _frameBuffer = 0;
            _renderBuffer = 0;
            _width = width;
            _height = height;

            InitTextures(data, filters);
            InitRenderTargets(attachments);
        }

        //Destructor
        ~TextureResource() {
            if (_refCount == 0) {
                GL.DeleteBuffers(_numTextures, _id);
                GL.DeleteFramebuffers(1, ref _frameBuffer);
                GL.DeleteRenderbuffer(_renderBuffer);
            }
        }

        private void InitRenderTargets(FramebufferAttachment[] attachments) {
            if (attachments.Length == 0)
                return;

            DrawBuffersEnum[] drawBuffers = new DrawBuffersEnum[_numTextures];
            bool hasDepth = false;

            for (int i = 0; i < _numTextures; i++) {
                if (attachments[i] == FramebufferAttachment.DepthAttachment) {
                    drawBuffers[i] = (DrawBuffersEnum)All.None;
                    hasDepth = true;
                }
                else
                    drawBuffers[i] = (DrawBuffersEnum)attachments[i];

                if (attachments[i] == (FramebufferAttachment)All.None) {
                    continue;
                }

                if (_frameBuffer == 0) {
                    GL.GenFramebuffers(1, out _frameBuffer);
                    GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, _frameBuffer);
                }

                GL.FramebufferTexture2D(FramebufferTarget.DrawFramebuffer, attachments[i], TextureTarget.Texture2D, _id[i], 0);
            }

            if (_frameBuffer == 0)
                return;

            if (!hasDepth) {
                GL.GenRenderbuffers(1, out _renderBuffer);
                GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, _renderBuffer);
                GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, (RenderbufferStorage)All.DepthComponent, _width, _height);
                GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, RenderbufferTarget.Renderbuffer, _renderBuffer);
            }

            GL.DrawBuffers(_numTextures, drawBuffers);

            if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferErrorCode.FramebufferComplete) {
                LogManager.Error("Framebuffer creation failed");
            }
        }

        public void AddReference() {
            _refCount++;
        }

        public bool RemoveReference() {
            _refCount--;
            return _refCount == 0;
        }

        private void InitTextures(List<char[]> data, TextureFilter[] filters) {
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

                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb8, _width, _height, 0, PixelFormat.Bgra, PixelType.UnsignedByte, data[i]);
            }
        }


        private void InitTextures(IntPtr[] data, TextureFilter[] filters) {
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

                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb8, _width, _height, 0, PixelFormat.Bgra, PixelType.UnsignedByte, data[i]);
            }
        }


        public void Bind(int textureNum) {
            GL.BindTexture(TextureTarget.Texture2D, _id[textureNum]);
        }

        public void BindAsRenderTarget() {
            GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, _frameBuffer);
            GL.Viewport(0, 0, _width, _height);
        }
    }
}
