using System.Collections.Generic;
using System.Drawing;
using BEPUutilities;
using NewEngine.Engine.Core;
using NewEngine.Engine.Rendering.ResourceManagament;

namespace NewEngine.Engine.Rendering {
    public class Material : MappedValues {
        private Dictionary<string, Texture> _stringTextureMap;

        public Material(Texture diffuse, float specularIntensity = 0.5f, float specularPower = 32.0f, Texture normalMap = null, Texture dispMap = null, float dispScale = 0.0f, float dispMapOffset = 0.0f) : base() {
            _stringTextureMap = new Dictionary<string, Texture>();
            AddTexture("diffuse", diffuse);

            if (normalMap == null)
                AddTexture("normalMap", new Texture("default_normal.png"));
            else
                AddTexture("normalMap", normalMap);

            if (dispMap == null)
                AddTexture("dispMap", new Texture("default_disp.jpg"));
            else
                AddTexture("dispMap", dispMap);

            AddFloat("dispMapScale", dispScale);

            float baseBias = GetFloat("dispMapScale") / 2.0f;

            AddFloat("dispMapBias", -baseBias + baseBias * dispMapOffset);

            AddFloat("specularIntensity", specularIntensity);
            AddFloat("specularPower", specularPower);
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
