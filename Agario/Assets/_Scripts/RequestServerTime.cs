using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class RequestServerTime : MonoBehaviour{

    [SerializeField] UnityEventSO messageSenderEventSo;
    
    static IPEndPoint serverEndpoint = new IPEndPoint(IPAddress.Loopback, 10000);
    static IPEndPoint clientEndpoint = new IPEndPoint(IPAddress.Loopback, 10001);
    static TcpClient tcpclient = new TcpClient(clientEndpoint);
    
    
    

    public void Connect(){
        SendRequest();
    }
    public void Disconnect(){
        
        DisconnectAsync();
    }
    

    public async Task SendRequest(){
        
        Debug.Log("Attempting to Connect...");
        await tcpclient.ConnectAsync(serverEndpoint.Address, serverEndpoint.Port);
        Debug.Log("Connection accepted.");

        var readFromStreamWhileConnected = new Task(() => {
            while (tcpclient.Connected){
                byte[] buffer = new byte[100];
                tcpclient.GetStream().Read(buffer, 0, 100);
                messageSenderEventSo.unityEventSo.Invoke(Encoding.ASCII.GetString(buffer));
                Debug.Log(Encoding.ASCII.GetString(buffer));
            }
        });
        readFromStreamWhileConnected.Start();


    }

    public async Task DisconnectAsync(){
        tcpclient.GetStream().Close();
        Debug.Log("Stream Closed");
        tcpclient.Close();
        Debug.Log("Client closed");
    }
    
}