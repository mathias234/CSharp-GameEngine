using System;
using System.Collections.Generic;
using System.Linq;
using NewEngine.Engine.Core;

namespace NewEngine.Engine.Rendering.ResourceManagament {
    public static class ResourceManager {
        private static Dictionary<string, IResourceManaged> _storedResources = new Dictionary<string, IResourceManaged>();
        private static int _lastID;

        public static T CreateResource<T>(bool newID, params object[] parameters) where T : IResourceManaged {
            var objectName = parameters.Aggregate("", (current, parameter) => current + parameter.ToString());

            if (newID)
                objectName += _lastID + 1;

            _lastID++;

            if (_storedResources.ContainsKey(objectName)) {
                return (T)_storedResources[objectName];
            }

            var resource = (T)Activator.CreateInstance(typeof(T), parameters);
            _storedResources.Add(objectName, resource);

            return (T)_storedResources[objectName];
        }

        public static void CleanupResources() {
            foreach (var resourceManaged in _storedResources) {
                LogManager.Debug("Removing resource: " + resourceManaged.Value.GetType().FullName);
                resourceManaged.Value.Cleanup();
            }
        }
    }
}
