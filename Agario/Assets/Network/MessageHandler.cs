using System;
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

        public async Task ReceiveNewMessage(string _jsonString){
            // var deserializedObject = JsonConvert.DeserializeObject(_jsonString);
            // Console.WriteLine($"Deserialized: {deserializedObject}");
            // var jsonObjectString = JsonUtility.ToJson(deserializedObject);
            // Console.WriteLine($"jsonObjectString: {jsonObjectString}");
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
                    //var message = JsonUtility.FromJson<AllPlayerInfoMessage>(_jsonString); //Todo: Is null
                    var dictionaryDeserialized = JsonConvert.DeserializeObject(_jsonString);
                    var dictionaryAsString = JsonUtility.ToJson(dictionaryDeserialized);
                    var message = JsonUtility.FromJson<AllPlayerInfoMessage>(dictionaryAsString);
                    if (message == null){
                        throw new ArgumentNullException();
                    }
                    
                    Debug.Log($"Message: {message}");
                    Debug.Log($"Message Dictionary: {message.allPlayersInfoDictionary}");
                    Debug.Log($"Message Name : "+message.allPlayersInfoDictionary["1"].name);
                    
                    player.playerDictionary = message.allPlayersInfoDictionary;
                    
                    //TODO: Remove Debug log
                    Debug.Log($"Message Name : {message.allPlayersInfoDictionary["1"].name}");
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