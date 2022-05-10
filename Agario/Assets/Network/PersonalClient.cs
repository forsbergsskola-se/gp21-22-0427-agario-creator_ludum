using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using AgarioShared;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Scripting;
using Newtonsoft.Json;
using ILogger = AgarioShared.ILogger;

namespace Network{
    public class PersonalClient : MonoBehaviour{
        //Using outdated Begin/End way to not have to deal with async in unity rn

        [SerializeField] public Vector2SO mapSizeSo;
        [SerializeField]  Player player;
        
        PlayerInfo playerInfo;
        MessageHandler messageHandler;
        static IPEndPoint serverEndPoint;


        TcpClient tcpClient;
        internal StreamReader streamReader;
        internal StreamWriter streamWriter;
        NetworkStream stream;

        string nameCriteria;
         int maxNameLenght;

        bool _isConnected;

        public bool IsConnected{
            get => _isConnected;
            private set{
                _isConnected = value;
                if (IsConnected){
                    Console.WriteLine("Is Connected: "+IsConnected);
                    new Task(() => messageHandler.ListenForMessageAsync().Start()).Start();
                }
            }
        }
        void Awake(){
            DontDestroyOnLoad(gameObject);
            messageHandler = GetComponent<MessageHandler>();
        }

        void Start(){
            playerInfo = player.playerInfo;
            serverEndPoint = new IPEndPoint(IPAddress.Loopback, 9000);
            nameCriteria = ",.;:0123456789 ";
            maxNameLenght = 15;
            tcpClient = new TcpClient();
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
        

        bool IsValidNameCheck(String _name){
            return (_name.Length  > 0 && _name.Length <= maxNameLenght && !(_name.Split(nameCriteria.ToCharArray()).Length > 1));
        }
        
        void OnApplicationQuit(){
            if (tcpClient != null){
                _isConnected = false;
                stream?.Close();
                stream?.Dispose();
                tcpClient.Client?.Close();
                tcpClient?.Dispose();
                GC.Collect();
            }
        }

    }
}