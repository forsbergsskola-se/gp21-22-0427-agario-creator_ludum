using System;
using System.Threading.Tasks;
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
                    Debug.Log(message.playerInfo.name);
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
            Debug.Log("Attempting to send data to host...");
            await personalClient.streamWriter.WriteLineAsync(JsonUtility.ToJson(message));
            Debug.Log("Data sent to host.");
            await personalClient.streamWriter.FlushAsync();
        }

        #endregion

        #region ReceiveMessageRegion

        public async Task ReceiveNewMessage(string _jsonString){
            var identityMessage = JsonUtility.FromJson<Message>(_jsonString);
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
                    player.playerDictionary = message.allPlayersInfoDictionary;
                    
                    //TODO: Remove Debug log
                    foreach (var dictionaryPlayer in  player.playerDictionary){
                        Debug.Log("HIHIHI");
                        Debug.Log($"Player: {dictionaryPlayer.Value.name} ({dictionaryPlayer.Key})");
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


        internal async Task ListenForMessageAsync(){
            Console.WriteLine("Starting Method: ListenForMessageAsync ");
            while (personalClient.IsConnected){
                Debug.Log("Awaiting data...");
                var jsonString = await personalClient.streamReader.ReadLineAsync();
                Debug.Log("Data Received.");

                if (jsonString == default){
                    Debug.Log("Data is null, discarding.");
                    throw new NotImplementedException();
                    return;
                }

                ReceiveNewMessage(jsonString);
            }

        }

        #endregion
       
        
        
       
        
    }
}