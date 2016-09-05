using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterBase : MonoBehaviour
{
    [SerializeField]
    protected Camera mainCamera;

    [SerializeField]
    protected float runSpeedThreshold = 0.09f;

    [SerializeField]
    protected float walkSpeedThreshold = 0.04f;

    protected Animator animator;
    protected Rigidbody rigidBody;

    protected Vector3 lookDirection = new Vector3(0, 0, 1);
    protected int directionsNumber = 8;

    void Start ()
    {
        animator = GetComponent<Animator>();
        rigidBody = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        UpdateAnimations();
    }

    public void SetLookDirection(Vector3 lookDirectionNew)
    {
        lookDirection = lookDirectionNew;
    }

    protected void UpdateAnimations()
    {
        Camera cam = GetCamera();

        Vector3 forward = cam.transform.TransformDirection(Vector3.forward);
        Vector3 right = cam.transform.TransformDirection(Vector3.right);

        float lookForward = Vector3.Dot(lookDirection, forward);
        float lookRight = Vector3.Dot(lookDirection, right);
        float lookAngle = Mathf.Atan2(lookForward, lookRight);

        lookAngle = (Mathf.PI * 2 / directionsNumber) * 
                     Mathf.Round(lookAngle / (Mathf.PI * 2 / directionsNumber));

        lookForward = Mathf.Sin(lookAngle);
        lookRight = Mathf.Cos(lookAngle);

        lookForward = Mathf.Sign(lookForward) * (Mathf.Abs(lookForward) > 0.1 ? 1 : 0);
        lookRight = Mathf.Sign(lookRight) * (Mathf.Abs(lookRight) > 0.1 ? 1 : 0);

        Vector3 velocity = rigidBody.velocity;
        float speed = velocity.magnitude;

        if (animator != null)
        {
            animator.SetBool("is_running", (speed > runSpeedThreshold));
            animator.SetBool("is_walking", (speed > walkSpeedThreshold));
            animator.SetFloat("move_speed", speed);
            animator.SetFloat("north_dir", lookForward);
            animator.SetFloat("east_dir", lookRight);
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
