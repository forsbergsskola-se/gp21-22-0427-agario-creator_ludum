using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerClient : MonoBehaviour
{
    [SerializeField] DataUnityEventSo newPlayerDataReadyToSendSo;
    byte[][] dataToSend;

    public int id;

    void Awake(){
        DontDestroyOnLoad(this);
    }

    void Start(){
        newPlayerDataReadyToSendSo.dataUnityEventSo.AddListener(StoreNewPlayerCreatedData);
    }

    public async void Connect(){
       await ConnectAsync();
    }

    public async Task ConnectAsync(){
        IPEndPoint hostEndpoint = new IPEndPoint(IPAddress.Loopback, 20000);
        IPEndPoint clientEndpoint = new IPEndPoint(IPAddress.Loopback, 20001);
        TcpClient tcpClient = new TcpClient(clientEndpoint);

        Debug.Log("Attempting to Connect...");
        await tcpClient.ConnectAsync(hostEndpoint.Address, hostEndpoint.Port);
        Debug.Log("Connection established");
        
        
        
    }

    public void Disconnect(){
        
    }
    void StoreNewPlayerCreatedData(byte[][] newData){
        dataToSend = newData;
    }

}
