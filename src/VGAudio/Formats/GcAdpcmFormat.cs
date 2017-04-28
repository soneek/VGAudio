﻿using System;
using System.Collections.Generic;
using System.Linq;
using VGAudio.Codecs;
using VGAudio.Formats.GcAdpcm;
using VGAudio.Utilities;

namespace VGAudio.Formats
{
    /// <summary>
    /// A 4-bit Nintendo ADPCM audio stream.
    /// The stream can contain any number of individual channels.
    /// </summary>
    public class GcAdpcmFormat : AudioFormatBase<GcAdpcmFormat, GcAdpcmFormatBuilder>
    {
        public GcAdpcmChannel[] Channels { get; }
        public List<GcAdpcmTrack> Tracks { get; }

        public int AlignmentMultiple { get; }
        private int AlignmentSamples => Helpers.GetNextMultiple(base.LoopStart, AlignmentMultiple) - base.LoopStart;
        public new int LoopStart => base.LoopStart + AlignmentSamples;
        public new int LoopEnd => base.LoopEnd + AlignmentSamples;
        public new int SampleCount => AlignmentSamples == 0 ? base.SampleCount : LoopEnd;

        public int UnalignedLoopStart => base.LoopStart;
        public int UnalignedLoopEnd => base.LoopEnd;
        public int UnalignedSampleCount =>  base.SampleCount;

        public GcAdpcmFormat() : base(0, 0, 0) => Channels = new GcAdpcmChannel[0];
        public GcAdpcmFormat(int sampleCount, int sampleRate, GcAdpcmChannel[] channels)
            : base(sampleCount, sampleRate, channels.Length)
        {
            Channels = channels;
            Tracks = GetDefaultTrackList(Channels.Length).ToList();
        }

        internal GcAdpcmFormat(GcAdpcmFormatBuilder b) : base(b)
        {
            Channels = b.Channels;
            Tracks = b.Tracks == null || b.Tracks.Count == 0 ? GetDefaultTrackList(b.Channels.Length).ToList() : b.Tracks;
            AlignmentMultiple = b.AlignmentMultiple;

            Parallel.For(0, Channels.Length, i =>
            {
                Channels[i] = Channels[i]
                    .GetCloneBuilder()
                    .WithLoop(Looping, UnalignedLoopStart, UnalignedLoopEnd)
                    .WithLoopAlignment(b.AlignmentMultiple)
                    .Build();
            });
        }
        
        public override Pcm16Format ToPcm16()
        {
            var pcmChannels = new short[Channels.Length][];
            Parallel.For(0, Channels.Length, i =>
            {
                pcmChannels[i] = Channels[i].GetPcmAudio();
            });

            return new Pcm16Format(SampleCount, SampleRate, pcmChannels)
                .WithLoop(Looping, LoopStart, LoopEnd);
        }

        public override GcAdpcmFormat EncodeFromPcm16(Pcm16Format pcm16)
        {
            var channels = new GcAdpcmChannel[pcm16.ChannelCount];

            Parallel.For(0, pcm16.ChannelCount, i =>
            {
                channels[i] = EncodeChannel(pcm16.SampleCount, pcm16.Channels[i]);
            });

            return new GcAdpcmFormatBuilder(channels, pcm16.SampleRate)
                .Loop(pcm16.Looping, pcm16.LoopStart, pcm16.LoopEnd)
                .Build();
        }

        protected override GcAdpcmFormat AddInternal(GcAdpcmFormat adpcm)
        {
            GcAdpcmFormatBuilder copy = GetCloneBuilder();
            copy.Channels = Channels.Concat(adpcm.Channels).ToArray();
            return copy.Build();
        }

        protected override GcAdpcmFormat GetChannelsInternal(int[] channelRange)
        {
            var channels = new List<GcAdpcmChannel>();

            foreach (int i in channelRange)
            {
                if (i < 0 || i >= Channels.Length)
                    throw new ArgumentException($"Channel {i} does not exist.", nameof(channelRange));
                channels.Add(Channels[i]);
            }

            GcAdpcmFormatBuilder copy = GetCloneBuilder();
            copy.Channels = channels.ToArray();
            copy.Tracks = null;
            return copy.Build();
        }

        public byte[] BuildSeekTable(int entryCount, Endianness endianness)
        {
            var tables = new short[Channels.Length][];

            Parallel.For(0, tables.Length, i =>
            {
                tables[i] = Channels[i].GetSeekTable();
            });

            short[] table = tables.Interleave(2);

            Array.Resize(ref table, entryCount * 2 * Channels.Length);
            return table.ToByteArray(endianness);
        }

        public static GcAdpcmFormatBuilder GetBuilder(GcAdpcmChannel[] channels, int sampleRate) => new GcAdpcmFormatBuilder(channels, sampleRate);
        public override GcAdpcmFormatBuilder GetCloneBuilder()
        {
            var builder = new GcAdpcmFormatBuilder(Channels, SampleRate);
            builder = GetCloneBuilderBase(builder);
            builder.Tracks = Tracks;
            builder.AlignmentMultiple = AlignmentMultiple;
            return builder;
        }

        public GcAdpcmFormat WithAlignment(int loopStartAlignment) => GetCloneBuilder()
            .WithAlignment(loopStartAlignment)
            .Build();

        private static GcAdpcmChannel EncodeChannel(int sampleCount, short[] pcm)
        {
            short[] coefs = GcAdpcmEncoder.DspCorrelateCoefs(pcm);
            byte[] adpcm = GcAdpcmEncoder.EncodeAdpcm(pcm, coefs);

            return new GcAdpcmChannel(adpcm, coefs, sampleCount);
        }

        private static IEnumerable<GcAdpcmTrack> GetDefaultTrackList(int channelCount)
        {
            int trackCount = channelCount.DivideByRoundUp(2);
            for (int i = 0; i < trackCount; i++)
            {
                int trackChannelCount = Math.Min(channelCount - i * 2, 2);
                yield return new GcAdpcmTrack
                {
                    ChannelCount = trackChannelCount,
                    ChannelLeft = i * 2,
                    ChannelRight = trackChannelCount >= 2 ? i * 2 + 1 : 0
                };
            }
        }
    }
}
