using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using AgarioShared;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class GlobalPlayerManager : MonoBehaviour{
    [SerializeField] PlayerInfoUnityEventSo playerInfoReceivedFromServerEventSo;
    //[SerializeField] PlayerInfoUnityEventSo playerDisconnectedEventSo;
    [SerializeField] GameObject playerPrefab;
    //[SerializeField] GameObject map;
    [SerializeField] ExecuteOnMainThread executeOnMainThread;

    [SerializeField] IntUnityEventSo maxPlayersAllowedEventSo;
    public Dictionary<int, Player> activePlayerDictionary;

   // public PlayerInfo[] allActivePlayersArray;

    void Awake(){
        DontDestroyOnLoad(this);
    }
    
    void Start(){
        maxPlayersAllowedEventSo.intUnityEventSo.AddListener(SetMaxPlayers);
        executeOnMainThread.Execute(() => playerInfoReceivedFromServerEventSo.playerInfoUnityEventSo.AddListener(HandlePlayerInfo));
        //executeOnMainThread.Execute(()=> playerDisconnectedEventSo.playerInfoUnityEventSo.AddListener(DestroyPlayer));
    }

    void HandlePlayerInfo(PlayerInfo _playerInfo){
        if (activePlayerDictionary[_playerInfo.id] == null ){
            CreateNewPlayer(_playerInfo);
            return;
        }
        UpdatePlayer(_playerInfo);
       
    }

    void UpdatePlayer(PlayerInfo _playerInfo){
        throw new NotImplementedException();
    }

    void CreateAllOtherPlayers(){
        
    }
    
    void CreateNewPlayer(PlayerInfo _playerInfo){
        
        var spawnedPlayer = Instantiate(playerPrefab, new Vector3(_playerInfo.positionX,_playerInfo.positionY),quaternion.identity);
        var player = spawnedPlayer.GetComponent<Player>();
        player.playerInfo = _playerInfo;
        player.SetEverything();
        activePlayerDictionary[_playerInfo.id] = player;
    }

    void DestroyPlayer(PlayerInfo _playerInfo){
        Debug.Log($"Destroying: {_playerInfo.name} + ({_playerInfo.id})...");
        Destroy(activePlayerDictionary[_playerInfo.id].gameObject);
        Debug.Log($"Destroyed: {_playerInfo.name} + ({_playerInfo.id}).");
        activePlayerDictionary.Remove(_playerInfo.id);
    }

    void SetMaxPlayers(int maxPlayer){
        Debug.Log($"Setting max player limit to: {maxPlayer}");
        activePlayerDictionary = new(maxPlayer);
        for (int i = 0; i < maxPlayer; i++){
            activePlayerDictionary.Add(i,null);
        }
    }
    
}
