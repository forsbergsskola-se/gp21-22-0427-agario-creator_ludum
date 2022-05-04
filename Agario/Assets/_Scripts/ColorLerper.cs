using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorLerper : MonoBehaviour{
    [SerializeField] Slider redSlider;
    [SerializeField] Slider greenSlider;
    [SerializeField] Slider blueSlider;
    [SerializeField] PlayerInfo playerInfo;
   
    Image displayPlayer;

  

    void Start(){
        displayPlayer = GetComponent<Image>();
        redSlider.onValueChanged.AddListener(ChangeRedColor);
        greenSlider.onValueChanged.AddListener(ChangeGreenColor);
        blueSlider.onValueChanged.AddListener(ChangeBlueColor);
       
        SetPlayerColor(displayPlayer.color);
    }

    void SetPlayerColor(Color _color){
        playerInfo.color = _color;
    }

    void ChangeRedColor(float _value){
        displayPlayer.color = new Color(_value, displayPlayer.color.g, displayPlayer.color.b);
        SetPlayerColor(displayPlayer.color);
    }
  
    void ChangeGreenColor(float _value){
        displayPlayer.color = new Color(displayPlayer.color.r, _value, displayPlayer.color.b);
        SetPlayerColor(displayPlayer.color);
    } 
    void ChangeBlueColor(float _value){
        displayPlayer.color = new Color(displayPlayer.color.r, displayPlayer.color.g, _value);
        SetPlayerColor(displayPlayer.color);
    }
}