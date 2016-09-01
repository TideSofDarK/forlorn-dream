using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterBase : MonoBehaviour
{
    protected Animator animator;
    protected Rigidbody rigidBody;

    protected bool isMoving = false;
    protected float northDir = -1;
    protected float eastDir  = 0;

    [SerializeField]
    protected float movingSpeed;

    void Start ()
    {
        animator = GetComponent<Animator>();
        rigidBody = GetComponent<Rigidbody>();
    }
	
	void Update ()
    {
        float verticalInput   = Input.GetAxisRaw("Vertical");
        float horizontalInput = Input.GetAxisRaw("Horizontal");        

        if (verticalInput != 0 || horizontalInput != 0)
        {
            northDir = Mathf.Sign(verticalInput) * (Mathf.Abs(verticalInput) > 0.1 ? 1 : 0);
            eastDir = Mathf.Sign(horizontalInput) * (Mathf.Abs(horizontalInput) > 0.1 ? 1 : 0);

            Vector3 velocity = rigidBody.velocity;
            velocity.z = northDir;
            velocity.x = eastDir;
            rigidBody.velocity = velocity.normalized * movingSpeed;

            isMoving = true;
        }
        else
        {
            rigidBody.velocity = Vector3.zero;

            isMoving = false;
        }
	}

    void FixedUpdate()
    {
        UpdateAnimations();
    }

    protected void UpdateAnimations()
    {
        if (animator != null)
        {
            animator.SetBool("is_running", isMoving);
            animator.SetFloat("move_speed", movingSpeed);
            animator.SetFloat("north_dir", northDir);
            animator.SetFloat("east_dir", eastDir);
        }
    }
}
