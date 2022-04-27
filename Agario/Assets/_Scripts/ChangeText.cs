using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class ChangeText : MonoBehaviour{
    [SerializeField] UnityEventSO updateEventSo;
    TextMeshProUGUI textMeshProUGUI;

    void Awake(){
        textMeshProUGUI = GetComponent<TextMeshProUGUI>();
    }

    void OnEnable(){
        updateEventSo.unityEventSo.AddListener(UpdateText);
    }

    void OnDisable(){
        updateEventSo.unityEventSo.RemoveListener(UpdateText);
    }

    void UpdateText(string text){
        textMeshProUGUI.text = text;
    }
    
}
