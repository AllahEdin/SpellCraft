using System;

public interface IPlayerInputManager
{
    /// <summary>
    /// player id, item id, spawn point id
    /// </summary>
    event Action<PlayerSlot, Guid , Guid> OnPlayerAttemptsToUseItem;
}