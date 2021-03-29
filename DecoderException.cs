using System;

namespace SoulWorkerResearch.Wireshark.PacketReader
{
    [Serializable]
    public sealed class ReaderException : Exception
    {
        public ReaderException()
        {
        }

        public ReaderException(string message) : base(message)
        {
        }

        public ReaderException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}