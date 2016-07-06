using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BEPUphysics;
using BEPUutilities.Threading;

namespace NewEngine.Engine.Physics {
    public static class PhysicsEngine {
        private static Space _physicsSpace;

        public static void Update(float deltaTime) {
            if (_physicsSpace == null) {
                _physicsSpace = new Space(new ParallelLooper());
                _physicsSpace.ForceUpdater.Gravity = new BEPUutilities.Vector3(0, -9.81f, 0);
            }

            _physicsSpace.Update(deltaTime);
        }

        public static void AddToPhysicsEngine(ISpaceObject obj) {
            _physicsSpace.Add(obj);
        }
    }
}
