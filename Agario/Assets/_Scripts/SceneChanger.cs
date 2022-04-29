using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneChanger : MonoBehaviour{
    [SerializeField] SceneAsset menuScene;
    [SerializeField] SceneAsset loadingScene;
    [SerializeField] SceneAsset gameScene;

    void Awake(){
        DontDestroyOnLoad(this);
    }

    public void LoadMenuScene(){
        SceneManager.LoadScene(menuScene.name);
    }
    public void LoadLoadingScreen(){
        SceneManager.LoadScene(loadingScene.name);
        //SceneManager.MergeScenes( SceneManager.GetSceneByName(loadingScene.name), SceneManager.GetSceneByBuildIndex(2));
    }

    public void LoadGameScene(){
        SceneManager.LoadScene(gameScene.name);
    }


}
