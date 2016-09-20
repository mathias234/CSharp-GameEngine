using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Audio.OpenAL;
using OpenTK.Audio;
using NewEngine.Engine.Core;
using OpenTK;

namespace NewEngine.Engine.Audio {
    public class AudioMaster {
        private static AudioContext _context;

        public static void Initialize() {
            try {
                _context = new AudioContext();
            }
            catch (Exception e) {
                LogManager.Debug(e.Message);
            }
        }

        public static void SetListener(float x, float y, float z) {
            AL.Listener(ALListener3f.Position, x, y, z);
            AL.Listener(ALListener3f.Velocity, 0, 0, 0);
        }

        public static Vector3 GetAudioListnerPosition() {
            Vector3 pos;
            AL.GetListener(ALListener3f.Position, out pos);
            return pos;
        }
    }
}
