using System;
using System.Collections;
using System.Collections.Generic;
using AgarioShared;
using Unity.Mathematics;
using UnityEngine;

public class GlobalOrbManager : MonoBehaviour{
    [SerializeField] GameObject orbPrefab;
    [SerializeField] OrbInfoUnityEventSo newOrbInfoFromServerEventSo;
    [SerializeField] OrbInfoUnityEventSo orbDeathSo;
    [SerializeField] IntUnityEventSo maxOrbsAllowedEventSo;
    public Orb[] activeOrbArray;


    void Awake(){
        DontDestroyOnLoad(this);
    }

    void Start(){
        maxOrbsAllowedEventSo.intUnityEventSo.AddListener(SetMaxOrbLimit);
        newOrbInfoFromServerEventSo.eventSo.AddListener(HandleNewOrbInfo);
        orbDeathSo.eventSo.AddListener(DestroyOrb);
    }

    void DestroyOrb(OrbInfo _orbInfo){
        Debug.Log($"Destroying: Orb ({_orbInfo.id})...");
        Destroy(activeOrbArray[_orbInfo.id].gameObject);
        Debug.Log($"Destroyed: Orb ({_orbInfo.id}).");
        activeOrbArray[_orbInfo.id] = default;
    }

    void SetMaxOrbLimit(int amount){
        Debug.Log($"Setting ActiveOrbArray to size: {amount}");
        activeOrbArray = new Orb[amount];
    }

    void HandleNewOrbInfo(OrbInfo _orbInfo){
        // if (activeOrbArray[_orbInfo.id].orbInfo.id != default){
        //     Debug.Log($"Aborted Creating new orb: Orb ({_orbInfo.id}) already exists");
        // }
        CreateNewOrb(_orbInfo);
    }

    void CreateNewOrb(OrbInfo _orbInfo){
        Debug.Log($"Spawning orb: ({_orbInfo.id})...");
        var spawnedOrb = Instantiate(orbPrefab, new Vector3(_orbInfo.positionX,_orbInfo.positionY),quaternion.identity);
        var orb = spawnedOrb.GetComponent<Orb>();
        orb.orbInfo = _orbInfo;
        activeOrbArray[_orbInfo.id] = orb;
        orb.SetEverything();
        
        Debug.Log($"Spawned player: ({_orbInfo.id}).");
    }
}
