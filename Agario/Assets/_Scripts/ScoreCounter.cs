using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreCounter : MonoBehaviour{
    [SerializeField] IntUnityEventSo scoreUGUIEventSO;
    TextMeshProUGUI textMeshProUGUI;

    void Awake(){
        textMeshProUGUI = GetComponent<TextMeshProUGUI>();
    }

    void Start(){
        scoreUGUIEventSO.intUnityEventSo.AddListener(SetScoreCounter);
    }

    void SetScoreCounter(int score){
        textMeshProUGUI.text = score.ToString();
    }
}
