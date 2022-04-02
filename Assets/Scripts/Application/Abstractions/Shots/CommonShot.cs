using Mirror;
using UnityEngine;

public class CommonShot : CustomNetworkBehaviour
{
    public PlayerSlot PlayerOwner { get; set; }

    public float Dmg => _dmg;

    private float _dmg;
    [SerializeField] private float _speed;
    [SerializeField] private float _lifetime;

    private float _currentTime;
    private bool _needRotate;
    private float _angle;

    public override void OnStartClient()
    {
        GetComponent<Collider>().enabled = false;
        base.OnStartClient();
    }

    [ServerCallback]
    private void FixedUpdate()
    {
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
    public void Rotate(float angle)
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
