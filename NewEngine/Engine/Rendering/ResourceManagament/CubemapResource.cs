﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NewEngine.Engine.Core;
using OpenTK.Graphics.OpenGL;

namespace NewEngine.Engine.Rendering.ResourceManagament {
   public class CubemapResource {
        private List<string> _textures = new List<string>();

        private int _cubeMapId;

       public CubemapResource(string textureTop, string textureBottom, string textureFront, string textureBack, string textureLeft, string textureRight) {
            _textures.Add(textureRight);
            _textures.Add(textureLeft);
            _textures.Add(textureTop);
            _textures.Add(textureBottom);
            _textures.Add(textureBack);
            _textures.Add(textureFront);

            InitCubemap();
        }

       public void Bind(int samplerSlot) {
            GL.ActiveTexture(TextureUnit.Texture0 + samplerSlot);
            GL.BindTexture(TextureTarget.TextureCubeMap, _cubeMapId);
        }


        private void InitCubemap() {
            GL.GenTextures(1, out _cubeMapId);
            GL.BindTexture(TextureTarget.TextureCubeMap, _cubeMapId);

            for (int i = 0; i < _textures.Count; i++) {
                Bitmap image;
                if (File.Exists(Path.Combine("./res/textures", _textures[i])))
                    image = new Bitmap(Path.Combine("./res/textures", _textures[i]));
                else {
                    LogManager.Debug("Cubemap Texture: " + Path.Combine("./res/textures", _textures[i]) + " does not exist");
                    image = new Bitmap(Path.Combine("./res/textures", "default_mask.png"));
                }

                var textureData = image.LockBits(new Rectangle(
                    0, 0, image.Width, image.Height),
                    System.Drawing.Imaging.ImageLockMode.ReadOnly,
                    System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                GL.TexImage2D(TextureTarget.TextureCubeMapPositiveX + i, 0, PixelInternalFormat.Rgba, textureData.Width, textureData.Height, 0, PixelFormat.Bgra, PixelType.UnsignedByte, textureData.Scan0);

                GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMinFilter, (float)TextureMinFilter.Linear);
                GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMagFilter, (float)TextureMagFilter.Linear);

                GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
                GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
                GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapR, (int)TextureWrapMode.ClampToEdge);
                image.UnlockBits(textureData);

                image.Dispose();
            }
        }
    }
}
