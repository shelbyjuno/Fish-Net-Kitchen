public interface IInteractable
{   
    public void Interact(Player player);
    public bool CanInteract(Player player);
    public string GetInteractText(Player player);
    public UnityEngine.Component GetComponent();
}
