using UnityEngine;
using System.Collections;

public class CharacterController : MonoBehaviour
{

    [SerializeField]
    protected Camera mainCamera;

    [SerializeField]
    protected float movingSpeed;

    protected Rigidbody rigidBody;
    protected CharacterBase characterBase;

    void Start ()
    {
        rigidBody = GetComponent<Rigidbody>();
        characterBase = GetComponent<CharacterBase>();

        Camera.main.transparencySortMode = TransparencySortMode.Orthographic;
    }

    void Update()
    {
        Camera cam = GetCamera();

        float verticalInput = Input.GetAxisRaw("Vertical");
        float horizontalInput = Input.GetAxisRaw("Horizontal");

        verticalInput = Mathf.Sign(verticalInput) * (Mathf.Abs(verticalInput) > 0.1 ? 1 : 0);
        horizontalInput = Mathf.Sign(horizontalInput) * (Mathf.Abs(horizontalInput) > 0.1 ? 1 : 0);

        if (verticalInput != 0 || horizontalInput != 0)
        {
            Vector3 forward = cam.transform.TransformDirection(Vector3.forward);
            Vector3 right = cam.transform.TransformDirection(Vector3.right);

            Vector3 velocity = Vector3.zero;

            velocity = forward * verticalInput + right * horizontalInput;
            velocity.y = 0;

            rigidBody.velocity = velocity.normalized * movingSpeed;

            characterBase.SetLookDirection(velocity.normalized);
        }
        else
        {
            rigidBody.velocity = Vector3.zero;
        }

        bool rotateLeft  = Input.GetKey(KeyCode.Q);
        bool rotateRight = Input.GetKey(KeyCode.E);

        if (rotateLeft || rotateRight)
        {
            CameraFollow cameraFollow = cam.GetComponent<CameraFollow>();

            if (cameraFollow != null)
            {
                if (rotateLeft)
                    cameraFollow.RotateAroundTargetSmooth(-45, 180);
                if (rotateRight)
                    cameraFollow.RotateAroundTargetSmooth(45, 180);
            }
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
