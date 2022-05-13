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
    [SerializeField] PlayerInfoUnityEventSo newPlayerJoinedEventSo;
    //[SerializeField] PlayerInfoUnityEventSo playerDisconnectedEventSo;
    [SerializeField] GameObject playerPrefab;
    [SerializeField] PlayerInfoUnityEventSo mimicPlayerDeathSo;
    [SerializeField] PlayerInfoUnityEventSo mainPlayerDeathSo;
    [SerializeField] ExecuteOnMainThread executeOnMainThread;
    [SerializeField] SceneChanger sceneChanger;
    

    [SerializeField] IntUnityEventSo maxPlayersAllowedEventSo;
    public Dictionary<int, Player> activePlayerDictionary;

    void Awake(){
        DontDestroyOnLoad(this);
    }
    
    void Start(){
        maxPlayersAllowedEventSo.intUnityEventSo.AddListener(SetMaxPlayers);
        executeOnMainThread.Execute(() => playerInfoReceivedFromServerEventSo.playerInfoUnityEventSo.AddListener(HandlePlayerInfo));
        executeOnMainThread.Execute(()=> newPlayerJoinedEventSo.playerInfoUnityEventSo.AddListener(HandlePlayerInfo));
        //executeOnMainThread.Execute(()=> playerDisconnectedEventSo.playerInfoUnityEventSo.AddListener(DestroyPlayer));
        executeOnMainThread.Execute(()=> mimicPlayerDeathSo.playerInfoUnityEventSo.AddListener(DestroyPlayer));
        executeOnMainThread.Execute(()=> mainPlayerDeathSo.playerInfoUnityEventSo.AddListener(HandleMainPlayerDeath));
    }

    void HandleMainPlayerDeath(PlayerInfo _playerInfo){
        DestroyAllButMainPlayer(_playerInfo);
        sceneChanger.LoadEndScene();
    }

    void DestroyAllButMainPlayer(PlayerInfo _playerInfo){
        foreach (var playerInDictionary in activePlayerDictionary){
            if (playerInDictionary.Value.playerInfo.id == _playerInfo.id){
                continue;
            }

            DestroyPlayer(playerInDictionary.Value.playerInfo);
        }
    }

    void HandlePlayerInfo(PlayerInfo _playerInfo){
        if (activePlayerDictionary.ContainsKey(_playerInfo.id)){
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
        Debug.Log($"Spawning player: {_playerInfo.name} ({_playerInfo.id})...");
        var spawnedPlayer = Instantiate(playerPrefab, new Vector3(_playerInfo.positionX,_playerInfo.positionY),quaternion.identity);
        var player = spawnedPlayer.GetComponent<Player>();
        player.playerInfo = _playerInfo;
        player.SetEverything();
        activePlayerDictionary[_playerInfo.id] = player;
        Debug.Log($"Spawned player: {_playerInfo.name} ({_playerInfo.id}).");
    }

    void DestroyPlayer(PlayerInfo _playerInfo){
        Debug.Log($"Destroying: {_playerInfo.name} ({_playerInfo.id})...");
        Destroy(activePlayerDictionary[_playerInfo.id].gameObject);
        Debug.Log($"Destroyed: {_playerInfo.name} ({_playerInfo.id}).");
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
