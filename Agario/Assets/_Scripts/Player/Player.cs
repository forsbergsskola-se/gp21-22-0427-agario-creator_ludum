using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour{
    [SerializeField] PlayerSO playerSo;
    [SerializeField] PlayerUnityEventSO playerCreatedEventSo;

    int id;

    public int Id{
        get => id;
        private set => id = value;
    }

    string name;

    public string Name{
        get => name;
        private set => name = value;
    }
    float size;
    public float Size{
        get => size;
        private set => size = value;
    }
    float score;
    public float Score{
        get => score;
        private set => score = value;
    }

    public Player(int _id, string _name, float _score, float _size){
        Id = _id;
        Name = _name;
        Score = _score;
        Size = _size;
        playerCreatedEventSo.playerUnityEventSo.Invoke(this);
    }
}
