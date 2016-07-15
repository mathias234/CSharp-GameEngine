using System.Collections.Generic;
using OpenTK;

namespace NewEngine.Engine.Rendering.ResourceManagament {
    public abstract class MappedValues {
        private Dictionary<string, Vector3> _stringVector3Map;
        private Dictionary<string, float> _stringFloatMap;

        public MappedValues() {
            _stringVector3Map = new Dictionary<string, Vector3>();
            _stringFloatMap = new Dictionary<string, float>();
        }

        public void AddFloat(string name, float value) {
            _stringFloatMap.Add(name, value);
        }

        public void AddVector3(string name, Vector3 value) {
            _stringVector3Map.Add(name, value);
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
