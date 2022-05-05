using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Numerics;
using System.Runtime.InteropServices;
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
   public static Dictionary<string, PlayerInfo> connectedPlayerDictionary;

    public static Action<int> clearDataEvent;
    
    
    public static void Main(){
        ServerSetUp();

        //Begins the connection process, but creates a new thread which deals with finishing it.
        Console.WriteLine("Starting task to listen for new tcp Clients");
        ListenForTcpClientsTask();
       // ReceiveUdpDataTask();
        Console.Read();
    }

    static void ServerSetUp(){
        hostListener = new TcpListener(hostEndpoint);
        udpHost = new UdpClient(hostEndpoint);
        
        connectedClientDictionary = new Dictionary<int, ClientSlot>(maxClients);
        connectedPlayerDictionary = new Dictionary<string, PlayerInfo>(maxClients);

        CreateEmptyClientSlots();

        Console.WriteLine($"Starting server...");
        hostListener.Start();
        //udpHost.Receive();
        Console.WriteLine($"Server Started (Port: {port})");
    }

    static void CreateEmptyClientSlots(){
        for (int i = 1; i <= maxClients; i++){
            connectedClientDictionary.Add(i, new ClientSlot(0, null));
            //connectedPlayerDictionary.Add(i, new PlayerInfo());
        }
    }

    static async Task ListenForTcpClientsTask(){
        while (true){ 
            Console.WriteLine("Waiting for connection...");
            var tcpClient = await hostListener.AcceptTcpClientAsync();
            
            Console.WriteLine($"New Client accepted.");
            var activatedClientSlot = await TryAssignClientToDictionary(tcpClient);
            new Task(()=> MessageHandler.PrepareThenSendMessage("InitialServerToClientMessage", activatedClientSlot).Start()).Start();
            new Task(()=> MessageHandler.StartReceivingMessagesTask(activatedClientSlot).Start()).Start();
        }
    }

    static async Task<ClientSlot>  TryAssignClientToDictionary(TcpClient tcpClient){
        for (int i = 1; i <= connectedClientDictionary.Count; i++){
            if (connectedClientDictionary[i].id == default){
                connectedClientDictionary[i] = new ClientSlot(i, tcpClient);
                Console.WriteLine($"New Client: ({tcpClient.Client.RemoteEndPoint}, Id: ({i}).");
                
                return connectedClientDictionary[i];
            }
        }

        Console.WriteLine("No available Client Slots");
        //TODO: Disconnect Client
        throw new NotImplementedException();
        return null;
    }

    static async Task ReceiveUdpDataTask(){
        throw new InvalidOleVariantTypeException();
        while (true){
            Console.WriteLine("Awaiting Udp Package...");
            var udpReceiveResult = await udpHost.ReceiveAsync();
            Console.WriteLine("Udp Package received.");
            new Task(() => { HandleReceivedUdpDataTask(udpReceiveResult); }).Start();
        }
    }

    static void HandleReceivedUdpDataTask(UdpReceiveResult udpReceiveResult){
        throw new InvalidOleVariantTypeException();
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