using Jitter.Dynamics;
using MonoGameEngine.Components;

namespace MonoGameEngine.Engine.Components {
    public class Collider : Component {
        public float Mass { get; set; }
        protected RigidBody RigidBody;


        public override void Init() {
            base.Init();
        }
    }
}
