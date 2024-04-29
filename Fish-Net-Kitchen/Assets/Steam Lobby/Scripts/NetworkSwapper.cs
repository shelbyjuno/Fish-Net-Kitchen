using FishNet.Managing.Transporting;
using FishNet.Transporting;
using FishNet.Transporting.Multipass;
using FishNet.Transporting.Tugboat;
using UnityEngine;

[ExecuteAlways]
public class NetworkSwapper : MonoBehaviour
{
    enum NetworkType { Steam, Tugboat }

    [Header("References")]
    [SerializeField] TransportManager transportManager = null;
    [SerializeField] Transport tugboat = null;
    [SerializeField] Transport fishySteamworks = null;

    [Header("Settings")]
    [SerializeField] NetworkType networkType = NetworkType.Steam;

    void Update()
    {
        Multipass mp = transportManager.GetTransport<Multipass>();
        if (networkType == NetworkType.Steam)
        {
            mp.SetClientTransport<FishySteamworks.FishySteamworks>();
            // transportManager.Transport = fishySteamworks;
        }
        else
        {
            mp.SetClientTransport<Tugboat>();
            // transportManager.Transport = tugboat;
        }
    }
    
}
