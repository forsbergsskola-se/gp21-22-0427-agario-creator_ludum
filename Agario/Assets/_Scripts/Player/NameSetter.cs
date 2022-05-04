using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NameSetter : MonoBehaviour{
    [SerializeField] PlayerInfo playerInfo;
    TMP_InputField nameInputField;
    
    void Start(){
        nameInputField = GetComponent<TMP_InputField>();
        nameInputField.onValueChanged.AddListener(SetPlayerName);
    }


    void SetPlayerName(string _name){
        playerInfo.name = _name;
    }
}
