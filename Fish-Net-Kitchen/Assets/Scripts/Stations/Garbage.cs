using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Garbage : MonoBehaviour, IInteractable
{
    public bool CanInteract(Player player) => player.GetCurrentFood() != null;

    public Component GetComponent() => this;

    public string GetInteractText(Player player) => $"Throw away {player.GetCurrentFood().GetColoredName()}";

    public void Interact(Player player) => player.SetCurrentFood(null);
}
