using System;
using System.Collections;
using UnityEngine;

public sealed class ClockManager : Photon.MonoBehaviour
{

    public static event Action OnClockReady;
    public static event Action OnTimeUp;


    private double _clockValue;
    private bool _clockReady = false;

    private GameUIControl _gameUIControl;
    private GameStatus _gameStatus;

    private void Awake() {
        
        GameManager.OnGameStatusChanged += OnGameStatusChanged;
    }
    private void Start()
    {
        _gameUIControl = DIContainer.Resolve<GameUIControl>();

    }
    private void Update()
    {
        if (_clockReady && _gameStatus == GameStatus.Playing)
        {
            if (_clockValue > 0)
            {
                if (_clockValue <= GameSettings.ROUND_TIME - 10)
                    _gameUIControl.SetTimerValue(Color.red);

                _clockValue -= Time.deltaTime;

                _gameUIControl.SetTimerValue(string.Format("{0:00}:{1:00}", (int)Mathf.Floor((float)_clockValue / 60), (int)Mathf.Floor((float)_clockValue % 60)));
            }
            else
            {
                _gameUIControl.SetTimerValue("--:--");

                OnTimeUp?.Invoke();
            }
        }
    }
    public void StartClock()
    {
        _clockValue = GameSettings.ROUND_TIME;
        _gameUIControl.SetTimerValue(Color.white);

        StartCoroutine(IESetGlobalTime());
        StartCoroutine(IEUpdateServerTime());
    }
    private void OnGameStatusChanged(GameStatus status)
    {
        _gameStatus = status;

        if (_gameStatus == GameStatus.Ready)
            StartClock();
    }

    #region Time Sync
    private IEnumerator IESetGlobalTime()
    {
        while (!PhotonNetwork.connectedAndReady) yield return null;

        if (PhotonNetwork.player.IsMasterClient)
            SetRoomPhotonTime();

    }
    private IEnumerator IEAutoServerTimeSync()
    {
        while (!PhotonNetwork.inRoom || !PhotonNetwork.connectedAndReady) yield return null;

        if (PhotonNetwork.player.IsMasterClient)
        {
            photonView.RPC(nameof(UpdateServerTimeRPC), PhotonTargets.All, _clockValue);
        }

        if (_gameStatus == GameStatus.Playing)
        {
            yield return new WaitForSeconds(3);
            StartCoroutine(IEAutoServerTimeSync());
        }
        else _clockReady = false;
    }
    private IEnumerator IEUpdateServerTime()
    {
        while (!PhotonNetwork.inRoom || !PhotonNetwork.connectedAndReady) yield return null;


        if (PhotonNetwork.player.IsMasterClient)
        {
            PhotonNetwork.room.IsVisible = true;
            _clockReady = true;

            yield return new WaitForSeconds(3);

            StartCoroutine(IEAutoServerTimeSync());
        }
    }
    private void SetRoomPhotonTime()
    {
        ExitGames.Client.Photon.Hashtable hash = PhotonNetwork.room.CustomProperties;

        if (!hash.ContainsKey("ServerTime"))
            hash.Add("ServerTime", PhotonNetwork.time);
        else hash["ServerTime"] = PhotonNetwork.time;

        PhotonNetwork.room.SetCustomProperties(hash);
    }
    [PunRPC]
    public void UpdateServerTimeRPC(double time)
    {
        _clockValue = time;
        _clockReady = true;

        OnClockReady?.Invoke();
    }
    #endregion
}