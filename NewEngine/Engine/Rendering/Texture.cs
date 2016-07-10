using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using NewEngine.Engine.Core;
using NewEngine.Engine.Rendering.ResourceManagament;
using OpenTK.Graphics.OpenGL;

namespace NewEngine.Engine.Rendering {
    public enum TextureType {
        Linear,
        Point
    }

    public class Texture : IDisposable {
        private static Dictionary<string, TextureResource> _loadedTextures = new Dictionary<string, TextureResource>();
        private TextureResource _resource;
        private string _filename;

        public Texture(string filename, TextureType type = TextureType.Linear) {
            if (_loadedTextures.ContainsKey(filename)) {
                _resource = _loadedTextures[filename];
                _resource.AddReference();
            }
            else {
                _resource = new TextureResource(LoadTexture(filename, type));
                _loadedTextures.Add(filename, _resource);
            }
        }

        public void Bind() {
            GL.BindTexture(TextureTarget.Texture2D, _resource.Id);
        }

        private static int LoadTexture(string filename, TextureType type) {
            try {
                Bitmap image = new Bitmap(Path.Combine("./res/textures", filename));

                var textureData = image.LockBits(new Rectangle(
                            0, 0, image.Width, image.Height),
                            System.Drawing.Imaging.ImageLockMode.ReadOnly,
                            System.Drawing.Imaging.PixelFormat.Format32bppRgb);

                int id;

                GL.GenTextures(1, out id);
                GL.BindTexture(TextureTarget.Texture2D, id);

                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

                switch (type) {
                    case TextureType.Linear:
                        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (float)TextureMinFilter.Linear);
                        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (float)TextureMagFilter.Linear);
                        break;
                    case TextureType.Point:
                        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (float)TextureMinFilter.Nearest);
                        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (float)TextureMagFilter.Nearest);
                        break;
                }

                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb8, textureData.Width, textureData.Height, 0, PixelFormat.Bgra, PixelType.UnsignedByte, textureData.Scan0);

                image.UnlockBits(textureData);

                return id;
            }
            catch (Exception e) {
                LogManager.Error("Failed to load texture: " + e.Message);
            }

            return 0;
        }

        public void Dispose() {
            if (_resource.RemoveReference() && _filename != "") {
                _loadedTextures.Remove(_filename);
            }
        }
    }
}
