using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using NewEngine.Engine.Core;
using NewEngine.Engine.Rendering.ResourceManagament;
using OpenTK.Graphics.OpenGL;
using PixelFormat = OpenTK.Graphics.OpenGL.PixelFormat;

namespace NewEngine.Engine.Rendering {
    public enum TextureFilter {
        Linear,
        Point
    }

    // TODO: maybe clean up the parameters soo much its choking me
    public class Texture {
        private static Dictionary<string, TextureResource> _loadedTextures = new Dictionary<string, TextureResource>();
        private TextureResource _resource;
        private TextureTarget _target;

        public Texture(string filename, TextureTarget target = TextureTarget.Texture2D,
            TextureFilter filter = TextureFilter.Linear, PixelInternalFormat internalFormat = PixelInternalFormat.Rgba,
            PixelFormat format = PixelFormat.Bgra, bool clamp = false,
            FramebufferAttachment attachment = FramebufferAttachment.ColorAttachment0) {
            if (_loadedTextures.ContainsKey(filename)) {
                _resource = _loadedTextures[filename];
            }
            else {
                _resource = LoadTexture(filename, filter, internalFormat, format, clamp, attachment, target);
                _loadedTextures.Add(filename, _resource);
            }
            _target = target;
        }

        public Texture(Bitmap image, TextureTarget target = TextureTarget.Texture2D,
            TextureFilter filter = TextureFilter.Linear, PixelInternalFormat internalFormat = PixelInternalFormat.Rgba,
            PixelFormat format = PixelFormat.Bgra, bool clamp = false,
            FramebufferAttachment attachment = FramebufferAttachment.ColorAttachment0) {
            _resource = LoadTexture(image, filter, internalFormat, format, clamp, attachment, target);
            _target = target;
        }

        public Texture(IntPtr data, int width, int height, TextureTarget target = TextureTarget.Texture2D,
            TextureFilter filter = TextureFilter.Linear, PixelInternalFormat internalFormat = PixelInternalFormat.Rgba,
            PixelFormat format = PixelFormat.Bgra, bool clamp = false,
            FramebufferAttachment attachment = FramebufferAttachment.ColorAttachment0) {
            _resource = LoadTexture(data, width, height, filter, internalFormat, format, clamp, attachment, target);
            _target = target;
        }

        /// <param name="data">Can be null, it will not break anything</param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="filter"></param>
        /// <param name="attachment"></param>
        /// <param name="internalFormat"></param>
        /// <param name="format"></param>
        public Texture(char[] data, int width, int height, TextureFilter filter = TextureFilter.Linear,
            PixelInternalFormat internalFormat = PixelInternalFormat.Rgba, PixelFormat format = PixelFormat.Bgra,
            bool clamp = false,
            FramebufferAttachment attachment = FramebufferAttachment.ColorAttachment0,
            TextureTarget target = TextureTarget.Texture2D) {
            _resource = LoadTexture(data, width, height, filter, internalFormat, format, clamp, attachment, target);
            _target = target;
        }

        public void BindAsRenderTarget() {
            _resource.BindAsRenderTarget();
        }

        public void Bind() {
            Bind(0, _target);
        }

        public void Bind(int samplerSlot, TextureTarget target) {
            GL.ActiveTexture(TextureUnit.Texture0 + samplerSlot);
            _resource.Bind(0, target);
        }

        private static TextureResource LoadTexture(Bitmap image, TextureFilter filter,
            PixelInternalFormat internalFormat = PixelInternalFormat.Rgba, PixelFormat format = PixelFormat.Bgra,
            bool clamp = false, FramebufferAttachment attachment = FramebufferAttachment.ColorAttachment0,
            TextureTarget target = TextureTarget.Texture2D) {
            if (image == null)
                return null;


            var textureData = image.LockBits(new Rectangle(
                0, 0, image.Width, image.Height),
                ImageLockMode.ReadOnly,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            var resource = LoadTexture(textureData.Scan0, textureData.Width, textureData.Height, filter, internalFormat,
                format, clamp,
                attachment, target);

            image.UnlockBits(textureData);
            image.Dispose();

            return resource;
        }

        private static TextureResource LoadTexture(char[] data, int width, int height, TextureFilter filter,
            PixelInternalFormat internalFormat = PixelInternalFormat.Rgba, PixelFormat format = PixelFormat.Bgra,
            bool clamp = false,
            FramebufferAttachment attachment = FramebufferAttachment.ColorAttachment0,
            TextureTarget target = TextureTarget.Texture2D) {
            try {
                var resource = new TextureResource(1, width, height, new List<char[]> {data}, new[] {filter},
                    new[] {internalFormat}, new[] {format}, clamp, new[] {attachment}, new[] {target});

                return resource;
            }
            catch (Exception e) {
                LogManager.Error("Failed to load texture: " + e.Message);
            }

            return null;
        }


        private static TextureResource LoadTexture(IntPtr data, int width, int height, TextureFilter filter,
            PixelInternalFormat internalFormat = PixelInternalFormat.Rgba, PixelFormat format = PixelFormat.Bgra,
            bool clamp = false,
            FramebufferAttachment attachment = FramebufferAttachment.ColorAttachment0,
            TextureTarget target = TextureTarget.Texture2D) {
            try {
                var resource = new TextureResource(1, width, height, new[] {data}, new[] {filter},
                    new[] {internalFormat}, new[] {format}, clamp, new[] {attachment}, new[] {target});

                return resource;
            }
            catch (Exception e) {
                LogManager.Error("Failed to load texture: " + e.Message);
            }

            return null;
        }

        private static TextureResource LoadTexture(string filename, TextureFilter filter,
            PixelInternalFormat internalFormat = PixelInternalFormat.Rgba, PixelFormat format = PixelFormat.Bgra,
            bool clamp = false, FramebufferAttachment attachment = FramebufferAttachment.ColorAttachment0,
            TextureTarget target = TextureTarget.Texture2D) {
            Bitmap image;
            if (File.Exists(Path.Combine("./res/textures", filename)))
                image = new Bitmap(Path.Combine("./res/textures", filename));
            else {
                LogManager.Error("Image does not exists");
                image = new Bitmap(Path.Combine("./res/textures", "default_mask.png"));
            }

            return LoadTexture(image, filter, internalFormat, format, clamp, attachment, target);
        }
    }
}