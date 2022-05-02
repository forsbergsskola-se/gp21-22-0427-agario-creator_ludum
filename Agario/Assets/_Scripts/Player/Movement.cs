using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Movement : MonoBehaviour{
   [SerializeField] PlayerSO playerSo;
   [SerializeField] GameObject map;
   float speed;
   Camera camera;

   Vector2 mousePosition;

   void Awake(){
      camera = Camera.main;
   }

   void Start(){
      speed = playerSo.baseMovementSpeed;
   }

   void Update(){

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
      
      var mapScale = map.transform.localScale;
      var boundaryX = mapScale.x/2;
      var boundaryY = mapScale.y/2;
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
}
