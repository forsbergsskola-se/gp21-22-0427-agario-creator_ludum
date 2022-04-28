using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class SendStringFromInputFieldToHost : MonoBehaviour{
    [SerializeField] TextMeshProUGUI outputTextMeshProUGUI;
    TMP_InputField tmpInputField;
    
    void Awake(){
         tmpInputField = GetComponent<TMP_InputField>();
    }
    public void SendTextToHost(){
        var clientEndpoint = new IPEndPoint(IPAddress.Loopback, 10003);
        var serverEndpoint = new IPEndPoint(IPAddress.Loopback, 10002);
        UdpClient udpClient = new UdpClient(clientEndpoint);
        byte[] encodedMessage = Encoding.ASCII.GetBytes(tmpInputField.text);
        
        Debug.Log("Sending message...");
        udpClient.Send(encodedMessage, encodedMessage.Length, serverEndpoint);
        Debug.Log("Message Sent.");
        
        Debug.Log("Receiving message...");
        var encodedResponse = udpClient.Receive(ref serverEndpoint);
        Debug.Log("Message received.");
        
        var decodedResponse = Encoding.ASCII.GetString(encodedResponse);
        if (decodedResponse == ""){
            throw new InvalidDataException($"Message ({tmpInputField.text}) was not accepted by host.");
        }
        
        outputTextMeshProUGUI.text = decodedResponse;

    }
}
