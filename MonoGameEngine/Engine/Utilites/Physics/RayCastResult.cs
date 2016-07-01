using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoGameEngine.Engine.Utilites.Physics {
    public class RayCastResult {
        public RayHit HitData;
        public GameObject HitObject;

        public RayCastResult(RayHit hitData, GameObject hitObject) {
            this.HitData = hitData;
            this.HitObject = hitObject;
        }
    }
}
