using System.Collections.Generic;
using System.Drawing;
using BEPUutilities;
using NewEngine.Engine.Core;

namespace NewEngine.Engine.Rendering {
    public class Material {
        private Dictionary<string, Texture> _stringTextureMap;
        private Dictionary<string, Vector3> _stringVector3Map;
        private Dictionary<string, float> _stringFloatMap;

        public Material() {
            _stringTextureMap = new Dictionary<string, Texture>();
            _stringVector3Map = new Dictionary<string, Vector3>();
            _stringFloatMap = new Dictionary<string, float>();
        }

        public Material AddTexture(string name, Texture texture) {
            _stringTextureMap.Add(name, texture);
            return this;
        }

        public Material AddVector3(string name, Vector3 vector3) {
            _stringVector3Map.Add(name, vector3);
            return this;
        }

        public Material AddFloat(string name, float value) {
            _stringFloatMap.Add(name, value);
            return this;
        }

        public Texture GetTexture(string name) {
            if (_stringTextureMap.ContainsKey(name)) {
                return _stringTextureMap[name];
            }

            return new Texture("test.png");
        }

        public Vector3 GetVector3(string name) {
            if (_stringVector3Map.ContainsKey(name)) {
                return _stringVector3Map[name];
            }

            return Vector3.Zero;
        }

        public float GetFloat(string name) {
            if (_stringFloatMap.ContainsKey(name)) {
                return _stringFloatMap[name];
            }

            return 0;
        }
    }
}
