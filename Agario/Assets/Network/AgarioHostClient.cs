using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using UnityEngine;

public class AgarioHostClient : MonoBehaviour
{
    //Keywords:
            // [] == when or Index in stored list
            // () == data transmission type or when extra info needed
            // {} == order of transmission
        
        //Who provides what:
            //Player Client:
                //Current Position [when changed/on connection] (UDP) {P1},{P∞}               <------Property with event
                //Current Name [on connection] (TCP) {P1}
                //Current Score [when changed] (TCP) {P2}                       <------Property with event
                //Current Size [when changed] (TCP) {P2}                        <------Property with event
                //Orb[ID] Removed [when happens] (TCP) {P?}                     //Possible Cheating can happen here
                
            //Server Client:
                //To all Players:
                    //Joined Player(all player info) [when happens] (TCP) {S?}
                    //Disconnected Player ID [when happens] (TCP) {S?}
                    //Player[ID] Score [when changed/Forwarded] (TCP) {S?x}
                    //Player[ID] Size [when changed/Forwarded] (TCP) {S?x}
                    //Player[ID] Position [when changed/Forwarded] (UDP)  {S?y}
                    //Current Orb Position [when changed] (UDP) {S1},{S∞}        <------Property with event
                    //New Orb(Type/Position) [when happens] (TCP) {S?}
                    //Orb[ID] Removed [when happens/Forwarded] (TCP) {S?}
                //To New Player:
                    //All Active Players(all Player info) [on connection] (TCP) {S1}
                    //All Active Orbs (Type/Position) [on connection] (TCP) {S1}
                    //Player ID [on connection] (TCP) {S1}
                    //Current Position [on connection] () (TCP) {S1}        <------- Gets Random Position from server

        //What is not provided:
            //On Player Client:
                //Board
                //










                
    public Dictionary<int, TcpClient> ClientDictionary = new Dictionary<int, TcpClient>();
    public Dictionary<int, Player> PlayerDictionary = new Dictionary<int, Player>();
   [Tooltip("Changing the amount of activeClients increases the max possible clients")] public int[] ActiveClients = new int[]{};

   TcpListener hostListener;
   bool waitingForNewClient = false;

    void Start(){
        HostServer();
    }

    async Task HostServer(){
        IPEndPoint hostEndPoint = new IPEndPoint(IPAddress.Loopback, 20000);
        hostListener = new TcpListener(hostEndPoint);
        hostListener.Start(); // Server Starts here
        
        while (true){
            if (!waitingForNewClient){
                waitingForNewClient = true;
                await NewClientConnection(); //Creates new thread so doesnt stop main thread
            }
            
        }
    }

    async Task NewClientConnection(){
        var tcpClient = await AcceptNewTcpClientAsync();
       // var player = CreateNewPlayer();
       // AddClientAndPlayerToDictionary(tcpClient,player);
        waitingForNewClient = false;

        
        
        var readNewPlayerDataTask = ReadNewPlayerData(tcpClient);
        readNewPlayerDataTask.Start();
        
        var newPlayerData = await readNewPlayerDataTask;
        Debug.Log(newPlayerData);
    }

    void AddClientAndPlayerToDictionary(TcpClient tcpClient, Player player){
        for (int i = 0; i < ActiveClients.Length; i++){
            if (ActiveClients[i] == default){
                ActiveClients[i] = i;
                PlayerDictionary.Add(ActiveClients[i],player);
                ClientDictionary.Add(ActiveClients[i], tcpClient);
            }
        }
    }

    async Task<byte[]> ReadNewPlayerData(TcpClient tcpClient){ 
        // IPEndPoint remoteEP = default;
    
        byte[] buffer = new byte[100];
        await tcpClient.GetStream().ReadAsync(buffer, 0, buffer.Length);
        return buffer;
    }

    async Task<TcpClient> AcceptNewTcpClientAsync(){
        var newTcpClient = await hostListener.AcceptTcpClientAsync();
        return newTcpClient;
    }
}