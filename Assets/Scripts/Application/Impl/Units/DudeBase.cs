using System;
using Mirror;
using UnityEngine;
using Random = UnityEngine.Random;

public abstract class DudeBase : CustomNetworkBehaviour
{
    private DudeOptionsBase _options;
    private NetworkObjectDescriptor _shotDescriptor;

    private PlayerSlot _owner;
    private Guid _spawnPointId;
    private float _hp;

    public PlayerSlot Owner
    {
        get => _owner;
        set
        {
            _isOwnerReady = true;
            _owner = value;
        }
    }

    public Guid SpawnPointId
    {
        get => _spawnPointId;
        set
        {
            _isSlotIdReady = true;
            _spawnPointId = value;
        }
    }

    public DudeOptionsBase Options
    {
        get => _options;
        set
        {
            _isOptionsReady = true;
            _hp = value.MaxHp;
            _options = value;
        }
    }

    public NetworkObjectDescriptor ShotDescriptor
    {
        get => _shotDescriptor;
        set
        {
            _isShotReady = true;
            _shotDescriptor = value;
        }
    }

    private float _currentTime;
    private bool _isOptionsReady;
    private bool _isShotReady;
    private bool _isOwnerReady;
    private bool _isSlotIdReady;

    [ServerCallback]
    private void FixedUpdate()
    {
        if (_isOptionsReady && _isShotReady && _isOwnerReady && _isSlotIdReady)
        {
            _currentTime += Time.deltaTime;

            _hp += _options.HpRegeneration * Time.deltaTime;

            if (_currentTime > _options.Cd)
            {
                _currentTime = 0;
                Shoot();
            }
        }
    }

    [Server]
    private void Shoot()
    {
        var angle = Random.Range(-Options.InaccuracyDegrees, Options.InaccuracyDegrees);
        ObjectManager.RegisterPrefab(_shotDescriptor);
        var instance =
            ObjectManager.Spawn(_shotDescriptor, transform.position, transform.rotation, o =>
            {
                var shot = o.GetComponent<CommonShot>();
                shot.Rotate(angle);
                shot.PlayerOwner = _owner;
            }, null);
    }


    [ServerCallback]
    private void OnTriggerEnter(Collider col)
    {
        if (!_isOptionsReady || !_isOwnerReady)
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
                    NetworkServer.Destroy(gameObject);
                }
            }
        }
    }

}

public class DudeOptionsBase
{
    public float Cd;
    public float InaccuracyDegrees;
    public float MaxHp;
    //public float Shield;
    public float HpRegeneration;
    //public float ShieldRegeneration;
}