using UnityEngine;
using UnityEngine.UI;

public class GameUiButtons : MonoBehaviour
{
    [SerializeField] private Button _increaseGoldBy1Button;
    [SerializeField] private Button _decreaseGoldBy1Button;

    [SerializeField] private Button _increaseDmgBy1Button;
    [SerializeField] private Button _decreaseDmgBy1Button;

    [SerializeField] private Button _increaseHpBy1Button;
    [SerializeField] private Button _decreaseHpBy1Button;

    [SerializeField] private Button _nextTurnButton;

    public Button GetNextTurnButton()
    {
        return _nextTurnButton;
    }

    public Button GetIncreaseGoldBy1Button()
    {
        return _increaseGoldBy1Button;
    }

    public Button GetDecreaseGoldBy1Button()
    {
        return _decreaseGoldBy1Button;
    }

    public Button GetIncreaseDmgBy1Button()
    {
        return _increaseDmgBy1Button;
    }

    public Button GetDecreaseDmgBy1Button()
    {
        return _decreaseDmgBy1Button;
    }

    public Button GetIncreaseHpBy1Button()
    {
        return _increaseHpBy1Button;
    }

    public Button GetDecreaseHpBy1Button()
    {
        return _decreaseHpBy1Button;
    }
}