using NewEngine.Engine.Core;
using OpenTK;
using OpenTK.Input;

namespace NewEngine.Engine.components {
    public class FreeMove : GameComponent {
        public float Speed;

        public FreeMove(float speed = 0.3f) {
            Speed = speed;
        }

        public override void Update(float deltaTime) {
            Vector3 input = new Vector3(0);

            if (Input.GetKey(Key.W)) {
                input -= new Vector3(0, 0, 1);
            }
            if (Input.GetKey(Key.S))
                input += new Vector3(0, 0, 1);

            if (Input.GetKey(Key.A))
                input -= new Vector3(1, 0, 0);
            if (Input.GetKey(Key.D))
                input += new Vector3(1, 0, 0);

            Vector3 move = (Transform.Forward * input.Z + Transform.Right * input.X) * Speed;

            Transform.Position += move;
        }
    }
}
