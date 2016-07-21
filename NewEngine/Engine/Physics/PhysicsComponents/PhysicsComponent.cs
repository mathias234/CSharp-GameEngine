using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BEPUphysics;
using NewEngine.Engine.components;
using OpenTK;

namespace NewEngine.Engine.Physics.PhysicsComponents {
    public class PhysicsComponent : GameComponent {
        protected ISpaceObject _physicsObject;

        protected BEPUutilities.Vector3 ToBepuVector3(Vector3 vector3) {
            return new BEPUutilities.Vector3(vector3.X, vector3.Y, vector3.Z);
        }
        protected Vector3 FromBepuVector3(BEPUutilities.Vector3 vector3) {
            return new Vector3(vector3.X, vector3.Y, vector3.Z);
        }
        protected Quaternion FromBepuQuaternion(BEPUutilities.Quaternion quaternion) {
            return new Quaternion(quaternion.X, quaternion.Y, quaternion.Z, quaternion.W);
        }

        public override void OnDestroyed() {
            PhysicsEngine.RemoveFromPhysicsEngine(PhysicsObject);
        }

        public ISpaceObject PhysicsObject {
            get { return _physicsObject; }
            set { _physicsObject = value; }
        }
    }
}
