using OpenTK;
using OpenTK.Input;

namespace NewEngine.Engine.Core {
    public static class Input {
        public const int NumKeycodes = 132;
        public const int NumMouseButtons = 12;

        private static bool[] _lastKeys = new bool[NumKeycodes];
        private static bool[] _lastMouse = new bool[NumMouseButtons];

        private static MouseDevice _mouseDevice;

        private static bool _lockMouse;
        private static Vector2 _lockMousePosition;

        public static void Update(MouseDevice mouse) {
            _mouseDevice = mouse;
            for (var i = 0; i < NumKeycodes; i++) {
                _lastKeys[i] = GetKey((Key)i);
            }
            for (var i = 0; i < NumMouseButtons; i++) {
                _lastMouse[i] = GetMouse((MouseButton)i);
            }

            if (_lockMouse) {
                SetMousePosition(_lockMousePosition);
            }
            else {
                _lockMousePosition = GetMousePosition();
            }
        }

        public static bool GetKey(Key keyCode) {
            if (CoreEngine.GetCoreEngine.Focused == false)
                return false;

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
            Vector2 mousePos = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
            return mousePos;
        }

        public static void LockMouse() {
            _lockMouse = !_lockMouse;
        }

        public static Vector2 GetWindowMousePosition() {
            return _mouseDevice == null ? new Vector2(0, 0) : new Vector2(_mouseDevice.X, _mouseDevice.Y);
        }

        public static void SetMousePosition(Vector2 pos) {
            Mouse.SetPosition(pos.X, pos.Y);
        }

        // TODO: needs a custom class
        public static float ClampFloat(float val, float min, float max) {
            if (val.CompareTo(min) < 0) return min;
            else if (val.CompareTo(max) > 0) return max;
            else return val;
        }
    }
}
