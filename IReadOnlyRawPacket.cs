using System.Collections.Generic;

namespace SoulWorkerResearch.Wireshark.PacketReader
{
    public interface IReadOnlyRawPacket
    {
        ulong StreamSeqId { get; }
        ulong StreamNextSeqId { get; }
        ulong StreamId { get; }
        ulong Frame { get; }
        string RelativeTime { get; }
        string SrcIp { get; }
        string DstIp { get; }
        IEnumerable<byte> Payload { get; }
    }
}