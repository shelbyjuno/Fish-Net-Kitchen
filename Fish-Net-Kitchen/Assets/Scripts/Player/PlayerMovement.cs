using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private PlayerNetworkEvent playerNetworkEvent;
    [SerializeField] private CharacterController controller;
    private Vector3 playerVelocity;

    [SerializeField] private float playerSpeed = 2.0f;

    void Awake()
    {
        playerNetworkEvent.OnStartClientEvent.AddListener((isOwner) =>
        {
            enabled = isOwner;
        });
    }

    void Start()
    {
        if(TryGetComponent(out CharacterController characterController)) controller = characterController;
        else controller = gameObject.AddComponent<CharacterController>();
    }

    void Update()
    {        
        Vector3 move = transform.TransformDirection(new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"))).normalized;

        if(move != Vector3.zero) controller.Move(move * Time.deltaTime * playerSpeed);
    }
}
