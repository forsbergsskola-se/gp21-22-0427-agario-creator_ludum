using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Scripting;
using Newtonsoft.Json;

public class PersonalClient : MonoBehaviour{ //Using outdated Begin/End way to not have to deal with async in unity rn

    [SerializeField] PlayerInfo playerInfo;
    [SerializeField] Vector2SO mapSizeSo;
    
    static IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Loopback, 9000);


    TcpClient tcpClient = new TcpClient();
    StreamReader streamReader;
    StreamWriter streamWriter;
    NetworkStream stream;

    string nameCriteria = ",.;:0123456789 ";
    const int maxNameLenght = 15;

    bool _isConnected;
    public bool IsConnected{
        get => _isConnected;
        private set{
            _isConnected = value;
            if (_isConnected){
                SendConnectToServerData();
                ListenForMessageAsync().Start();
            }
        }
    }
    

    public void Connect(){
        

        if (!IsValidNameCheck(playerInfo.name)){
            Debug.Log($"Please Input a name without a space or any of the following characters: {nameCriteria}");
            return;
        }
        
        new Task(() => ConnectToServer().Start()).Start();
    }

    async Task ConnectToServer(){
        Debug.Log("Begin looking for connection...");
        await tcpClient.Client.ConnectAsync(serverEndPoint);
        Debug.Log("Connection established.");
        
        Debug.Log("Connecting stream...");
        stream = tcpClient.GetStream();
        Debug.Log("Stream connected.");

        streamReader = new StreamReader(stream);
        streamWriter = new StreamWriter(stream);
        streamWriter.AutoFlush = true;
        IsConnected = true; // Calls SendConnectToServerData
    }
    

    public void SendConnectToServerData(){
        var playerInfoMessage = new ConnectToServerMessage(){
            messageName = "ConnectToServerMessage",
            name = this.playerInfo.name,
            color = this.playerInfo.color
            
        };

        new Task(() =>SendMessageAsync(playerInfoMessage).Start()).Start();
    }


    async Task SendMessageAsync<T>(T message){
        Debug.Log("Attempting to send data to host...");
        await streamWriter.WriteLineAsync(JsonUtility.ToJson(message));
        Debug.Log("Data sent to host.");
        await streamWriter.FlushAsync();
    }

    async Task ListenForMessageAsync(){
        while (IsConnected){
            Debug.Log("Awaiting data...");
            var jsonString = await streamReader.ReadLineAsync();
            Debug.Log("Data Received.");
            
            if (jsonString == default){
                Debug.Log("Data is null, discarding.");
                throw new NotImplementedException();
                return;
            }
                    
            ReadFromJsonTask(jsonString).Start();
        }
        
    }

    async Task ReadFromJsonTask(string jsonString){
        var message = JsonUtility.FromJson<Message>(jsonString);
        Debug.Log($"Data is of type: {message.messageName}");
        if (message.messageName == "InitialServerToClientMessage"){
            var initialMessage = JsonUtility.FromJson<InitialServerToClientMessage>(jsonString);
            playerInfo.id = initialMessage.id;
            playerInfo.score = initialMessage.score;
            playerInfo.size = initialMessage.size;
            
            playerInfo.positionX = initialMessage.positionX;
            playerInfo.positionY = initialMessage.positionY;
            
            
            var newMapSize = new Vector2(initialMessage.mapSizeX, initialMessage.mapSizeX);
            mapSizeSo.vector2 = newMapSize;
            
            playerInfo.playerDictionary.Add(initialMessage.id,initialMessage.playerDictionary[initialMessage.id]);
             
            // mapSizeSo.vector2.x = initialMessage.mapSizeX;
            // mapSizeSo.vector2.y = initialMessage.mapSizeY;
            Debug.Log("newPlayerPosition : "+ $"({initialMessage.positionX+ ", " +initialMessage.positionX})");
            Debug.Log("newMapSize : "+newMapSize);

            foreach (var player in playerInfo.playerDictionary){
               Debug.Log($"Player: {player.Value.name} ({player.Value.id})");
            }
            
        }
        else{
            throw new NotImplementedException("Currently Only InitialServerToClientMessage accepted message type");
        }
    }


    bool IsValidNameCheck(String _name) {
        return (_name.Length is > 0 and <= maxNameLenght && !(_name.Split(nameCriteria.ToCharArray()).Length > 1));
    }
   
   
    void OnApplicationQuit(){
        if (tcpClient != null){
            stream.Close();
            stream.Dispose();
            tcpClient.Client.Close();
            tcpClient.Dispose();
            GC.Collect();
        }
    }
}