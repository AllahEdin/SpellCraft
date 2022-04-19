using System;
using Mirror;
using UnityEngine;
using Random = UnityEngine.Random;

public abstract class DudeBase : SpawnableCustomNetworkBehaviourBase
{
    private PlayerSlot? _owner;
    private Guid? _spawnPointId;
    private DudeOptions _options;
    private float _hp;
    private IGameManager _gameManager;
    private bool _active;

    protected bool Active => _active;

    public override void OnStartServer()
    {
        _gameManager = FindObjectOfType<CustomGameManager>();
        _gameManager.SrvStateChanged += SrvGameManagerOnSrvStateChanged;
        base.OnStartServer();
    }

    [Server]
    private void SrvGameManagerOnSrvStateChanged(GameState state)
    {
        switch (state)
        {
            case GameState.WaitingForConnections:
                _active = false;
                break;
            case GameState.ChooseSpell:
                _active = false;
                break;
            case GameState.Round:
                _active = true;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }

    public override void OnStartClient()
    {
        GetComponent<Collider>().enabled = false;
        base.OnStartClient();
    }

    private DudeOptions Options =>
        _options ?? throw new Exception();

    private PlayerSlot Owner =>
        _owner ?? throw new Exception();

    private Guid SpawnPointId =>
        _spawnPointId ?? throw new Exception();

    private bool _isReady;
    private float _currentTime;

    [ServerCallback]
    private void FixedUpdate()
    {
        if (_isReady && _active)
        {
            _currentTime += Time.deltaTime;

            _hp += _options.regeneration * Time.deltaTime;

            if (_currentTime > _options.cd)
            {
                _currentTime = 0;
                SrvShoot();
            }
        }
    }

    [ServerCallback]
    private void OnTriggerEnter(Collider col)
    {
        if (!_isReady || !_active)
            return;

        var shot = col.transform.GetComponent<CommonShot>();
        if (shot != null)
        {
            if (shot.PlayerOwner != Owner)
            {
                _hp -= shot.Dmg;
                if (_hp <= 0)
                {
                    SpawnPointsManager.SrvSetIsEmpty(Owner, SpawnPointId, true);
                    shot.HitTarget(this);
                    _gameManager.SrvStateChanged -= SrvGameManagerOnSrvStateChanged;
                    NetworkServer.Destroy(gameObject);
                }
            }
        }
    }


    [Server]
    public override void SrvApplyOptions(NetworkObjectOptions options)
    {
        var dudeOptions = options.GetOptions<DudeOptions>();
        _options = dudeOptions;
        SrvCheckIfReady();
    }

    [Server]
    public void SrvSetOwner(PlayerSlot owner)
    {
        _owner = owner;
        SrvCheckIfReady();
    }

    [Server]
    public void SrvSetSpawnPoint(Guid spawnPointId)
    {
        _spawnPointId = spawnPointId;
        SrvCheckIfReady();
    }

    [Server]
    private void SrvCheckIfReady()
    {
        _isReady =
            _owner.HasValue && _spawnPointId.HasValue && _options != null;
    }

    [Server]
    protected virtual void SrvShoot()
    {
        var angle = Random.Range(-Options.inaccuracy_angle, Options.inaccuracy_angle);
        ObjectManager.RegisterPrefab(Options.shot_descriptor);
        var instance =
            ObjectManager.SrvSpawn(Options.shot_descriptor, new NetworkObjectOptions(new ShotOptions()
            {
                dmg = 1
            }), transform.position, transform.rotation, o =>
            {
                var shot = o.GetComponent<CommonShot>();
                shot.SrvRotate(angle);
                shot.SrvSetActive(_active);
                shot.PlayerOwner = Owner;
            }, null);
    }
}