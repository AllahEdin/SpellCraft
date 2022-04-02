using System;
using Mirror;

public interface IClientToServerConnection<out T>
{
    T ConnectionInstance { get; }

    PlayerSlot Slot { get; }

    Guid Id { get; }
}