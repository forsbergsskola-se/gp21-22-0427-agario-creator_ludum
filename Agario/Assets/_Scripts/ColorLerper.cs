using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorLerper : MonoBehaviour{
    [SerializeField] Slider redSlider;
    [SerializeField] Slider greenSlider;
    [SerializeField] Slider blueSlider;
   
    Image displayPlayer;

    [HideInInspector] public Color playerColor;

    void Start(){
        displayPlayer = GetComponent<Image>();
        redSlider.onValueChanged.AddListener(ChangeRedColor);
        greenSlider.onValueChanged.AddListener(ChangeGreenColor);
        blueSlider.onValueChanged.AddListener(ChangeBlueColor);
        playerColor = displayPlayer.color;
    }


    void ChangeRedColor(float _value){
        displayPlayer.color = new Color(_value, displayPlayer.color.g, displayPlayer.color.b);
        playerColor.r = _value;
    }
  
    void ChangeGreenColor(float _value){
        displayPlayer.color = new Color(displayPlayer.color.r, _value, displayPlayer.color.b);
        playerColor.g = _value;
    } 
    void ChangeBlueColor(float _value){
        displayPlayer.color = new Color(displayPlayer.color.r, displayPlayer.color.g, _value);
        playerColor.b = _value;
    }
}