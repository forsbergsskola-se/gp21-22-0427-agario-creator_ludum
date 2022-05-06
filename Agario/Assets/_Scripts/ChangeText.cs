using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class ChangeText : MonoBehaviour{
    [SerializeField] StringUnityEventSO updateEventSo;
    TextMeshProUGUI textMeshProUGUI;

    void Awake(){
        textMeshProUGUI = GetComponent<TextMeshProUGUI>();
    }

    void OnEnable(){
        updateEventSo.stringUnityEventSo.AddListener(UpdateText);
    }

    void OnDisable(){
        updateEventSo.stringUnityEventSo.RemoveListener(UpdateText);
    }

    void UpdateText(string text){
        textMeshProUGUI.text = text;
    }
    
}
