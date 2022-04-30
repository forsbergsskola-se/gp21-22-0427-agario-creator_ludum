using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class PersonalClient : MonoBehaviour{
   static readonly IPEndPoint serverEndpoint = new IPEndPoint(IPAddress.Loopback, 9000);
   static readonly IPEndPoint clientEndpoint = new IPEndPoint(IPAddress.Loopback, 9001);
   TcpClient tcpClient = new TcpClient(clientEndpoint);



   public void Connect(){
       tcpClient.Connect(serverEndpoint);
   }
}
