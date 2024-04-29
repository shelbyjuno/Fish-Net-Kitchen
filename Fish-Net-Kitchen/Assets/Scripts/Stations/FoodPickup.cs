using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodPickup : MonoBehaviour, IInteractable
{
    [Header("References")]
    [SerializeField] private  Food food;
    [SerializeField] private FoodModel foodModel;

    public bool CanInteract(Player player) => player.GetCurrentFood() == null && food != null;

    public Component GetComponent() => this;

    public string GetInteractText(Player player) => $"Pick up {food?.GetColoredName()}";

    public void Interact(Player player) => player.SetCurrentFood(food);

    void Awake()
    {
        foodModel?.SetFoodModel(food.GetDefaultModel());
    }
}
