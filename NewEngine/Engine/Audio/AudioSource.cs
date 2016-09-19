using NewEngine.Engine.components;
using NewEngine.Engine.Core;
using OpenTK;
using OpenTK.Audio.OpenAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewEngine.Engine.Audio {
    public class AudioSource : GameComponent {
        private int _sourceId;
        private bool _is3D;

        public AudioSource() {
            AL.GenSources(1, out _sourceId);
            AL.Source(_sourceId, ALSourcef.Gain, 1);
            AL.Source(_sourceId, ALSourcef.Pitch, 1);
            AL.Source(_sourceId, ALSource3f.Position, 0, 0, 0);
        }

        public void Play(string filename) {
            Stop();

            int buffer;
            AL.GenBuffers(1, out buffer);
            WaveData waveFile = new WaveData(filename);
            AL.BufferData(buffer, waveFile.SoundFormat, waveFile.SoundData, waveFile.SoundData.Length, waveFile.SampleRate);
            waveFile.dispose();

            AL.Source(_sourceId, ALSourcei.Buffer, buffer);
            Continue();
        }

        public void Delete() {
            AL.DeleteSources(1, ref _sourceId);
        }

        public void SetVolume(float volume) {
            AL.Source(_sourceId, ALSourcef.Gain, volume);
        }

        public void SetPitch(float pitch) {
            AL.Source(_sourceId, ALSourcef.Pitch, pitch);
        }

        public void SetLooping(bool loop) {
            AL.Source(_sourceId, ALSourceb.Looping, loop);
        }

        public void Pause() {
            AL.SourcePause(_sourceId);
        }

        public void Continue() {
            AL.SourcePlay(_sourceId);
        }

        public void Stop() {
            AL.SourceStop(_sourceId);
        }

        public bool IsPlaying() {
            int isPlaying;
            AL.GetSource(_sourceId, ALGetSourcei.SourceState, out isPlaying);
    
            if (isPlaying == (int)ALSourceState.Playing)
                return true;
            else
                return false;
        }

        public void Is3D(bool value) {
            _is3D = value;
        }

        public override void Update(float deltaTime) {
            if (_is3D)
                AL.Source(_sourceId, ALSource3f.Position, Transform.GetTransformedPosition().X, Transform.GetTransformedPosition().Y, Transform.GetTransformedPosition().Z);
            else {
                var position = AudioMaster.GetAudioListnerPosition();
                AL.Source(_sourceId, ALSource3f.Position, position.X, position.Y, position.Z);
            }
        }
    }
}
