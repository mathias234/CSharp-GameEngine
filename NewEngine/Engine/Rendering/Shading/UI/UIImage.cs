using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NewEngine.Engine.Core;
using OpenTK;

namespace NewEngine.Engine.Rendering.Shading.UI {
    public class UIImage : Shader {

        private static UIImage _instance;

        public static UIImage Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new UIImage();

                return _instance;
            }
        }


        public UIImage() : base("UIImage") {

        }
    }
}
