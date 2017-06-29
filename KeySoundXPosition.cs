using System;

namespace bucklespring.net
{
    public class KeySoundXPosition
    {
        private static Byte[][] KeyLocations = new Byte[][]
        {
            new Byte[] { 0x1b, 0x70, 0x71, 0x72, 0x73, 0x74, 0x75, 0x76, 0x77, 0x78, 0x79, 0x7a, 0x7b, 0x2c, 0x91, 0x13 },
            new Byte[] { 0xc0, 0x31, 0x32, 0x33, 0x34, 0x35, 0x36, 0x37, 0x38, 0x39, 0x30, 0xbd, 0xbb, 0x8, 0x2d, 0x24, 0x21, 0x90, 0x6f, 0x6a, 0x6d, 0xa2 },
            new Byte[] { 0x9, 0x51, 0x57, 0x45, 0x52, 0x54, 0x59, 0x55, 0x49, 0x4f, 0x50, 0xdb, 0xdd, 0xdc, 0x2e, 0x23, 0x22, 0x67, 0x68, 0x69 },
            new Byte[] { 0x14, 0x41, 0x53, 0x44, 0x46, 0x47, 0x48, 0x4a, 0x4b, 0x4c, 0xba, 0xde, 0xd, 0x64, 0x65, 0x66, 0x6b },
            new Byte[] { 0xa0, 0x5a, 0x58, 0x43, 0x56, 0x42, 0x4e, 0x4d, 0xbc, 0xbe, 0xbf, 0xa1, 0x26, 0x61, 0x62, 0x63 },
            new Byte[] { 0xa2, 0x5b, 0x20, 0x5c, 0x5d, 0xa3, 0x25, 0x28, 0x27, 0x60, 0x6e, 0xd }
        };

        /// <summary>
        /// Gets the OpenAL x position of the key. (Range: -.75 - .25) We don't want full left or full right, as that is unatural sounding.
        /// </summary>
        /// <param name="code">Key code of the key</param>
        /// <returns>The position between -.75 and .25. Returns 0 if key not found in lookup table.</returns>
        public static float GetXPosition(Int32 code)
        {
            int row = 0;
            int col = 0;
            Boolean foundCode = false;

            for (row = 0; row < KeyLocations.Length; row++)
            {
                for (col = 0; col < KeyLocations[row].Length; col++)
                {
                    if (KeyLocations[row][col] == code)
                    {
                        foundCode = true;
                        break;
                    }
                }
                if (foundCode)
                    break;
            }
            if (foundCode)
            {
                //turns into 0.0-1.0
                float v = (float)col / KeyLocations[row].Length;

                //make it -.75 to .25
                //Should never be all left or all right channel...
                v = v * 0.5f - 0.25f;

                return v;
            }
            return 0;
        }
    }
}
