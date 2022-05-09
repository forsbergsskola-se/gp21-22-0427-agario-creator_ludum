using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeChanger : MonoBehaviour{
    [SerializeField] AudioSource audioSource;
    Slider slider;

    void Awake(){
        slider = GetComponent<Slider>();
    }

    void Start(){
        slider.onValueChanged.AddListener(SetVolume);
    }

    void SetVolume(float volume){
        audioSource.volume = volume;
    }
}
