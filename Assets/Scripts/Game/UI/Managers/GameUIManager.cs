using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class GameUIManager : MonoBehaviour
{
    public static event Action<GameControlType> OnControlButtonPressed;

    [Header("Game Controls")]
    [SerializeField] private Button _startButton, _resetButton, _stopButton;

    [Header("Clock UI")]
    [SerializeField] private TextMeshProUGUI _countdownTimerText;
    [Header("Player UI")]
    [SerializeField] private TextMeshProUGUI _localPlayerNicknameText;
    [SerializeField] private GameObject _winnerPanel;
    [SerializeField] private TextMeshProUGUI _winnerNickname;

    private void Awake()
    {
        DIContainer.Register(this);

        InitializeButtonListeners();
    }

    private void InitializeButtonListeners()
    {
        _startButton.onClick.AddListener(() => OnControlButtonPressed?.Invoke(GameControlType.Start));
        _stopButton.onClick.AddListener(() => OnControlButtonPressed?.Invoke(GameControlType.Stop));
        _resetButton.onClick.AddListener(() => OnControlButtonPressed?.Invoke(GameControlType.Restart));
    }

    public void DisplayUIWinner(string nickname)
    {
        _winnerNickname.text = nickname;
        _winnerPanel.SetActive(true);
        _countdownTimerText.gameObject.SetActive(false);
    }
    
    public void SetUITimerValue(string value, Color color)
    {
        SetUITimerValue(color);
        SetUITimerValue(value);
    }
    public void SetUITimerValue(Color color) => _countdownTimerText.color = color;
    public void SetUITimerValue(string value) => _countdownTimerText.text = value;
    
    public void SetUIPlayerNickname(string value) => _localPlayerNicknameText.text = value;


    private void OnDestroy() => RemoveButtonListeners();

    private void RemoveButtonListeners()
    {
        _startButton.onClick.RemoveAllListeners();
        _stopButton.onClick.RemoveAllListeners();
        _resetButton.onClick.RemoveAllListeners();
    }
}
