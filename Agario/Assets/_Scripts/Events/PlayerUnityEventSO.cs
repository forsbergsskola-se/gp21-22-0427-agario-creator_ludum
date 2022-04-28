using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "New Player Unity Event", menuName = "Events/Unity Events/Player Event")]
public class PlayerUnityEventSO : ScriptableObject
{
    public UnityEvent<Player> playerUnityEventSo;

    void OnEnable(){
        if (playerUnityEventSo == null){
            playerUnityEventSo = new UnityEvent<Player>();
        }
    }
}
