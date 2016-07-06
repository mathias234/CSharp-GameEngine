namespace NewEngine.Engine.Core {
    public static class Fps {
        public static double Time;
        private static double _frames;
        private static int _fps;

        public static int GetFps(double time) {
            Time += time;
            if (Time < 1.0) {
                _frames++;
                return _fps;
            }
            else {
                _fps = (int)_frames;
                Time = 0.0;
                _frames = 0.0;
                return _fps;
            }
        }
    }
}