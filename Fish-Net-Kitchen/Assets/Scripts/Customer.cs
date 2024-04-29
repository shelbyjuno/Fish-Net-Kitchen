using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using TMPro;
using UnityEngine;

public class Customer : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] Food bun;
    [SerializeField] Food[] availableFoods;
    [SerializeField] TextMeshProUGUI orderText;

    [Header("Settings")]
    [SerializeField] private int minOrderSize = 3;
    [SerializeField] private int maxOrderSize = 5;

    [SerializeField] private int minBurgerSize = 1;
    [SerializeField] private int maxBurgerSize = 3;
    private List<Food> order = new List<Food>();

    public override void OnStartClient()
    {
        base.OnStartClient();

        if(!IsServerStarted) return;

        SetOrderServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetOrderServerRpc()
    {
        order.Clear();
        
        int burgerSize = UnityEngine.Random.Range(minBurgerSize, maxBurgerSize + 1);

        for(int i = 0; i < burgerSize; i++)
        {
            order.Add(bun);
            int orderSize = UnityEngine.Random.Range(minOrderSize, maxOrderSize + 1);

            for(int j = 0; j < orderSize; j++)
            {
                order.Add(availableFoods[UnityEngine.Random.Range(0, availableFoods.Length)]);
            }

        }
        order.Add(bun);

        SetOrderObserversRpc(order);
    }

    [ObserversRpc]
    private void SetOrderObserversRpc(List<Food> o)
    {
        if(!IsServerStarted) order = o;
        UpdateOrderText();
    }

    [Server]
    public void ServeFood(List<Food> foods)
    {
        bool correctOrder = CheckOrder(foods);
        ServeFoodObserversRpc(correctOrder);
        SetOrderServerRpc();
    }

    [ObserversRpc]
    private void ServeFoodObserversRpc(bool correct)
    {
        Debug.Log($"Correct order: {correct}");
    }

    private bool CheckOrder(List<Food> foods)
    {
        if (foods.Count != order.Count) return false;

        for (int i = 0; i < foods.Count; i++)
            if (foods[i] != order[i])
                return false;

        return true;
    }

    private void UpdateOrderText()
    {
        orderText.SetText(string.Join(", ", order.ConvertAll(food => food.GetColoredName())));
    }
}
