using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class Server : MonoBehaviour{
    public int MaxClients{ get; private set; } = 10;
    public int Port{ get; private set; } = 20000;
    public TcpListener hostListener;

    void Start(){
        hostListener = new TcpListener(IPAddress.Any, Port);
        
        Debug.Log($"Starting server (Port: {Port})...");
        hostListener.Start();
        Debug.Log($"Server Started (Port: {Port})");

        Debug.Log("Listening for connections...");
        //This Begins listening to connection, and when found, creates a new thread with the callback, and finishes connecting there
        hostListener.BeginAcceptTcpClient(BeginAcceptTcpClientCallback, hostListener);
    }

    void BeginAcceptTcpClientCallback(IAsyncResult callbackResult){
        var client =hostListener.EndAcceptTcpClient(callbackResult);
        Debug.Log($"Connection established with: {client.Client}");
        
        //Make sure we continue listening to new connections
        hostListener.BeginAcceptTcpClient(BeginAcceptTcpClientCallback, hostListener); 
    }
}
