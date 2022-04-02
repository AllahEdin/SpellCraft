using Mirror;

public abstract class CustomNetworkBehaviour : NetworkBehaviour
{
    private IPlayerInputManager _playerInputManager;
    private ISpawnPointsManager _spawnPointsManager;
    private IObjectManager _objectManager;
    private IPlayersManager<NetworkConnection> _playersManager;
    private IItemManager _itemsManager;

    protected IPlayerInputManager PlayerInputManager =>
        _playerInputManager ??= FindObjectOfType<PlayerInputManager>();

    protected IPlayersManager<NetworkConnection> PlayersManager =>
        _playersManager ??= FindObjectOfType<PlayerManager>();

    protected IObjectManager ObjectManager =>
        _objectManager ??= FindObjectOfType<ObjectManager>();

    protected IItemManager ItemManager =>
        _itemsManager ??= FindObjectOfType<ItemManager>();

    protected ISpawnPointsManager SpawnPointsManager =>
        _spawnPointsManager ??= FindObjectOfType<SpawnPointsManager>();

    public abstract void SrvApplyOptions(NetworkObjectOptions options);
}