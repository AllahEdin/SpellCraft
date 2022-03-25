using Mirror;
using UnityEngine;

public class RandomShootingDude : CustomNetworkBehaviour
{
    [SerializeField] private NetworkObjectDescriptor _shotDescriptor;

    [SerializeField] private float _cd;
    [SerializeField] private float _missAngle;

    private float _currentTime;
    private bool _isReady;

    [Server]
    public void SetReady()
    {
        _isReady = true;
    }

    [ServerCallback]
    private void FixedUpdate()
    {
        if (_isReady)
        {
            _currentTime += Time.deltaTime;

            if (_currentTime > _cd)
            {
                _currentTime = 0;
                Shoot();
            }
        }
    }

    [Server]
    private void Shoot()
    {
        var angle = Random.Range(-_missAngle, _missAngle);
        ObjectManager.RegisterPrefab(_shotDescriptor);
        var instance =
            ObjectManager.Spawn(_shotDescriptor, transform.position, transform.rotation, o =>
            {
                o.GetComponent<CommonShot>().Rotate(angle);
            }, null);
    }
}
