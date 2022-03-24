using UnityEngine;

public class TextView : MonoBehaviour
{
    [SerializeField] private TextMesh _text;

    private string _localText;

    private void FixedUpdate()
    {
        if (!string.IsNullOrWhiteSpace(_localText))
        {
            _text.text = _localText;
        }
    }

    public void SetText(string txt)
    {
        _localText = txt;
    }
}
