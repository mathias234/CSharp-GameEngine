using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using MonoGameEngine.Engine.UI;

namespace MonoGameEngine.Engine.Components.UI {
    public class UIScrollRect : UIComponent {
        public UIScrollBar Scrollbar;
        public UIComponent ScrollRect;
        public int ScrollAmount;

        public float lastScrollAmount;
        public float currScrollAmount;

        public override void Update(float deltaTime) {
            base.Update(deltaTime);

            Debug.WriteLine(ScrollRect);

            lastScrollAmount = currScrollAmount;
            currScrollAmount = Scrollbar.value;

            var scrollDelta = currScrollAmount - lastScrollAmount;

            ScrollRect.Rect =
                new Rectangle(ScrollRect.Rect.X, (int)(Scrollbar.value * ScrollRect.Rect.Height),
                ScrollRect.Rect.Width, ScrollRect.Rect.Height);
        }
    }
}
