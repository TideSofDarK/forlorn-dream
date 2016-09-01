using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterBase : MonoBehaviour {

    protected SpriteRenderer spriteRenderer;

    public List<Sprite> spriteIdleSouth     = new List<Sprite>();
    public List<Sprite> spriteIdleNorth     = new List<Sprite>();
    public List<Sprite> spriteIdleWest      = new List<Sprite>();
    
    public List<Sprite> spriteRunSouth      = new List<Sprite>();
    public List<Sprite> spriteRunNorth      = new List<Sprite>();
    public List<Sprite> spriteRunWest       = new List<Sprite>();
    public List<Sprite> spriteRunNorthEast  = new List<Sprite>();
    public List<Sprite> spriteRunSouthWest  = new List<Sprite>();

    void Start () {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

    }
	
	void Update () {
	
	}
}
