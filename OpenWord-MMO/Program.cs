using System.Net;
using System.Net.Sockets;
using System.Text;

public class Program{
    static string message = "";
    public static async Task Main(){
        
        var serverEndpoint = new IPEndPoint(IPAddress.Loopback, 10002);
        var server = new UdpClient(serverEndpoint);

        while (true){
            IPEndPoint remoteEP = default;
            Console.WriteLine("Waiting for packet...");
            var udpReceivedData =  server.Receive(ref remoteEP);
            Console.WriteLine("packet received.");
            var succsessMessage =  server.SendAsync(Encoding.ASCII.GetBytes("Successful Transmission!" + "\n"),remoteEP);
            
            var udpReceivedDataAsString = Encoding.ASCII.GetString(udpReceivedData).Trim();
            if (udpReceivedData == null){
                throw new NullReferenceException();
            }
            
            if (!IsWord(udpReceivedDataAsString)){
                throw new InvalidDataException();
            }
            
            message += " " + udpReceivedDataAsString;
            
            Console.WriteLine(message);
            Console.WriteLine("Sending Data...");
            await succsessMessage;
            await server.SendAsync(Encoding.ASCII.GetBytes(message+"\n"),remoteEP);
            Console.WriteLine("Data Sent.");
            Console.WriteLine("Closing Client...");
            Console.WriteLine("Client Closed.");
        }
        server.Close();
        
        
    }
    
    static bool IsWord(String text) {
        return (text.Length > 0 && text.Length <= 20 && !(text.Split(",.;: ".ToCharArray()).Length > 1));
    }
}