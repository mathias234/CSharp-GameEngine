using System;
using System.Drawing;
using System.IO;
using NewEngine.Engine.Core;
using OpenTK.Graphics.OpenGL;

namespace NewEngine.Engine.Rendering {
    public class Texture {
        private int _id;

        public Texture(string fileName) {
            _id = LoadTexture(fileName);
        }

        public Texture(int id) {
            this._id = id;
        }

        public void Bind() {
            GL.BindTexture(TextureTarget.Texture2D, Id);
        }

        public int Id {
            get { return _id; }
        }

        private static int LoadTexture(string filename) {
            try {
                Bitmap image = new Bitmap(Path.Combine("./res/textures", filename));

                System.Drawing.Imaging.BitmapData textureData =
                    image.LockBits(
                        new Rectangle(
                            0, 0, image.Width, image.Height),
                            System.Drawing.Imaging.ImageLockMode.ReadOnly,
                            System.Drawing.Imaging.PixelFormat.Format32bppRgb);

                //Code to get the data to the OpenGL Driver
                int texture;

                //generate one texture and put its ID number into the "Texture" variable
                GL.GenTextures(1, out texture);
                //tell OpenGL that this is a 2D texture
                GL.BindTexture(TextureTarget.Texture2D, texture);

                //the following code sets certian parameters for the texture
                GL.TexEnv(TextureEnvTarget.TextureEnv,
                TextureEnvParameter.TextureEnvMode,
                (float)TextureEnvMode.Modulate);

                GL.TexParameter(TextureTarget.Texture2D,
                TextureParameterName.TextureMinFilter, (float)TextureMinFilter.Nearest);

                GL.TexParameter(TextureTarget.Texture2D,
                TextureParameterName.TextureMagFilter,
                (float)TextureMagFilter.Nearest);

                //load the data by telling OpenGL to build mipmaps out of the bitmap data
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.GenerateMipmap, 1); // 1 = True
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, textureData.Width, textureData.Height, 0, PixelFormat.Bgra, PixelType.UnsignedByte, textureData.Scan0);

                //free the bitmap data (we dont need it anymore because it has been passed to the OpenGL driver
                image.UnlockBits(textureData);

                return  texture;
            }
            catch (Exception e) {
                LogManager.Error("Failed to load texture: " + e.Message);
            }

            return 0;
        }
    }
}
