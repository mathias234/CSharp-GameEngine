using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NewEngine.Engine.Rendering.Shading;

namespace NewEngine.Engine.Rendering.ResourceManagament {
    public static class ResourceManager {
        private static Dictionary<object[], IResourceManaged> _storedResources = new Dictionary<object[], IResourceManaged>();

        public static T CreateResource<T>(params object[] parameters) where T : IResourceManaged {
            if (_storedResources.ContainsKey(parameters)) {
                return (T)_storedResources[parameters];
            }

            var resource = (T)Activator.CreateInstance(typeof(T), parameters);
            _storedResources.Add(parameters, resource);

            return (T)_storedResources[parameters];
        }

        public static void CleanupResources() {
            foreach (var resourceManaged in _storedResources) {
                resourceManaged.Value.Cleanup();
            }
        }
    }
}
