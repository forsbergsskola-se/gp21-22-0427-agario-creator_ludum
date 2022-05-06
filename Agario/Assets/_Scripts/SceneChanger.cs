using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneChanger : MonoBehaviour{
    [SerializeField] UnityEventSO playerReadyEventSo;
    [SerializeField] SceneAsset menuScene;
    [SerializeField] SceneAsset loadingScene;
    [SerializeField] SceneAsset gameScene;

    void Awake(){
        DontDestroyOnLoad(gameObject);
    }

    void Start(){
       // playerReadyEventSo.unityEventSo.AddListener(LoadGameScene);
    }

    public void LoadMenuScene(){
        SceneManager.LoadScene(menuScene.name,LoadSceneMode.Additive);
    }
    public void LoadLoadingScreen(){
        SceneManager.LoadScene(loadingScene.name);
    }

    public void LoadGameScene(){
        Debug.Log("Loading gameScene... ");
        SceneManager.LoadScene(gameScene.name,LoadSceneMode.Additive);
        Debug.Log("Finished loading gameScene. ");
    }

    public async Task LoadGameSceneAsync(){
        Debug.Log("Loading gameScene... ");
       SceneManager.LoadScene(gameScene.name,LoadSceneMode.Single);
        Debug.Log("Finished loading gameScene. ");
    }


}
