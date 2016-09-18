using System.Linq;
using BEPUphysics;
using BEPUutilities;
using BEPUutilities.Threading;
using NewEngine.Engine.Core;
using NewEngine.Engine.Physics.PhysicsComponents;

namespace NewEngine.Engine.Physics {
    public static class PhysicsEngine {
        private static Space _physicsSpace;

        public static void Start() { }

        public static void Update(float deltaTime) {
            _physicsSpace?.Update(deltaTime);
        }

        public static void AddToPhysicsEngine(ISpaceObject obj) {
            if (_physicsSpace == null) {
                var parallelLooper = new ParallelLooper();

                for (var i = 0; i < System.Environment.ProcessorCount; i++) {
                    parallelLooper.AddThread();
                }

                _physicsSpace = new Space(parallelLooper) { ForceUpdater = { Gravity = new Vector3(0, -9.81f, 0) } };
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
            if(_physicsSpace == null) {
                result = null;
                return;
            }
            var tempRay = new BEPUutilities.Ray(ToBepuVector3(ray.Position), ToBepuVector3(ray.Direction));

            BEPUphysics.RayCastResult tempResult;

            _physicsSpace.RayCast(tempRay, maximumLength, out tempResult);

            var hit = new RayHit {
                Location = FromBepuVector3(tempResult.HitData.Location),
                Normal = FromBepuVector3(tempResult.HitData.Normal),
                T = tempResult.HitData.T
            };

            GameObject hitObject = null;// GetOwner((ISpaceObject)tempResult.HitObject, CoreEngine.GetCoreEngine.Game.GetRootObject);


            result = new RayCastResult(hit, hitObject);
        }

        // not sure if this will work
        public static GameObject GetOwner(ISpaceObject obj, GameObject gameObject) {
            return (from gObj in gameObject.GetAllAttached() from gameComponent in gObj.GetComponents() let collider = gameComponent as PhysicsComponent where collider != null let tempComponent = collider where Equals(obj, tempComponent.PhysicsObject) select gObj).FirstOrDefault();
        }

        private static Vector3 ToBepuVector3(OpenTK.Vector3 vector3) {
            return new Vector3(vector3.X, vector3.Y, vector3.Z);
        }

        private static OpenTK.Vector3 FromBepuVector3(Vector3 vector3) {
            return new OpenTK.Vector3(vector3.X, vector3.Y, vector3.Z);
        }
    }
}