using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerInteraction))]
public sealed class PlayerController : MonoBehaviour, IPlayer
{
    public static PlayerController Instance { get; private set; }

    public float moveDistance = 1f;
    public float moveInterval = 1f;
    private void Awake()
    {
        if (Instance != null)
            Destroy(Instance.gameObject);

        Instance = this;

    }
    private void Start()
    {

    }

    public void StartMovement()
    {
        InvokeRepeating(nameof(TryMoveForward), moveInterval, moveInterval);
    }

    public void StopMovement()
    {
        CancelInvoke(nameof(TryMoveForward));
    }

    private void TryMoveForward()
    {
        Vector3 newPosition = transform.position + transform.forward * moveDistance;

        byte mazeX = (byte)Mathf.RoundToInt(4.5f - newPosition.z);
        byte mazeZ = (byte)Mathf.RoundToInt(newPosition.x + 4.5f);

        if (GameManager.Instance.TryMovePlayerOnMaze(mazeX, mazeZ))
        {
            transform.position = newPosition;
        }
    }

    public void ChangeDirection(Direction newDirection)
    {
        //
        Debug.Log(newDirection.ToString());
    }

    public void TakeDamage(int damage)
    {
        //
    }

    private void OnDestroy()
    {
    }

}
