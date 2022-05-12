using System;
using System.Collections;
using System.Collections.Generic;
using AgarioShared;
using UnityEngine;

public class Orb : MonoBehaviour{
   public OrbInfo orbInfo;
   
  [SerializeField] SpriteRenderer spriteRenderer;
   ExecuteOnMainThread executeOnMainThread;

   void Awake(){
       executeOnMainThread = GameObject.FindWithTag("GameController").GetComponent<ExecuteOnMainThread>();
       DontDestroyOnLoad(gameObject);
   }
   
   
    #region SetVariablesRegion
       
       public void SetEverything(){
           Debug.Log($"Setting Everything  Orb({orbInfo.id})");
           SetPosition();
           SetColor();
           SetScale();
       }
   
       void SetPosition(){
           Debug.Log($"Setting Orb position... ({orbInfo.id})");
           var newPosition = new Vector3(orbInfo.positionX, orbInfo.positionY,0);
           Debug.Log("New  OrbPosition: "+newPosition);
           this.gameObject.transform.position = newPosition;
           Debug.Log($"Position Orb set. ({orbInfo.id})");
       }
   
       void SetColor(){
           Debug.Log($"Setting Orb color... ({orbInfo.id})");
           var newColor= new Color(orbInfo.colorR/255f, orbInfo.colorG/255f, orbInfo.colorB/255f);
           spriteRenderer.color = newColor;
           Debug.Log($"Color Orb Set. ({orbInfo.id})");
       }
   
       void SetScale(){
           Debug.Log($"Setting Orb Scale... ({orbInfo.id})");
           transform.localScale = new Vector3(orbInfo.size, orbInfo.size);
           Debug.Log($"Orb Scale set. ({orbInfo.id})");
       }
       
   
       #endregion

    

}
