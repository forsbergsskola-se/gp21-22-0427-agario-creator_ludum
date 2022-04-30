using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class PersonalClient : MonoBehaviour{ //Using outdated Begin/End way to not have to deal with async in unity rn
    [SerializeField] ByteArrayUnityEventSo sendableInformationSo;
    
    static readonly IPEndPoint serverEndpoint = new IPEndPoint(IPAddress.Loopback, 9000);
    static readonly IPEndPoint clientEndpoint = new IPEndPoint(IPAddress.Loopback, 9001);
    static readonly int bufferSize = 4000;
   
   
    TcpClient tcpClient;
    NetworkStream stream;
    byte[] buffer = new byte[bufferSize];



    public void Connect(){
        tcpClient = new TcpClient(clientEndpoint);
       
        Debug.Log("Begin looking for connection...");
        tcpClient.BeginConnect(serverEndpoint.Address,serverEndpoint.Port,BeginConnectCallback,tcpClient);

        
        sendableInformationSo.dataUnityEventSo.AddListener(WriteOnStream);
        
        
        //TEST
        byte[] testBuffer = new byte[100];
        testBuffer = Encoding.ASCII.GetBytes("Hello");
        sendableInformationSo.dataUnityEventSo.Invoke(testBuffer);
        //END OF TEST
    }
    
    void BeginConnectCallback(IAsyncResult callbackResult){
        tcpClient.EndConnect(callbackResult);
        stream = tcpClient.GetStream();
        Debug.Log("Connection established.");
    }

    #region WriteOnStreamRegion

    void WriteOnStream(byte[] data){ //Because Unity is silly and cant directly send the data to a task :(
        Debug.Log("Received event with data to send, forwarding...");
        WriteOnStreamTask(data);
    }
   
    async Task WriteOnStreamTask(byte[] data){
        
        Debug.Log("Attempting to send data to host...");
        stream.Write(data);
        Debug.Log("Data sent to host.");
    }

    #endregion
   
   
    void OnApplicationQuit(){
        stream.Close();
        tcpClient.Close(); 
    }
}