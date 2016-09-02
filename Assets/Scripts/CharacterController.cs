using UnityEngine;
using System.Collections;

public class CharacterController : MonoBehaviour
{

    [SerializeField]
    protected Camera mainCamera;

    [SerializeField]
    protected float movingSpeed;

    protected Rigidbody rigidBody;

    void Start ()
    {
        rigidBody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        float verticalInput = Input.GetAxisRaw("Vertical");
        float horizontalInput = Input.GetAxisRaw("Horizontal");

        verticalInput = Mathf.Sign(verticalInput) * (Mathf.Abs(verticalInput) > 0.1 ? 1 : 0);
        horizontalInput = Mathf.Sign(horizontalInput) * (Mathf.Abs(horizontalInput) > 0.1 ? 1 : 0);

        if (verticalInput != 0 || horizontalInput != 0)
        {
            Camera cam = GetCamera();

            Vector3 forward = cam.transform.TransformDirection(Vector3.forward);
            Vector3 right = cam.transform.TransformDirection(Vector3.right);

            Vector3 velocity = Vector3.zero;

            velocity = forward * verticalInput + right * horizontalInput;
            velocity.y = 0;

            rigidBody.velocity = velocity.normalized * movingSpeed;
        }
        else
        {
            rigidBody.velocity = Vector3.zero;
        }
    }

    protected Camera GetCamera()
    {
        if (mainCamera != null)
            return mainCamera;
        else
            return Camera.main;
    }
}
