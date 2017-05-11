using System.IO;
using VGAudio.Containers.Adx;
using VGAudio.Formats;
using VGAudio.Utilities;
using static VGAudio.Utilities.Helpers;

namespace VGAudio.Containers
{
    public class AdxReader : AudioReader<AdxReader, AdxStructure, AdxConfiguration>
    {
        protected override AdxStructure ReadFile(Stream stream, bool readAudioData = true)
        {
            using (BinaryReader reader = GetBinaryReader(stream, Endianness.BigEndian))
            {
                if (reader.ReadUInt16() != 0x8000)
                {
                    throw new InvalidDataException("File doesn't have ADX signature (0x80 0x00)");
                }

                var structure = new AdxStructure();

                ReadHeader(reader, structure);

                if (readAudioData)
                {
                    reader.BaseStream.Position = structure.CopyrightOffset + 4;
                    ReadData(reader, structure);
                }

                return structure;
            }
        }

        protected override IAudioFormat ToAudioStream(AdxStructure structure)
        {
            return new CriAdxFormat.Builder(structure.AudioData, structure.SampleCount, structure.SampleRate, structure.FrameSize, structure.HighpassFreq)
                .Loop(structure.Looping, structure.LoopStartSample, structure.LoopEndSample)
                .Build();
        }

        private static void ReadHeader(BinaryReader reader, AdxStructure structure)
        {
            structure.CopyrightOffset = reader.ReadInt16();
            structure.EncodingType = reader.ReadByte();
            structure.FrameSize = reader.ReadByte();
            structure.BitDepth = reader.ReadByte();
            structure.ChannelCount = reader.ReadByte();
            structure.SampleRate = reader.ReadInt32();
            structure.SampleCount = reader.ReadInt32();
            structure.HighpassFreq = reader.ReadInt16();
            structure.Version = reader.ReadByte();
            structure.Flags = reader.ReadByte();
            structure.AlignmentSamples = reader.ReadInt16();
            structure.Looping = reader.ReadInt16() == 1;
            reader.BaseStream.Position += 4;
            structure.LoopStartSample = reader.ReadInt32();
            structure.LoopStartByte = reader.ReadInt32();
            structure.LoopEndSample = reader.ReadInt32();
            structure.LoopEndByte = reader.ReadInt32();
        }

        private static void ReadData(BinaryReader reader, AdxStructure structure)
        {
            const ushort footerBytes = 0x8001;
            int blockSize = structure.FrameSize * structure.ChannelCount;
            int blockCount = 0;

            int fileSize = (int)reader.BaseStream.Length;
            int position = (int)reader.BaseStream.Position;

            while (position < fileSize)
            {
                reader.BaseStream.Position = position;
                ushort peek = reader.ReadUInt16();
                if (peek == footerBytes)
                {
                    break;
                }
                blockCount++;
                position += blockSize;
            }

            reader.BaseStream.Position = structure.CopyrightOffset + 4;
            int dataLength = blockCount * blockSize;
            structure.AudioData = reader.BaseStream.DeInterleave(dataLength, structure.FrameSize, structure.ChannelCount);
        }
    }
}
