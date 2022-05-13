using System;
using System.Collections;
using System.Collections.Generic;
using AgarioShared;
using UnityEngine;

public class PlayerScoreHandler : MonoBehaviour{
    Player player;
    PlayerInfo playerInfo;

    void Awake(){
        player = GetComponent<Player>();
        playerInfo = player.playerInfo;
    }
    

    void OnTriggerEnter2D(Collider2D col){
        Debug.Log($"{this.playerInfo.name} {playerInfo.id}Collision with: {col.name}");
        if (col.CompareTag("Player")){
            var otherPlayer = col.GetComponent<Player>();
            var otherPlayerInfo = otherPlayer.playerInfo;

            if (playerInfo.score > (otherPlayerInfo.score * otherPlayerInfo.scoreDefenseModifier)){
                playerInfo.score += otherPlayerInfo.score;
            }
            else if ((playerInfo.score * playerInfo.scoreDefenseModifier) < otherPlayerInfo.score){
                player.Die();
            }
        }

        if (col.CompareTag("Orb")){
            var otherOrb = col.GetComponent<Orb>();
            var otherOrbInfo = otherOrb.orbInfo;
            if (playerInfo.score > (otherOrbInfo.score * otherOrbInfo.scoreDefenseModifier)){
                playerInfo.score += otherOrbInfo.score;
                otherOrb.Die();
            }
        }
    }

    

   
}
