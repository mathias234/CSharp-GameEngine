using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NewEngine.Engine.components;
using NewEngine.Engine.Core;
using NewEngine.Engine.Rendering.ResourceManagament;
using NewEngine.Engine.Rendering.Shading;

namespace NewEngine.Engine.Rendering {
    public class BaseRenderingEngine : MappedValues {
        protected Dictionary<string, int> SamplerMap;

        public BaseRenderingEngine() {
        }

        public virtual void Render(float time) {

        }

        public virtual void AddToEngine(GameComponent gc) {
        }

        public virtual void UpdateUniformStruct(Transform transform, Material material, Shader shader, string uniformName, string uniformType) {
        }

        public int GetSamplerSlot(string samplerName) {
            return SamplerMap[samplerName];
        }
    }
}
