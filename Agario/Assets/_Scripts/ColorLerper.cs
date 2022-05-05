using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorLerper : MonoBehaviour{
    [SerializeField] Slider redSlider;
    [SerializeField] Slider greenSlider;
    [SerializeField] Slider blueSlider;
    [SerializeField] Player player;
    PlayerInfo playerInfo;
    
   
    Image displayPlayer;

  

    void Start(){
        playerInfo = player.playerInfo;
        displayPlayer = GetComponent<Image>();
        redSlider.onValueChanged.AddListener(ChangeRedColor);
        greenSlider.onValueChanged.AddListener(ChangeGreenColor);
        blueSlider.onValueChanged.AddListener(ChangeBlueColor);
       
        playerInfo.colorR = (int) (displayPlayer.color.r*255);
        // Debug.Log("Red: "+playerInfo.colorR);
        playerInfo.colorG = (int) (displayPlayer.color.g*255);
        playerInfo.colorB = (int) (displayPlayer.color.b*255);
    }
    

    void ChangeRedColor(float _value){
        displayPlayer.color = new Color(_value, displayPlayer.color.g, displayPlayer.color.b);
        // Debug.Log("Red: "+playerInfo.colorR);
        playerInfo.colorR = (int) (_value * 255);
    }
  
    void ChangeGreenColor(float _value){
        displayPlayer.color = new Color(displayPlayer.color.r, _value, displayPlayer.color.b);
        // Debug.Log("Green: "+playerInfo.colorG);
        playerInfo.colorG = (int) (_value * 255);
    } 
    void ChangeBlueColor(float _value){
        displayPlayer.color = new Color(displayPlayer.color.r, displayPlayer.color.g, _value);
        // Debug.Log("Blue: "+playerInfo.colorB);
        playerInfo.colorB = (int) (_value * 255);
    }
}