using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _promtText;

    public void Awake()
    {
        if(_promtText == null)
            _promtText = GameObject.Find("PromptText").GetComponent<TextMeshProUGUI>();
    }

    public void UpdateText(string promtMessage)
    {
        _promtText.text = promtMessage;
    }
}
