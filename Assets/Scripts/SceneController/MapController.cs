using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapController : MonoBehaviour {

    [Header("SceneLoad")]
    public int GameSceneIndex;

    private MapPage page;

    // Use this for initialization
    void Start()
    {
        page = new MapPage(App.Instance, this);
    }

    // 초기화
    public void OnInitialize()
    {
        PoolManager.Instance.Initiallize();
    }

    public void OnOpen()
    {
        
    }

    public void OnClose()
    {
        Debug.Log("Map Close");
    }

    public void PlayGame(int levelID)
    {
        if (GameSceneIndex < 0 || SceneManager.sceneCountInBuildSettings <= GameSceneIndex)
        {
            Debug.LogError("Scene index is out of range! " + GameSceneIndex);
            return;
        }

        SceneLoadManager.LoadScene(GameSceneIndex);
    }

}
