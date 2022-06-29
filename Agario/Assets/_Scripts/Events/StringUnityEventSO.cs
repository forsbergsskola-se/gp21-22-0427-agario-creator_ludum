using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
[CreateAssetMenu(fileName = "New String Unity Event", menuName = "Events/Unity Events/String Unity Event")]
public class StringUnityEventSO : ScriptableObject{
    public UnityEvent<String> stringUnityEventSo;

    void OnEnable(){
        if (stringUnityEventSo == null){
            stringUnityEventSo = new UnityEvent<String>();
        }
    }
}
