using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

public class Player : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerNetworkEvent playerNetworkEvent;
    [SerializeField] private Food currentFood;
    [SerializeField] private Plate currentPlate;
    [SerializeField] private FoodModel foodModel;

    private readonly SyncVar<Food> CurrentFood = new SyncVar<Food>();

    public Food GetCurrentFood() => currentFood;

    public void SetCurrentFood(Food food)
    {
        currentFood = food;
        foodModel.SetFoodModel(food?.GetDefaultModel());

        if(IsOwner) SetServerFood(food);
    }

    void Awake() => CurrentFood.OnChange += OnServerFoodChanged;

    [ServerRpc] private void SetServerFood(Food value) => CurrentFood.Value = value;

    void OnServerFoodChanged(Food oldValue, Food newValue, bool asServer)
    {
        if(!IsOwner) SetCurrentFood(newValue);
    }
}
