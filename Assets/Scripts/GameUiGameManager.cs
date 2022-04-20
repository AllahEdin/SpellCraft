using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class GameUiGameManager : NetworkBehaviour
{
    [SerializeField] private Text _playerNumberText;
    [SerializeField] private Text _playerTurnText;
    [SerializeField] private Text _playerGoldText;
    [SerializeField] private Text _playerDmgText;
    [SerializeField] private Text _playerHpText;
    [SerializeField] private Text _yourHpText;
    [SerializeField] private Text _enemyHpText;


    private GameUiNetworkManager _networkManager;
    private GameUiNetworkManager NetworkManager =>
        _networkManager ??= FindObjectOfType<GameUiNetworkManager>();

    private bool _ready;
    private int _playerTurn = -1;

    private int _currentPlayerGold = 0;
    private int _currentPlayerDmg = 0;
    private int _currentPlayerHp = 0;

    private int _clientPlayerNumber;

    private int[] _hp = new int[2]
    {
        50,
        50
    };

    [TargetRpc]
    public void TargetSetPlayerNumber(NetworkConnection connection, int number)
    {
        _clientPlayerNumber = number;
        _playerNumberText.text = $"{number}";
    }

    [Server]
    public void ServerStart()
    {
        _ready = true;
        ServerSetPlayerTurn(0);
    }





    [Server]
    public void ServerSetPlayerTurn(int num)
    {
        if (!_ready)
            return;
        _playerTurn = num;
        RpcSetPlayerTurnNumber(num);
        ServerSetPlayerGold(0);
        ServerSetPlayerDmg(0);
        ServerSetPlayerHp(0);
    }

    [ClientRpc]
    public void RpcSetPlayerTurnNumber(int num)
    {
        _playerTurnText.text =
            _clientPlayerNumber == num 
                ? "Your turn"
                : "Enemy turn";
    }




    [Server]
    public void ServerChangeGoldForPlayer(int playerNumber, int changeValue)
    {
        Debug.Log($"Player {playerNumber} trying to change gold changeValue by {changeValue}");

        if (!_ready)
            return;

        if (_playerTurn != playerNumber)
            return;

        if (_currentPlayerGold + changeValue < 0)
        {
            return;
        }

        ServerSetPlayerGold(_currentPlayerGold + changeValue);
    }

    [Server]
    public void ServerSetPlayerGold(int value)
    {
        _currentPlayerGold = value;
        RpcChangeGold(_currentPlayerGold);
    }

    [ClientRpc]
    public void RpcChangeGold(int currentValue)
    {
        _playerGoldText.text = $"{currentValue} gold";
    }





    [Server]
    public void ServerChangeDmgForPlayer(int playerNumber, int changeValue)
    {
        Debug.Log($"Player {playerNumber} trying to change dmg changeValue by {changeValue}");

        if (!_ready)
            return;

        if (_playerTurn != playerNumber)
            return;

        if (_currentPlayerDmg + changeValue < 0)
        {
            return;
        }

        ServerSetPlayerDmg(_currentPlayerDmg + changeValue);
    }

    [Server]
    public void ServerSetPlayerDmg(int value)
    {
        _currentPlayerDmg = value;
        RpcChangeDmg(_currentPlayerDmg);
    }

    [ClientRpc]
    public void RpcChangeDmg(int currentValue)
    {
        _playerDmgText.text = $"{currentValue} dmg";
    }




    [Server]
    public void ServerChangeHpForPlayer(int playerNumber, int changeValue)
    {
        Debug.Log($"Player {playerNumber} trying to change dmg changeValue by {changeValue}");

        if (!_ready)
            return;

        if (_playerTurn != playerNumber)
            return;

        if (_currentPlayerHp + changeValue < 0)
        {
            return;
        }

        ServerSetPlayerHp(_currentPlayerHp + changeValue);
    }

    [Server]
    public void ServerSetPlayerHp(int value)
    {
        _currentPlayerHp = value;
        RpcChangeHp(_currentPlayerHp);
    }

    [ClientRpc]
    public void RpcChangeHp(int currentValue)
    {
        _playerHpText.text = $"{currentValue} hp";
    }




    [Server]
    public void ServerEndTurn(int playerNum)
    {
        if (!_ready)
            return;

        if (_playerTurn != playerNum)
            return;

        var nextPlayer =
            _playerTurn == 0
                ? 1
                : 0;

        _hp[nextPlayer] -= _currentPlayerDmg;
        _hp[playerNum] += _currentPlayerHp;
        RpcUpdateHp(_hp[0], _hp[1]);
        ServerSetPlayerTurn(nextPlayer);
    }

    [ClientRpc]
    public void RpcUpdateHp(int player0, int player1)
    {
        if (_clientPlayerNumber == 0)
        {
            _yourHpText.text = $"{player0}";
            _enemyHpText.text = $"{player1}";
        }
        else
        {
            _yourHpText.text = $"{player1}";
            _enemyHpText.text = $"{player0}";
        }
    }
}