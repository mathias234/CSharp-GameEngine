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
    public class Texture : IResourceManaged {
        private TextureResource _resource;
        private TextureTarget _target;
        private string _filename;

        private int _width;
        private int _height;

        /// <summary>
        /// Use GetTexture, if you dont the resource manager WILL not handle this instance
        /// </summary>
        public Texture(string filename, TextureTarget target = TextureTarget.Texture2D,
            TextureMinFilter filter = TextureMinFilter.LinearMipmapLinear, PixelInternalFormat internalFormat = PixelInternalFormat.Rgba,
            PixelFormat format = PixelFormat.Bgra, bool clamp = false,
            FramebufferAttachment attachment = FramebufferAttachment.ColorAttachment0) {

            _filename = filename;

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
            _target = target;
        }
        /// <summary>
        /// Resource managed version for filenames
        /// </summary>
        public static Texture GetTexture(string filename, TextureTarget target = TextureTarget.Texture2D,
            TextureMinFilter filter = TextureMinFilter.LinearMipmapLinear,
            PixelInternalFormat internalFormat = PixelInternalFormat.Rgba,
            PixelFormat format = PixelFormat.Bgra, bool clamp = false,
            FramebufferAttachment attachment = FramebufferAttachment.ColorAttachment0) {
            return ResourceManager.CreateResource<Texture>(false, filename, target, filter, internalFormat, format, clamp, attachment);
        }

        /// <summary>
        /// Use GetTexture, if you dont the resource manager WILL not handle this instance
        /// </summary>
        public Texture(Bitmap image, TextureTarget target = TextureTarget.Texture2D,
            TextureMinFilter filter = TextureMinFilter.LinearMipmapLinear, PixelInternalFormat internalFormat = PixelInternalFormat.Rgba,
            PixelFormat format = PixelFormat.Bgra, bool clamp = false,
            FramebufferAttachment attachment = FramebufferAttachment.ColorAttachment0) {

            _filename = image.GetPixel(2, 3).ToString() + image.GetPixel(1, 1);
            _width = image.Width;
            _height = image.Height;

            _resource = LoadTexture(image, filter, internalFormat, format, clamp, attachment, target);
            _target = target;
        }

        /// <summary>
        /// Resource managed version of bitmap
        /// </summary>
        public static Texture GetTexture(Bitmap image, TextureTarget target = TextureTarget.Texture2D,
            TextureMinFilter filter = TextureMinFilter.LinearMipmapLinear, PixelInternalFormat internalFormat = PixelInternalFormat.Rgba,
            PixelFormat format = PixelFormat.Bgra, bool clamp = false,
            FramebufferAttachment attachment = FramebufferAttachment.ColorAttachment0) {
            return ResourceManager.CreateResource<Texture>(true, image, target, filter, internalFormat, format, clamp, attachment);
        }

        /// <summary>
        /// Use GetTexture, if you dont the resource manager WILL not handle this instance
        /// </summary>
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

        /// <summary>
        /// Resource managed version of char[]
        /// </summary>
        public static Texture GetTexture(char[] data, int width, int height, TextureMinFilter filter = TextureMinFilter.LinearMipmapLinear,
            PixelInternalFormat internalFormat = PixelInternalFormat.Rgba, PixelFormat format = PixelFormat.Bgra,
            bool clamp = false,
            FramebufferAttachment attachment = FramebufferAttachment.ColorAttachment0,
            TextureTarget target = TextureTarget.Texture2D) {
            return ResourceManager.CreateResource<Texture>(true, data, width, height, filter, internalFormat, format, clamp, attachment, target);
        }

        /// <summary>
        /// Use GetTexture, if you dont the resource manager WILL not handle this instance
        /// </summary>
        public Texture(IntPtr data, int width, int height, TextureMinFilter filter = TextureMinFilter.LinearMipmapLinear,
            PixelInternalFormat internalFormat = PixelInternalFormat.Rgba, PixelFormat format = PixelFormat.Bgra,
            bool clamp = false,
            FramebufferAttachment attachment = FramebufferAttachment.ColorAttachment0,
            TextureTarget target = TextureTarget.Texture2D) {
            _width = width;
            _height = height;

            _resource = LoadTexture(data, width, height, filter, internalFormat, format, clamp, attachment);
            _target = target;
        }
        /// <summary>
        /// Resource managed version of IntPtr
        /// </summary>
        public static Texture GetTexture(IntPtr data, int width, int height, TextureMinFilter filter = TextureMinFilter.LinearMipmapLinear,
            PixelInternalFormat internalFormat = PixelInternalFormat.Rgba, PixelFormat format = PixelFormat.Bgra,
            bool clamp = false,
            FramebufferAttachment attachment = FramebufferAttachment.ColorAttachment0,
            TextureTarget target = TextureTarget.Texture2D) {
            return ResourceManager.CreateResource<Texture>(true, data, width, height, filter, internalFormat, format, clamp, attachment, target);
        }

        public void Cleanup() {
            LogManager.Debug("removing texture : " + _filename);
            _resource.Cleanup();
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