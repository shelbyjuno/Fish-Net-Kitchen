using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private PlayerNetworkEvent playerNetworkEvent;
    [SerializeField] private Transform playerBody;
    [SerializeField] private float xSensitivity = 3.0f;
    [SerializeField] private float ySensitivity = 3.0f;

    private float xRotation = 0.0f;

    void Awake()
    {
        playerNetworkEvent.OnStartClientEvent.AddListener((isOwner) =>
        {
            GetComponent<Camera>().enabled = isOwner;
            GetComponent<AudioListener>().enabled = isOwner;
            enabled = isOwner;
        });
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void OnEnable()
    {
        xRotation = 0.0f;
    }
    
    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * xSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * ySensitivity;
        
        xRotation = Mathf.Clamp(xRotation + mouseY, -90.0f, 90.0f);
        transform.localRotation = Quaternion.Euler(-xRotation, 0.0f, 0.0f);

        playerBody.Rotate(Vector3.up * mouseX);
    }

}
