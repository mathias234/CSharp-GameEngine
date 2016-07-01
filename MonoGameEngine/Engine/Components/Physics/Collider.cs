using BEPUphysics;
using MonoGameEngine.Engine.Components;

namespace MonoGameEngine.Engine.Physics {
    public class Collider : Component {
        public float Mass { get; set; }
        public ISpaceObject RigidBody { get; protected set; }


        public override void Init() {
            base.Init();
        }
    }
}
