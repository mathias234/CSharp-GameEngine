using System.Collections.Generic;
using NewEngine.Engine.Core;
using NewEngine.Engine.Rendering.ResourceManagament;

namespace NewEngine.Engine.Rendering {
    public class CubemapTexture : IResourceManaged {
        private CubemapResource _resource;
        private string _filename;

        public CubemapTexture(string textureTop, string textureBottom, string textureFront, string textureBack,
            string textureLeft, string textureRight) {
            _filename = textureTop + textureBottom + textureFront + textureBack + textureLeft + textureRight;

            _resource = new CubemapResource(textureTop, textureBottom, textureFront, textureBack, textureLeft,
                textureRight);
        }

        public static CubemapTexture GetCubemap(string textureTop, string textureBottom, string textureFront, string textureBack,
            string textureLeft, string textureRight) {
            return ResourceManager.CreateResource<CubemapTexture>(false, textureTop, textureBottom, textureFront, textureBack, textureLeft,
                    textureRight);
        }

        public void Cleanup() {
            LogManager.Debug("removing CubemapTexture : " + _filename);
            _resource.Cleanup();
        }

        public void Bind(int samplerSlot) {
            _resource.Bind(samplerSlot);
        }
    }
}