using System.Net;
using System.Net.Sockets;
public class Server{
    static byte[] buffer = new byte[4000]; //4kb
    static readonly int port = 9000;
    static readonly IPEndPoint hostEndpoint = new IPEndPoint(IPAddress.Loopback, port);
    static TcpListener hostListener;
    static NetworkStream stream;
    static int maxClients = 10;
    static Dictionary<int, ClientSlot> connectedClientDictionary;
    
    
    public static void Main(){
        hostListener = new TcpListener(hostEndpoint);

        connectedClientDictionary = new Dictionary<int, ClientSlot>(maxClients);

        CreateEmptyClientSlots(); 
        
        Console.WriteLine($"Starting server...");
        hostListener.Start();
        Console.WriteLine($"Server Started (Port: {port})");
        
        //Begins the connection process, but creates a new thread which deals with finishing it.
        Console.WriteLine("Waiting for connection...");
        hostListener.BeginAcceptTcpClient(BeginAcceptTcpClientCallback, hostListener);
        
        Console.Read();
        
        
    }

    static void CreateEmptyClientSlots(){
        for (int i = 1; i <= maxClients; i++){
            connectedClientDictionary.Add(i, new ClientSlot(i, null));
        }
    }

    static void BeginAcceptTcpClientCallback(IAsyncResult result){
        //finishes the connection and assigns client to variable
        var tcpClient = hostListener.EndAcceptTcpClient(result);
        Console.WriteLine("Connection established.");
        TryAssignClientToDictionary(tcpClient);
        
        //Ensure we keep listening for new clients
        Console.WriteLine("Waiting for connection...");
      hostListener.BeginAcceptTcpClient(BeginAcceptTcpClientCallback, hostListener);

    }

    static void TryAssignClientToDictionary(TcpClient? tcpClient){
        for (int i = 1; i <= connectedClientDictionary.Count; i++){
            if (connectedClientDictionary[i].tcpClient == default){
                connectedClientDictionary[i] = new ClientSlot(i, tcpClient);
                Console.WriteLine($"New Client accepted: ({tcpClient}, Id: {i})");
                return;
            }
        }

        Console.WriteLine("No available Client Slots");
        //TODO: Disconnect Client
    }
}

internal class ClientSlot{
    public int id;
    public TcpClient? tcpClient;

    public ClientSlot(int _id, TcpClient? _tcpClient){
        id = _id;
        tcpClient = _tcpClient;
    }
}