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
                //Current Position [when changed] (UDP) {P1},{P∞}               <------Property with event
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












            void Start(){
        HostServer().Start();
    }

    async Task HostServer(){
        IPEndPoint hostEndPoint = new IPEndPoint(IPAddress.Loopback, 20000);
        TcpListener hostListener = new TcpListener(hostEndPoint);
        hostListener.Start(); // Server Starts here
        
        while (true){
            var newClientTask = await hostListener.AcceptTcpClientAsync(); // Waits for New Client
            new Task(() => ClientConnection(hostListener,newClientTask).Start()).Start(); // Creates and starts a new task which will handle the new client
        }
    }
    async Task ClientConnection(TcpListener hostListener,TcpClient tcpClient){ 
        // IPEndPoint remoteEP = default;

        byte[] buffer = new byte[100];
        tcpClient.GetStream().Read(buffer, 0, buffer.Length);
    }
}