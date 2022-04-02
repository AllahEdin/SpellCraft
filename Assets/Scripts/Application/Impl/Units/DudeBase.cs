using System;
using Mirror;
using UnityEngine;
using Random = UnityEngine.Random;

public abstract class DudeBase : CustomNetworkBehaviour
{
    private PlayerSlot? _owner;
    private Guid? _spawnPointId;
    private DudeOptions _options;
    private float _hp;

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
        if (_isReady)
        {
            _currentTime += Time.deltaTime;

            _hp += _options.regeneration * Time.deltaTime;

            if (_currentTime > _options.cd)
            {
                _currentTime = 0;
                Shoot();
            }
        }
    }

    [ServerCallback]
    private void OnTriggerEnter(Collider col)
    {
        if (!_isReady)
            return;

        var shot = col.transform.GetComponent<CommonShot>();
        if (shot != null)
        {
            if (shot.PlayerOwner != Owner)
            {
                _hp -= shot.Dmg;
                if (_hp <= 0)
                {
                    SpawnPointsManager.SetIsEmpty(Owner, SpawnPointId, true);
                    shot.HitTarget(this);
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
    private void Shoot()
    {
        var angle = Random.Range(-Options.inaccuracy_angle, Options.inaccuracy_angle);
        ObjectManager.RegisterPrefab(Options.shot_descriptor);
        var instance =
            ObjectManager.Spawn(Options.shot_descriptor, new NetworkObjectOptions(new ShotOptions()
            {
                dmg = 1
            }), transform.position, transform.rotation, o =>
            {
                var shot = o.GetComponent<CommonShot>();
                shot.Rotate(angle);
                shot.PlayerOwner = Owner;
            }, null);
    }
}