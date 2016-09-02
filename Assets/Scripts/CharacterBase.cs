using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterBase : MonoBehaviour
{
    [SerializeField]
    protected Camera mainCamera;

    [SerializeField]
    protected float runSpeedThreshold;

    protected Animator animator;
    protected Rigidbody rigidBody;

    protected float northDir = -1;
    protected float eastDir = 0;

    void Start ()
    {
        animator = GetComponent<Animator>();
        rigidBody = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        UpdateAnimations();
    }

    protected void UpdateAnimations()
    {
        Vector3 velocity = rigidBody.velocity;
        float speed = velocity.magnitude;

        if (speed > runSpeedThreshold)
        {
            Camera cam = GetCamera();

            Vector3 forward = cam.transform.TransformDirection(Vector3.forward);
            Vector3 right = cam.transform.TransformDirection(Vector3.right);

            northDir = Vector3.Dot(velocity, forward);
            eastDir = Vector3.Dot(velocity, right);

            northDir = Mathf.Sign(northDir) * (Mathf.Abs(northDir) > 0.1 ? 1 : 0);
            eastDir = Mathf.Sign(eastDir) * (Mathf.Abs(eastDir) > 0.1 ? 1 : 0);
        }

        if (animator != null)
        {   
            animator.SetBool("is_running", (speed > runSpeedThreshold));
            animator.SetFloat("move_speed", speed);
            animator.SetFloat("north_dir", northDir);
            animator.SetFloat("east_dir", eastDir);
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
