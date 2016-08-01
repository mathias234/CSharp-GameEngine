using BEPUphysics;
using BEPUutilities;
using NewEngine.Engine.components;
using NewEngine.Engine.Core;
using Quaternion = OpenTK.Quaternion;

namespace NewEngine.Engine.Physics.PhysicsComponents {
    public class PhysicsComponent : GameComponent {
        public ISpaceObject PhysicsObject { get; set; }

        protected Vector3 ToBepuVector3(OpenTK.Vector3 vector3) {
            return new Vector3(vector3.X, vector3.Y, vector3.Z);
        }

        protected OpenTK.Vector3 FromBepuVector3(Vector3 vector3) {
            return new OpenTK.Vector3(vector3.X, vector3.Y, vector3.Z);
        }

        protected Quaternion FromBepuQuaternion(BEPUutilities.Quaternion quaternion) {
            return new Quaternion(quaternion.X, quaternion.Y, quaternion.Z, quaternion.W);
        }

        public override void OnDestroyed(CoreEngine engine) {
            PhysicsEngine.RemoveFromPhysicsEngine(PhysicsObject);
        }
    }
}