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
    static int maxClients = 10;
    static Dictionary<int, ClientSlot> connectedClientDictionary;

    public static Action<int> clearDataEvent;
    
    
    public static void Main(){
        ServerSetUp();

        //Begins the connection process, but creates a new thread which deals with finishing it.
        Console.WriteLine("Starting task to listen for new tcp Clients");
        ListenForTcpClientsTask();
        Console.Read();
        
        
    }

    static void ServerSetUp(){
        hostListener = new TcpListener(hostEndpoint);

        connectedClientDictionary = new Dictionary<int, ClientSlot>(maxClients);

        CreateEmptyClientSlots();

        Console.WriteLine($"Starting server...");
        hostListener.Start();
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
        }
    }

    static ClientSlot TryAssignClientToDictionary(TcpClient tcpClient){
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
            string jsonString = await streamReader.ReadLineAsync();

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
                clientSlot.player.name = playerConnectToServerData.name;
                Console.WriteLine($"Name: {playerConnectToServerData.name} ({id})");
                clientSlot.player.color = playerConnectToServerData.color;
                Console.WriteLine($"Color: {playerConnectToServerData.color} ({id})");
            }
            else{
                Console.WriteLine("Faulty Message name");
            }
            
           
            


        }
    }
}

internal class ClientSlot{
    public int id;
    public TcpClient tcpClient;
    public Player player;
    public int bufferSize = 4000;
    public byte[] buffer;//4kb
    
    public ClientSlot(int _id, TcpClient _tcpClient){
        id = _id;
        tcpClient = _tcpClient;


        Server.clearDataEvent +=  ClearAllData;
        buffer = new byte[bufferSize];
        player = new Player();
    }

    public void ClearAllData(int _id){
        if (id != _id){
            return;
        }
        
        Console.WriteLine($"Disconnecting client ({id})...");
        tcpClient.Client.Disconnect(true);
        Console.WriteLine($"Client ({id}) disconnected.");
        
        Console.WriteLine($"Clearing data for client ({id})...");
        id = 0;
        tcpClient.GetStream().Close();
        tcpClient.GetStream().Dispose();
        tcpClient.Client.Close();
        tcpClient.Client.Dispose();
        tcpClient.Close();
        tcpClient.Dispose();
        tcpClient = default;
        GC.Collect();
        Console.WriteLine($"Data for client ({id}) has been cleared.");
    }


    

}
public class Player{
    public string name;
    public Color color;
    public Vector3 position;
    
    public float size = 3f; 
    public int score = 0;
    

    
}