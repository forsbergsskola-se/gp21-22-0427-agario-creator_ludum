using System.Net.Sockets;
using AgarioShared;

namespace AgarioServer;

public class ClientSlot{
    public int id;
    public TcpClient tcpClient;
    public PlayerInfo playerInfo;
   
    
    public ClientSlot(int _id, TcpClient _tcpClient){
        id = _id;
        tcpClient = _tcpClient;
        
        playerInfo = new PlayerInfo{
            id = id
        };
    }

    public async Task ClearAllData(int _id){
        if (id != _id){
            return;
        }
        
        Console.WriteLine($"Disconnecting client ({id})...");
        tcpClient?.Client?.Disconnect(true);
        Console.WriteLine($"Client ({id}) disconnected.");
        
        Console.WriteLine($"Clearing data for client ({id})...");
        id = 0;
        tcpClient?.GetStream()?.Close();
        tcpClient?.GetStream()?.Dispose();
        tcpClient?.Client?.Close();
        tcpClient?.Client?.Dispose();
        tcpClient?.Close();
        tcpClient?.Dispose();
        //tcpClient = default;
        //GC.Collect();
        Console.WriteLine($"Data for client ({id}) has been cleared.");
    }
}