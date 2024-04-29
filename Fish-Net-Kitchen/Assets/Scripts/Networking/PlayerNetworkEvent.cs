using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;
using UnityEngine.Events;

public class PlayerNetworkEvent : NetworkBehaviour
{
    [SerializeField] private bool debugEvents = false;

    public UnityEvent<bool> OnStartClientEvent = new UnityEvent<bool>();
    public UnityEvent OnStartServerEvent = new UnityEvent();

    void Awake()
    {
        OnStartClientEvent.AddListener((isOwner) =>
        {
            if (debugEvents) Debug.Log("OnStartClientEvent: " + isOwner);
        });

        OnStartServerEvent.AddListener(() =>
        {
            if (debugEvents) Debug.Log("OnStartServerEvent");
        });
        
    }
    
    public override void OnStartClient()
    {
        base.OnStartClient();
        OnStartClientEvent?.Invoke(IsOwner);
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        OnStartServerEvent?.Invoke();
    }
}
