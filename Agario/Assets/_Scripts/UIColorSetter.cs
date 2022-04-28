using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIColorSetter : MonoBehaviour{
    [SerializeField] [CanBeNull] UIColor imageColor;
    [SerializeField] [CanBeNull] UIColor textColor;
    [SerializeField] bool setSpriteColor;
    [SerializeField] bool setImageColor;
    [SerializeField] bool setTextColor;
    [Tooltip("Press to instantly update Color")][SerializeField] bool updateColor;
    Image image;
    SpriteRenderer spriteRenderer;
    TextMeshProUGUI textMeshProUGUI;

    void OnValidate(){
        if(updateColor){
            if (setSpriteColor){
                if (imageColor == null){
                    Debug.Log($"No {imageColor} set");
                }
                else{
                    spriteRenderer = GetComponent<SpriteRenderer>();
                    spriteRenderer.color = imageColor.color;
                }
            }
            
            if (setImageColor){
                if (imageColor == null){
                    Debug.Log($"No {imageColor} set");
                }
                else{
                    image = GetComponent<Image>();
                    image.color = imageColor.color;
                }
                                    
            }
                        
            if (setTextColor){
                if (textColor == null){
                    Debug.Log($"No {textColor} set");
                }
                else{
                    textMeshProUGUI = GetComponent<TextMeshProUGUI>();
                    textMeshProUGUI.color = textColor.color;
                }
                                    
            }

            updateColor = false;
        }
        
    }
}