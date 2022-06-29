using System.Collections;
using System.Collections.Generic;
using AgarioShared;
using UnityEngine;
using UnityEngine.Events;
[CreateAssetMenu(fileName = "New Player Info Unity Event", menuName = "Events/Unity Events/Player Info Event")]
public class PlayerInfoUnityEventSo : ScriptableObject
{
    public UnityEvent<PlayerInfo> playerInfoUnityEventSo;
    
    void Awake(){
        if (playerInfoUnityEventSo == null){
            playerInfoUnityEventSo = new UnityEvent<PlayerInfo>();
        }
    }
}
