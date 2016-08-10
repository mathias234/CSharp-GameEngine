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
    // TODO: maybe clean up the parameters soo much its choking me
    public class Texture {
        private static Dictionary<string, TextureResource> _loadedTextures = new Dictionary<string, TextureResource>();
        private TextureResource _resource;
        private TextureTarget _target;
        private string _filename;

        private int _width;
        private int _height;

        public Texture(string filename, TextureTarget target = TextureTarget.Texture2D,
            TextureMinFilter filter = TextureMinFilter.LinearMipmapLinear, PixelInternalFormat internalFormat = PixelInternalFormat.Rgba,
            PixelFormat format = PixelFormat.Bgra, bool clamp = false,
            FramebufferAttachment attachment = FramebufferAttachment.ColorAttachment0) {

            _filename = filename;

            if (_loadedTextures.ContainsKey(filename)) {
                _resource = _loadedTextures[filename];
                _resource.AddReference();
            }
            else {
                Bitmap image;
                if (File.Exists(Path.Combine("./res/textures", filename)))
                    image = new Bitmap(Path.Combine("./res/textures", filename));
                else {
                    LogManager.Error("Image does not exists");
                    image = new Bitmap(Path.Combine("./res/textures", "default_mask.png"));
                }
                _width = image.Width;
                _height = image.Height;

                _resource = LoadTexture(image, filter, internalFormat, format, clamp, attachment, target);
                _loadedTextures.Add(filename, _resource);
            }
            _target = target;
        }

        public Texture(Bitmap image, TextureTarget target = TextureTarget.Texture2D,
            TextureMinFilter filter = TextureMinFilter.LinearMipmapLinear, PixelInternalFormat internalFormat = PixelInternalFormat.Rgba,
            PixelFormat format = PixelFormat.Bgra, bool clamp = false,
            FramebufferAttachment attachment = FramebufferAttachment.ColorAttachment0) {

            _filename = image.GetPixel(2, 3).ToString() + image.GetPixel(1, 1);

            if (_loadedTextures.ContainsKey(_filename)) {
                _resource = _loadedTextures[_filename];
                _resource.AddReference();
            }
            else {
                _width = image.Width;
                _height = image.Height;

                _resource = LoadTexture(image, filter, internalFormat, format, clamp, attachment, target);
                _loadedTextures.Add(_filename, _resource);
            }
            _target = target;
        }


        public Texture(char[] data, int width, int height, TextureMinFilter filter = TextureMinFilter.LinearMipmapLinear,
            PixelInternalFormat internalFormat = PixelInternalFormat.Rgba, PixelFormat format = PixelFormat.Bgra,
            bool clamp = false,
            FramebufferAttachment attachment = FramebufferAttachment.ColorAttachment0,
            TextureTarget target = TextureTarget.Texture2D) {

            _width = width;
            _height = height;

            _resource = LoadTexture(data, width, height, filter, internalFormat, format, clamp, attachment, target);
            _target = target;
        }

        public Texture(IntPtr data, int width, int height, TextureMinFilter filter = TextureMinFilter.LinearMipmapLinear,
            PixelInternalFormat internalFormat = PixelInternalFormat.Rgba, PixelFormat format = PixelFormat.Bgra,
            bool clamp = false,
            FramebufferAttachment attachment = FramebufferAttachment.ColorAttachment0,
            TextureTarget target = TextureTarget.Texture2D) {
            _width = width;
            _height = height;

            _resource = LoadTexture(data, width, height, filter, internalFormat, format, clamp, attachment, target);
            _target = target;
        }



        ~Texture() {
            LogManager.Debug("removing texture : " + _filename);
            if (_resource != null && _resource.RemoveReference()) {
                if (_filename != null) {
                    _loadedTextures.Remove(_filename);
                }
            }
        }

        public int Width
        {
            get { return _width; }
        }

        public int Height
        {
            get { return _height; }
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


        public void BindDepthBuffer(int samplerSlot, TextureTarget target) {
            GL.ActiveTexture(TextureUnit.Texture0 + samplerSlot);
            _resource.Bind(1, target);
        }

        private static TextureResource LoadTexture(Bitmap image, TextureMinFilter filter,
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

        private static TextureResource LoadTexture(char[] data, int width, int height, TextureMinFilter filter,
            PixelInternalFormat internalFormat = PixelInternalFormat.Rgba, PixelFormat format = PixelFormat.Bgra,
            bool clamp = false,
            FramebufferAttachment attachment = FramebufferAttachment.ColorAttachment0,
            TextureTarget target = TextureTarget.Texture2D) {
            try {

                var resource = new TextureResource(1, width, height, new List<char[]> { data }, new[] { filter },
                    new[] { internalFormat }, new[] { format }, clamp, new[] { attachment }, new[] { target });

                return resource;
            }
            catch (Exception e) {
                LogManager.Error("Failed to load texture: " + e.Message);
            }

            return null;
        }


        private static TextureResource LoadTexture(IntPtr data, int width, int height, TextureMinFilter filter,
            PixelInternalFormat internalFormat = PixelInternalFormat.Rgba, PixelFormat format = PixelFormat.Bgra,
            bool clamp = false,
            FramebufferAttachment attachment = FramebufferAttachment.ColorAttachment0,
            TextureTarget target = TextureTarget.Texture2D) {
            try {

                var resource = new TextureResource(1, width, height, new[] { data }, new[] { filter },
                new[] { internalFormat }, new[] { format }, clamp, new[] { attachment }, new[] { target });

                return resource;
            }
            catch (Exception e) {
                LogManager.Error("Failed to load texture: " + e.Message);
            }

            return null;
        }
    }
}