using System.Diagnostics;
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

            var udpReceivedDataAsString = Encoding.ASCII.GetString(udpReceivedData).Trim();
            
            if (udpReceivedData == null || !IsWordCheck(udpReceivedDataAsString)){
                //throw new NullReferenceException();
                Console.WriteLine("Received Invalid Package, discarding");
                Console.WriteLine("Sending empty response...");
                await server.SendAsync(Encoding.ASCII.GetBytes(""),remoteEP);
                Console.WriteLine("Empty response sent.");
                continue;
            }

            message += " " + udpReceivedDataAsString;
            
            Console.WriteLine(message);
            Console.WriteLine("Sending Data...");
            //await succsessMessage;
            await server.SendAsync(Encoding.ASCII.GetBytes(message+"\n"),remoteEP);
            Console.WriteLine("Data Sent.");
            Console.WriteLine("Closing Client...");
            Console.WriteLine("Client Closed.");
        }
        server.Close();
    }
    
    static bool IsWordCheck(String text) {
        return (text.Length > 0 && text.Length <= 20 && !(text.Split(",.;: ".ToCharArray()).Length > 1));
    }
}