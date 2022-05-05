using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetMapSize : MonoBehaviour{
    [SerializeField] Vector2SO mapSizeSo;

    void Awake(){
        transform.localScale = mapSizeSo.vector2;
    }
}
