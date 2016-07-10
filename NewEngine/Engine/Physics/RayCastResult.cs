using NewEngine.Engine.Core;

namespace NewEngine.Engine.Physics {
    public class RayCastResult {
        public RayHit HitData;
        public GameObject HitObject;

        public RayCastResult(RayHit hitData, GameObject hitObject) {
            this.HitData = hitData;
            this.HitObject = hitObject;
        }
    }
}
