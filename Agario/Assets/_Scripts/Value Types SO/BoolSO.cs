using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New BoolSo", menuName = "BoolSo")]
public class BoolSO : ScriptableObject{
    public bool boolSo;

    void Awake(){
        boolSo = false;
    }
    void OnDisable(){
        boolSo = false;
    }

    void OnEnable(){
        boolSo = false;
    }
}
