using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Color = System.Drawing.Color;

public class Player : MonoBehaviour{
     
    public UnityEventSO playerReadyEventSo;
    public PlayerInfo playerInfo;
    public  PlayerInfo[] allActivePlayersArray;
    
    SpriteRenderer spriteRenderer;
    

    void Awake(){
        playerInfo = new PlayerInfo();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start(){
        playerReadyEventSo.unityEventSo.AddListener(SetEverything);
    }

    #region SetVariablesRegion

    void SetEverything(){
        Debug.Log($"Setting Everything ({playerInfo.id})");
        SetPosition();
        SetColor();
        SetScale();
        //SetDisplayName();
        //SetDisplayScore();
    }

    void SetPosition(){
        Debug.Log($"Setting position... ({playerInfo.id})");
        var newPosition = new Vector3(playerInfo.positionX, playerInfo.positionY,0);
        Debug.Log("New Position: "+newPosition);
        this.gameObject.transform.position = newPosition;
        Debug.Log($"Position set. ({playerInfo.id})");
    }

    void SetColor(){
        Debug.Log($"Setting color... ({playerInfo.id})");
        spriteRenderer.color = new UnityEngine.Color(playerInfo.colorR, playerInfo.colorG, playerInfo.colorB);
        Debug.Log($"Color Set. ({playerInfo.id})");
    }

    void SetScale(){
        Debug.Log($"Setting Scale... ({playerInfo.id})");
        transform.localScale = new Vector3(playerInfo.size, playerInfo.size);
        Debug.Log($"Scale set. ({playerInfo.id})");
    }

    void SetDisplayScore(){
        Debug.Log($"Setting Everything ({playerInfo.id})");
        //blabla = playerInfo.score;
        throw new NotImplementedException();
    }

    void SetDisplayName(){
        Debug.Log($"Setting Everything ({playerInfo.id})");
        //blabla = playerInfo.name;
        throw new NotImplementedException();
    }

    #endregion
}
[Serializable]public class PlayerInfo{
         public int id;
         public int score;
     
         public string name;
         
         public int colorR;
         public int colorG;
         public int colorB;

         public float size;
         public float positionX;
         public float positionY;
     }