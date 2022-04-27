using System.Net;
using System.Net.Sockets;
using System.Text;

public class Program{

    
    
    public static async Task Main(){

        var endpoint = new IPEndPoint(
            // IP-Address: Used with IP-Protocol to find the right computer
            IPAddress.Loopback, // 127.0.0.1 
            // Port: Used with TCP / UDP Protocol to find the right program on a computer
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