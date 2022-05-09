using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;


public class Player : MonoBehaviour{
    public ExecuteOnMainThread executeOnMainThread;
    public UnityEventSO playerReadyEventSo;
    public PlayerInfo playerInfo;
    public  PlayerInfo[] allActivePlayersArray;
    
    SpriteRenderer spriteRenderer;
    Color colorr;

    void Awake(){
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start(){
        executeOnMainThread.Execute(() => playerReadyEventSo.unityEventSo.AddListener(()=>executeOnMainThread.Execute(SetEverything)));
    }

    #region SetVariablesRegion
    
    void SetEverything(){
        Debug.Log($"Setting Everything ({playerInfo.id})");
        SetPosition();
        SetColor();
        SetScale();
        //SetDisplayName();
        //SetDisplayScore();
        Debug.Log("Invoking Player Ready Event");
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
        var newColor= new Color(playerInfo.colorR/255f, playerInfo.colorG/255f, playerInfo.colorB/255f);
        spriteRenderer.color = newColor;
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
         public float baseMovementSpeed; //TODO Assign
     }