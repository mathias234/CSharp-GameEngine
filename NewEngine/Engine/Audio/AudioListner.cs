using NewEngine.Engine.components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewEngine.Engine.Audio {
    public class AudioListener : GameComponent {
        public override void Update(float deltaTime) {
            base.Update(deltaTime);
            AudioMaster.SetListener(Transform.GetTransformedPosition().X, Transform.GetTransformedPosition().Y, Transform.GetTransformedPosition().Z);
        }
    }
}
