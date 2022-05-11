using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Movement : MonoBehaviour{
    [SerializeField] Vector2SO mapSizeVector2;
    [SerializeField] BoolSO gameSceneIsActiveBoolSo;
    
    Player player;
    float speed;
    Camera camera;
    Vector2 mousePosition;

    bool enabled;

    void Start(){
        player = GetComponent<Player>();
    }

    void Update(){
        if (!gameSceneIsActiveBoolSo.boolSo){
            return;
        }

        if (camera == null){
            camera =  GameObject.FindWithTag("MainCamera")?.GetComponent<Camera>();
            speed = player.playerInfo.movementSpeed;
        }

        EnsureStayingOnMap();

        if (Input.GetMouseButton(0)){
            Vector3 mousePosition = camera.ScreenToWorldPoint(Input.mousePosition);

            transform.position =
                Vector2.MoveTowards(transform.position, mousePosition, CalculateSpeed() * Time.deltaTime);
        }
    }

    float CalculateSpeed(){
        return speed;
    }

    void EnsureStayingOnMap(){
        var position = transform.position;
        var positionX = position.x;
        var positionY = position.y;


        var boundaryX = mapSizeVector2.vector2.x / 2;
        var boundaryY = mapSizeVector2.vector2.y / 2;
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