using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Xml;
using AgarioShared;
using Network;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace AgarioServer;

internal class MessageHandler{
    
    #region SendMessageRegion
    public static async Task PrepareThenSendMessage(string messageName, ClientSlot clientSlot){

        Console.WriteLine("Checking Message type to send...");
        switch (messageName){
            case "InitialServerToClientMessage":{
                Random random = new Random();

                var message = new InitialServerToClientMessage{
                    messageName = "InitialServerToClientMessage",
                    id = clientSlot.id,
                    maxPlayers = WorldManager.maxPlayers,
                    positionX = random.NextSingle() * (WorldManager._mapSizeX / 2f), //0-1 * 300 can be 0.34*300 == 102
                    positionY = random.NextSingle() * (WorldManager._mapSizeY / 2f),
                    score = WorldManager.playerScoreStart,
                    scoreDefenseModifier = WorldManager.scoreDefenseModifier,
                    size = WorldManager.playerSizeStart,
                    movementSpeed = WorldManager.playerMovementSpeed,
                    mapSizeX = WorldManager._mapSizeX,
                    mapSizeY = WorldManager._mapSizeY
                };
                Console.WriteLine($"Clientslot Id ({clientSlot.id})");
                await SendMessageTask(message,clientSlot);
                break;
            }
            case "AllPlayerInfoMessage":{
                var message = new AllPlayerInfoMessage{
                    messageName = "AllPlayerInfoMessage",
                    allPlayersInfoArray = WorldManager.connectedPlayerArray
                };
                
                Console.WriteLine($"All Player Array: {message.allPlayersInfoArray}");
               

                foreach (var _playerInfo in message.allPlayersInfoArray){
                    if (_playerInfo == null){
                        continue;
                    }
                    Console.WriteLine($"Player {_playerInfo.id}:  {_playerInfo.name}");
                }
                
                await SendMessageTask(message, clientSlot);
                
                break;
            }
            case "PlayerInfoMessage":{
                var message = new PlayerInfoMessage{
                    messageName = "PlayerInfoMessage",
                    playerInfo = clientSlot.playerInfo
                };

                await SendMessageTask(message, clientSlot);
                break;
            }
            case "AllOrbsInfoMessage":{
                var message = new AllOrbsInfoMessage{
                    messageName = "AllOrbsInfoMessage",
                    allOrbsArray = WorldManager.activeOrbsArray
                };
                Console.WriteLine($"Sending Orb array {message.allOrbsArray.Length}");
                await SendMessageTask(message, clientSlot);
                break;
            }

            default:{
                throw new NotImplementedException();
            }
                
                
        }
    }

    public static async Task PrepareThenSendMessageToAllConnectedClients<T>(string messageName, T content){
        Console.WriteLine("Checking Message type to send to all clients...");
        switch (messageName){
            case "NewPlayerJoinedInfoMessage":{
                var message = new NewPlayerJoinedInfoMessage{
                    messageName = "NewPlayerJoinedInfoMessage",
                    playerInfo = content as PlayerInfo,
                };
                Console.WriteLine($"New Player Joined Message to send: {message.playerInfo.name} ({message.playerInfo.id})");
                foreach (var connectedClient in Server.connectedClientDictionary){
                    if (connectedClient.Value.id == (content as PlayerInfo).id){
                        continue;
                    }
                    await SendMessageTask(message, connectedClient.Value);
                }
                
                break;
            }
            case "PositionMessage":{ //TODO: Convert To udp data
                foreach (var connectedClient in Server.connectedClientDictionary){
                    if (connectedClient.Value.id == (content as PositionMessage).id){
                        continue;
                    }

                    await SendMessageTask(content, connectedClient.Value);
                } 
                break;
            }
        }
    }

    static async Task SendMessageTask<T>(T message, ClientSlot clientSlot){
        var id = clientSlot.id;
        var address = clientSlot.tcpClient.Client.RemoteEndPoint;
        var stream = clientSlot.tcpClient.GetStream();
        var streamWriter = new StreamWriter(stream);
        streamWriter.AutoFlush = true;
        var jsonOptions = new JsonSerializerOptions(){
            IncludeFields = true
        };
        
        Console.WriteLine($"Awaiting to send {message} to: {address} ({id})...");
        await streamWriter.WriteLineAsync(JsonSerializer.Serialize(message,jsonOptions));
        Console.WriteLine($"Sent {message} to: {address} ({id}).");
        await streamWriter.FlushAsync();
    }
    #endregion
    
    
    
    

    #region ReceiveMessageRegion
    public static async Task StartReceivingMessagesTask(ClientSlot clientSlot){ //Should be called once by each new connection
        var id = clientSlot.id;
        var address = clientSlot.tcpClient.Client.RemoteEndPoint;
        var stream = clientSlot.tcpClient.GetStream();
        var streamReader = new StreamReader(stream);

        while (clientSlot.tcpClient.Connected){
            Console.WriteLine($"Listening for data stream from {address} ({id}).");
            var jsonString = await streamReader.ReadLineAsync();
            
            if (jsonString == null){
                //No data received
                Console.WriteLine($"Data stream from {address} ({id}) was empty, discarding.");
            
                //Disconnecting Client
               
                WorldManager.connectedPlayerArray[id] = new PlayerInfo();

                foreach (var slot in Server.connectedClientDictionary){
                    if (slot.Value.id == default || slot.Value.id == clientSlot.id){
                        continue;
                    }
                    PrepareThenSendMessage("AllPlayerInfoMessage", slot.Value);
                }
                await clientSlot.ClearAllData(id);
                continue;
            }

            ReceiveMessageTask(jsonString,clientSlot);
        }
        
    }
    
    
    public static async Task ReceiveMessageTask(string _jsonString, ClientSlot clientSlot){
        var id = clientSlot.id;
        var jsonOptions = new JsonSerializerOptions(){
            IncludeFields = true
        };
        
        Console.WriteLine($"Deserializing message from Client ({id})...");
        var identityMessage = JsonSerializer.Deserialize<Message>(_jsonString,jsonOptions);
        Console.WriteLine($"Message deserialized, Type: {identityMessage.messageName} from Client ({id})");
        
        
        switch (identityMessage.messageName){
            case "PlayerInfoMessage":{
                var message = JsonSerializer.Deserialize<PlayerInfoMessage>(_jsonString,jsonOptions);
                
                clientSlot.playerInfo = message.playerInfo;
                Console.WriteLine($"Adding PlayerInfo from Client ({id})...");
                
                Console.WriteLine("Name: "+clientSlot.playerInfo.name + " Id: " + clientSlot.playerInfo.id + " Score: " +clientSlot.playerInfo.score + " Size: " +clientSlot.playerInfo.size +
                              ("ColorR: " +clientSlot.playerInfo.colorR + " ColorG: " + clientSlot.playerInfo.colorG + " ColorB: "+clientSlot.playerInfo.colorB) + " PositionX: " +clientSlot.playerInfo.positionX + " PositionY: " +clientSlot.playerInfo.positionY);
                WorldManager.connectedPlayerArray[clientSlot.playerInfo.id] = clientSlot.playerInfo;
                Console.WriteLine($"Adding PlayerInfo from Client ({id}): {WorldManager.connectedPlayerArray[clientSlot.playerInfo.id].name}");
                
                PrepareThenSendMessage("AllPlayerInfoMessage",clientSlot);
                PrepareThenSendMessage("AllOrbsInfoMessage", clientSlot);
                
                PrepareThenSendMessageToAllConnectedClients("NewPlayerJoinedInfoMessage", message.playerInfo);
                break;
            }
            
            
        }
    }

    #endregion

    #region UDPReceiveMessageRegion

    internal static void HandleReceivedUdpDataTask(string jsonUdpResult){
        var jsonOptions = new JsonSerializerOptions(){
            IncludeFields = true
        };
       
        var udpMessage = JsonSerializer.Deserialize<Message>(jsonUdpResult,jsonOptions);

        if (udpMessage == default){
            Console.WriteLine("Received Udp Message: Invalid, discarding.");
            return;
        }

        //Console.WriteLine($"Udp Message is of type : {udpMessage.messageName}");
        switch (udpMessage.messageName){
            case "PositionMessage":{
                
                var result = JsonSerializer.Deserialize<PositionMessage>(jsonUdpResult,jsonOptions);

                Console.WriteLine($"Assigning position values for Player({result.id})");
                Server.connectedClientDictionary[result.id].playerInfo.positionX = result.positionX;
                Server.connectedClientDictionary[result.id].playerInfo.positionX = result.positionY;
                PrepareThenSendMessageToAllConnectedClients("PositionMessage", result);
                break;
            }
                default: {
                Console.WriteLine("Not assigned Udp Message Type");
                throw new NotImplementedException();
                break;
            }
        }
        
    }

    #endregion
    
}