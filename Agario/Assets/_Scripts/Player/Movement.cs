using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Movement : MonoBehaviour{
   [SerializeField] PlayerSO playerSo;
   [SerializeField] Vector2SO mapSizeVector2;
   [SerializeField] UnityEventSO playerReadyEventSo;
   float speed;
   Camera camera;
   Vector2 mousePosition;

   bool enabled;

   void Awake(){
      
   }

   void Start(){
      speed = playerSo.baseMovementSpeed;
     // playerReadyEventSo.unityEventSo.AddListener(SetScriptActive);
   }

   void Update(){

      if (!enabled){
         return;
      }
      
      EnsureStayingOnMap();
     
      if (Input.GetMouseButton(0)){
         Vector3 mousePosition = camera.ScreenToWorldPoint(Input.mousePosition);
         
         transform.position = Vector2.MoveTowards(transform.position,mousePosition,CalculateSpeed()*Time.deltaTime);
      }
      
   }

   float CalculateSpeed(){
      return speed;
   }

   void EnsureStayingOnMap(){
      var position = transform.position;
      var positionX = position.x;
      var positionY = position.y;
      
     
      var boundaryX = mapSizeVector2.vector2.x/2;
      var boundaryY = mapSizeVector2.vector2.y/2;
      if (positionX >= boundaryX){
         positionX -= 5f;
      }
      else if (positionX <= -boundaryX){
         positionX += 5f;
      }
      else if (positionY >= boundaryY){
         positionY -= 5f;
      }
      else if (positionY <= -boundaryY){
         positionY += 5f;
      }

      transform.position = new Vector2(positionX, positionY);
   }

   void SetScriptActive(){
      Debug.Log("Enabling Movement");
      camera = Camera.main;
      enabled = true;
   }
   
}
