using System;
using Mirror;

public class MirrorConnection : IClientToServerConnection<NetworkConnection>
{
    private Guid? _id;
    private readonly NetworkConnection _connection;

    public MirrorConnection(NetworkConnection connection, PlayerSlot slot)
    {
        _connection = connection;
        Slot = slot;
    }

    public NetworkConnection ConnectionInstance =>
        _connection;

    public PlayerSlot Slot { get; }

    public Guid Id
    {
        get
        {
            if (_id.HasValue)
            {
                return _id.Value;
            }

            var guid = Guid.NewGuid();
            _id = guid;
            return guid;
        }
    }

    public void Send<T>(T message)
    where T : struct, NetworkMessage
    {
        _connection.Send(message);
    }
}
