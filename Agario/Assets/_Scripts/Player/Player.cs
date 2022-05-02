using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour{
    [SerializeField] PlayerSO playerSo;
    [SerializeField] PlayerUnityEventSO playerCreatedEventSo;
    
    Dictionary<int, Player> playerDictionary = new Dictionary<int, Player>();
    //Dictionary<int, Orb> orbDictionary = new Dictionary<int, Orb>();
    
    public Color color;
    public int id;
    public string name;

    float size;
    public float Size{
        get => size;
        private set{
             size = value;
             transform.localScale = new Vector3(Size, Size, 1);
        }
    }
    float score;
    public float Score{
        get => score;
        private set => score = value;
    }

    public Player(int _id, string _name, float _score, float _size){
        id = _id;
        name = _name;
        Score = _score;
        Size = _size;
        playerCreatedEventSo.playerUnityEventSo.Invoke(this);
    }

    public void SetName(string _name){
        name = _name;
    }
}