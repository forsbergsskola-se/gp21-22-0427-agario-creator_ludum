using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "New Unity Event", menuName = "Events/Unity Events/Unity Event")]
public class UnityEventSO : ScriptableObject{
    public UnityEvent unityEventSo;

    void Awake(){
        DontDestroyOnLoad(this);
    }

    void OnEnable(){
        if (unityEventSo == null){
            unityEventSo = new UnityEvent();
        }
    }
}
