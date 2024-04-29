using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerNetworkEvent playerNetworkEvent;
    public Player player;
    public Transform forward;
    public TextMeshProUGUI interactText;

    [Header("Settings")]
    public KeyCode interactKey = KeyCode.E;
    public float interactDistance = 2.0f;

    void Awake()
    {
        playerNetworkEvent.OnStartClientEvent.AddListener((isOwner) =>
        {
            enabled = isOwner;
        });
    }
    
    void Update()
    {
        if(!GetInteractable(out IInteractable interactable) || !interactable.CanInteract(player))
        {
            interactText.SetText("");
            return;
        }

        interactText.SetText(interactable.GetInteractText(player));
        
        if(Input.GetKeyDown(interactKey))
        {
            interactable.Interact(player);
        }
    }

    private bool GetInteractable(out IInteractable interactable)
    {
        interactable = null;

        RaycastHit hit;
        if(Physics.Raycast(forward.position, forward.forward, out hit, interactDistance))
        {
            if(hit.collider.TryGetComponent(out IInteractable hitInteractable))
            {
                interactable = hitInteractable;
                return true;
            }
        }

        return false;
    }
}
