using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AgarioShared;
using Newtonsoft.Json;
using UnityEngine;

namespace Network{
    public class MessageHandler : MonoBehaviour{
        public UnityEventSO playerReadyEventSo;
        public GlobalPlayerManager globalPlayerManager;
        [SerializeField] ExecuteOnMainThread executeOnMainThread;
        [SerializeField] IntUnityEventSo maxPlayersAllowedEventSo;
        [SerializeField] PlayerInfoUnityEventSo playerInfoReceivedFromServerEventSo;
        [SerializeField] OrbInfoUnityEventSo newOrbInfoFromServerEventSo;
        [SerializeField] IntUnityEventSo maxOrbsAllowedEventSo;
        [SerializeField] PlayerInfoUnityEventSo newPlayerJoinedEventSo;
        // [SerializeField] PlayerInfoUnityEventSo playerDisconnectedEventSo;
        
        Player player;
        PersonalClient personalClient;
        PlayerInfo playerInfo;
        
        void Awake(){
            player = GetComponent<Player>();
            personalClient = GetComponent<PersonalClient>();
            playerInfo = player.playerInfo;
        }

        #region SendMessageRegion

        public async Task PrepareThenSendMessage(string messageName){
           
            switch (messageName){
                case "PlayerInfoMessage":{
                    var message = new PlayerInfoMessage(){
                        messageName = "PlayerInfoMessage",
                        playerInfo = player.playerInfo
                        
                    };
                    await SendMessageTask(message);
                    break;
                }
                default:{
                    throw new NotImplementedException();
                }
            }
           
        }
        
        async Task SendMessageTask<T>(T message){
            Debug.Log($"Attempting to send data to host ({typeof(T)})...");
            await personalClient.streamWriter.WriteLineAsync(JsonUtility.ToJson(message));
            Debug.Log($"Data sent to host  ({typeof(T)}).");
            await personalClient.streamWriter.FlushAsync();
        }

        #endregion


        #region UDPSendMessagesRegion

        public async Task PrepareThenSendUdpMessages(string messageName){
            switch (messageName){
                case "PositionMessage":{
                    var message = new PositionMessage{
                        messageName = "PositionMessage",
                        id = playerInfo.id,
                        positionX = playerInfo.positionX,
                        positionY = playerInfo.positionY
                    };
                    await SendUDPMessageTask(message);
                    break;
                }
            }
        }

        async Task SendUDPMessageTask<T>(T message) where T : Message{
            Debug.Log($"Sending UDP Message of Type: {typeof(T)}...");
            var convertedMessage = JsonUtility.ToJson(message);

            await personalClient.udpClient.SendAsync(Encoding.ASCII.GetBytes(convertedMessage),
                convertedMessage.Length,personalClient.serverEndPoint);
            Debug.Log($"Sent UDP Message of Type: {typeof(T)}.");
        }

        #endregion
        
        
        
        #region ReceiveMessageRegion

        internal async Task ListenForMessageAsync(){
            Console.WriteLine("Starting Method: ListenForMessageAsync ");
            while (personalClient.IsConnected){
                Debug.Log("Awaiting data...");
                var jsonString = await personalClient.streamReader.ReadLineAsync();

                if (jsonString == default){
                    Debug.Log("Data is null, discarding.");
                    throw new NotImplementedException();
                    return;
                }

                ReceiveNewMessage(jsonString);
            }

        }
        
        public async Task ReceiveNewMessage(string _jsonString){
            var identityMessage = JsonUtility.FromJson<Message>(_jsonString);
            Console.WriteLine($"Identity: {identityMessage}");
            
            
            Debug.Log($"Data is of type: {identityMessage.messageName}");
            switch (identityMessage.messageName){
                case "InitialServerToClientMessage":{
                    var message = JsonUtility.FromJson<InitialServerToClientMessage>(_jsonString);
                    playerInfo.id = message.id;
                    playerInfo.score = message.score;
                    playerInfo.size = message.size;
                    playerInfo.movementSpeed = message.movementSpeed;
                    playerInfo.positionX = message.positionX;
                    playerInfo.positionY = message.positionY;
                    personalClient.mapSizeSo.vector2.x = message.mapSizeX;
                    personalClient.mapSizeSo.vector2.y = message.mapSizeY;
                    
                    executeOnMainThread.Execute(()=> maxPlayersAllowedEventSo.intUnityEventSo.Invoke(message.maxPlayers));
                    Debug.Log($"Id: {playerInfo.id}, score: {playerInfo.score}, size: {playerInfo.size}");
                    
                    await PrepareThenSendMessage("PlayerInfoMessage");
                    
                    break;
                }
                
                case "PlayerInfoMessage":{
                    var message = JsonUtility.FromJson<PlayerInfoMessage>(_jsonString);
                    playerInfo = message.playerInfo;
                    break;
                }
                case "NewPlayerJoinedInfoMessage":{
                    var message = JsonUtility.FromJson<NewPlayerJoinedInfoMessage>(_jsonString);

                    Debug.Log($"New Player Joined Message Received: {message.playerInfo.name} ({message.playerInfo.id})");
                    Debug.Log("Invoking newPlayerJoinedEvent");
                    executeOnMainThread.Execute(()=> newPlayerJoinedEventSo.playerInfoUnityEventSo.Invoke(message.playerInfo));
                   
                    break;
                }
                case "AllPlayerInfoMessage":{
                    var message = JsonUtility.FromJson<AllPlayerInfoMessage>(_jsonString);
                    if (message == null){
                        throw new ArgumentNullException();
                    }
                    
                    foreach (var _playerInfo in message.allPlayersInfoArray){
                        if (_playerInfo.id == playerInfo.id ||_playerInfo.id == default){
                            continue;
                        }
                        Debug.Log($"Invoking playerInfoEvent for player ({_playerInfo.id})");
                        executeOnMainThread.Execute(
                            () => playerInfoReceivedFromServerEventSo.playerInfoUnityEventSo.Invoke(_playerInfo));
                    }

                    Debug.Log("Invoking Player Ready Event On Main Thread");
                    executeOnMainThread.Execute(playerReadyEventSo.unityEventSo.Invoke); //Tells everyone who needs to know that the player is ready
                    
                    break;
                }
                case "PositionMessage":{
                    var message = JsonUtility.FromJson<PositionMessage>(_jsonString);
                   
                    executeOnMainThread.Execute(SetPlayerPosition);

                    void SetPlayerPosition(){
                        globalPlayerManager.activePlayerDictionary[message.id].playerInfo.positionX = message.positionX;
                        globalPlayerManager.activePlayerDictionary[message.id].playerInfo.positionY = message.positionY;
                        globalPlayerManager.activePlayerDictionary[message.id].transform.position = new Vector2(message.positionX,message.positionY);
                    }
                    
                    break;
                }
                case "AllOrbsInfoMessage":{
                    var message = JsonUtility.FromJson<AllOrbsInfoMessage>(_jsonString);
                    if (message == null){
                        throw new ArgumentNullException();
                    }
                    Debug.Log($"Max allowed orbs: {message.allOrbsArray.Length}");
                    executeOnMainThread.Execute(()=> maxOrbsAllowedEventSo.intUnityEventSo.Invoke(message.allOrbsArray.Length));
                    foreach (var orbInfo in message.allOrbsArray){
                        if (orbInfo.id == default){
                            continue;
                        }
                        Debug.Log($"Invoking orbInfoEvent for orb ({orbInfo.id})");
                        executeOnMainThread.Execute(
                            () => newOrbInfoFromServerEventSo.eventSo.Invoke(orbInfo));
                    }
                    break;
                    
                }
                default:{
                    throw new NotImplementedException();
                }
                
            }
        }

        #endregion
       
        
        
       
        
    }
}