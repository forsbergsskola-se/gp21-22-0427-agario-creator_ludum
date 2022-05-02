using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class RequestServerTime : MonoBehaviour{

    [SerializeField] UnityEventSO messageSenderEventSo;
    
    public void SendsRequest(){
        SendRequestAsync();
    }

    public async Task SendRequestAsync(){
        
        IPEndPoint serverEndpoint = new IPEndPoint(IPAddress.Loopback, 10000);
        IPEndPoint clientEndpoint = new IPEndPoint(IPAddress.Loopback, 10001);
        TcpClient tcpClient = new TcpClient(clientEndpoint);
                    
        Debug.Log("Attempting to Connect...");
        await tcpClient.ConnectAsync(serverEndpoint.Address, serverEndpoint.Port);
        Debug.Log("Connection accepted.");
            
        byte[] buffer = new byte[100];
        tcpClient.GetStream().Read(buffer,0, 100);
        messageSenderEventSo.unityEventSo.Invoke(Encoding.ASCII.GetString(buffer));
        Debug.Log(Encoding.ASCII.GetString(buffer));
        tcpClient.GetStream().Close();
        Debug.Log("Stream Closed");
        tcpClient.Close();
        Debug.Log("Client closed");

    }
    
}