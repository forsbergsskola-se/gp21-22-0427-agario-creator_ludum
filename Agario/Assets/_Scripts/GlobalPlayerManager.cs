using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class GlobalPlayerManager : MonoBehaviour{
    // [SerializeField] PlayerUnityEventSO playerCreatedEventSo;
    // [SerializeField] DataUnityEventSo newPlayerDataReadyEventSo;
    // [SerializeField] IntUnityEventSo createNewPlayerEventSo;
    // [SerializeField] GameObject playerPrefab;
    // [SerializeField] GameObject map;
    // Dictionary<int, Player> playerDictionary = new Dictionary<int, Player>();
    //
    // public Dictionary<int, Player> PlayerDictionary{
    //     get => playerDictionary;
    //     set{
    //         playerDictionary = value;
    //         
    //     }
    // }
    //
    // void Awake(){
    //     DontDestroyOnLoad(this);
    // }
    //
    // void Start(){
    //     playerCreatedEventSo.playerUnityEventSo.AddListener(SendNewPlayerData);
    //     createNewPlayerEventSo.intUnityEventSo.AddListener(CreateNewPlayer);
    // }
    //
    //
    //
    // void SendNewPlayerData(Player player){
    //    byte[][] playerData = ConvertNewPlayerDataToBytes(player);
    //    newPlayerDataReadyEventSo.dataUnityEventSo.Invoke(playerData);
    // }
    //
    // byte[][] ConvertNewPlayerDataToBytes(Player player){
    //     byte[][] convertedPlayerDataArray = new byte[][]{ };
    //     
    //     var playerName = Encoding.ASCII.GetBytes(player.name);
    //     var playerPosition = Encoding.ASCII.GetBytes(player.transform.position.ToString());
    //     var playerColor = Encoding.ASCII.GetBytes(player.color.ToString());
    //
    //     
    //     
    //     convertedPlayerDataArray[0] = playerName;
    //     convertedPlayerDataArray[1] = playerPosition;
    //     convertedPlayerDataArray[2] = playerColor;
    //     
    //     return convertedPlayerDataArray;
    // }
    //
    // void SendAllPlayerData(){}
    //
    // void CreateAllOtherPlayers(){
    //     
    // }
    //
    // void CreateNewPlayer(int id){
    //     var spawnedPlayer =Instantiate(playerPrefab,RandomLocation(),quaternion.identity);
    //     var player = spawnedPlayer.GetComponent<Player>();
    //     player.id = id;
    //    // player.name;
    //    // player.color;
    // }
    //
    // Vector3 RandomLocation(){
    //     var mapScale = map.transform.localScale;
    //     var boundaryX = mapScale.x/2;
    //     var boundaryY = mapScale.y/2;
    //     return new Vector3(Random.Range(0,boundaryX), Random.Range(0,boundaryY), 0);
    // }
}
