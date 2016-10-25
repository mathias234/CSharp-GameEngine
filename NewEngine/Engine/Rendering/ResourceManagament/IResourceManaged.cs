using System.Security.Cryptography.X509Certificates;

namespace NewEngine.Engine.Rendering.ResourceManagament {
    public interface IResourceManaged {
        void Cleanup();
    }
}