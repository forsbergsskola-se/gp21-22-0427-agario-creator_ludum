using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Color = System.Drawing.Color;

public class Player : MonoBehaviour{
    public PlayerInfo playerInfo = new();
    public Dictionary<string, PlayerInfo> playerDictionary;
}
[Serializable]public class PlayerInfo{
         public int id;
         public int score;
     
         public string name = "TestName";
         
         public int colorR;
         public int colorG;
         public int colorB;

         public float size;
         public float positionX;
         public float positionY;
     }