using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using NewEngine.Engine.Core;
using NewEngine.Engine.Rendering.ResourceManagament;
using OpenTK.Graphics.OpenGL;

namespace NewEngine.Engine.Rendering {
    public enum TextureFilter {
        Linear,
        Point
    }

    public class Texture {
        private static Dictionary<string, TextureResource> _loadedTextures = new Dictionary<string, TextureResource>();
        private TextureResource _resource;
        private string _filename;

        public Texture(string filename, TextureFilter filter = TextureFilter.Linear) {
            if (_loadedTextures.ContainsKey(filename)) {
                _resource = _loadedTextures[filename];
                _resource.AddReference();
            }
            else {
                _resource = LoadTexture(filename, filter);
                _loadedTextures.Add(filename, _resource);
            }
        }

        ~Texture() {
            if (_resource.RemoveReference() && _filename != "") {
                _loadedTextures.Remove(_filename);
            }
        }

        public Texture(Bitmap image, TextureFilter filter = TextureFilter.Linear) {
            _resource = LoadTexture(image, filter);
        }

        public void BindAsRenderTarget() {

        }

        public void Bind() {
            Bind(0);
        }

        public void Bind(int samplerSlot) {
            GL.ActiveTexture(TextureUnit.Texture0 + samplerSlot);
            _resource.Bind(0);
        }

        private static TextureResource LoadTexture(Bitmap image, TextureFilter filter) {
            try {
                if (image == null)
                    return null;

                var textureData = image.LockBits(new Rectangle(
                            0, 0, image.Width, image.Height),
                            System.Drawing.Imaging.ImageLockMode.ReadOnly,
                            System.Drawing.Imaging.PixelFormat.Format32bppRgb);

                TextureResource resource = new TextureResource(1, new[] { textureData }, new[] { filter });


                image.UnlockBits(textureData);

                return resource;
            }
            catch (Exception e) {
                LogManager.Error("Failed to load texture: " + e.Message);
            }

            return null;
        }

        private static TextureResource LoadTexture(string filename, TextureFilter filter) {
            Bitmap image;
            if (File.Exists(Path.Combine("./res/textures", filename)))
                image = new Bitmap(Path.Combine("./res/textures", filename));
            else {
                image = new Bitmap(Path.Combine("./res/textures", "default_normal.png"));

            }
            return LoadTexture(image, filter);
        }

   
    }
}
