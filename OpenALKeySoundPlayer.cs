using OpenTK.Audio.OpenAL;
using System;
using System.IO;

namespace bucklespring.net
{
    public class OpenALKeySoundPlayer : IDisposable
    {
        private const Int32 MAX_NUM_SAMPLES = 3;
        private const String AudioPath = @".\wav";
        private const Int32 StereoWidth = 50;
        private const Int32 Gain = 100;
        private static float[] ListenerOrientation = { 0.0f, 0.0f, 1.0f, 0.0f, 1.0f, 0.0f };

        private Int32 alBuffer = -1;
        private Int32 alSource = -1;
        private IntPtr Device = IntPtr.Zero;
        private OpenTK.ContextHandle context = OpenTK.ContextHandle.Zero;

        public OpenALKeySoundPlayer()
        {
            String defaultDevice = Alc.GetString(IntPtr.Zero, AlcGetString.DefaultDeviceSpecifier);
            Device = Alc.OpenDevice(defaultDevice);
            if (Device == IntPtr.Zero)
            {
                Console.WriteLine("Failed to open default sound device");
                return;
            }

            context = Alc.CreateContext(Device, new int[] { });
            if (!Alc.MakeContextCurrent(context))
            {
                Console.WriteLine("failed to make default context");
                return;
            }

            AL.Listener(ALListener3f.Position, 0, 0, 0);
            AL.Listener(ALListener3f.Velocity, 0, 0, 0);
            AL.Listener(ALListenerfv.Orientation, ref ListenerOrientation);
        }

        private String GetValidAudioSourceFileName(Int32 code, Boolean isDown, Int32 randomSubSoundIndex)
        {
            String fileName = Path.Combine(AudioPath, String.Format("{0:x}-{1}-{2}.wav", code, isDown ? 1 : 0, randomSubSoundIndex));
            if (!File.Exists(fileName))
            {
                randomSubSoundIndex = 0;
                fileName = Path.Combine(AudioPath, String.Format("{0:x}-{1}-{2}.wav", code, isDown ? 1 : 0, randomSubSoundIndex));
            }
            if (!File.Exists(fileName))
            {
                code = 0x31;
                fileName = Path.Combine(AudioPath, String.Format("{0:x}-{1}-{2}.wav", code, isDown ? 1 : 0, randomSubSoundIndex));
            }

            if (File.Exists(fileName))
                return fileName;
            return null;
        }

        public void PlayKey(Int32 code, Boolean isDown)
        {
            float x = KeySoundXPosition.GetXPosition(code);

            Int32 randomSubSoundIndex = new Random().Next(0, MAX_NUM_SAMPLES);

            String fileName = GetValidAudioSourceFileName(code, isDown, randomSubSoundIndex);
            if(String.IsNullOrWhiteSpace(fileName))
            {
                Console.WriteLine("Failed to find sound to play");
                return;
            }

            CleanupSource();
            GenerateNewSource();
            SetAudioPositionAndGain(x);
            BufferAudioIntoOpenAL(fileName);
            
            AL.SourcePlay(alSource);
        }

        private void SetAudioPositionAndGain(float x)
        {
            AL.Source(alSource, ALSource3f.Position, -x, 0f, (100 - StereoWidth) / 100.0f);
            AL.Source(alSource, ALSourcef.Gain, Gain / 100.0f);
        }

        private void BufferAudioIntoOpenAL(String fileName)
        {
            using (WavData wavFileData = new WavData(fileName))
            {
                AL.BufferData(alBuffer, wavFileData.SoundFormat, wavFileData.SoundData, wavFileData.SoundData.Length, wavFileData.SampleRate);
            }
            AL.Source(alSource, ALSourcei.Buffer, alBuffer);
        }

        private void GenerateNewSource()
        {
            AL.GenSources(1, out alSource);
            AL.GenBuffers(1, out alBuffer);
        }

        private void CleanupSource()
        {
            if (alSource > -1)
            {
                AL.DeleteBuffer(alBuffer);
                AL.DeleteSource(alSource);
            }
        }

        public void Dispose()
        {
            Device = Alc.GetContextsDevice(context);
            Alc.MakeContextCurrent(OpenTK.ContextHandle.Zero);
            if (context != OpenTK.ContextHandle.Zero)
                Alc.DestroyContext(context);
            if (Device != IntPtr.Zero)
                Alc.CloseDevice(Device);
        }
    }
}
