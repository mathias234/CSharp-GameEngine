using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameEngine.Engine.UI;

namespace MonoGameEngine.Engine.Components.UI {
    public class UIMask : UIComponent {
        // if null use rect as mask
        public Texture2D ImageMask;
    }
}
