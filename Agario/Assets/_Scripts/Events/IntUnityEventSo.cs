using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "New Int Event", menuName ="Events/Unity Events/Int Event")]
public class IntUnityEventSo : MonoBehaviour
{
    public UnityEvent<int> intUnityEventSo;

    void OnEnable(){
        if (intUnityEventSo == null){
            intUnityEventSo = new UnityEvent<int>();
        }
    }
}
