using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NewEngine.Engine.Rendering;

namespace NewEngine.Engine {
    public interface ICoreEngine {
        RenderingEngine RenderingEngine { get; set; }
        bool Focused { get; }
        void SwapBuffers();
    }
}
