using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerInteraction))]
public sealed class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }

    private void Awake() {
        if(Instance != null)
            Destroy(Instance.gameObject);

        Instance = this;
    }
    private void Start()
    {
        
    }

}
