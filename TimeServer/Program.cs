using System.Net;
using System.Net.Sockets;
using System.Text;

public class Program{

    
    
    public static async Task Main(){

        var endpoint = new IPEndPoint(
            IPAddress.Loopback, // 127.0.0.1 
            10000
        );
        
        TcpListener tcpListener = new TcpListener(endpoint);
        
        while (true){
            tcpListener.Start();
            Console.WriteLine("Awaiting for connection");
            var tcpClient = await tcpListener.AcceptTcpClientAsync();
            Console.WriteLine("Connection accepted");
            
            string currentDateTime = DateTime.Now.ToString();
            string message = $"Connection Successful! Current time is : {currentDateTime}";
            var encodedMessage = Encoding.ASCII.GetBytes(currentDateTime);
            
            
            tcpClient.GetStream().Write(encodedMessage);
            tcpClient.GetStream().Close();
            tcpClient.Close();
        }
    }
    
    
}