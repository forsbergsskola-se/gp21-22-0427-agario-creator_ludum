using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Xml;
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
                var _mapSizeX = 300;
                var _mapSizeY = 300;
                
                var message = new InitialServerToClientMessage{
                    messageName = "InitialServerToClientMessage",
                    id = clientSlot.id,
                    maxPlayers = Server.maxClients,
                    positionX = random.NextSingle() * (_mapSizeX / 2f), //0-1 * 300 can be 0.34*300 == 102
                    positionY = random.NextSingle() * (_mapSizeY / 2f),
                    score = 0,
                    size = 3f,
                    movementSpeed = 20f,
                    mapSizeX = _mapSizeX,
                    mapSizeY = _mapSizeY
                };
                Console.WriteLine($"Clientslot Id ({clientSlot.id})");
                await SendMessageTask(message,clientSlot);
                break;
            }
            case "AllPlayerInfoMessage":{
                var message = new AllPlayerInfoMessage{
                    messageName = "AllPlayerInfoMessage",
                    allPlayersInfoArray = Server.connectedPlayerArray
                };
                
                Console.WriteLine($"All Player Array: {message.allPlayersInfoArray}");
                Console.WriteLine($"Player 1: {message.allPlayersInfoArray[1].name}");
                
                await SendMessageTask(message, clientSlot);
                
                break;
            }
            default:{
                throw new NotImplementedException();
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
                clientSlot.ClearAllData(id);
            
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
                Server.connectedPlayerArray[clientSlot.playerInfo.id] = clientSlot.playerInfo;
                Console.WriteLine($"Adding PlayerInfo from Client ({id}): {Server.connectedPlayerArray[clientSlot.playerInfo.id].name}");
                
                PrepareThenSendMessage("AllPlayerInfoMessage",clientSlot);
                break;
            }
            
            
        }
    }

    #endregion
    
}