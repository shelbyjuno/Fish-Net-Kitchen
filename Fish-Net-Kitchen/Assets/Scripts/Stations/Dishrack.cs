using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

public class Dishrack : NetworkBehaviour, IInteractable
{
    [Header("References")]
    [SerializeField] private Plate plate;
    [SerializeField] private GameObject[] plateModels;

    private int currentCleanPlates = 0;

    public bool CanInteract(Player player) => currentCleanPlates > 0 && plate.IsInSink() && player.GetCurrentFood() == null;

    public Component GetComponent() => this;

    public string GetInteractText(Player player) => "Send plate to window";

    public void Interact(Player player)
    {
        ReturnPlateToWindowServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void ReturnPlateToWindowServerRpc()
    {
        if(currentCleanPlates <= 0) return;

        plate.ReturnPlateToWindow();
        currentCleanPlates--;

        UpdateCleanPlatesObserversRpc(currentCleanPlates);
    }
    
    [ObserversRpc]
    private void UpdateCleanPlatesObserversRpc(int cleanPlates)
    {
        if(!IsServerStarted) currentCleanPlates = cleanPlates;
        UpdatePlateModels();
    }

    void Start()
    {
        currentCleanPlates = plateModels.Length;
    }

    void UpdatePlateModels()
    {
        for (int i = 0; i < plateModels.Length; i++)
            plateModels[i].SetActive(i < currentCleanPlates);
    }

    [Server]
    public void AddPlate()
    {
        currentCleanPlates++;
        
        UpdateCleanPlatesObserversRpc(currentCleanPlates);
    }
}
