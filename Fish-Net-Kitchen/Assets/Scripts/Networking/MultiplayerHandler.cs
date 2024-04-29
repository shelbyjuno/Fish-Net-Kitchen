using System.Collections;
using System.Collections.Generic;
using FishNet.Managing;
using UnityEngine;
using Unity.Multiplayer.Playmode;
using System.Linq;
using FishNet.Transporting.Multipass;


public class MultiplayerHandler : MonoBehaviour
{
    public Multipass multipass;

    [Header("MPPM Settings")]
    [SerializeField] private bool autoHost = false;
    [SerializeField] private bool autoClient = false;

    [Header("Host Settings")]
    [SerializeField] private KeyCode hostKey = KeyCode.H;

    [Header("Client Settings")]
    [SerializeField] private KeyCode clientKey = KeyCode.C;

    void Start()
    {
        var tags = CurrentPlayer.ReadOnlyTags();

        if (tags.Contains("Host") && autoHost)
        {
            multipass.StartConnection(true);
            multipass.StartConnection(false);
        }

        if (tags.Contains("Client") && autoClient)
        {
            multipass.StartConnection(false);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(hostKey))
        {
            multipass.StartConnection(true);
            multipass.StartConnection(false);
        }
        else if (Input.GetKeyDown(clientKey))
        {
            multipass.StartConnection(false);
        }
    }
}
