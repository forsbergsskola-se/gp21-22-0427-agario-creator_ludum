using System.Net;
using System.Net.Sockets;
using System.Text;

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
                Console.WriteLine($"New Client: ({tcpClient.Client.RemoteEndPoint}, Id: {i}).");
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

        while (tcpClient.Connected){
            
             int bufferSize = 4000;
             char[] buffer = new char[bufferSize]; //4kb
           
            Console.WriteLine($"Listening for data stream from {address} ({id}).");
            var receivedByteSize = await streamReader.ReadAsync(buffer, 0, bufferSize);
            //var receivedByteSize = await stream.ReadAsync(buffer,0,bufferSize);
            Console.WriteLine($"Received data stream from {address} ({id}).");
            
            if (receivedByteSize <= 0){
                //No data received
                Console.WriteLine($"Data stream from {address} ({id}) was empty, discarding.");
                
                //Disconnecting Client
                clientSlot.ClearAllData(id);
                
                continue;
            }
            Console.WriteLine(Encoding.ASCII.GetString(Encoding.ASCII.GetBytes(buffer),0,receivedByteSize));
        }
    }
}

internal class ClientSlot{
    public int id;
    public TcpClient tcpClient;
    public int bufferSize = 4000;
    public byte[] buffer;//4kb
    
    public ClientSlot(int _id, TcpClient _tcpClient){
        id = _id;
        tcpClient = _tcpClient;


        Server.clearDataEvent +=  ClearAllData;
        buffer = new byte[bufferSize];
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