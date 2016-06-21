using Jitter.Dynamics;
using MonoGameEngine.Engine.Components;

namespace MonoGameEngine.Engine.Physics {
    public class Collider : Component {
        public float Mass { get; set; }
        protected RigidBody RigidBody;


        public override void Init() {
            base.Init();
        }
    }
}
