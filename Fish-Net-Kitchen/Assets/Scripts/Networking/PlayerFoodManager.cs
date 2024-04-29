using System.Collections;
using System.Collections.Generic;
using FishNet.Connection;
using FishNet.Object;
using UnityEngine;

public class PlayerFoodManager : NetworkBehaviour
{
    public void SetPlayerFood(Player player, Food value)
    {
        if(IsServerStarted) SetPlayerFoodTargetRpc(player.Owner, player, value);
        else SetPlayerFoodServerRpc(player, value);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerFoodServerRpc(Player player, Food value)
    {
        SetPlayerFoodTargetRpc(player.Owner, player, value);
    }

    [TargetRpc]
    private void SetPlayerFoodTargetRpc(NetworkConnection conn, Player player, Food value)
    {
        player.SetCurrentFood(value);
    }
}
