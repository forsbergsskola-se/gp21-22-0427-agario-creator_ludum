using System.Net;
using System.Net.Sockets;
public class Server{
    static byte[] buffer = new byte[4000]; //4kb
    static readonly int port = 9000;
    static readonly IPEndPoint hostEndpoint = new IPEndPoint(IPAddress.Loopback, port);
    static TcpListener hostListener;
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
        Console.WriteLine("Starting task to listen for new tcp Clients");
        ListenForTcpClients();
        Console.Read();
        
        
    }

    static void CreateEmptyClientSlots(){
        for (int i = 1; i <= maxClients; i++){
            connectedClientDictionary.Add(i, new ClientSlot(i, null));
        }
    }

    static async Task ListenForTcpClients(){
        while (true){ 
            Console.WriteLine("Waiting for connection...");
            var tcpClient = await hostListener.AcceptTcpClientAsync();
            TryAssignClientToDictionary(tcpClient);
        }
    }

    static void TryAssignClientToDictionary(TcpClient tcpClient){
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
    public TcpClient tcpClient;
    public NetworkStream stream;
    public int bufferSize = 4000;
    public byte[] buffer;//4kb
    public ClientSlot(int _id, TcpClient _tcpClient){
        id = _id;
        tcpClient = _tcpClient;
        
        buffer = new byte[bufferSize];
        ReadFromStream();
    }

    async Task ReadFromStream(){
        stream = tcpClient.GetStream();
        Console.WriteLine($"Listening for data stream from {tcpClient} ({id}).");
        var receivedByteSize = await stream.ReadAsync(buffer,0,bufferSize);
        Console.WriteLine($"Received data stream from {tcpClient} ({id}).");
        if (receivedByteSize <= 0){
            //No data received
            Console.WriteLine($"Data stream from {tcpClient} ({id}) was empty, discarding.");
            return;
        }
        byte[] receivedDataBuffer = new byte[receivedByteSize];

        //Copies the changed data in the buffer with the size gotten, onto a new array holding only the relevant new data
        Array.Copy(buffer, receivedDataBuffer, receivedByteSize);
    }
    
}