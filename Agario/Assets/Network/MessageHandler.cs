using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

namespace Network{
    public class MessageHandler : MonoBehaviour{
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
                
                case "PositionMessage":{
                    throw new NotImplementedException();
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
                    playerInfo.positionX = message.positionX;
                    playerInfo.positionY = message.positionY;
                    personalClient.mapSizeSo.vector2.x = message.mapSizeX;
                    personalClient.mapSizeSo.vector2.y = message.mapSizeY;

                    await PrepareThenSendMessage("PlayerInfoMessage");
                    
                    break;
                }
                
                case "PlayerInfoMessage":{
                    var message = JsonUtility.FromJson<PlayerInfoMessage>(_jsonString);
                    playerInfo = message.playerInfo;
                    break;
                }
                case "AllPlayerInfoMessage":{
                    var message = JsonUtility.FromJson<AllPlayerInfoMessage>(_jsonString);
                    if (message == null){
                        throw new ArgumentNullException();
                    }
                    // Console.WriteLine("Message Array (1) name: "+ message.allPlayersInfoArray[1].name);
                    //player.allActivePlayersArray = message.allPlayersInfoArray;

                    player.allActivePlayersArray = new PlayerInfo[message.allPlayersInfoArray.Length];
                    player.allActivePlayersArray = message.allPlayersInfoArray;

                    foreach (var playerInfomercial in  player.allActivePlayersArray){
                        if (playerInfomercial.id == default){
                            continue;
                        }

                        Debug.Log("Active Player: "+playerInfomercial.name + $"({playerInfomercial.id})");
                    }
                    
                    for (int i = 1; i <= message.allPlayersInfoArray.Length; i++){

                        player.allActivePlayersArray[i] = message.allPlayersInfoArray[i];
                        Console.WriteLine( player.allActivePlayersArray[i].name);
                    }

                    break;
                }
                case "PositionMessage":{
                    var message = JsonUtility.FromJson<PositionMessage>(_jsonString);
                    playerInfo.positionX = message.positionX;
                    playerInfo.positionY = message.positionY;
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