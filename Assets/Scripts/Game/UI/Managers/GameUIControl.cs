using System;
using UnityEngine;
using UnityEngine.UI;

public sealed class GameUIControl : MonoBehaviour
{
    public static event Action OnStartButtonPressed;
    public static event Action OnStopButtonPressed;

    [SerializeField] private Button _startButton;
    [SerializeField] private Button _stopButton;

    private void Awake()
    {
        DIContainer.Register(this);

        _startButton.onClick.AddListener(Start);
        _stopButton.onClick.AddListener(Stop);
    }

    private void Start()
    {
        OnStartButtonPressed?.Invoke();
    }
    private void Stop()
    {
       OnStopButtonPressed?.Invoke();
    }

}
