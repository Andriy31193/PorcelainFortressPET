using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class GameUIControl : MonoBehaviour
{
    public static event Action OnStartButtonPressed, OnResetButtonPressed, OnStopButtonPressed;

    [SerializeField] private Button _startButton, _resetButton, _stopButton;
    [SerializeField] private TextMeshProUGUI _countdownTimerText;

    private void Awake()
    {
        DIContainer.Register(this);
    }

    private void Start()
    {
        _startButton.onClick.AddListener(StartEvent);
        _stopButton.onClick.AddListener(StopEvent);
        _resetButton.onClick.AddListener(ResetEvent);
    }

    private void StartEvent()
    {
        OnStartButtonPressed?.Invoke();
    }
    private void StopEvent()
    {
        OnStopButtonPressed?.Invoke();
    }
    private void ResetEvent()
    {
        OnResetButtonPressed?.Invoke();
    }
    public void SetTimerValue(Color color)
    {
        _countdownTimerText.color = color;
    }
    public void SetTimerValue(string value, Color color)
    {
        SetTimerValue(color);
        SetTimerValue(value);
    }
    public void SetTimerValue(string value)
    {
        _countdownTimerText.text = value;
    }

    private void OnDestroy()
    {
        _startButton.onClick.RemoveListener(StartEvent);
        _stopButton.onClick.RemoveListener(StopEvent);
        _resetButton.onClick.RemoveListener(ResetEvent);
    }
}
