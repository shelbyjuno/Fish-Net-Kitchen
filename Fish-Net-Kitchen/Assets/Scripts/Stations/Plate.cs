using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

public class Plate : NetworkBehaviour, IInteractable
{
    enum PlateState { Clean, Dirty, InSink }
    [Header("Referernces")]
    [SerializeField] PlayerFoodManager playerFoodManager;
    [SerializeField] Sink sink;
    [SerializeField] Customer customer;
    [SerializeField] GameObject plateModel;

    [Header("Settings")]
    [SerializeField] private Vector3 foodStartOffset = new Vector3(0, 0.1f, 0);

    // State
    private readonly SyncVar<PlateState> state = new SyncVar<PlateState>(PlateState.InSink);
    private readonly SyncList<Food> foods = new SyncList<Food>();
    private List<FoodModel> foodModels = new List<FoodModel>();

    public bool CanInteract(Player player)
    {
        if(player.GetCurrentFood() != null && state.Value == PlateState.Clean) return true;
        if(foods.Count > 0 && state.Value == PlateState.Clean) return true;
        else if(state.Value == PlateState.Dirty && !sink.IsClean() && !sink.IsRunning() && player.GetCurrentFood() == null) return true;
        return false;
    }

    public Component GetComponent() => this;

    public string GetInteractText(Player player)
    {
        if(player.GetCurrentFood() != null && state.Value == PlateState.Clean) return $"Place {player.GetCurrentFood().GetColoredName()} on plate";
        if(foods.Count > 0 && state.Value == PlateState.Clean) return $"Give plate\n({string.Join(", ", foods.GetCollection(false).ConvertAll(f => f.GetColoredName()).ToArray())})";
        else if (state.Value == PlateState.Dirty) return "Return plate to sink";
        return "";
    }

    public void Interact(Player player)
    {
        if(player.GetCurrentFood() != null && state.Value == PlateState.Clean) PlaceFoodServerRpc(player, player.GetCurrentFood());
        else if(foods.Count > 0 && state.Value == PlateState.Clean) ServePlateServerRpc(player);
        else if(state.Value == PlateState.Dirty) ReturnPlateToSinkServerRpc();
    }
    void Awake()
    {
        state.OnChange += OnStateChanged;
        foods.OnChange += OnFoodsChanged;
    }

    private void OnFoodsChanged(SyncListOperation op, int index, Food oldItem, Food newItem, bool asServer)
    {
        if(asServer) return;

        if(op == SyncListOperation.Clear)
        {
            foreach(var m in foodModels)
                Destroy(m.gameObject);
            foodModels.Clear();
            return;
        }
        else if (op == SyncListOperation.Add)
        {
            var m = new GameObject().AddComponent<FoodModel>();

            GameObject foodModel = newItem.GetPlateable().GetPlateModel();
            Bun bun = newItem.GetComponent<Bun>();

            if(bun != null)
            {
                if(!HasCorrespondingBun()) foodModel = bun.GetBottomBun();
                else foodModel = bun.GetTopBun();
            }

            m.SetFoodModel(foodModel);
            m.transform.SetParent(transform);
            m.transform.localPosition = GetFoodPosition(foods.Count - 1);
            foodModels.Add(m);
        }
    }

    private void OnStateChanged(PlateState prev, PlateState next, bool asServer)
    {
        if(next == PlateState.InSink) plateModel.SetActive(false);
        else plateModel.SetActive(true);
    }

    void Start()
    {
        plateModel.SetActive(false);
    }

    [Server]
    public void ReturnPlateToWindow()
    {
        state.Value = PlateState.Clean;
    }

    [ServerRpc(RequireOwnership = false)]
    private void ReturnPlateToSinkServerRpc()
    {
        if(state.Value != PlateState.Dirty) return;

        state.Value = PlateState.InSink;
        sink.AddPlate();
    }
    
    [ServerRpc(RequireOwnership = false)]
    private void ServePlateServerRpc(Player player)
    {
        if(state.Value != PlateState.Clean) return;

        customer.ServeFood(foods.GetCollection(true));
        foods.Clear();

        state.Value = PlateState.Dirty;
    }

    [ServerRpc(RequireOwnership = false)]
    private void PlaceFoodServerRpc(Player player, Food food)
    {
        if(state.Value != PlateState.Clean) return;
        
        playerFoodManager.SetPlayerFood(player, null);
        foods.Add(food);
    }

    public bool HasCorrespondingBun()
    {
        int bunCount = 0;
        foreach(var f in foods.GetCollection(false))
        {
            if(f.GetComponent<Bun>()) bunCount++;
        }

        return bunCount % 2 == 0;
    }

    private Vector3 GetFoodPosition(int index)
    {
        Vector3 pos = foodStartOffset;
        for(int i = 0; i < index; i++)
            pos += foods[i].GetSize();

        return pos;
    }

    public bool IsInSink() => state.Value == PlateState.InSink;
}