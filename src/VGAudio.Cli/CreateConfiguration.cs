﻿using System.IO;
using VGAudio.Containers;
using VGAudio.Containers.Bxstm;
using VGAudio.Containers.Dsp;
using VGAudio.Containers.Hps;
using VGAudio.Containers.Idsp;
using VGAudio.Containers.Wave;

namespace VGAudio.Cli
{
    internal static class CreateConfiguration
    {
        public static IConfiguration Wave(Options options, IConfiguration inConfig = null)
        {
            var config = inConfig as WaveConfiguration ?? new WaveConfiguration();

            switch (options.OutFormat)
            {
                case AudioFormat.GcAdpcm:
                    throw new InvalidDataException("Can't use format GcAdpcm with Wave files");
                case AudioFormat.Pcm16:
                    config.Codec = WaveCodec.Pcm16Bit;
                    break;
                case AudioFormat.Pcm8:
                    config.Codec = WaveCodec.Pcm8Bit;
                    break;
            }

            return config;
        }

        public static IConfiguration Dsp(Options options, IConfiguration inConfig = null)
        {
            var config = inConfig as DspConfiguration ?? new DspConfiguration();

            switch (options.OutFormat)
            {
                case AudioFormat.Pcm16:
                    throw new InvalidDataException("Can't use format PCM16 with DSP files");
                case AudioFormat.Pcm8:
                    throw new InvalidDataException("Can't use format PCM8 with DSP files");
            }

            return config;
        }

        public static IConfiguration Idsp(Options options, IConfiguration inConfig = null)
        {
            var config = inConfig as IdspConfiguration ?? new IdspConfiguration();

            switch (options.OutFormat)
            {
                case AudioFormat.Pcm16:
                    throw new InvalidDataException("Can't use format PCM16 with IDSP files");
                case AudioFormat.Pcm8:
                    throw new InvalidDataException("Can't use format PCM8 with IDSP files");
            }

            return config;
        }

        public static IConfiguration Brstm(Options options, IConfiguration inConfig = null)
        {
            var config = inConfig as BrstmConfiguration ?? new BrstmConfiguration();

            switch (options.OutFormat)
            {
                case AudioFormat.GcAdpcm:
                    config.Codec = BxstmCodec.Adpcm;
                    break;
                case AudioFormat.Pcm16:
                    config.Codec = BxstmCodec.Pcm16Bit;
                    break;
                case AudioFormat.Pcm8:
                    config.Codec = BxstmCodec.Pcm8Bit;
                    break;
            }

            return config;
        }

        public static IConfiguration Bcstm(Options options, IConfiguration inConfig = null)
        {
            var config = inConfig as BcstmConfiguration ?? new BcstmConfiguration();

            switch (options.OutFormat)
            {
                case AudioFormat.GcAdpcm:
                    config.Codec = BxstmCodec.Adpcm;
                    break;
                case AudioFormat.Pcm16:
                    config.Codec = BxstmCodec.Pcm16Bit;
                    break;
                case AudioFormat.Pcm8:
                    config.Codec = BxstmCodec.Pcm8Bit;
                    break;
            }

            return config;
        }

        public static IConfiguration Bfstm(Options options, IConfiguration inConfig = null)
        {
            var config = inConfig as BfstmConfiguration ?? new BfstmConfiguration();

            switch (options.OutFormat)
            {
                case AudioFormat.GcAdpcm:
                    config.Codec = BxstmCodec.Adpcm;
                    break;
                case AudioFormat.Pcm16:
                    config.Codec = BxstmCodec.Pcm16Bit;
                    break;
                case AudioFormat.Pcm8:
                    config.Codec = BxstmCodec.Pcm8Bit;
                    break;
            }

            return config;
        }

        public static IConfiguration Hps(Options options, IConfiguration inConfig = null)
        {
            var config = inConfig as HpsConfiguration ?? new HpsConfiguration();

            switch (options.OutFormat)
            {
                case AudioFormat.Pcm16:
                    throw new InvalidDataException("Can't use format PCM16 with HPS files");
                case AudioFormat.Pcm8:
                    throw new InvalidDataException("Can't use format PCM8 with HPS files");
            }

            return config;
        }
    }
}
