using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerInteraction))]
public sealed class PlayerController : Photon.MonoBehaviour, IPlayer
{

    public float moveDistance = 1f;
    public float moveInterval = 1f;

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
        photonView.RPC(nameof(ProcessWinner), PhotonTargets.All);
        Debug.Log("I WON!");
    }
    /// <summary>
    /// Temporary solution
    /// </summary>
    [PunRPC]
    public void ProcessWinner()
    {
        Debug.Log("WINNER IS SELECTED");
        this.StopMovement();
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

    public void ChangeDirection(Direction newDirection)
    {
        Debug.Log(newDirection.ToString());
        switch(newDirection)
        {
            case Direction.Left:
            transform.Rotate(0,-90,0);
            break;
            case Direction.Right:
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
