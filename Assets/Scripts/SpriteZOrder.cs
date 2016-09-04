using UnityEngine;
using System.Collections;

public class SpriteZOrder : MonoBehaviour {

    [SerializeField]
    protected SpriteRenderer spriteRenderer;

    public bool isMoving = false;

    void Start () {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
	
	void Update () {
        if (spriteRenderer) {
            
            int pos = Mathf.RoundToInt(Vector3.Distance(transform.position, Camera.main.transform.position) * 10f);
            pos /= 1;
            spriteRenderer.sortingOrder = (pos * -1); // + (isMoving ? 10 : 0)
        }
    }
}
