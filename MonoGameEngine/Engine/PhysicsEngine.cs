using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using BEPUphysics;
using BEPUutilities;
using BEPUutilities.Threading;
using MonoGameEngine.Engine.Physics;
using Ray = Microsoft.Xna.Framework.Ray;
using RayHit = MonoGameEngine.Engine.Utilites.Physics.RayHit;
using Vector3 = Microsoft.Xna.Framework.Vector3;

namespace MonoGameEngine.Engine {
    public class PhysicsEngine {
        private Space _bepuSpace;

        private static Space BepuSpace {
            get { return CoreEngine.instance.GetPhysicsEngine._bepuSpace; }
            set { CoreEngine.instance.GetPhysicsEngine._bepuSpace = value; }
        }


        public PhysicsEngine() {
            _bepuSpace = new Space();
            _bepuSpace.ForceUpdater.Gravity = new BEPUutilities.Vector3(0, -9.81f, 0);
        }

        public void Update(GameTime gameTime) {
            float step = (float)gameTime.ElapsedGameTime.TotalSeconds;
            BepuSpace.Update(step);

        }

        public static void AddPhysicsObject(ISpaceObject spaceObj) {
            BepuSpace.Add(spaceObj);
        }


        public static void RemovePhysicsObject(ISpaceObject spaceObj) {

            try {
            BepuSpace.Remove(spaceObj);
            }
            catch (Exception) {
            }
        }

        public static void Raycast(Ray ray, float maximumLength, out MonoGameEngine.Engine.Utilites.Physics.RayCastResult result) {
            BEPUutilities.Ray tempRay = new BEPUutilities.Ray(fromXnaVector3(ray.Position), fromXnaVector3(ray.Direction));


            RayCastResult tempResult;

            BepuSpace.RayCast(tempRay, maximumLength, out tempResult);

            RayHit hit = new RayHit() {
                Location = toXnaVector3(tempResult.HitData.Location),
                Normal = toXnaVector3(tempResult.HitData.Normal),
                T = tempResult.HitData.T
            };

            GameObject hitObject = null;
            // might be slow
            foreach (var gameObject in CoreEngine.instance.GameObjects) {
                foreach (var component in gameObject._components) {
                    var collider = component as Collider;
                    if (collider == null) continue;

                    var tempComponent = collider;
                    if (Equals(tempResult.HitObject, tempComponent.RigidBody)) {
                        hitObject = gameObject;
                    }

                }
            }


            result = new Utilites.Physics.RayCastResult(hit, hitObject);
        }

        private static BEPUutilities.Vector3 fromXnaVector3(Vector3 vector3) {
            return new BEPUutilities.Vector3(vector3.X, vector3.Y, vector3.Z);
        }


        private static Vector3 toXnaVector3(BEPUutilities.Vector3 vector3) {
            return new Vector3(vector3.X, vector3.Y, vector3.Z);
        }


        public void Reset() {
            BepuSpace = new Space(new ParallelLooper());
        }
    }
}
