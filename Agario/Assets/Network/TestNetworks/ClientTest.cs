using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using UnityEditor.MemoryProfiler;
using UnityEngine;

public class ClientTest : MonoBehaviour{
    public static int bufferSize = 4096;
    public int id;
    public PlayerTCP playerTcp;
    

    public ClientTest(int _id){
        id = _id;
        playerTcp = new PlayerTCP(id);
    }

    public class PlayerTCP{

        public TcpClient tcpClient;
        NetworkStream stream;
        byte[] receiveBuffer;
        readonly int id;

        public PlayerTCP(int _id){
            id = _id;
        }


        public void Connect(TcpClient _tcpClient){
            tcpClient = _tcpClient;

            //Sets buffer sizes
            tcpClient.ReceiveBufferSize = bufferSize;
            tcpClient.SendBufferSize = bufferSize;

            stream = tcpClient.GetStream();
            
            receiveBuffer = new byte[bufferSize];

            //Begins reading when able and creating a new thread with the call back to deal with the data.
            stream.BeginRead(receiveBuffer, 0, bufferSize, BeginReadCallback, stream);
        }

        void BeginReadCallback(IAsyncResult callbackResult){
            try{
                int byteLenght =  stream.EndRead(callbackResult);
                //When nothing was read, return
                if (byteLenght <= 0){
                    return;
                }

                byte[] data = new byte[byteLenght];
                //This copies the lenght of the stream of the read data which was put on the sourceArray, onto the destinationArray
                Array.Copy(receiveBuffer,data,byteLenght);

            }
            catch (Exception e){
                Debug.Log($"Problem receiving data {e}");
                throw;
                //TODO: Disconnect Here
            }
           
            stream.BeginRead(receiveBuffer, 0, bufferSize, BeginReadCallback, stream);
        }
    }
}
