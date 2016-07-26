using BEPUphysics.Entities.Prefabs;

namespace NewEngine.Engine.Physics.PhysicsComponents {
    public class BoxCollider : PhysicsComponent {
        private float _height;
        private float _length;

        private float _mass;
        private float _width;

        public BoxCollider(float width, float height, float length, float mass) {
            _width = width;
            _height = height;
            _length = length;
            _mass = mass;
        }

        public override void OnEnable() {
            PhysicsObject = new Box(ToBepuVector3(Parent.Transform.Position), _width, _height, _length, _mass);

            PhysicsEngine.AddToPhysicsEngine(PhysicsObject);
        }

        public override void Update(float deltaTime) {
            var obj = (Box) PhysicsObject;
            if (obj == null) return;
            Parent.Transform.Position = FromBepuVector3(obj.Position);
            Parent.Transform.Rotation = FromBepuQuaternion(obj.Orientation);
        }
    }
}