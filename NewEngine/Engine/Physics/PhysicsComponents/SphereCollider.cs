using BEPUphysics.Entities.Prefabs;

namespace NewEngine.Engine.Physics.PhysicsComponents {
    public class SphereCollider : PhysicsComponent {
        private float _mass;
        private float _radius;

        public SphereCollider(float radius, float mass) {
            _radius = radius;
            _mass = mass;
        }

        public override void OnEnable() {
            PhysicsObject = new Sphere(ToBepuVector3(gameObject.Transform.Position), _radius, _mass);

            PhysicsEngine.AddToPhysicsEngine(PhysicsObject);
        }

        public override void Update(float deltaTime) {
            var obj = (Sphere) PhysicsObject;
            if (obj == null) return;
            gameObject.Transform.Position = FromBepuVector3(obj.Position);
            gameObject.Transform.Rotation = FromBepuQuaternion(obj.Orientation);
        }
    }
}