using UnityEngine;
using System.Collections;

public class FadeObstacle : MonoBehaviour
{
    [SerializeField]
    protected Camera mainCamera;

    protected SpriteRenderer spriteRenderer;

    void Start ()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
	}
	
	void FixedUpdate ()
    {
        Fade();
	}

    protected void Fade()
    {
        Camera cam = GetCamera();

        if (cam != null && spriteRenderer != null)
        {
            Color newColor = spriteRenderer.color;

            float dotProd =  Vector3.Dot(cam.transform.forward, transform.forward);
            if (dotProd > 0)
                newColor.a = 0;
            else
                newColor.a = 1;

            spriteRenderer.color = newColor;
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
