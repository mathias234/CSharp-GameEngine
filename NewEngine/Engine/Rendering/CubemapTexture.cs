using System.Collections.Generic;
using NewEngine.Engine.Core;
using NewEngine.Engine.Rendering.ResourceManagament;

namespace NewEngine.Engine.Rendering {
    public class CubemapTexture {
        private static Dictionary<string, CubemapResource> _loadedCubemaps = new Dictionary<string, CubemapResource>();
        private CubemapResource _resource;
        private string _filename;

        public CubemapTexture(string textureTop, string textureBottom, string textureFront, string textureBack,
            string textureLeft, string textureRight) {
            _filename = textureTop + textureBottom + textureFront + textureBack + textureLeft + textureRight;

            if (_loadedCubemaps.ContainsKey(_filename)) {
                _resource = _loadedCubemaps[_filename];
            }
            else {
                _resource = new CubemapResource(textureTop, textureBottom, textureFront, textureBack, textureLeft,
                    textureRight);
                _loadedCubemaps.Add(_filename, _resource);
            }
        }


        ~CubemapTexture() {
            LogManager.Debug("removing CubemapTexture : " + _filename);
            if (_resource != null && _resource.RemoveReference()) {
                if (_filename != null) {
                    _loadedCubemaps.Remove(_filename);
                }
            }
        }

        public void Bind(int samplerSlot) {
            _resource.Bind(samplerSlot);
        }
    }
}