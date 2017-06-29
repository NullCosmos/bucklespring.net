using OpenTK.Audio.OpenAL;
using System;
using System.IO;

namespace bucklespring.net
{
    public class WavData : IDisposable
    {
        public Int32 Channels { get; private set; }
        public Int32 BitsPerSample { get; private set; }
        public Int32 SampleRate { get; private set; }
        public Byte[] SoundData { get; private set; }
        public ALFormat SoundFormat
        {
            get
            {
                switch (Channels)
                {
                    case 1: return BitsPerSample == 8 ? ALFormat.Mono8 : ALFormat.Mono16;
                    case 2: return BitsPerSample == 8 ? ALFormat.Stereo8 : ALFormat.Stereo16;
                    default: throw new NotSupportedException("The specified sound format is not supported.");
                }
            }
        }

        public WavData(String fileName)
        {
            Load(fileName);
        }

        private void Load(String fileName)
        {
            Stream stream = File.Open(fileName, FileMode.Open);
            if (stream == null)
                throw new ArgumentNullException("Invalid file");

            using (BinaryReader reader = new BinaryReader(stream))
            {
                // Parse RIFF header
                String signature = new String(reader.ReadChars(4));
                if (signature != "RIFF")
                    throw new NotSupportedException("Not a wav file");

                reader.ReadInt32(); // Riff Chunk Size

                String format = new String(reader.ReadChars(4));
                if (format != "WAVE")
                    throw new NotSupportedException("Not a wav file");

                // WAVE header
                String formatSignature = new String(reader.ReadChars(4));
                if (formatSignature != "fmt ")
                    throw new NotSupportedException("Wav format not supported");

                reader.ReadInt32(); // Format chunk size
                reader.ReadInt16(); // Audio format
                Channels = reader.ReadInt16();
                SampleRate = reader.ReadInt32();
                reader.ReadInt32(); // byte rate
                reader.ReadInt16(); // block align
                BitsPerSample = reader.ReadInt16();

                string dataSignature = new string(reader.ReadChars(4));
                if (dataSignature != "data")
                    throw new NotSupportedException("Wav has unexpected signature");

                reader.ReadInt32(); //Data chunk size
                
                SoundData = reader.ReadBytes((Int32)reader.BaseStream.Length);
            }
        }
        
        public void Dispose()
        {
            SoundData = null;
        }
    }
}
