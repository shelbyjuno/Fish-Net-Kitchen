using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using Unity.VisualScripting;
using UnityEngine;

public class Sink : NetworkBehaviour, IInteractable
{
    enum SinkState { Empty, Dirty, Washing, Clean }

    [Header("References")]
    [SerializeField] private Dishrack dishrack;
    [SerializeField] private ProgressWheel progressWheel;

    [Header("Sink Settings")]
    [SerializeField] private GameObject[] plateModels;
    [SerializeField] private float washTime = 5f;

    // State
    private readonly SyncVar<SinkState> state = new SyncVar<SinkState>(SinkState.Empty);
    private int currentPlates = 0;
    private float remainingWashTime = 0;

    public void Interact(Player player)
    {
        if(currentPlates > 0 && state.Value == SinkState.Dirty) WashDishesServerRpc();
        else if(currentPlates > 0 && state.Value == SinkState.Clean) SentPlateToDishrackServerRpc();
    }

    public bool CanInteract(Player player)
    {
        return currentPlates > 0 && (state.Value == SinkState.Dirty || state.Value == SinkState.Clean) && player.GetCurrentFood() == null;
    }

    public string GetInteractText(Player player)
    {
        if(state.Value == SinkState.Dirty) return "Wash plates";
        else if(state.Value == SinkState.Clean) return "Send plate to dishrack";
        return "";
    }

    public Component GetComponent() => this;

    private void Awake()
    {
        state.OnChange += OnStateChanged;
    }

    private void Update()
    {
        if(state.Value == SinkState.Washing)
        {
            remainingWashTime -= Time.deltaTime;
            progressWheel.SetProgress(remainingWashTime / (washTime * currentPlates), true);

            if(remainingWashTime <= 0 && IsServerStarted)
            {
                state.Value = SinkState.Clean;
            }
        }
    }

    private void OnStateChanged(SinkState prev, SinkState next, bool asServer)
    {
        if(next == SinkState.Empty)
        {

        }
        else if (next == SinkState.Dirty)
        {

        }
        else if (next == SinkState.Washing)
        {
            progressWheel.SetVisible(true);
        }
        else if (next == SinkState.Clean)
        {
            progressWheel.SetVisible(false);
            progressWheel.SetProgress(0);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void WashDishesServerRpc()
    {
        if(state.Value != SinkState.Dirty || currentPlates <= 0) return;
            
        state.Value = SinkState.Washing;
        remainingWashTime = washTime * currentPlates;

        WashDishesObserversRpc(remainingWashTime);
    }

    [ObserversRpc(ExcludeServer = true)]
    private void WashDishesObserversRpc(float washTime) => remainingWashTime = washTime;

    [ServerRpc(RequireOwnership = false)]
    private void SentPlateToDishrackServerRpc()
    {
        if(state.Value != SinkState.Clean || currentPlates <= 0) return;

        dishrack.AddPlate();
        currentPlates--;

        UpdateCurrentPlatesObserversRpc(currentPlates);

        if(currentPlates == 0) state.Value = SinkState.Empty;
    }



    [Server]
    public void AddPlate()
    {
        if(state.Value == SinkState.Empty) state.Value = SinkState.Dirty;

        currentPlates++;

        UpdateCurrentPlatesObserversRpc(currentPlates);
    }

    [ObserversRpc]
    private void UpdateCurrentPlatesObserversRpc(int plates)
    {
        if(!IsServerStarted) currentPlates = plates;
        UpdatePlateModels();
    }

    public bool IsClean() => state.Value == SinkState.Clean;
    public bool IsRunning() => state.Value == SinkState.Washing;

    void UpdatePlateModels()
    {
        for (int i = 0; i < plateModels.Length; i++)
            plateModels[i].SetActive(i < currentPlates);
    }
}
