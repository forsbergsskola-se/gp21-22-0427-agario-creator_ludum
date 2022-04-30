using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Scripting;

public class PersonalClient : MonoBehaviour{ //Using outdated Begin/End way to not have to deal with async in unity rn
    [SerializeField] ByteArrayUnityEventSo sendableInformationSo;

    static IPEndPoint serverEndPoint;
    static IPEndPoint clientEndPoint;
    static readonly int bufferSize = 4000;
   
   
    TcpClient tcpClient;
    NetworkStream stream;
    byte[] buffer = new byte[bufferSize];



    public void Connect(){
        serverEndPoint = new IPEndPoint(IPAddress.Loopback, 9000);
        clientEndPoint = new IPEndPoint(IPAddress.Loopback, 9002);
        
        tcpClient = new TcpClient(clientEndPoint);
       
        Debug.Log("Begin looking for connection...");
        tcpClient.BeginConnect(serverEndPoint.Address,serverEndPoint.Port,BeginConnectCallback,tcpClient);

        
        sendableInformationSo.dataUnityEventSo.AddListener(WriteOnStream);
    }

    
    
    void BeginConnectCallback(IAsyncResult callbackResult){
        tcpClient.EndConnect(callbackResult);
        Debug.Log("Connection established.");
        
    }

    #region WriteOnStreamRegion

    void WriteOnStream(byte[] data){ //Because Unity is silly and cant directly send the data to a task :(
        Debug.Log("Received event with data to send, forwarding...");
        
        Debug.Log("Connecting stream...");
        stream = tcpClient.GetStream();
        Debug.Log("Stream connected.");
        
        WriteOnStreamTask(data);
    }
   
    async Task WriteOnStreamTask(byte[] data){
        
        Debug.Log("Attempting to send data to host...");
        stream.BeginWrite(data, 0, data.Length, BeginWriteCallback, stream);
        Debug.Log("Data sent to host.");
    }

    void BeginWriteCallback(IAsyncResult result){
        stream.EndWrite(result);
        stream.Close();
    }

    #endregion

    public void SendTestData(){
        //TEST
        byte[] testBuffer = new byte[100];
        testBuffer = Encoding.ASCII.GetBytes("Hello");
        sendableInformationSo.dataUnityEventSo.Invoke(testBuffer);
        //END OF TEST
    }
   
   
    void OnApplicationQuit(){
        stream.Close();
        stream.Dispose();
        tcpClient.Close();
        tcpClient.Dispose();
        tcpClient = null;
        GC.Collect();
    }
}