using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using AgarioServer;
using AgarioShared;
using Network;

public class Server{
    static readonly int port = 9000;
    static readonly IPEndPoint hostEndpoint = new IPEndPoint(IPAddress.Any, port);
    static TcpListener hostListener;
    static UdpClient udpHost;
    public static int maxClients = 10;
    
   public static Dictionary<int, ClientSlot> connectedClientDictionary;

   public static async Task Main(){
        ServerSetUp();

        //Begins the connection process, but creates a new thread which deals with finishing it.
        ListenForTcpClientsTask();
        ReceiveUdpDataTask();
        Console.Read();
    }

    static async Task ServerSetUp(){
        Console.WriteLine("Starts Server Set Up");
        hostListener = new TcpListener(hostEndpoint);
        udpHost = new UdpClient(hostEndpoint);
        connectedClientDictionary = new Dictionary<int, ClientSlot>(maxClients);

        var worldSetUpTask = WorldManager.SetUp(); // Refactor somehow to remove need of other script. Event, delegate?
        CreateEmptyClientSlots();
        await worldSetUpTask;
        
        Console.WriteLine($"Starting server...");
        hostListener.Start();
        Console.WriteLine($"Server Started (Port: {port})");
        Console.WriteLine("Finished Server Set Up");
    }

    static void CreateEmptyClientSlots(){
        for (int i = 1; i < maxClients; i++){
            connectedClientDictionary.Add(i, new ClientSlot(0, null));
        }
    }

    static async Task ListenForTcpClientsTask(){
        Console.WriteLine("Starting task to listen for new tcp Clients");
        while (true){ 
            Console.WriteLine("Waiting for connection...");
            var tcpClient = await hostListener.AcceptTcpClientAsync();
            
            Console.WriteLine($"New Client accepted.");
            var activatedClientSlot = await TryAssignClientToDictionary(tcpClient);
            new Task(()=> MessageHandler.PrepareThenSendMessage("InitialServerToClientMessage", activatedClientSlot)).Start();
            new Task(()=> MessageHandler.StartReceivingMessagesTask(activatedClientSlot)).Start();
        }
    }

    static async Task<ClientSlot>  TryAssignClientToDictionary(TcpClient tcpClient){
        for (int i = 1; i < connectedClientDictionary.Count; i++){
            if (connectedClientDictionary[i].id == default){
                connectedClientDictionary[i] = new ClientSlot(i, tcpClient); //TODO: Adding one here
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
        Console.WriteLine("Starting ReceiveUdpDataTask");
        while (true){
            //Console.WriteLine("Awaiting Udp Package...");
            var udpReceiveResult = await udpHost.ReceiveAsync();
            //Console.WriteLine("Udp Package received.");
            
            //Console.WriteLine("Udp Result: "+ udpReceiveResult.Buffer);
            var receivedMessageAsString = Encoding.ASCII.GetString(udpReceiveResult.Buffer);
            //Console.WriteLine("UDPreceivedMessageAsString: "+ receivedMessageAsString);
            //var jsonString = JsonSerializer.Serialize(receivedMessageAsString);
            // Console.WriteLine("udpMessageAsJson: "+ jsonString);
            new Task(() => { MessageHandler.HandleReceivedUdpDataTask(receivedMessageAsString); }).Start();
        }
    }
   
}