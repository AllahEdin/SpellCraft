using Mirror;
using UnityEngine;

public class GameUiPlayer : NetworkBehaviour
{
    private GameUiGameManager _gameManager;
    private GameUiButtons _buttons;

    private bool _ready;

    private int _playerNumber;

    private GameUiGameManager GameManager =>
        _gameManager ??= FindObjectOfType<GameUiGameManager>();

    private GameUiButtons Buttons =>
        _buttons ??= FindObjectOfType<GameUiButtons>();

    public override void OnStartLocalPlayer()
    {
        Buttons.GetIncreaseGoldBy1Button().onClick.AddListener(() => ChangeGold(1));
        Buttons.GetDecreaseGoldBy1Button().onClick.AddListener(() => ChangeGold(-1));

        Buttons.GetIncreaseDmgBy1Button().onClick.AddListener(() => ChangeDmg(1));
        Buttons.GetDecreaseDmgBy1Button().onClick.AddListener(() => ChangeDmg(-1));

        Buttons.GetIncreaseHpBy1Button().onClick.AddListener(() => ChangeHp(1));
        Buttons.GetDecreaseHpBy1Button().onClick.AddListener(() => ChangeHp(-1));

        Buttons.GetNextTurnButton().onClick.AddListener(ClientEndTurn);
    }

    [Client]
    public void ChangeGold(int value)
    {
        if (_ready && isLocalPlayer)
        {
            Debug.Log($"Client: {_playerNumber} gold value: {value}");
            CmdChangeGold(_playerNumber, value);
        }
    }

    [Command]
    public void CmdChangeGold(int playerNumber, int value)
    {
        GameManager.ServerChangeGoldForPlayer(playerNumber, value);
    }


    [Client]
    public void ChangeDmg(int value)
    {
        if (_ready && isLocalPlayer)
        {
            Debug.Log($"Client: {_playerNumber} gold value: {value}");
            CmdChangeDmg(_playerNumber, value);
        }
    }

    [Command]
    public void CmdChangeDmg(int playerNumber, int value)
    {
        GameManager.ServerChangeDmgForPlayer(playerNumber, value);
    }

    [Client]
    public void ChangeHp(int value)
    {
        if (_ready && isLocalPlayer)
        {
            Debug.Log($"Client: {_playerNumber} gold value: {value}");
            CmdChangeHp(_playerNumber, value);
        }
    }

    [Command]
    public void CmdChangeHp(int playerNumber, int value)
    {
        GameManager.ServerChangeHpForPlayer(playerNumber, value);
    }


    [Client]
    private void ClientEndTurn()
    {
        if (_ready && isLocalPlayer)
        {
            CmdEndTurn(_playerNumber);
        }
    }

    [Command]
    private void CmdEndTurn(int playerNum)
    {
        GameManager.ServerEndTurn(playerNum);
    }

    [TargetRpc]
    public void TargetSetPlayerNumber(int number)
    {
        _ready = true;
        _playerNumber = number;
    }
}