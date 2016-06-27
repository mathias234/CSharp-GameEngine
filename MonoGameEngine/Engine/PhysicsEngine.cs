using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using BEPUphysics;
using BEPUutilities.Threading;

namespace MonoGameEngine.Engine {
    public class PhysicsEngine {
        private Space bepuSpace;

        public PhysicsEngine() {
            Debug.WriteLine("Physics Engine initialized");

   

            bepuSpace = new Space();
            bepuSpace.ForceUpdater.Gravity = new BEPUutilities.Vector3(0, -9.81f, 0);
        }

        public void Update(GameTime gameTime) {
            float step = (float)gameTime.ElapsedGameTime.TotalSeconds;
            bepuSpace.Update(step);

        }

        public static void AddPhysicsObject(ISpaceObject spaceObj) {
            CoreEngine.instance.GetPhysicsEngine.bepuSpace.Add(spaceObj);
        }

        public void Reset() {
            bepuSpace = new Space(new ParallelLooper());

        }
    }
}
