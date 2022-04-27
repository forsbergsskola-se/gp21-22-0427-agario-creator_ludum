using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
[CreateAssetMenu(fileName = "New Unity Event", menuName = "Events/Unity Event")]
public class UnityEventSO : ScriptableObject{
    public UnityEvent<String> unityEventSo;

    void OnEnable(){
        if (unityEventSo == null){
            unityEventSo = new UnityEvent<String>();
        }
    }
}
