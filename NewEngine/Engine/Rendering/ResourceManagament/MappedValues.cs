using System.Collections.Generic;
using OpenTK;

namespace NewEngine.Engine.Rendering.ResourceManagament {
    public abstract class MappedValues {
        private static Texture defaultTexture;

        private Dictionary<string, CubemapTexture> _stringCubemapTextureMap;
        private Dictionary<string, float> _stringFloatMap;
        private Dictionary<string, Texture> _stringTextureMap;
        private Dictionary<string, Vector3> _stringVector3Map;
        private Dictionary<string, Vector4> _stringVector4Map;

        protected MappedValues() {
            if(defaultTexture == null)
               defaultTexture = new Texture("default_mask.png");

            _stringVector3Map = new Dictionary<string, Vector3>();
            _stringVector4Map = new Dictionary<string, Vector4>();
            _stringFloatMap = new Dictionary<string, float>();
            _stringTextureMap = new Dictionary<string, Texture>();
            _stringCubemapTextureMap = new Dictionary<string, CubemapTexture>();
        }

        public void SetFloat(string name, float value) {
            if (_stringFloatMap.ContainsKey(name))
                _stringFloatMap[name] = value;
            else
                _stringFloatMap.Add(name, value);
        }

        public void SetVector3(string name, Vector3 value) {
            if (_stringVector3Map.ContainsKey(name))
                _stringVector3Map[name] = value;
            else
                _stringVector3Map.Add(name, value);
        }

        public void SetVector4(string name, Vector4 value) {
            if (_stringVector4Map.ContainsKey(name))
                _stringVector4Map[name] = value;
            else
                _stringVector4Map.Add(name, value);
        }


        public void SetTexture(string name, Texture texture) {
            if (_stringTextureMap.ContainsKey(name))
                _stringTextureMap[name] = texture;
            else
                _stringTextureMap.Add(name, texture);
        }

        public void SetCubemapTexture(string name, CubemapTexture texture) {
            if (_stringCubemapTextureMap.ContainsKey(name))
                _stringCubemapTextureMap[name] = texture;
            else
                _stringCubemapTextureMap.Add(name, texture);
        }

        public Vector3 GetVector3(string name) {
            return _stringVector3Map.ContainsKey(name) ? _stringVector3Map[name] : Vector3.Zero;
        }

        public Vector4 GetVector4(string name) {
            return _stringVector4Map.ContainsKey(name) ? _stringVector4Map[name] : Vector4.Zero;
        }

        public float GetFloat(string name) {
            return _stringFloatMap.ContainsKey(name) ? _stringFloatMap[name] : 0;
        }

        public Texture GetTexture(string name) {
            return _stringTextureMap.ContainsKey(name) ? _stringTextureMap[name] : defaultTexture;
        }


        public CubemapTexture GetCubemapTexture(string name) {
            return _stringCubemapTextureMap.ContainsKey(name) ? _stringCubemapTextureMap[name] : null;
        }
    }
}