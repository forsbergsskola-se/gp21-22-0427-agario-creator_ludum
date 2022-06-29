using System;
using System.Collections;
using System.Collections.Generic;
using Network;
using UnityEngine;

public class UpdatePlayerPositionValues : MonoBehaviour{
    [SerializeField] MessageHandler messageHandler;
    [SerializeField] BoolSO gameSceneIsActiveBoolSo;
    Player player;
    
    Vector2 TempPosition{
        set{
            player.playerInfo.positionX = value.x;
            player.playerInfo.positionY = value.y;
            messageHandler.PrepareThenSendUdpMessages("PositionMessage");
        }
    }

    void Awake(){
        player = GetComponent<Player>();
    }

    void FixedUpdate(){
        if (!gameSceneIsActiveBoolSo.boolSo){
            return;
        }
        TempPosition = transform.position;
    }
}
