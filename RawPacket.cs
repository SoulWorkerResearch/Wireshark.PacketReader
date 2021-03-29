using System;

namespace SoulWorkerResearch.Wireshark.PacketReader
{
    internal sealed record RawPacket : IReadOnlyRawPacket
    {
        public ulong StreamSeqId { get; init; }
        public ulong StreamNextSeqId { get; init; }
        public ulong StreamId { get; init; }
        public ulong Frame { get; init; }
        public string RelativeTime { get; init; } = string.Empty;
        public string SrcIp { get; init; } = string.Empty;
        public string DstIp { get; init; } = string.Empty;
        public byte[] Payload { get; init; } = Array.Empty<byte>();
    }
}