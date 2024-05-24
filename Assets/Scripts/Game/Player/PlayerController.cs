using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerInteraction))]
public sealed class PlayerController : Photon.MonoBehaviour, IPlayer
{
    
    public float moveDistance = 1f;
    public float moveInterval = 1f;

    private string _nickname = string.Empty;

    private GameManager _gm;
    private Vector3 _startPosition;

    private void Awake()
    {
        DIContainer.Register<IPlayer>(this);
    }
    private void Start()
    {
        _gm = DIContainer.Resolve<GameManager>();

        _startPosition = transform.position;
    }
    public void SetNickname(string value)
    {
        _nickname = value;
        PhotonNetwork.playerName = _nickname;
    }
    public string GetNickname() => _nickname;
    public void StartMovement()
    {
        InvokeRepeating(nameof(TryMoveForward), moveInterval, moveInterval);
    }
    
    public void ResetMovement()
    {
        StopMovement();
        transform.position = _startPosition;
        transform.rotation = Quaternion.identity;
    }

    public void StopMovement()
    {
        CancelInvoke(nameof(TryMoveForward));
    }
    public void Finish()
    {
        GameManager.OnSelectWinner?.Invoke(this);
    }
    private void TryMoveForward()
    {
        Vector3 newPosition = transform.position + transform.forward * moveDistance;

        byte mazeX = (byte)Mathf.RoundToInt(4.5f - newPosition.z);
        byte mazeZ = (byte)Mathf.RoundToInt(newPosition.x + 4.5f);

        if (_gm.TryMovePlayerOnMaze(mazeX, mazeZ))
        {
            transform.position = newPosition;
            GameManager.OnPlayerMoved?.Invoke(this, mazeX, mazeZ);
        }
    }

    public void ChangeDirection(DirectionType newDirection)
    {
        Debug.Log(newDirection.ToString());
        switch(newDirection)
        {
            case DirectionType.Left:
            transform.Rotate(0,-90,0);
            break;
            case DirectionType.Right:
            transform.Rotate(0,90,0);
            break;
        }
    }

    public void TakeDamage(int damage)
    {
        //
    }

    private void OnDestroy()
    {
    }
}
