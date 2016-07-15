using System.Collections.Generic;
using System.Drawing;
using BEPUutilities;
using NewEngine.Engine.Core;
using NewEngine.Engine.Rendering.ResourceManagament;

namespace NewEngine.Engine.Rendering {
    public class Material : MappedValues {
        private Dictionary<string, Texture> _stringTextureMap;

        public Material() : base() {
            _stringTextureMap = new Dictionary<string, Texture>();
        }

        public Material AddTexture(string name, Texture texture) {
            _stringTextureMap.Add(name, texture);
            return this;
        }

        public Texture GetTexture(string name) {
            if (_stringTextureMap.ContainsKey(name)) {
                return _stringTextureMap[name];
            }

            return new Texture("test.png");
        }
    }
}
