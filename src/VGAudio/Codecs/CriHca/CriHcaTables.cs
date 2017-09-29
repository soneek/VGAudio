﻿using System;

namespace VGAudio.Codecs.CriHca
{
    public static class CriHcaTables
    {
        public static float[] DequantizerScalingTable { get; } = GenerateTable(64, DequantizerScalingFunction);
        public static float[] DequantizerNormalizeTable { get; } = GenerateTable(16, DequantizerNormalizeFunction);
        public static float[] IntensityRatioTable { get; } = GenerateTable(16, IntensityRatioFunction);
        public static float[] ScaleConversionTable { get; } = GenerateTable(128, ScaleConversionTableFunction);

        public static byte[] ScaleToResolutionCurve { get; } =
        {
            0x0F, 0x0E, 0x0E, 0x0E, 0x0E, 0x0E, 0x0E, 0x0D,
            0x0D, 0x0D, 0x0D, 0x0D, 0x0D, 0x0C, 0x0C, 0x0C,
            0x0C, 0x0C, 0x0C, 0x0B, 0x0B, 0x0B, 0x0B, 0x0B,
            0x0B, 0x0A, 0x0A, 0x0A, 0x0A, 0x0A, 0x0A, 0x0A,
            0x09, 0x09, 0x09, 0x09, 0x09, 0x09, 0x08, 0x08,
            0x08, 0x08, 0x08, 0x08, 0x07, 0x06, 0x06, 0x05,
            0x04, 0x04, 0x04, 0x03, 0x03, 0x03, 0x02, 0x02,
            0x02, 0x02, 0x01
        };

        public static byte[] QuantizedSpectrumMaxBits { get; } =
        {
            0, 2, 3, 3, 4, 4, 4, 4, 5, 6, 7, 8, 9, 10, 11, 12
        };

        public static byte[,] QuantizedSpectrumBits { get; } =
        {
            {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            {1, 1, 2, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            {2, 2, 2, 2, 2, 2, 3, 3, 0, 0, 0, 0, 0, 0, 0, 0},
            {2, 2, 3, 3, 3, 3, 3, 3, 0, 0, 0, 0, 0, 0, 0, 0},
            {3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 4, 4},
            {3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 4, 4, 4, 4, 4, 4},
            {3, 3, 3, 3, 3, 3, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4},
            {3, 3, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4}
        };

        public static sbyte[,] QuantizedSpectrumValue { get; } =
        {
            {+0, +0, +0, +0, +0, +0, +0, +0, +0, +0, +0, +0, +0, +0, +0, +0},
            {+0, +0, +1, -1, +0, +0, +0, +0, +0, +0, +0, +0, +0, +0, +0, +0},
            {+0, +0, +1, +1, -1, -1, +2, -2, +0, +0, +0, +0, +0, +0, +0, +0},
            {+0, +0, +1, -1, +2, -2, +3, -3, +0, +0, +0, +0, +0, +0, +0, +0},
            {+0, +0, +1, +1, -1, -1, +2, +2, -2, -2, +3, +3, -3, -3, +4, -4},
            {+0, +0, +1, +1, -1, -1, +2, +2, -2, -2, +3, -3, +4, -4, +5, -5},
            {+0, +0, +1, +1, -1, -1, +2, -2, +3, -3, +4, -4, +5, -5, +6, -6},
            {+0, +0, +1, -1, +2, -2, +3, -3, +4, -4, +5, -5, +6, -6, +7, -7}
        };

        // Don't know what the window function is.
        // It's close to a KBD window with an alpha of around 3.82.
        // AAC and Vorbis windows are similar to it.
        //Todo: Make float
        public static double[] MdctWindow { get; } =
        {
            6.90533780e-4f, 1.97623484e-3f, 3.67386453e-3f, 5.72424009e-3f, 8.09670333e-3f, 1.07731819e-2f, 1.37425177e-2f, 1.69978570e-2f,
            2.05352642e-2f, 2.43529025e-2f, 2.84505188e-2f, 3.28290947e-2f, 3.74906212e-2f, 4.24378961e-2f, 4.76744287e-2f, 5.32043017e-2f,
            5.90321124e-2f, 6.51628822e-2f, 7.16020092e-2f, 7.83552229e-2f, 8.54284912e-2f, 9.28280205e-2f, 1.00560151e-1f, 1.08631350e-1f,
            1.17048122e-1f, 1.25816986e-1f, 1.34944350e-1f, 1.44436508e-1f, 1.54299513e-1f, 1.64539129e-1f, 1.75160721e-1f, 1.86169162e-1f,
            1.97568730e-1f, 2.09362969e-1f, 2.21554622e-1f, 2.34145418e-1f, 2.47135997e-1f, 2.60525763e-1f, 2.74312705e-1f, 2.88493186e-1f,
            3.03061932e-1f, 3.18011731e-1f, 3.33333343e-1f, 3.49015296e-1f, 3.65043819e-1f, 3.81402701e-1f, 3.98073107e-1f, 4.15033519e-1f,
            4.32259798e-1f, 4.49725032e-1f, 4.67399567e-1f, 4.85251158e-1f, 5.03244936e-1f, 5.21343827e-1f, 5.39508522e-1f, 5.57697773e-1f,
            5.75868905e-1f, 5.93978047e-1f, 6.11980557e-1f, 6.29831433e-1f, 6.47486031e-1f, 6.64900243e-1f, 6.82031155e-1f, 6.98837578e-1f,
            7.15280414e-1f, 7.31323123e-1f, 7.46932149e-1f, 7.62077332e-1f, 7.76731849e-1f, 7.90872812e-1f, 8.04481268e-1f, 8.17542017e-1f,
            8.30044091e-1f, 8.41980159e-1f, 8.53346705e-1f, 8.64143789e-1f, 8.74374807e-1f, 8.84046197e-1f, 8.93167078e-1f, 9.01749134e-1f,
            9.09806132e-1f, 9.17353690e-1f, 9.24408972e-1f, 9.30990338e-1f, 9.37117040e-1f, 9.42809045e-1f, 9.48086798e-1f, 9.52970862e-1f,
            9.57481921e-1f, 9.61640537e-1f, 9.65466917e-1f, 9.68980789e-1f, 9.72201586e-1f, 9.75147963e-1f, 9.77837980e-1f, 9.80289042e-1f,
            9.82517719e-1f, 9.84539866e-1f, 9.86370564e-1f, 9.88024116e-1f, 9.89514053e-1f, 9.90853190e-1f, 9.92053449e-1f, 9.93126273e-1f,
            9.94082093e-1f, 9.94930983e-1f, 9.95682180e-1f, 9.96344328e-1f, 9.96925533e-1f, 9.97433305e-1f, 9.97874618e-1f, 9.98256087e-1f,
            9.98583674e-1f, 9.98862922e-1f, 9.99099135e-1f, 9.99296963e-1f, 9.99460995e-1f, 9.99595225e-1f, 9.99703407e-1f, 9.99789119e-1f,
            9.99855518e-1f, 9.99905586e-1f, 9.99941945e-1f, 9.99967217e-1f, 9.99983609e-1f, 9.99993265e-1f, 9.99998033e-1f, 9.99999762e-1f
        };

        /// <summary>
        /// Represents an Absolute Threshold of Hearing (ATH) curve. 
        /// This curve is used when deriving resolutions from scale factors in very old HCA versions.
        /// </summary>
        /// <seealso cref="CriHcaFrame.ScaleAthCurve"/>
        /// <remarks>This curve seems to be a slight modification of the standard Painter & Spanias ATH curve formula</remarks>
        public static byte[] AthCurve { get; } =
        {
            0x78, 0x5F, 0x56, 0x51, 0x4E, 0x4C, 0x4B, 0x49, 0x48, 0x48, 0x47, 0x46, 0x46, 0x45, 0x45, 0x45,
            0x44, 0x44, 0x44, 0x44, 0x43, 0x43, 0x43, 0x43, 0x43, 0x43, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42,
            0x42, 0x42, 0x41, 0x41, 0x41, 0x41, 0x41, 0x41, 0x41, 0x41, 0x41, 0x41, 0x40, 0x40, 0x40, 0x40,
            0x40, 0x40, 0x40, 0x40, 0x40, 0x3F, 0x3F, 0x3F, 0x3F, 0x3F, 0x3F, 0x3F, 0x3F, 0x3F, 0x3F, 0x3F,
            0x3F, 0x3F, 0x3F, 0x3E, 0x3E, 0x3E, 0x3E, 0x3E, 0x3E, 0x3D, 0x3D, 0x3D, 0x3D, 0x3D, 0x3D, 0x3D,
            0x3C, 0x3C, 0x3C, 0x3C, 0x3C, 0x3C, 0x3C, 0x3C, 0x3B, 0x3B, 0x3B, 0x3B, 0x3B, 0x3B, 0x3B, 0x3B,
            0x3B, 0x3B, 0x3B, 0x3B, 0x3B, 0x3B, 0x3B, 0x3B, 0x3B, 0x3B, 0x3B, 0x3B, 0x3B, 0x3B, 0x3B, 0x3B,
            0x3B, 0x3B, 0x3B, 0x3B, 0x3B, 0x3B, 0x3B, 0x3B, 0x3C, 0x3C, 0x3C, 0x3C, 0x3C, 0x3C, 0x3C, 0x3C,
            0x3D, 0x3D, 0x3D, 0x3D, 0x3D, 0x3D, 0x3D, 0x3D, 0x3E, 0x3E, 0x3E, 0x3E, 0x3E, 0x3E, 0x3E, 0x3F,
            0x3F, 0x3F, 0x3F, 0x3F, 0x3F, 0x3F, 0x3F, 0x3F, 0x3F, 0x3F, 0x3F, 0x3F, 0x3F, 0x3F, 0x3F, 0x3F,
            0x3F, 0x3F, 0x3F, 0x3F, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40,
            0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x41, 0x41, 0x41, 0x41, 0x41, 0x41, 0x41,
            0x41, 0x41, 0x41, 0x41, 0x41, 0x41, 0x41, 0x41, 0x41, 0x41, 0x41, 0x41, 0x41, 0x41, 0x41, 0x41,
            0x41, 0x41, 0x41, 0x41, 0x41, 0x41, 0x41, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42,
            0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x43, 0x43, 0x43,
            0x43, 0x43, 0x43, 0x43, 0x43, 0x43, 0x43, 0x43, 0x43, 0x43, 0x43, 0x43, 0x43, 0x43, 0x44, 0x44,
            0x44, 0x44, 0x44, 0x44, 0x44, 0x44, 0x44, 0x44, 0x44, 0x44, 0x44, 0x44, 0x45, 0x45, 0x45, 0x45,
            0x45, 0x45, 0x45, 0x45, 0x45, 0x45, 0x45, 0x45, 0x46, 0x46, 0x46, 0x46, 0x46, 0x46, 0x46, 0x46,
            0x46, 0x46, 0x47, 0x47, 0x47, 0x47, 0x47, 0x47, 0x47, 0x47, 0x47, 0x47, 0x48, 0x48, 0x48, 0x48,
            0x48, 0x48, 0x48, 0x48, 0x49, 0x49, 0x49, 0x49, 0x49, 0x49, 0x49, 0x49, 0x4A, 0x4A, 0x4A, 0x4A,
            0x4A, 0x4A, 0x4A, 0x4A, 0x4B, 0x4B, 0x4B, 0x4B, 0x4B, 0x4B, 0x4B, 0x4C, 0x4C, 0x4C, 0x4C, 0x4C,
            0x4C, 0x4D, 0x4D, 0x4D, 0x4D, 0x4D, 0x4D, 0x4E, 0x4E, 0x4E, 0x4E, 0x4E, 0x4E, 0x4F, 0x4F, 0x4F,
            0x4F, 0x4F, 0x4F, 0x50, 0x50, 0x50, 0x50, 0x50, 0x51, 0x51, 0x51, 0x51, 0x51, 0x52, 0x52, 0x52,
            0x52, 0x52, 0x53, 0x53, 0x53, 0x53, 0x54, 0x54, 0x54, 0x54, 0x54, 0x55, 0x55, 0x55, 0x55, 0x56,
            0x56, 0x56, 0x56, 0x57, 0x57, 0x57, 0x57, 0x57, 0x58, 0x58, 0x58, 0x59, 0x59, 0x59, 0x59, 0x5A,
            0x5A, 0x5A, 0x5A, 0x5B, 0x5B, 0x5B, 0x5B, 0x5C, 0x5C, 0x5C, 0x5D, 0x5D, 0x5D, 0x5D, 0x5E, 0x5E,
            0x5E, 0x5F, 0x5F, 0x5F, 0x60, 0x60, 0x60, 0x61, 0x61, 0x61, 0x61, 0x62, 0x62, 0x62, 0x63, 0x63,
            0x63, 0x64, 0x64, 0x64, 0x65, 0x65, 0x66, 0x66, 0x66, 0x67, 0x67, 0x67, 0x68, 0x68, 0x68, 0x69,
            0x69, 0x6A, 0x6A, 0x6A, 0x6B, 0x6B, 0x6B, 0x6C, 0x6C, 0x6D, 0x6D, 0x6D, 0x6E, 0x6E, 0x6F, 0x6F,
            0x70, 0x70, 0x70, 0x71, 0x71, 0x72, 0x72, 0x73, 0x73, 0x73, 0x74, 0x74, 0x75, 0x75, 0x76, 0x76,
            0x77, 0x77, 0x78, 0x78, 0x78, 0x79, 0x79, 0x7A, 0x7A, 0x7B, 0x7B, 0x7C, 0x7C, 0x7D, 0x7D, 0x7E,
            0x7E, 0x7F, 0x7F, 0x80, 0x80, 0x81, 0x81, 0x82, 0x83, 0x83, 0x84, 0x84, 0x85, 0x85, 0x86, 0x86,
            0x87, 0x88, 0x88, 0x89, 0x89, 0x8A, 0x8A, 0x8B, 0x8C, 0x8C, 0x8D, 0x8D, 0x8E, 0x8F, 0x8F, 0x90,
            0x90, 0x91, 0x92, 0x92, 0x93, 0x94, 0x94, 0x95, 0x95, 0x96, 0x97, 0x97, 0x98, 0x99, 0x99, 0x9A,
            0x9B, 0x9B, 0x9C, 0x9D, 0x9D, 0x9E, 0x9F, 0xA0, 0xA0, 0xA1, 0xA2, 0xA2, 0xA3, 0xA4, 0xA5, 0xA5,
            0xA6, 0xA7, 0xA7, 0xA8, 0xA9, 0xAA, 0xAA, 0xAB, 0xAC, 0xAD, 0xAE, 0xAE, 0xAF, 0xB0, 0xB1, 0xB1,
            0xB2, 0xB3, 0xB4, 0xB5, 0xB6, 0xB6, 0xB7, 0xB8, 0xB9, 0xBA, 0xBA, 0xBB, 0xBC, 0xBD, 0xBE, 0xBF,
            0xC0, 0xC1, 0xC1, 0xC2, 0xC3, 0xC4, 0xC5, 0xC6, 0xC7, 0xC8, 0xC9, 0xC9, 0xCA, 0xCB, 0xCC, 0xCD,
            0xCE, 0xCF, 0xD0, 0xD1, 0xD2, 0xD3, 0xD4, 0xD5, 0xD6, 0xD7, 0xD8, 0xD9, 0xDA, 0xDB, 0xDC, 0xDD,
            0xDE, 0xDF, 0xE0, 0xE1, 0xE2, 0xE3, 0xE4, 0xE5, 0xE6, 0xE7, 0xE8, 0xE9, 0xEA, 0xEB, 0xED, 0xEE,
            0xEF, 0xF0, 0xF1, 0xF2, 0xF3, 0xF4, 0xF5, 0xF7, 0xF8, 0xF9, 0xFA, 0xFB, 0xFC, 0xFD
        };

        private static float DequantizerScalingFunction(int x) => (float)(Math.Sqrt(128) * Math.Pow(Math.Pow(2, 53f / 128), x - 63));
        private static float ScaleConversionTableFunction(int x) => x > 1 && x < 127 ? (float)Math.Pow(Math.Pow(2, 53f / 128), x - 64) : 0;
        private static float IntensityRatioFunction(int x) => x <= 14 ? (14 - x) / 7f : 0;

        private static float DequantizerNormalizeFunction(int x)
        {
            if (x == 0) return 0;
            if (x < 8) return 2f / (2 * x + 1);
            return 2f / ((1 << (x - 3)) - 1);
        }

        private static T[] GenerateTable<T>(int count, Func<int, T> elementGenerator)
        {
            var table = new T[count];
            for (int i = 0; i < count; i++)
            {
                table[i] = elementGenerator(i);
            }
            return table;
        }
    }
}
