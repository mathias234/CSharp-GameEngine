using System;
using System.Collections.Generic;
using NewEngine.Engine.Core;
using OpenTK.Graphics.OpenGL;

namespace NewEngine.Engine.Rendering.ResourceManagament {
    public class TextureResource : IDisposable {
        private int _frameBuffer;
        private int _height;
        private int[] _id;
        private int _numTextures;
        private int _renderBuffer;
        private int _width;
        private int _refCount;

        public TextureResource(int numTexture, int width, int height, List<char[]> data, TextureMinFilter[] filters,
            PixelInternalFormat[] internalFormat, PixelFormat[] format, bool clamp, FramebufferAttachment[] attachments,
            TextureTarget[] targets) {
            _id = new int[numTexture];
            _numTextures = numTexture;
            _frameBuffer = 0;
            _renderBuffer = 0;
            _width = width;
            _height = height;

            InitTextures(data, filters, internalFormat, format, clamp, targets);
            InitRenderTargets(attachments, targets);
            _refCount = 1;
        }

        public TextureResource(int numTexture, int width, int height, IntPtr[] data, TextureMinFilter[] filters,
            PixelInternalFormat[] internalFormat, PixelFormat[] format, bool clamp, FramebufferAttachment[] attachments,
            TextureTarget[] targets) {
            _id = new int[numTexture];
            _numTextures = numTexture;
            _frameBuffer = 0;
            _renderBuffer = 0;
            _width = width;
            _height = height;

            InitTextures(data, filters, internalFormat, format, clamp, targets);
            InitRenderTargets(attachments, targets);
            _refCount = 1;
        }

        private void InitRenderTargets(FramebufferAttachment[] attachments, TextureTarget[] targets) {
            if (attachments.Length == 0)
                return;

            var drawBuffers = new DrawBuffersEnum[32];

            var hasDepth = false;
            for (var i = 0; i < _numTextures; i++) {
                if (attachments[i] == FramebufferAttachment.DepthAttachment) {
                    drawBuffers[i] = (DrawBuffersEnum)All.None;
                    hasDepth = true;
                }
                else
                    drawBuffers[i] = (DrawBuffersEnum)attachments[i];

                if ((All)attachments[i] == All.None) {
                    continue;
                }

                if (_frameBuffer == 0) {
                    GL.GenFramebuffers(1, out _frameBuffer);
                    GL.BindFramebuffer(FramebufferTarget.Framebuffer, _frameBuffer);
                }

                GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, attachments[i], targets[i], _id[i], 0);
            }

            if (_frameBuffer == 0)
                return;

            if (!hasDepth) {
                GL.GenRenderbuffers(1, out _renderBuffer);
                GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, _renderBuffer);
                GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, (RenderbufferStorage)All.DepthComponent, _width,
                    _height);
                GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment,
                    RenderbufferTarget.Renderbuffer, _renderBuffer);
            }

            GL.DrawBuffers(_numTextures, drawBuffers);

            if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferErrorCode.FramebufferComplete) {
                LogManager.Error("Framebuffer creation failed");
            }
        }

        private void InitTextures(List<char[]> data, TextureMinFilter[] filters, PixelInternalFormat[] internalFormat,
            PixelFormat[] format, bool clamp, TextureTarget[] targets) {
            GL.GenTextures(_numTextures, _id);

            for (var i = 0; i < _numTextures; i++) {
                GL.BindTexture(targets[i], _id[i]);


                GL.TexParameter(targets[i], TextureParameterName.TextureMinFilter,
                    (float)filters[i]);
                GL.TexParameter(targets[i], TextureParameterName.TextureMagFilter,
                    (float)filters[i]);

                if (clamp) {
                    GL.TexParameter(targets[i], TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
                    GL.TexParameter(targets[i], TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
                }

                GL.TexImage2D(targets[i], 0, internalFormat[i], _width, _height, 0, format[i], PixelType.UnsignedByte, data[i]);

                if (filters[i] == TextureMinFilter.NearestMipmapNearest ||
                    filters[i] == TextureMinFilter.NearestMipmapLinear ||
                    filters[i] == TextureMinFilter.LinearMipmapNearest ||
                    filters[i] == TextureMinFilter.LinearMipmapLinear) {

                    GL.GenerateMipmap((GenerateMipmapTarget)targets[i]);

                    float maxAnisotropy;

                    GL.GetFloat((GetPName)ExtTextureFilterAnisotropic.MaxTextureMaxAnisotropyExt, out maxAnisotropy);
                    GL.TexParameter(targets[i], (TextureParameterName)ExtTextureFilterAnisotropic.TextureMaxAnisotropyExt, maxAnisotropy);
                }
                else {
                    GL.TexParameter(targets[i], TextureParameterName.TextureBaseLevel, 0);
                    GL.TexParameter(targets[i], TextureParameterName.TextureMaxLevel, 0);
                }
            }
        }


        private void InitTextures(IntPtr[] data, TextureMinFilter[] filters, PixelInternalFormat[] internalFormat,
            PixelFormat[] format, bool clamp, TextureTarget[] targets) {
            GL.GenTextures(_numTextures, _id);

            for (var i = 0; i < _numTextures; i++) {
                GL.BindTexture(targets[i], _id[i]);

                GL.TexParameter(targets[i], TextureParameterName.TextureMinFilter,
                    (float)filters[i]);
                GL.TexParameter(targets[i], TextureParameterName.TextureMagFilter,
                    (float)filters[i]);

                if (clamp) {
                    GL.TexParameter(targets[i], TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
                    GL.TexParameter(targets[i], TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
                }

                GL.TexImage2D(targets[i], 0, internalFormat[i], _width, _height, 0, format[i], PixelType.UnsignedByte, data[i]);

                if (filters[i] == TextureMinFilter.NearestMipmapNearest ||
                    filters[i] == TextureMinFilter.NearestMipmapLinear ||
                    filters[i] == TextureMinFilter.LinearMipmapNearest ||
                    filters[i] == TextureMinFilter.LinearMipmapLinear) {

                    GL.GenerateMipmap((GenerateMipmapTarget)targets[i]);

                    float maxAnisotropy;

                    GL.GetFloat((GetPName)ExtTextureFilterAnisotropic.MaxTextureMaxAnisotropyExt, out maxAnisotropy);
                    GL.TexParameter(targets[i], (TextureParameterName)ExtTextureFilterAnisotropic.TextureMaxAnisotropyExt, maxAnisotropy);
                }
                else {
                    GL.TexParameter(targets[i], TextureParameterName.TextureBaseLevel, 0);
                    GL.TexParameter(targets[i], TextureParameterName.TextureMaxLevel, 0);
                }


            }
        }


        public void Bind(int textureNum, TextureTarget targets) {
            GL.BindTexture(targets, _id[textureNum]);
        }

        public void BindAsRenderTarget() {
            GL.BindTexture(TextureTarget.ProxyTexture2D, 0);
            GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, _frameBuffer);
            GL.Viewport(0, 0, _width, _height);
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
                GL.DeleteTextures(_numTextures, _id);
                GL.DeleteRenderbuffers(1, ref _renderBuffer);
                GL.DeleteFramebuffers(1, ref _frameBuffer);
            }
        }
    }
}