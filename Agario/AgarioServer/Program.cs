using System.Net;
using System.Net.Sockets;
using System.Text;

public class Server{
    static readonly int bufferSize = 4000;
    static byte[] buffer = new byte[bufferSize]; //4kb
    static readonly int port = 9000;
    static readonly IPEndPoint hostEndpoint = new IPEndPoint(IPAddress.Loopback, port);
    static TcpListener hostListener;
    static int maxClients = 10;
    static Dictionary<int, ClientSlot> connectedClientDictionary;
    
    
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
            connectedClientDictionary.Add(i, new ClientSlot(i, null));
        }
    }

    static async Task ListenForTcpClientsTask(){
        while (true){ 
            Console.WriteLine("Waiting for connection...");
            var tcpClient = await hostListener.AcceptTcpClientAsync();
            
            Console.WriteLine($"New Client accepted.");
            var activatedClientSlot = TryAssignClientToDictionary(tcpClient);
            
            ReadFromStreamTask(activatedClientSlot);
        }
    }

    static ClientSlot TryAssignClientToDictionary(TcpClient tcpClient){
        for (int i = 1; i <= connectedClientDictionary.Count; i++){
            if (connectedClientDictionary[i].tcpClient == default){
                connectedClientDictionary[i] = new ClientSlot(i, tcpClient);
                Console.WriteLine($"New Client: ({tcpClient}, Id: {i}).");
                return connectedClientDictionary[i];
            }
        }

        Console.WriteLine("No available Client Slots");
        //TODO: Disconnect Client
        throw new NotImplementedException();
        return null;
    }

    static async Task ReadFromStreamTask(ClientSlot clientSlot){
        var tcpClient = clientSlot.tcpClient;
        var id = clientSlot.id;
        var stream = clientSlot.tcpClient.GetStream();

        while (true){
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
            Console.WriteLine(Encoding.ASCII.GetString(buffer));
        }
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
    }

    
    
}