using System;
using Mirror;
using UnityEngine;

public class CommonShot : SpawnableCustomNetworkBehaviourBase
{
    public PlayerSlot PlayerOwner { get; set; }

    public float Dmg => _dmg;

    private float _dmg;
    [SerializeField] private float _speed;
    [SerializeField] private float _lifetime;

    private IGameManager _gameManager;

    private float _currentTime;
    private bool _needRotate;
    private float _angle;
    private bool _active;

    public override void OnStartClient()
    {
        GetComponent<Collider>().enabled = false;
        base.OnStartClient();
    }

    public override void OnStartServer()
    {
        _gameManager = FindObjectOfType<CustomGameManager>();
        _gameManager.SrvStateChanged += SrvGameManagerOnSrvStateChanged;
        base.OnStartServer();
    }

    [ServerCallback]
    private void FixedUpdate()
    {
        if (!_active)
            return;

        if (_needRotate)
        {
            transform.RotateAround(transform.position, Vector3.up, _angle);
            _needRotate = false;
        }

        _currentTime += Time.deltaTime;

        if (_currentTime > _lifetime)
        {
            NetworkServer.Destroy(gameObject);
        }
        else
        {
            transform.position += transform.forward.normalized * (Time.deltaTime * _speed);
        }
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

    [Server]
    public void SrvSetActive(bool active)
    {
        _active = active;
    }

    [Server]
    public void SrvRotate(float angle)
    {
        _angle = angle;
        _needRotate = true;
    }

    [Server]
    public void HitTarget(DudeBase dude)
    {
        NetworkServer.Destroy(gameObject);
    }

    [Server]
    public override void SrvApplyOptions(NetworkObjectOptions options)
    {
        var shotOptions = options.GetOptions<ShotOptions>();
        _dmg = shotOptions.dmg;
    }
}
