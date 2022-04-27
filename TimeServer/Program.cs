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
            string currentDateTime = DateTime.Now.ToString();
            tcpListener.Start();
            Console.WriteLine("Awaiting for connection");
            var tcpClient = await tcpListener.AcceptTcpClientAsync();
            Console.WriteLine("Connection accepted");
            var executeAllTasksTask = new Task(() => {
                var connectedClient = tcpClient;
                while (connectedClient.Connected){
                    Console.WriteLine("Connection stable");
                    var encodedMessage = Encoding.ASCII.GetBytes(currentDateTime);
                    tcpClient.GetStream().Write(encodedMessage);
                }

                Console.WriteLine("Connection Terminated");
                tcpClient.GetStream().Close();
                tcpClient.Close();
            });
            executeAllTasksTask.Start();
            
            // string currentDateTime = DateTime.Now.ToString();
            // string message = $"Connection Successful! Current time is : {currentDateTime}";
            // var encodedMessage = Encoding.ASCII.GetBytes(currentDateTime);
            //
            //
            //tcpClient.GetStream().Write(encodedMessage);
            tcpClient.GetStream().Close();
            tcpClient.Close();
        }
    }
    
    
    
    
}