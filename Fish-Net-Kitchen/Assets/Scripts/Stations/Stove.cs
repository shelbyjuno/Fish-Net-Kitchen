using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

public class Stove : NetworkBehaviour, IInteractable
{
    enum StoveState { Empty, Cooking, Done }

    [Header("References")]
    public FoodModel foodModel;
    public PlayerFoodManager playerFoodManager;
    public ProgressWheel progressWheel;

    // State
    private readonly SyncVar<StoveState> state = new SyncVar<StoveState>();
    private readonly SyncVar<Food> currentFood = new SyncVar<Food>();

    private float currentCookTime = 0;
    private float remainingCookTime = 0;

    public void Interact(Player player)
    {
        if (state.Value == StoveState.Empty)
        {
            FillStoveServerRpc(player, player.GetCurrentFood());
        }
        else if (state.Value == StoveState.Done)
        {
            EmptyStoveServerRpc(player);
        }
    }

    public bool CanInteract(Player player)
    {
        Food food = player.GetCurrentFood();
        if(state.Value == StoveState.Empty) return food != null && food.IsCookable();
        else if(state.Value == StoveState.Cooking) return false;
        else if(state.Value == StoveState.Done) return player.GetCurrentFood() == null;

        return false;
    }

    public string GetInteractText(Player player)
    {
        if(state.Value == StoveState.Empty) return $"Cook {player.GetCurrentFood().GetColoredName()}";
        else if(state.Value == StoveState.Done) return $"Pick up {currentFood.Value.GetCookable().GetCookedFood().GetColoredName()}";

        return "";
    }

    public Component GetComponent() => this;

    private void Awake()
    {
        state.OnChange += OnStateChange;
    }

    private void OnStateChange(StoveState prev, StoveState next, bool asServer)
    {
        if(next == StoveState.Empty)
        {
            // Update model
            foodModel.SetFoodModel(null);

            // Reset wheel
            progressWheel.SetVisible(false);
            progressWheel.SetProgress(0);

            // Reset state
            currentCookTime = 0;
            remainingCookTime = 0;
        }
        else if (next == StoveState.Cooking)
        {
            Cookable cookableFood = currentFood.Value.GetCookable();

            foodModel.SetFoodModel(cookableFood.GetRawModel());
            progressWheel.SetVisible(true);
        }
        else if (next == StoveState.Done)
        {
            foodModel.SetFoodModel(currentFood.Value.GetCookable().GetCookedModel());
            progressWheel.SetProgress(1);
            progressWheel.SetVisible(false);
        }
    }

    private void Update()
    {
        if (state.Value == StoveState.Cooking)
        {
            remainingCookTime -= Time.deltaTime;
            progressWheel.SetProgress((remainingCookTime / currentCookTime), true);

            if (remainingCookTime <= 0 && IsServerStarted)
            {
                state.Value = StoveState.Done;
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void FillStoveServerRpc(Player player, Food food)
    {
        if(state.Value != StoveState.Empty) return;
        
        playerFoodManager.SetPlayerFood(player, null);

        currentFood.Value = food;
        Cookable cookableFood = currentFood.Value.GetCookable();

        foodModel.SetFoodModel(cookableFood.GetRawModel());
        progressWheel.SetVisible(true);

        currentCookTime = Random.Range(cookableFood.GetMinCookTime(), cookableFood.GetMaxCookTime());
        remainingCookTime = currentCookTime;
        SetCookTimeObserversRpc(currentCookTime);

        state.Value = StoveState.Cooking;
    }

    [ObserversRpc(ExcludeServer = true)]
    private void SetCookTimeObserversRpc(float cookTime)
    {
        currentCookTime = cookTime;
        remainingCookTime = cookTime;
    }

    [ServerRpc(RequireOwnership = false)]
    private void EmptyStoveServerRpc(Player player)
    {
        if(state.Value != StoveState.Done) return;

        playerFoodManager.SetPlayerFood(player, currentFood.Value.GetCookable().GetCookedFood());
        
        // Reset state
        currentFood.Value = null;
        state.Value = StoveState.Empty;
    }
}
