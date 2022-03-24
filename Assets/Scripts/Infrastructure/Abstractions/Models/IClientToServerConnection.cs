using System;
using Mirror;

public interface IClientToServerConnection<out T>
{
    T ConnectionInstance { get; }

    PlayerSlot Slot { get; }

    Guid Id { get; }

    void Send<TMessage>(TMessage message)
        where TMessage : struct, NetworkMessage;
}