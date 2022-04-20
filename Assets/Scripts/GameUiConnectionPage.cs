using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class GameUiConnectionPage : MonoBehaviour
{
    [SerializeField] private GameUiNetworkManager _networkManager;
    [SerializeField] private InputField _ipInputField;
    [SerializeField] private GameObject _hideOnConnect;
    [SerializeField] private Button _connectClientButton;
    [SerializeField] private Button _connectServerButton;

    private bool _connected = false;

    void Start()
    {
        _connectClientButton.onClick.AddListener(() =>
        {
            _networkManager.StartClient();
        });

        _connectServerButton.onClick.AddListener(() =>
        {
            _networkManager.StartHost();
        });

        _ipInputField.onValueChanged.AddListener(value => _networkManager.networkAddress = value);
    }

    void Update()
    {
        if (_connected)
            return;

        if (NetworkClient.isConnected || NetworkServer.active)
        {
            _connectClientButton.onClick.RemoveAllListeners();
            _connectServerButton.onClick.RemoveAllListeners();
            _ipInputField.onValueChanged.RemoveAllListeners();
            _hideOnConnect.SetActive(false);
            _connected = true;
        }
    }
}
