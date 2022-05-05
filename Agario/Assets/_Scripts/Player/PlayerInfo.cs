using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo : MonoBehaviour{
   public int id;
   public int score;
    
   public string name;
    
   public Color color;
   public Dictionary<int, PlayerInfo> playerDictionary;
    
   public float size;
   public float positionX;
   public float positionY;
}
