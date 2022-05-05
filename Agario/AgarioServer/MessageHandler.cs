using System.Text.Json;

namespace AgarioServer;

internal class MessageHandler{
    
    #region SendMessageRegion
    public static async Task PrepareThenSendMessage(string messageName, ClientSlot clientSlot){

        switch (messageName){
            case "InitialServerToClientMessage":{
                Random random = new Random();
                var _mapSizeX = 300;
                var _mapSizeY = 300;
                
                var message = new InitialServerToClientMessage{
                    messageName = "InitialServerToClientMessage",
                    id = clientSlot.id,
                    positionX = random.NextSingle() * _mapSizeX, //0-1 * 300 can be 0.34*300 == 102
                    positionY = random.NextSingle() * _mapSizeY,
                    score = 0,
                    size = 3f,
                    mapSizeX = _mapSizeX,
                    mapSizeY = _mapSizeY
                };
                await SendMessageTask(message,clientSlot);
                break;
            }
            case "AllPlayerInfoMessage":{

                var message = new AllPlayerInfoMessage{
                    messageName = "AllPlayerInfoMessage",
                    allPlayerDictionary = Server.connectedPlayerDictionary
                };
                await SendMessageTask(message,clientSlot);
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
                
                clientSlot.playerInfo = message.playerInfo; // ITS null...
                Console.WriteLine(message.playerInfo);
                Console.WriteLine($"Adding PlayerInfo from Client ({id})...");
                //TODO: FIX BUG
                Console.Write("Name: "+clientSlot.playerInfo.name, "Id: " + clientSlot.playerInfo.id + "Score: " +clientSlot.playerInfo.score + "Size: " +clientSlot.playerInfo.size +
                              ("ColorR: " +clientSlot.playerInfo.colorR + "ColorG: " + clientSlot.playerInfo.colorG + "ColorB: "+clientSlot.playerInfo.colorB) + "PositionX: " +clientSlot.playerInfo.positionX + "PositionY: " +clientSlot.playerInfo.positionY);
                Server.connectedPlayerDictionary[clientSlot.playerInfo.id] = clientSlot.playerInfo;
                Console.WriteLine($"Adding PlayerInfo from Client ({id}): {Server.connectedPlayerDictionary[clientSlot.playerInfo.id].name}");
                
                PrepareThenSendMessage("AllPlayerInfoMessage",clientSlot);
                break;
            }
            
            
        }
    }

    #endregion
    
}