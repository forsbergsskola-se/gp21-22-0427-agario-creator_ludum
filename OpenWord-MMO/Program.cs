using System.Net;
using System.Net.Sockets;
using System.Text;

public class Program{
    
    public static async Task Main(){
        string message = "";
        var udpClient = new UdpClient("127.0.0.1",10002);
        var remoteEP = new IPEndPoint(IPAddress.Any, 10003); 

        while (true){
            Console.WriteLine("Waiting for connection...");
            var udpReceivedData =  udpClient.Receive(ref remoteEP);
            Console.WriteLine("Connection accepted.");
            var uudpReceivedDataAsString = udpReceivedData.ToString();
            
            if (udpReceivedData == null){
                throw new NullReferenceException();
            }

            if (!IsWord(uudpReceivedDataAsString)){
                throw new InvalidDataException();
            }

            message +=  uudpReceivedDataAsString + " ";

            Console.WriteLine("Sending Data...");
            await udpClient.SendAsync(Encoding.ASCII.GetBytes(message),remoteEP);
            Console.WriteLine("Data Sent.");
            Console.WriteLine("Closing Client...");
            udpClient.Close();
            Console.WriteLine("Client Closed.");
        }
        
        
    }
    
    static bool IsWord(String text) {
        return (text.Length > 0 && text.Length <= 20 && !(text.Split(",.;: ".ToCharArray()).Length > 1));
    }
}