using System.Collections;
using System.Collections.Generic;
using AgarioShared;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "New Orb Unity Event", menuName = "Events/Unity Events/Orb Info Event")]
public class OrbInfoUnityEventSo : ScriptableObject
{
    public UnityEvent<OrbInfo> eventSo;
    
    void Awake(){
        if (eventSo == null){
            eventSo = new UnityEvent<OrbInfo>();
        }
    }
}
