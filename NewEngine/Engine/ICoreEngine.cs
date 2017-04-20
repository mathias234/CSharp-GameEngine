using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NewEngine.Engine.Rendering;
using NewEngine.Engine.Rendering.GUI;

namespace NewEngine.Engine {
    public interface ICoreEngine {
        RenderingEngine RenderingEngine { get; set; }
        GUIRenderer GUIRenderingEngine { get; set; }
        bool Focused { get; }
        void SwapBuffers();
    }
}
