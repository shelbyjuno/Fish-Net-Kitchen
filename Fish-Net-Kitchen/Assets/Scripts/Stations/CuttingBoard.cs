using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using Unity.VisualScripting;
using UnityEngine;

public class CuttingBoard : NetworkBehaviour, IInteractable
{
    enum CuttingBoardState { Empty, Full };
    
    [Header("References")]
    public FoodModel foodModel;
    public PlayerFoodManager playerFoodManager;
    public ProgressWheel progressWheel;

    // State
    private readonly SyncVar<CuttingBoardState> state = new SyncVar<CuttingBoardState>();
    private int currentChopCount = 0;
    private int currentChopsRemaining = 0;
    private Food currentFood;

    public void Interact(Player player)
    {
        // If the cutting board is empty, fill it
        if (state.Value == CuttingBoardState.Empty)
        {
            FillCuttingBoardServerRpc(player, player.GetCurrentFood());
        }
        else if (state.Value == CuttingBoardState.Full && currentChopsRemaining > 0)
        {
            ChopIngredientServerRpc();
        }
        else if (state.Value == CuttingBoardState.Full && currentChopsRemaining <= 0)
        {
            PickupIngredientServerRpc(player);
        }
    }

    public bool CanInteract(Player player)
    {
        if(state.Value == CuttingBoardState.Empty) return player.GetCurrentFood() != null && player.GetCurrentFood().IsChoppable();
        else if(state.Value == CuttingBoardState.Full && currentChopsRemaining > 0  && player.GetCurrentFood() == null) return true;
        else if(state.Value == CuttingBoardState.Full && currentChopsRemaining <= 0 && player.GetCurrentFood() == null) return true;
        return false;
    }

    public Component GetComponent() => this;

    public string GetInteractText(Player player)
    {
        switch (state.Value)
        {
            case CuttingBoardState.Empty:
                return $"Place {player.GetCurrentFood().GetColoredName()} on cutting board";
            case CuttingBoardState.Full:
                return currentChopsRemaining > 0 ? $"Chop {currentFood.GetColoredName()}" 
                                                    : $"Pick up {currentFood.GetChoppable().GetChoppedFood().GetColoredName()}";
            default:
                return "Interact";
        }
    }

    void Awake()
    {
        state.OnChange += OnStateChange;
    }

    private void OnStateChange(CuttingBoardState prev, CuttingBoardState next, bool asServer)
    {
        UpdateFoodModel();

        if(next == CuttingBoardState.Full)
        {
            progressWheel.SetVisible(true);
        }
        else
        {
            progressWheel.SetVisible(false);
            progressWheel.SetProgress(0);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void PickupIngredientServerRpc(Player player)
    {
        if(state.Value != CuttingBoardState.Full) return;

        playerFoodManager.SetPlayerFood(player, currentFood.GetChoppable().GetChoppedFood());

        state.Value = CuttingBoardState.Empty;
        currentChopCount = 0;
        currentChopsRemaining = 0;

        PickupIngredientObserverRpc();
    }

    [ObserversRpc(ExcludeServer = true)]
    private void PickupIngredientObserverRpc()
    {
        currentChopCount = 0;
        currentChopsRemaining = 0;
    }

    [ServerRpc(RequireOwnership = false)]
    private void ChopIngredientServerRpc()
    {
        if(state.Value != CuttingBoardState.Full) return;

        currentChopsRemaining = Mathf.Max(0, currentChopsRemaining - 1);

        ChopIngredientClientRpc(currentChopsRemaining);
    }

    [ObserversRpc]
    private void ChopIngredientClientRpc(int remaining)
    {
        if(!IsServerStarted) currentChopsRemaining = remaining;

        progressWheel.SetProgress((float)currentChopsRemaining / currentChopCount, true);

        UpdateFoodModel();
    }

    [ServerRpc(RequireOwnership = false)]
    private void FillCuttingBoardServerRpc(Player player, Food food)
    {
        if(state.Value != CuttingBoardState.Empty) return;

        playerFoodManager.SetPlayerFood(player, null);

        currentChopCount = Random.Range(food.GetChoppable().GetMinChop(), food.GetChoppable().GetMaxChop());
        currentChopsRemaining = currentChopCount;
        currentFood = food;

        state.Value = CuttingBoardState.Full;

        FillCuttingBoardClientRpc(food, currentChopCount);
    }

    [ObserversRpc(ExcludeServer = true)]
    private void FillCuttingBoardClientRpc(Food food, int chops)
    {
        currentChopCount = chops;
        currentChopsRemaining = currentChopCount;
        currentFood = food;
    }

    public void UpdateFoodModel()
    {
        if(state.Value == CuttingBoardState.Empty)
        {
            foodModel.SetFoodModel(null);
        }
        else if(state.Value == CuttingBoardState.Full)
        {
            Choppable choppable = currentFood.GetChoppable();
            if(currentChopsRemaining > currentChopCount / 2) foodModel.SetFoodModel(choppable.GetNormalModel());
            else if(currentChopsRemaining > 0) foodModel.SetFoodModel(choppable.GetPartiallyChoppedModel());
            else foodModel.SetFoodModel(choppable.GetFullyChoppedModel());
        }
    }
}