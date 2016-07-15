using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BEPUphysics;
using BEPUutilities.Threading;
using NewEngine.Engine.components;
using NewEngine.Engine.Core;
using NewEngine.Engine.Physics.PhysicsComponents;
using OpenTK;

namespace NewEngine.Engine.Physics {
    public static class PhysicsEngine {
        private static Space _physicsSpace;

        public static void Start() {

        }

        public static void Update(float deltaTime) {
            if (_physicsSpace == null) {
                return;
            }

            _physicsSpace.Update(deltaTime);
        }

        public static void AddToPhysicsEngine(ISpaceObject obj) {
            if (_physicsSpace == null) {
                _physicsSpace = new Space(new ParallelLooper());
                _physicsSpace.ForceUpdater.Gravity = new BEPUutilities.Vector3(0, -9.81f, 0);
            }

            if (obj == null)
                return;
            _physicsSpace.Add(obj);
        }

        public static void RemoveFromPhysicsEngine(ISpaceObject obj) {
            if (obj == null)
                return;
            _physicsSpace.Remove(obj);
        }

        public static void Raycast(Ray ray, float maximumLength, out RayCastResult result) {
            BEPUutilities.Ray tempRay = new BEPUutilities.Ray(ToBepuVector3(ray.Position), ToBepuVector3(ray.Direction));



            BEPUphysics.RayCastResult tempResult;

            _physicsSpace.RayCast(tempRay, maximumLength, out tempResult);

            RayHit hit = new RayHit() {
                Location = FromBepuVector3(tempResult.HitData.Location),
                Normal = FromBepuVector3(tempResult.HitData.Normal),
                T = tempResult.HitData.T
            };

            GameObject hitObject = GetOwner((ISpaceObject)tempResult.HitObject, CoreEngine.GetCoreEngine.Game.GetRootObject);

            result = new RayCastResult(hit, hitObject);
        }

        // not sure if this will work
        public static GameObject GetOwner(ISpaceObject obj, GameObject gameObject) {
            foreach (var gObj in gameObject.GetAllAttached()) {
                foreach (var gameComponent in gObj.GetComponents()) {
                    var collider = gameComponent as PhysicsComponent;

                    if (collider == null) continue;

                    var tempComponent = collider;
                    if (Equals(obj, tempComponent.PhysicsObject)) {
                        return gObj;
                    }
                }
            }
            return null;
        }

        static BEPUutilities.Vector3 ToBepuVector3(Vector3 vector3) {
            return new BEPUutilities.Vector3(vector3.X, vector3.Y, vector3.Z);
        }
        static Vector3 FromBepuVector3(BEPUutilities.Vector3 vector3) {
            return new Vector3(vector3.X, vector3.Y, vector3.Z);
        }

    }
}
