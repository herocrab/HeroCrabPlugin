using HeroCrabPlugin.Core;

namespace HeroCrabPlugin.Stream
{
    public interface INetStream
    {
        event NetStream.SessionConnectedHandler SessionConnected;
        event NetStream.SessionDisconnectedHandler SessionDisconnected;

        event NetStream.ElementCreatedHandler ElementCreated;
        event NetStream.ElementDeletedHandler ElementDeleted;

        int ElementCount { get; }
        int SessionCount { get; }
        void DeleteSession(INetSublayer netSublayer);
        void Process(float time);
        void Clear();
    }
}
