using System;
using System.Collections.Generic;
using NewEngine.Engine.Core;
using OpenTK.Graphics.OpenGL;

namespace NewEngine.Engine.Rendering.ResourceManagament {
    public class TextureResource {
        private int _frameBuffer;
        private int _height;
        private int[] _id;
        private int _numTextures;
        private int _renderBuffer;
        private int _width;

        public TextureResource(int numTexture, int width, int height, List<char[]> data, TextureFilter[] filters,
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
        }

        public TextureResource(int numTexture, int width, int height, IntPtr[] data, TextureFilter[] filters,
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
        }

        private void InitRenderTargets(FramebufferAttachment[] attachments, TextureTarget[] targets) {
            if (attachments.Length == 0)
                return;

            var drawBuffers = new DrawBuffersEnum[32];

            var hasDepth = false;
            for (var i = 0; i < _numTextures; i++) {
                if (attachments[i] == FramebufferAttachment.DepthAttachment) {
                    drawBuffers[i] = (DrawBuffersEnum) All.None;
                    hasDepth = true;
                }
                else
                    drawBuffers[i] = (DrawBuffersEnum) attachments[i];

                if ((All) attachments[i] == All.None) {
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
                GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, (RenderbufferStorage) All.DepthComponent, _width,
                    _height);
                GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment,
                    RenderbufferTarget.Renderbuffer, _renderBuffer);
            }

            GL.DrawBuffers(_numTextures, drawBuffers);

            if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferErrorCode.FramebufferComplete) {
                LogManager.Error("Framebuffer creation failed");
            }
        }

        private void InitTextures(List<char[]> data, TextureFilter[] filters, PixelInternalFormat[] internalFormat,
            PixelFormat[] format, bool clamp, TextureTarget[] targets) {
            GL.GenTextures(_numTextures, _id);

            for (var i = 0; i < _numTextures; i++) {
                GL.BindTexture(targets[i], _id[i]);


                if (clamp) {
                    GL.TexParameter(targets[i], TextureParameterName.TextureWrapS, (int) TextureWrapMode.ClampToEdge);
                    GL.TexParameter(targets[i], TextureParameterName.TextureWrapT, (int) TextureWrapMode.ClampToEdge);
                }

                switch (filters[i]) {
                    case TextureFilter.Linear:
                        GL.TexParameter(targets[i], TextureParameterName.TextureMinFilter,
                            (float) TextureMinFilter.Linear);
                        GL.TexParameter(targets[i], TextureParameterName.TextureMagFilter,
                            (float) TextureMagFilter.Linear);
                        break;
                    case TextureFilter.Point:
                        GL.TexParameter(targets[i], TextureParameterName.TextureMinFilter,
                            (float) TextureMinFilter.Nearest);
                        GL.TexParameter(targets[i], TextureParameterName.TextureMagFilter,
                            (float) TextureMagFilter.Nearest);
                        break;
                }

                GL.TexImage2D(targets[i], 0, internalFormat[i], _width, _height, 0, format[i], PixelType.UnsignedByte,
                    data[i]);
            }
        }


        private void InitTextures(IntPtr[] data, TextureFilter[] filters, PixelInternalFormat[] internalFormat,
            PixelFormat[] format, bool clamp, TextureTarget[] targets) {
            GL.GenTextures(_numTextures, _id);

            for (var i = 0; i < _numTextures; i++) {
                GL.BindTexture(targets[i], _id[i]);

                if (clamp) {
                    GL.TexParameter(targets[i], TextureParameterName.TextureWrapS, (int) TextureWrapMode.ClampToEdge);
                    GL.TexParameter(targets[i], TextureParameterName.TextureWrapT, (int) TextureWrapMode.ClampToEdge);
                }

                switch (filters[i]) {
                    case TextureFilter.Linear:
                        GL.TexParameter(targets[i], TextureParameterName.TextureMinFilter,
                            (float) TextureMinFilter.Linear);
                        GL.TexParameter(targets[i], TextureParameterName.TextureMagFilter,
                            (float) TextureMagFilter.Linear);
                        break;
                    case TextureFilter.Point:
                        GL.TexParameter(targets[i], TextureParameterName.TextureMinFilter,
                            (float) TextureMinFilter.Nearest);
                        GL.TexParameter(targets[i], TextureParameterName.TextureMagFilter,
                            (float) TextureMagFilter.Nearest);
                        break;
                }
                GL.TexImage2D(targets[i], 0, internalFormat[i], _width, _height, 0, format[i], PixelType.UnsignedByte,
                    data[i]);
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
    }
}