using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Numerics;
using System.Text;
using System.Text.Json;
using AgarioServer;

public class Server{
    static readonly int port = 9000;
    static readonly IPEndPoint hostEndpoint = new IPEndPoint(IPAddress.Any, port);
    static TcpListener hostListener;
    static UdpClient udpHost;
    static int maxClients = 10;
    static Dictionary<int, ClientSlot> connectedClientDictionary;
    static Dictionary<int, PlayerInfo> connectedPlayerDictionary;

    public static Action<int> clearDataEvent;
    
    
    public static void Main(){
        ServerSetUp();

        //Begins the connection process, but creates a new thread which deals with finishing it.
        Console.WriteLine("Starting task to listen for new tcp Clients");
        ListenForTcpClientsTask();
        ReceiveUdpDataTask();
        Console.Read();
        
        
    }

    static void ServerSetUp(){
        hostListener = new TcpListener(hostEndpoint);
        udpHost = new UdpClient(hostEndpoint);
        
        connectedClientDictionary = new Dictionary<int, ClientSlot>(maxClients);
        connectedPlayerDictionary = new Dictionary<int, PlayerInfo>(maxClients);

        CreateEmptyClientSlots();

        Console.WriteLine($"Starting server...");
        hostListener.Start();
        //udpHost.Receive();
        Console.WriteLine($"Server Started (Port: {port})");
    }

    static void CreateEmptyClientSlots(){
        for (int i = 1; i <= maxClients; i++){
            connectedClientDictionary.Add(i, new ClientSlot(0, null));
        }
    }

    static async Task ListenForTcpClientsTask(){
        while (true){ 
            Console.WriteLine("Waiting for connection...");
            var tcpClient = await hostListener.AcceptTcpClientAsync();
            
            Console.WriteLine($"New Client accepted.");
            var activatedClientSlot = TryAssignClientToDictionary(tcpClient);

            new Task(()=> ReadFromStreamTask(activatedClientSlot).Start()).Start();
            new Task(() => SendTcpDataTask(activatedClientSlot).Start()).Start();
        }
    }

    static ClientSlot TryAssignClientToDictionary(TcpClient tcpClient){
        for (int i = 1; i <= connectedClientDictionary.Count; i++){
            if (connectedClientDictionary[i].id == default){
                connectedClientDictionary[i] = new ClientSlot(i, tcpClient);
                Console.WriteLine($"New Client: ({tcpClient.Client.RemoteEndPoint}, Id: ({i}).");

                connectedPlayerDictionary[i] = connectedClientDictionary[i].playerInfo;
                
                return connectedClientDictionary[i];
            }
        }

        Console.WriteLine("No available Client Slots");
        //TODO: Disconnect Client
        throw new NotImplementedException();
        return null;
    }

    static async Task ReadFromStreamTask(ClientSlot clientSlot){
        var tcpClient = clientSlot.tcpClient.Client;
        var id = clientSlot.id;
        var address = clientSlot.tcpClient.Client.RemoteEndPoint;
        var stream = clientSlot.tcpClient.GetStream();
        var streamReader = new StreamReader(stream);
        var streamWriter = new StreamWriter(stream);
        streamWriter.AutoFlush = true;
        var jsonOptions = new JsonSerializerOptions(){
            IncludeFields = true
        };

        while (tcpClient.Connected){

            Console.WriteLine($"Listening for data stream from {address} ({id}).");
            var jsonString = await streamReader.ReadLineAsync();
            
            if (jsonString == default){
                //No data received
                Console.WriteLine($"Data stream from {address} ({id}) was empty, discarding.");
                
                //Disconnecting Client
                clientSlot.ClearAllData(id);
                
                continue;
            }

            Console.WriteLine($"Received data from {address} ({id})");

            Console.WriteLine("Deseralizing data...");
            var message = JsonSerializer.Deserialize<Message>(jsonString, jsonOptions);
            Console.WriteLine($"Data Deseralized, Type: {message.messageName}");
            if (message.messageName == "ConnectToServerMessage"){
                var playerConnectToServerData = JsonSerializer.Deserialize<ConnectToServerMessage>(jsonString, jsonOptions);
                Console.WriteLine($"Received data of type ({message.messageName}) from {address} ({id})");
                clientSlot.playerInfo.name = playerConnectToServerData.name;
                Console.WriteLine($"Name: {playerConnectToServerData.name} ({id})");
                clientSlot.playerInfo.color = playerConnectToServerData.color;
                Console.WriteLine($"Color: {playerConnectToServerData.color} ({id})");
            }
            else{
                Console.WriteLine("Faulty Tcp Message name");
            }

        }
    }

    static async Task SendTcpDataTask(ClientSlot clientSlot){
        await SendInitialTcpConnectionData(clientSlot);
        //TODO: Update data
    }

    static async Task SendInitialTcpConnectionData(ClientSlot clientSlot){
        var id = clientSlot.id;
        var address = clientSlot.tcpClient.Client.RemoteEndPoint;
        var stream = clientSlot.tcpClient.GetStream();
        var streamWriter = new StreamWriter(stream);
        streamWriter.AutoFlush = true;
        var jsonOptions = new JsonSerializerOptions(){
            IncludeFields = true
        };

        InitialServerToClientMessage message = new (){
            messageName = "InitialServerToClientMessage",
            id = clientSlot.id,
            playerDictionary = connectedPlayerDictionary,
            positionX = 5f,
            positionY = 5f,
            mapSizeX = 300f,
            mapSizeY = 300f
        };
        
        Console.WriteLine($"Awaiting to send {message.messageName} to: {address} ({id})...");
        await streamWriter.WriteLineAsync(JsonSerializer.Serialize(message,jsonOptions));
        Console.WriteLine($"Sent {message.messageName} to: {address} ({id}).");

    }

    static async Task ReceiveUdpDataTask(){
        while (true){
            Console.WriteLine("Awaiting Udp Package...");
            var udpReceiveResult = await udpHost.ReceiveAsync();
            Console.WriteLine("Udp Package received.");
            new Task(() => { HandleReceivedUdpDataTask(udpReceiveResult); }).Start();
        }
    }

    static void HandleReceivedUdpDataTask(UdpReceiveResult udpReceiveResult){
        var udpMessage = JsonSerializer.Deserialize<Message>(udpReceiveResult.Buffer);

        if (udpMessage == default){
            Console.WriteLine("Received Udp Message: Invalid, discarding.");
            return;
        }

        string messageType = "";
        if (udpMessage.messageName == "PositionMessage"){
            var result = JsonSerializer.Deserialize<PositionMessage>(udpReceiveResult.Buffer);
            foreach (var clientSlot in connectedClientDictionary){
                if (result.id == clientSlot.Key){
                    clientSlot.Value.playerInfo.positionX = result.positionX;
                    clientSlot.Value.playerInfo.positionY = result.positionY;
                }
            }

            messageType = udpMessage.messageName;
        }
        else{
            Console.WriteLine("Not assigned Udp Message Type");
        }

        Console.WriteLine($"Udp packaged: {messageType}");
    }
}

