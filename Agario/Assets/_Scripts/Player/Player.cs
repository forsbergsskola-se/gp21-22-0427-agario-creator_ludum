using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using AgarioShared;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;


public class Player : MonoBehaviour{
    [SerializeField]  IntUnityEventSo scoreUGUIEventSO;
    [SerializeField] bool isMainPlayer;
    [SerializeField] IntSo intSo;
    
    public UnityEventSO playerReadyEventSo;
    public PlayerInfo playerInfo;
    
    ExecuteOnMainThread executeOnMainThread;
    TextMeshPro namePlate;
    SpriteRenderer spriteRenderer;

    void Awake(){
        spriteRenderer = GetComponent<SpriteRenderer>();
        namePlate = GetComponentInChildren<TextMeshPro>();
        executeOnMainThread = GameObject.FindWithTag("GameController").GetComponent<ExecuteOnMainThread>();
    }

    void Start(){
        executeOnMainThread.Execute(() => playerReadyEventSo.unityEventSo.AddListener(()=>executeOnMainThread.Execute(SetEverything)));
    }

    
    #region SetVariablesRegion
    
    public void SetEverything(){
        Debug.Log($"Setting Everything ({playerInfo.id})");
        SetPosition();
        SetColor();
        SetScale();
        SetDisplayName();
        SetDisplayScore();
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
        if (!isMainPlayer){
            Debug.Log($"Player : {playerInfo.name} + ({playerInfo.id}) is not MainPlayer and will not display Score atm.");
            return;
        }
        Debug.Log($"Setting Everything ({playerInfo.id})");
        scoreUGUIEventSO.intUnityEventSo.Invoke(playerInfo.score);
        intSo.intSO = playerInfo.score;
    }
    

    void SetDisplayName(){
        Debug.Log($"Setting Everything ({playerInfo.id})");
        namePlate.text = playerInfo.name;
    }

    #endregion
}
