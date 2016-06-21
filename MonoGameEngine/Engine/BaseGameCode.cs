using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoGameEngine.Engine {
    public class BaseGameCode {
        public virtual void Initialize() {}
        public virtual void Update(float deltaTime) { }
    }
}
