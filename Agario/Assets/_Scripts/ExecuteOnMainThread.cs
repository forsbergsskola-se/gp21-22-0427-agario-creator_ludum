using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExecuteOnMainThread : MonoBehaviour{
    
    static ExecuteOnMainThread instance = null;  
    public static ExecuteOnMainThread Instance {  
        get {  
            if (instance == null) {  
                lock(instance) {  
                    if (instance == null) {  
                        instance = new ExecuteOnMainThread();  
                    }  
                }  
            }  
            return instance;  
        }  
    } 
    
    Queue<Action> actions;

    void Awake(){
        actions = new Queue<Action>();
    }

    public void Execute(Action action){
        actions.Enqueue(action);
    }
    
    void Update(){
        while(actions.Count>0){
            actions.Dequeue().Invoke();
        }
    }
}


