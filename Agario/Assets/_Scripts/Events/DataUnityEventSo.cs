using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "New Data Event", menuName ="Events/Unity Events/Data Event")]
public class DataUnityEventSo : ScriptableObject
{
    public UnityEvent<Byte[][]> dataUnityEventSo;

    void OnEnable(){
        if (dataUnityEventSo == null){
            dataUnityEventSo = new UnityEvent<Byte[][]>();
        }
    }
}
