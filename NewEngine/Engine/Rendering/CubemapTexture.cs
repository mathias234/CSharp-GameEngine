using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NewEngine.Engine.Core;
using NewEngine.Engine.Rendering.ResourceManagament;
using OpenTK.Graphics.OpenGL;

namespace NewEngine.Engine.Rendering {
    public class CubemapTexture {
        private static Dictionary<string, CubemapResource> _loadedCubemaps = new Dictionary<string, CubemapResource>();
        private CubemapResource _resource;

        public CubemapTexture(string textureTop, string textureBottom, string textureFront, string textureBack, string textureLeft, string textureRight) {
            var cubemapName = textureTop + textureBottom + textureFront + textureBack + textureLeft + textureRight;

            if (_loadedCubemaps.ContainsKey(cubemapName)) {
                _resource = _loadedCubemaps[cubemapName];
            }
            else {
                _resource = new CubemapResource(textureTop, textureBottom, textureFront, textureBack, textureLeft, textureRight);
                _loadedCubemaps.Add(cubemapName, _resource);
            }
        }


        public void Bind(int samplerSlot) {
            _resource.Bind(samplerSlot);
        }
    }
}
