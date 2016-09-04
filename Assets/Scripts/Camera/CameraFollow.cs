using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour
{
    [SerializeField]
    protected Transform targetToFollow;

    protected Vector3 positionRelToTarget;

    protected float currentAngle;
    protected float targetAngle;
    protected float rotationSmoothness;
    protected bool isRotating = false;

    void Start()
    {
        CalcRelativePosition();
    }


    void Update()
    {
        FollowTarget();
        Rotate();
    }

    public void SetFollowTarget(Transform target)
    {
        targetToFollow = target;
        CalcRelativePosition();
        currentAngle = 0;
    }

    public void RotateAroundTargetSmooth(float angle, float smoothness)
    {
        if (targetToFollow != null)
        {
            if (!isRotating)
                targetAngle = currentAngle + angle;

            rotationSmoothness = smoothness;

            isRotating = true;
        }
    }

    protected void SetRotationAngle(float angle)
    {
        if (targetToFollow != null)
        {
            float angleDiff = angle - currentAngle;

            Quaternion rotation = Quaternion.AngleAxis(angleDiff, new Vector3(0, 1, 0));
            positionRelToTarget = rotation * positionRelToTarget;

            currentAngle = angle;
        }
    }

    protected void Rotate()
    {
        if (isRotating)
        {
            float newAngle = Mathf.Lerp(currentAngle, targetAngle, rotationSmoothness);
            SetRotationAngle(newAngle);

            if (Mathf.Abs(currentAngle - targetAngle) < 0.1)
            {
                SetRotationAngle(targetAngle);
                isRotating = false;
            }
        }
    }

    protected void FollowTarget()
    {
        if (targetToFollow != null)
        {
            transform.position = targetToFollow.position + positionRelToTarget;
            transform.LookAt(targetToFollow);
        }
    }

    protected void CalcRelativePosition()
    {
        if (targetToFollow != null)
        {
            positionRelToTarget = transform.position - targetToFollow.position;
        }
    }
}
