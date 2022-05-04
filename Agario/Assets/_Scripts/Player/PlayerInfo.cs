using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo : MonoBehaviour{
   public int id;
   public string name;
   public Color color;
   public int score;
   public float size;

   Vector2 _position;
   public Vector2 Position{
      get => _position;
      set{
         _position = value;
         transform.position = _position;
      }
   }
}
