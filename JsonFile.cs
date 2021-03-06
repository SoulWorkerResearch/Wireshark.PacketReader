using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace SoulWorkerResearch.Wireshark.PacketReader
{
    public static class JsonFile
    {
        public static async Task<IEnumerable<IEnumerable<IReadOnlyRawPacket>>> ReadAsync(string path)
        {
            await using FileStream inputFile = new(path, FileMode.Open, FileAccess.Read, FileShare.Read);

            using JsonDocument json = JsonDocument.Parse(inputFile);
            using JsonElement.ArrayEnumerator enumerator = json.RootElement.EnumerateArray();

            IEnumerable<IReadOnlyRawPacket> rawPackets = enumerator
                .Select(element =>
                {
                    if (!element.TryGetProperty("_source", out JsonElement source))
                    {
                        throw new ReaderException();
                    }

                    if (!source.TryGetProperty("layers", out JsonElement layers))
                    {
                        throw new ReaderException();
                    }

                    if (!layers.TryGetProperty("tcp", out JsonElement tcp))
                    {
                        throw new ReaderException();
                    }

                    if (!layers.TryGetProperty("frame", out JsonElement frame))
                    {
                        throw new ReaderException();
                    }

                    if (!frame.TryGetProperty("frame.number", out JsonElement jsonFrameNumber))
                    {
                        throw new ReaderException();
                    }

                    if (!layers.TryGetProperty("ip", out JsonElement ip))
                    {
                        throw new ReaderException();
                    }

                    if (!ip.TryGetProperty("ip.dst_host", out JsonElement dstHost))
                    {
                        throw new ReaderException();
                    }

                    if (!ip.TryGetProperty("ip.src_host", out JsonElement srcHost))
                    {
                        throw new ReaderException();
                    }

                    if (!frame.TryGetProperty("frame.time_relative", out JsonElement timeRelativeElement))
                    {
                        throw new ReaderException();
                    }

                    if (!tcp.TryGetProperty("tcp.payload", out JsonElement payload))
                    {
                        return null;
                    }

                    if (!tcp.TryGetProperty("tcp.stream", out JsonElement jsonTcpStream))
                    {
                        return null;
                    }

                    if (!tcp.TryGetProperty("tcp.seq", out JsonElement jsonTcpSeq))
                    {
                        return null;
                    }

                    if (!tcp.TryGetProperty("tcp.seq", out JsonElement jsonTcpNxtseq))
                    {
                        return null;
                    }

                    string? frameNumber = jsonFrameNumber.GetString();
                    if (string.IsNullOrEmpty(frameNumber))
                    {
                        return null;
                    }

                    string? frameDstIp = dstHost.GetString();
                    if (string.IsNullOrEmpty(frameDstIp))
                    {
                        return null;
                    }

                    string? frameSrcIp = srcHost.GetString();
                    if (string.IsNullOrEmpty(frameSrcIp))
                        return null;

                    string? frameRelativeTime = timeRelativeElement.GetString();
                    if (string.IsNullOrEmpty(frameRelativeTime))
                    {
                        return null;
                    }

                    string? framePayload = payload.GetString();
                    if (string.IsNullOrEmpty(framePayload))
                    {
                        return null;
                    }

                    string? tcpStream = jsonTcpStream.GetString();
                    if (string.IsNullOrEmpty(tcpStream))
                    {
                        return null;
                    }

                    string? tcpSeq = jsonTcpSeq.GetString();
                    if (string.IsNullOrEmpty(tcpSeq))
                    {
                        return null;
                    }

                    string? tcpNextSeq = jsonTcpNxtseq.GetString();
                    if (string.IsNullOrEmpty(tcpNextSeq))
                    {
                        return null;
                    }

                    return new RawPacket()
                    {
                        StreamNextSeqId = ulong.Parse(tcpNextSeq),
                        StreamSeqId = ulong.Parse(tcpSeq),
                        StreamId = ulong.Parse(tcpStream),
                        Frame = ulong.Parse(frameNumber),
                        DstIp = frameDstIp,
                        SrcIp = frameSrcIp,
                        RelativeTime = frameRelativeTime,
                        Payload = Convert.FromHexString(framePayload.Where(s => s != ':').ToArray())
                    };
                })
                .Where(s => s is not null)
                .Cast<IReadOnlyRawPacket>()
                .ToArray();

            return rawPackets.Select(s => s.StreamId).Distinct().Select(k => rawPackets.Where(s => s.StreamId == k));
        }
    }
}