using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomizeSpriteColor : MonoBehaviour
{
    SpriteRenderer spriteRenderer;

    void Awake(){
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start(){
        RandomizeColor();
    }

    void RandomizeColor(){

        spriteRenderer.color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
    }
}
