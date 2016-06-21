using Jitter.Dynamics;
using SwiftEngine.Components;

namespace SwiftEngine.Engine.Components {
    public class Collider : Component {
        public float Mass { get; set; }
        protected RigidBody RigidBody;


        public override void Init() {
            base.Init();
        }
    }
}
