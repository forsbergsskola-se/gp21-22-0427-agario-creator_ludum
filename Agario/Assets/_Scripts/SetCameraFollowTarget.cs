using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Network;
using UnityEngine;

public class SetCameraFollowTarget : MonoBehaviour{
   CinemachineVirtualCamera cinemachineCamera;


   void Awake(){
      cinemachineCamera = GetComponent<CinemachineVirtualCamera>();
   }

   void Start(){
      var player = FindObjectOfType<PersonalClient>().transform;
      cinemachineCamera.LookAt = player;
      cinemachineCamera.Follow = player;
   }
}
