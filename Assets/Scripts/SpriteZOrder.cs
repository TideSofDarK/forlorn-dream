using UnityEngine;
using System.Collections;

public class SpriteZOrder : MonoBehaviour {

    [SerializeField]
    protected SpriteRenderer spriteRenderer;

    void Start () {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
	
	void Update () {
        if (spriteRenderer)
        {
            spriteRenderer.sortingOrder = Mathf.RoundToInt(transform.position.z * 100f) * -1;
        }
    }
}
