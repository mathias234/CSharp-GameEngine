using OpenTK;
using OpenTK.Input;

namespace NewEngine.Engine.Core {
    public static class Input {
        public const int NumKeycodes = 132;
        public const int NumMouseButtons = 12;

        private static bool[] _lastKeys = new bool[NumKeycodes];
        private static bool[] _lastMouse = new bool[NumMouseButtons];

        private static MouseDevice _mouseDevice;

        public static void Update(MouseDevice mouse) {
            _mouseDevice = mouse;
            for (var i = 0; i < NumKeycodes; i++) {
                _lastKeys[i] = GetKey((Key)i);
            }
            for (var i = 0; i < NumMouseButtons; i++) {
                _lastMouse[i] = GetMouse((MouseButton)i);
            }
        }

        public static bool GetKey(Key keyCode) {
            return Keyboard.GetState().IsKeyDown(keyCode);
        }

        public static bool GetKeyDown(Key keyCode) {
            return GetKey(keyCode) && !_lastKeys[(int)keyCode];
        }

        public static bool GetKeyUp(Key keyCode) {
            return !GetKey(keyCode) && _lastKeys[(int)keyCode];
        }

        public static bool GetMouse(MouseButton mouseButton) {
            return Mouse.GetState().IsButtonDown(mouseButton);
        }

        public static bool GetMouseDown(MouseButton mouseButton) {
            return GetMouse(mouseButton) && !_lastMouse[(int)mouseButton];
        }

        public static bool GetMouseUp(MouseButton mouseButton) {
            return !GetMouse(mouseButton) && _lastMouse[(int)mouseButton];
        }

        public static Vector2 GetMousePosition() {
            return new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
        }

        public static Vector2 GetWindowMousePosition() {
            if (_mouseDevice == null)
                return new Vector2(0,0);
            return new Vector2(_mouseDevice.X, _mouseDevice.Y);
        }

        public static void SetMousePosition(Vector2 pos) {
            Mouse.SetPosition(pos.X, pos.Y);
        }
    }
}
