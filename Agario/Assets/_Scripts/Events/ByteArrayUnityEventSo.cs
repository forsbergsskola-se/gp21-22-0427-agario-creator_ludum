using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "New ByteArray Event", menuName ="Events/Unity Events/ByteArray Event")]
public class ByteArrayUnityEventSo : ScriptableObject
{
    public UnityEvent<byte[]> dataUnityEventSo;

    void OnEnable(){
        if (dataUnityEventSo == null){
            dataUnityEventSo = new UnityEvent<byte[]>();
        }
    }
}
